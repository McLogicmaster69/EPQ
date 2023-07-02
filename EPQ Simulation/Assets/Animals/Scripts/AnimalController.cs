using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    public class AnimalController : MonoBehaviour
    {
        public int ProfileIndex { get; private set; }
        public bool IsAnimal { get { return Attributes.IsAnimal; } }
        public int TicksOfFood { get; private set; }
        public int TicksToRegrow { get; private set; }
        public int TicksToMove { get; private set; }
        public Vector2Int CurrentPosition { get; private set; }
        public bool IsGrown { get; private set; }

        private AnimalAttributes Attributes;

        public void InitAnimal(int index, Vector2Int position, AnimalAttributes attributes)
        {
            ProfileIndex = index;
            CurrentPosition = position;
            TicksOfFood = attributes.FoodRequirement * WorldManager.STARTING_FOOD * WorldManager.TICKS_IN_DAY;
            TicksToRegrow = 1;
            TicksToMove = 1;
            IsGrown = true;
            Attributes = attributes;
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
            }
        }

        private void HandleFoodTick()
        {
            TicksOfFood--;
        }
        private void HandleMoveTick()
        {
            TicksToMove--;
        }
        private void HandleRegrowTick()
        {
            TicksToRegrow--;
        }
    }
}