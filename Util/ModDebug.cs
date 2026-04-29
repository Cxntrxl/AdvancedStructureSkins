using AdvancedStructureSkins.API;
using Il2CppRUMBLE.MoveSystem;
using UnityEngine;

namespace AdvancedStructureSkins.Util;

public class ModDebug
{
    public static void AddInputs()
    {
        InputHandler.GetKeyDown(new[] { KeyCode.LeftAlt, KeyCode.S }, CustomShaders.LogCache);
        InputHandler.GetKeyDown(KeyCode.F9, LogPropertyNames);
        InputHandler.GetKeyDown(KeyCode.F8, ScanForTextureReference);
        InputHandler.GetKeyDown(new[] { KeyCode.LeftAlt, KeyCode.N}, ReplaceProperties);
    }

    private static void LogPropertyNames()
    {
        GameObject target = UnityEngine.Object.FindObjectsOfType<GameObject>()
            .First(x => x.gameObject.name == "SmallRock");
        Material mat = target.GetComponentInChildren<MeshRenderer>().material;
        for (int i = 0; i < 6; i++)
        {
            ASS.Log("[" + (MaterialPropertyType)i + "]: \n");
            foreach (string prop in mat.GetPropertyNames((MaterialPropertyType)i))
            {
                ASS.Log(prop);
            }
        }
    }

    private static void ReplaceProperties()
    {
        foreach (Structure structure in UnityEngine.Object.FindObjectsOfType<Structure>())
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            structure.meshRenderer.material.SetTexture("Texture2D_2058E65A", Texture2D.normalTexture);

            ASS.Log("Replaced textures for " + structure.gameObject.name);
        }
    }

    private static void ScanForTextureReference()
    {
        ASS.Log("[Texture Scan]: Starting...");
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
            ASS.Log("[Texture Scan]: Could not find Largerock_Lighting");
        }

        foreach (var m in Resources.FindObjectsOfTypeAll<Material>())
        {
            foreach (var s in m.GetTexturePropertyNames())
            {
                if (m.GetTexture(s) == target)
                {
                    ASS.Log("[Texture Scan]: Found Largerock_Lighting on material \"" + m.name + "\"");
                    ASS.Log("[Texture Scan]: Material shader is " + m.shader.name);
                    ASS.Log("[Texture Scan]: Property name is \"" + s + "\"");
                    return;
                }
            }
        }
    }
}