using UnityEngine;
using UnityEditor;
using System.IO;

public class HeartImporter : MonoBehaviour
{
    [MenuItem("Tools/Import Heart Sprite")]
    static void ImportHeart()
    {
        string srcPath = "Assets/8-bit-color-clip-art-pixel-art-thumbnail.jpg";
        string dstPath = "Assets/Sprites/HeartLife.png";

        // Load the texture
        TextureImporter ti = AssetImporter.GetAtPath(srcPath) as TextureImporter;
        if (ti == null) { Debug.LogError("Source not found: " + srcPath); return; }

        // Temporarily make it readable
        ti.isReadable = true;
        ti.textureType = TextureImporterType.Default;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        AssetDatabase.ImportAsset(srcPath, ImportAssetOptions.ForceUpdate);

        Texture2D src = AssetDatabase.LoadAssetAtPath<Texture2D>(srcPath);
        if (src == null) { Debug.LogError("Failed to load texture"); return; }

        // Create new RGBA texture
        Texture2D dst = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false);
        Color[] pixels = src.GetPixels();

        // Remove white/near-white background
        for (int i = 0; i < pixels.Length; i++)
        {
            Color c = pixels[i];
            float brightness = (c.r + c.g + c.b) / 3f;
            // If pixel is very bright (white background), make transparent
            if (brightness > 0.92f && c.r > 0.85f && c.g > 0.85f && c.b > 0.85f)
                pixels[i] = new Color(c.r, c.g, c.b, 0f);
            else if (brightness > 0.8f)
                // Semi-transparent for near-white edges (anti-alias)
                pixels[i] = new Color(c.r, c.g, c.b, 1f - (brightness - 0.8f) * 5f);
        }

        dst.SetPixels(pixels);
        dst.Apply();

        // Save as PNG
        byte[] bytes = dst.EncodeToPNG();
        if (!Directory.Exists("Assets/Sprites")) Directory.CreateDirectory("Assets/Sprites");
        File.WriteAllBytes(dstPath, bytes);
        AssetDatabase.Refresh();

        // Configure as sprite
        TextureImporter dstTi = AssetImporter.GetAtPath(dstPath) as TextureImporter;
        if (dstTi != null)
        {
            dstTi.textureType = TextureImporterType.Sprite;
            dstTi.spriteImportMode = SpriteImportMode.Single;
            dstTi.alphaIsTransparency = true;
            dstTi.mipmapEnabled = false;
            dstTi.filterMode = FilterMode.Point;
            AssetDatabase.ImportAsset(dstPath, ImportAssetOptions.ForceUpdate);
        }

        // Restore source
        ti.isReadable = false;
        ti.textureType = TextureImporterType.Sprite;
        AssetDatabase.ImportAsset(srcPath, ImportAssetOptions.ForceUpdate);

        Debug.Log("Heart sprite imported to: " + dstPath);
    }
}
