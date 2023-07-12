using EPQ.Foodweb.Nodes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EPQ.Foodweb.Connections
{
    public class NodeLineConnector : MonoBehaviour, IPointerClickHandler
    {
        public RectTransform Target1;
        public RectTransform Target2;
        public NodeManager Target1Node;
        public NodeManager Target2Node;
        public LineConnection LineConnection;
        public Material Gradient;
        public float Thickness = 5f;
        public int ID;

        private Image image;
        private RectTransform rectTransform;

        void Start()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            LineConnection = PlaygroundNavigator.main.GetConnection(ID);
        }

        void Update()
        {
            if (Target1.gameObject.activeSelf && Target2.gameObject.activeSelf)
            {
                AdjustPosition();
                if(LineConnection == null)
                    LineConnection = PlaygroundNavigator.main.GetConnection(ID);
                ChangeGradient();
            }
        }
        private void AdjustPosition()
        {
            if (Target1.localPosition == Target2.localPosition)
            {
                image.enabled = false;
                return;
            }
            image.enabled = true;
            rectTransform.localPosition = (Target1.localPosition + Target2.localPosition) / 2;
            Vector3 dif = Target2.localPosition - Target1.localPosition;
            rectTransform.sizeDelta = new Vector3(dif.magnitude, Thickness);
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
        private void ChangeGradient()
        {
            if (LineConnection.TwoWay)
            {
                image.material = null;
            }
            else if (LineConnection.ID1 == Target1Node.Profile.ID)
            {
                image.material = Gradient;
                if (Target1.localPosition.x < Target2.localPosition.x)
                    rectTransform.localScale = new Vector3(-1, -1, 1);
                else
                    rectTransform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                image.material = Gradient;
                if (Target1.localPosition.x < Target2.localPosition.x)
                    rectTransform.localScale = new Vector3(1, 1, 1);
                else
                    rectTransform.localScale = new Vector3(-1, -1, 1);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                PlaygroundNavigator.main.OpenLineUI(ID);
            else if(eventData.button == PointerEventData.InputButton.Right)
            {
                if(PlaygroundNavigator.main.ConnectionUI.activeInHierarchy)
                    if (PlaygroundNavigator.main.ConnectionUI.GetComponent<ConnectionOptionsManager>().Profile.LineID == ID)
                        PlaygroundNavigator.main.ConnectionUI.SetActive(false);
                PlaygroundNavigator.main.RemoveConnection(ID);
            }

        }
    }
}