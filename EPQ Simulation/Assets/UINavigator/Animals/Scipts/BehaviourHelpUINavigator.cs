using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EPQ.Animals
{
    public class BehaviourHelpUINavigator : MonoBehaviour
    {
        public static BehaviourHelpUINavigator main;

        public GameObject UI;
        public TMP_Text Attribute;
        public TMP_Text Description;

        private void Awake()
        {
            main = this;
        }
        private void Start()
        {
            UI.SetActive(false);
        }

        public void ShowUI(HelpMessage help)
        {
            Attribute.text = help.AttributeName;
            Description.text = help.AttributeDescription;
            UI.SetActive(true);
        }
        public void HideUI()
        {
            UI.SetActive(false);
        }
    }
}