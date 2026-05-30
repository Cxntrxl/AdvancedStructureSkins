using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using AdvancedStructureSkins;
using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Shared.SDK;
using AdvancedStructureSkins.Skins;
using AdvancedStructureSkins.UI;
using AdvancedStructureSkins.Util;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppPhoton.Pun;
using Il2CppRUMBLE.MoveSystem;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Networking;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Players.Subsystems;
using RumbleModdingAPI;
using RumbleModdingAPI.RMAPI;
using UIFramework;
using UnityEngine.Rendering;
using ModInfo = AdvancedStructureSkins.ModInfo;
using Object = UnityEngine.Object;
using Random = System.Random;

[assembly: 
    MelonInfo(
        typeof(ASS), 
        ModInfo.ModName, 
        ModInfo.ModVersion,
        ModInfo.ModAuthor
    )
]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonColor(255, 210, 180, 145)]
[assembly: MelonAuthorColor(255, 5, 210, 240)]
[assembly: MelonAdditionalDependencies("UIFramework")]

namespace AdvancedStructureSkins;

public static class ModInfo
{
    public const string ModName = "Advanced Structure Skins";
    public const string ModVersion = "2.0.0";
    public const string ModAuthor = "Cxntrxl";
    public const string Description = "Allows custom shaders and skins to be applied to structures, and provides mod developers with a simple API for loading their own shaders.";
    public const bool ExperimentalBuild = true;
}

// ReSharper disable once InconsistentNaming
public class ASS : MelonMod
{
    public static ASS Instance;
    
    public static Random Random = new Random();
    private static bool VerboseLogging => UIHandler.Initialized ? UIHandler.DebugMode.Value : ModInfo.ExperimentalBuild;

    private static AssetBundle _cosmeticBundle;
    private static GameObject _cosmeticPrefab;
    private static Texture2D _hasAssIcon;

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();

        Instance ??= this;
        
        ClassInjector.RegisterTypeInIl2Cpp<AdvancedSkin>();
        ClassInjector.RegisterTypeInIl2Cpp<GameObjectDebugger>();
        ClassInjector.RegisterTypeInIl2Cpp<SkinManifest>();
        ClassInjector.RegisterTypeInIl2Cpp<SkinSelector>();
        ClassInjector.RegisterTypeInIl2Cpp<ShaderSelector>();
        ClassInjector.RegisterTypeInIl2Cpp<TextureSelector>();
        
        AssAPI.Init();
        UIHandler.Init();
        
        BulkPatcher.ApplyPatches(new HarmonyLib.Harmony("AdvancedStructureSkins.ASS"));
        LoadCosmeticBundle();
        Actions.onPlayerSpawned += OnPlayerSpawned;
        _hasAssIcon = LoadIcon();
    }
    
    public override void OnLateInitializeMelon()
    {
        base.OnLateInitializeMelon();
        SkinHandler.Init();
        InputHandler.Init();
        AddInputs();
        ModDebug.AddInputs();
        
        Lighting.UseBounceLighting(true);
        Lighting.SetHorizonLightColor(new Color(0.5f, 0.5f, 0.55f));
        Lighting.SetGroundLightColor(new Color(0.35f, 0.35f, 0.42f));
        
        Log("Advanced Structure Skins Initialized!");
    }
    
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        base.OnSceneWasLoaded(buildIndex, sceneName);
        Random = new Random(sceneName.GetHashCode());
    }

    private void LoadCosmeticBundle()
    {
        _cosmeticBundle = AssetBundles.LoadAssetBundleFromStream(this, "AdvancedStructureSkins.Resources.Cosmetics");
        _cosmeticPrefab = _cosmeticBundle.LoadAllAssets<GameObject>().FirstOrDefault();
        _cosmeticBundle.hideFlags = HideFlags.DontUnloadUnusedAsset;
        if (_cosmeticPrefab != null) _cosmeticPrefab.hideFlags = HideFlags.HideAndDontSave;
    }

    private static void OnPlayerSpawned(Player player)
    {
        ApplyCosmetics(player);
        
        if (player.Controller.ControllerType == ControllerType.Local) return;
        if (Calls.Players.GetLocalPlayer().Data.GeneralData.PlayFabMasterId == "932BFF1427FC3A2D") AddHasModCheck(player);
    }

    private static void ApplyCosmetics(Player player)
    {
        if (player.Data.GeneralData.PlayFabMasterId != "932BFF1427FC3A2D") return;

        Transform headBone = player.Controller.transform.FindChild("Visuals/Skelington/Bone_Pelvis/Bone_Spine_A/Bone_Chest/Bone_Neck/Bone_Head");
        Object.Instantiate(_cosmeticPrefab, headBone);

        if (player.Controller.ControllerType == ControllerType.Local) return;
        CreateAssIcon(player);
    }

    private static void AddHasModCheck(Player player)
    {
        if (!Calls.Mods.doesOpponentHaveMod("Advanced Structure Skins", "", false)) return;
        CreateAssIcon(player);
    }

    private static void CreateAssIcon(Player player)
    {
        PlayerUIBar remoteUI = player.Controller.GetSubsystem<PlayerUI>().remoteUIBar;
        GameObject hasAss = GameObject.CreatePrimitive(PrimitiveType.Quad);
        hasAss.transform.SetParent(remoteUI.transform);
        hasAss.transform.localPosition = new Vector3(0.4727f, 0f, 0f);
        hasAss.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        hasAss.transform.localScale = Vector3.one * 0.15f;
        
        MeshRenderer mr = hasAss.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit")) { mainTexture = _hasAssIcon };
        mr.material.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        mr.material.SetOverrideTag("RenderType", "Transparent");
        mr.material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        mr.material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        mr.material.SetInt("_ZWrite", 0);

        mr.material.renderQueue = (int)RenderQueue.Transparent;

        mr.material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    private static Texture2D LoadIcon()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.icon.png");
        
        if (stream == null) return null;
            
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);
            
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(buffer);
        tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
            
        return tex;
    }

    private static void AddInputs()
    {
        InputHandler.GetKeyDown(KeyCode.F5, () => { foreach (var s in UnityEngine.Object.FindObjectsOfType<Structure>()) { SkinHandler.ApplySkinTo(s); } });
    }

    public static void Log(object msg)
    {
        MelonLogger.Msg(msg);
    }

    public static void Warn(object msg)
    {
        MelonLogger.Warning(msg);
    }

    public static void Error(object msg)
    {
        MelonLogger.Error(msg);
    }

    public static void LogVerbose(object msg)
    {
        if (VerboseLogging)
            Log("[Verbose] " + msg);
    }
    
    public static void WarnVerbose(object msg)
    {
        if (VerboseLogging)
            Warn("[Verbose] " + msg);
    }
    
    public static void ErrorVerbose(object msg)
    {
        if (VerboseLogging)
            Error("[Verbose] " + msg);
    }
}