using EPQ.Animals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.UI
{
    public class AnimalUINavigator : MonoBehaviour
    {
        public static AnimalUINavigator main;

        [Header("Buttons")]
        public GameObject PreviousButton;
        public GameObject NextButton;

        [Header("Tab Properties")]
        public int StartingY = -80;
        public int Height = 160;

        [Header("Animal Profiles")]
        public GameObject ProfileObject;
        public Transform ProfilesTab;

        public List<AnimalProfile> Profiles = new List<AnimalProfile>();

        private List<GameObject> profileObjects = new List<GameObject>();
        private int CurrentPage = 0;
        private const int ItemsPerPage = 5;

        private void Awake()
        {
            main = this;
        }
        private void Start()
        {
            UpdateButtons();
        }

        public void NextPage()
        {
            CurrentPage++;
            UpdateButtons();
            UpdateUI();
        }
        public void PreviousPage()
        {
            CurrentPage--;
            UpdateButtons();
            UpdateUI();
        }
        private void UpdateButtons()
        {
            PreviousButton.SetActive(CurrentPage != 0);
            NextButton.SetActive(CurrentPage + 1 < 
                ((Profiles.Count / ItemsPerPage) * ItemsPerPage == Profiles.Count 
                ? Profiles.Count / ItemsPerPage
                : Profiles.Count / ItemsPerPage + 1));
        }

        public void CreateAnimal()
        {
            Profiles.Add(AnimalProfile.BlankProfile(Profiles.Count));
            UpdateButtons();
            UpdateUI();
        }

        public void UpdateUI()
        {
            for (int i = 0; i < profileObjects.Count; i++)
            {
                Destroy(profileObjects[i]);
            }
            profileObjects = new List<GameObject>();
            for (int i = CurrentPage * ItemsPerPage; i < ItemsPerPage * (CurrentPage + 1); i++)
            {
                if (i >= Profiles.Count)
                    break;
                GameObject o = Instantiate(ProfileObject, ProfilesTab);
                o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, StartingY - (i - CurrentPage * ItemsPerPage) * Height);
                profileObjects.Add(o);
            }
        }
    }
}