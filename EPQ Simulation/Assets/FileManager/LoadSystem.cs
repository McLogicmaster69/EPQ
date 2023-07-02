using EPQ.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Files
{
    public class LoadSystem<T>
    {
        public string Version { get; private set; }
        public Action<T>[] Methods { get; private set; }

        public LoadSystem(string version, Action<T>[] methods)
        {
            Version = version;
            Methods = methods;
        }

        public void CallMethods(T file)
        {
            foreach(Action<T> method in Methods)
            {
                method(file);
            }
        }
    }
}