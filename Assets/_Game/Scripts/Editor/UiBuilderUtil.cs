using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.EditorTools
{
    public static class UiBuilderUtil
    {
        public static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.localScale = Vector3.one;
            return rt;
        }

        public static Image AddImage(RectTransform rt, Sprite sprite,
            bool raycastTarget = false, Image.Type type = Image.Type.Simple)
        {
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = sprite;
            img.type = type;
            img.raycastTarget = raycastTarget;
            if (sprite == null) img.color = new Color(0f, 0f, 0f, 0.85f);
            return img;
        }

        public static TextMeshProUGUI AddText(RectTransform rt, string text,
            float fontSize, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.raycastTarget = false;
            TMP_FontAsset font = ResolveDefaultFont();
            if (font != null)
                tmp.font = font;
            return tmp;
        }

        public static TMP_FontAsset ResolveDefaultFont()
        {
            if (TMP_Settings.instance != null && TMP_Settings.defaultFontAsset != null)
                return TMP_Settings.defaultFontAsset;

            foreach (string guid in AssetDatabase.FindAssets("t:TMP_FontAsset"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                if (font != null) return font;
            }
            return null;
        }

        public static Button AddButton(RectTransform rt, Sprite sprite)
        {
            var img = rt.gameObject.AddComponent<Image>();
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
            img.raycastTarget = true;
            if (sprite == null) img.color = new Color(0.8f, 0.5f, 0.15f);

            var button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = img;
            return button;
        }

        public static void SetAnchors(RectTransform rt, Vector2 min, Vector2 max, Vector2 pivot)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.pivot = pivot;
        }

        public static void SetSizePos(RectTransform rt, Vector2 size, Vector2 anchoredPos)
        {
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;
        }

        public static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}

