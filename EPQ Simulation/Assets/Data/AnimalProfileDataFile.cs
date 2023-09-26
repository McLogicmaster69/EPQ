using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// Contains saveable data about an animal profile. Class is serializable
    /// </summary>
    [System.Serializable]
    public class AnimalProfileDataFile
    {
        /// <summary>
        /// The ID of the animal
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the animal
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ratio of how many of this animal is initially spawned
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// The values of how the animal should behave
        /// </summary>
        public AnimalAttributes Attributes { get; set; }

        /// <summary>
        /// The red value of the animal
        /// </summary>
        public float r { get; set; }

        /// <summary>
        /// The green value of the animal
        /// </summary>
        public float g { get; set; }

        /// <summary>
        /// The blue value of the animal
        /// </summary>
        public float b { get; set; }
    }
}