using EPQ.Compiler;
using EPQ.Data;
using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    public class AnimalController : MonoBehaviour
    {
        public int ID { get; private set; }
        public int ProfileIndex { get; private set; }
        public bool IsAnimal { get { return Profile.Attributes.IsAnimal; } }
        public int TicksOfFood { get; private set; }
        public int TicksToRegrow { get; private set; }
        public int TicksToMove { get; private set; }
        public int TicksSinceLastRandom { get; private set; }
        public Vector2Int CurrentPosition { get; private set; }
        public bool IsGrown { get; private set; }
        public List<int> SortedEat { get; private set; }

        private CompiledAnimalProfile Profile;
        private Material Material;
        private WorldManager Manager;

        private AnimalController Target;
        private AnimalController MateTarget;
        private int currentTargetIndexInEat = -1;
        private Vector2Int TargetPosition = Vector2Int.zero;
        private int foodCollectedToMate = 0;

        private const int RANDOM_TICKS = 30;

        #region Initialisation
        public void InitAnimal(int index, Vector2Int position, CompiledAnimalProfile profile, int id)
        {
            Manager = WorldManager.main;
            ID = id;
            Profile = profile;
            ProfileIndex = index;
            CurrentPosition = position;

            InitAttributes();
            InitMaterial();
            InitEat();
        }
        public void InitAnimal(ControllerDataFile file)
        {
            Manager = WorldManager.main;
            InitAttributes(file);
            InitMaterial();
        }
        private void InitAttributes()
        {
            TicksOfFood = Profile.Attributes.FoodRequirement * WorldManager.STARTING_FOOD * WorldManager.TICKS_IN_DAY;
            TicksToRegrow = 1;
            TicksToMove = 1;
            IsGrown = true;
            TargetPosition = GetRandomPosition();
            transform.position = new Vector3(CurrentPosition.x, 1f, CurrentPosition.y);
        }
        private void InitMaterial()
        {
            Material = new Material(Manager.BlankMaterial);
            Material.color = Profile.Color;
            GetComponent<MeshRenderer>().material = Material;
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

            currentTargetIndexInEat = SortedEat.Count;
        }
        private void InitAttributes(ControllerDataFile file)
        {
            ID = file.ID;
            ProfileIndex = file.ProfileIndex;
            TicksOfFood = file.TicksOfFood;
            TicksToRegrow = file.TicksToRegrow;
            TicksToMove = file.TicksToMove;
            TicksSinceLastRandom = file.TicksSinceLastRandom;
            CurrentPosition = new Vector2Int(file.CurrentPositionX, file.CurrentPositionY);
            IsGrown = file.IsGrown;
            SortedEat = new List<int>(file.SortedEat);
            currentTargetIndexInEat = file.CurrentTargetIndexInEat;
            TargetPosition = new Vector2Int(file.TagetPositionX, file.TagetPositionY);
            Profile = Manager.GetProfile(file.ProfileIndex);
            transform.position = new Vector3(CurrentPosition.x, 1f, CurrentPosition.y);
        }
        public void InitIndexes(ControllerDataFile file)
        {
            if(file.IndexOfTarget != -1)
                Target = Manager.Controllers[file.IndexOfTarget];
            if(file.IndexOfMate != -1)
                MateTarget = Manager.Controllers[file.IndexOfMate];
        }
        #endregion
        #region Tick
        public void Tick()
        {
            if (IsAnimal)
            {
                HandleExistence();
                HandleTarget();
                HandleTargetPosition();
                HandleFoodTick();
                HandleMoveTick();
                HandleMateTick();
            }
            else
            {
                HandleRegrowTick();
            }
        }
        private void HandleFoodTick()
        {
            TicksOfFood--;
            if(TicksOfFood <= 0)
            {
                Kill();
            }
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
        private void HandleMateTick()
        {
            if(MateTarget != null)
            {
                if(CurrentPosition.x == MateTarget.CurrentPosition.x)
                {
                    if(Mathf.Abs(CurrentPosition.y - MateTarget.CurrentPosition.y) == 1)
                    {
                        Mate();
                    }
                }
                else if(CurrentPosition.y == MateTarget.CurrentPosition.y)
                {
                    if (Mathf.Abs(CurrentPosition.x - MateTarget.CurrentPosition.x) == 1)
                    {
                        Mate();
                    }
                }
            }
        }
        private void HandleTarget()
        {
            if(foodCollectedToMate == Profile.Attributes.FoodToReproduce || MateTarget != null)
            {
                if(MateTarget == null)
                {
                    if(Manager.AnimalWorld.ClosestInRadius(ProfileIndex, CurrentPosition.x, CurrentPosition.y, Profile.Attributes.DetectionRange,out int xPos, out int yPos))
                    {
                        AnimalController controller = Manager.ControllersGrid.GetCell(xPos, yPos);
                        if (controller.OfferMate(this))
                        {
                            MateTarget = controller;
                        }
                    }
                }
            }

            if (MateTarget != null)
                return;

            if(Target == null)
            {
                currentTargetIndexInEat = SortedEat.Count;
            }

            for (int i = 0; i < currentTargetIndexInEat; i++)
            {
                if (ClosestInRadius(SortedEat[i], out int xPos, out int yPos))
                {
                    Target = Manager.ControllersGrid.GetCell(xPos, yPos);
                    currentTargetIndexInEat = i;
                    return;
                }
            }

            if (Target != null)
            {
                if(Target.IsAnimal == false && Target.IsGrown == false)
                {
                    Target = null;
                    currentTargetIndexInEat = SortedEat.Count;
                    return;
                }
            }
        }
        private void HandleTargetPosition()
        {
            if(MateTarget != null)
            {
                TargetPosition = MateTarget.CurrentPosition;
            }
            else if (Target != null)
            {
                TargetPosition = Target.CurrentPosition;
            }
            else
            {
                for (int i = 0; i < Profile.CanBeEatenBy.Count; i++)
                {
                    if(ScanRadius(Profile.CanBeEatenBy[i], out int[] xPos, out int[] yPos) > 0)
                    {
                        Vector2Int average = GetAveragePosition(xPos, yPos);
                        TargetPosition = GetOppositePosition(average);
                        return;
                    }
                }

                if (CurrentPosition == TargetPosition || TicksSinceLastRandom >= RANDOM_TICKS)
                {
                    TicksSinceLastRandom = 0;
                    TargetPosition = GetRandomPosition();
                }

                TicksSinceLastRandom++;
            }
        }
        private void HandleExistence()
        {
            AnimalController controller = Manager.ControllersGrid.GetCell(CurrentPosition.x, CurrentPosition.y);
            if (controller.ID != this.ID)
            {
                if (CheckIfCanEat(controller.ProfileIndex))
                {
                    controller.Kill(this);
                    Manager.SetAnimalCell(CurrentPosition.x, CurrentPosition.y, ProfileIndex, this);
                }
                else
                {
                    this.Kill(controller);
                }
            }
        }
        #endregion
        #region Movement
        private void Move()
        {
            if (CurrentPosition.x != TargetPosition.x && CurrentPosition.y != TargetPosition.y)
            {
                if (Random.Range(0, 2) == 0)
                    MoveX();
                else
                    MoveY();
            }
            else if (CurrentPosition.x != TargetPosition.x)
            {
                MoveX();
            }
            else if (CurrentPosition.y != TargetPosition.y)
            {
                MoveY();
            }
        }
        private void MoveX()
        {
            if (CurrentPosition.x < TargetPosition.x)
                TryMove(new Vector2Int(CurrentPosition.x + 1, CurrentPosition.y));
            else
                TryMove(new Vector2Int(CurrentPosition.x - 1, CurrentPosition.y));
        }
        private void MoveY()
        {
            if (CurrentPosition.y < TargetPosition.y)
                TryMove(new Vector2Int(CurrentPosition.x, CurrentPosition.y + 1));
            else
                TryMove(new Vector2Int(CurrentPosition.x, CurrentPosition.y - 1));
        }
        private void TryMove(Vector2Int position)
        {
            if(Target != null)
            {
                if(position == Target.CurrentPosition)
                {
                    MoveAnimal(position);
                    return;
                }
            }
            if (Manager.AnimalWorld.GetCell(position.x, position.y) == -1)
            {
                MoveAnimal(position);
            }
            else if (CanEatIndex(Manager.AnimalWorld.GetCell(position.x, position.y)))
            {
                MoveAnimal(position);
            }
            else if (position.x == CurrentPosition.x)
            {
                if (Manager.AnimalWorld.InBounds(position.x, position.y + 1))
                {
                    if (GetAnimalCell(new Vector2Int(position.x, position.y + 1)) == -1)
                    {
                        MoveAnimal(new Vector2Int(position.x, position.y + 1));
                        return;
                    }
                }
                if (Manager.AnimalWorld.InBounds(position.x, position.y - 1))
                {
                    if (GetAnimalCell(new Vector2Int(position.x, position.y - 1)) == -1)
                    {
                        MoveAnimal(new Vector2Int(position.x, position.y - 1));
                        return;
                    }
                }
            }
            else if (position.y == CurrentPosition.y)
            {
                if (Manager.AnimalWorld.InBounds(position.x + 1, position.y))
                {
                    if (GetAnimalCell(new Vector2Int(position.x + 1, position.y)) == -1)
                    {
                        MoveAnimal(new Vector2Int(position.x + 1, position.y + 1));
                        return;
                    }
                }
                if (Manager.AnimalWorld.InBounds(position.x - 1, position.y))
                {
                    if (GetAnimalCell(new Vector2Int(position.x - 1, position.y)) == -1)
                    {
                        MoveAnimal(new Vector2Int(position.x - 1, position.y));
                        return;
                    }
                }
            }
        }
        private void MoveAnimal(Vector2Int position)
        {
            Vector2Int actualPosition = Manager.AnimalWorld.PutInBounds(position);
            Manager.MoveAnimal(CurrentPosition.x, CurrentPosition.y, actualPosition.x, actualPosition.y, this);
            CurrentPosition = actualPosition;
        }
        #endregion
        #region Death
        public void Kill()
        {
            if (IsAnimal)
            {
                if (Manager.AnimalWorld.GetCell(CurrentPosition.x, CurrentPosition.y) == ProfileIndex)
                    Manager.DeleteAnimal(CurrentPosition.x, CurrentPosition.y);
                Destroy(gameObject);
            }
            else
                Eat();
        }
        public void Kill(AnimalController controller)
        {
            controller.Feed(Profile.Attributes.FoodValue);
            if (IsAnimal)
                Destroy(gameObject);
            else
                Eat();
        }
        private void Eat()
        {
            IsGrown = false;
            TicksToRegrow = (Profile.Attributes.TimeToRegrow / 8) * WorldManager.TICKS_IN_DAY;
            GetComponent<MeshRenderer>().enabled = false;
        }
        public void Feed(int foodValue)
        {
            TicksOfFood += (foodValue * WorldManager.TICKS_IN_DAY) / Profile.Attributes.FoodRequirement;
            if (foodCollectedToMate < Profile.Attributes.FoodToReproduce)
                foodCollectedToMate++;
            if (foodCollectedToMate == Profile.Attributes.FoodToReproduce && Profile.Attributes.RequiresMate == false)
                SelfMate();
        }
        #endregion
        #region Reproduction
        private void Mate()
        {
            foodCollectedToMate = 0;
            MateTarget.Mated();
            MateTarget = null;
            PlaceChild();
        }
        private void SelfMate()
        {
            foodCollectedToMate = 0;
            PlaceChild();
        }
        private void PlaceChild()
        {
            if (Manager.AnimalWorld.InBounds(CurrentPosition.x + 1, CurrentPosition.y))
            {
                if (Manager.AnimalWorld.GetCell(CurrentPosition.x + 1, CurrentPosition.y) == -1)
                {
                    Manager.CreateAnimal(CurrentPosition.x + 1, CurrentPosition.y, ProfileIndex);
                    return;
                }
            }
            if (Manager.AnimalWorld.InBounds(CurrentPosition.x - 1, CurrentPosition.y))
            {
                if (Manager.AnimalWorld.GetCell(CurrentPosition.x - 1, CurrentPosition.y) == -1)
                {
                    Manager.CreateAnimal(CurrentPosition.x - 1, CurrentPosition.y, ProfileIndex);
                    return;
                }
            }
            if (Manager.AnimalWorld.InBounds(CurrentPosition.x, CurrentPosition.y + 1))
            {
                if (Manager.AnimalWorld.GetCell(CurrentPosition.x, CurrentPosition.y + 1) == -1)
                {
                    Manager.CreateAnimal(CurrentPosition.x, CurrentPosition.y + 1, ProfileIndex);
                    return;
                }
            }
            if (Manager.AnimalWorld.InBounds(CurrentPosition.x, CurrentPosition.y - 1))
            {
                if (Manager.AnimalWorld.GetCell(CurrentPosition.x, CurrentPosition.y - 1) == -1)
                {
                    Manager.CreateAnimal(CurrentPosition.x, CurrentPosition.y - 1, ProfileIndex);
                    return;
                }
            }
        }
        public void Mated()
        {
            MateTarget = null;
            foodCollectedToMate = 0;
        }
        public bool OfferMate(AnimalController controller)
        {
            if (MateTarget == null)
            {
                MateTarget = controller;
                return true;
            }
            return false;
        }
        #endregion
        #region Positioning
        private Vector2Int GetAveragePosition(int[] x, int[] y)
        {
            if(x.Length != y.Length)
            {
                Debug.LogError("Positions are not the same size");
                return Vector2Int.zero;
            }

            Vector2Int total = Vector2Int.zero;
            for (int i = 0; i < x.Length; i++)
            {
                total += new Vector2Int(x[i], y[i]);
            }
            return total / x.Length;
        }
        private Vector2Int GetOppositePosition(Vector2Int position)
        {
            Vector2Int difference = CurrentPosition - position;
            return Manager.AnimalWorld.PutInBounds(CurrentPosition + difference + new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2)));
        }
        private Vector2Int GetRandomPosition() => Manager.AnimalWorld.GetRandomPosition();
        private void PlaceInPosition()
        {
            if (GetCurrentAnimalCell() == -1)
            {
                Manager.SetAnimalCell(CurrentPosition.x, CurrentPosition.y, ProfileIndex, this);
                GetComponent<MeshRenderer>().enabled = true;
            }
        }
        #endregion
        #region Scanning
        private bool ClosestInRadius(int index, out int xPos, out int yPos) => Manager.AnimalWorld.ClosestInRadius(index, CurrentPosition.x, CurrentPosition.y, Profile.Attributes.DetectionRange, out xPos, out yPos);
        private int ScanRadius(int index, out int[] xPos, out int[] yPos) => Manager.AnimalWorld.NumberInRadius(index, CurrentPosition.x, CurrentPosition.y, Profile.Attributes.DetectionRange, out xPos, out yPos);
        #endregion
        #region World
        private int GetAnimalCell(Vector2Int position) => Manager.AnimalWorld.GetCell(position.x, position.y);
        private int GetCurrentCell() => Manager.GetGroundCell(CurrentPosition.x, CurrentPosition.y);
        private int GetCurrentAnimalCell() => Manager.GetAnimalCell(CurrentPosition.x, CurrentPosition.y);
        #endregion
        #region Other
        private bool CanEatIndex(int index)
        {
            for (int i = 0; i < SortedEat.Count; i++)
            {
                if (SortedEat[i] == index)
                    return true;
            }
            return false;
        }
        private bool CheckIfCanEat(int index)
        {
            for (int i = 0; i < SortedEat.Count; i++)
            {
                if (SortedEat[i] == index)
                    return true;
            }
            return false;
        }
        public ControllerDataFile CreateDataFile()
        {
            ControllerDataFile data = new ControllerDataFile()
            {
                ID = ID,
                ProfileIndex = ProfileIndex,
                TicksOfFood = TicksOfFood,
                TicksToRegrow = TicksToRegrow,
                TicksToMove = TicksToMove,
                TicksSinceLastRandom = TicksSinceLastRandom,
                IsGrown = IsGrown,
                SortedEat = SortedEat.ToArray(),
                FoodCollectedToMate = foodCollectedToMate,

                CurrentTargetIndexInEat = currentTargetIndexInEat,
                IndexOfTarget = GetIndexOfController(Target),
                IndexOfMate = GetIndexOfController(MateTarget),

                CurrentPositionX = CurrentPosition.x,
                CurrentPositionY = CurrentPosition.y,
                TagetPositionX = TargetPosition.x,
                TagetPositionY= TargetPosition.y
            };
            return data;
        }
        private int GetIndexOfController(AnimalController controller)
        {
            if (controller == null)
                return -1;
            for (int i = 0; i < Manager.Controllers.Count; i++)
            {
                if (controller.ID == Manager.Controllers[i].ID)
                    return i;
            }
            return -1;
        }
        #endregion
    }
}