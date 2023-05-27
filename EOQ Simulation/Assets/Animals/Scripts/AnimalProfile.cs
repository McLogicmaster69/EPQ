using EPQ.Foodweb;
using EPQ.Foodweb.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    public class AnimalProfile
    {
        public string Name;
        public int ID;
        public NodeManager Node;

        public void DestroyNode()
        {
            Node.SelfDestruct();
        }

        public static AnimalProfile BlankProfile(int id)
        {
            AnimalProfile profile = new AnimalProfile();
            profile.ID = id;
            profile.Name = GetRandomName();
            return profile;
        }

        private static string GetRandomName()
        {
            string[] names = { 
                "Bob",
                "Bulbasaur",
                "Charmander",
                "Squirtle",
                "Caterpie",
                "Weedle",
                "Pidgey",
                "Rattata",
                "Spearow",
                "Ekans",
                "Pikachu",
                "Sandshrew",
                "Nidoran",
                "Clefairy",
                "Vulpix",
                "Jigglypuff",
                "Zubat",
                "Oddish",
                "Paras",
                "Venonat",
                "Diglett",
                "Meowth",
                "Psyduck",
                "Mankey",
                "Growlithe",
                "Poliwag",
                "Abra",
                "Machop",
                "Bellsprout",
                "Tentacool",
                "Geodude"
            };
            return names[Random.Range(0, names.Length)];
        }
    }
}