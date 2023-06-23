using EPQ.Animals;
using EPQ.Meshes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EPQ.Clock;

namespace EPQ.Worlds
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager main;

        public bool ActiveSimulation { get; private set; } = false;

        public GameObject ActiveSimulationText;
        public GameObject Camera;
        public Material[] Materials;
        public Transform GroundParent;
        public Transform AnimalParent;

        private World<int> GroundWorld;
        private World<int> AnimalWorld;
        private List<CompiledAnimalProfile> AnimalProfiles;

        private List<GameObject> activeWorldObjects = new List<GameObject>();

        private void Awake()
        {
            main = this;
        }

        public void SetupWorld(World<int> ground, World<int> animal, List<CompiledAnimalProfile> profiles)
        {
            GroundWorld = ground;
            AnimalWorld = animal;
            AnimalProfiles = profiles;

            Clock.main.Tick += Tick;

            ActiveSimulation = true;
            ActiveSimulationText.SetActive(false);
            Camera.transform.position = new Vector3(0f, 40f, 0f);
        }
        public void DisplayWorld()
        {
            DestroyWorld();

            MeshGroup[] meshGroups = new MeshGroup[Materials.Length];
            for (int i = 0; i < meshGroups.Length; i++)
            {
                meshGroups[i] = new MeshGroup(Materials[i]);
            }

            for (int y = 0; y < GroundWorld.Y; y++)
            {
                for (int x = 0; x < GroundWorld.X; x++)
                {
                    meshGroups[GroundWorld.GetCell(x, y)].AddCube(x, y);
                }
            }

            for (int i = 0; i < meshGroups.Length; i++)
            {
                foreach(GameObject o in meshGroups[i].BuildMeshes(GroundParent))
                {
                    activeWorldObjects.Add(o);
                }
            }
        }

        public void Tick(object sender, TickEventArgs e)
        {

        }

        private void DestroyWorld()
        {
            for (int i = 0; i < activeWorldObjects.Count; i++)
            {
                Destroy(activeWorldObjects[i]);
            }
            activeWorldObjects = new List<GameObject>();
        }
    }
}