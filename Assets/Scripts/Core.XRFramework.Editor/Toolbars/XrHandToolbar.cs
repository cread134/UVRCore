using Core.DevTools.UI;
using Core.XRFramework.Context;
using Core.XRFramework.Interaction;
using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.XrFramework.Toolbars
{
    [Overlay(typeof(SceneView), "Xr Controls", true)]
    public class XrHandToolbar : Overlay
    {
        bool leftOpen;
        bool rightOpen;

        public override VisualElement CreatePanelContent()
        {

            var root = new VisualElement() { name = "Xr Toolbar" };
            var context = GameObject.FindFirstObjectByType<XrContext>();
            if (context == null)
            {
                root.AddHeader("No XrContext found");
                return root;
            }

            root.AddHeader("XR TOOLS");

            root.Add(CreateHandElement(HandType.Right, context));
            root.Add(CreateHandElement(HandType.Left, context));
            return root;

        }

        VisualElement CreateHandElement(HandType HandType, XrContext context)
        {
            var root = new VisualElement();

            var controller = context.GetController(HandType);
            var controlsOpen = rightOpen;
            Action accessAction = () => AccessRightHand(false);
            Action accessFrame = () => AccessRightHand(true);
            Action mainButtonAction = OnRightMainButton;
            Action secondaryButtonAction = OnRightSecondaryButton;
            EventCallback<ChangeEvent<float>> gripAction = OnRightGripChange;
            EventCallback<ChangeEvent<float>> triggerAction = OnRightTriggerChange;
            if (HandType == HandType.Left)
            {
                controlsOpen = leftOpen;
                accessAction = () => AccessLeftHand(false);
                accessFrame = () => AccessLeftHand(true);
                mainButtonAction = OnLeftMainButton;
                secondaryButtonAction = OnLeftSecondaryButton;
                gripAction = OnLeftGripChange;
                triggerAction = OnLeftTriggerChange;
            }

            var horizontalBox = new VisualElement();
            horizontalBox.style.alignContent = Align.FlexStart;
            horizontalBox.style.flexDirection = FlexDirection.Row;
            horizontalBox.AddButton($"{HandType}Hand", accessAction);
            horizontalBox.AddButton($"{HandType}Hand_Frame", accessFrame);
            var controlsButton = horizontalBox.AddButton("Ctrl", () =>
            {
                if (HandType == HandType.Right)
                {
                    rightOpen = !rightOpen;
                }
                else
                {
                    leftOpen = !leftOpen;
                }
                this.Redraw();
            });

            root.Add(horizontalBox);
            if (controlsOpen)
            {
                root.style.width = 300;
                controlsButton.style.backgroundColor = Color.grey;
                root.AddSubtitle($"Controls {HandType}");
                root.AddSlider("Grip", 1f, 0f, gripAction, controller.GripValue);
                root.AddSlider("Trigger", 1f, 0f, triggerAction, controller.TriggerValue);
                root.AddButton("MainButton", mainButtonAction);
                root.AddButton("SecondaryButton", secondaryButtonAction);
            }

            return root;
        }

        #region updateGrip
        void OnRightGripChange(ChangeEvent<float> changeEvent) => OnGripSliderChange(changeEvent, HandType.Right);
        void OnLeftGripChange(ChangeEvent<float> changeEvent) => OnGripSliderChange(changeEvent, HandType.Left);
        void OnGripSliderChange(ChangeEvent<float> changeEvent, HandType side)
        {
            var context = GameObject.FindFirstObjectByType<XrContext>();
            if (context == null) return;

            var element = (Slider)changeEvent.target;
            element.label = $"Grip {changeEvent.newValue}";

            var handController = context.GetController(side);
            handController.GripValue = changeEvent.newValue;
        }
        #endregion

        #region updateTrigger

        void OnLeftTriggerChange(ChangeEvent<float> changeEvent) => OnTriggerSliderChange(changeEvent, HandType.Left);
        void OnRightTriggerChange(ChangeEvent<float> changeEvent) => OnTriggerSliderChange(changeEvent, HandType.Right);
        void OnTriggerSliderChange(ChangeEvent<float> changeEvent, HandType side)
        {
            var context = GameObject.FindFirstObjectByType<XrContext>();
            if (context == null) return;

            var element = (Slider)changeEvent.target;
            element.label = $"Trigger {changeEvent.newValue}";

            var handController = context.GetController(side);
            handController.TriggerValue = changeEvent.newValue;
        }
        #endregion

        #region mainButton
        void OnRightMainButton() => OnMainButtonPressed(HandType.Right);
        void OnLeftMainButton() => OnMainButtonPressed(HandType.Left);
        void OnMainButtonPressed(HandType HandType)
        {
            var context = GameObject.FindFirstObjectByType<XrContext>();
            if (context == null) return;
            var controller = context.GetController(HandType);

            controller.OnMainButtonDown();
        }
        #endregion

        #region secondaryButton
        void OnRightSecondaryButton() => OnSecondaryButtonPressed(HandType.Right);
        void OnLeftSecondaryButton() => OnSecondaryButtonPressed(HandType.Left);
        void OnSecondaryButtonPressed(HandType HandType)
        {
            var context = GameObject.FindFirstObjectByType<XrContext>();
            if (context == null) return;
            var controller = context.GetController(HandType);

            controller.OnSecondaryButtonDown();
        }
        #endregion

        void AccessRightHand(bool frameObject) => AccessHandCore(HandType.Right, frameObject);
        void AccessLeftHand(bool frameObject) => AccessHandCore(HandType.Left, frameObject);

        void AccessHandCore(HandType HandType, bool frameObject)
        {
            var context = GameObject.FindFirstObjectByType<XrContext>();
            var hand = context?.GetController(HandType);
            if (hand != null)
            {
                Selection.activeTransform = hand.transform;
                if (frameObject)
                {
                    SceneView.lastActiveSceneView.FrameSelected();
                }
            }

            CreatePanelContent();
        }
    }
}
