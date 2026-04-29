using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.MoveSystem;
using MelonLoader;
using UIFramework;

namespace AdvancedStructureSkins.Settings;

public class UIHandler
{
    private const string UserData = "UserData/AdvancedStructureSkins/";
    private const string ConfigFile = "config.cfg";

    private static MelonPreferences_Category _shaders;
    private static MelonPreferences_Category _textures;
    private static MelonPreferences_Category _meshes;

    public static MelonPreferences_Entry<string> ShaderSettings;
    public static MelonPreferences_Entry<string> TextureSettings;
    public static MelonPreferences_Entry<string> MeshSettings;

    public static void Init(MelonBase mod)
    {
        if (!Directory.Exists(UserData))
            Directory.CreateDirectory(UserData);

        _shaders = MelonPreferences.CreateCategory("AdvancedStructureSkins", "Shaders");
        _textures = MelonPreferences.CreateCategory("AdvancedStructureSkins", "Textures");
        _meshes = MelonPreferences.CreateCategory("AdvancedStructureSkins", "Meshes");

        ShaderSettings = _shaders.CreateEntry("shaders", "[{\"default\":true}]", "shaders");
        TextureSettings = _textures.CreateEntry("textures", "[{\"default\":true}]", "textures");
        MeshSettings = _meshes.CreateEntry("meshes", "[{\"default\":true}]", "meshes");
        
        UI.Register(mod, _shaders, _textures, _meshes).OnModSaved += ModSaved;

        /*AssUI.ModName = ModInfo.ModName;
        AssUI.ModVersion = ModInfo.ModVersion;
        AssUI.SetFolder("AdvancedStructureSkins");

        AssUI.AddDescription("Keybinds", "",
            $"Press F5 to reload shaders on currently spawned structures. {Environment.NewLine}" +
            $"Press Left Alt + S to print cached structure skins data.{Environment.NewLine}" +
            $"Press and Hold F6 for 3s to clear cached data.",
            new Tags());

        AddSetting(
                "GlobalSkinEnabled",
                "Use Global Skin",
                true,
                0,
                "Toggles on/off the global skin, allowing you to either select one skin for all structures or one skin per structure."
            );

        AddSetting(
                "GlobalSkinPath",
                "Global Skin Path",
                "default",
                $"The path to the shader used by all structures if Global Skin is enabled. {Environment.NewLine}" +
                $"'myShader' maps to 'RUMBLE/UserData/Skins/myShader.bundle'.{Environment.NewLine}" +
                $"Enter 'default' to select the structure's default shader.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Don't forget to press Enter after typing your shader path to save the value.{Environment.NewLine}" +
                $"The text box should say 'Current Value: myShader' before you hit save."
            );

        /*AddSetting(
            "UseLegacyStructures",
            "Use Legacy Structures",
            false,
            0,
            "Uses old structure meshes instead of the new ones for compatibility with old textures."
            );#1#

        AddSetting(
                "DiscSkinPath",
                "Disc Skin Path",
                "default",
                $"The path to the shader used by disc. {Environment.NewLine}" +
                $"'Disc/myShader' maps to 'RUMBLE/UserData/Skins/Disc/myShader.bundle'. {Environment.NewLine}" +
                $"Enter 'default' to select the structure's default shader.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Don't forget to press Enter after typing your shader path to save the value.{Environment.NewLine}" +
                $"The text box should say 'Current Value: myShader' before you hit save."
            );

        AddSetting(
                "PillarSkinPath",
                "Pillar Skin Path",
                "default",
                $"The path to the shader used by pillar. {Environment.NewLine}" +
                $"'Pillar/myShader' maps to 'RUMBLE/UserData/Skins/Pillar/myShader.bundle'. {Environment.NewLine}" +
                $"Enter 'default' to select the structure's default shader.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Don't forget to press Enter after typing your shader path to save the value.{Environment.NewLine}" +
                $"The text box should say 'Current Value: myShader' before you hit save."
            );

        AddSetting(
                "BallSkinPath",
                "Ball Skin Path",
                "default",
                $"The path to the shader used by ball. {Environment.NewLine}" +
                $"'Ball/myShader' maps to 'RUMBLE/UserData/Skins/Ball/myShader.bundle'. {Environment.NewLine}" +
                $"Enter 'default' to select the structure's default shader.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Don't forget to press Enter after typing your shader path to save the value.{Environment.NewLine}" +
                $"The text box should say 'Current Value: myShader' before you hit save."
            );

        AddSetting(
                "RockCubeSkinPath",
                "Cube Skin Path",
                "default",
                $"The path to the shader used by cube. {Environment.NewLine}" +
                $"'Cube/myShader' maps to 'RUMBLE/UserData/Skins/Cube/myShader.bundle'. {Environment.NewLine}" +
                $"Enter 'default' to select the structure's default shader.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Don't forget to press Enter after typing your shader path to save the value.{Environment.NewLine}" +
                $"The text box should say 'Current Value: myShader' before you hit save."
            );

        AddSetting(
                "WallSkinPath",
                "Wall Skin Path",
                "default",
                $"The path to the shader used by wall. {Environment.NewLine}" +
                $"'Wall/myShader' maps to 'RUMBLE/UserData/Skins/Wall/myShader.bundle'. {Environment.NewLine}" +
                $"Enter 'default' to select the structure's default shader.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Don't forget to press Enter after typing your shader path to save the value.{Environment.NewLine}" +
                $"The text box should say 'Current Value: myShader' before you hit save."
            );

        AddSetting(
                "SmallRockSkinPath",
                "SmallRock Skin Path",
                "default",
                $"The path to the shader used by the small rocks found in the gym. {Environment.NewLine}" +
                $"'SmallRock/myShader' maps to 'RUMBLE/UserData/Skins/SmallRock/myShader.bundle'. {Environment.NewLine}" +
                $"Enter 'default' to select the structure's default shader.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Don't forget to press Enter after typing your shader path to save the value.{Environment.NewLine}" +
                $"The text box should say 'Current Value: myShader' before you hit save."
            );

        AddSetting(
                "LargeRockSkinPath",
                "Boulder Skin Path",
                "default",
                $"The path to the shader used by boulders. {Environment.NewLine}" +
                $"'LargeRock/myShader' maps to 'RUMBLE/UserData/Skins/LargeRock/myShader.bundle'. {Environment.NewLine}" +
                $"Enter 'default' to select the structure's default shader.{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Don't forget to press Enter after typing your shader path to save the value.{Environment.NewLine}" +
                $"The text box should say 'Current Value: myShader' before you hit save."
            );

        AssUI.GetFromFile();
        UI.instance.UI_Initialized += () => { UI.instance.AddMod(AssUI); };
        AssUI.ModSaved += SavePersistentModUISettings;

        if (PersistentSettings.data.lastLoadedVersion != AssUI.ModVersion)
        {
            PersistentSettings.data.lastLoadedVersion = AssUI.ModVersion;
            foreach (var setting in PersistentSettings.data.GetModSettings())
            {
                if (ASS.Settings.ContainsKey(setting.Key))
                    AssUI.ChangeValue(setting.Key, setting.Value);
            }

            AssUI.SaveModData($"Overwritten by Advanced Structure Skins v{BuildInfo.ModVersion}");
        }

        SavePersistentModUISettings();*/
    }

    private static void ModSaved()
    {
        foreach (var s in UnityEngine.Object.FindObjectsOfType<Structure>())
        {
            SkinHandler.ApplySkinTo(s);
        }
    }
    
    private static void AddSetting(string dictionaryKey, string name, string defaultValue, string description)
    {
        /*ASS.Settings.Add(
            dictionaryKey,
            AssUI.AddToList(
                name,
                defaultValue,
                description,
                new Tags()
            )
        );*/
    }
    
    public static void AddSetting(string dictionaryKey, string name, bool defaultValue, int linkGroup, string description)
    {
        /*ASS.Settings.Add(
            dictionaryKey,
            AssUI.AddToList(
                name,
                defaultValue,
                linkGroup,
                description,
                new Tags()
            )
        );*/
    }
    
    private static void SavePersistentModUISettings()
    {
        //PersistentSettings.data.ApplyFromModUISettings();
        //PersistentSettings.SaveSettings();
    }
}