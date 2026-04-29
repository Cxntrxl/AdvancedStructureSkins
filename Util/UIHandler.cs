using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.MoveSystem;
using MelonLoader;
using UIFramework;

namespace AdvancedStructureSkins.Util;

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
        /*_textures = MelonPreferences.CreateCategory("AdvancedStructureSkins", "Textures");
        _meshes = MelonPreferences.CreateCategory("AdvancedStructureSkins", "Meshes");*/

        ShaderSettings = _shaders.CreateEntry("shaders", "", "shaders");
        /*TextureSettings = _textures.CreateEntry("textures", "[{\"default\":true}]", "textures");
        MeshSettings = _meshes.CreateEntry("meshes", "[{\"default\":true}]", "meshes");*/
        
        UI.Register(mod, _shaders/*, _textures, _meshes*/).OnModSaved += ModSaved;
        AssAPI.InitialUIUpdate(ShaderSettings.Value);
    }

    private static void ModSaved()
    {
        ShaderSettings.Value = AssAPI.InitialUIUpdate(ShaderSettings.Value);
        
        foreach (var s in UnityEngine.Object.FindObjectsOfType<Structure>())
        {
            SkinHandler.ApplySkinTo(s);
        }
    }
}