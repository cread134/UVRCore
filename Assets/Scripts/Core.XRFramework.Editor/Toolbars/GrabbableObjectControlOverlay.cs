using Core.DevTools.Scripting;
using Core.XRFramework.Interaction.WorldObject;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.XrFramework.Toolbars
{
    [Overlay(typeof(SceneView), "Grabbable Object Overlay", true)]
    public class GrabbableObjectControlOverlay : ContextualOverlay<GrabbableObject>
    {
        public override VisualElement GetContent(GrabbableObject contextualObject, VisualElement root)
        {
            var headerLabel = new Label("Grabbable Object Controls");
            var dataBuilder = new System.Text.StringBuilder();
            dataBuilder.AppendLine($"IsGrabbed: {contextualObject.IsBeingGrabbed}");
            dataBuilder.AppendLine($"IsHovered: {contextualObject.IsHovered}");

            var dataLabel = new Label(dataBuilder.ToString());
            root.Add(headerLabel);
            root.Add(dataLabel);
            
            var releaseButton = new Button(() => ReleaseObject(contextualObject));
            releaseButton.text = "Release Object";
            root.Add(releaseButton);
            return root;
        }

        void ReleaseObject(GrabbableObject grabbableObject)
        {
            if (!Application.isPlaying) return;
            grabbableObject.DoRelease();
        }
    }
}
