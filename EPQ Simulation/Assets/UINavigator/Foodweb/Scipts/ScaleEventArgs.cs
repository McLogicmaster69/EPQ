using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Foodweb
{
    /// <summary>
    /// Arguments for when a scale event occur
    /// </summary>
    public class ScaleEventArgs
    {
        public Vector2 Centre;
        public float OldScale;
        public float NewScale;
    }
}