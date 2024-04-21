using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.DevTools.UI
{
    public static class UiExtensions
    {
        public static Label AddHeader(this VisualElement element, string text)
        {
            var headerLabel = new Label(text);
            headerLabel.style.fontSize = 18;
            element.Add(headerLabel);
            return headerLabel;
        }

        public static Label AddSubtitle(this VisualElement element, string text)
        {
            var subtitleLabel = new Label(text);
            subtitleLabel.style.fontSize = 14;
            subtitleLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            element.Add(subtitleLabel);
            return subtitleLabel;
        }

        public static Button AddButton(this VisualElement element, string text, Action action)
        {
            var button = new Button();
            button.text = text;
            button.clicked += action;
            element.Add(button);

            return button;
        }

        #region slider
        public static Slider AddSlider(this VisualElement element, string text, float maxValue, float minValue, EventCallback<ChangeEvent<float>> action, float? value = null)
        {
            var slider = CreateSlider(text, maxValue, minValue);
            slider.RegisterCallback(action);
            if (value != null)
            {
                slider.value = (float)value;
            }
            element.Add(slider);
            return slider;
        }

        public static Slider AddSlider(this VisualElement element, string text, float maxValue, float minValue, SerializedProperty property, EventCallback<ChangeEvent<float>> action)
        {
            var slider = CreateSlider(text, maxValue, minValue);
            slider.BindProperty(property);
            slider.RegisterCallback(action);

            element.Add(slider);
            return slider;
        }

        public static Slider AddSlider(this VisualElement element, string text, float maxValue, float minValue, SerializedProperty property)
        {
            var slider = CreateSlider(text, maxValue, minValue);
            slider.BindProperty(property);

            element.Add(slider);
            return slider;
        }

        public static Slider AddSlider(this VisualElement element, string text, float maxValue, float minValue)
        {
            var slider = CreateSlider(text, maxValue, minValue);

            element.Add(slider);
            return slider;
        }

        static Slider CreateSlider(string text, float maxValue, float minValue)
        {
            var slider = new Slider();
            slider.label = text;
            slider.lowValue = minValue;
            slider.value = minValue;
            slider.highValue = maxValue;
            return slider;
        }
        #endregion

    }
}
