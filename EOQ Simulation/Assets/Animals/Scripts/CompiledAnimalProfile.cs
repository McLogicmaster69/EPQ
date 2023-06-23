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

        public CompiledAnimalProfile(AnimalProfile profile)
        {
            ID = profile.ID;
            CanEat = new List<int>();

            List<LineConnection> Connections = PlaygroundNavigator.main.Connections;
            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].TwoWay)
                {
                    if (Connections[i].ID1 == ID)
                        CanEat.Add(Connections[i].ID2);
                    else if(Connections[i].ID2 == ID)
                        CanEat.Add(Connections[i].ID1);
                }
                else if(Connections[i].ID1 == ID)
                {
                    CanEat.Add(Connections[i].ID2);
                }
            }
        }
    }
}