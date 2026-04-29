/*using System.Reflection;
using MelonLoader.Utils;

namespace AdvancedStructureSkins.Util;

public static class PersistentSettings
{
    public static SettingsDataV1 data;
    public readonly static string path = Path.Combine(MelonEnvironment.UserDataDirectory, "AdvancedStructureSkins", "Persistence.txt");
    
    public static void LoadSettings()
    {
        Dictionary<string, string> save = LoadSettingsDictFromFile();
        
        switch (int.Parse(save["version"]))
        {
            case 1:
                data = LoadSettingsV1(save);
                break;
            default:
                ASS.Warn("Persistent Settings failed to load or does not exist.");
                break;
        }
    }

    public static void SaveSettings()
    {
        SaveSettingsV1();
    }
    
    public static Dictionary<string, string> LoadSettingsDictFromFile()
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        
        if (!File.Exists(path))
        {
            return new Dictionary<string, string>
            {
                ["version"] = 1.ToString()
            };
        }
        
        string[] settings = File.ReadAllText(path).Split(Environment.NewLine);
        Array.Resize(ref settings, settings.Length - 1);
        
        Dictionary<string, string> save = new Dictionary<string, string>();
        
        foreach (string setting in settings)
        {
            string[] split = setting.Split(": ");
            save.Add(split[0], split[1]);
        }

        return save;
    }

    public static SettingsDataV1 LoadSettingsV1(Dictionary<string, string> settings)
    {
        SettingsDataV1 save = new SettingsDataV1();
        var type = typeof(SettingsDataV1);
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            if (!settings.TryGetValue(field.Name, out string value))
                continue;

            object val = Convert.ChangeType(value, field.FieldType);
            field.SetValueDirect(__makeref(save), val);
        }

        return save;
    }

    public static void SaveSettingsV1()
    {
        string save = "";
        var fields = typeof(SettingsDataV1).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            save += $"{field.Name}: {field.GetValue(data)}{Environment.NewLine}";
        }
        
        File.WriteAllText(path, save);
    }
}

public struct SettingsDataV1
{
    public int version;
    public bool shownGraphicsNotification;
    public string lastLoadedVersion;
    public bool useGlobalSkin;
    public string globalSkinPath;
    public bool useLegacyModels;
    public string discSkinPath;
    public string pillarSkinPath;
    public string ballSkinPath;
    public string cubeSkinPath;
    public string wallSkinPath;
    public string smallRockSkinPath;
    public string boulderSkinPath;

    public SettingsDataV1()
    {
        version = 1;
        shownGraphicsNotification = false;
        lastLoadedVersion = ModInfo.ModVersion;
        useGlobalSkin = true;
        globalSkinPath = "default";
        useLegacyModels = false;
        discSkinPath = "default";
        pillarSkinPath = "default";
        ballSkinPath = "default";
        cubeSkinPath = "default";
        wallSkinPath = "default";
        smallRockSkinPath = "default";
        boulderSkinPath = "default";
    }

    public void ApplyFromModUISettings()
    {
        useGlobalSkin = GetSetting<bool>("GlobalSkinEnabled");
        globalSkinPath = GetSetting<string>("GlobalSkinPath");
        useLegacyModels = GetSetting<bool>("UseLegacyStructures");
        discSkinPath = GetSetting<string>("DiscSkinPath");
        pillarSkinPath = GetSetting<string>("PillarSkinPath");
        ballSkinPath = GetSetting<string>("BallSkinPath");
        cubeSkinPath = GetSetting<string>("RockCubeSkinPath");
        wallSkinPath = GetSetting<string>("WallSkinPath");
        smallRockSkinPath = GetSetting<string>("SmallRockSkinPath");
        boulderSkinPath = GetSetting<string>("LargeRockSkinPath");
    }

    private static T GetSetting<T>(string key)
    {
        if (ASS.Settings.TryGetValue(key, out var setting))
            return (T)setting.SavedValue;

        return default;
    }

    public Dictionary<string, string> GetModSettings()
    {
        Dictionary<string, string> settings = new Dictionary<string, string>();
        settings.Add("Use Global Skin", useGlobalSkin.ToString());
        settings.Add("Global Skin Path", globalSkinPath);
        settings.Add("Use Legacy Structures", useLegacyModels.ToString());
        settings.Add("Disc Skin Path", discSkinPath);
        settings.Add("Pillar Skin Path", pillarSkinPath);
        settings.Add("Ball Skin Path", ballSkinPath);
        settings.Add("Cube Skin Path", cubeSkinPath);
        settings.Add("Wall Skin Path", wallSkinPath);
        settings.Add("SmallRock Skin Path", smallRockSkinPath);
        settings.Add("Boulder Skin Path", boulderSkinPath);
        return settings;
    }
}*/