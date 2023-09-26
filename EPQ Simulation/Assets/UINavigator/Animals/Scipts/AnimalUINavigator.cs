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
        [SerializeField] private GameObject PreviousButton;
        [SerializeField] private GameObject NextButton;

        [Header("Tab Properties")]
        [SerializeField] private int StartingY = -80;
        [SerializeField] private int Height = 160;

        [Header("Animal Profiles")]
        [SerializeField] private GameObject ProfileObject;
        [SerializeField] private Transform ProfilesTab;

        [Header("Nodes")]
        [SerializeField] private GameObject Node;
        [SerializeField] private Transform NodeParent;
        [SerializeField] private float NodeSize = 10f;

        [Header("Other")]
        [SerializeField] private GameObject LonelyText;
        [SerializeField] private GameObject FriendsText;
        [SerializeField] private GameObject ColorCodeUI;

        /// <summary>
        /// All the existing profiles
        /// </summary>
        public List<AnimalProfile> Profiles { get; private set; } = new List<AnimalProfile>();

        private const int ItemsPerPage = 5;

        private List<GameObject> _profileObjects = new List<GameObject>();
        private List<int> _visibleIndexes = new List<int>();
        private int _currentPage = 0;
        private int _currentIDCount = 0;
        private int _currentColorIndex = -1;

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
            _currentPage++;
            UpdateButtons();
            UpdateUI();
        }
        public void PreviousPage()
        {
            _currentPage--;
            UpdateButtons();
            UpdateUI();
        }
        private void UpdateButtons()
        {
            PreviousButton.SetActive(_currentPage != 0);
            NextButton.SetActive(_currentPage + 1 < 
                ((Profiles.Count / ItemsPerPage) * ItemsPerPage == Profiles.Count 
                ? Profiles.Count / ItemsPerPage
                : Profiles.Count / ItemsPerPage + 1));
        }

        public void CreateAnimal()
        {
            // Create profile and assign node
            AnimalProfile newProfile = AnimalProfile.BlankProfile(_currentIDCount);
            newProfile.Node = CreateNode(newProfile).GetComponent<NodeManager>();

            // Give color code
            if (Profiles.Count != 0)
                newProfile.ColorCode = Profiles[Profiles.Count - 1].ColorCode;

            // Update UI
            Profiles.Add(newProfile);
            _currentIDCount++;
            UpdateButtons();
            UpdateUI();
        }

        public void DeleteAnimal(int index)
        {
            HandleColorUIOnDelete(index);
            OrginaizeUIUponDelete(index);
            DestroyNodesAndEvents(index);
            PreviousPageIfEmpty();
        }
        private void HandleColorUIOnDelete(int index)
        {
            if (index == _currentColorIndex)
                CloseColorCodeUI();
            else if (index < _currentColorIndex)
            {
                ColorCodeUI.GetComponent<ColorCodeManager>().CurrentIndex--;
                _currentColorIndex--;
            }
        }
        private void OrginaizeUIUponDelete(int index)
        {
            bool found = false;
            for (int i = 0; i < _visibleIndexes.Count; i++)
            {
                if (found)
                {
                    AnimalProfileManager manager = _profileObjects[i].GetComponent<AnimalProfileManager>();
                    manager.Index--;
                }
                else if (index == _visibleIndexes[i])
                {
                    AnimalProfileManager manager = _profileObjects[i].GetComponent<AnimalProfileManager>();
                    Profiles[manager.Index].OnColorChange -= manager.OnColorChange;
                    Profiles[manager.Index].OnAnimalChange -= manager.OnAnimalToggle;
                    Destroy(_profileObjects[i]);
                    found = true;
                    _visibleIndexes.RemoveAt(i);
                    _profileObjects.RemoveAt(i);
                    i--;
                }
            }
        }
        private void PreviousPageIfEmpty()
        {
            if (_currentPage * ItemsPerPage == Profiles.Count && Profiles.Count != 0)
            {
                PreviousPage();
            }
            else
            {
                UpdateButtons();
                UpdateUI();
            }
        }

        private void DestroyNodesAndEvents(int index)
        {
            Profiles[index].OnColorChange -= Profiles[index].Node.OnColorChange;
            Profiles[index].OnAnimalChange -= Profiles[index].Node.OnToggleAnimal;
            Profiles[index].DestroyNode();
            Profiles.RemoveAt(index);
        }

        public void OpenColorCodeUI(int index)
        {
            ColorCodeUI.SetActive(true);

            ColorCodeManager colorCode = ColorCodeUI.GetComponent<ColorCodeManager>();
            colorCode.CurrentIndex = index;
            colorCode.SetupColor(Profiles[index].ColorCode);
            _currentColorIndex = index;
        }
        public void CloseColorCodeUI()
        {
            ColorCodeUI.SetActive(false);
            _currentColorIndex = -1;
        }
        public void OpenBehaviourUI(int index)
        {
            BehaviourUINavigator.main.OpenUI(Profiles[index]);
        }

        private GameObject CreateNode(AnimalProfile profile)
        {
            // Setup display
            GameObject o = Instantiate(Node, NodeParent);
            o.GetComponent<NodeManager>().Profile = profile;
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(_currentIDCount * NodeSize, 0);

            // Setup color
            NodeManager manager = o.GetComponent<NodeManager>();
            manager.OnColorChange(this, new ColorChangeArgs() { NewColor = profile.ColorCode });
            profile.OnColorChange += manager.OnColorChange;
            profile.OnAnimalChange += manager.OnToggleAnimal;
            manager.ColorCode.color = profile.ColorCode;

            return o;
        }

        public void UpdateUI()
        {
            // Clear old UI list
            for (int i = 0; i < _profileObjects.Count; i++)
            {
                AnimalProfileManager manager = _profileObjects[i].GetComponent<AnimalProfileManager>();
                Profiles[manager.Index].OnColorChange -= manager.OnColorChange;
                Profiles[manager.Index].OnAnimalChange -= manager.OnAnimalToggle;
                Destroy(_profileObjects[i]);
            }
            _profileObjects = new List<GameObject>();
            _visibleIndexes = new List<int>();

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
            for (int i = _currentPage * ItemsPerPage; i < ItemsPerPage * (_currentPage + 1); i++)
            {
                if (i >= Profiles.Count)
                    break;

                GameObject o = Instantiate(ProfileObject, ProfilesTab);

                // Setup manager
                AnimalProfileManager manager = o.GetComponent<AnimalProfileManager>();
                manager.Index = i;
                manager.SetupWithProfile(Profiles[i]);

                o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, StartingY - (i - _currentPage * ItemsPerPage) * Height);
                _profileObjects.Add(o);
                _visibleIndexes.Add(i);
            }
        }
        public AnimalProfile GetProfile(int id)
        {
            int index = id < Profiles.Count ? id : Profiles.Count - 1;
            while(index >= 0)
            {
                if (Profiles[index].ID == id)
                    return Profiles[index];
                index--;
            }
            return null;
        }
        public int GetIndex(int id)
        {
            int index = id < Profiles.Count ? id : Profiles.Count - 1;
            while (index >= 0)
            {
                if (Profiles[index].ID == id)
                    return index;
                index--;
            }
            return -1;
        }
        public GameObject GetObject(int index)
        {
            return _profileObjects[index];
        }

        public AnimalUIDataFile SaveAnimalUIDataFile()
        {
            AnimalUIDataFile file = new AnimalUIDataFile();
            file.CurrentIDCount = _currentIDCount;
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
                APDF.BatchSize = Profiles[i].BatchSize;
                APDF.r = Profiles[i].ColorCode.r;
                APDF.g = Profiles[i].ColorCode.g;
                APDF.b = Profiles[i].ColorCode.b;
                APDF.Attributes = new AnimalAttributes(Profiles[i].Attributes);
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
            _currentIDCount = file.AnimalUI.CurrentIDCount;

            for (int i = 0; i < file.AnimalProfiles.Length; i++)
            {
                AnimalProfile profile = new AnimalProfile();
                profile.ID = file.AnimalProfiles[i].ID;
                profile.Name = file.AnimalProfiles[i].Name;
                profile.BatchSize = 4;
                profile.ColorCode = new Color(1f, 1f, 1f);
                profile.Attributes = new AnimalAttributes();

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
            _currentIDCount = file.AnimalUI.CurrentIDCount;

            for (int i = 0; i < file.AnimalProfiles.Length; i++)
            {
                AnimalProfile profile = new AnimalProfile();
                profile.ID = file.AnimalProfiles[i].ID;
                profile.Name = file.AnimalProfiles[i].Name;
                profile.BatchSize = 4;
                profile.ColorCode = new Color(
                    file.AnimalProfiles[i].r,
                    file.AnimalProfiles[i].g,
                    file.AnimalProfiles[i].b);
                profile.Attributes = new AnimalAttributes();

                GameObject node = CreateNode(profile);
                node.transform.position = new Vector3(file.Nodes[i].NodePosition[0], file.Nodes[i].NodePosition[1], file.Nodes[i].NodePosition[2]);
                profile.Node = node.GetComponent<NodeManager>();
                Profiles.Add(profile);
            }

            // Update UI
            UpdateButtons();
            UpdateUI();
        }
        public void LoadFromFileV3(DataFile file)
        {
            // Delete all current profiles
            while (Profiles.Count > 0)
            {
                DeleteAnimal(0);
            }

            // Load in new data
            _currentIDCount = file.AnimalUI.CurrentIDCount;

            for (int i = 0; i < file.AnimalProfiles.Length; i++)
            {
                AnimalProfile profile = new AnimalProfile();
                profile.ID = file.AnimalProfiles[i].ID;
                profile.Name = file.AnimalProfiles[i].Name;
                profile.BatchSize = 4;
                profile.ColorCode = new Color(
                    file.AnimalProfiles[i].r,
                    file.AnimalProfiles[i].g,
                    file.AnimalProfiles[i].b);
                profile.Attributes = new AnimalAttributes(file.AnimalProfiles[i].Attributes);

                GameObject node = CreateNode(profile);
                node.transform.position = new Vector3(file.Nodes[i].NodePosition[0], file.Nodes[i].NodePosition[1], file.Nodes[i].NodePosition[2]);
                profile.Node = node.GetComponent<NodeManager>();
                Profiles.Add(profile);
            }

            // Update UI
            UpdateButtons();
            UpdateUI();
        }
        public void LoadFromFileV4(DataFile file)
        {
            // Delete all current profiles
            while (Profiles.Count > 0)
            {
                DeleteAnimal(0);
            }

            // Load in new data
            _currentIDCount = file.AnimalUI.CurrentIDCount;

            for (int i = 0; i < file.AnimalProfiles.Length; i++)
            {
                AnimalProfile profile = new AnimalProfile();
                profile.ID = file.AnimalProfiles[i].ID;
                profile.Name = file.AnimalProfiles[i].Name;
                profile.BatchSize = file.AnimalProfiles[i].BatchSize;
                profile.ColorCode = new Color(
                    file.AnimalProfiles[i].r,
                    file.AnimalProfiles[i].g,
                    file.AnimalProfiles[i].b);
                profile.Attributes = new AnimalAttributes(file.AnimalProfiles[i].Attributes);

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