using EPQ.Animals;
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

        private List<GameObject> profileObjects = new List<GameObject>();
        private int CurrentPage = 0;
        private const int ItemsPerPage = 5;
        private int CurrentIDCount = 0;

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
            AnimalProfile newProfile = AnimalProfile.BlankProfile(CurrentIDCount);
            newProfile.Node = CreateNode(newProfile).GetComponent<NodeManager>();
            Profiles.Add(newProfile);
            CurrentIDCount++;
            UpdateButtons();
            UpdateUI();
        }
        public void DeleteAnimal(int index)
        {
            Profiles[index].DestroyNode();
            Profiles.RemoveAt(index);
            UpdateButtons();
            UpdateUI();
        }

        private GameObject CreateNode(AnimalProfile profile)
        {
            GameObject o = Instantiate(Node, NodeParent);
            o.GetComponent<NodeManager>().Profile = profile;
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(CurrentIDCount * NodeSize, 0);
            return o;
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

                // Setup manager
                AnimalProfileManager manager = o.GetComponent<AnimalProfileManager>();
                manager.Index = i;
                manager.SetupWithProfile(Profiles[i]);

                o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, StartingY - (i - CurrentPage * ItemsPerPage) * Height);
                profileObjects.Add(o);
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


        public void LoadFromFile(DataFile file)
        {
            while(Profiles.Count > 0)
            {
                DeleteAnimal(0);
            }

            CurrentIDCount = file.AnimalUI.CurrentIDCount;

            for (int i = 0; i < file.AnimalProfiles.Length; i++)
            {
                AnimalProfile profile = new AnimalProfile();
                profile.ID = file.AnimalProfiles[i].ID;
                profile.Name = file.AnimalProfiles[i].Name;
                GameObject node = CreateNode(profile);
                node.transform.position = new Vector3(file.Nodes[i].NodePosition[0], file.Nodes[i].NodePosition[1], file.Nodes[i].NodePosition[2]);
                profile.Node = node.GetComponent<NodeManager>();
                Profiles.Add(profile);
            }

            UpdateButtons();
            UpdateUI();
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
    }
}