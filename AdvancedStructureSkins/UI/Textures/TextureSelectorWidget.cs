using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Shared.SDK;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdvancedStructureSkins.UI;

public class TextureSelectorWidget : SkinSelectorWidget
{
    internal TextureSelectorWidget(SkinSelector parent, int index) : base(parent, index)
    {
        widget.transform.Find("Data/Toggle").gameObject.SetActive(false);
        
        string structureName = ((StructureFlags)bitmask).ToString();
        
        widget.transform.Find("Data/Label").GetComponent<TMP_Text>().text = $"{structureName} Texture Group";
        if (bitmask != 128) widget.transform.Find("Description").GetComponent<TMP_Text>().text =
            $"Select which texture sets you want {structureName}s to use!";
    }

    public override void Populate()
    {
        base.Populate();
        
        foreach (AssTextureSet textureSet in AssAPI.TextureSets)
        {
            if (textureSet.isDefaultTextureSet && textureSet.targets != bitmask)
                continue;

            if ((textureSet.targets & bitmask) == 0) continue;
            
            Toggle button = CreateSkinItem(textureSet.name, 
                textureSet.previewTexture == null ? textureSet.GetRandomTexture(TextureType.Main) : textureSet.previewTexture,
                textureSet.isDefaultTextureSet ? new Color(176f / 255f, 142f / 255f, 115f / 255f) : Color.white);
            button.isOn = (textureSet.enabledForStructure & bitmask) != 0;

            button.onValueChanged.AddListener((Action<bool>)(toggle =>
            {
                ToggleBit(ref textureSet.enabledForStructure, toggle);
                Submit();
            }));
            buttons.Add(textureSet.name, button);
        }
    }
}