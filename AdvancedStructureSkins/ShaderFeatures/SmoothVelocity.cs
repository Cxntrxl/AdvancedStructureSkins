using AdvancedStructureSkins.Skins;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.ShaderFeatures;

public class SmoothVelocity : ShaderFeature
{
    public SmoothVelocity(AdvancedSkin target) : base(target, ShaderPropertyType.Vector, "_smoothVelocity") { }

    private struct Sample
    {
        public Vector3 velocity;
        public float time;
    }
    
    private readonly Queue<Sample> _samples = new();
    private Vector3 _sum = Vector3.zero;
    private const float TimeWindow = 2f;

    private void Add(Vector3 value)
    {
        float now = Time.time;
        _samples.Enqueue(new Sample{ velocity = value, time = now });
        _sum += value;

        while (_samples.Count > 0 && now - _samples.Peek().time > TimeWindow)
        {
            var old = _samples.Dequeue();
            _sum -= old.velocity;
        }
    }

    private Vector3 GetAverage()
    {
        if (_samples.Count == 0) return Vector3.zero;
        
        return _sum / _samples.Count;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        SetProperty(GetAverage());
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        Add(target.structure.rigidBody.velocity);
    }
}