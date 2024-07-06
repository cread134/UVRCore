using UnityEditor.Overlays;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Core.DevTools.Scripting;

namespace Core.XrFramework.Toolbars
{
    [Overlay(typeof(SceneView), "GizmoControls", true)]
    public class GizmoToolbar : Overlay
    {
        public override VisualElement CreatePanelContent()
        {
            var flagsField = new EnumFlagsField(string.Empty, GizmoGroup.GroupType.ComponentIndicators | GizmoGroup.GroupType.ComponentData);
            flagsField.RegisterValueChangedCallback((evt) =>
            {
                GizmoGroup.ActiveType = (GizmoGroup.GroupType)evt.newValue;
            });
            flagsField.style.width = 500;
            return flagsField;
        }
    }
}