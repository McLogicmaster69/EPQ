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
    public class AnimalProfileManager : MonoBehaviour
    {
        public int Index;
        public TMP_InputField Name;
        public Image ColorCode;

        public void SetupWithProfile(AnimalProfile profile)
        {
            SetName(profile.Name);
            ColorCode.color = profile.ColorCode;
            profile.OnColorChange += OnColorChange;
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

        public void OnColorChange(object sender, ColorChangeArgs e)
        {
            ColorCode.color = e.NewColor;
        }
    }
}