using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EPQ.Animals
{
    [System.Serializable]
    public class AttributeSlider
    {
        public int MinValue = 1;
        public int MaxValue = 10;

        public TMP_InputField Text;
        public Slider Slider;

        private Action<int> UpdateValue;

        public void InitSlider(Action<int> updateMethod)
        {
            UpdateValue = updateMethod;
            if(MinValue >= MaxValue)
                Debug.LogError("MIN VALUE IS GREATER OR EQUAL TO THE MAX VALUE");
            Slider.minValue = MinValue;
            Slider.maxValue = MaxValue;
        }
        public void SetValue(int value)
        {
            int v = Mathf.Clamp(value, MinValue, MaxValue);
            Text.text = v.ToString();
            Slider.value = v;
        }

        public void ChangeValue(Single value)
        {
            int num = System.Convert.ToInt32(value);
            if (num.ToString() != Text.text)
            {
                Text.text = num.ToString();
                UpdateValue(num);
            }
        }
        public void ChangeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int num = Mathf.Clamp(System.Convert.ToInt32(text), MinValue, MaxValue);
            Slider.value = num;
            UpdateValue(num);
        }
    }
}