using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    [System.Serializable]
    public class ControllerDataFile
    {
        public int ID;
        public int ProfileIndex;
        public int TicksOfFood;
        public int TicksToRegrow;
        public int TicksToMove;
        public int TicksSinceLastRandom;
        public bool IsGrown;
        public int[] SortedEat;
        public int FoodCollectedToMate;

        public int CurrentTargetIndexInEat;
        public int IndexOfTarget;
        public int IndexOfMate;

        public int CurrentPositionX;
        public int CurrentPositionY;
        public int TagetPositionX;
        public int TagetPositionY;
    }
}