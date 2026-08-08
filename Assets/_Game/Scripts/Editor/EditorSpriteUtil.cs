using UnityEditor;
using UnityEngine;

namespace Vertigo.EditorTools
{
    public static class EditorSpriteUtil
    {
        private const string ContentDir = "Assets/demo_content/";

        public static Sprite Load(string fileName)
        {
            string path = ContentDir + fileName;

            Sprite sprite = LoadFirstSprite(path);
            if (sprite != null) return sprite;

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
                sprite = LoadFirstSprite(path);
            }

            if (sprite == null)
                Debug.LogWarning($"[Vertigo] Sprite yüklenemedi: {path}");

            return sprite;
        }

        private static Sprite LoadFirstSprite(string path)
        {
            Object[] all = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (Object obj in all)
                if (obj is Sprite sprite)
                    return sprite;
            return null;
        }
    }
}

