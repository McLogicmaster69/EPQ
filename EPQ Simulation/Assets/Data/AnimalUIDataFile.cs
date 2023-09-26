using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// Contains data about the animal UI to be saved
    /// </summary>
    [System.Serializable]
    public class AnimalUIDataFile
    {
        /// <summary>
        /// The newest ID to be used
        /// </summary>
        public int CurrentIDCount { get; set; }
    }
}