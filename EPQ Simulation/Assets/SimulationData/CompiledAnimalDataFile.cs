using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// A data file that contains compiled information about an animal that can be saved
    /// </summary>
    [System.Serializable]
    public class CompiledAnimalDataFile
    {
        /// <summary>
        /// The ID of the animal
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// What animals this one can eat
        /// </summary>
        public int[] CanEat { get; set; }

        /// <summary>
        /// What animals this one can be eaten by
        /// </summary>
        public int[] CanBeEatenBy { get; set; }

        /// <summary>
        /// The values controlling this animal’s behaviour
        /// </summary>
        public AnimalAttributes Attributes { get; set; }

        /// <summary>
        /// The red value of the color of the animal
        /// </summary>
        public float r { get; set; }

        /// <summary>
        /// The green value of the color of the animal
        /// </summary>
        public float g { get; set; }

        /// <summary>
        /// The blue value of the color of the animal
        /// </summary>
        public float b { get; set; }

        /// <summary>
        /// Creates a data file containing the compiled profile
        /// </summary>
        /// <param name="profile">The profile used to create the class</param>
        public CompiledAnimalDataFile(CompiledAnimalProfile profile)
        {
            ID = profile.ID;
            CanEat = profile.CanEat.ToArray();
            CanBeEatenBy = profile.CanBeEatenBy.ToArray();
            Attributes = new AnimalAttributes(profile.Attributes);
            r = profile.Color.r;
            g = profile.Color.g;
            b = profile.Color.b;
        }
    }
}