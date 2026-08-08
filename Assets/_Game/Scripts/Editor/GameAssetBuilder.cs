using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.Data;
using Vertigo.Presentation.Views;

namespace Vertigo.EditorTools
{
    public static class GameAssetBuilder
    {
        public sealed class Result
        {
            public GameConfigSO Config;
            public RewardCatalogSO Catalog;
            public WheelSliceView SlicePrefab;
            public WalletEntryView WalletEntryPrefab;
        }

        private const string SoDir = "Assets/_Game/ScriptableObjects";
        private const string PrefabDir = "Assets/_Game/Prefabs";

        public static Result BuildAll()
        {
            EnsureFolder("Assets/_Game", "ScriptableObjects");
            EnsureFolder(SoDir, "Rewards");
            EnsureFolder(SoDir, "Wheels");
            EnsureFolder("Assets/_Game", "Prefabs");

            Dictionary<string, RewardDefinitionSO> rewards = CreateRewards();
            RewardCatalogSO catalog = CreateCatalog(rewards);

            WheelConfigSO bronze = CreateBronzeWheel(rewards);
            WheelConfigSO silver = CreateSilverWheel(rewards);
            WheelConfigSO golden = CreateGoldenWheel(rewards);
            GameConfigSO config = CreateGameConfig(bronze, silver, golden);

            WheelSliceView slicePrefab = CreateSlicePrefab();
            WalletEntryView entryPrefab = CreateWalletEntryPrefab();

            AssetDatabase.SaveAssets();

            return new Result
            {
                Config = config,
                Catalog = catalog,
                SlicePrefab = slicePrefab,
                WalletEntryPrefab = entryPrefab,
            };
        }

        private static Dictionary<string, RewardDefinitionSO> CreateRewards()
        {
            var defs = new (string id, string file, string name)[]
            {
                ("gold", "UI_icon_gold.png", "Gold"),
                ("cash", "UI_icon_cash.png", "Cash"),
                ("chest", "UI_icon_chest_gold_nolight.png", "Chest"),
                ("rifle", "UI_Icon_Renders_tier2_rifle.png", "Rifle"),
                ("pumpkin", "ui_icon_helmet_pumpkin.png", "Pumpkin"),
            };

            var result = new Dictionary<string, RewardDefinitionSO>();
            foreach (var d in defs)
            {
                var so = ScriptableObject.CreateInstance<RewardDefinitionSO>();
                SetField(so, "_id", d.id);
                SetField(so, "_displayName", d.name);
                SetField(so, "_icon", EditorSpriteUtil.Load(d.file));
                AssetDatabase.CreateAsset(so, $"{SoDir}/Rewards/reward_{d.id}.asset");
                result[d.id] = so;
            }
            return result;
        }

        private static RewardCatalogSO CreateCatalog(Dictionary<string, RewardDefinitionSO> rewards)
        {
            var catalog = ScriptableObject.CreateInstance<RewardCatalogSO>();
            SetField(catalog, "_rewards", new List<RewardDefinitionSO>(rewards.Values));
            AssetDatabase.CreateAsset(catalog, $"{SoDir}/reward_catalog.asset");
            return catalog;
        }

        private static object NewSlice(bool isBomb, RewardDefinitionSO reward, int baseAmount, float weight)
        {
            var slice = System.Activator.CreateInstance(typeof(SliceConfig));
            SetField(slice, "_isBomb", isBomb);
            SetField(slice, "_reward", reward);
            SetField(slice, "_baseAmount", baseAmount);
            SetField(slice, "_weight", weight);
            return slice;
        }

        private static WheelConfigSO CreateWheel(string id, string baseSpriteFile, IList<object> slices)
        {
            var wheel = ScriptableObject.CreateInstance<WheelConfigSO>();
            SetField(wheel, "_baseSprite", EditorSpriteUtil.Load(baseSpriteFile));

            var typedList = (System.Collections.IList)System.Activator.CreateInstance(
                typeof(List<>).MakeGenericType(typeof(SliceConfig)));
            foreach (object s in slices) typedList.Add(s);
            SetField(wheel, "_slices", typedList);

            AssetDatabase.CreateAsset(wheel, $"{SoDir}/Wheels/wheel_{id}.asset");
            return wheel;
        }

        private static WheelConfigSO CreateBronzeWheel(Dictionary<string, RewardDefinitionSO> r)
        {
            var slices = new List<object>
            {
                NewSlice(false, r["chest"], 1, 1f),
                NewSlice(false, r["gold"], 1, 1f),
                NewSlice(false, r["rifle"], 5, 1f),
                NewSlice(false, r["cash"], 100, 1f),
                NewSlice(false, r["gold"], 1, 1f),
                NewSlice(false, r["cash"], 100, 1f),
                NewSlice(false, r["pumpkin"], 1, 1f),
                NewSlice(true, null, 0, 1f),
            };
            return CreateWheel("bronze_normal", "ui_spin_bronze_base.png", slices);
        }

