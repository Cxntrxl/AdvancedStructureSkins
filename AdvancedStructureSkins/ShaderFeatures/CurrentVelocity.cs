using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.MoveSystem;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class CurrentVelocity : ShaderFeature
{
    private Structure Structure => target.parent;
    
    public CurrentVelocity(AdvancedSkin target) : base(target, "_currentVelocity") { }

    public override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(ShaderPropertyType.Vector, Structure.CurrentVelocity);
    }
}