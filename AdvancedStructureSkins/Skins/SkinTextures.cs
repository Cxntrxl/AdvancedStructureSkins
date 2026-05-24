using AdvancedStructureSkins;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = System.Random;

namespace AdvancedStructureSkins.Skins;

public class SkinTextures
{
    public string Name;
    private Texture2D[][] _textures = new Texture2D[4][];
    private static Random _random = new Random();
    
    private readonly string[] _texTypes =
    {
        "Normal",
        "Main",
        "Mat",
        "Grounded"
    };

    public SkinTextures(string folderPath)
    {
        Name = Path.GetFileName(folderPath);
        _textures = LoadTexturesFromPath(folderPath);
    }

    private Texture2D[][] LoadTexturesFromPath(string folderPath)
    {
        // folderPath is something like /RUMBLE/UserData/Skins/Disc/RumbleCoin/
        // folder structure is as follows:
        // RumbleCoin
        // |    Main
        // |    |   textures
        // |    Normal
        // |    |   textures
        // |    Mat
        // |    |   textures
        // |    Ground
        // |    |   textures
        
        // but could also have
        // RumbleCoin
        // | Main.png
        // | Normal.png
        // | Mat.png
        // | Ground.png
        
        // _textures[TextureType][TextureIndex]
        
        Texture2D[][] result = new Texture2D[4][];
        
        // Repeat for Main, Normal, Mat, Ground
        for (int type = 0; type < result.Length; type++)
        {
            // If Main.png exists in /RumbleCoin/
            string singlePath = Path.Combine(folderPath, _texTypes[type] + ".png");
            if (File.Exists(singlePath))
            {
                result[type] = new Texture2D[1];
                result[type][0] = LoadTexture(singlePath, type);
                continue;
            }

            // If /RumbleCoin/Main/ exists
            string multiPath = Path.Combine(folderPath, _texTypes[type]);
            if (Directory.Exists(multiPath))
            {
                string[] files = Directory.GetFiles(multiPath);
                result[type] = new Texture2D[files.Length];
                for (int textureIndex = 0; textureIndex < files.Length; textureIndex++)
                {
                    result[type][textureIndex] = LoadTexture(files[textureIndex], type);
                }
            }
        }
        
        return result;
    }

    private Texture2D LoadTexture(string path, int textureType)
    {
        if (!File.Exists(path)) return null;
        
        Texture2D result = new Texture2D(2, 2);
        byte[] bytes = File.ReadAllBytes(path);
        ImageConversion.LoadImage(result, bytes);
        result.hideFlags = HideFlags.HideAndDontSave;

        if (textureType == 0)
        {
            Texture2D convertedNormal = new Texture2D(result.width, result.height, result.format, true, true);

            Color[] srgb = result.GetPixels();
            Color[] linear = new Color[srgb.Length];

            for (int i = 0; i < srgb.Length; i++)
            {
                linear[i] = srgb[i].linear;
            }
            
            convertedNormal.SetPixels(linear);
            convertedNormal.hideFlags = HideFlags.HideAndDontSave;
            convertedNormal.Apply();

            if (convertedNormal != null) return convertedNormal;
        }
        
        return result;
    }

    public Texture2D GetTexture(int textureType)
    {
        if (_textures[textureType] == null)
        {
            ASS.Error("Requested SkinTexture type is null.");
            return null;
        }

        if (_textures[textureType].Length == 0)
        {
            ASS.Error("Requested SkinTexture type is empty.");
            return null;
        }
        
        int resultIndex = _random.Next(_textures[textureType].Length);

        if (_textures[textureType][resultIndex] == null)
        {
            ASS.Error("Requested SkinTexture image is null");
            return null;
        }
        
        return _textures[textureType][resultIndex];
    }

    public bool HasTextureFor(int textureType)
    {
        if (_textures[textureType] == null) return false;
        return _textures[textureType].Length > 0;
    }
}