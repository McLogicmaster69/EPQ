using EPQ.Animals;
using EPQ.Data;
using EPQ.Files;
using EPQ.Meshes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EPQ.Clock;

namespace EPQ.Worlds
{
    public class WorldManager : MonoBehaviour
    {
        public static readonly int TICKS_IN_DAY = 25;
        public static readonly int STARTING_FOOD = 3;
        public static readonly int MAX_FOOD = 10;

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
        private const float CAMERA_HEIGHT = 160f;

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
            ResetCameraPosition(ground.X / 2f);
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

        public void SaveData(out CompiledAnimalDataFile[] animalData, out WorldDataFile<int> ground, out WorldDataFile<int> animals)
        {
            CompiledAnimalDataFile[] profiles = new CompiledAnimalDataFile[AnimalProfiles.Count];
            for (int i = 0; i < AnimalProfiles.Count; i++)
            {
                profiles[i] = new CompiledAnimalDataFile(AnimalProfiles[i]);
            }

            animalData = profiles;
            ground = new WorldDataFile<int>(GroundWorld);
            animals = new WorldDataFile<int>(AnimalWorld);
        }
        public void LoadDataV1(SimulationDataFile data)
        {
            for (int i = 0; i < data.Profiles.Length; i++)
            {
                AnimalProfiles.Add(new CompiledAnimalProfile(data.Profiles[i]));
            }

            GroundWorld = new World<int>(data.Ground);
            AnimalWorld = new World<int>(data.Animals);
        }

        public void Tick(object sender, TickEventArgs e)
        {

        }

        private void ResetCameraPosition(float centre)
        {
            float x = centre - Mathf.Sqrt((CAMERA_HEIGHT * CAMERA_HEIGHT) / 2);
            Camera.transform.position = new Vector3(x, CAMERA_HEIGHT, x);
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