using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Meshes
{
    /// <summary>
    /// Creates the shape of the world where the simulation will take place
    /// </summary>
    public class MeshGroup
    {
        private List<SquareMesh> _meshes;
        private int _totalVerts = 0;
        private int _layer = 0;
        private Material _material = null;
        private float _border = 0f;

        private const int MAX_VERTS = 50000;

        public MeshGroup()
        {
            _meshes = new List<SquareMesh>();
            _meshes.Add(new SquareMesh());
        }
        public MeshGroup(int layer)
        {
            _meshes = new List<SquareMesh>();
            _meshes.Add(new SquareMesh());
            _layer = layer;
        }
        public MeshGroup(Material material)
        {
            _meshes = new List<SquareMesh>();
            _meshes.Add(new SquareMesh());
            _material = material;
        }
        public MeshGroup(int layer, Material material)
        {
            _meshes = new List<SquareMesh>();
            _meshes.Add(new SquareMesh());
            _layer = layer;
            _material = material;
        }
        public MeshGroup(float border)
        {
            _meshes = new List<SquareMesh>();
            _meshes.Add(new SquareMesh(border));
            _border = border;
        }
        public MeshGroup(int layer, float border)
        {
            _meshes = new List<SquareMesh>();
            _meshes.Add(new SquareMesh(border));
            _layer = layer;
            _border = border;
        }
        public MeshGroup(float border, Material material)
        {
            _meshes = new List<SquareMesh>();
            _meshes.Add(new SquareMesh(border));
            _material = material;
            _border = border;
        }
        public MeshGroup(int layer, float border, Material material)
        {
            _meshes = new List<SquareMesh>();
            _meshes.Add(new SquareMesh(border));
            _layer = layer;
            _material = material;
            _border = border;
        }

        public void AddCube(int X, int Z)
        {
            if (_totalVerts > MAX_VERTS)
            {
                _totalVerts -= MAX_VERTS;
                _meshes.Add(new SquareMesh(_border));
            }
            _meshes[_meshes.Count - 1].AddCube(X, Z);
            _totalVerts += 6;
        }
        public List<GameObject> BuildMeshes()
        {
            List<GameObject> objects = new List<GameObject>();
            foreach (SquareMesh mesh in _meshes)
            {
                GameObject o = GameObject.CreatePrimitive(PrimitiveType.Plane);
                objects.Add(o);
                o.GetComponent<MeshFilter>().mesh = mesh.Mesh;
                o.GetComponent<MeshCollider>().sharedMesh = mesh.Mesh;
                o.layer = _layer;
                o.transform.position = new Vector3(-0.5f, 0, -0.5f);
                if (_material != null)
                    o.GetComponent<MeshRenderer>().material = _material;
            }
            return objects;
        }
        public List<GameObject> BuildMeshes(Transform parent)
        {
            List<GameObject> objects = new List<GameObject>();
            foreach (SquareMesh mesh in _meshes)
            {
                GameObject o = GameObject.CreatePrimitive(PrimitiveType.Plane);
                objects.Add(o);
                o.transform.parent = parent;
                o.GetComponent<MeshFilter>().mesh = mesh.Mesh;
                o.GetComponent<MeshCollider>().sharedMesh = mesh.Mesh;
                o.layer = _layer;
                o.transform.position = new Vector3(-0.5f, 0, -0.5f);
                if (_material != null)
                    o.GetComponent<MeshRenderer>().material = _material;
            }
            return objects;
        }
    }
}