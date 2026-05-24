namespace AdvancedStructureSkins.UI;

public class ShaderSelector : SkinSelector
{
    protected override void DisplayEntryInfo(string displayName, string description)
    {
        _widgets = new SkinSelectorWidget[8];
        // Jank system to move Global (byte 1 << 7) to the top of the list
        // while otherwise ordering the list normally.
        for (int i = 7; i >= 0; i--)
        {
            int structureIndex = i == 7 ? 7 : 6 - i; 
            _widgets[structureIndex] = new ShaderSelectorWidget(this, structureIndex);
        }
    }
}