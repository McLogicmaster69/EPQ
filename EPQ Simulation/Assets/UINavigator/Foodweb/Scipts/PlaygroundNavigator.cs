using EPQ.Animals;
using EPQ.Data;
using EPQ.Foodweb.Connections;
using EPQ.Foodweb.Nodes;
using EPQ.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Foodweb
{
    /// <summary>
    /// Controls how the user interacts with the food web
    /// </summary>
    public class PlaygroundNavigator : MonoBehaviour
    {
        public static PlaygroundNavigator main;

        public Canvas canvas;
        public GameObject ChildObjects;
        [Header("Scale")]
        public float Scale = 1f;
        public float MinimumScale = 0.3f;
        public float MaximumScale = 1.6f;
        [Header("Connections")]
        public GameObject LineConnector;
        public Transform LineParent;
        public GameObject ConnectionUI;
        public GameObject ConnectionCancelButton;

        public bool CreatingConnection { get; private set; } = false;
        public List<LineConnection> Connections { get; private set; } = new List<LineConnection>();

        private NodeManager StartNode;
        private float oldScale = 1f;
        private int CurrentLineID = 0;

        public event MoveEvent OnPlaygroundMove;
        public event ScaleEvent OnPlaygroundScale;

        private void Awake()
        {
            main = this;
        }
        private void OnEnable()
        {
            ConnectionUI.SetActive(false);
        }
        private void Start()
        {
            ConnectionCancelButton.SetActive(false);
        }

        private void Update()
        {
            int verticalInp = 0;
            int horizontalInp = 0;

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                verticalInp--;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                verticalInp++;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                horizontalInp--;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                horizontalInp++;

            Vector3 movement = new Vector3(horizontalInp, verticalInp) * SimulationOptions.MoveSpeed * Time.deltaTime;
            ChildObjects.transform.position += movement;

            if(horizontalInp != 0 || verticalInp != 0)
                OnPlaygroundMove?.Invoke(this, new MoveEventArgs { Movement = movement });

            if (Input.GetKey(KeyCode.R))
            {
                Vector3 requiredMovement = ChildObjects.GetComponent<RectTransform>().anchoredPosition3D;
                ChildObjects.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                OnPlaygroundMove?.Invoke(this, new MoveEventArgs { Movement = -requiredMovement });
            }

            Scale += Input.mouseScrollDelta.y * 0.1f;
            Scale = Mathf.Clamp(Scale, MinimumScale, MaximumScale);
            ChildObjects.GetComponent<RectTransform>().localScale = new Vector3(Scale, Scale);

            if(oldScale != Scale)
            {
                Vector2 position = ChildObjects.GetComponent<RectTransform>().anchoredPosition;
                Vector2 zoomAdjustment = (position * (Scale - oldScale)) / oldScale;
                ChildObjects.GetComponent<RectTransform>().anchoredPosition += zoomAdjustment;

                OnPlaygroundScale?.Invoke(this, new ScaleEventArgs { Centre = -ChildObjects.GetComponent<RectTransform>().anchoredPosition, OldScale = oldScale, NewScale = Scale });
                oldScale = Scale;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CreatingConnection = false;
                ConnectionUI.SetActive(false);
                ConnectionCancelButton.SetActive(false);
            }
        }

        public void StartConnection(NodeManager startNode)
        {
            CreatingConnection = true;
            StartNode = startNode;
            ConnectionCancelButton.SetActive(true);
        }
        public void EndConnection(NodeManager endNode)
        {
            if (!CreatingConnection)
                return;
            if(endNode.Profile.ID != StartNode.Profile.ID && CreatingConnection)
            {
                for (int i = 0; i < Connections.Count; i++)
                {
                    if ((Connections[i].ID1 == StartNode.Profile.ID && Connections[i].ID2 == endNode.Profile.ID)
                        || (Connections[i].ID1 == endNode.Profile.ID && Connections[i].ID2 == StartNode.Profile.ID))
                        return;
                }

                GameObject o = Instantiate(LineConnector, LineParent);
                InitConnector(o, endNode);

                CreatingConnection = false;
                ConnectionCancelButton.SetActive(false);

                Connections.Add(new LineConnection() { ID1 = StartNode.Profile.ID, ID2 = endNode.Profile.ID, TwoWay = false, GameObject = o, LineID = CurrentLineID });
                CurrentLineID++;
            }
        }
        private void InitConnector(GameObject o, NodeManager endNode)
        {
            NodeLineConnector connector = o.GetComponent<NodeLineConnector>();

            connector.Target1 = StartNode.GetComponent<RectTransform>();
            connector.Target2 = endNode.GetComponent<RectTransform>();
            connector.Target1Node = StartNode;
            connector.Target2Node = endNode;
            connector.ID = CurrentLineID;
        }
        public void CancelConnection()
        {
            CreatingConnection = false;
            ConnectionCancelButton.SetActive(false);
        }
        public LineConnection GetConnection(int ID)
        {
            int index = ID < Connections.Count ? ID : Connections.Count - 1;
            while(index >= 0)
            {
                if (Connections[index].LineID == ID)
                    return Connections[index];
                index--;
            }
            return null;
        }

        public void NukeID(int ID)
        {
            int i = 0;
            while(i < Connections.Count)
            {
                if (Connections[i].ID1 == ID || Connections[i].ID2 == ID)
                {
                    Destroy(Connections[i].GameObject);
                    Connections.RemoveAt(i);
                }
                else
                    i++;
            }
        }

        public void RemoveConnection(int ID)
        {
            int index = ID < Connections.Count ? ID : Connections.Count - 1;
            while (index >= 0)
            {
                if (Connections[index].LineID == ID)
                {
                    Destroy(Connections[index].GameObject);
                    Connections.RemoveAt(index);
                    return;
                }
                index--;
            }
        }
        public void OpenLineUI(int ID)
        {
            LineConnection LC = null;
            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].LineID == ID)
                {
                    LC = Connections[i];
                    break;
                }
            }
            if(LC == null)
            {
                Debug.LogError("ya dun goofed: unkown ID for connection");
                return;
            }

            ConnectionUI.SetActive(true);
            ConnectionOptionsManager COM = ConnectionUI.GetComponent<ConnectionOptionsManager>();
            AnimalProfile Profile1 = null;
            AnimalProfile Profile2 = null;
            for (int i = 0; i < AnimalUINavigator.main.Profiles.Count; i++)
            {
                if(AnimalUINavigator.main.Profiles[i].ID == LC.ID1)
                {
                    Profile1 = AnimalUINavigator.main.Profiles[i];
                }
                if (AnimalUINavigator.main.Profiles[i].ID == LC.ID2)
                {
                    Profile2 = AnimalUINavigator.main.Profiles[i];
                }

                if (Profile1 != null && Profile2 != null)
                    break;
            }

            if(Profile1 == null || Profile2 == null)
            {
                Debug.LogError($"ya dun goofed: unkown ID for nodes" +
                    $"\nIDs: {LC.ID1}, {LC.ID2}");
            }

            COM.Target1 = Profile1;
            COM.Target2 = Profile2;
            COM.Profile = LC;
            COM.Connector = LC.GameObject.GetComponent<NodeLineConnector>();
        }

        public PlaygroundDataFile SavePlaygroundFile()
        {
            PlaygroundDataFile PDF = new PlaygroundDataFile();
            PDF.CurrentLineID = CurrentLineID;
            return PDF;
        }
        public LineConnectionDataFile[] SaveLineConnectionFiles()
        {
            List<LineConnectionDataFile> files = new List<LineConnectionDataFile>();

            for (int i = 0; i < Connections.Count; i++)
            {
                LineConnectionDataFile LCDF = new LineConnectionDataFile();
                LCDF.ID1 = Connections[i].ID1;
                LCDF.ID2 = Connections[i].ID2;
                LCDF.LineID = Connections[i].LineID;
                LCDF.TwoWay = Connections[i].TwoWay;
                files.Add(LCDF);
            }

            return files.ToArray();
        }

        //
        // LOADING
        //

        public void LoadFromFileV1(DataFile file)
        {
            CurrentLineID = file.Playground.CurrentLineID;
            Connections = new List<LineConnection>();


            for (int i = 0; i < file.LineConnections.Length; i++)
            {

                GameObject o = Instantiate(LineConnector, LineParent);

                LineConnection connection = new LineConnection()
                {
                    ID1 = file.LineConnections[i].ID1,
                    ID2 = file.LineConnections[i].ID2,
                    TwoWay = file.LineConnections[i].TwoWay,
                    GameObject = o,
                    LineID = file.LineConnections[i].LineID
                };
                Connections.Add(connection);

                NodeManager nd1 = null;
                NodeManager nd2 = null;

                for (int j = 0; j < AnimalUINavigator.main.Profiles.Count; j++)
                {
                    if (AnimalUINavigator.main.Profiles[j].ID == file.LineConnections[i].ID1)
                        nd1 = AnimalUINavigator.main.Profiles[j].Node;
                    if (AnimalUINavigator.main.Profiles[j].ID == file.LineConnections[i].ID2)
                        nd2 = AnimalUINavigator.main.Profiles[j].Node;

                    if (nd1 != null && nd2 != null)
                        break;
                }

                NodeLineConnector con = o.GetComponent<NodeLineConnector>();
                con.Target1 = nd1.GetComponent<RectTransform>();
                con.Target2 = nd2.GetComponent<RectTransform>();
                con.Target1Node = AnimalUINavigator.main.GetProfile(file.LineConnections[i].ID1).Node;
                con.Target2Node = AnimalUINavigator.main.GetProfile(file.LineConnections[i].ID2).Node;
                con.ID = file.LineConnections[i].LineID;
                CreatingConnection = false;
            }
        }
        public void LoadFromFileV2(DataFile file)
        {
            LoadFromFileV1(file);
        }
        public void LoadFromFileV3(DataFile file)
        {
            LoadFromFileV2(file);
        }
        public void LoadFromFileV4(DataFile file)
        {
            LoadFromFileV3(file);
        }
    }
}