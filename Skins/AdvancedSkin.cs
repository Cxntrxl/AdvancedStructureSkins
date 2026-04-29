using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppPhoton.Pun;
using Il2CppRUMBLE.Audio;
using Il2CppRUMBLE.MoveSystem;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Utilities;
using MelonLoader;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Playables;
using Object = Il2CppSystem.Object;

namespace AdvancedStructureSkins.Skins;

[RegisterTypeInIl2Cpp]
public class AdvancedSkin : MonoBehaviour
{
    public MeshRenderer BMesh;
    public Structure Parent;
    public MeshObject meshSkin;
    private bool skipFrame = false;

    public float timeSinceLastModified = 0f;
    private static bool _hasIncrementedGlobalPropertiesThisFrame = false;
    public static float TimeSinceLastPose = 0f;
    public float timeSinceGroundStateChanged = 0f;
    public float isHitstop = 0f;
    public float timeSinceHitstop = 0f;
    public float timeSinceLocalDamageTaken = 0f;
    public float timeSinceRemoteDamageTaken = 0f;

    /*private void OnEnable()
    {
        Actions.onPlayerHealthChanged += OnPlayerHealthChanged;
    }

    private void OnDisable()
    {
        Actions.onPlayerHealthChanged -= OnPlayerHealthChanged;
    }*/
    
    private void Update()
    {
        //_hasIncrementedGlobalPropertiesThisFrame = false;
        HandleBananaMesh();
        /*IncrementCounters(Time.deltaTime);
        DecrementCounters(Time.deltaTime);

        //TODO:
        //Wobble
        
        Material material = Parent.MeshRenderer.material;
        material.SetVector("_Velocity", Parent.CurrentVelocity);
        material.SetFloat("_TimeSinceModified", timeSinceLastModified);
        material.SetFloat("_TimeSincePoseHit", TimeSinceLastPose);
        material.SetFloat("_TimeSinceGroundStateChanged", timeSinceGroundStateChanged);
        material.SetFloat("_TimeSinceHitstop", timeSinceHitstop);
        material.SetFloat("_Hitstop", isHitstop);
        material.SetFloat("_IsHost", Calls.Players.IsHost() ? 1 : 0);
        material.SetFloat("_TimeSinceLocalDamage", timeSinceLocalDamageTaken);
        material.SetFloat("_TimeSinceRemoteDamage", timeSinceRemoteDamageTaken);*/
        
        // Not working rn? no clue why. fix later.
        /*if (meshSkin != null && (bool)ASS.Settings["UseLegacyStructures"].SavedValue)
        {
            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            Parent.MeshRenderer.GetPropertyBlock(properties);
            meshSkin.renderer.SetPropertyBlock(properties);
        }*/
    }

    private void IncrementCounters(float delta)
    {
        timeSinceLastModified += delta;
        timeSinceGroundStateChanged += delta;
        timeSinceHitstop += delta;
        timeSinceLocalDamageTaken += delta;
        timeSinceRemoteDamageTaken += delta;

        if (_hasIncrementedGlobalPropertiesThisFrame)
            return;
        
        _hasIncrementedGlobalPropertiesThisFrame = true;

        TimeSinceLastPose += delta;
    }

    private void DecrementCounters(float delta)
    {
        isHitstop -= delta;
    }
    
    private void OnPlayerHealthChanged(Player p, int amount)
    {
        if (p.Controller.ControllerType == ControllerType.Local)
            timeSinceLocalDamageTaken = 0f;
        else
            timeSinceRemoteDamageTaken = 0f;
    }

    private void HandleBananaMesh()
    {
        if (!BMesh)
            return;

        if (!BMesh.enabled)
            return;

        if (skipFrame)
        {
            skipFrame = false;
            return;
        }
        
        if (BMesh.isVisible)
            return;

        ToggleBanana();
    }

    public void UseMeshObject(AssetBundle bundle)
    {
        if (meshSkin == null)
        {
            meshSkin = new MeshObject(Parent);
        }

        meshSkin.SetBundle(bundle);
        
        if (meshSkin.renderer == null)
        {
            meshSkin = null;
        }
    }

    public Il2CppReferenceArray<MeshRenderer> GetRenderers()
    {
        List<MeshRenderer> results = new List<MeshRenderer>();
        results.Add(Parent.MeshRenderer);
        
        if (meshSkin != null)
            results.Add(meshSkin.renderer);
        
        return new Il2CppReferenceArray<MeshRenderer>(results.ToArray());
    }

    public void ToggleBanana()
    {
        if (!BMesh)
            return;
        
        Parent.MeshRenderer.enabled = BMesh.enabled;
        BMesh.enabled = !BMesh.enabled;
        skipFrame = true;
    }
}

public class MeshObject
{
    public AssetBundle bundle;
    public Structure structure;
    public string structureName;
    public MeshRenderer renderer;
    public MeshFilter filter;

    public MeshObject(Structure s)
    {
        structure = s;
        structureName = SkinHandler.ConvertResourceName(structure.ResourceName);
    }
    
    public void SetBundle(AssetBundle newBundle)
    {
        if (newBundle == null)
            return;
        
        if (bundle == newBundle)
            return;
        
        if (renderer != null)
            renderer.gameObject.Destroy();
        
        bundle = newBundle;
        try
        {
            GameObject skin = GameObject.Instantiate(bundle.LoadAsset<GameObject>(structureName),
                structure.transform);
            SetSkin(skin);
        }
        catch (Exception ex)
        {
            //ASS.Log("No structure mesh found for " + structureName);
        }
    }
    
    public void SetSkin(GameObject obj)
    {
        renderer = obj.GetComponent<MeshRenderer>();
        filter = obj.GetComponent<MeshFilter>();
    }

    public void SetActive(bool active)
    {
        if (renderer == null)
        {
            structure.MeshRenderer.enabled = true;
            return;
        }
        
        renderer.enabled = active;
        structure.MeshRenderer.enabled = !active;
    }
}