using AdvancedStructureSkins.Skins;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class LastLocalDamageTaken : ShaderFeature
{
    public LastLocalDamageTaken(AdvancedSkin target) : base(target, ShaderPropertyType.Float, "_lastLocalDamageTaken") { }
}