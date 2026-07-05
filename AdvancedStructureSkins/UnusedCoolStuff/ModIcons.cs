using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Players.Subsystems;
using MelonLoader;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace AdvancedStructureSkins.UnusedCoolStuff;

public struct IconData
{
    public IconData(MelonBase mod, Texture2D icon)
    {
        this.mod = mod;
        this.icon = icon;
    }
    
    public MelonBase mod;
    public Texture2D icon;
}

public class ModIcons
{
    public static readonly List<IconData> RegisteredModIcons = new();
    
    private static readonly Vector3 Anchor = new Vector3(0.4727f, 0f, 0f);

    private const int BaseCapacity = 4;

    private const float BaseRadius = 0.1f;
    private const float RadiusGrowth = 0.125f;

    public static void RegisterIcon(MelonBase mod, Texture2D icon)
    {
        RegisteredModIcons.Add(new(mod, icon));
        RebuildIconsForAllPlayers();
    }

    public static void RebuildIconsForAllPlayers()
    {
        foreach (Player player in Calls.Players.GetAllPlayers())
        {
            PlayerUIBar remoteUI = player.Controller.GetSubsystem<PlayerUI>().remoteUIBar;
            for (int i = remoteUI.transform.childCount - 1; i >= 2; i--)
            {
                Object.Destroy(remoteUI.transform.GetChild(i).gameObject);
            }
            
            AddIconsTo(player);
        }
    }

    public static void AddIconsTo(Player player)
    {
        PlayerUIBar remoteUI = player.Controller.GetSubsystem<PlayerUI>().remoteUIBar;
        
        List<GameObject> icons = new List<GameObject>();
        foreach (var data in RegisteredModIcons)
        {
            icons.Add(CreateIcon(data));
        }

        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].transform.SetParent(remoteUI.transform);
            PositionIcon(icons[i], i, icons.Count);
        }
    }
    
    private static GameObject CreateIcon(IconData data)
    {
        GameObject icon = GameObject.CreatePrimitive(PrimitiveType.Quad);
        icon.transform.localPosition = new Vector3(0.4727f, 0f, 0f);
        icon.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        icon.transform.localScale = Vector3.one * 0.15f;
        
        MeshRenderer mr = icon.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit")) { mainTexture = data.icon };
        mr.material.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        mr.material.SetOverrideTag("RenderType", "Transparent");
        mr.material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        mr.material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        mr.material.SetInt("_ZWrite", 0);

        mr.material.renderQueue = (int)RenderQueue.Transparent;

        mr.material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        return icon;
    }
    
    public static void PositionIcon(GameObject icon, int position, int total)
    {
        if (icon == null || total <= 0)
            return;

        int indexInRing = GetIndexInRing(position, out int ring, out int ringCapacity);
        GetIndexInRing(total - 1, out int maxRings, out _);
        
        int actualCountInRing = Mathf.Min(ringCapacity, total - (position - indexInRing));
        actualCountInRing = Mathf.Max(actualCountInRing, 1);

        float radius = BaseRadius + ring * RadiusGrowth;
        float maxRadius = BaseRadius + maxRings * RadiusGrowth;

        Vector3 center = Anchor;

        float angleStep = (Mathf.PI * 2f) / actualCountInRing;
        float angle = angleStep * indexInRing + Mathf.PI / actualCountInRing * ((ring + 1) % 2);

        Vector3 offset = new Vector3(
            Mathf.Cos(angle),
            Mathf.Sin(angle),
            0f
        ) * radius;
        
        offset.x *= -1f;
        offset.x += maxRadius;

        icon.transform.localPosition = center + offset;
        icon.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public static int GetIndexInRing(int position, out int ring, out int ringCapacity)
    {
        int remaining = position;
        ringCapacity = BaseCapacity;
        ring = 0;
        
        while (remaining >= ringCapacity)
        {
            remaining -= ringCapacity;
            ring++;
            ringCapacity = BaseCapacity * (1 << ring); // 4 * 2^ring
        }

        return remaining;
    }
}