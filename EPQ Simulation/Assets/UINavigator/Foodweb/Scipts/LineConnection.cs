using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Foodweb.Connections
{
    /// <summary>
    /// Hold information about a connection between two animals
    /// </summary>
    public class LineConnection
    {
        public int ID1 { get; set; }
        public int ID2 { get; set; }
        public bool TwoWay { get; set; }
        public int LineID { get; set; }
        public GameObject GameObject { get; set; }
    }
}