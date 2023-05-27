using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EPQ.Data;
using EPQ.Files;

namespace EPQ.Menu
{
    public class MenuNavigator : MonoBehaviour
    {
        public GameObject MainMenu;
        public GameObject SaveMenu;
        public GameObject LoadMenu;
        public GameObject OptionsMenu;

        public TMP_InputField SaveName;
        public TMP_InputField LoadName;

        private void Start()
        {
            OpenMainMenu();
        }
        public void OpenMainMenu()
        {
            MainMenu.SetActive(true);
            SaveMenu.SetActive(false);
            LoadMenu.SetActive(false);
            OptionsMenu.SetActive(false);
        }
        public void OpenSaveMenu()
        {
            MainMenu.SetActive(false);
            SaveMenu.SetActive(true);
            LoadMenu.SetActive(false);
            OptionsMenu.SetActive(false);
        }
        public void OpenLoadMenu()
        {
            MainMenu.SetActive(false);
            SaveMenu.SetActive(false);
            LoadMenu.SetActive(true);
            OptionsMenu.SetActive(false);
        }
        public void OpenOptionsMenu()
        {
            MainMenu.SetActive(false);
            SaveMenu.SetActive(false);
            LoadMenu.SetActive(false);
            OptionsMenu.SetActive(true);
        }
        public void Quit()
        {
            Application.Quit();
        }

        public void SaveFile()
        {
            SaveSystem.Save<DataFile>(FileManager.SaveFile(), SaveName.text, "sv");
            OpenMainMenu();
        }
        public void LoadFile()
        {
            FileManager.LoadFile(SaveSystem.Load<DataFile>(LoadName.text, "sv"));
            OpenMainMenu();
        }
        public void ChangeFoodwebSpeed(string s)
        {
            SimulationOptions.MoveSpeed = System.Convert.ToSingle(s);
        }
    }
}