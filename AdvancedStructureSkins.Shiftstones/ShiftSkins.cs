using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Shiftstones;
using MelonLoader;

[assembly: 
    MelonInfo(
        typeof(ShiftSkins), 
        ModInfo.ModName, 
        ModInfo.ModVersion,
        ModInfo.ModAuthor
    )
]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 210, 180, 145)]
[assembly: MelonAuthorColor(255, 5, 210, 240)]
[assembly: MelonAdditionalDependencies("AdvancedStructureSkins")]

namespace AdvancedStructureSkins.Shiftstones;

public static class ModInfo
{
    public const string ModName = "Shiftstone Structure Skins";
    public const string ModVersion = "1.0.0";
    public const string ModAuthor = "Cxntrxl";
}

// ReSharper disable once InconsistentNaming
public class ShiftSkins : MelonMod
{
    public override void OnLateInitializeMelon()
    {
        base.OnLateInitializeMelon();
    }
}