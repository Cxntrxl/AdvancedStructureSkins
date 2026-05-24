using AdvancedStructureSkins.ShaderFeatures;
using HarmonyLib;
using Il2CppRUMBLE.MoveSystem;

namespace AdvancedStructureSkins.Skins;

[HarmonyPatch(typeof(Structure), "OnFetchFromPool")]
public static class StructureSpawnPatch
{
    private static void Postfix(ref Structure __instance)
    {
        try { SkinHandler.ApplySkinTo(__instance); }
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

[HarmonyPatch(typeof(PlayerStackProcessor), "Execute")]
public static class PlayerStackProcessorExecutePatch
{
    private static void Postfix(Stack stack, StackConfiguration overrideConfig, ref PlayerStackProcessor __instance)
    {
        try
        {
            ProcessableComponent comp = stack?.targetLinks[0]?.TargetObject
                ?.GetTarget(__instance.Cast<IProcessor>(), new StackConfiguration())
                .GetProcessorComponent<ProcessableComponent>();
            if (comp == null) return;

            comp.GetComponent<AdvancedSkin>().GetShaderFeature<TimeSinceLastModified>().ResetTimer();

            foreach (AdvancedSkin skin in UnityEngine.Object.FindObjectsOfType<AdvancedSkin>().Where(skin => skin.shaderFeatures[typeof(TimeSinceLastPose)].enabled))
                skin.GetShaderFeature<TimeSinceLastPose>().ResetTimer();
        } catch (Exception ex) { ASS.ErrorVerbose(ex); }
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