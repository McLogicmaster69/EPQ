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
        public static readonly int TICKS_IN_DAY = 50;
        public static readonly int STARTING_FOOD = 3;
        public static readonly int MAX_FOOD = 10;

        public static WorldManager main;

        public bool ActiveSimulation { get; private set; } = false;

        public GameObject ActiveSimulationText;
        public GameObject Camera;
        public GameObject AnimalControllerObject;
        public Material[] Materials;
        public Transform GroundParent;
        public Transform AnimalParent;
        public Material BlankMaterial;

        private World<int> GroundWorld;
        private World<int> AnimalWorld;
        private World<AnimalController> ControllersGrid;
        private List<CompiledAnimalProfile> AnimalProfiles;
        private List<GameObject> ActiveAnimals = new List<GameObject>();

        private List<GameObject> activeWorldObjects = new List<GameObject>();
        private List<AnimalController> Controllers = new List<AnimalController>();
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
            SpawnAnimals(animal);

            ActiveSimulation = true;
            ActiveSimulationText.SetActive(false);
            ResetCameraPosition(ground.X / 2f);

            DisplayWorld();
        }
        private void SpawnAnimals(World<int> animals)
        {
            for (int i = 0; i < ActiveAnimals.Count; i++)
            {
                Destroy(ActiveAnimals[i]);
            }
            ActiveAnimals = new List<GameObject>();

            ControllersGrid = new World<AnimalController>(animals.X, animals.Y);
            Controllers = new List<AnimalController>();
            for (int x = 0; x < animals.X; x++)
            {
                for (int y = 0; y < animals.Y; y++)
                {
                    int cellID = animals.GetCell(x, y);
                    if (cellID != -1)
                    {
                        AnimalController controller = SpawnAnimal(cellID, new Vector2Int(x, y));
                        Controllers.Add(controller);
                        ControllersGrid.SetCell(x, y, controller);
                    }
                }
            }
        }
        private AnimalController SpawnAnimal(int index, Vector2Int position)
        {
            GameObject o = Instantiate(AnimalControllerObject, AnimalParent);
            ActiveAnimals.Add(o);
            AnimalController controller = o.GetComponent<AnimalController>();
            controller.InitAnimal(index, position, AnimalProfiles[index]);
            return controller;
        }
        private void DisplayWorld()
        {
            DestroyWorld();

            MeshGroup[] meshGroups = new MeshGroup[Materials.Length];
            for (int i = 0; i < meshGroups.Length; i++)
            {
                meshGroups[i] = new MeshGroup(0.05f, Materials[i]);
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
        public CompiledAnimalProfile GetProfile(int index) => AnimalProfiles[index];

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

        public void MoveAnimal(int startX, int startY, int endX, int endY)
        {
            if(GroundWorld.GetCell(endX, endY) != -1)
            {
                ControllersGrid.GetCell(endX, endY).Kill();
            }
            GroundWorld.MoveValue(startX, startY, endX, endY);
            ControllersGrid.MoveValue(startX, startY, endX, endY);
        }
        public void SetAnimalCell(int x, int y, int ID, AnimalController controller)
        {
            AnimalWorld.SetCell(x, y, ID);
            ControllersGrid.SetCell(x, y, controller);
        }
        public int GetGroundCell(int x, int y)
        {
            return GroundWorld.GetCell(x, y);
        }
        public int GetAnimalCell(int x, int y)
        {
            return AnimalWorld.GetCell(x, y);
        }
    }
}