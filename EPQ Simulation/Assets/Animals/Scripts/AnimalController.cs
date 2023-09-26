using EPQ.Compiler;
using EPQ.Data;
using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    /// <summary>
    /// Controls the animal within the simulation
    /// </summary>
    public class AnimalController : MonoBehaviour
    {
        /// <summary>
        /// The ID of the animal
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// The index of the animal profile associated with this animal
        /// </summary>
        public int ProfileIndex { get; private set; }

        /// <summary>
        /// If it is an animal or a plant
        /// </summary>
        public bool IsAnimal { get { return _profile.Attributes.IsAnimal; } }

        /// <summary>
        /// How many ticks of food until it dies
        /// </summary>
        public int TicksOfFood { get; private set; }

        /// <summary>
        /// How many ticks until it regrows
        /// </summary>
        public int TicksToRegrow { get; private set; }

        /// <summary>
        /// How many ticks until it moves
        /// </summary>
        public int TicksToMove { get; private set; }

        /// <summary>
        /// How many ticks since it moved to a random position
        /// </summary>
        public int TicksSinceLastRandom { get; private set; }

        /// <summary>
        /// The current position of the animal
        /// </summary>
        public Vector2Int CurrentPosition { get; private set; }

        /// <summary>
        /// If the plant is grown and available to eat
        /// </summary>
        public bool IsGrown { get; private set; }

        /// <summary>
        /// A sorted list of what the animal cat eat
        /// </summary>
        public List<int> SortedEat { get; private set; }

        private CompiledAnimalProfile _profile;
        private Material _material;
        private WorldManager _manager;

        private AnimalController _target;
        private AnimalController _mateTarget;
        private int _currentTargetIndexInEat = -1;
        private Vector2Int _targetPosition = Vector2Int.zero;
        private int _foodCollectedToMate = 0;

        private const int RANDOM_TICKS = 30;

        #region Initialisation
        /// <summary>
        /// Initialises the animal with a profile index, the starting position, a compiled profile and the id
        /// </summary>
        /// <param name="index">The profile index</param>
        /// <param name="position">Starting position</param>
        /// <param name="profile">Compiled animal profile</param>
        /// <param name="id">Animal ID</param>
        public void InitAnimal(int index, Vector2Int position, CompiledAnimalProfile profile, int id)
        {
            _manager = WorldManager.main;
            ID = id;
            _profile = profile;
            ProfileIndex = index;
            CurrentPosition = position;

            InitAttributes();
            InitMaterial();
            InitEat();
        }

        /// <summary>
        /// Initialises the animal following what the data files says
        /// </summary>
        /// <param name="file">The data file</param>
        public void InitAnimal(ControllerDataFile file)
        {
            _manager = WorldManager.main;
            InitAttributes(file);
            InitMaterial();
        }

        private void InitAttributes()
        {
            TicksOfFood = _profile.Attributes.FoodRequirement * WorldManager.STARTING_FOOD * WorldManager.TICKS_IN_DAY;
            TicksToRegrow = 1;
            TicksToMove = 1;
            IsGrown = true;
            _targetPosition = GetRandomPosition();
            transform.position = new Vector3(CurrentPosition.x, 1f, CurrentPosition.y);
        }
        private void InitMaterial()
        {
            _material = new Material(_manager.BlankMaterial);
            _material.color = _profile.Color;
            GetComponent<MeshRenderer>().material = _material;
        }
        private void InitEat()
        {
            SortedEat = new List<int>();
            List<CompiledAnimalProfile> profiles = new List<CompiledAnimalProfile>();
            for (int i = 0; i < _profile.CanEat.Count; i++)
            {
                profiles.Add(_manager.GetProfile(_profile.CanEat[i]));
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

            _currentTargetIndexInEat = SortedEat.Count;
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
            _currentTargetIndexInEat = file.CurrentTargetIndexInEat;
            _foodCollectedToMate = file.FoodCollectedToMate;
            _targetPosition = new Vector2Int(file.TagetPositionX, file.TagetPositionY);
            _profile = _manager.GetProfile(file.ProfileIndex);
            transform.position = new Vector3(CurrentPosition.x, 1f, CurrentPosition.y);
        }

        /// <summary>
        /// Initialises the targets of the animal following what the data file says
        /// </summary>
        /// <param name="file">The data file</param>
        public void InitTargets(ControllerDataFile file)
        {
            if(file.IndexOfTarget != -1)
                _target = _manager.Controllers[file.IndexOfTarget];
            if(file.IndexOfMate != -1)
                _mateTarget = _manager.Controllers[file.IndexOfMate];
        }
        #endregion
        #region Tick
        /// <summary>
        /// Runs a tick of behaviour on the animal
        /// </summary>
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
                    TicksToMove = (11 - _profile.Attributes.GrassSpeed);
                else
                    TicksToMove = (11 - _profile.Attributes.WaterSpeed);
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
            if(_mateTarget != null)
            {
                if(CurrentPosition.x == _mateTarget.CurrentPosition.x)
                {
                    if(Mathf.Abs(CurrentPosition.y - _mateTarget.CurrentPosition.y) == 1)
                    {
                        Mate();
                    }
                }
                else if(CurrentPosition.y == _mateTarget.CurrentPosition.y)
                {
                    if (Mathf.Abs(CurrentPosition.x - _mateTarget.CurrentPosition.x) == 1)
                    {
                        Mate();
                    }
                }
            }
        }
        private void HandleTarget()
        {
            if(_foodCollectedToMate == _profile.Attributes.FoodToReproduce || _mateTarget != null)
            {
                if(_mateTarget == null)
                {
                    if(_manager.AnimalWorld.ClosestInRadius(ProfileIndex, CurrentPosition.x, CurrentPosition.y, _profile.Attributes.DetectionRange,out int xPos, out int yPos))
                    {
                        AnimalController controller = _manager.ControllersGrid.GetCell(xPos, yPos);
                        if (controller.OfferMate(this))
                        {
                            _mateTarget = controller;
                        }
                    }
                }
            }

            if (_mateTarget != null)
                return;

            if(_target == null)
            {
                _currentTargetIndexInEat = SortedEat.Count;
            }

            for (int i = 0; i < _currentTargetIndexInEat; i++)
            {
                if (ClosestInRadius(SortedEat[i], out int xPos, out int yPos))
                {
                    _target = _manager.ControllersGrid.GetCell(xPos, yPos);
                    _currentTargetIndexInEat = i;
                    return;
                }
            }

            if (_target != null)
            {
                if(_target.IsAnimal == false && _target.IsGrown == false)
                {
                    _target = null;
                    _currentTargetIndexInEat = SortedEat.Count;
                    return;
                }
            }
        }
        private void HandleTargetPosition()
        {
            if(_mateTarget != null)
            {
                _targetPosition = _mateTarget.CurrentPosition;
            }
            else if (_target != null)
            {
                _targetPosition = _target.CurrentPosition;
            }
            else
            {
                for (int i = 0; i < _profile.CanBeEatenBy.Count; i++)
                {
                    if(ScanRadius(_profile.CanBeEatenBy[i], out int[] xPos, out int[] yPos) > 0)
                    {
                        Vector2Int average = GetAveragePosition(xPos, yPos);
                        _targetPosition = GetOppositePosition(average);
                        return;
                    }
                }

                if (CurrentPosition == _targetPosition || TicksSinceLastRandom >= RANDOM_TICKS)
                {
                    TicksSinceLastRandom = 0;
                    _targetPosition = GetRandomPosition();
                }

                TicksSinceLastRandom++;
            }
        }
        private void HandleExistence()
        {
            AnimalController controller = _manager.ControllersGrid.GetCell(CurrentPosition.x, CurrentPosition.y);
            if (controller.ID != this.ID)
            {
                if (CheckIfCanEat(controller.ProfileIndex))
                {
                    controller.Kill(this);
                    _manager.SetAnimalCell(CurrentPosition.x, CurrentPosition.y, ProfileIndex, this);
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
            if (CurrentPosition.x != _targetPosition.x && CurrentPosition.y != _targetPosition.y)
            {
                if (Random.Range(0, 2) == 0)
                    MoveX();
                else
                    MoveY();
            }
            else if (CurrentPosition.x != _targetPosition.x)
            {
                MoveX();
            }
            else if (CurrentPosition.y != _targetPosition.y)
            {
                MoveY();
            }
        }
        private void MoveX()
        {
            if (CurrentPosition.x < _targetPosition.x)
                TryMove(new Vector2Int(CurrentPosition.x + 1, CurrentPosition.y));
            else
                TryMove(new Vector2Int(CurrentPosition.x - 1, CurrentPosition.y));
        }
        private void MoveY()
        {
            if (CurrentPosition.y < _targetPosition.y)
                TryMove(new Vector2Int(CurrentPosition.x, CurrentPosition.y + 1));
            else
                TryMove(new Vector2Int(CurrentPosition.x, CurrentPosition.y - 1));
        }
        private void TryMove(Vector2Int position)
        {
            if(_target != null)
            {
                if(position == _target.CurrentPosition)
                {
                    MoveAnimal(position);
                    return;
                }
            }
            if (_manager.AnimalWorld.GetCell(position.x, position.y) == -1)
            {
                MoveAnimal(position);
            }
            else if (CanEatIndex(_manager.AnimalWorld.GetCell(position.x, position.y)))
            {
                MoveAnimal(position);
            }
            else if (position.x == CurrentPosition.x)
            {
                if (_manager.AnimalWorld.InBounds(position.x, position.y + 1))
                {
                    if (GetAnimalCell(new Vector2Int(position.x, position.y + 1)) == -1)
                    {
                        MoveAnimal(new Vector2Int(position.x, position.y + 1));
                        return;
                    }
                }
                if (_manager.AnimalWorld.InBounds(position.x, position.y - 1))
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
                if (_manager.AnimalWorld.InBounds(position.x + 1, position.y))
                {
                    if (GetAnimalCell(new Vector2Int(position.x + 1, position.y)) == -1)
                    {
                        MoveAnimal(new Vector2Int(position.x + 1, position.y + 1));
                        return;
                    }
                }
                if (_manager.AnimalWorld.InBounds(position.x - 1, position.y))
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
            Vector2Int actualPosition = _manager.AnimalWorld.PutInBounds(position);
            _manager.MoveAnimal(CurrentPosition.x, CurrentPosition.y, actualPosition.x, actualPosition.y, this);
            CurrentPosition = actualPosition;
        }
        #endregion
        #region Death
        /// <summary>
        /// Kills the animal or sets the plant to not being available for eating
        /// </summary>
        public void Kill()
        {
            if (IsAnimal)
            {
                if (_manager.AnimalWorld.GetCell(CurrentPosition.x, CurrentPosition.y) == ProfileIndex)
                    _manager.DeleteAnimal(CurrentPosition.x, CurrentPosition.y);
                Destroy(gameObject);
            }
            else
                Eat();
        }

        /// <summary>
        /// Kills the animal or sets the plant to not being available for eating. Feeds the controller eating it.
        /// </summary>
        /// <param name="controller"></param>
        public void Kill(AnimalController controller)
        {
            controller.Feed(_profile.Attributes.FoodValue);
            if (IsAnimal)
                Destroy(gameObject);
            else
                Eat();
        }
        private void Eat()
        {
            IsGrown = false;
            TicksToRegrow = (_profile.Attributes.TimeToRegrow / 8) * WorldManager.TICKS_IN_DAY;
            GetComponent<MeshRenderer>().enabled = false;
        }

        /// <summary>
        /// Feeds the animal
        /// </summary>
        /// <param name="foodValue">The amount of food</param>
        public void Feed(int foodValue)
        {
            TicksOfFood += (foodValue * WorldManager.TICKS_IN_DAY) / _profile.Attributes.FoodRequirement;
            if (_foodCollectedToMate < _profile.Attributes.FoodToReproduce)
                _foodCollectedToMate++;
            if (_foodCollectedToMate == _profile.Attributes.FoodToReproduce && _profile.Attributes.RequiresMate == false)
                SelfMate();
        }
        #endregion
        #region Reproduction
        private void Mate()
        {
            _foodCollectedToMate = 0;
            _mateTarget.Mated();
            _mateTarget = null;
            PlaceChild();
        }
        private void SelfMate()
        {
            _foodCollectedToMate = 0;
            PlaceChild();
        }
        private void PlaceChild()
        {
            if (_manager.AnimalWorld.InBounds(CurrentPosition.x + 1, CurrentPosition.y))
            {
                if (_manager.AnimalWorld.GetCell(CurrentPosition.x + 1, CurrentPosition.y) == -1)
                {
                    _manager.CreateAnimal(CurrentPosition.x + 1, CurrentPosition.y, ProfileIndex);
                    return;
                }
            }
            if (_manager.AnimalWorld.InBounds(CurrentPosition.x - 1, CurrentPosition.y))
            {
                if (_manager.AnimalWorld.GetCell(CurrentPosition.x - 1, CurrentPosition.y) == -1)
                {
                    _manager.CreateAnimal(CurrentPosition.x - 1, CurrentPosition.y, ProfileIndex);
                    return;
                }
            }
            if (_manager.AnimalWorld.InBounds(CurrentPosition.x, CurrentPosition.y + 1))
            {
                if (_manager.AnimalWorld.GetCell(CurrentPosition.x, CurrentPosition.y + 1) == -1)
                {
                    _manager.CreateAnimal(CurrentPosition.x, CurrentPosition.y + 1, ProfileIndex);
                    return;
                }
            }
            if (_manager.AnimalWorld.InBounds(CurrentPosition.x, CurrentPosition.y - 1))
            {
                if (_manager.AnimalWorld.GetCell(CurrentPosition.x, CurrentPosition.y - 1) == -1)
                {
                    _manager.CreateAnimal(CurrentPosition.x, CurrentPosition.y - 1, ProfileIndex);
                    return;
                }
            }
        }

        /// <summary>
        /// Signals that the animal has mated
        /// </summary>
        public void Mated()
        {
            _mateTarget = null;
            _foodCollectedToMate = 0;
        }

        /// <summary>
        /// Checks if the animal is available for mating
        /// </summary>
        /// <param name="controller">The other mate</param>
        /// <returns></returns>
        public bool OfferMate(AnimalController controller)
        {
            if (_mateTarget == null)
            {
                _mateTarget = controller;
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
            return _manager.AnimalWorld.PutInBounds(CurrentPosition + difference + new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2)));
        }
        private Vector2Int GetRandomPosition() => _manager.AnimalWorld.GetRandomPosition();
        private void PlaceInPosition()
        {
            if (GetCurrentAnimalCell() == -1)
            {
                _manager.SetAnimalCell(CurrentPosition.x, CurrentPosition.y, ProfileIndex, this);
                GetComponent<MeshRenderer>().enabled = true;
            }
        }
        #endregion
        #region Scanning
        private bool ClosestInRadius(int index, out int xPos, out int yPos) => _manager.AnimalWorld.ClosestInRadius(index, CurrentPosition.x, CurrentPosition.y, _profile.Attributes.DetectionRange, out xPos, out yPos);
        private int ScanRadius(int index, out int[] xPos, out int[] yPos) => _manager.AnimalWorld.NumberInRadius(index, CurrentPosition.x, CurrentPosition.y, _profile.Attributes.DetectionRange, out xPos, out yPos);
        #endregion
        #region World
        private int GetAnimalCell(Vector2Int position) => _manager.AnimalWorld.GetCell(position.x, position.y);
        private int GetCurrentCell() => _manager.GetGroundCell(CurrentPosition.x, CurrentPosition.y);
        private int GetCurrentAnimalCell() => _manager.GetAnimalCell(CurrentPosition.x, CurrentPosition.y);
        #endregion
        #region Other
        /// <summary>
        /// Creates a data file containing the information inside the animal
        /// </summary>
        /// <returns>A data file</returns>
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
                FoodCollectedToMate = _foodCollectedToMate,

                CurrentTargetIndexInEat = _currentTargetIndexInEat,
                IndexOfTarget = GetIndexOfController(_target),
                IndexOfMate = GetIndexOfController(_mateTarget),

                CurrentPositionX = CurrentPosition.x,
                CurrentPositionY = CurrentPosition.y,
                TagetPositionX = _targetPosition.x,
                TagetPositionY = _targetPosition.y
            };
            return data;
        }
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
        private int GetIndexOfController(AnimalController controller)
        {
            if (controller == null)
                return -1;
            for (int i = 0; i < _manager.Controllers.Count; i++)
            {
                if (controller.ID == _manager.Controllers[i].ID)
                    return i;
            }
            return -1;
        }
        #endregion
    }
}