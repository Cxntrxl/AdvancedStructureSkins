using AdvancedStructureSkins.Skins;
using Il2CppRUMBLE.MoveSystem;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class ShaderFeature
{
    public bool enabled = false;
    
    internal readonly AdvancedSkin target;
    internal readonly string propertyName;
    private Material Material => target.MeshRenderer.material;

    public ShaderFeature(AdvancedSkin target, string propertyName)
    {
        this.target = target;
        this.propertyName = propertyName;
    }

    public virtual void OnEnable() { }
    
    public void Update()
    {
        if (!enabled) return;
        OnUpdate();
    }
    
    public virtual void OnUpdate() { }

    public virtual void SetProperty<T>(ShaderPropertyType type, T value)
    {
        try
        {
            switch (type)
            {
                case ShaderPropertyType.Color:
                    if (value is Color color) Material.SetColor(propertyName, color);
                    else ASS.WarnVerbose("Tried setting colour on ShaderFeature with a non-color value.");
                    break;
            
                case ShaderPropertyType.Vector:
                    switch (value) {
                        case Vector2 vector2: Material.SetVector(propertyName, vector2); break;
                        case Vector3 vector3: Material.SetVector(propertyName, vector3); break;
                        case Vector4 vector4: Material.SetVector(propertyName, vector4); break;
                        default: ASS.WarnVerbose("Tried setting vector on ShaderFeature with a non-vector value."); break; 
                    }
                    break;
            
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    if (value is float f) Material.SetFloat(propertyName, f);
                    else ASS.WarnVerbose("Tried setting float on ShaderFeature with a non-float value.");
                    break;
            
                case ShaderPropertyType.Texture:
                    if (value is Texture texture) Material.SetTexture(propertyName, texture);
                    else ASS.WarnVerbose("Tried setting texture on ShaderFeature with a non-texture value.");
                    break;
            
                case ShaderPropertyType.Int:
                    if (value is int i) Material.SetInt(propertyName, i);
                    else ASS.WarnVerbose("Tried setting int on ShaderFeature with a non-int value.");
                    break;
            
                default:
                    ASS.Warn("Tried setting invalid type on ShaderFeature");
                    break;
            }
        } catch (Exception ex) { ASS.ErrorVerbose(ex); }
    }
}