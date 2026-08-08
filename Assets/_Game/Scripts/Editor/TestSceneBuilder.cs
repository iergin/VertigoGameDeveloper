using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vertigo.Data;
using Vertigo.Domain.Zones;
using Vertigo.Presentation.Game;
using Vertigo.Presentation.Views;

namespace Vertigo.EditorTools
{
    public static class TestSceneBuilder
    {
        private const string ScenePath = "Assets/_Game/Scenes/WheelOfFortune.unity";
        private const int ZoneCellCount = 6;

        [MenuItem("Vertigo/Build Test Scene")]
        public static void Build()
        {
            EnsureTmpEssentials();

            if (!IsTmpReady())
            {
                EditorUtility.DisplayDialog(
                    "TMP kaynakları hazırlanıyor",
                    "TextMeshPro temel kaynakları import ediliyor.\n\n" +
                    "Import bittikten sonra 'Vertigo > Build Test Scene'i tekrar çalıştır.\n\n" +
                    "(Import başlamadıysa: Window > TextMeshPro > Import TMP Essential Resources)",
                    "Tamam");
                return;
            }

            GameAssetBuilder.Result assets = GameAssetBuilder.BuildAll();

            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            Canvas canvas = CreateCanvas();
            CreateBackground(canvas.transform);

            ZoneBarView zoneBar = CreateZoneBar(canvas.transform);
            WheelView wheel = CreateWheel(canvas.transform, assets);
            Button spinButton = CreateSpinButton(wheel.transform);
            Button collectButton = CreateCollectButton(canvas.transform);
            WalletView wallet = CreateWalletPanel(canvas.transform, assets);
            ResultPopupView result = CreateResultPopup(canvas.transform);
            GameButtonsView buttons = CreateButtonsHub(canvas.transform, spinButton, collectButton);

            CreateEventSystem();
            CreateController(canvas.transform, assets, wheel, zoneBar, buttons, result, wallet);

            Directory.CreateDirectory("Assets/_Game/Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Vertigo] Test sahnesi kuruldu: " + ScenePath);
        }

