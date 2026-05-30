using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class LastRemoteDamageTaken : ShaderFeature
{
    public LastRemoteDamageTaken(AdvancedSkin target) : base(target, ShaderPropertyType.Float,"_lastRemoteDamageTaken") { }
}