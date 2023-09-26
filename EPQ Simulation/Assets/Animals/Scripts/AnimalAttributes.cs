using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    /// <summary>
    /// Contains attributes describing how an animal should behave
    /// </summary>
    [System.Serializable]
    public class AnimalAttributes
    {
        /// <summary>
        /// If it is an animal or a plant
        /// </summary>
        public bool IsAnimal { get; set; } = true;

        /// <summary>
        /// How much food other animals get by eating this one
        /// </summary>
        public int FoodValue { get; set; } = 2;

        /// <summary>
        /// How fast the animal moves on grass
        /// </summary>
        public int GrassSpeed { get; set; } = 6;

        /// <summary>
        /// How fast the animal moves in water
        /// </summary>
        public int WaterSpeed { get; set; } = 3;

        /// <summary>
        /// The range that the animal can detect its prey
        /// </summary>
        public int DetectionRange { get; set; } = 20;

        /// <summary>
        /// The range it can call for help hunting its prey
        /// </summary>
        public int HelpRange { get; set; } = 10;

        /// <summary>
        /// How much food the animal needs to survive
        /// </summary>
        public int FoodRequirement { get; set; } = 5;

        /// <summary>
        /// If the animal requires a mate to reproduce
        /// </summary>
        public bool RequiresMate { get; set; } = true;

        /// <summary>
        /// How much food the animal requires to reproduce
        /// </summary>
        public int FoodToReproduce { get; set; } = 3;

        /// <summary>
        /// How long the plant takes to regrow after being eaten
        /// </summary>
        public int TimeToRegrow { get; set; } = 20;

        /// <summary>
        /// Creates a class with default values
        /// </summary>
        public AnimalAttributes()
        {
        }

        /// <summary>
        /// Creates a class with copied values from ‘attributes’
        /// </summary>
        /// <param name="attributes">The attributes being copied</param>
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