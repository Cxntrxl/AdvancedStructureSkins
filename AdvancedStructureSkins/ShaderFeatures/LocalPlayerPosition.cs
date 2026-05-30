using AdvancedStructureSkins.Skins;
using RumbleModdingAPI.RMAPI;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class LocalPlayerPosition : ShaderFeature
{
    public LocalPlayerPosition(AdvancedSkin target) : base(target, ShaderPropertyType.Float,"_localPlayerPosition") { }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(Calls.Players.GetLocalPlayerController().transform.position);
    }
}