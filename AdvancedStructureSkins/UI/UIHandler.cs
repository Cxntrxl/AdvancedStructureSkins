using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.MoveSystem;
using MelonLoader;
using RumbleModdingAPI.RMAPI;
using UIFramework;
using UIFramework.UiExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedStructureSkins.UI;

public enum PreferenceType
{
    None,
    Global,
    Shader,
    Texture,
    Mesh
}

public class UIHandler
{
    public static Dictionary<string, float> scrollPoints = new Dictionary<string, float>();
    
    private static MelonPreferences_Category _settings;
    private static MelonPreferences_Category _shaders;
    private static MelonPreferences_Category _textures;

    public static MelonPreferences_Entry<bool> CompMode;
    public static MelonPreferences_Entry<bool> DebugMode;
    public static MelonPreferences_Entry<string> ShaderSettings;
    public static MelonPreferences_Entry<string> TextureSettings;

    private static AssetBundle PrefabBundle;
    public static GameObject ShaderUIPrefab;
    public static GameObject TextureUIPrefab;
    public static GameObject SkinSelectorPrefab;
    public static GameObject SkinItemPrefab;

    public static bool Initialized;

    public static void Init()
    {
        LoadUIPrefabs();
        RegisterUIModels();
        SyncUIWithAssAPI();

        ASS.LogVerbose("UIFramework Setup Initialized");
        Initialized = true;
    }

    private static void LoadUIPrefabs()
    {
        PrefabBundle = AssetBundles.LoadAssetBundleFromStream(ASS.Instance, "AdvancedStructureSkins.Resources.UIFramework");
        ShaderUIPrefab = new GameObject("ShaderUI") { hideFlags = HideFlags.DontUnloadUnusedAsset };
        ShaderUIPrefab.AddComponent<RectTransform>().localPosition = new(5, -7, 0);
        ShaderUIPrefab.AddComponent<VerticalLayoutGroup>().spacing = 5f;
        ShaderUIPrefab.AddComponent<ShaderSelector>();
        GameObject.DontDestroyOnLoad(ShaderUIPrefab);
        
        TextureUIPrefab = new GameObject("TextureUI") { hideFlags = HideFlags.DontUnloadUnusedAsset };
        TextureUIPrefab.AddComponent<RectTransform>().localPosition = new(5, -7, 0);
        TextureUIPrefab.AddComponent<VerticalLayoutGroup>().spacing = 5f;
        TextureUIPrefab.AddComponent<TextureSelector>();
        GameObject.DontDestroyOnLoad(TextureUIPrefab);
        
        SkinSelectorPrefab = PrefabBundle.LoadAsset<GameObject>("skinselector");
        SkinSelectorPrefab.hideFlags = HideFlags.HideAndDontSave;
        ASS.LogVerbose("ShaderSelectorPrefab Loaded. Is Null?: " + (SkinSelectorPrefab == null));
        foreach (var c in SkinSelectorPrefab.GetComponentsInChildren<Component>(true)) if (c == null) ASS.ErrorVerbose("Missing component in prefab!!!");
        
        SkinItemPrefab = PrefabBundle.LoadAsset<GameObject>("skinitem");
        SkinItemPrefab.hideFlags = HideFlags.HideAndDontSave;
        ASS.LogVerbose("SkinItemPrefab Loaded. Is Null?: " + (SkinItemPrefab == null));
        foreach (var c in SkinItemPrefab.GetComponentsInChildren<Component>(true)) if (c == null) ASS.ErrorVerbose("Missing component in prefab!!!");
    }
    
    private static void RegisterUIModels()
    {
        _settings = MelonPreferences.CreateCategory("ass_settings", "Settings");
        _shaders = MelonPreferences.CreateCategory("ass_shaders", "Shaders");
        _textures = MelonPreferences.CreateCategory("ass_textures", "Textures");

        CompMode = _settings.CreateEntry("compMode", false, "Competitive Mode", "Disables shaders which provide a competitive advantage through enhanced feedback systems.");
        DebugMode = _settings.CreateEntry("debugMode", false, "Debug Mode", "Enables verbose debug logs and extra debugging tools.");
        ShaderSettings = _shaders.CreateEntry("shaders", "", "shaders", "", false, false, new CustomViewProvider { EntryViewPrefab = ShaderUIPrefab });
        TextureSettings = _textures.CreateEntry("textures", "", "textures", "", false, false, new CustomViewProvider { EntryViewPrefab = TextureUIPrefab });
        
        UIFramework.UI.Register(ASS.Instance, _settings, _shaders, _textures).OnModSaved += ModSaved;
    }

    private static void ModSaved()
    {
        SyncUIWithAssAPI();
        
        foreach (var s in UnityEngine.Object.FindObjectsOfType<Structure>()) SkinHandler.ApplySkinTo(s);
        foreach(var a in UnityEngine.Object.FindObjectsOfType<AdvancedSkin>()) a.debugger?.SetEnabled(DebugMode.Value);
    }

    private static void SyncUIWithAssAPI()
    {
        string settings = AssAPI.SyncUI(BuildSerializedSettings());
        ShaderSettings.Value = ExtractSerializedSettings(settings, PreferenceType.Shader);
        TextureSettings.Value = ExtractSerializedSettings(settings, PreferenceType.Texture);
    }

    private static string ExtractSerializedSettings(string input, PreferenceType type)
    {
        string[] lines = input.Split('\n');

        List<string> result = new();

        PreferenceType currentSection = PreferenceType.None;

        foreach (string raw in lines)
        {
            string line = raw.TrimEnd('\r');
            
            switch (line)
            {
                case "[Shader Settings]":
                case "[Shaders]":
                {
                    currentSection = PreferenceType.Shader;

                    if (type == PreferenceType.Shader)
                        result.Add(line);

                    continue;
                }
                
                case "[Textures]":
                {
                    currentSection = PreferenceType.Texture;

                    if (type == PreferenceType.Texture)
                        result.Add(line);

                    continue;
                }
            }
            
            if (type == currentSection) result.Add(line);
        }

        return string.Join("\n", result);
    }

    private static string BuildSerializedSettings()
    {
        return ShaderSettings.Value + TextureSettings.Value /*+ MeshSettings.Value*/;
    }
}