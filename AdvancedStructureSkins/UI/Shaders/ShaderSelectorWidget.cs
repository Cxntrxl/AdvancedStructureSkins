using AdvancedStructureSkins.API;
using AdvancedStructureSkins.Shared.SDK;
using Il2CppTMPro;
using UnityEngine.UI;

namespace AdvancedStructureSkins.UI;

public class ShaderSelectorWidget : SkinSelectorWidget
{
    private readonly Toggle _shaderGroupToggle;

    internal ShaderSelectorWidget(SkinSelector parent, int index) : base(parent, index)
    {
        _shaderGroupToggle = widget.transform.Find("Data/Toggle").GetComponent<Toggle>();
        
        string structureName = ((StructureFlags)bitmask).ToString();
        
        widget.transform.Find("Data/Label").GetComponent<TMP_Text>().text = $"{structureName} Shader Group";
        if (bitmask != 128) widget.transform.Find("Description").GetComponent<TMP_Text>().text =
            $"If this structure group is enabled, it will override any selections from the Global Shader Group when the mod applies shaders to {structureName}s";
    }

    public override void Populate()
    {
        base.Populate();
        
        _shaderGroupToggle.isOn = (AssAPI.ShaderGroups & bitmask) != 0 || bitmask == 128;
        _shaderGroupToggle.gameObject.SetActive(bitmask != 128);
        _shaderGroupToggle.onValueChanged.AddListener((Action<bool>)(toggle =>
        {
            ToggleBit(ref AssAPI.ShaderGroups, toggle);
            Submit();
        }));
        
        foreach (AssShader shader in AssAPI.Shaders)
        {
            if (shader.defaultShaderFor != 0 && shader.defaultShaderFor != bitmask)
                continue;
            
            Toggle button = CreateSkinItem(shader.name, shader.previewTexture);
            button.isOn = (shader.enabledForStructure & bitmask) != 0;

            button.onValueChanged.AddListener((Action<bool>)(toggle =>
            {
                ToggleBit(ref shader.enabledForStructure, toggle);
                Submit();
            }));
            buttons.Add(shader.name, button);
        }
    }
}