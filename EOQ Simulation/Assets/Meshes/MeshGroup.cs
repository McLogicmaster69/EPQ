using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Meshes
{
    public class MeshGroup : MonoBehaviour
    {
        public List<SquareMesh> meshes;
        private int totalVerts = 0;
        private const int MaxVerts = 50000;
        public MeshGroup()
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh());
        }
        public void AddCube(int X, int Z)
        {
            if (totalVerts > MaxVerts)
            {
                totalVerts -= MaxVerts;
                meshes.Add(new SquareMesh());
            }
            meshes[meshes.Count - 1].AddCube(X, Z);
            totalVerts += 6;
        }
    }
}