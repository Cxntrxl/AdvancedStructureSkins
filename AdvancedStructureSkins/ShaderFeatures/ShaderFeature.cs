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
    internal readonly ShaderPropertyType type;
    internal readonly object defaultValue;
    private Material[] Materials => target.MeshRenderer.materials;

    public ShaderFeature(AdvancedSkin target, ShaderPropertyType type, string propertyName, object defaultValue)
    {
        this.target = target;
        this.type = type;
        this.propertyName = propertyName;
        this.defaultValue = defaultValue;
    }

    public virtual void OnEnable()
    {
        if (!enabled) return;
        
        SetProperty(defaultValue);
    }
    
    public void Update()
    {
        if (!enabled) return;
        OnUpdate();
    }

    public void FixedUpdate()
    {
        if (!enabled) return;
        OnFixedUpdate();
    }

    protected virtual void OnUpdate() { }

    protected virtual void OnFixedUpdate() { }

    public virtual void SetProperty<T>(T value)
    {
        if (!enabled) return;
        
        try
        {
            switch (type)
            {
                case ShaderPropertyType.Color:
                    if (value is Color color) SetColor(color);
                    else ASS.WarnVerbose("Tried setting colour on ShaderFeature with a non-color value.");
                    break;
            
                case ShaderPropertyType.Vector:
                    switch (value) {
                        case Vector2 vector2: SetVector(vector2); break;
                        case Vector3 vector3: SetVector(vector3); break;
                        case Vector4 vector4: SetVector(vector4); break;
                        default: ASS.WarnVerbose("Tried setting vector on ShaderFeature with a non-vector value."); break; 
                    }
                    break;
            
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    if (value is float f) SetFloat(f);
                    else ASS.WarnVerbose("Tried setting float on ShaderFeature with a non-float value.");
                    break;
            
                case ShaderPropertyType.Texture:
                    if (value is Texture texture) SetTexture(texture);
                    else ASS.WarnVerbose("Tried setting texture on ShaderFeature with a non-texture value.");
                    break;
            
                case ShaderPropertyType.Int:
                    if (value is int i) SetInt(i);
                    else ASS.WarnVerbose("Tried setting int on ShaderFeature with a non-int value.");
                    break;
            
                default:
                    ASS.Warn("Tried setting invalid type on ShaderFeature");
                    break;
            }
        } catch (Exception ex) { ASS.ErrorVerbose(ex); }
    }

    private void SetColor(Color value)
    {
        foreach (Material mat in Materials) mat.SetColor(propertyName, value);
    }

    private void SetVector(Vector4 value)
    {
        foreach (Material mat in Materials) mat.SetVector(propertyName, value);
    }

    private void SetFloat(float value)
    {
        foreach (Material mat in Materials) mat.SetFloat(propertyName, value);
    }

    private void SetTexture(Texture value)
    {
        foreach (Material mat in Materials) mat.SetTexture(propertyName, value);
    }

    private void SetInt(int value)
    {
        foreach (Material mat in Materials) mat.SetInt(propertyName, value);
    }
}