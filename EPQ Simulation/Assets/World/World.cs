using EPQ.Data;
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
        public T DefaultValue { get; private set; }

        public World(World<T> world)
        {
            X = world.X;
            Y = world.Y;
            DefaultValue = world.DefaultValue;
            worldData = (T[,])world.worldData.Clone();
        }
        public World(WorldDataFile<T> data)
        {
            X = data.X;
            Y = data.Y;
            DefaultValue = data.DefaultValue;
            worldData = data.Data;
        }
        public World(int x, int y)
        {
            X = x;
            Y = y;
            DefaultValue = default(T);
            worldData = new T[x, y];
        }
        public World(int x, int y, T value)
        {
            X = x;
            Y = y;
            DefaultValue = default(T);
            worldData = new T[x, y];

            for (int _x = 0; _x < x; _x++)
            {
                for (int _y = 0; _y < y; _y++)
                {
                    worldData[_x, _y] = value;
                }
            }
        }
        public World(int x, int y, T value, T defaultValue)
        {
            X = x;
            Y = y;
            DefaultValue = defaultValue;
            worldData = new T[x, y];

            for (int _x = 0; _x < x; _x++)
            {
                for (int _y = 0; _y < y; _y++)
                {
                    worldData[_x, _y] = value;
                }
            }
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
        public bool InRange(T value, int x, int y, int range, out int xPos, out int yPos)
        {
            for (int _x = -range; _x <= range; _x++)
            {
                for (int _y = -range; _y <= range; _y++)
                {
                    if (InBounds(x + _x, y + _y))
                    {
                        if (worldData[x + _x, y + _y].Equals(value))
                        {
                            xPos = x + _x;
                            yPos = y + _y;
                            return true;
                        }
                    }
                }
            }
            xPos = 0;
            yPos = 0;
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
        public int NumberInRange(T value, int x, int y, int range, out int[] xPos, out int[] yPos)
        {
            List<int> xp = new List<int>();
            List<int> yp = new List<int>();
            int number = 0;
            for (int _x = -range; _x <= range; _x++)
            {
                for (int _y = -range; _y <= range; _y++)
                {
                    if (InBounds(x + _x, y + _y))
                    {
                        if (worldData[x + _x, y + _y].Equals(value))
                        {
                            xp.Add(x + _x);
                            yp.Add(y + _y);
                            number++;
                        }
                    }
                }
            }
            xPos = xp.ToArray();
            yPos = yp.ToArray();
            return number;
        }
        public bool InExactRange(T value, int x, int y, int range)
        {
            for (int _x = -range; _x <= range; _x++)
            {
                if (InBounds(x + _x, y + -range))
                {
                    if (worldData[x + _x, y + -range].Equals(value))
                        return true;
                }
                if (InBounds(x + _x, y + range))
                {
                    if (worldData[x + _x, y + range].Equals(value))
                        return true;
                }
            }

            for (int _y = 1 - range; _y <= range - 1; _y++)
            {
                if (InBounds(x + -range, y + _y))
                {
                    if (worldData[x + -range, y + _y].Equals(value))
                        return true;
                }
                if (InBounds(x + range, y + _y))
                {
                    if (worldData[x + range, y + _y].Equals(value))
                        return true;
                }
            }

            return false;
        }
        public bool InExactRange(T value, int x, int y, int range, out int xPos, out int yPos)
        {
            for (int _x = -range; _x <= range; _x++)
            {
                if (InBounds(x + _x, y + -range))
                {
                    if (worldData[x + _x, y + -range].Equals(value))
                    {
                        xPos = x + _x;
                        yPos = y + -range;
                        return true;
                    }
                }
                if (InBounds(x + _x, y + range))
                {
                    if (worldData[x + _x, y + range].Equals(value))
                    {
                        xPos = x + _x;
                        yPos = y + range;
                        return true;
                    }
                }
            }

            for (int _y = 1 - range; _y <= range - 1; _y++)
            {
                if (InBounds(x + -range, y + _y))
                {
                    if (worldData[x + -range, y + _y].Equals(value))
                    {
                        xPos = x + -range;
                        yPos = y + _y;
                        return true;
                    }
                }
                if (InBounds(x + range, y + _y))
                {
                    if (worldData[x + range, y + _y].Equals(value))
                    {
                        xPos = x + range;
                        yPos = y + _y;
                        return true;
                    }
                }
            }

            xPos = 0;
            yPos = 0;
            return false;
        }
        public int NumberInExactRange(T value, int x, int y, int range)
        {
            int number = 0;
            for (int _x = -range; _x <= range; _x++)
            {
                if (InBounds(x + _x, y + -range))
                {
                    if (worldData[x + _x, y + -range].Equals(value))
                        number++;
                }
                if (InBounds(x + _x, y + range))
                {
                    if (worldData[x + _x, y + range].Equals(value))
                        number++;
                }
            }

            for (int _y = 1 - range; _y <= range - 1; _y++)
            {
                if (InBounds(x + -range, y + _y))
                {
                    if (worldData[x + -range, y + _y].Equals(value))
                        number++;
                }
                if (InBounds(x + range, y + _y))
                {
                    if (worldData[x + range, y + _y].Equals(value))
                        number++;
                }
            }

            return number;
        }
        public int NumberInExactRange(T value, int x, int y, int range, out int[] xPos, out int[] yPos)
        {
            int number = 0;
            List<int> xp = new List<int>();
            List<int> yp = new List<int>();

            for (int _x = -range; _x <= range; _x++)
            {
                if (InBounds(x + _x, y + -range))
                {
                    if (worldData[x + _x, y + -range].Equals(value))
                    {
                        xp.Add(x + _x);
                        yp.Add(y + -range);
                        number++;
                    }
                }
                if (InBounds(x + _x, y + range))
                {
                    if (worldData[x + _x, y + range].Equals(value))
                    {
                        xp.Add(x + _x);
                        yp.Add(y + range);
                        number++;
                    }
                }
            }

            for (int _y = 1 - range; _y <= range - 1; _y++)
            {
                if (InBounds(x + -range, y + _y))
                {
                    if (worldData[x + -range, y + _y].Equals(value))
                    {
                        xp.Add(x + -range);
                        yp.Add(y + _y);
                        number++;
                    }
                }
                if (InBounds(x + range, y + _y))
                {
                    if (worldData[x + range, y + _y].Equals(value))
                    {
                        xp.Add(x + range);
                        yp.Add(y + _y);
                        number++;
                    }
                }
            }

            xPos = xp.ToArray();
            yPos = yp.ToArray();
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
        public bool InRadius(T value, int x, int y, int radius, out int xPos, out int yPos)
        {
            for (int _x = -radius; _x <= radius; _x++)
            {
                for (int _y = -(radius - Mathf.Abs(_x)); _y <= (radius - Mathf.Abs(_x)); _y++)
                {
                    if (InBounds(x + _x, y + _y))
                    {
                        if (worldData[x + _x, y + _y].Equals(value))
                        {
                            xPos = x + _x;
                            yPos = y + _y;
                            return true;
                        }
                    }
                }
            }
            xPos = 0;
            yPos = 0;
            return false;
        }
        public bool InExactRadius(T value, int x, int y, int radius)
        {
            for (int _x = -radius; _x <= radius; _x++)
            {
                int y1 = radius - Mathf.Abs(_x);
                int y2 = radius - Mathf.Abs(_x);
                if (InBounds(x + _x, y + y1))
                {
                    if (worldData[x + _x, y + y1].Equals(value))
                        return true;
                }
                if (InBounds(x + _x, y + y2))
                {
                    if (worldData[x + _x, y + y2].Equals(value))
                        return true;
                }
            }
            return false;
        }
        public bool InExactRadius(T value, int x, int y, int radius, out int xPos, out int yPos)
        {
            for (int _x = -radius; _x <= radius; _x++)
            {
                int y1 = radius - Mathf.Abs(_x);
                int y2 = radius - Mathf.Abs(_x);
                if (InBounds(x + _x, y + y1))
                {
                    if (worldData[x + _x, y + y1].Equals(value))
                    {
                        xPos = x + _x;
                        yPos = y + y1;
                        return true;
                    }
                }
                if (InBounds(x + _x, y + y2))
                {
                    if (worldData[x + _x, y + y2].Equals(value))
                    {
                        xPos = x + _x;
                        yPos = y + y2;
                        return true;
                    }
                }
            }
            xPos = 0;
            yPos = 0;
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
        public int NumberInRadius(T value, int x, int y, int radius, out int[] xPos, out int[] yPos)
        {
            List<int> xp = new List<int>();
            List<int> yp = new List<int>();
            int number = 0;
            for (int _x = -radius; _x <= radius; _x++)
            {
                for (int _y = -(radius - Mathf.Abs(_x)); _y <= (radius - Mathf.Abs(_x)); _y++)
                {
                    if (InBounds(x + _x, y + _y))
                    {
                        if (worldData[x + _x, y + _y].Equals(value))
                        {
                            xp.Add(x + _x);
                            yp.Add(y + _y);
                            number++;
                        }
                    }
                }
            }
            xPos = xp.ToArray();
            yPos = yp.ToArray();
            return number;
        }
        public int NumberInExactRadius(T value, int x, int y, int radius)
        {
            int number = 0;
            for (int _x = -radius; _x <= radius; _x++)
            {
                int y1 = radius - Mathf.Abs(_x);
                int y2 = radius - Mathf.Abs(_x);
                if (InBounds(x + _x, y + y1))
                {
                    if (worldData[x + _x, y + y1].Equals(value))
                        number++;
                }
                if (InBounds(x + _x, y + y2))
                {
                    if (worldData[x + _x, y + y2].Equals(value))
                        number++;
                }
            }
            return number;
        }
        public int NumberInExactRadius(T value, int x, int y, int radius, out int[] xPos, out int[] yPos)
        {
            List<int> xp = new List<int>();
            List<int> yp = new List<int>();
            int number = 0;
            for (int _x = -radius; _x <= radius; _x++)
            {
                int y1 = radius - Mathf.Abs(_x);
                int y2 = radius - Mathf.Abs(_x);
                if (InBounds(x + _x, y + y1))
                {
                    if (worldData[x + _x, y + y1].Equals(value))
                    {
                        xp.Add(x + _x);
                        yp.Add(y + y1);
                        number++;
                    }
                }
                if (InBounds(x + _x, y + y2))
                {
                    if (worldData[x + _x, y + y2].Equals(value))
                    {
                        xp.Add(x + _x);
                        yp.Add(y + y2);
                        number++;
                    }
                }
            }
            xPos = xp.ToArray();
            yPos = yp.ToArray();
            return number;
        }
        public bool ClosestInRange(T value, int x, int y, int range, out int xPos, out int yPos)
        {
            for (int i = 1; i <= range; i++)
            {
                if(InExactRange(value, x, y, i, out int xResult, out int yResult))
                {
                    xPos = xResult;
                    yPos = yResult;
                    return true;
                }
            }
            xPos = 0;
            yPos = 0;
            return false;
        }
        public int ClosestNumberInRange(T value, int x, int y, int range, out int[] xPos, out int[] yPos)
        {
            for (int i = 1; i <= range; i++)
            {
                int number = NumberInExactRange(value, x, y, i, out int[] xResult, out int[] yResult);
                if (number > 0)
                {
                    xPos = xResult;
                    yPos = yResult;
                    return number;
                }
            }
            xPos = new int[0];
            yPos = new int[0];
            return 0;
        }
        public bool ClosestInRadius(T value, int x, int y, int radius, out int xPos, out int yPos)
        {
            for (int i = 1; i <= radius; i++)
            {
                if (InExactRadius(value, x, y, i, out int xResult, out int yResult))
                {
                    xPos = xResult;
                    yPos = yResult;
                    return true;
                }
            }
            xPos = 0;
            yPos = 0;
            return false;
        }
        public int ClosestNumberInRadius(T value, int x, int y, int range, out int[] xPos, out int[] yPos)
        {
            for (int i = 1; i <= range; i++)
            {
                int number = NumberInExactRadius(value, x, y, i, out int[] xResult, out int[] yResult);
                if (number > 0)
                {
                    xPos = xResult;
                    yPos = yResult;
                    return number;
                }
            }
            xPos = new int[0];
            yPos = new int[0];
            return 0;
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0
                && x < X
                && y >= 0
                && y < Y;
        }

        public void MoveValue(int startX, int startY, int endX, int endY)
        {
            MoveValue(startX, startY, endX, endY, DefaultValue);
        }
        public void MoveValue(int startX, int startY, int endX, int endY, T replaceValue)
        {
            worldData[endX, endY] = worldData[startX, startY];
            worldData[startX, startY] = replaceValue;
        }
    }
}