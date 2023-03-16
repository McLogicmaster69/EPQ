using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EPQ.UI
{
    public class UINavigator : MonoBehaviour
    {
        public Button[] Buttons;
        public GameObject[] Panels;
        public int CurrentSelected;
        public Color SelectedColor;
        public Color UnselectedColor;
        private void Start()
        {
            UpdateVisuals();
        }
        private void Update()
        {
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
    }
}