using EPQ.Data;
using EPQ.Foodweb;
using EPQ.Foodweb.Connections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    public class CompiledAnimalProfile
    {
        public readonly int ID;
        public readonly List<int> CanEat;
        public Color Color;
        public readonly AnimalAttributes Attributes;

        public CompiledAnimalProfile(AnimalProfile profile)
        {
            ID = profile.ID;
            CanEat = new List<int>();
            Attributes = new AnimalAttributes(profile.Attributes);
            Color = profile.ColorCode;

            List<LineConnection> Connections = PlaygroundNavigator.main.Connections;
            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].TwoWay)
                {
                    if (Connections[i].ID1 == ID)
                        CanEat.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID2));
                    else if(Connections[i].ID2 == ID)
                        CanEat.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID1));
                }
                else if(Connections[i].ID1 == ID)
                {
                    CanEat.Add(AnimalUINavigator.main.GetIndex(Connections[i].ID2));
                }
            }
        }
        public CompiledAnimalProfile(CompiledAnimalDataFile profile)
        {
            ID = profile.ID;
            CanEat = new List<int>(profile.CanEat);
            Attributes = profile.Attributes;
            Color = new Color(
                profile.r,
                profile.g,
                profile.b
                );
        }
    }
}