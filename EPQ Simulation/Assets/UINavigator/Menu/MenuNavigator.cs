using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EPQ.Data;
using EPQ.Files;

namespace EPQ.Menu
{
    /// <summary>
    /// Manages the main menu displayed to the user
    /// </summary>
    public class MenuNavigator : MonoBehaviour
    {
        [SerializeField] private GameObject MainMenu;
        [SerializeField] private GameObject SaveMenu;
        [SerializeField] private GameObject LoadMenu;
        [SerializeField] private GameObject OptionsMenu;

        [SerializeField] private TMP_InputField SaveName;
        [SerializeField] private TMP_InputField LoadName;
        [SerializeField] private TMP_Text LoadStatusText;

        private const string LOAD_FAILED_MESSAGE = "FAILED TO OPEN FILE ";

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
            LoadStatusText.text = string.Empty;
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
            DataFile data = SaveSystem.Load<DataFile>(LoadName.text, "sv");

            if(data == null)
            {
                LoadStatusText.text = FormatLoadFailMessage(LoadName.text);
                return;
            }

            FileManager.LoadFile(data);
            OpenMainMenu();
        }
        public void ChangeFoodwebSpeed(string s)
        {
            SimulationOptions.MoveSpeed = System.Convert.ToSingle(s);
        }

        private string FormatLoadFailMessage(string filename) => LOAD_FAILED_MESSAGE + filename;
    }
}