using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EPQ.Animals
{
    /// <summary>
    /// Manages the UI help prompts given within the behaviour UI
    /// </summary>
    public class BehaviourHelpUINavigator : MonoBehaviour
    {
        public static BehaviourHelpUINavigator main;

        [SerializeField] private GameObject UI;
        [SerializeField] private TMP_Text Attribute;
        [SerializeField] private TMP_Text Description;

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