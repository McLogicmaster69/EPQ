using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    [System.Serializable]
    public class CompiledAnimalDataFile
    {
        public int ID;
        public int[] CanEat;
        public AnimalAttributes Attributes;

        public float r;
        public float g;
        public float b;

        public CompiledAnimalDataFile(CompiledAnimalProfile profile)
        {
            ID = profile.ID;
            CanEat = profile.CanEat.ToArray();
            Attributes = new AnimalAttributes(profile.Attributes);
            r = profile.Color.r;
            g = profile.Color.g;
            b = profile.Color.b;
        }
    }
}