using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    /// <summary>
    /// Contains data about a node to be saved
    /// </summary>
    [System.Serializable]
    public class NodeDataFile
    {
        /// <summary>
        /// Where the node is located in the food web
        /// </summary>
        public float[] NodePosition { get; set; }
    }
}