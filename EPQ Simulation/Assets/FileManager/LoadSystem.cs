using EPQ.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Files
{
    /// <summary>
    /// Contains a load method that is ran under a specific version
    /// </summary>
    /// <typeparam name="FileType"></typeparam>
    public class LoadSystem<FileType>
    {
        public string Version { get; private set; }
        public LoadMethod<FileType>[] Methods { get; private set; }

        /// <summary>
        /// Creates a new load system
        /// </summary>
        /// <param name="version"></param>
        /// <param name="methods"></param>
        public LoadSystem(string version, LoadMethod<FileType>[] methods)
        {
            Version = version;
            Methods = methods;
        }

        public void CallMethods(FileType file)
        {
            foreach(LoadMethod<FileType> method in Methods)
            {
                method(file);
            }
        }
    }
}