using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    [System.Serializable]
    public class AnimalAttributes
    {
        public bool IsAnimal = true;
        public int FoodValue = 2;

        public int GrassSpeed = 6;
        public int WaterSpeed = 3;
        public int DetectionRange = 20;
        public int HelpRange = 10;
        public int FoodRequirement = 5;
        public bool RequiresMate = true;
        public int FoodToReproduce = 3;

        public int TimeToRegrow = 20;

        public AnimalAttributes()
        {
        }

        public AnimalAttributes(AnimalAttributes attributes)
        {
            IsAnimal = attributes.IsAnimal;
            FoodValue = attributes.FoodValue;
            GrassSpeed = attributes.GrassSpeed;
            WaterSpeed = attributes.WaterSpeed;
            DetectionRange = attributes.DetectionRange;
            HelpRange = attributes.HelpRange;
            FoodRequirement = attributes.FoodRequirement;
            RequiresMate = attributes.RequiresMate;
            FoodToReproduce = attributes.FoodToReproduce;
            TimeToRegrow = attributes.TimeToRegrow;
        }
    }
}