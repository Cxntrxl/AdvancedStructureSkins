using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Shared.SDK;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AdvancedStructureSkins.UI;

public class SkinSelectorWidget
{
    protected readonly SkinSelector parent;
    protected readonly byte bitmask;
    protected readonly Dictionary<string, Toggle> buttons = new Dictionary<string, Toggle>();
    protected readonly GameObject widget;
    protected readonly ScrollRect skinGroupScrollRect;
    
    private RectTransform ContentRect => skinGroupScrollRect?.content;

    protected SkinSelectorWidget(SkinSelector parent, int index)
    {
        this.parent = parent;
        widget = Object.Instantiate(UIHandler.SkinSelectorPrefab, parent.transform);
        skinGroupScrollRect = widget.transform.Find("Scroll View").GetComponent<ScrollRect>();
        bitmask = (byte)(1 << index);
    }

    public virtual void Populate()
    {
        foreach (var toggle in buttons.Values) Object.Destroy(toggle.gameObject);
        buttons.Clear();
    }

    public virtual void PostPopulate()
    {
        skinGroupScrollRect.horizontalScrollbar.value = GetScrollPoint();
    }

    protected void ToggleBit(ref byte currentValue, bool toggle)
    {
        currentValue = toggle ? (byte)(currentValue | bitmask) : (byte)(currentValue & ~bitmask);
    }

    protected Toggle CreateSkinItem(string name, Texture preview = null, Color? color = null)
    {
        Toggle skinItem = Object.Instantiate(UIHandler.SkinItemPrefab, ContentRect).GetComponent<Toggle>();
        skinItem.transform.Find("Description").GetComponent<TMP_Text>().text = name;
        if (preview != null)
            skinItem.transform.Find("Background/Preview").GetComponent<RawImage>().texture = preview;
        
        if (color != null)
            skinItem.transform.Find("Background/Preview").GetComponent<RawImage>().color = (Color)color;

        return skinItem;
    }

    protected void Submit()
    {
        SetScrollPoint();
        parent.Submit();
    }

    public void SetScrollPoint()
    {
        UIHandler.scrollPoints[$"{GetType().Name}_{bitmask}"] = skinGroupScrollRect.horizontalScrollbar.value;
    }

    private float GetScrollPoint()
    {
        return UIHandler.scrollPoints.GetValueOrDefault($"{GetType().Name}_{bitmask}", 0);
    }
}