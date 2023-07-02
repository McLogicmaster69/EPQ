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
        public bool IsTicking = false;
        public float Interval = 0.1f;

        private float counter = 0f;

        public class TickEventArgs
        {
        }

        private void Awake()
        {
            main = this;
        }
        private void Update()
        {
            if (IsTicking)
            {
                if(counter <= 0f)
                {
                    counter = Interval;
                    DoTick();
                }
                counter -= Time.deltaTime;
            }
        }

        public void DoTick()
        {
            Tick?.Invoke(this, new TickEventArgs());
        }
    }
}