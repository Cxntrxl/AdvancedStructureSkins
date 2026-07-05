using AdvancedStructureSkins.Skins;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class TimedShaderFeature : ShaderFeature
{
    private float _time = 0;
    private bool _counting = true;

    public TimedShaderFeature(AdvancedSkin target, string propertyName, float defaultValue = 999f) : base(target, ShaderPropertyType.Float, propertyName, defaultValue) { }

    public override void OnEnable()
    {
        base.OnEnable();
        if (!enabled) return;
        _time = (float)defaultValue;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (_counting) _time += Time.deltaTime;
        SetProperty(_time);
    }

    public virtual void ResetTimer()
    {
        _time = 0;
    }

    public virtual void PauseTimer(bool counting = false) { _counting = counting; }
    public virtual void ResumeTimer(bool counting = true) { _counting = counting; }
}