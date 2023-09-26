using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    [System.Serializable]
    public class LineConnectionDataFile
    {
        public int ID1 { get; set; }
        public int ID2 { get; set; }
        public bool TwoWay { get; set; }
        public int LineID { get; set; }
    }
}