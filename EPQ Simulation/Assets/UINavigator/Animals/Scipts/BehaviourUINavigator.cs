using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EPQ.Animals
{
    /// <summary>
    /// Manages the UI related to the behaviour of an animal
    /// </summary>
    public class BehaviourUINavigator : MonoBehaviour
    {
        public static BehaviourUINavigator main;

        [SerializeField] private GameObject BehaviourUI;
        [SerializeField] private GameObject AnimalToggle;
        [SerializeField] private GameObject PlantToggle;
        [SerializeField] private GameObject AnimalAttributes;
        [SerializeField] private GameObject PlantAttributes;
        [SerializeField] private Toggle RequiresMateToggle;

        [SerializeField] private Color AnimalColor;
        [SerializeField] private Color PlantColor;

        [SerializeField] private HelpMessage[] HelpMessages;
        [SerializeField] private AttributeSlider[] Sliders;

        public Color ColorAnimal { get { return AnimalColor; } }
        public Color ColorPlant { get { return PlantColor; } }

        private AnimalProfile animalProfile;
        private System.Action<int>[] UpdateMethods = new System.Action<int>[] { 
            (int value) => { main.animalProfile.Attributes.FoodValue = value; },
            (int value) => { main.animalProfile.Attributes.GrassSpeed = value; },
            (int value) => { main.animalProfile.Attributes.WaterSpeed = value; },
            (int value) => { main.animalProfile.Attributes.DetectionRange = value; },
            (int value) => { main.animalProfile.Attributes.HelpRange = value; },
            (int value) => { main.animalProfile.Attributes.FoodRequirement = value; },
            (int value) => { main.animalProfile.Attributes.FoodToReproduce = value; },
            (int value) => { main.animalProfile.Attributes.TimeToRegrow = value; },
        };

        private void Awake()
        {
            main = this;
        }
        private void Start()
        {
            InitUI();
        }

        private void InitUI()
        {
            AnimalAttributes.SetActive(true);
            PlantAttributes.SetActive(false);
            BehaviourUI.SetActive(false);
        }
        private void InitSliderValues()
        {
            for (int i = 0; i < Sliders.Length; i++)
            {
                Sliders[i].InitSlider(UpdateMethods[i]);
            }
            Sliders[0].SetValue(animalProfile.Attributes.FoodValue);
            Sliders[1].SetValue(animalProfile.Attributes.GrassSpeed);
            Sliders[2].SetValue(animalProfile.Attributes.WaterSpeed);
            Sliders[3].SetValue(animalProfile.Attributes.DetectionRange);
            Sliders[4].SetValue(animalProfile.Attributes.HelpRange);
            Sliders[5].SetValue(animalProfile.Attributes.FoodRequirement);
            Sliders[6].SetValue(animalProfile.Attributes.FoodToReproduce);
            Sliders[7].SetValue(animalProfile.Attributes.TimeToRegrow);
        }

        public void OpenUI(AnimalProfile profile)
        {
            animalProfile = profile;
            BehaviourUI.SetActive(true);
            InitUI(profile);
            InitSliderValues();
        }
        public void CloseUI()
        {
            BehaviourUI.SetActive(false);
        }
        private void InitUI(AnimalProfile profile)
        {
            AnimalToggle.SetActive(animalProfile.IsAnimal);
            PlantToggle.SetActive(!animalProfile.IsAnimal);
            AnimalAttributes.SetActive(animalProfile.IsAnimal);
            PlantAttributes.SetActive(!animalProfile.IsAnimal);
            RequiresMateToggle.isOn = animalProfile.Attributes.RequiresMate;
        }

        public void ToggleAnimal()
        {
            animalProfile.ToggleAnimal(animalProfile.IsAnimal ? PlantColor : AnimalColor);
            AnimalToggle.SetActive(animalProfile.IsAnimal);
            PlantToggle.SetActive(!animalProfile.IsAnimal);
            AnimalAttributes.SetActive(animalProfile.IsAnimal);
            PlantAttributes.SetActive(!animalProfile.IsAnimal);
        }
        public void AttributeHelp(int index)
        {
            BehaviourHelpUINavigator.main.ShowUI(HelpMessages[index]);
        }

        public void UpdateFoodValueSlider(System.Single value)
        {
            Sliders[0].ChangeValue(value);
        }
        public void UpdateFoodValueText(string value)
        {
            Sliders[0].ChangeText(value);
        }
        public void UpdateGrassSpeedSlider(System.Single value)
        {
            Sliders[1].ChangeValue(value);
        }
        public void UpdateGrassSpeedText(string value)
        {
            Sliders[1].ChangeText(value);
        }
        public void UpdateWaterSpeedSlider(System.Single value)
        {
            Sliders[2].ChangeValue(value);
        }
        public void UpdateWaterSpeedText(string value)
        {
            Sliders[2].ChangeText(value);
        }
        public void UpdateDetectionRangeSlider(System.Single value)
        {
            Sliders[3].ChangeValue(value);
        }
        public void UpdateDetectionRangeText(string value)
        {
            Sliders[3].ChangeText(value);
        }
        public void UpdateHelpRangeSlider(System.Single value)
        {
            Sliders[4].ChangeValue(value);
        }
        public void UpdateHelpRangeText(string value)
        {
            Sliders[4].ChangeText(value);
        }
        public void UpdateDaysWithoutFoodSlider(System.Single value)
        {
            Sliders[5].ChangeValue(value);
        }
        public void UpdateDaysWithoutFoodText(string value)
        {
            Sliders[5].ChangeText(value);
        }
        public void UpdateFoodToReproduceSlider(System.Single value)
        {
            Sliders[6].ChangeValue(value);
        }
        public void UpdateFoodToReproduceText(string value)
        {
            Sliders[6].ChangeText(value);
        }
        public void UpdateTimeToRegrowSlider(System.Single value)
        {
            Sliders[7].ChangeValue(value);
        }
        public void UpdateTimeToRegrowText(string value)
        {
            Sliders[7].ChangeText(value);
        }
        public void ToggleRequiresMate(bool value)
        {
            animalProfile.Attributes.RequiresMate = value;
        }
    }
}