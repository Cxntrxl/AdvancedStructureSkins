using AdvancedStructureSkins.Shared.SDK;
using UnityEngine;

namespace AdvancedStructureSkins.API;

[Serializable]
public class AssTextureSet
{
    public string name;
    public Texture previewTexture;
    public byte targets;
    public byte enabledForStructure;
    public bool initialized = false;
    public bool isDefaultTextureSet = false;

    private readonly Dictionary<TextureType, List<Texture>> _textures =
        new();

    private static readonly System.Random Random = new();

    public AssTextureSet(string name, byte targets, bool isDefault)
    {
        this.name = name;
        this.targets = targets;
        enabledForStructure = (byte)StructureFlags.None;
        isDefaultTextureSet = isDefault;

        foreach (var type in Enum.GetValues(typeof(TextureType)).Cast<TextureType>())
        {
            _textures[type] = new List<Texture>();
        }
    }
    
    public AssTextureSet(
        string folderPath,
        byte targets)
    {
        name = GetSkinName(folderPath);
        this.targets = targets;
        enabledForStructure = (byte)StructureFlags.None;

        Load(folderPath);
        initialized = true;
    }

    public AssTextureSet(TextureSetManifest manifest)
    {
        name = manifest.name;
        targets = (byte)manifest.targets;
        enabledForStructure = (byte)StructureFlags.None;
        previewTexture = manifest.previewTexture;
        _textures = LoadFromManifest(manifest);
    }

    private void Load(string folderPath)
    {
        foreach (TextureType type in Enum.GetValues(typeof(TextureType)))
        {
            _textures[type] = LoadTextureGroup(folderPath, type);
        }
    }

    public static bool DirectoryContainsTextures(string path)
    {
        foreach (TextureType type in Enum.GetValues(typeof(TextureType)))
        {
            string single =
                Path.Combine(path, type + ".png");

            if (File.Exists(single))
                return true;

            string multi =
                Path.Combine(path, type.ToString());

            if (Directory.Exists(multi))
                return true;
        }

        return false;
    }

    private static bool DirectoryContainsTypedFolders(string path)
    {
        return (from TextureType type in Enum.GetValues(typeof(TextureType)) select Directory.Exists(Path.Combine(path, type.ToString()))).FirstOrDefault();
    }
    
    private static bool IsSingleSkinSingleTexture(string path)
    {
        return Enum.GetValues(typeof(TextureType)).Cast<TextureType>().Any(type => File.Exists(Path.Combine(path, type + ".png")));
    }

    private static bool IsMultiSkinSingleTexture(string path)
    {
        return !DirectoryContainsTypedFolders(path) && 
               (from dir in Directory.GetDirectories(path) from TextureType type in Enum.GetValues(typeof(TextureType)) where dir.Contains(type.ToString()) select dir).Any();
    }

    private static bool IsSingleSkinMultiTexture(string path)
    {
        return Enum.GetValues(typeof(TextureType)).Cast<TextureType>().Any(type => Directory.Exists(Path.Combine(path, type.ToString())));
    }

    private static bool IsMultiSkinMultiTexture(string path)
    {
        return !DirectoryContainsTypedFolders(path) && Directory.GetDirectories(path).Any(IsSingleSkinMultiTexture);
    }

    public static bool IsSingleSkin(string path)
    {
        return IsSingleSkinSingleTexture(path) || IsSingleSkinMultiTexture(path);
    }

    public static bool IsMultiSkin(string path)
    {
        return IsMultiSkinSingleTexture(path) || IsMultiSkinMultiTexture(path);
    }
    
    private List<Texture> LoadTextureGroup(
        string folderPath,
        TextureType type)
    {
        List<Texture> result = new();
        
        // Main.png
        // Normal.png

        if (IsSingleSkinSingleTexture(folderPath))
        {
            Texture2D tex = LoadSingleTexture(folderPath, type);
            
            if (tex != null)
                result.Add(tex);

            return result;
        }
        
        // Main
        // Tex.png
        // Tex2.png
        // Normal
        // Tex.png
        // Tex2.png

        if (IsSingleSkinMultiTexture(folderPath))
        {
            result.AddRange(LoadMultiTexture(folderPath, type));
            return result;
        }
        
        ASS.Error("Invalid UserData/Skins/ folder structure detected. Please contact @cxntrxl on Discord if you need help.");
        
        return result;
    }

    private static Texture2D LoadSingleTexture(string folderPath, TextureType type)
    {
        string typeName = type.ToString();
        string singlePath = Path.Combine(folderPath, typeName + ".png");
        Texture2D tex = LoadTexture(singlePath, type);

        return tex;
    }

    private static Texture2D[] LoadMultiTexture(string folderPath, TextureType type)
    {
        List<Texture2D> result = new();
        string typeName = type.ToString();
        string multiPath = Path.Combine(folderPath, typeName);

        foreach (string file in Directory.GetFiles(multiPath))
        {
            if (!file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) continue;

            Texture2D tex = LoadTexture(file, type);
                
            if (tex != null) 
                result.Add(tex);
        }
        
        return result.ToArray();
    }

    private static Texture2D LoadTexture(
        string path,
        TextureType type)
    {
        try
        {
            if (!File.Exists(path))
            {
                ASS.LogVerbose("Could not find texture file at " + path);
                return null;
            }
            
            byte[] bytes = File.ReadAllBytes(path);

            Texture2D tex = new Texture2D(2, 2);

            tex.LoadImage(bytes);

            tex.hideFlags = HideFlags.HideAndDontSave;

            // Normal maps should be linear
            if (type == TextureType.Normal)
            {
                Texture2D linear = new Texture2D(
                    tex.width,
                    tex.height,
                    tex.format,
                    true,
                    true
                );

                Color[] srgb = tex.GetPixels();

                Color[] converted =
                    new Color[srgb.Length];

                for (int i = 0; i < srgb.Length; i++)
                {
                    converted[i] = srgb[i].linear;
                }

                linear.SetPixels(converted);
                linear.Apply();

                linear.hideFlags =
                    HideFlags.HideAndDontSave;

                return linear;
            }

            return tex;
        }
        catch (Exception ex)
        {
            ASS.Error(
                $"Failed loading texture at {path}\n{ex}"
            );

            return null;
        }
    }

    public bool HasTexture(TextureType type)
    {
        return _textures.ContainsKey(type)
               && _textures[type] != null
               && _textures[type].Count > 0;
    }

    public Texture GetRandomTexture(TextureType type)
    {
        if (!HasTexture(type))
            return null;

        List<Texture> list = _textures[type];

        return list[Random.Next(list.Count)];
    }

    private static string GetSkinName(string path)
    {
        HashSet<string> textureFolders =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "Main",
            "Normal",
            "Mat",
            "Ground"
        };
        
        path = Path.GetFullPath(path);

        DirectoryInfo dir;
        
        if (File.Exists(path))
            dir = new FileInfo(path).Directory!;
        else
            dir = new DirectoryInfo(path);
        
        if (textureFolders.Contains(dir.Name))
        {
            dir = dir.Parent!;
        }
        
        return dir.Name;
    }

    public void AddExistingTexture(TextureType type, Texture texture)
    {
        _textures[type] ??= new List<Texture>();
        _textures[type].Add(texture);
    }

    public Dictionary<TextureType, List<Texture>> LoadFromManifest(TextureSetManifest manifest)
    {
        return Enum.GetValues(typeof(TextureType))
            .Cast<TextureType>()
            .ToDictionary(
                type => type,
                type => manifest.textures.Where(entry => entry.type == type).SelectMany(entry => entry.textures).ToList()
            );
    }
}