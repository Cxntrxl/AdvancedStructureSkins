using System.Collections;
using AdvancedStructureSkins.API;
using MelonLoader;
using UIFramework.Adapters;
using UnityEngine;

namespace AdvancedStructureSkins.UI;

public class SkinSelector : DataEntryAdapter
{
    protected SkinSelectorWidget[] _widgets;

    protected override void DisplayData(object boxedValue)
    {
        base.DisplayData(boxedValue);
        if (boxedValue is not string)
        {
            ASS.Error("DisplayData is not string");
            return;
        }

        foreach (SkinSelectorWidget widget in _widgets)
        {
            widget.Populate();
            widget.PostPopulate();
        }
    }

    public override void PreSaveAction()
    {
        base.PreSaveAction();
        foreach (SkinSelectorWidget widget in _widgets) widget.SetScrollPoint();
    }

    internal void Submit()
    {
        SubmitValue(AssAPI.Serialize());
    }
}