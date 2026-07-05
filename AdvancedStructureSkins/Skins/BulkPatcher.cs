using System.Reflection;
using AdvancedStructureSkins.ShaderFeatures;
using HarmonyLib;
using Il2CppRUMBLE.MoveSystem;

namespace AdvancedStructureSkins.Skins;

public static class BulkPatcher
{
    public static void ApplyPatches(HarmonyLib.Harmony harmony)
    {
        PatchModifiers(harmony);
    }

    public static void PatchModifiers(HarmonyLib.Harmony harmony)
    {
        Type baseType = typeof(Modifier);

        Assembly asm = baseType.Assembly;

        var derivedTypes = asm.GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                baseType.IsAssignableFrom(t));

        foreach (Type type in derivedTypes)
        {
            try
            {
                MethodInfo method = type.GetMethod(
                    "Execute",
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic);

                if (method == null)
                    continue;
                
                if (method.DeclaringType != type)
                    continue;

                harmony.Patch(
                    original: method,
                    postfix: new HarmonyMethod(
                        typeof(BulkPatcher)
                            .GetMethod(nameof(ModifierExecutePostfix),
                                BindingFlags.Static |
                                BindingFlags.Public)));

                ASS.LogVerbose($"Patched: {type.FullName}.Execute");
            }
            catch (Exception ex)
            {
                ASS.ErrorVerbose(
                    $"Failed patching {type.FullName}: {ex}");
            }
        }
    }

    public static void ModifierExecutePostfix(Modifier __instance, IProcessor processor, StackConfiguration config)
    {
        try
        {
            var comp = config.TargetProcessable.GetProcessorComponent<ProcessableComponent>();
            if (comp == null) return;
            
            AdvancedSkin skin = comp.GetComponent<AdvancedSkin>();
            TimeSinceLastModified feature = skin?.GetShaderFeature<TimeSinceLastModified>();
            feature?.ResetTimer();

            foreach (AdvancedSkin s in UnityEngine.Object.FindObjectsOfType<AdvancedSkin>())
            {
                if (!s || !s.isActiveAndEnabled) continue;
                var pose = s.GetShaderFeature<TimeSinceLastPose>();
                if (pose is not { enabled: true })
                    continue;

                pose.ResetTimer();
            }
        } catch (Exception ex) { ASS.ErrorVerbose(ex); }
    }
}