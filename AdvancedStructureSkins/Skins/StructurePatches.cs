using AdvancedStructureSkins.ShaderFeatures;
using HarmonyLib;
using Il2CppRUMBLE.MoveSystem;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.Skins;

[HarmonyPatch(typeof(Structure), "OnFetchFromPool")]
public static class StructureSpawnPatch
{
    private static void Postfix(ref Structure __instance)
    {
        try
        {
            SkinHandler.ApplySkinTo(__instance); 
            __instance.GetComponent<AdvancedSkin>().GetShaderFeature<IsExplodeApplied>().SetProperty(ShaderPropertyType.Float, 0f);
        }
        catch (Exception ex) { ASS.Error(ex); }
    }
}

[HarmonyPatch(typeof(Structure), "Awake")]
public static class StructureStartPatch
{
    private static void Postfix(ref Structure __instance)
    {
        try
        {
            AdvancedSkin skin = __instance.gameObject.AddComponent<AdvancedSkin>();
            skin.parent = __instance;
            skin.Init();
        }
        catch (Exception ex) { ASS.Error(ex); }
    }
}

[HarmonyPatch(typeof(Structure), "OnPhysicsStateChanged")]
public static class StructurePhysicsStateChangedPatch
{
    private static void Postfix(Structure.PhysicsState previousState, Structure.PhysicsState newState, Structure __instance)
    {
        try
        {
            if (DidChangeGroundState(previousState, newState))
                __instance.GetComponent<AdvancedSkin>().GetShaderFeature<TimeSinceGroundStateChanged>().ResetTimer();
        } catch (Exception ex) { ASS.ErrorVerbose(ex); }
    }

    private static bool DidChangeGroundState(Structure.PhysicsState currentState, Structure.PhysicsState newState)
    {
        Structure.PhysicsState[] groundedStates = 
            { 
                Structure.PhysicsState.FreeGrounded, 
                Structure.PhysicsState.StableGrounded 
            };
        return groundedStates.Contains(currentState) != groundedStates.Contains(newState);
    }
}

[HarmonyPatch(typeof(Structure), "Shake")]
public static class StructureShakePatch
{
    private static void Postfix(float time, Structure __instance)
    {
        try
        {
            __instance.gameObject.GetComponent<AdvancedSkin>().GetShaderFeature<TimeSinceHitstop>().ResetTimer();
        } catch (Exception ex) { ASS.ErrorVerbose(ex); }
    }
}

[HarmonyPatch(typeof(ExplodeModifier), nameof(ExplodeModifier.Execute))]
public static class ExplodeModifierExecutePatch
{
    public static void Postfix(ExplodeModifier __instance, IProcessor processor, StackConfiguration config)
    {
        try
        {
            var comp = config.TargetProcessable.GetProcessorComponent<ProcessableComponent>();
            if (comp == null) return;
            
            AdvancedSkin skin = comp.GetComponent<AdvancedSkin>();
            
            skin?.GetShaderFeature<IsExplodeApplied>().SetProperty(ShaderPropertyType.Float, 1f);
            skin?.GetShaderFeature<TimeSinceExplodeApplied>().ResetTimer();
        } catch (Exception ex) { ASS.ErrorVerbose(ex); }
    }
}