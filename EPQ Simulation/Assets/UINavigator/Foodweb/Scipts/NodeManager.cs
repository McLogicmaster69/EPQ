using EPQ.Animals;
using EPQ.Colors;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EPQ.Animals.AnimalProfile;

namespace EPQ.Foodweb.Nodes
{
    /// <summary>
    /// Manages the visual side of a node
    /// </summary>
    public class NodeManager : MonoBehaviour
    {
        public AnimalProfile Profile;

        [SerializeField] private TMP_Text NameText;
        [SerializeField] private GameObject CreateConnectionButton;
        public Image ColorCode;
        [SerializeField] private Image BackgroundImage;

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
