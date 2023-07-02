using EPQ.Animals;
using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EPQ.Data;
using EPQ.Files;

namespace EPQ.Compiler
{
    public class WorldCompiler : MonoBehaviour
    {
        public static readonly int GRASS_ID = 0;
        public static readonly int WATER_ID = 1;

        [Header("UI")]
        public TMP_Dropdown WorldType;
        public TMP_InputField WorldSeed;
        public Slider WorldSizeSlider;
        public TMP_InputField WorldSizeInput;
        public AttributeSlider AnimalPercentSlider;
        public TMP_Text StatusText;

        [Header("River UI")]
        public GameObject RiverOptionButton;
        public GameObject RiverOptionsUI;
        public Slider RiverFrequencySlider;
        public TMP_InputField RiverFrequencyInput;
        public Slider RiverSizeSlider;
        public TMP_InputField RiverSizeInput;
        public Slider RiverConcentrationSlider;
        public TMP_InputField RiverConcentrationInput;

        [Header("Lake UI")]
        public GameObject LakeOptionButton;
        public GameObject LakeOptionsUI;
        public Slider LakeFrequencySlider;
        public TMP_InputField LakeFrequencyInput;
        public Slider LakeSizeSlider;
        public TMP_InputField LakeSizeInput;
        public Slider LakeConcentrationSlider;
        public TMP_InputField LakeConcentrationInput;

        [Header("Load")]
        public GameObject LoadUI;
        public TMP_InputField FileName;
        public TMP_Text LoadStatusText;

        [Header("World")]
        public int WorldSize = 50;
        public int AnimalPercent = 10;

        [Header("River")]
        [Range(0f, 1f)]
        public float RiverPercentProportion = 0.04f;
        public int RiverSize = 2;
        [Range(0f, 1f)]
        public float RiverExpandChance = 0.85f;

        [Header("Lake")]
        [Range(0f, 1f)]
        public float LakePercentProportion = 0.01f;
        public int LakeSize = 8;
        [Range(0f, 1f)]
        public float LakeExpandChance = 0.85f;

        private float NumberOfRivers { get { return WorldSize * 2 * RiverPercentProportion; } }
        private float NumberOfLakes { get { return WorldSize * WorldSize * LakePercentProportion; } }

        private const string LOAD_FAILED_MESSAGE = "FAILED TO OPEN FILE ";

        private void Start()
        {
            RiverOptionsUI.SetActive(false);
            ChangeSelectedOption(0);
            InitGeneralUI();
            InitRiverValues();
            InitWorldValues();
            InitLakeValues();
            InitLoadUI();
        }

        public void CompileAndRun()
        {
            System.DateTime startTime = System.DateTime.Now;

            InitRandomSeed();
            List<CompiledAnimalProfile> profiles = CompileAnimalProfiles();
            World<int> world = WorldGeneration();
            World<int> animals = AnimalGeneration();

            WorldManager.main.SetupWorld(world, animals, profiles);
            Clock.main.IsTicking = false;

            System.DateTime endTime = System.DateTime.Now;
            System.TimeSpan timeTaken = endTime - startTime;
            ChangeStatus($"Created world in {timeTaken.TotalSeconds} seconds");
        }

        #region World Setup
        private void InitRandomSeed()
        {
            if (string.IsNullOrEmpty(WorldSeed.text))
                WorldSeed.text = Random.Range(0, 99999999).ToString();

            Random.InitState(WorldSeed.text.GetHashCode());
        }
        private void InitGeneralUI()
        {
            AnimalPercentSlider.InitSlider((int value) => { AnimalPercent = value; });
            AnimalPercentSlider.SetValue(AnimalPercent);
        }
        private List<CompiledAnimalProfile> CompileAnimalProfiles()
        {
            List<CompiledAnimalProfile> profiles = new List<CompiledAnimalProfile>();
            for (int i = 0; i < AnimalUINavigator.main.Profiles.Count; i++)
            {
                profiles.Add(new CompiledAnimalProfile(AnimalUINavigator.main.Profiles[i]));
            }
            return profiles;
        }
        private World<int> WorldGeneration()
        {
            World<int> world = new World<int>(WorldSize, WorldSize);
            switch (WorldType.value)
            {
                case 0:
                    BlankGeneration(ref world);
                    break;
                case 1:
                    RiversGeneration(ref world);
                    break;
                case 2:
                    LakeGeneration(ref world);
                    break;
            }
            return world;
        }
        #endregion

