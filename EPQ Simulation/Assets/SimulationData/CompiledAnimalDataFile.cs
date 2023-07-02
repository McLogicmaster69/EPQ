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

        public CompiledAnimalDataFile(CompiledAnimalProfile profile)
        {
            ID = profile.ID;
            CanEat = profile.CanEat.ToArray();
            Attributes = new AnimalAttributes(profile.Attributes);
        }
    }
}