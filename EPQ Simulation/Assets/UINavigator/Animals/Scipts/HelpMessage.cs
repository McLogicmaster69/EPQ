using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    /// <summary>
    /// Holds information required to display a help message to the user
    /// </summary>
    [System.Serializable]
    public class HelpMessage
    {
        public string AttributeName;
        [Multiline] public string AttributeDescription;
    }
}