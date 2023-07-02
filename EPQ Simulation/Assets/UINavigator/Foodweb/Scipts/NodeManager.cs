using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EPQ.Animals.AnimalProfile;

namespace EPQ.Foodweb.Nodes
{
    public class NodeManager : MonoBehaviour
    {
        public AnimalProfile Profile;
        public TMP_Text NameText;
        public GameObject CreateConnectionButton;
        public Image ColorCode;
        public Image BackgroundImage;

        private void Update()
        {
            NameText.text = Profile.Name;
            CreateConnectionButton.SetActive(!PlaygroundNavigator.main.CreatingConnection);
        }

        public void MakeConnection()
        {
            PlaygroundNavigator.main.StartConnection(this);
        }
        public void EndConnection()
        {
            PlaygroundNavigator.main.EndConnection(this);
        }
        public void SelfDestruct()
        {
            PlaygroundNavigator.main.NukeID(Profile.ID);
            Destroy(transform.gameObject);
        }

        public void OnColorChange(object sender, ColorChangeArgs e)
        {
            ColorCode.color = e.NewColor;
        }
        public void OnToggleAnimal(object sender, ColorChangeArgs e)
        {
            BackgroundImage.color = e.NewColor;
        }
    }
}
