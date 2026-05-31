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
    
    public Structure structure;
    public GameObjectDebugger debugger;
    public MeshRenderer MeshRenderer => structure.MeshRenderer;
    public AssShader currentShader;
    public AssTextureSet currentTexture;

    public Dictionary<Type, ShaderFeature> shaderFeatures = new();
    
    private void OnEnable()
    {
        if (UIHandler.DebugMode.Value && debugger == null) debugger = gameObject.AddComponent<GameObjectDebugger>();
        debugger?.Clear();
        debugger?.BindDefaults(this);

        shaderFeatures = AssAPI.GetFeaturesFor(this);
        foreach (var feature in shaderFeatures.Values) feature.OnEnable();

        Actions.onPlayerHealthChanged += PlayerHit;
    }

    private void OnDisable()
    {
        Actions.onPlayerHealthChanged -= PlayerHit;
        shaderFeatures.Clear();
    }

    private void PlayerHit(Player player, int change)
    {
        if (player.Controller.ControllerType == ControllerType.Local)
        {
            GetShaderFeature<TimeSinceLocalDamageTaken>().ResetTimer();
            GetShaderFeature<LastLocalDamageTaken>().SetProperty((float)change);
        }
        else
        {
            GetShaderFeature<TimeSinceRemoteDamageTaken>().ResetTimer();
            GetShaderFeature<LastRemoteDamageTaken>().SetProperty((float)change);
        }
    }

    private void Update()
    {
        foreach (var feature in shaderFeatures.Values) feature.Update();
    }

    private void FixedUpdate()
    {
        foreach (var feature in shaderFeatures.Values) feature.FixedUpdate();
    }

    public T GetShaderFeature<T>() where T : ShaderFeature
    {
        if (shaderFeatures.TryGetValue(typeof(T), out var feature)) return (T)feature;

        ASS.ErrorVerbose($"Could not find requested shader feature '{typeof(T).Name}'");
        return null;
    }

    public void RefreshEnabledShaderFeatures()
    {
        foreach (ShaderFeature feature in shaderFeatures.Values) { feature.enabled = !UIHandler.CompMode.Value && MeshRenderer.material.HasProperty(feature.propertyName); feature.OnEnable(); }
    }
}