        private static WheelConfigSO CreateSilverWheel(Dictionary<string, RewardDefinitionSO> r)
        {
            var slices = new List<object>
            {
                NewSlice(false, r["chest"], 2, 1f),
                NewSlice(false, r["gold"], 2, 1f),
                NewSlice(false, r["rifle"], 10, 1f),
                NewSlice(false, r["cash"], 200, 1f),
                NewSlice(false, r["gold"], 2, 1f),
                NewSlice(false, r["cash"], 200, 1f),
                NewSlice(false, r["pumpkin"], 2, 1f),
                NewSlice(false, r["chest"], 3, 1f),
            };
            return CreateWheel("silver_safe", "ui_spin_silver_base.png", slices);
        }

        private static WheelConfigSO CreateGoldenWheel(Dictionary<string, RewardDefinitionSO> r)
        {
            var slices = new List<object>
            {
                NewSlice(false, r["chest"], 5, 1f),
                NewSlice(false, r["gold"], 5, 1f),
                NewSlice(false, r["rifle"], 25, 1f),
                NewSlice(false, r["cash"], 500, 1f),
                NewSlice(false, r["gold"], 10, 1f),
                NewSlice(false, r["cash"], 1000, 1f),
                NewSlice(false, r["pumpkin"], 5, 1f),
                NewSlice(false, r["chest"], 10, 1f),
            };
            return CreateWheel("golden_super", "ui_spin_golden_base.png", slices);
        }

        private static GameConfigSO CreateGameConfig(WheelConfigSO bronze, WheelConfigSO silver, WheelConfigSO golden)
        {
            var config = ScriptableObject.CreateInstance<GameConfigSO>();
            SetField(config, "_safeInterval", 5);
            SetField(config, "_superInterval", 30);
            SetField(config, "_rewardGrowthPerZone", 1.15f);
            SetField(config, "_normalWheel", bronze);
            SetField(config, "_safeWheel", silver);
            SetField(config, "_superWheel", golden);
            SetField(config, "_useFixedSeed", false);
            SetField(config, "_seed", 12345);
            AssetDatabase.CreateAsset(config, $"{SoDir}/game_config.asset");
            return config;
        }

        private static WheelSliceView CreateSlicePrefab()
        {
            RectTransform root = UiBuilderUtil.NewRect("WheelSliceItem", null);
            UiBuilderUtil.SetSizePos(root, new Vector2(120f, 120f), Vector2.zero);

            RectTransform iconRt = UiBuilderUtil.NewRect("ui_image_slice_icon", root);
            UiBuilderUtil.SetAnchors(iconRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UiBuilderUtil.SetSizePos(iconRt, new Vector2(96f, 96f), new Vector2(0f, 10f));
            Image icon = UiBuilderUtil.AddImage(iconRt, null);

            RectTransform amountRt = UiBuilderUtil.NewRect("ui_text_slice_amount_value", root);
            UiBuilderUtil.SetAnchors(amountRt, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            UiBuilderUtil.SetSizePos(amountRt, new Vector2(120f, 34f), new Vector2(0f, -2f));
            TextMeshProUGUI amount = UiBuilderUtil.AddText(amountRt, "x1", 26f);
            amount.fontStyle = FontStyles.Bold;

            var view = root.gameObject.AddComponent<WheelSliceView>();
            SetField(view, "_rectTransform", root);
            SetField(view, "_iconImage", icon);
            SetField(view, "_amountText", amount);

            string path = $"{PrefabDir}/WheelSliceItem.prefab";
            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root.gameObject, path);
            Object.DestroyImmediate(root.gameObject);
            return saved.GetComponent<WheelSliceView>();
        }

        private static WalletEntryView CreateWalletEntryPrefab()
        {
            RectTransform root = UiBuilderUtil.NewRect("WalletEntry", null);
            UiBuilderUtil.SetSizePos(root, new Vector2(300f, 72f), Vector2.zero);
            var layout = root.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = 72f;

            RectTransform iconRt = UiBuilderUtil.NewRect("ui_image_wallet_icon", root);
            UiBuilderUtil.SetAnchors(iconRt, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));
            UiBuilderUtil.SetSizePos(iconRt, new Vector2(60f, 60f), new Vector2(40f, 0f));
            Image icon = UiBuilderUtil.AddImage(iconRt, null);

            RectTransform amountRt = UiBuilderUtil.NewRect("ui_text_wallet_amount_value", root);
            UiBuilderUtil.SetAnchors(amountRt, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f));
            amountRt.offsetMin = new Vector2(90f, 0f);
            amountRt.offsetMax = new Vector2(-10f, 0f);
            TextMeshProUGUI amount = UiBuilderUtil.AddText(amountRt, "x0", 30f, TextAlignmentOptions.Left);

            var view = root.gameObject.AddComponent<WalletEntryView>();
            SetField(view, "_iconImage", icon);
            SetField(view, "_amountText", amount);

            string path = $"{PrefabDir}/WalletEntry.prefab";
            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root.gameObject, path);
            Object.DestroyImmediate(root.gameObject);
            return saved.GetComponent<WalletEntryView>();
        }

        public static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(
                fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                Debug.LogError($"[Vertigo] Alan bulunamadı: {target.GetType().Name}.{fieldName}");
                return;
            }
            field.SetValue(target, value);
        }

        private static void EnsureFolder(string parent, string child)
        {
            string full = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(full))
                AssetDatabase.CreateFolder(parent, child);
            Directory.CreateDirectory(full);
        }
    }
}

