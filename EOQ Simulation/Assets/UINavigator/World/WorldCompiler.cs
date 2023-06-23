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

        public TMP_Dropdown WorldType;
        public TMP_InputField WorldSeed;

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

        }

        private void LakeGeneration(ref World<int> world)
        {

        }
    }
}