using UIFramework.Adapters;

namespace AdvancedStructureSkins.UI;

public class TextureSelector : SkinSelector
{
    protected override void DisplayEntryInfo(string displayName, string description)
    {
        _widgets = new SkinSelectorWidget[7];
        // Jank system to move Global (byte 1 << 7) to the top of the list
        // while otherwise ordering the list normally.
        for (int i = 0; i < 7; i++)
        {
            _widgets[i] = new TextureSelectorWidget(this, i);
        }
    }
}