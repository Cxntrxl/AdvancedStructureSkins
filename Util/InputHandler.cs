using MelonLoader;
using UnityEngine;

namespace AdvancedStructureSkins.Util;

public static class InputHandler
{
    private static readonly List<KeyInput> KeyDown = new();
    private static readonly List<KeyHeldInput> KeyHeld = new();
    private static readonly List<KeyInput> Key = new();
    private static readonly List<KeyInput> KeyUp = new();
    
    public static void Init()
    {
        MelonEvents.OnUpdate.Subscribe(Update);
    }

    public static void GetKeyDown(KeyCode key, Delegate action)
    {
        KeyDown.Add(new KeyInput(new[]{key}, action));
    }
    
    public static void GetKeyDown(KeyCode[] key, Delegate action)
    {
        KeyDown.Add(new KeyInput(key, action));
    }

    public static void GetKeyHeld(KeyCode key, Delegate action, float holdTime)
    {
        KeyHeld.Add(new KeyHeldInput(new[]{key}, action, holdTime));
    }
    
    public static void GetKeyHeld(KeyCode[] key, Delegate action, float holdTime)
    {
        KeyHeld.Add(new KeyHeldInput(key, action, holdTime));
    }

    public static void GetKey(KeyCode key, Delegate action)
    {
        Key.Add(new KeyInput(new[]{key}, action));
    }
    
    public static void GetKey(KeyCode[] key, Delegate action)
    {
        Key.Add(new KeyInput(key, action));
    }

    public static void GetKeyUp(KeyCode key, Delegate action)
    {
        KeyUp.Add(new KeyInput(new[]{key}, action));
    }
    
    public static void GetKeyUp(KeyCode[] key, Delegate action)
    {
        KeyUp.Add(new KeyInput(key, action));
    }

    private static void Update()
    {
        foreach (KeyInput input in KeyDown)
        {
            if (input.key.All(Input.GetKey) && input.key.Any(Input.GetKeyDown))
            {
                input.action.DynamicInvoke();
            }
        }

        foreach (KeyHeldInput input in KeyHeld)
        {
            if (input.key.All(Input.GetKey))
            {
                input.time += Time.deltaTime;
                if (input.time >= input.holdTime)
                {
                    input.time = -100f;
                    input.action.DynamicInvoke();
                }
            }

            if (input.key.Any(Input.GetKeyUp))
            {
                input.time = 0;
            }
        }

        foreach (KeyInput input in Key)
        {
            if (input.key.All(Input.GetKey))
            {
                input.action.DynamicInvoke();
            }
        }

        foreach (KeyInput input in KeyUp)
        {
            if (input.key.All(Input.GetKeyUp))
            {
                input.action.DynamicInvoke();
            }
        }
    }
}

public class KeyInput
{
    protected KeyInput(){}
    
    public KeyInput(KeyCode[] key, Delegate action)
    {
        this.key = key;
        this.action = action;
    }
    
    public KeyCode[] key;
    public Delegate action;
}

public class KeyHeldInput : KeyInput
{
    public KeyHeldInput(KeyCode[] key, Delegate action, float holdTime)
    {
        this.key = key;
        this.action = action;
        this.holdTime = holdTime;
    }

    public readonly float holdTime;
    public float time;
}