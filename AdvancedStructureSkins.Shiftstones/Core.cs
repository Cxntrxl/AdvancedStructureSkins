using AdvancedStructureSkins.API;
using MelonLoader;

[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 210, 180, 145)]
[assembly: MelonAuthorColor(255, 5, 210, 240)]
[assembly: MelonAdditionalDependencies("AdvancedStructureSkins")]

namespace ShiftstoneStructureSkins;

// ReSharper disable once InconsistentNaming
public class Core : MelonMod
{
    public override void OnLateInitializeMelon()
    {
        base.OnLateInitializeMelon();
    }
}