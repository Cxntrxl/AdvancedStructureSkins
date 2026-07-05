using AdvancedStructureSkins.Skins;
using HarmonyLib;
using Il2CppRUMBLE.Combat.ShiftStones;
using Il2CppRUMBLE.MoveSystem;

namespace AdvancedStructureSkins.Shiftstones.Charge;

[HarmonyPatch(typeof(HoldModifier), nameof(HoldModifier.Execute))]
public static class HoldModifierExecutePatch
{
    public static void Postfix(IProcessor processor, StackConfiguration config, HoldModifier __instance)
    {
        var comp = config.TargetProcessable.GetProcessorComponent<ProcessableComponent>();
        if (comp == null) return;
        
        AdvancedSkin skin = comp.GetComponent<AdvancedSkin>();
        skin.GetComponent<TimeSinceChargingStarted>().ResetTimer();
    }
}