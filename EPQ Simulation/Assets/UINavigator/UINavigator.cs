using EPQ.Animals;
using EPQ.Foodweb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EPQ.UI
{
    public class UINavigator : MonoBehaviour
    {
        public static UINavigator main;

        public Button[] Buttons;
        public GameObject[] Panels;
        public int CurrentSelected;
        public Color SelectedColor;
        public Color UnselectedColor;

        private int frame = 0;

        private void Awake()
        {
            main = this;
        }
        private void Start()
        {
            UpdateVisuals();
        }
        private void Update()
        {
            if(frame == 0)
            {
                for (int i = 0; i < Panels.Length; i++)
                {
                    Panels[i].SetActive(true);
                }
                frame++;
            }
            else if(frame == 1)
            {
                UpdateVisuals();
                frame++;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    CurrentSelected--;
                else
                    CurrentSelected++;

                if (CurrentSelected >= Panels.Length)
                    CurrentSelected = 0;
                if (CurrentSelected < 0)
                    CurrentSelected = Panels.Length - 1;

                UpdateVisuals();
            }
        }
        private void UpdateVisuals()
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].image.color = i == CurrentSelected ? SelectedColor : UnselectedColor;
                Panels[i].SetActive(i == CurrentSelected);
            }
        }
        public void ChangeMenu(int index)
        {
            CurrentSelected = index;
            UpdateVisuals();
        }
        public void FindNode(int index)
        {
            GameObject nodeObject = AnimalUINavigator.main.Profiles[index].Node.gameObject;
            ChangeMenu(2);
            PlaygroundNavigator.main.ChildObjects.GetComponent<RectTransform>().anchoredPosition = -nodeObject.GetComponent<RectTransform>().anchoredPosition * PlaygroundNavigator.main.Scale;
        }
    }
}