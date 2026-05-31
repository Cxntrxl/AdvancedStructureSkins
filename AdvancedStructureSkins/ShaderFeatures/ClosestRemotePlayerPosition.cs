using System.Numerics;
using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.Managers;
using Il2CppRUMBLE.Players.Subsystems;
using RumbleModdingAPI.RMAPI;
using UnityEngine.Rendering;
using Vector3 = UnityEngine.Vector3;

namespace AdvancedStructureSkins.ShaderFeatures;

public class ClosestRemotePlayerPosition : ShaderFeature
{
    private PlayerVR ClosestRemotePlayer => Calls.Players.GetClosestPlayer(target.transform.position, true)?.Controller?.PlayerVR;
    
    public ClosestRemotePlayerPosition(AdvancedSkin target) : base(target, ShaderPropertyType.Vector, "_closestRemotePlayerPosition", Vector3.zero) { }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (ClosestRemotePlayer == null) return;
        
        SetProperty(ClosestRemotePlayer.GetFeetPosition() + Vector3.up * (ClosestRemotePlayer.GetCurrentPlayerHeight() * 0.5f));
    }
}