using EPQ.Compiler;
using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    public class AnimalController : MonoBehaviour
    {
        public int ProfileIndex { get; private set; }
        public bool IsAnimal { get { return Profile.Attributes.IsAnimal; } }
        public int TicksOfFood { get; private set; }
        public int TicksToRegrow { get; private set; }
        public int TicksToMove { get; private set; }
        public Vector2Int CurrentPosition { get; private set; }
        public bool IsGrown { get; private set; }

        private CompiledAnimalProfile Profile;
        private Material Material;

        private AnimalController Target;
        private Vector2Int TargetPosition = Vector2Int.zero;

        public void InitAnimal(int index, Vector2Int position, CompiledAnimalProfile profile)
        {
            Profile = profile;

            ProfileIndex = index;
            CurrentPosition = position;
            TicksOfFood = Profile.Attributes.FoodRequirement * WorldManager.STARTING_FOOD * WorldManager.TICKS_IN_DAY;
            TicksToRegrow = 1;
            TicksToMove = 1;
            IsGrown = true;

            transform.position = new Vector3(position.x, 1f, position.y);
            Material = new Material(WorldManager.main.BlankMaterial);
            Material.color = profile.Color;
            GetComponent<MeshRenderer>().material = Material;
        }
        public void Tick()
        {
            if (IsAnimal)
            {
                HandleFoodTick();
                HandleMoveTick();
            }
            else
            {
                HandleRegrowTick();
                PlaceInPosition();
            }
        }

        private void HandleFoodTick()
        {
            TicksOfFood--;
        }
        private void HandleMoveTick()
        {
            TicksToMove--;

            if(TicksToMove <= 0)
            {
                Move();
                if (GetCurrentCell() == WorldCompiler.GRASS_ID)
                    TicksToMove = (11 - Profile.Attributes.GrassSpeed);
                else
                    TicksToMove = (11 - Profile.Attributes.WaterSpeed);
            }
        }
        private void HandleRegrowTick()
        {
            TicksToRegrow--;
            if(TicksToRegrow <= 0)
            {
                IsGrown = false;
            }
        }

        private void PlaceInPosition()
        {
            if(GetCurrentAnimalCell() == -1)
            {
                WorldManager.main.SetAnimalCell(CurrentPosition.x, CurrentPosition.y, ProfileIndex, this);
            }
        }

        private void Move()
        {

        }
        private int GetCurrentCell() => WorldManager.main.GetGroundCell(CurrentPosition.x, CurrentPosition.y);
        private int GetCurrentAnimalCell() => WorldManager.main.GetAnimalCell(CurrentPosition.x, CurrentPosition.y);

        public void Kill()
        {
            if (IsAnimal)
                Destroy(gameObject);
            else
                Eat();
        }
        public void Eat()
        {
            IsGrown = false;
            TicksToRegrow = Profile.Attributes.TimeToRegrow * WorldManager.TICKS_IN_DAY;
        }
    }
}