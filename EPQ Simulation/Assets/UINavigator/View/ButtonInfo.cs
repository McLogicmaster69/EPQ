using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EPQ.View
{
    /// <summary>
    /// Contains information about a button
    /// </summary>
    [System.Serializable]
    public class ButtonInfo
    {
        public Button Button;
        public Image Image;
        public TMP_Text Text;
    }
}