        [MenuItem("Vertigo/Import TMP Essentials")]
        public static void EnsureTmpEssentials()
        {
            if (AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/TextMesh Pro/Resources/TMP Settings.asset") != null)
                return;

            string builtIn = EditorApplication.applicationContentsPath +
                "/Resources/PackageManager/BuiltInPackages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage";

            string path = File.Exists(builtIn) ? builtIn : FindEssentialsInPackageCache();
            if (path == null)
            {
                Debug.LogWarning("[Vertigo] TMP Essential Resources bulunamadı. " +
                    "Window > TextMeshPro > Import TMP Essential Resources'ı elle çalıştır.");
                return;
            }

            AssetDatabase.ImportPackage(path, false);
            AssetDatabase.Refresh();
            Debug.Log("[Vertigo] TMP Essential Resources import edildi.");
        }

        private static bool IsTmpReady()
        {
            return AssetDatabase.FindAssets("t:TMP_FontAsset").Length > 0;
        }

        private static string FindEssentialsInPackageCache()
        {
            foreach (string dir in Directory.GetDirectories("Library/PackageCache"))
            {
                if (!dir.Contains("com.unity.ugui")) continue;
                string candidate = Path.Combine(dir, "Package Resources", "TMP Essential Resources.unitypackage");
                if (File.Exists(candidate)) return candidate;
            }
            return null;
        }

        private static Canvas CreateCanvas()
        {
            var go = new GameObject("ui_canvas_root",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            return canvas;
        }

        private static void CreateBackground(Transform parent)
        {
            RectTransform rt = UiBuilderUtil.NewRect("ui_image_background", parent);
            UiBuilderUtil.Stretch(rt);
            Image img = UiBuilderUtil.AddImage(rt, null);
            img.color = new Color(0.32f, 0.33f, 0.34f);
        }

        private static ZoneBarView CreateZoneBar(Transform parent)
        {
            RectTransform panel = UiBuilderUtil.NewRect("ui_panel_zone_bar", parent);
            UiBuilderUtil.SetAnchors(panel, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
            UiBuilderUtil.SetSizePos(panel, new Vector2(1180f, 100f), new Vector2(0f, -16f));
            Image bg = UiBuilderUtil.AddImage(panel, EditorSpriteUtil.Load("ui_card_panel_zone_bg.png"), false, Image.Type.Sliced);
            bg.color = new Color(0.12f, 0.12f, 0.13f);

            RectTransform group = UiBuilderUtil.NewRect("ui_group_zone_cells", panel);
            UiBuilderUtil.Stretch(group);
            group.offsetMin = new Vector2(12f, 10f);
            group.offsetMax = new Vector2(-12f, -10f);
            var layout = group.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            var view = panel.gameObject.AddComponent<ZoneBarView>();
            var cells = new List<ZoneCellView>();
            for (int i = 0; i < ZoneCellCount; i++)
                cells.Add(CreateZoneCell(group, i));
            GameAssetBuilder.SetField(view, "_cells", cells);
            return view;
        }

        private static ZoneCellView CreateZoneCell(Transform parent, int index)
        {
            RectTransform rt = UiBuilderUtil.NewRect($"ui_image_zone_cell_{index}", parent);
            Image bg = UiBuilderUtil.AddImage(rt, EditorSpriteUtil.Load("ui_card_panel_zone_white.png"), false, Image.Type.Sliced);

            RectTransform numRt = UiBuilderUtil.NewRect("ui_text_zone_number_value", rt);
            UiBuilderUtil.Stretch(numRt);
            TextMeshProUGUI num = UiBuilderUtil.AddText(numRt, (index + 1).ToString(), 34f);
            num.fontStyle = FontStyles.Bold;

            var cell = rt.gameObject.AddComponent<ZoneCellView>();
            GameAssetBuilder.SetField(cell, "_background", bg);
            GameAssetBuilder.SetField(cell, "_numberText", num);
            return cell;
        }

        private static WheelView CreateWheel(Transform parent, GameAssetBuilder.Result assets)
        {
            RectTransform root = UiBuilderUtil.NewRect("ui_wheel_root", parent);
            UiBuilderUtil.SetAnchors(root, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UiBuilderUtil.SetSizePos(root, new Vector2(540f, 540f), new Vector2(330f, -30f));

            RectTransform rotor = UiBuilderUtil.NewRect("rotor", root);
            UiBuilderUtil.Stretch(rotor);

            RectTransform baseRt = UiBuilderUtil.NewRect("ui_image_wheel_base_value", rotor);
            UiBuilderUtil.Stretch(baseRt);
            Image baseImg = UiBuilderUtil.AddImage(baseRt, assets.Config.WheelFor(ZoneType.Normal).BaseSprite);

            RectTransform slices = UiBuilderUtil.NewRect("ui_group_slices", rotor);
            UiBuilderUtil.SetAnchors(slices, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UiBuilderUtil.SetSizePos(slices, Vector2.zero, Vector2.zero);

            var view = root.gameObject.AddComponent<WheelView>();
            GameAssetBuilder.SetField(view, "_rotor", rotor);
            GameAssetBuilder.SetField(view, "_baseImage", baseImg);
            GameAssetBuilder.SetField(view, "_sliceContainer", slices);
            GameAssetBuilder.SetField(view, "_sliceItemPrefab", assets.SlicePrefab);
            GameAssetBuilder.SetField(view, "_bombIcon", EditorSpriteUtil.Load("ui_card_icon_death.png"));
            GameAssetBuilder.SetField(view, "_sliceRadius", 155f);
            return view;
        }

        private static Button CreateSpinButton(Transform wheelRoot)
        {
            RectTransform rt = UiBuilderUtil.NewRect("ui_button_spin", wheelRoot);
            UiBuilderUtil.SetAnchors(rt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UiBuilderUtil.SetSizePos(rt, new Vector2(150f, 150f), Vector2.zero);
            Button button = UiBuilderUtil.AddButton(rt, EditorSpriteUtil.Load("ui_spin_generic_button.png"));

            RectTransform labelRt = UiBuilderUtil.NewRect("ui_text_spin_label", rt);
            UiBuilderUtil.Stretch(labelRt);
            TextMeshProUGUI label = UiBuilderUtil.AddText(labelRt, "SPIN", 36f);
            label.fontStyle = FontStyles.Bold;
            return button;
        }

        private static Button CreateCollectButton(Transform parent)
        {
            RectTransform rt = UiBuilderUtil.NewRect("ui_button_collect", parent);
            UiBuilderUtil.SetAnchors(rt, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            UiBuilderUtil.SetSizePos(rt, new Vector2(220f, 80f), new Vector2(180f, -140f));
            Button button = UiBuilderUtil.AddButton(rt, EditorSpriteUtil.Load("UI_button_orange_standard.png"));

            RectTransform labelRt = UiBuilderUtil.NewRect("ui_text_collect_label", rt);
            UiBuilderUtil.Stretch(labelRt);
            TextMeshProUGUI label = UiBuilderUtil.AddText(labelRt, "Collect", 30f);
            label.fontStyle = FontStyles.Bold;
            return button;
        }

        private static WalletView CreateWalletPanel(Transform parent, GameAssetBuilder.Result assets)
        {
            RectTransform panel = UiBuilderUtil.NewRect("ui_panel_wallet", parent);
            UiBuilderUtil.SetAnchors(panel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            UiBuilderUtil.SetSizePos(panel, new Vector2(300f, 640f), new Vector2(70f, -240f));
            Image bg = UiBuilderUtil.AddImage(panel, null);
            bg.color = new Color(0f, 0f, 0f, 0.85f);

            RectTransform content = UiBuilderUtil.NewRect("ui_group_wallet_content", panel);
            UiBuilderUtil.SetAnchors(content, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f));
            content.offsetMin = new Vector2(0f, 0f);
            content.offsetMax = new Vector2(0f, 0f);
            content.anchoredPosition = new Vector2(0f, 0f);
            var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.padding = new RectOffset(8, 8, 12, 12);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var view = panel.gameObject.AddComponent<WalletView>();
            GameAssetBuilder.SetField(view, "_content", content);
            GameAssetBuilder.SetField(view, "_entryPrefab", assets.WalletEntryPrefab);
            return view;
        }

        private static ResultPopupView CreateResultPopup(Transform parent)
        {
            RectTransform host = UiBuilderUtil.NewRect("ui_result", parent);
            UiBuilderUtil.Stretch(host);

            RectTransform overlay = UiBuilderUtil.NewRect("ui_panel_result_overlay", host);
            UiBuilderUtil.Stretch(overlay);
            Image dim = UiBuilderUtil.AddImage(overlay, null, raycastTarget: true);
            dim.color = new Color(0f, 0f, 0f, 0.7f);

            RectTransform box = UiBuilderUtil.NewRect("ui_panel_result_box", overlay);
            UiBuilderUtil.SetAnchors(box, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UiBuilderUtil.SetSizePos(box, new Vector2(640f, 420f), Vector2.zero);
            Image boxBg = UiBuilderUtil.AddImage(box, EditorSpriteUtil.Load("ui_card_panel_zone_bg.png"), true, Image.Type.Sliced);
            boxBg.color = new Color(0.15f, 0.15f, 0.17f);

            RectTransform titleRt = UiBuilderUtil.NewRect("ui_text_result_title_value", box);
            UiBuilderUtil.SetAnchors(titleRt, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f));
            UiBuilderUtil.SetSizePos(titleRt, new Vector2(-40f, 90f), new Vector2(0f, -60f));
            TextMeshProUGUI title = UiBuilderUtil.AddText(titleRt, "TITLE", 48f);
            title.fontStyle = FontStyles.Bold;

            RectTransform bodyRt = UiBuilderUtil.NewRect("ui_text_result_body_value", box);
            UiBuilderUtil.SetAnchors(bodyRt, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f));
            bodyRt.offsetMin = new Vector2(40f, 110f);
            bodyRt.offsetMax = new Vector2(-40f, -120f);
            UiBuilderUtil.AddText(bodyRt, "body", 30f);

            RectTransform btnRt = UiBuilderUtil.NewRect("ui_button_result_dismiss", box);
            UiBuilderUtil.SetAnchors(btnRt, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            UiBuilderUtil.SetSizePos(btnRt, new Vector2(240f, 80f), new Vector2(0f, 40f));
            Button dismiss = UiBuilderUtil.AddButton(btnRt, EditorSpriteUtil.Load("UI_button_orange_standard.png"));
            RectTransform btnLabel = UiBuilderUtil.NewRect("ui_text_dismiss_label", btnRt);
            UiBuilderUtil.Stretch(btnLabel);
            UiBuilderUtil.AddText(btnLabel, "Devam", 30f).fontStyle = FontStyles.Bold;

            var view = host.gameObject.AddComponent<ResultPopupView>();
            GameAssetBuilder.SetField(view, "_root", overlay.gameObject);
            GameAssetBuilder.SetField(view, "_titleText", title);
            GameAssetBuilder.SetField(view, "_bodyText", GetText(bodyRt));
            GameAssetBuilder.SetField(view, "_dismissButton", dismiss);
            return view;
        }

        private static TextMeshProUGUI GetText(RectTransform rt) => rt.GetComponent<TextMeshProUGUI>();

        private static GameButtonsView CreateButtonsHub(Transform parent, Button spin, Button collect)
        {
            var go = new GameObject("ui_game_buttons", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var view = go.AddComponent<GameButtonsView>();
            GameAssetBuilder.SetField(view, "_spinButton", spin);
            GameAssetBuilder.SetField(view, "_leaveButton", collect);
            return view;
        }

        private static void CreateEventSystem()
        {
            var go = new GameObject("EventSystem", typeof(EventSystem));

            go.AddComponent<InputSystemUIInputModule>();
        }

        private static void CreateController(
            Transform parent, GameAssetBuilder.Result assets,
            WheelView wheel, ZoneBarView zoneBar, GameButtonsView buttons,
            ResultPopupView result, WalletView wallet)
        {
            var go = new GameObject("GameController");
            var controller = go.AddComponent<GameController>();
            GameAssetBuilder.SetField(controller, "_config", assets.Config);
            GameAssetBuilder.SetField(controller, "_catalog", assets.Catalog);
            GameAssetBuilder.SetField(controller, "_wheelView", wheel);
            GameAssetBuilder.SetField(controller, "_zoneBarView", zoneBar);
            GameAssetBuilder.SetField(controller, "_buttonsView", buttons);
            GameAssetBuilder.SetField(controller, "_resultView", result);
            GameAssetBuilder.SetField(controller, "_walletView", wallet);
        }

        private static void AddSceneToBuildSettings(string path)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (scenes.Exists(s => s.path == path)) return;
            scenes.Insert(0, new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}

