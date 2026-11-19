using AdvancedStructureSkins.API;
using Il2CppRUMBLE.MoveSystem;
using UnityEngine;

namespace AdvancedStructureSkins.Util;

public class ModDebug
{
    public static void AddInputs()
    {
        if (!ModInfo.Experimental)
            return;
        
        Core.Log("ExperimentalBuild enabled. Adding debug binds");
        
        InputHandler.GetKeyDown(KeyCode.F1, LogPropertyNames);
        InputHandler.GetKeyDown(KeyCode.F2, ScanForTextureReference);
        InputHandler.GetKeyDown(new[] { KeyCode.LeftAlt, KeyCode.N}, ReplaceProperties);
    }

    private static void LogPropertyNames()
    {
        GameObject target = UnityEngine.Object.FindObjectsOfType<GameObject>()
            .First(x => x.gameObject.name == "SmallRock");
        Material mat = target.GetComponentInChildren<MeshRenderer>().material;
        for (int i = 0; i < 6; i++)
        {
            Core.Log("[" + (MaterialPropertyType)i + "]: \n");
            foreach (string prop in mat.GetPropertyNames((MaterialPropertyType)i))
            {
                Core.Log(prop);
            }
        }
    }

    private static void ReplaceProperties()
    {
        foreach (Structure structure in UnityEngine.Object.FindObjectsOfType<Structure>())
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            structure.meshRenderer.material.SetTexture("Texture2D_2058E65A", Texture2D.normalTexture);

            Core.Log("Replaced textures for " + structure.gameObject.name);
        }
    }

    private static void ScanForTextureReference()
    {
        Core.Log("[Texture Scan]: Starting...");
        Texture2D target = null;
        foreach (var t in Resources.FindObjectsOfTypeAll<Texture2D>())
        {
            if (t.name == "Largerock_Lighting")
            {
                target = t;
            }
        }

        if (!target)
        {
            Core.Log("[Texture Scan]: Could not find Largerock_Lighting");
        }

        foreach (var m in Resources.FindObjectsOfTypeAll<Material>())
        {
            foreach (var s in m.GetTexturePropertyNames())
            {
                if (m.GetTexture(s) == target)
                {
                    Core.Log("[Texture Scan]: Found Largerock_Lighting on material \"" + m.name + "\"");
                    Core.Log("[Texture Scan]: Material shader is " + m.shader.name);
                    Core.Log("[Texture Scan]: Property name is \"" + s + "\"");
                    return;
                }
            }
        }
    }
}