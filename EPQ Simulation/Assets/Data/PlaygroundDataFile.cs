using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// Stores data about a food web in a file that can be saved
    /// </summary>
    [System.Serializable]
    public class PlaygroundDataFile
    {
        public int CurrentLineID { get; set; }
    }
}