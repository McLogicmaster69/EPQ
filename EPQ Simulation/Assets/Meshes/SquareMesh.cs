using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Meshes
{
    /// <summary>
    /// Holds the information about a square mesh
    /// </summary>
    public sealed class SquareMesh
    {
        private List<Vector3> _verts;
        private List<int> _tris;
        public int Cubes { get; private set; } = 0;
        private int CubesAdd { get { return Cubes * 4; } }
        public Vector3[] Vertices { get { return _verts.ToArray(); } }
        public int[] Triangles { get { return _tris.ToArray(); } }

        private float Border = 0f;

        public Mesh Mesh
        {
            get
            {
                Mesh m = new Mesh
                {
                    vertices = _verts.ToArray(),
                    triangles = _tris.ToArray()
                };
                m.RecalculateNormals();
                m.RecalculateBounds();
                return m;
            }
        }
        public SquareMesh()
        {
            _verts = new List<Vector3>();
            _tris = new List<int>();
        }
        public SquareMesh(float border)
        {
            _verts = new List<Vector3>();
            _tris = new List<int>();
            Border = border;
        }
        public void AddCube(int X, int Z)
        {
            List<Vector3> vlist = new List<Vector3>
            {
                new Vector3(X + Border, 1, Z + (1 - Border)),
                new Vector3(X + (1 - Border), 1, Z + (1 - Border)),
                new Vector3(X + (1 - Border), 1, Z + Border),
                new Vector3(X + Border, 1, Z + Border),
            };
            List<int> ilist = new List<int>
            {
                CubesAdd + 0, CubesAdd + 1, CubesAdd + 2,
                CubesAdd + 0, CubesAdd + 2, CubesAdd + 3,
            };
            AddToVerts(vlist);
            AddToTris(ilist);
            Cubes++;
        }
        private void AddToVerts(List<Vector3> list)
        {
            foreach(Vector3 v in list)
            {
                _verts.Add(v);
            }
        }
        private void AddToTris(List<int> list)
        {
            foreach(int i in list)
            {
                _tris.Add(i);
            }
        }
    }
}