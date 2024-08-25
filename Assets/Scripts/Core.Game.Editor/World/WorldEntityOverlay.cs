using Core.DevTools.Scripting;
using Core.Game.World.EntityInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.Game.World
{
    [Overlay(typeof(SceneView), "World Entity Overlay", true)]
    public class WorldEntityOverlay : ContextualOverlay<IWorldEntity>
    {
        public override VisualElement GetContent(IWorldEntity contextualObject, VisualElement root)
        {
            root.Add(new Label("World Entity Controls"));
            return base.GetContent(contextualObject, root);
        }
    }
}
