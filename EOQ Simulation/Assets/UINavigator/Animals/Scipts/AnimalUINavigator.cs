using EPQ.Animals;
using EPQ.Colors;
using EPQ.Data;
using EPQ.Foodweb;
using EPQ.Foodweb.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
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

        [Header("Nodes")]
        public GameObject Node;
        public Transform NodeParent;
        public float NodeSize = 10f;

        public List<AnimalProfile> Profiles = new List<AnimalProfile>();

        [Header("Other")]
        public GameObject LonelyText;
        public GameObject FriendsText;
        public GameObject ColorCodeUI;

        private const int ItemsPerPage = 5;

        private List<GameObject> profileObjects = new List<GameObject>();
        private List<int> visibleIndexes = new List<int>();
        private int CurrentPage = 0;
        private int CurrentIDCount = 0;
        private int CurrentColorIndex = -1;

        private void Awake()
        {
            main = this;
        }
        private void Start()
        {
            ColorCodeUI.SetActive(false);
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
            // Create profile and assign node
            AnimalProfile newProfile = AnimalProfile.BlankProfile(CurrentIDCount);
            newProfile.Node = CreateNode(newProfile).GetComponent<NodeManager>();

            // Give color code
            if (Profiles.Count != 0)
                newProfile.ColorCode = Profiles[Profiles.Count - 1].ColorCode;

            // Update UI
            Profiles.Add(newProfile);
            CurrentIDCount++;
            UpdateButtons();
            UpdateUI();
        }
        public void DeleteAnimal(int index)
        {
            // Handle Color Picker UI
            if (index == CurrentColorIndex)
                CloseColorCodeUI();
            else if (index < CurrentColorIndex)
            {
                ColorCodeUI.GetComponent<ColorCodeManager>().CurrentIndex--;
                CurrentColorIndex--;
            }

            // Identify current color picked item
            bool found = false;
            for (int i = 0; i < visibleIndexes.Count; i++)
            {
                if (found)
                {
                    AnimalProfileManager manager = profileObjects[i].GetComponent<AnimalProfileManager>();
                    manager.Index--;
                }
                else if(index == visibleIndexes[i])
                {
                    AnimalProfileManager manager = profileObjects[i].GetComponent<AnimalProfileManager>();
                    Profiles[manager.Index].OnColorChange -= manager.OnColorChange;
                    Destroy(profileObjects[i]);
                    found = true;
                    visibleIndexes.RemoveAt(i);
                    profileObjects.RemoveAt(i);
                    i--;
                }
            }

            // Destroy node and events
            Profiles[index].OnColorChange -= Profiles[index].Node.ChangeColorCode;
            Profiles[index].DestroyNode();
            Profiles.RemoveAt(index);

            // Go to previous page if empty
            if(CurrentPage * ItemsPerPage == Profiles.Count && Profiles.Count != 0)
            {
                PreviousPage();
            }
            else
            {
                UpdateButtons();
                UpdateUI();
            }
        }
        public void OpenColorCodeUI(int index)
        {
            ColorCodeUI.SetActive(true);

            ColorCodeManager colorCode = ColorCodeUI.GetComponent<ColorCodeManager>();
            colorCode.CurrentIndex = index;
            colorCode.SetupColor(Profiles[index].ColorCode);
            CurrentColorIndex = index;
        }
        public void CloseColorCodeUI()
        {
            ColorCodeUI.SetActive(false);
            CurrentColorIndex = -1;
        }

        private GameObject CreateNode(AnimalProfile profile)
        {
            // Setup display
            GameObject o = Instantiate(Node, NodeParent);
            o.GetComponent<NodeManager>().Profile = profile;
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(CurrentIDCount * NodeSize, 0);

            // Setup color
            NodeManager manager = o.GetComponent<NodeManager>();
            profile.OnColorChange += manager.ChangeColorCode;
            manager.ColorCode.color = profile.ColorCode;

            return o;
        }

        public void UpdateUI()
        {
            // Clear old UI list
            for (int i = 0; i < profileObjects.Count; i++)
            {
                AnimalProfileManager manager = profileObjects[i].GetComponent<AnimalProfileManager>();
                Profiles[manager.Index].OnColorChange -= manager.OnColorChange;
                Destroy(profileObjects[i]);
            }
            profileObjects = new List<GameObject>();
            visibleIndexes = new List<int>();

            // Display lonely text
            if(Profiles.Count == 0)
            {
                LonelyText.SetActive(true);
                FriendsText.SetActive(true);
                return;
            }
            LonelyText.SetActive(false);
            FriendsText.SetActive(false);

            // Append new UI elements
            for (int i = CurrentPage * ItemsPerPage; i < ItemsPerPage * (CurrentPage + 1); i++)
            {
                if (i >= Profiles.Count)
                    break;

                GameObject o = Instantiate(ProfileObject, ProfilesTab);

                // Setup manager
                AnimalProfileManager manager = o.GetComponent<AnimalProfileManager>();
                manager.Index = i;
                manager.SetupWithProfile(Profiles[i]);

                o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, StartingY - (i - CurrentPage * ItemsPerPage) * Height);
                profileObjects.Add(o);
                visibleIndexes.Add(i);
            }
        }
        public AnimalProfile GetProfile(int ID)
        {
            for (int i = 0; i < Profiles.Count; i++)
            {
                if(Profiles[i].ID == ID)
                {
                    return Profiles[i];
                }
            }
            return null;
        }
        public GameObject GetObject(int index)
        {
            return profileObjects[index];
        }

        public AnimalUIDataFile SaveAnimalUIDataFile()
        {
            AnimalUIDataFile file = new AnimalUIDataFile();
            file.CurrentIDCount = CurrentIDCount;
            return file;
        }
        public void SaveAnimalProfileDataFiles(out AnimalProfileDataFile[] profileFiles, out NodeDataFile[] nodeFiles)
        {
            List<AnimalProfileDataFile> ProfileFiles = new List<AnimalProfileDataFile>();
            List<NodeDataFile> NodeFiles = new List<NodeDataFile>();

            for (int i = 0; i < Profiles.Count; i++)
            {
                AnimalProfileDataFile APDF = new AnimalProfileDataFile();
                APDF.Name = Profiles[i].Name;
                APDF.ID = Profiles[i].ID;
                APDF.r = Profiles[i].ColorCode.r;
                APDF.g = Profiles[i].ColorCode.g;
                APDF.b = Profiles[i].ColorCode.b;
                ProfileFiles.Add(APDF);

                NodeDataFile NDF = new NodeDataFile();
                NDF.NodePosition = new float[3];
                NDF.NodePosition[0] = Profiles[i].Node.transform.position.x;
                NDF.NodePosition[1] = Profiles[i].Node.transform.position.y;
                NDF.NodePosition[2] = Profiles[i].Node.transform.position.z;
                NodeFiles.Add(NDF);
            }

            profileFiles = ProfileFiles.ToArray();
            nodeFiles = NodeFiles.ToArray();
        }

        //
        // LOADING
        //

        public void LoadFromFileV1(DataFile file)
        {
            // Delete all current profiles
            while (Profiles.Count > 0)
            {
                DeleteAnimal(0);
            }

            // Load in new data
            CurrentIDCount = file.AnimalUI.CurrentIDCount;

            for (int i = 0; i < file.AnimalProfiles.Length; i++)
            {
                AnimalProfile profile = new AnimalProfile();
                profile.ID = file.AnimalProfiles[i].ID;
                profile.Name = file.AnimalProfiles[i].Name;
                profile.ColorCode = new Color(1f, 1f, 1f);

                GameObject node = CreateNode(profile);
                node.transform.position = new Vector3(file.Nodes[i].NodePosition[0], file.Nodes[i].NodePosition[1], file.Nodes[i].NodePosition[2]);
                profile.Node = node.GetComponent<NodeManager>();
                Profiles.Add(profile);
            }

            // Update UI
            UpdateButtons();
            UpdateUI();
        }
        public void LoadFromFileV2(DataFile file)
        {
            // Delete all current profiles
            while (Profiles.Count > 0)
            {
                DeleteAnimal(0);
            }

            // Load in new data
            CurrentIDCount = file.AnimalUI.CurrentIDCount;

            for (int i = 0; i < file.AnimalProfiles.Length; i++)
            {
                AnimalProfile profile = new AnimalProfile();
                profile.ID = file.AnimalProfiles[i].ID;
                profile.Name = file.AnimalProfiles[i].Name;
                profile.ColorCode = new Color(
                    file.AnimalProfiles[i].r,
                    file.AnimalProfiles[i].g,
                    file.AnimalProfiles[i].b);

                GameObject node = CreateNode(profile);
                node.transform.position = new Vector3(file.Nodes[i].NodePosition[0], file.Nodes[i].NodePosition[1], file.Nodes[i].NodePosition[2]);
                profile.Node = node.GetComponent<NodeManager>();
                Profiles.Add(profile);
            }

            // Update UI
            UpdateButtons();
            UpdateUI();
        }
    }
}