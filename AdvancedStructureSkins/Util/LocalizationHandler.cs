using Il2CppSmartLocalization;

namespace AdvancedStructureSkins.Util;

public class LocalizationHandler
{
    public static void AddLocalizationKey(string key, string value)
    {
        LanguageManager.instance.RawTextDatabase.Add(key, value);
        LocalizedObject lobj = new LocalizedObject(LanguageManager.instance.LanguageDatabase["Rank.Mountain.Name"]);
        lobj.textValue = value;
        LanguageManager.instance.LanguageDatabase.Add(key, lobj);
    }
}