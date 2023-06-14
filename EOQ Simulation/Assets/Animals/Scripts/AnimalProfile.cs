using EPQ.Foodweb;
using EPQ.Foodweb.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    public class AnimalProfile
    {
        public string Name;
        public int ID;
        public Color ColorCode;
        public NodeManager Node;

        public event EventHandler<ColorChangeArgs> OnColorChange;

        public class ColorChangeArgs : EventArgs
        {
            public Color NewColor;
        }

        public void DestroyNode()
        {
            Node.SelfDestruct();
        }
        public void SetRColor(float r)
        {
            ColorCode.r = r;
            OnColorChange?.Invoke(this, new ColorChangeArgs() { NewColor = ColorCode });
        }
        public void SetGColor(float g)
        {
            ColorCode.g = g;
            OnColorChange?.Invoke(this, new ColorChangeArgs() { NewColor = ColorCode });
        }
        public void SetBColor(float b)
        {
            ColorCode.b = b;
            OnColorChange?.Invoke(this, new ColorChangeArgs() { NewColor = ColorCode });
        }

        public static AnimalProfile BlankProfile(int id)
        {
            AnimalProfile profile = new AnimalProfile();
            profile.ID = id;
            profile.Name = GetRandomName();
            profile.ColorCode = new Color(1, 1, 1);
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
            return names[UnityEngine.Random.Range(0, names.Length)];
        }
    }
}