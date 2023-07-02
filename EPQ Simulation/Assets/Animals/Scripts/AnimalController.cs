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
        public List<int> SortedEat { get; private set; }

        private CompiledAnimalProfile Profile;
        private Material Material;
        private WorldManager Manager;

        private AnimalController Target;
        private Vector2Int TargetPosition = Vector2Int.zero;

        public void InitAnimal(int index, Vector2Int position, CompiledAnimalProfile profile)
        {
            Profile = profile;
            Manager = WorldManager.main;

            ProfileIndex = index;
            CurrentPosition = position;
            TicksOfFood = Profile.Attributes.FoodRequirement * WorldManager.STARTING_FOOD * WorldManager.TICKS_IN_DAY;
            TicksToRegrow = 1;
            TicksToMove = 1;
            IsGrown = true;

            transform.position = new Vector3(position.x, 1f, position.y);
            Material = new Material(Manager.BlankMaterial);
            Material.color = profile.Color;
            GetComponent<MeshRenderer>().material = Material;

            InitEat();
        }
        private void InitEat()
        {
            SortedEat = new List<int>();
            List<CompiledAnimalProfile> profiles = new List<CompiledAnimalProfile>();
            for (int i = 0; i < Profile.CanEat.Count; i++)
            {
                profiles.Add(Manager.GetProfile(Profile.CanEat[i]));
            }

            if (profiles.Count == 0)
                return;

            SortedEat.Add(AnimalUINavigator.main.GetIndex(profiles[0].ID));
            for (int i = 1; i < profiles.Count; i++)
            {
                int index = AnimalUINavigator.main.GetIndex(profiles[i].ID);
                int foodScore = AnimalUINavigator.main.Profiles[index].Attributes.FoodValue;
                bool sorted = false;
                for (int j = 0; j < SortedEat.Count; j++)
                {
                    if(foodScore > AnimalUINavigator.main.Profiles[SortedEat[j]].Attributes.FoodValue)
                    {
                        SortedEat.Insert(j, index);
                        sorted = true;
                        break;
                    }
                }
                if (!sorted)
                    SortedEat.Add(index);
            }
        }
        public void Tick()
        {
            if (IsAnimal)
            {
                HandleFoodTick();
                HandleMoveTick();
                HandleTarget();
                HandleTargetPosition();
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

            if(TicksToMove <= 0)
            {
                Move();
                if (GetCurrentCell() == WorldCompiler.GRASS_ID)
                    TicksToMove = (11 - Profile.Attributes.GrassSpeed);
                else
                    TicksToMove = (11 - Profile.Attributes.WaterSpeed);
                transform.position = new Vector3(CurrentPosition.x, 1f, CurrentPosition.y);
            }
        }
        private void HandleRegrowTick()
        {
            TicksToRegrow--;
            if(TicksToRegrow <= 0)
            {
                IsGrown = true;
                PlaceInPosition();
            }
        }
        private void HandleTarget()
        {
            if(Target != null)
            {
                if(Target.IsAnimal == false && Target.IsGrown == false)
                {
                    Target = null;
                    return;
                }
            }
        }
        private void HandleTargetPosition()
        {

        }

        private void PlaceInPosition()
        {
            if(GetCurrentAnimalCell() == -1)
            {
                Manager.SetAnimalCell(CurrentPosition.x, CurrentPosition.y, ProfileIndex, this);
                GetComponent<MeshRenderer>().enabled = true;
            }
        }

        private void Move()
        {
            if(CurrentPosition.x != TargetPosition.x && CurrentPosition.y != TargetPosition.y)
            {
                if (Random.Range(0, 2) == 0)
                    MoveX();
                else
                    MoveY();
            }
            else if(CurrentPosition.x != TargetPosition.y)
            {
                MoveX();
            }
            else
            {
                MoveY();
            }
        }
        private void MoveX()
        {
            if(CurrentPosition.x < TargetPosition.x)
                CurrentPosition = new Vector2Int(CurrentPosition.x + 1, CurrentPosition.y);
            else
                CurrentPosition = new Vector2Int(CurrentPosition.x - 1, CurrentPosition.y);
        }
        private void MoveY()
        {
            if (CurrentPosition.y < TargetPosition.y)
                CurrentPosition = new Vector2Int(CurrentPosition.x, CurrentPosition.y + 1);
            else
                CurrentPosition = new Vector2Int(CurrentPosition.x, CurrentPosition.y - 1);
        }
        private int GetCurrentCell() => Manager.GetGroundCell(CurrentPosition.x, CurrentPosition.y);
        private int GetCurrentAnimalCell() => Manager.GetAnimalCell(CurrentPosition.x, CurrentPosition.y);

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
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}