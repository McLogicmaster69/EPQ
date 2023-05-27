using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    [System.Serializable]
    public class DataFile
    {
        //
        // ANIMALS
        //

        public AnimalUIDataFile AnimalUI;
        public AnimalProfileDataFile[] AnimalProfiles;

        //
        // FOODWEB
        //

        public PlaygroundDataFile Playground;
        public NodeDataFile[] Nodes;
        public LineConnectionDataFile[] LineConnections;

        //
        // WORLD
        //
    }
}