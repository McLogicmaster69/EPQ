using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// Stores information created by the user that is required to create a simulation that can be saved
    /// </summary>
    [System.Serializable]
    public class DataFile
    {
        public string Version { get; set; }

        //
        // ANIMALS
        //

        public AnimalUIDataFile AnimalUI { get; set; }
        public AnimalProfileDataFile[] AnimalProfiles { get; set; }

        //
        // FOODWEB
        //

        public PlaygroundDataFile Playground { get; set; }
        public NodeDataFile[] Nodes { get; set; }
        public LineConnectionDataFile[] LineConnections { get; set; }

        //
        // WORLD
        //
    }
}