        #region Animal Generation
        private World<int> AnimalGeneration()
        {
            World<int> animals = new World<int>(WorldSize, WorldSize, -1);

            float total = GetTotalBatch();
            if (total == 0)
                return animals;

            List<Vector2Int> availablePositoins = GenerateAvailablePositoins();
            AddAnimalsToWorld(animals, availablePositoins, total);
            return animals;
        }
        private List<Vector2Int> GenerateAvailablePositoins()
        {
            List<Vector2Int> availablePositoins = new List<Vector2Int>();
            for (int x = 0; x < WorldSize; x++)
            {
                for (int y = 0; y < WorldSize; y++)
                {
                    availablePositoins.Add(new Vector2Int(x, y));
                }
            }
            return availablePositoins;
        }
        private float GetTotalBatch()
        {
            float total = 0;
            for (int i = 0; i < AnimalUINavigator.main.Profiles.Count; i++)
            {
                total += AnimalUINavigator.main.Profiles[i].BatchSize;
            }
            return total;
        }
        private void AddAnimalsToWorld(World<int> world, List<Vector2Int> availablePositoins, float total)
        {
            for (int i = 0; i < AnimalUINavigator.main.Profiles.Count; i++)
            {
                AnimalProfile profile = AnimalUINavigator.main.Profiles[i];
                int amountToGenerate = Mathf.FloorToInt((profile.BatchSize / total) * (AnimalPercent / 200f) * (WorldSize * WorldSize));
                for (int j = 0; j < amountToGenerate; j++)
                {
                    if (availablePositoins.Count == 0)
                        return;
                    int index = Random.Range(0, availablePositoins.Count);
                    world.SetCell(availablePositoins[index].x, availablePositoins[index].y, i);
                    availablePositoins.RemoveAt(index);
                }
            }
        }
        #endregion

        #region Blank Generation
        private void BlankGeneration(ref World<int> world)
        {
            for (int x = 0; x < WorldSize; x++)
            {
                for (int y = 0; y < WorldSize; y++)
                {
                    world.SetCell(x, y, GRASS_ID);
                }
            }
        }
        #endregion
        #region River Generation
        private void RiversGeneration(ref World<int> world)
        {
            CreateRiverOutline(ref world);
            ExpandRivers(ref world);
        }
        private void CreateRiverOutline(ref World<int> world)
        {
            int rivers = Mathf.FloorToInt(NumberOfRivers);
            for (int i = 0; i < rivers; i++)
            {
                int direction = Random.Range(0, 4);
                switch (direction)
                {
                    case 0:
                        RiverNorthGeneration(ref world);
                        break;
                    case 1:
                        RiverEastGeneration(ref world);
                        break;
                    case 2:
                        RiverSouthGeneration(ref world);
                        break;
                    case 3:
                        RiverWestGeneration(ref world);
                        break;
                }
            }
        }
        private void RiverNorthGeneration(ref World<int> world)
        {
            InitRiverGenerationValues(out int x, out int leftBoundary, out int rightBoundary, world.X);

            for (int y = 0; y < world.Y; y++)
            {
                world.SetCell(x, y, WATER_ID);
                int fowardDirection = Random.Range(-7, 8);
                if (!MoveRiverPosition(ref x, world.X - 1, fowardDirection, leftBoundary, rightBoundary))
                    return;
            }
        }
        private void RiverEastGeneration(ref World<int> world)
        {
            InitRiverGenerationValues(out int y, out int leftBoundary, out int rightBoundary, world.Y);

            for (int x = 0; x < world.X; x++)
            {
                world.SetCell(x, y, WATER_ID);
                int fowardDirection = Random.Range(-7, 8);
                if (!MoveRiverPosition(ref y, world.Y - 1, fowardDirection, leftBoundary, rightBoundary))
                    return;
            }
        }
        private void RiverSouthGeneration(ref World<int> world)
        {
            InitRiverGenerationValues(out int x, out int leftBoundary, out int rightBoundary, world.X);

            for (int y = world.Y - 1; y >= 0; y--)
            {
                world.SetCell(x, y, WATER_ID);
                int fowardDirection = Random.Range(-7, 8);
                if (!MoveRiverPosition(ref x, world.X - 1, fowardDirection, leftBoundary, rightBoundary))
                    return;
            }
        }
        private void RiverWestGeneration(ref World<int> world)
        {
            InitRiverGenerationValues(out int y, out int leftBoundary, out int rightBoundary, world.Y);

            for (int x = world.X - 1; x >= 0; x--)
            {
                world.SetCell(x, y, WATER_ID);
                int fowardDirection = Random.Range(-7, 8);
                if (!MoveRiverPosition(ref y, world.Y - 1, fowardDirection, leftBoundary, rightBoundary))
                    return;
            }
        }
        private void InitRiverGenerationValues(out int pos, out int leftBoundary, out int rightBoundary, int upperPosBoundary)
        {
            pos = Random.Range(0, upperPosBoundary);
            leftBoundary = Random.Range(-7, 0);
            rightBoundary = Random.Range(1, 8);
        }
        private bool MoveRiverPosition(ref int pos, int upperPos, int fowardDirection, int leftBoundary, int rightBoundary)
        {
            if (fowardDirection < leftBoundary)
            {
                if (pos > 0)
                    pos--;
                else
                    return false;
            }
            else if (fowardDirection > rightBoundary)
            {
                if (pos < upperPos)
                    pos++;
                else
                    return false;
            }
            return true;
        }
        private void ExpandRivers(ref World<int> world)
        {
            for (int i = 0; i < RiverSize; i++)
            {
                World<int> editingWorld = new World<int>(world);
                for (int x = 0; x < editingWorld.X; x++)
                {
                    for (int y = 0; y < editingWorld.Y; y++)
                    {
                        int numberInRange = world.NumberInRange(WATER_ID, x, y, 1);
                        if (numberInRange > 0)
                        {
                            if (numberInRange <= 3)
                            {
                                if (Random.Range(0, 1000) < RiverExpandChance * 1000)
                                    editingWorld.SetCell(x, y, WATER_ID);
                            }
                            else
                                editingWorld.SetCell(x, y, WATER_ID);
                        }
                    }
                }
                world.SetWorld(editingWorld);
            }
        }
        #endregion
        #region Lake Generation
        private void LakeGeneration(ref World<int> world)
        {
            GenerateLakeStartingPoint(ref world);
            ExpandLakes(ref world);
        }
        private void GenerateLakeStartingPoint(ref World<int> world)
        {
            for (int i = 0; i < NumberOfLakes; i++)
            {
                int randX = Random.Range(0, world.X);
                int randY = Random.Range(0, world.Y);
                world.SetCell(randX, randY, WATER_ID);
            }
        }
        private void ExpandLakes(ref World<int> world)
        {
            for (int i = 0; i < LakeSize; i++)
            {
                World<int> editingWorld = new World<int>(world);
                for (int x = 0; x < editingWorld.X; x++)
                {
                    for (int y = 0; y < editingWorld.Y; y++)
                    {
                        int numberInRange = world.NumberInRadius(1, x, y, 1);
                        float multiplier = numberInRange / 4f;
                        if (Random.Range(0, 1000) / multiplier < LakeExpandChance * 1000)
                            editingWorld.SetCell(x, y, WATER_ID);
                    }
                }
                world.SetWorld(editingWorld);
            }
        }
        #endregion

