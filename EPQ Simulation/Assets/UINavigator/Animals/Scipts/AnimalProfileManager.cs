using EPQ.Colors;
using EPQ.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EPQ.Animals.AnimalProfile;

namespace EPQ.Animals
{
    /// <summary>
    /// Manages the UI display of an animal profile
    /// </summary>
    public class AnimalProfileManager : MonoBehaviour
    {
        public int Index;
        public TMP_InputField Name;
        public Image ColorCode;
        public Image BackgroundColor;
        public AttributeSlider BatchSizeSlider;

        /// <summary>
        /// Setups the manager and relates it to a given profile
        /// </summary>
        /// <param name="profile">The profile managed by the class</param>
        public void SetupWithProfile(AnimalProfile profile)
        {
            SetName(profile.Name);
            ColorCode.color = profile.ColorCode;
            BackgroundColor.color = profile.IsAnimal ? BehaviourUINavigator.main.ColorAnimal : BehaviourUINavigator.main.ColorPlant;
            profile.OnColorChange += OnColorChange;
            profile.OnAnimalChange += OnAnimalToggle;
            BatchSizeSlider.InitSlider((int value) => { AnimalUINavigator.main.Profiles[Index].BatchSize = value; });
            BatchSizeSlider.SetValue(profile.BatchSize);
        }

        public void ChangeColor()
        {
            AnimalUINavigator.main.OpenColorCodeUI(Index);
        }
        public void SetName(string name)
        {
            Name.text = name;
        }
        public void ChangeName(string name)
        {
            if (!string.IsNullOrEmpty(name))
                AnimalUINavigator.main.Profiles[Index].Name = name;
            else
                SetName(AnimalUINavigator.main.Profiles[Index].Name);
        }
        public void FindProfile()
        {
            UINavigator.main.FindNode(Index);
        }
        public void DeleteProfile()
        {
            AnimalUINavigator.main.DeleteAnimal(Index);
        }
        public void BehaviourUI()
        {
            AnimalUINavigator.main.OpenBehaviourUI(Index);
        }

        public void OnColorChange(object sender, ColorChangeArgs e)
        {
            ColorCode.color = e.NewColor;
        }
        public void OnAnimalToggle(object send, ColorChangeArgs e)
        {
            BackgroundColor.color = e.NewColor;
        }

        public void ChangeBatchSizeSlider(System.Single value)
        {
            BatchSizeSlider.ChangeValue(value);
        }
        public void ChangeBatchSizeText(string text)
        {
            BatchSizeSlider.ChangeText(text);
        }
    }
}