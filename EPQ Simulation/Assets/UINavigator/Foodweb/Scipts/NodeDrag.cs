using EPQ.Foodweb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EPQ.Foodweb.Nodes
{
    /// <summary>
    /// Gives the functionality to a node to be able to be dragged by the user
    /// </summary>
    public class NodeDrag : MonoBehaviour, IPointerClickHandler
    {
        private Canvas canvas;
        private RectTransform rectTransform;
        private NodeManager manager;
        private Vector3 offset;

        private bool canDrag = false;
        private bool prepareDrag = false;

        private void Awake()
        {
            manager = GetComponent<NodeManager>();
        }
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = PlaygroundNavigator.main.canvas;
            PlaygroundNavigator.main.OnPlaygroundMove += PlaygroundMovement;
            PlaygroundNavigator.main.OnPlaygroundScale += PlaygroundScale;
        }
        private void Update()
        {
            if (!Input.GetMouseButton(0))
                canDrag = false;

            if (canDrag)
            {
                Vector3 newPos = Input.mousePosition;
                newPos.z = transform.position.z;
                transform.position = newPos + offset * PlaygroundNavigator.main.Scale;
            }
        }

        private void PlaygroundMovement(object sender, MoveEventArgs e)
        {
            if (prepareDrag)
            {
                canDrag = true;
            }
        }
        private void PlaygroundScale(object sender, ScaleEventArgs e)
        {
            if (prepareDrag)
            {
                canDrag = true;
            }
        }

        public void PrepareDrag()
        {
            prepareDrag = true;
            offset = (transform.position - Input.mousePosition) / PlaygroundNavigator.main.Scale;
            offset.z = 0;
        }
        public void Drag()
        {
            canDrag = true;
        }
        public void EndDrag()
        {
            prepareDrag = false;
            canDrag = false;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            canDrag = false;
            prepareDrag = false;
            manager.EndConnection();
        }
    }
}