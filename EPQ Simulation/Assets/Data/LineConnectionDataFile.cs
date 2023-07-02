using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Data
{
    [System.Serializable]
    public class LineConnectionDataFile
    {
        public int ID1;
        public int ID2;
        public bool TwoWay;
        public int LineID;
    }
}