using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    [System.Serializable]
    public class HelpMessage
    {
        public string AttributeName;
        [Multiline] public string AttributeDescription;
    }
}