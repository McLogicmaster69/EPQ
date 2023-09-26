using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EPQ.Foodweb.Nodes
{
    /// <summary>
    /// Allows a node to detect it is being dragged by the user
    /// </summary>
    public class NodeDragChecker : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public NodeDrag node;

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            node.PrepareDrag();
        }
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            node.Drag();
        }
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            node.EndDrag();
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            node.Drag();
        }
    }
}