using AdvancedStructureSkins.Skins;
using Il2CppPhoton.Pun;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class IsHost : ShaderFeature
{
    private static bool Host => !PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient;
    
    public IsHost(AdvancedSkin target) : base(target, ShaderPropertyType.Int,"_isHost") { }
    
    public override void OnEnable()
    {
        base.OnEnable();
        SetProperty(Host ? 1 : 0);
    }
}