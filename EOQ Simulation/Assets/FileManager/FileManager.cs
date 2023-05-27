using EPQ.Animals;
using EPQ.Data;
using EPQ.Foodweb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Files
{
    public static class FileManager
    {
        public static DataFile SaveFile()
        {
            DataFile file = new DataFile();
            file.AnimalUI = AnimalUINavigator.main.SaveAnimalUIDataFile();
            AnimalUINavigator.main.SaveAnimalProfileDataFiles(out AnimalProfileDataFile[] profiles, out NodeDataFile[] nodes);
            file.AnimalProfiles = profiles;
            file.Nodes = nodes;
            file.Playground = PlaygroundNavigator.main.SavePlaygroundFile();
            file.LineConnections = PlaygroundNavigator.main.SaveLineConnectionFiles();

            return file;
        }

        public static void LoadFile(DataFile file)
        {
            AnimalUINavigator.main.LoadFromFile(file);
            PlaygroundNavigator.main.LoadFromFile(file);
        }
    }
}