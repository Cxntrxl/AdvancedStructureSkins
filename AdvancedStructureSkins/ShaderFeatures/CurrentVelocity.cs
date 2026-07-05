using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.MoveSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class CurrentVelocity : ShaderFeature
{
    private Structure Structure => target.structure;
    
    public CurrentVelocity(AdvancedSkin target) : base(target, ShaderPropertyType.Vector, "_currentVelocity", Vector3.zero) { }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(Structure.CurrentVelocity);
    }
}