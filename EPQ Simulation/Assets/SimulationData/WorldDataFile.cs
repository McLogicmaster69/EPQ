using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// Contains data about a world that can be saved
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class WorldDataFile<T>
    {
        public int X;
        public int Y;
        public T[,] Data;
        public T DefaultValue;

        public WorldDataFile(World<T> world)
        {
            X = world.X;
            Y = world.Y;
            Data = world.world;
            DefaultValue = world.DefaultValue;
        }
    }
}