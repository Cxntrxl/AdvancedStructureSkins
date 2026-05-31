using System.Numerics;
using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Players.Subsystems;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Rendering;
using Vector3 = UnityEngine.Vector3;

namespace AdvancedStructureSkins.ShaderFeatures;

public class LocalPlayerPosition : ShaderFeature
{
    private PlayerVR LocalPlayer => Calls.Players.GetLocalPlayerController().PlayerVR;
    
    public LocalPlayerPosition(AdvancedSkin target) : base(target, ShaderPropertyType.Vector,"_localPlayerPosition", Vector3.zero) { }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (LocalPlayer == null) return;
        
        SetProperty(LocalPlayer.GetFeetPosition() + Vector3.up * (LocalPlayer.GetCurrentPlayerHeight() * 0.5f));
    }
}