using System.Text;
using AdvancedStructureSkins.Skins;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedStructureSkins.Util;

[RegisterTypeInIl2Cpp]
public class GameObjectDebugger : MonoBehaviour
{
    public GameObjectDebugger(IntPtr ptr) : base(ptr) { }
    
    private Transform _uiRoot;
    private Canvas _canvas;
    
    private TextMeshProUGUI _persistentText;
    private TextMeshProUGUI _logText;

    private readonly List<string> _logs = new();

    private readonly Dictionary<string, Func<string>> _persistentData = new();

    private const int MaxLogs = 8;
    private readonly Vector3 _worldOffset = new(0, 0f, 0);
    private readonly StringBuilder _builder = new();

    private void Awake()
    {
        CreateCanvas();
    }

    private void LateUpdate()
    {
        UpdatePersistentData();

        _uiRoot.transform.rotation = Quaternion.identity;
        _uiRoot.transform.position = transform.position + _worldOffset;
    }

    private void OnWillRenderObject()
    {
        Camera cam = Camera.main;

        if (cam == null) return;

        Vector3 forward = _canvas.transform.position - cam.transform.position;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.0001f) return;

        _canvas.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
    }

    private void CreateCanvas()
    {
        _uiRoot = new GameObject("UI Root").transform;
        _uiRoot.SetParent(transform);
        
        GameObject canvasObj = new GameObject("DebugCanvas");
        
        canvasObj.transform.SetParent(_uiRoot);
        canvasObj.transform.localScale = Vector3.one * 0.0075f;
        
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 20;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(400, 300);

        _logText = CreateText(
            "Logs",
            canvasObj.transform,
            new Vector2(0, 100)
        );

        _persistentText = CreateText(
            "Persistent",
            canvasObj.transform,
            new Vector2(0, -100)
        );

        _persistentText.alignment = TextAlignmentOptions.Bottom;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, Vector2 anchoredPos)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 200);
        rect.anchoredPosition = anchoredPos;
        
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 20;
        text.alignment = TextAlignmentOptions.Top;
        text.text = "";
        
        return text;
    }

    public void Log(string message)
    {
        _logs.Add(message);

        while (_logs.Count > MaxLogs)
        {
            _logs.RemoveAt(0);
        }

        RefreshLogs();
    }

    private void RefreshLogs()
    {
        _builder.Clear();

        foreach (string log in _logs)
        {
            _builder.AppendLine(log);
        }
        
        _logText.text = _builder.ToString();
    }

    public void BindPersistent(string label, Func<string> getter)
    {
        _persistentData[label] = getter;
    }

    private void UpdatePersistentData()
    {
        _builder.Clear();

        foreach (var pair in _persistentData)
        {
            _builder.AppendLine($"{pair.Key}: {pair.Value()}");
        }
        
        _persistentText.text = _builder.ToString();
    }

    public void Clear()
    {
        _logs.Clear();
        _persistentData.Clear();
    }

    public void BindDefaults(AdvancedSkin skin)
    {
        try
        {
            BindPersistent("Tint", skin.MeshRenderer.material.GetColor("Color_D943764B").ToString);
            BindPersistent("Velocity", skin.parent.currentVelocity.ToString);
            BindPersistent("Shader", skin.currentShader != null ? skin.currentShader.name.ToString : () => "null");
            BindPersistent("Texture Set", skin.currentTexture != null ? skin.currentTexture.name.ToString : () => "null");
        }
        catch { /*ignored*/ }
    }

    public void SetEnabled(bool toggle)
    {
        _uiRoot.gameObject.SetActive(toggle);
    }
}