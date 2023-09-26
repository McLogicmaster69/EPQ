using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// Stores information about an animal controller that can be saved
    /// </summary>
    [System.Serializable]
    public class ControllerDataFile
    {
        /// <summary>
        /// The ID of the animal
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The index of the profile in compiled data files
        /// </summary>
        public int ProfileIndex { get; set; }

        /// <summary>
        /// How many ticks until the animal dies of starvation
        /// </summary>
        public int TicksOfFood { get; set; }

        /// <summary>
        /// How many ticks until the plant grows
        /// </summary>
        public int TicksToRegrow { get; set; }

        /// <summary>
        /// How many ticks until the animal can move
        /// </summary>
        public int TicksToMove { get; set; }

        /// <summary>
        /// How many ticks since the animal last moved randomly
        /// </summary>
        public int TicksSinceLastRandom { get; set; }

        /// <summary>
        /// If the plant is available to eat
        /// </summary>
        public bool IsGrown { get; set; }

        /// <summary>
        /// A sorted list of what the animal can eat
        /// </summary>
        public int[] SortedEat { get; set; }

        /// <summary>
        /// How much food has been collected
        /// </summary>
        public int FoodCollectedToMate { get; set; }

        /// <summary>
        /// The index of the type of animal being hunted in SortedEat
        /// </summary>
        public int CurrentTargetIndexInEat { get; set; }

        /// <summary>
        /// Index of the target being hunted
        /// </summary>
        public int IndexOfTarget { get; set; }

        /// <summary>
        /// Index of the attempted mate
        /// </summary>
        public int IndexOfMate { get; set; }

        /// <summary>
        /// The current X coordinate of the animal
        /// </summary>
        public int CurrentPositionX { get; set; }

        /// <summary>
        /// The current Y coordinate of the animal
        /// </summary>
        public int CurrentPositionY { get; set; }

        /// <summary>
        /// The current X coordinate of the target
        /// </summary>
        public int TagetPositionX { get; set; }

        /// <summary>
        /// The current Y coordinate of the target
        /// </summary>
        public int TagetPositionY { get; set; }
    }
}