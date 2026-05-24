using AdvancedStructureSkins.API;
using AdvancedStructureSkins.ShaderFeatures;
using AdvancedStructureSkins.UI;
using AdvancedStructureSkins.Util;
using Il2CppRUMBLE.MoveSystem;
using Il2CppRUMBLE.Players;
using MelonLoader;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedStructureSkins.Skins;

[RegisterTypeInIl2Cpp]
public class AdvancedSkin : MonoBehaviour
{
    public AdvancedSkin(IntPtr ptr) : base(ptr) { }
    
    public Structure parent;
    public GameObjectDebugger debugger;
    public MeshRenderer MeshRenderer => parent.MeshRenderer;
    public AssShader currentShader;
    public AssTextureSet currentTexture;

    public readonly Dictionary<Type, ShaderFeature> shaderFeatures = new();

    public void Init()
    {
        AddShaderFeature(new CurrentVelocity(this));
        AddShaderFeature(new TimeSinceLastModified(this));
        AddShaderFeature(new TimeSinceLastPose(this)); 
        AddShaderFeature(new TimeSinceGroundStateChanged(this));
        AddShaderFeature(new TimeSinceHitstop(this)); 
        AddShaderFeature(new TimeSinceLocalDamageTaken(this)); 
        AddShaderFeature(new TimeSinceRemoteDamageTaken(this));
        AddShaderFeature(new LastLocalDamageTaken(this));
        AddShaderFeature(new LastRemoteDamageTaken(this));
        AddShaderFeature(new IsHost(this));
    }

    private void AddShaderFeature<T>(T feature) where T : ShaderFeature
    {
        if (!shaderFeatures.ContainsKey(feature.GetType())) shaderFeatures.Add(feature.GetType(), feature);
        else ASS.WarnVerbose("Tried adding a second " + feature.GetType() + " ShaderFeature to dictionary, ignored request.");
    }
    
    private void OnEnable()
    {
        if (UIHandler.DebugMode.Value && debugger == null) debugger = gameObject.AddComponent<GameObjectDebugger>();
        debugger?.Clear();
        debugger?.BindDefaults(this);

        foreach (var feature in shaderFeatures.Values) feature.OnEnable();

        Actions.onPlayerHealthChanged += PlayerHit;
    }

    private void OnDisable()
    {
        Actions.onPlayerHealthChanged -= PlayerHit;
    }

    private void PlayerHit(Player player, int change)
    {
        if (player.Controller.ControllerType == ControllerType.Local)
        {
            GetShaderFeature<TimeSinceLocalDamageTaken>().ResetTimer();
            GetShaderFeature<LastLocalDamageTaken>().SetProperty(ShaderPropertyType.Float, (float)change);
        }
        else
        {
            GetShaderFeature<TimeSinceRemoteDamageTaken>().ResetTimer();
            GetShaderFeature<LastRemoteDamageTaken>().SetProperty(ShaderPropertyType.Float, (float)change);
        }
    }

    private void Update()
    {
        foreach (var feature in shaderFeatures.Values) feature.Update();
    }

    public T GetShaderFeature<T>() where T : ShaderFeature
    {
        if (shaderFeatures.TryGetValue(typeof(T), out var feature)) return (T)feature;

        ASS.ErrorVerbose($"Could not find requested shader feature '{typeof(T).Name}'");
        return null;
    }

    public void RefreshEnabledShaderFeatures()
    {
        foreach (ShaderFeature feature in shaderFeatures.Values) { feature.enabled = !UIHandler.CompMode.Value && MeshRenderer.material.HasProperty(feature.propertyName); }
    }
}