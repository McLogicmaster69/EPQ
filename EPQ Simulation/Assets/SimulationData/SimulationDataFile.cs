using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// Contains information about a simulation that can be saved
    /// </summary>
    [System.Serializable]
    public class SimulationDataFile
    {
        public string Version;

        public int CurrentID;

        public WorldDataFile<int> Ground;
        public WorldDataFile<int> Animals;
        public CompiledAnimalDataFile[] Profiles;
        public ControllerDataFile[] Controllers;
    }
}