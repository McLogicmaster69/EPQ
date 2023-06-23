using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EPQ.Clock;

namespace EPQ.Worlds
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager main;

        public List<GameObject> Tiles = new List<GameObject>();
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
        }
        public void DisplayWorld()
        {
            DestroyWorld();

            for (int y = 0; y < GroundWorld.Y; y++)
            {
                for (int x = 0; x < GroundWorld.X; x++)
                {
                    GameObject o = Instantiate(Tiles[GroundWorld.GetCell(x, y)], new Vector3(x, 0, y), Quaternion.identity, GroundParent);
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