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
    public class GrabPointControlOverlay : Overlay
    {
        public override void OnCreated()
        {
            Selection.selectionChanged += OnObjectSelected;
        }

        public override void OnWillBeDestroyed()
        {
            Selection.selectionChanged -= OnObjectSelected;
        }

        public override VisualElement CreatePanelContent()
        {
            VisualElement root = new VisualElement { name = "ObjectControls" };

            var goTarget = Selection.activeGameObject?.GetComponent<GrabPoint>();
            if (goTarget != null)
            {
                root.AddHeader("Point controls");
                root.AddButton("Grab point", () => SelectGrabPoint(goTarget));
            }
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

        bool validSelection;
        void OnObjectSelected()
        {
            var selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null && selectedGameObject.TryGetComponent(out GrabPoint go))
            {
                if (validSelection == false)
                {
                    this.Redraw();
                }
                validSelection = true;
            }
            else
            {
                if (validSelection == true)
                {
                    this.Redraw();
                }
                validSelection = false;
            }
        }
    }
}
