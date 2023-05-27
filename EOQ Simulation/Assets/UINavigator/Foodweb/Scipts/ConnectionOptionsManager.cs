using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EPQ.Foodweb.Connections
{
    public class ConnectionOptionsManager : MonoBehaviour
    {
        public NodeLineConnector Connector;
        public LineConnection Profile;
        public AnimalProfile Target1;
        public AnimalProfile Target2;

        public TMP_Text Target1Text;
        public TMP_Text Target2Text;
        public TMP_Text TwoWayText;

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