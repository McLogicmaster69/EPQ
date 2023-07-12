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

        public World<int> GroundWorld { get; private set; }
        public World<int> AnimalWorld { get; private set; }
        public World<AnimalController> ControllersGrid { get; private set; }
        public List<CompiledAnimalProfile> AnimalProfiles { get; private set; }
        public List<AnimalController> Controllers { get; private set; } = new List<AnimalController>();

        private List<GameObject> ActiveAnimals = new List<GameObject>();

        private List<GameObject> activeWorldObjects = new List<GameObject>();
        private const float CAMERA_HEIGHT = 160f;
        private int CurrentID = 0;

        #region Unity
        private void Awake()
        {
            main = this;
        }
        private void Start()
        {
            Clock.main.Tick += Tick;
        }
        #endregion
        #region Setup
        public void SetupWorld(World<int> ground, World<int> animal, List<CompiledAnimalProfile> profiles)
        {
            GroundWorld = ground;
            AnimalWorld = animal;
            AnimalProfiles = profiles;

            SpawnAnimals(animal);

            ActiveSimulation = true;
            ActiveSimulationText.SetActive(false);
            ResetCameraPosition(ground.X / 2f);

            DisplayWorld();
        }
        private void SpawnAnimals(World<int> animals)
        {
            DestroyAllActiveAnimals();

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
        private void DestroyAllActiveAnimals()
        {
            for (int i = 0; i < ActiveAnimals.Count; i++)
            {
                Destroy(ActiveAnimals[i]);
            }
            ActiveAnimals = new List<GameObject>();
        }
        private AnimalController SpawnAnimal(int index, Vector2Int position)
        {
            GameObject o = Instantiate(AnimalControllerObject, AnimalParent);
            ActiveAnimals.Add(o);
            AnimalController controller = o.GetComponent<AnimalController>();
            controller.InitAnimal(index, position, AnimalProfiles[index], CurrentID);
            CurrentID++;
            return controller;
        }
        private AnimalController SpawnAnimal(ControllerDataFile data)
        {
            GameObject o = Instantiate(AnimalControllerObject, AnimalParent);
            ActiveAnimals.Add(o);
            AnimalController controller = o.GetComponent<AnimalController>();
            controller.InitAnimal(data);
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
                foreach (GameObject o in meshGroups[i].BuildMeshes(GroundParent))
                {
                    activeWorldObjects.Add(o);
                }
            }
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
        #endregion
        #region Tick
        public void Tick(object sender, TickEventArgs e)
        {
            for (int i = 0; i < Controllers.Count; i++)
            {
                try
                {
                    if (Controllers[i] != null)
                        Controllers[i].Tick();
                }
                catch (System.Exception)
                {

                }
            }
            DeleteExtraControllers();
            Clock.main.handledTick = true;
        }
        #endregion
        #region Animal Grid
        public void MoveAnimal(int startX, int startY, int endX, int endY)
        {
            AnimalWorld.MoveValue(startX, startY, endX, endY, -1);
            AnimalController controller = ControllersGrid.MoveValue(startX, startY, endX, endY, null);
            if (controller != null)
                controller.Kill();
        }
        public void MoveAnimal(int startX, int startY, int endX, int endY, AnimalController eater)
        {
            AnimalWorld.MoveValue(startX, startY, endX, endY, -1);
            AnimalController controller = ControllersGrid.MoveValue(startX, startY, endX, endY, null);
            if (controller != null)
                controller.Kill(eater);
        }
        public void SetAnimalCell(int x, int y, int index, AnimalController controller)
        {
            AnimalWorld.SetCell(x, y, index);
            ControllersGrid.SetCell(x, y, controller);
        }
        public int GetAnimalCell(int x, int y)
        {
            return AnimalWorld.GetCell(x, y);
        }
        public void DeleteAnimal(int x, int y)
        {
            AnimalWorld.SetCell(x, y, -1);
            ControllersGrid.SetCell(x, y, null);
        }
        public void CreateAnimal(int x, int y, int type)
        {
            AnimalController controller = SpawnAnimal(type, new Vector2Int(x, y));
            Controllers.Add(controller);
            ControllersGrid.SetCell(x, y, controller);
            AnimalWorld.SetCell(x, y, type);
        }
        #endregion
        #region Ground
        public int GetGroundCell(int x, int y)
        {
            return GroundWorld.GetCell(x, y);
        }
        #endregion
        #region Save and Load
        public void SaveData(out CompiledAnimalDataFile[] animalData, out ControllerDataFile[] controllersData, out WorldDataFile<int> ground, out WorldDataFile<int> animals, out int currentId)
        {
            Clock.main.IsTicking = false;
            DeleteExtraControllers();

            CompiledAnimalDataFile[] profiles = new CompiledAnimalDataFile[AnimalProfiles.Count];
            for (int i = 0; i < AnimalProfiles.Count; i++)
            {
                profiles[i] = new CompiledAnimalDataFile(AnimalProfiles[i]);
            }
            ControllerDataFile[] controllers = new ControllerDataFile[Controllers.Count];
            for (int i = 0; i < Controllers.Count; i++)
            {
                controllers[i] = Controllers[i].CreateDataFile();
            }

            animalData = profiles;
            controllersData = controllers;
            ground = new WorldDataFile<int>(GroundWorld);
            animals = new WorldDataFile<int>(AnimalWorld);
            currentId = CurrentID;
        }
        public void LoadDataV1(SimulationDataFile data)
        {
            DestroyAllActiveAnimals();

            for (int i = 0; i < data.Profiles.Length; i++)
            {
                AnimalProfiles.Add(new CompiledAnimalProfile(data.Profiles[i]));
            }

            GroundWorld = new World<int>(data.Ground);
            AnimalWorld = new World<int>(data.Animals);
        }
        public void LoadDataV2(SimulationDataFile data)
        {
            DestroyAllActiveAnimals();
            CurrentID = data.CurrentID;
            GroundWorld = new World<int>(data.Ground);
            AnimalWorld = new World<int>(data.Animals);
            ControllersGrid = new World<AnimalController>(AnimalWorld.X, AnimalWorld.Y);
            AnimalProfiles = new List<CompiledAnimalProfile>();

            for (int i = 0; i < data.Profiles.Length; i++)
            {
                AnimalProfiles.Add(new CompiledAnimalProfile(data.Profiles[i]));
            }
            for (int i = 0; i < data.Controllers.Length; i++)
            {
                AnimalController controller = SpawnAnimal(data.Controllers[i]);
                ControllersGrid.SetCell(data.Controllers[i].CurrentPositionX, data.Controllers[i].CurrentPositionY, controller);
                Controllers.Add(controller);
            }
            for (int i = 0; i < data.Controllers.Length; i++)
            {
                Controllers[i].InitIndexes(data.Controllers[i]);
            }

            ActiveSimulation = true;
            ActiveSimulationText.SetActive(false);
            ResetCameraPosition(GroundWorld.X / 2f);
            DisplayWorld();
        }
        #endregion
        #region Other
        public CompiledAnimalProfile GetProfile(int index) => AnimalProfiles[index];
        private void DeleteExtraControllers()
        {
            List<int> controllerToRemove = new List<int>();
            for (int i = 0; i < Controllers.Count; i++)
            {
                if (Controllers[i] == null)
                {
                    controllerToRemove.Add(i - controllerToRemove.Count);
                }
            }
            for (int i = 0; i < controllerToRemove.Count; i++)
            {
                Controllers.RemoveAt(controllerToRemove[i]);
            }
        }
        #endregion
    }
}