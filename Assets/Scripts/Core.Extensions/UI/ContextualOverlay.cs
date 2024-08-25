using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor.Overlays;
using Core.DevTools.UI;

namespace Core.DevTools.Scripting
{
    public class ContextualOverlay<T> : Overlay
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
            var root = new VisualElement { name = "ObjectControls" };
            var header = new Label($"{typeof(T).Name}Overlay");
            root.Add(header);
            var selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null && selectedGameObject.TryGetComponent(out T go))
            {
                GetContent(go, root);
            }
            return root;
        }

        public virtual VisualElement GetContent(T contextualObject, VisualElement root)
        {
            return null;
        }

        bool validSelection;
        void OnObjectSelected()
        {
            var selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null && selectedGameObject.TryGetComponent(out T go))
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