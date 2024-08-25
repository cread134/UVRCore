using Core.DevTools.Scripting;
using Core.DevTools.UI;
using Core.XRFramework;
using Core.XRFramework.Context;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.XrFramework.Toolbars
{
    [Overlay(typeof(SceneView), "GrabPointControls", true)]
    public class GrabPointControlOverlay : ContextualOverlay<GrabPoint>
    {
        public override VisualElement GetContent(GrabPoint contextualObject, VisualElement root)
        {
            root.AddHeader("Point controls");
            var data = @$"
Hand type: {contextualObject.handType}
IsGrabbed: {contextualObject.IsGrabbed}";
            root.Add(new Label(data));
            root.AddButton("Grab point", () => SelectGrabPoint(contextualObject));
            return root;
        }

        void SelectGrabPoint(GrabPoint goTarget)
        {
            if (!Application.isPlaying) return;
            var context = GameObject.FindFirstObjectByType<XrContext>();
            if (context != null)
            {
                var handtype = goTarget.handType;
                var useHand = context.GetHand(handtype);
                var conroller = context.GetController(handtype);
                if (goTarget.Parent != null)
                {
                    useHand.TryRelease();
                    if (goTarget.Parent.TryGetGrab(handtype, goTarget.transform.position, goTarget.transform.up, goTarget.transform.rotation, out var newPosition, out var newRotation))
                    {
                        useHand.transform.position = newPosition;
                        useHand.transform.rotation = newRotation;
                        conroller.transform.position = newPosition;
                        conroller.transform.rotation = newRotation;
                        conroller.GripValue = 1f;
                        useHand.hoveredObject = goTarget.Parent;
                        useHand.TryGrab();
                        Selection.activeGameObject = conroller.gameObject;
                    }
                }
            }
        }
    }
}
