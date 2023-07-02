using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EPQ.Colors
{
    public class ColorCodeManager : MonoBehaviour
    {
        public int CurrentIndex;
        public Slider[] Sliders;
        public TMP_InputField[] Numbers;

        public void ChangeRSliderValue(System.Single value)
        {
            int r = System.Convert.ToInt32(value);
            if (r.ToString() != Numbers[0].text)
            {
                Numbers[0].text = r.ToString();
                UpdateRValue(r);
            }
        }
        public void ChangeRText(string text)
        {
            int r = Mathf.Clamp(System.Convert.ToInt32(text), 0, 255);
            Sliders[0].value = r;
            UpdateRValue(r);
        }
        private void UpdateRValue(int value)
        {
            AnimalUINavigator.main.Profiles[CurrentIndex].SetRColor(value / 255f);
        }
        public void ChangeGSliderValue(System.Single value)
        {
            int g = System.Convert.ToInt32(value);
            Numbers[1].text = g.ToString();
            UpdateGValue(g);
        }
        public void ChangeGText(string text)
        {
            int g = Mathf.Clamp(System.Convert.ToInt32(text), 0, 255);
            Sliders[1].value = g;
            UpdateGValue(g);
        }
        private void UpdateGValue(int value)
        {
            AnimalUINavigator.main.Profiles[CurrentIndex].SetGColor(value / 255f);
        }
        public void ChangeBSliderValue(System.Single value)
        {
            int b = System.Convert.ToInt32(value);
            Numbers[2].text = b.ToString();
            UpdateBValue(b);
        }
        public void ChangeBText(string text)
        {
            int b = Mathf.Clamp(System.Convert.ToInt32(text), 0, 255);
            Sliders[2].value = b;
            UpdateBValue(b);
        }
        private void UpdateBValue(int value)
        {
            AnimalUINavigator.main.Profiles[CurrentIndex].SetBColor(value / 255f);
        }

        public void SetupColor(Color color)
        {
            Numbers[0].text = Mathf.FloorToInt(color.r * 255).ToString();
            Numbers[1].text = Mathf.FloorToInt(color.g * 255).ToString();
            Numbers[2].text = Mathf.FloorToInt(color.b * 255).ToString();
        }

        public void CloseUI()
        {
            AnimalUINavigator.main.CloseColorCodeUI();
        }
    }
}