        #region Main UI
        private void InitWorldValues()
        {
            UpdateWorldSizeValue((int)WorldSizeSlider.value);
        }
        public void ChangeSelectedOption(int option)
        {
            switch (option)
            {
                case 0:
                    RiverOptionButton.SetActive(false);
                    LakeOptionButton.SetActive(false);
                    break;
                case 1:
                    RiverOptionButton.SetActive(true);
                    LakeOptionButton.SetActive(false);
                    break;
                case 2:
                    RiverOptionButton.SetActive(false);
                    LakeOptionButton.SetActive(true);
                    break;
            }
        }
        public void ChangeWorldSizeValue(System.Single value)
        {
            int r = System.Convert.ToInt32(value);
            if (r.ToString() != WorldSizeInput.text)
            {
                WorldSizeInput.text = r.ToString();
                UpdateWorldSizeValue(r);
            }
        }
        public void ChangeWorldSizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int r = Mathf.Clamp(System.Convert.ToInt32(text), 25, 150);
            WorldSizeSlider.value = r;
            UpdateWorldSizeValue(r);
        }
        private void UpdateWorldSizeValue(int value)
        {
            WorldSize = value;
        }
        public void ChangeAnimalPercentSlider(System.Single value)
        {
            AnimalPercentSlider.ChangeValue(value);
        }
        public void ChangeAnimalPercentText(string text)
        {
            AnimalPercentSlider.ChangeText(text);
        }
        private void ChangeStatus(string message)
        {
            StatusText.text = message;
        }
        #endregion
        #region River UI
        private void InitRiverValues()
        {
            UpdateRiverFrequencyValue((int)RiverFrequencySlider.value);
            UpdateRiverSizeValue((int)RiverSizeSlider.value);
            UpdateRiverConcentrationValue((int)RiverConcentrationSlider.value);
        }
        public void ToggleRiverOptionsUI()
        {
            RiverOptionsUI.SetActive(!RiverOptionsUI.activeSelf);
        }
        public void ChangeRiverFrequencyValue(System.Single value)
        {
            int r = System.Convert.ToInt32(value);
            if (r.ToString() != RiverFrequencyInput.text)
            {
                RiverFrequencyInput.text = r.ToString();
                UpdateRiverFrequencyValue(r);
            }
        }
        public void ChangeRiverFrequencyText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int r = Mathf.Clamp(System.Convert.ToInt32(text), 1, 10);
            RiverFrequencySlider.value = r;
            UpdateRiverFrequencyValue(r);
        }
        private void UpdateRiverFrequencyValue(int value)
        {
            RiverPercentProportion = value / 100f;
        }
        public void ChangeRiverSizeValue(System.Single value)
        {
            int r = System.Convert.ToInt32(value);
            if (r.ToString() != RiverSizeInput.text)
            {
                RiverSizeInput.text = r.ToString();
                UpdateRiverSizeValue(r);
            }
        }
        public void ChangeRiverSizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int r = Mathf.Clamp(System.Convert.ToInt32(text), 1, 4);
            RiverSizeSlider.value = r;
            UpdateRiverSizeValue(r);
        }
        private void UpdateRiverSizeValue(int value)
        {
            RiverSize = value;
        }
        public void ChangeRiverConcentrationValue(System.Single value)
        {
            int r = System.Convert.ToInt32(value);
            if (r.ToString() != RiverConcentrationInput.text)
            {
                RiverConcentrationInput.text = r.ToString();
                UpdateRiverConcentrationValue(r);
            }
        }
        public void ChangeRiverConcentrationText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int r = Mathf.Clamp(System.Convert.ToInt32(text), 1, 10);
            RiverConcentrationSlider.value = r;
            UpdateRiverConcentrationValue(r);
        }
        private void UpdateRiverConcentrationValue(int value)
        {
            RiverExpandChance = 0.5f + value * 0.05f;
        }
        #endregion
        #region Lake UI
        private void InitLakeValues()
        {
            UpdateLakeFrequencyValue((int)LakeFrequencySlider.value);
            UpdateLakeSizeValue((int)LakeSizeSlider.value);
            UpdateLakeConcentrationValue((int)LakeConcentrationSlider.value);
        }
        public void ToggleLakeOptionsUI()
        {
            LakeOptionsUI.SetActive(!LakeOptionsUI.activeSelf);
        }
        public void ChangeLakeFrequencyValue(System.Single value)
        {
            int r = System.Convert.ToInt32(value);
            if (r.ToString() != LakeFrequencyInput.text)
            {
                LakeFrequencyInput.text = r.ToString();
                UpdateLakeFrequencyValue(r);
            }
        }
        public void ChangeLakeFrequencyText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int r = Mathf.Clamp(System.Convert.ToInt32(text), 1, 10);
            LakeFrequencySlider.value = r;
            UpdateLakeFrequencyValue(r);
        }
        private void UpdateLakeFrequencyValue(int value)
        {
            LakePercentProportion = value / 2000f;
        }
        public void ChangeLakeSizeValue(System.Single value)
        {
            int r = System.Convert.ToInt32(value);
            if (r.ToString() != LakeSizeInput.text)
            {
                LakeSizeInput.text = r.ToString();
                UpdateLakeSizeValue(r);
            }
        }
        public void ChangeLakeSizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int r = Mathf.Clamp(System.Convert.ToInt32(text), 25, 50);
            LakeSizeSlider.value = r;
            UpdateLakeSizeValue(r);
        }
        private void UpdateLakeSizeValue(int value)
        {
            LakeSize = value;
        }
        public void ChangeLakeConcentrationValue(System.Single value)
        {
            int r = System.Convert.ToInt32(value);
            if (r.ToString() != LakeConcentrationInput.text)
            {
                LakeConcentrationInput.text = r.ToString();
                UpdateLakeConcentrationValue(r);
            }
        }
        public void ChangeLakeConcentrationText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            int r = Mathf.Clamp(System.Convert.ToInt32(text), 1, 10);
            LakeConcentrationSlider.value = r;
            UpdateLakeConcentrationValue(r);
        }
        private void UpdateLakeConcentrationValue(int value)
        {
            LakeExpandChance = 0.5f + value * 0.05f;
        }
        #endregion
        #region Load UI
        public void InitLoadUI()
        {
            LoadStatusText.text = string.Empty;
            LoadUI.SetActive(false);
        }
        public void ToggleLoadUI()
        {
            LoadUI.SetActive(!LoadUI.activeInHierarchy);
        }
        public void LoadSimulation()
        {
            SimulationDataFile data = SaveSystem.Load<SimulationDataFile>(FileName.text, "sim");

            if (data == null)
            {
                LoadStatusText.text = FormatLoadFailMessage(FileName.text);
                return;
            }

            FileManager.LoadFile(data);
            ChangeStatus("Loaded simulation");
            ToggleLoadUI();
        }
        private string FormatLoadFailMessage(string filename) => LOAD_FAILED_MESSAGE + filename;
        #endregion
    }
}