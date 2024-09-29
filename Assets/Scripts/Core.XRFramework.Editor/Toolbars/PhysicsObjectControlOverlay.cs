using Core.DevTools.Scripting;
using Core.XRFramework.Physics;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.XrFramework.Toolbars
{
    [Overlay(typeof(SceneView), "Physics Object Overlay", true)]
    public class PhysicsObjectControlOverlay : ContextualOverlay<PhysicsObject>
    {
        public override VisualElement GetContent(PhysicsObject contextualObject, VisualElement root)
        {
            var headerLabel = new Label("Physics Object Controls");
            var dataBuilder = new System.Text.StringBuilder();
            dataBuilder.AppendLine($"IsKinematic: {contextualObject.IsKinematic}");

            var dataLabel = new Label(dataBuilder.ToString());
            root.Add(headerLabel);
            root.Add(dataLabel);

            var toggleKinematicButton = new Button(() => ToggleKinematic(contextualObject));
            toggleKinematicButton.text = "Toggle Kinematic";
            root.Add(toggleKinematicButton);

            var existingBindings = contextualObject.ActiveBindings;
            foreach (var binding in existingBindings)
            {
                var bindingLabel = new Label($"Binding: {binding}");
                root.Add(bindingLabel);
            }

            var breakButton = new Button(() => BreakBinding(contextualObject));
            breakButton.text = "Break Bindings";
            root.Add(breakButton);

            return root;
        }

        void ToggleKinematic(PhysicsObject physicsObject)
        {
            if (!Application.isPlaying) return;
            physicsObject.IsKinematic = !physicsObject.IsKinematic;
        }

        void BreakBinding(PhysicsObject physicsObject)
        {
            if (!Application.isPlaying) return;
            physicsObject.ReleaseBindings();
        }
    }
}
