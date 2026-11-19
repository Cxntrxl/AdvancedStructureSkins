using System.Reflection;
using AdvancedStructureSkins.ShaderFeatures;
using HarmonyLib;
using Il2CppRUMBLE.MoveSystem;
using Il2CppInterop.Common;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.MethodInfo;

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
        
        Dictionary<IntPtr, MethodInfo> seenAddresses = new();

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
                
                IntPtr address = GetNativeMethodPointer(method);

                if (seenAddresses.TryGetValue(address, out MethodInfo existing))
                {
                    Core.WarnVerbose(
                        $"\nNative Address Collision in BulkPatcher\n" +
                        "You probably don't need to worry about this - Cxntrxl\n" +
                        $"Address: 0x{address.ToInt64():X}\n" +
                        $"Existing: {existing.DeclaringType.FullName}.{existing.Name}\n" +
                        $"Current: {method.DeclaringType.FullName}.{method.Name}"
                    );
                    continue;
                }
                else
                {
                    seenAddresses.Add(address, method);
                }

                harmony.Patch(
                    original: method,
                    postfix: new HarmonyMethod(
                        typeof(BulkPatcher)
                            .GetMethod(nameof(ModifierExecutePostfix),
                                BindingFlags.Static |
                                BindingFlags.Public)));

                Core.LogVerbose($"Patched: {type.FullName}.Execute");
            }
            catch (Exception ex)
            {
                Core.ErrorVerbose(
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
        } catch (Exception ex) { Core.ErrorVerbose(ex); }
    }
    
    private static unsafe IntPtr GetNativeMethodPointer(MethodBase method)
    {
        FieldInfo field =
            Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method);

        if (field == null)
            throw new Exception($"Could not resolve MethodInfo: {method}");

        IntPtr methodInfoPtr = (IntPtr)field.GetValue(null);

        var nativeMethodInfo =
            UnityVersionHandler.Wrap((Il2CppMethodInfo*)methodInfoPtr);

        return nativeMethodInfo.MethodPointer;
    }
}