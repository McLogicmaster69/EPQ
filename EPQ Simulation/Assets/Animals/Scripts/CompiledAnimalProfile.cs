using EPQ.Data;
using EPQ.Foodweb;
using EPQ.Foodweb.Connections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    /// <summary>
    /// A class containing data required for an animal to be ran in the simulation
    /// </summary>
    public class CompiledAnimalProfile
    {
        public readonly int ID;
        public readonly List<int> CanEat;
        public readonly List<int> CanBeEatenBy;
        public Color Color;
        public readonly AnimalAttributes Attributes;

        /// <summary>
        /// Creates a compiled profile from an existing profile
        /// </summary>
        /// <param name="profile"></param>
        public CompiledAnimalProfile(AnimalProfile profile)
        {
            ID = profile.ID;
            CanEat = new List<int>();
            CanBeEatenBy = new List<int>();
            Attributes = new AnimalAttributes(profile.Attributes);
            Color = profile.ColorCode;

            List<LineConnection> Connections = PlaygroundNavigator.main.Connections;
            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].TwoWay)
                {
                    if (Connections[i].ID1 == ID)
                    {
                        CanEat.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID2));
                        CanBeEatenBy.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID1));
                    }
                    else if (Connections[i].ID2 == ID)
                    {
                        CanEat.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID1));
                        CanBeEatenBy.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID2));
                    }
                }
                else if(Connections[i].ID1 == ID)
                {
                    CanBeEatenBy.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID2));
                }
                else if(Connections[i].ID2 == ID)
                {
                    CanEat.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID1));
                }
            }
        }

        /// <summary>
        /// Creates a duplicate compiled file
        /// </summary>
        /// <param name="profile"></param>
        public CompiledAnimalProfile(CompiledAnimalDataFile profile)
        {
            ID = profile.ID;
            CanEat = new List<int>(profile.CanEat);
            CanBeEatenBy = new List<int>(profile.CanBeEatenBy);
            Attributes = profile.Attributes;
            Color = new Color(
                profile.r,
                profile.g,
                profile.b
                );
        }
    }
}