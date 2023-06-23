using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ
{
    public class Clock : MonoBehaviour
    {
        public static Clock main;

        public event EventHandler<TickEventArgs> Tick;

        public class TickEventArgs
        {
        }

        private void Awake()
        {
            main = this;
        }
    }
}