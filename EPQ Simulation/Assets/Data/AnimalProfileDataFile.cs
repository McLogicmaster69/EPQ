using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    [System.Serializable]
    public class AnimalProfileDataFile
    {
        public int ID;
        public string Name;
        public int BatchSize;
        public AnimalAttributes Attributes;

        public float r;
        public float g;
        public float b;
    }
}