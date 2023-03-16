using EPQ.Meshes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Land
{
    public class Generation : MonoBehaviour
    {
        public int Size = 1000;
        public List<SquareMesh> Meshes;
        private void Start()
        {
            Generate(new GenerationInfo());
        }
        private void Generate(GenerationInfo info)
        {

        }
    }
}