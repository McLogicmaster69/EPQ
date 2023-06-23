using EPQ.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Files
{
    public class LoadSystem
    {
        public string Version { get; private set; }
        public Action<DataFile>[] Methods { get; private set; }

        public LoadSystem(string version, Action<DataFile>[] methods)
        {
            Version = version;
            Methods = methods;
        }

        public void CallMethods(DataFile file)
        {
            foreach(Action<DataFile> method in Methods)
            {
                method(file);
            }
        }
    }
}