using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EPQ.Foodweb.Connections
{
    /// <summary>
    /// Manages the connection between two nodes on the food web
    /// </summary>
    public class ConnectionOptionsManager : MonoBehaviour
    {
        /// <summary>
        /// The visual connection between two nodes
        /// </summary>
        public NodeLineConnector Connector { get; set; }

        /// <summary>
        /// The information about the connection
        /// </summary>
        public LineConnection Profile { get; set; }

        /// <summary>
        /// The animal being eaten
        /// </summary>
        public AnimalProfile Target1 { get; set; }

        /// <summary>
        /// The animal that is eating
        /// </summary>
        public AnimalProfile Target2 { get; set; }

        [SerializeField] private TMP_Text Target1Text;
        [SerializeField] private TMP_Text Target2Text;
        [SerializeField] private TMP_Text TwoWayText;

        private void Update()
        {
            if (Connector == null)
                return;
            Target1Text.text = Target1.Name;
            Target2Text.text = Target2.Name;
            TwoWayText.text = Profile.TwoWay ? "<-->" : "--->";
        }

        public void DeleteConnection()
        {
            PlaygroundNavigator.main.RemoveConnection(Connector.ID);
            CloseUI();
        }
        public void CloseUI()
        {
            gameObject.SetActive(false);
        }
        public void SwapAnimals()
        {
            int IDtemp = Profile.ID1;
            Profile.ID1 = Profile.ID2;
            Profile.ID2 = IDtemp;

            AnimalProfile animalTemp = Target1;
            Target1 = Target2;
            Target2 = animalTemp;
        }
        public void ToggleTwoWay()
        {
            Profile.TwoWay = !Profile.TwoWay;
        }
    }
}