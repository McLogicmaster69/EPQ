using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Meshes
{
    public class MeshGroup
    {
        public List<SquareMesh> meshes;

        private int totalVerts = 0;
        private const int MaxVerts = 50000;
        private int Layer = 0;
        private Material Material = null;
        private float Border = 0f;

        public MeshGroup()
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh());
        }
        public MeshGroup(int layer)
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh());
            Layer = layer;
        }
        public MeshGroup(Material material)
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh());
            Material = material;
        }
        public MeshGroup(int layer, Material material)
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh());
            Layer = layer;
            Material = material;
        }
        public MeshGroup(float border)
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh(border));
            Border = border;
        }
        public MeshGroup(int layer, float border)
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh(border));
            Layer = layer;
            Border = border;
        }
        public MeshGroup(float border, Material material)
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh(border));
            Material = material;
            Border = border;
        }
        public MeshGroup(int layer, float border, Material material)
        {
            meshes = new List<SquareMesh>();
            meshes.Add(new SquareMesh(border));
            Layer = layer;
            Material = material;
            Border = border;
        }

        public void AddCube(int X, int Z)
        {
            if (totalVerts > MaxVerts)
            {
                totalVerts -= MaxVerts;
                meshes.Add(new SquareMesh(Border));
            }
            meshes[meshes.Count - 1].AddCube(X, Z);
            totalVerts += 6;
        }
        public List<GameObject> BuildMeshes()
        {
            List<GameObject> objects = new List<GameObject>();
            foreach (SquareMesh mesh in meshes)
            {
                GameObject o = GameObject.CreatePrimitive(PrimitiveType.Plane);
                objects.Add(o);
                o.GetComponent<MeshFilter>().mesh = mesh.Mesh;
                o.GetComponent<MeshCollider>().sharedMesh = mesh.Mesh;
                o.layer = Layer;
                o.transform.position = new Vector3(-0.5f, 0, -0.5f);
                if (Material != null)
                    o.GetComponent<MeshRenderer>().material = Material;
            }
            return objects;
        }
        public List<GameObject> BuildMeshes(Transform parent)
        {
            List<GameObject> objects = new List<GameObject>();
            foreach (SquareMesh mesh in meshes)
            {
                GameObject o = GameObject.CreatePrimitive(PrimitiveType.Plane);
                objects.Add(o);
                o.transform.parent = parent;
                o.GetComponent<MeshFilter>().mesh = mesh.Mesh;
                o.GetComponent<MeshCollider>().sharedMesh = mesh.Mesh;
                o.layer = Layer;
                o.transform.position = new Vector3(-0.5f, 0, -0.5f);
                if (Material != null)
                    o.GetComponent<MeshRenderer>().material = Material;
            }
            return objects;
        }
    }
}