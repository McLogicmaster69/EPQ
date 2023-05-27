using EPQ.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EPQ.Animals
{
    public class AnimalProfileManager : MonoBehaviour
    {
        public int Index;
        public TMP_InputField Name;

        public void SetupWithProfile(AnimalProfile profile)
        {
            SetName(profile.Name);
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
    }
}