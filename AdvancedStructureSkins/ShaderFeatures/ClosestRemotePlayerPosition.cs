using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.Managers;
using RumbleModdingAPI.RMAPI;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class ClosestRemotePlayerPosition : ShaderFeature
{
    public ClosestRemotePlayerPosition(AdvancedSkin target) : base(target, ShaderPropertyType.Vector, "_closestRemotePlayerPosition") { }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(Calls.Players.GetClosestPlayer(target.transform.position, true));
    }
}