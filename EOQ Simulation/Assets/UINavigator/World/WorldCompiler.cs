using EPQ.Animals;
using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EPQ.Compiler
{
    public class WorldCompiler : MonoBehaviour
    {
        public const int WORLD_SIZE = 50;

        [Header("UI")]
        public TMP_Dropdown WorldType;
        public TMP_InputField WorldSeed;

        [Header("River")]
        [Range(0f, 1f)]
        public float RiverPercentProportion = 0.04f;
        public int RiverSize = 2;
        [Range(0f, 1f)]
        public float RiverExpandChance = 0.85f;

        private float NumberOfRivers { get { return (WORLD_SIZE * 2) * RiverPercentProportion; } }

        public void CompileAndRun()
        {
            if (string.IsNullOrEmpty(WorldSeed.text))
                WorldSeed.text = Random.Range(0, 99999999).ToString();

            Random.InitState(WorldSeed.text.GetHashCode());

            List<CompiledAnimalProfile> profiles = new List<CompiledAnimalProfile>();
            for (int i = 0; i < AnimalUINavigator.main.Profiles.Count; i++)
            {
                profiles.Add(new CompiledAnimalProfile(AnimalUINavigator.main.Profiles[i]));
            }

            // WORLD GENERATION
            World<int> world = new World<int>(WORLD_SIZE, WORLD_SIZE);

            switch (WorldType.value)
            {
                case 0:
                    BlankGeneration(ref world);
                    break;
                case 1:
                    RiversGeneration(ref world);
                    break;
            }

            // ANIMAL GENERATION
            World<int> animals = new World<int>(WORLD_SIZE, WORLD_SIZE);

            // SETUP WORLD
            WorldManager.main.SetupWorld(world, animals, profiles);
            WorldManager.main.DisplayWorld();
        }

        private void BlankGeneration(ref World<int> world)
        {
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                for (int y = 0; y < WORLD_SIZE; y++)
                {
                    world.SetCell(x, y, 0);
                }
            }
        }

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
                world.SetCell(x, y, 1);
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
                world.SetCell(x, y, 1);
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
                world.SetCell(x, y, 1);
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
                world.SetCell(x, y, 1);
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
                        int numberInRange = world.NumberInRange(1, x, y, 1);
                        if (numberInRange > 0)
                        {
                            if (numberInRange <= 3)
                            {
                                if (Random.Range(0, 1000) < RiverExpandChance * 1000)
                                    editingWorld.SetCell(x, y, 1);
                            }
                            else
                                editingWorld.SetCell(x, y, 1);
                        }
                    }
                }
                world.SetWorld(editingWorld);
            }
        }

        private void LakeGeneration(ref World<int> world)
        {

        }
    }
}