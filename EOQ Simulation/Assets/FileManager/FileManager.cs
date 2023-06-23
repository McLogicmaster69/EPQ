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
        public static readonly LoadSystem[] VersionLoads =
        {
            new LoadSystem("FlVrsn0.1", new System.Action<DataFile>[2] {  AnimalUINavigator.main.LoadFromFileV1, PlaygroundNavigator.main.LoadFromFileV1} ),
            new LoadSystem("FlVrsn0.2", new System.Action<DataFile>[2] {  AnimalUINavigator.main.LoadFromFileV2, PlaygroundNavigator.main.LoadFromFileV2})
        };

        public static string Version { get { return VersionLoads[VersionLoads.Length - 1].Version; } } 

        public static DataFile SaveFile()
        {
            DataFile file = new DataFile();
            file.AnimalUI = AnimalUINavigator.main.SaveAnimalUIDataFile();
            AnimalUINavigator.main.SaveAnimalProfileDataFiles(out AnimalProfileDataFile[] profiles, out NodeDataFile[] nodes);
            file.AnimalProfiles = profiles;
            file.Nodes = nodes;
            file.Playground = PlaygroundNavigator.main.SavePlaygroundFile();
            file.LineConnections = PlaygroundNavigator.main.SaveLineConnectionFiles();

            file.Version = Version;

            return file;
        }

        public static void LoadFile(DataFile file)
        {
            if (string.IsNullOrEmpty(file.Version))
            {
                OldLoad(file);
                return;
            }

            for (int i = 0; i < VersionLoads.Length; i++)
            {
                if(file.Version == VersionLoads[i].Version)
                {
                    VersionLoads[i].CallMethods(file);
                    return;
                }
            }
        }

        private static void OldLoad(DataFile file)
        {
            AnimalUINavigator.main.LoadFromFileV1(file);
            PlaygroundNavigator.main.LoadFromFileV1(file);
        }
    }
}