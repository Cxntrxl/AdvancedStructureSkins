using HarmonyLib;
using Il2CppInterop.Runtime.Runtime;
using Il2CppRUMBLE.MoveSystem;
using Il2CppRUMBLE.Networking;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using Object = System.Object;

namespace AdvancedStructureSkins.Skins;

[HarmonyPatch(typeof(Structure), "OnFetchFromPool")]
public static class StructureSpawnPatch
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref Structure __instance)
    {
        try
        {
            SkinHandler.ApplySkinTo(__instance);
            if (ASS.Random.NextSingle() < 0.02f)
            {
                __instance.gameObject.GetComponent<AdvancedSkin>().ToggleBanana();
            }
        }
        catch (Exception ex)
        {
            ASS.Log(ex);
        }
    }
}

[HarmonyPatch(typeof(Structure), "Awake")]
public static class StructureStartPatch
{
    private static void Postfix(ref Structure __instance)
    {
        AdvancedSkin skin = __instance.gameObject.AddComponent<AdvancedSkin>();
        skin.Parent = __instance;
        
        try
        {
            // If you're a modder decompiling ASS to figure out why it's suddenly 8mb, it's because
            // of this incredibly early setup for an april fools day joke. you caught me.
            // lets keep this between us PLEASE I SPENT SO MUCH TIME ON THIS LMFAO
            if (!(DateTime.Today.Month == 4 && DateTime.Today.Day == 1))
                return;
            
            if (!ASS.Meshes)
            {
                ASS.Meshes = AssetBundles.LoadAssetBundleFromStream(ModInfo.ModName, ModInfo.ModAuthor,
                    "AdvancedStructureSkins.Resources.structuremesh");
            }
            
            GameObject mesh = UnityEngine.Object.Instantiate(ASS.Meshes.LoadAsset<GameObject>(__instance.ResourceName), __instance.transform);
            skin.BMesh = mesh.GetComponent<MeshRenderer>();
            skin.BMesh.enabled = false;
            
            skin.BMesh.material = new Material(Shader.Find("Shader Graphs/RUMBLE_Prop"));
            skin.BMesh.material.SetColor("_Overlay", new Color(0.925f, 0.854f, 0.423f));
            skin.BMesh.material.SetTexture("_Albedo", ASS.Meshes.LoadAsset<Texture2D>("T_Banana"));
        }
        catch (Exception ex)
        {
        }
    }
}

/*[HarmonyPatch(typeof(PlayerStackProcessor), "Execute")]
public static class PlayerStackProcessorExecutePatch
{
    private static void Postfix(Stack stack, StackConfiguration overrideConfig, ref PlayerStackProcessor __instance)
    {
        ProcessableComponent comp = stack?.targetLinks[0]?.TargetObject
            ?.GetTarget(__instance.Cast<IProcessor>(), new StackConfiguration())
            .GetProcessorComponent<ProcessableComponent>();
        if (comp == null) return;
        
        comp.GetComponent<AdvancedSkin>().timeSinceLastModified = 0f;
        AdvancedSkin.TimeSinceLastPose = 0f;
    }
}

[HarmonyPatch(typeof(Structure), "OnPhysicsStateChanged")]
public static class StructurePhysicsStateChangedPatch
{
    private static void Postfix(Structure.PhysicsState previousState, Structure.PhysicsState newState, Structure __instance)
    {
        if (DidChangeGroundState(previousState, newState))
            __instance.GetComponent<AdvancedSkin>().timeSinceGroundStateChanged = 0f;
    }

    private static bool DidChangeGroundState(Structure.PhysicsState currentState, Structure.PhysicsState newState)
    {
        if (currentState == Structure.PhysicsState.FreeGrounded ||
            currentState == Structure.PhysicsState.StableGrounded)
        {
            return newState != Structure.PhysicsState.FreeGrounded || newState != Structure.PhysicsState.StableGrounded;
        }
        else
        {
            return newState == Structure.PhysicsState.FreeGrounded || newState == Structure.PhysicsState.StableGrounded;
        }
    }
}*/

/*[HarmonyPatch(typeof(Structure), "Shake")]
public static class StructureShakePatch
{
    private static void Postfix(float time, Structure __instance)
    {
        AdvancedSkin advancedSkin = __instance.gameObject.AddComponent<AdvancedSkin>();
        advancedSkin.timeSinceHitstop = 0f;
        advancedSkin.isHitstop = 1f + time;
    }
}*/