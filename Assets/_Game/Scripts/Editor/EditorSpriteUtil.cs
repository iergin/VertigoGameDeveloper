using System.IO;
using UnityEditor;
using UnityEngine;

namespace Vertigo.EditorTools
{
    public static class EditorSpriteUtil
    {
        private const string ContentDir = "Assets/demo_content/";

        public static Sprite Load(string fileName)
        {
            Sprite sprite = LoadSpriteAtPath(ContentDir + fileName);
            if (sprite != null) return sprite;

            string root = Path.Combine(Application.dataPath, "demo_content");
            if (Directory.Exists(root))
            {
                string[] matches = Directory.GetFiles(root, fileName, SearchOption.AllDirectories);
                if (matches.Length > 0)
                {
                    string assetPath = ToAssetPath(matches[0]);
                    sprite = LoadSpriteAtPath(assetPath);
                    if (sprite != null) return sprite;
                }
            }

            Debug.LogWarning($"[Vertigo] Sprite yüklenemedi: {fileName}");
            return null;
        }

        private static Sprite LoadSpriteAtPath(string assetPath)
        {
            Sprite sprite = LoadFirstSprite(assetPath);
            if (sprite != null) return sprite;

            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
                sprite = LoadFirstSprite(assetPath);
            }
            return sprite;
        }

        private static Sprite LoadFirstSprite(string assetPath)
        {
            Object[] all = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (Object obj in all)
                if (obj is Sprite sprite)
                    return sprite;
            return null;
        }

        private static string ToAssetPath(string absolutePath)
        {
            absolutePath = absolutePath.Replace("\\", "/");
            string assetsRoot = Application.dataPath.Replace("\\", "/");
            return "Assets" + absolutePath.Substring(assetsRoot.Length);
        }
    }
}
