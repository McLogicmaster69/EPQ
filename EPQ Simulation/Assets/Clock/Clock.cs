using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ
{
    /// <summary>
    /// Gives a constant and regular tick output
    /// </summary>
    public class Clock : MonoBehaviour
    {
        public static Clock main;

        /// <summary>
        /// If the clock is giving out a tick
        /// </summary>
        public bool IsTicking { get; set; } = false;

        /// <summary>
        /// The interval that a tick occurs
        /// </summary>
        public float Interval { get; set; } = 0.1f;

        /// <summary>
        /// If the previous tick has been handled
        /// </summary>
        public bool HandledTick { get; set; } = true;

        /// <summary>
        /// Called when the clock ticks
        /// </summary>
        public event TickEvent Tick;

        private float _counter = 0f;

        private void Awake()
        {
            main = this;
        }
        private void Update()
        {
            if (IsTicking)
            {
                if(_counter <= 0f)
                {
                    _counter = Interval;
                    HandledTick = false;
                    DoTick();
                }
                if(HandledTick)
                    _counter -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Forces the next tick to occur
        /// </summary>
        public void DoTick()
        {
            Tick?.Invoke(this, new TickEventArgs());
        }
    }
}