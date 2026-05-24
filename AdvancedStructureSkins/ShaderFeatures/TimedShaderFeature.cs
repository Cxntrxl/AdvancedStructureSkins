using AdvancedStructureSkins.Skins;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimedShaderFeature : ShaderFeature
{
    private float _time = 0;
    private bool _counting = true;

    public TimedShaderFeature(AdvancedSkin target, string propertyName) : base(target, propertyName) { }

    public override void OnEnable()
    {
        base.OnEnable();
        if (!enabled) return;
        ResetTimer();
    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (_counting) _time += Time.deltaTime;
        SetProperty(ShaderPropertyType.Float, _time);
    }

    public virtual void ResetTimer()
    {
        _time = 0;
    }

    public virtual void PauseTimer(bool counting = false)
    {
        _counting = counting;
    }
}