using EPQ.Animals;
using EPQ.Data;
using EPQ.Foodweb;
using EPQ.Worlds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Files
{
    public static class FileManager
    {
        #region Base
        public static readonly LoadSystem<DataFile>[] VersionLoads =
        {
            new LoadSystem<DataFile>("FlVrsn0.1", new System.Action<DataFile>[2] {  AnimalUINavigator.main.LoadFromFileV1, PlaygroundNavigator.main.LoadFromFileV1} ),
            new LoadSystem<DataFile>("FlVrsn0.2", new System.Action<DataFile>[2] {  AnimalUINavigator.main.LoadFromFileV2, PlaygroundNavigator.main.LoadFromFileV2} ),
            new LoadSystem<DataFile>("FlVrsn0.3", new System.Action<DataFile>[2] {  AnimalUINavigator.main.LoadFromFileV3, PlaygroundNavigator.main.LoadFromFileV3} ),
            new LoadSystem<DataFile>("FlVrsn0.4", new System.Action<DataFile>[2] {  AnimalUINavigator.main.LoadFromFileV4, PlaygroundNavigator.main.LoadFromFileV4} )
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
        #endregion
        #region Simulation
        public static readonly LoadSystem<SimulationDataFile>[] SimulationVersionLoads =
        {
            new LoadSystem<SimulationDataFile>("SimVer0.1", new System.Action<SimulationDataFile>[] { WorldManager.main.LoadDataV1 } ),
            new LoadSystem<SimulationDataFile>("SimVer0.2", new System.Action<SimulationDataFile>[] { WorldManager.main.LoadDataV2 } ),
        };

        public static string SimulationVersion { get { return SimulationVersionLoads[SimulationVersionLoads.Length - 1].Version; } }

        public static SimulationDataFile SaveSimulation()
        {
            WorldManager.main.SaveData(out CompiledAnimalDataFile[] animalData, out ControllerDataFile[] controllers, out WorldDataFile<int> ground, out WorldDataFile<int> animals, out int currentID);
            SimulationDataFile data = new SimulationDataFile
            {
                Version = SimulationVersion,
                Profiles = animalData,
                Controllers = controllers,
                Ground = ground,
                Animals = animals,
                CurrentID = currentID
            };
            return data;
        }
        public static void LoadFile(SimulationDataFile file)
        {
            for (int i = 0; i < SimulationVersionLoads.Length; i++)
            {
                if (file.Version == SimulationVersionLoads[i].Version)
                {
                    SimulationVersionLoads[i].CallMethods(file);
                    return;
                }
            }
        }
        #endregion
    }
}