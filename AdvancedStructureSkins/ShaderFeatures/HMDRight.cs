using AdvancedStructureSkins.Skins;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

// ReSharper disable once InconsistentNaming
public class HMDRight : ShaderFeature
{
    public HMDRight(AdvancedSkin target) : base(target, ShaderPropertyType.Vector, "_HMDRight", Vector3.right) { }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(Calls.Players.GetLocalPlayerController().PlayerVR.Headset.Transform.right);
    }
}