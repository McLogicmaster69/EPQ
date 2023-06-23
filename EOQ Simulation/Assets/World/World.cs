using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Worlds
{
    public class World<T>
    {
        private T[,] worldData;
        public T[,] world { get { return worldData; } }

        public readonly int X;
        public readonly int Y;

        public World(int x, int y)
        {
            X = x;
            Y = y;
            worldData = new T[x, y];
        }

        public void SetCell(int x, int y, T value)
        {
            worldData[x, y] = value;
        }
        public T GetCell(int x, int y)
        {
            return worldData[x, y];
        }
    }
}