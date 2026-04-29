using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using AdvancedStructureSkins;
using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Skins;
using AdvancedStructureSkins.Util;
using HarmonyLib;
using Il2CppRUMBLE.MoveSystem;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Networking;
using Il2CppRUMBLE.Players;
using OptimizedGraphicsFix.API;
using RumbleModdingAPI.RMAPI;
using UIFramework;
using UnityEngine.Rendering;
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
    public static AssetBundle Meshes;
    public static Random Random = new Random();
    private const bool VerboseLogging = true;

    public override void OnInitializeMelon()
    {
        base.OnInitializeMelon();
        AssAPI.Init();
        UIHandler.Init(this);
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

    private void AddInputs()
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