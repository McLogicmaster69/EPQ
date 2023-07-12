using EPQ.Data;
using EPQ.Files;
using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EPQ.View
{
    public class SimulationControls : MonoBehaviour
    {
        [Header("Main UI")]
        public GameObject ExpandedControls;
        public GameObject MinimisedControls;
        public GameObject NoSimulationPanel;

        [Header("Save UI")]
        public GameObject SaveUI;
        public TMP_InputField FileName;

        [Header("Clock buttons")]

        public ButtonInfo PlayButton;
        public ButtonInfo StepButton;
        public ButtonInfo MinimisedPlayButton;
        public ButtonInfo MinimisedStepButton;
        public TMP_InputField Input;
        public Color PlayingColor;
        public Color PausedColor;

        private bool minimised = false;

        private void Start()
        {
            InitObjects();
            UpdateButtons();
            EndEditInterval("");
        }

        public void ToggleControls()
        {
            minimised = !minimised;
            ExpandedControls.SetActive(!minimised);
            MinimisedControls.SetActive(minimised);
        }
        public void ToggleSaveUI()
        {
            Clock.main.IsTicking = false;
            SaveUI.SetActive(!SaveUI.activeInHierarchy);
        }
        public void SaveSimulation()
        {
            if (string.IsNullOrEmpty(FileName.text))
                return;

            SaveFile(FileName.text);
        }
        public void ToggleTicking()
        {
            Clock.main.IsTicking = !Clock.main.IsTicking;
            UpdateButtons();
        }
        public void StepTick()
        {
            Clock.main.DoTick();
        }
        public void EditInterval(string interval)
        {
            Clock.main.IsTicking = false;
            UpdateButtons();
            if (string.IsNullOrEmpty(interval))
                return;
            if (float.TryParse(interval, out float value))
            {
                if(value < 0.1f)
                    Clock.main.Interval = 0.1f;
                else if(value > 10f)
                    Clock.main.Interval = 10f;
                else
                    Clock.main.Interval = value;
            }
        }
        public void EndEditInterval(string interval)
        {
            if (interval != Clock.main.Interval.ToString())
                Input.text = Clock.main.Interval.ToString();
        }

        private void InitObjects()
        {
            NoSimulationPanel.SetActive(true);
            ExpandedControls.SetActive(true);
            MinimisedControls.SetActive(false);
            SaveUI.SetActive(false);
        }
        private void UpdateButtons()
        {
            PlayButton.Image.color = Clock.main.IsTicking ? PlayingColor : PausedColor;
            PlayButton.Text.text = Clock.main.IsTicking ? "PAUSE" : "PLAY";
            MinimisedPlayButton.Image.color = Clock.main.IsTicking ? PlayingColor : PausedColor;
            MinimisedPlayButton.Text.text = Clock.main.IsTicking ? "PAUSE" : "PLAY";
            StepButton.Button.interactable = !Clock.main.IsTicking;
            MinimisedStepButton.Button.interactable = !Clock.main.IsTicking;
        }
        private void SaveFile(string fileName)
        {
            SimulationDataFile data = FileManager.SaveSimulation();
            SaveSystem.Save<SimulationDataFile>(data, fileName, "sim");
        }
    }
}