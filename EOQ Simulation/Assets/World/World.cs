using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Worlds
{
    public class World<T>
    {
        private T[,] worldData;

        public T[,] world { get { return worldData; } }
        public int X { get; private set; }
        public int Y { get; private set; }

        public World(World<T> world)
        {
            X = world.X;
            Y = world.Y;
            worldData = (T[,])world.worldData.Clone();
        }
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
        public void SetWorld(World<T> world)
        {
            X = world.X;
            Y = world.Y;
            worldData = (T[,])world.worldData.Clone();
        }
        public World<T> Clone()
        {
            return new World<T>(this);
        }
        public bool InRange(T value, int x, int y, int range)
        {
            for (int _x = -range; _x <= range; _x++)
            {
                for (int _y = -range; _y <= range; _y++)
                {
                    if (InBounds(x + _x, y + _y))
                    {
                        if (worldData[x + _x, y + _y].Equals(value))
                            return true;
                    }
                }
            }
            return false;
        }
        public int NumberInRange(T value, int x, int y, int range)
        {
            int number = 0;
            for (int _x = -range; _x <= range; _x++)
            {
                for (int _y = -range; _y <= range; _y++)
                {
                    if (InBounds(x + _x, y + _y))
                    {
                        if (worldData[x + _x, y + _y].Equals(value))
                            number++;
                    }
                }
            }
            return number;
        }
        public bool InRadius(T value, int x, int y, int radius)
        {
            for (int _x = -radius; _x <= radius; _x++)
            {
                for (int _y = -(radius - Mathf.Abs(_x)); _y <= (radius - Mathf.Abs(_x)); _y++)
                {
                    if (InBounds(x + _x, y + _y))
                    {
                        if (worldData[x + _x, y + _y].Equals(value))
                            return true;
                    }
                }
            }
            return false;
        }
        public int NumberInRadius(T value, int x, int y, int radius)
        {
            int number = 0;
            for (int _x = -radius; _x <= radius; _x++)
            {
                for (int _y = -(radius - Mathf.Abs(_x)); _y <= (radius - Mathf.Abs(_x)); _y++)
                {
                    if (InBounds(x + _x, y + _y))
                    {
                        if (worldData[x + _x, y + _y].Equals(value))
                            number++;
                    }
                }
            }
            return number;
        }
        public bool InBounds(int x, int y)
        {
            return x >= 0
                && x < X
                && y >= 0
                && y < Y;
        }
    }
}