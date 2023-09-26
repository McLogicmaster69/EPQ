using EPQ.Colors;
using EPQ.Foodweb;
using EPQ.Foodweb.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    /// <summary>
    /// Manages all behaviour of a certain animal type. Controls how they act in the simulation and how they are presented in the user interface.
    /// </summary>
    public class AnimalProfile
    {
        /// <summary>
        /// The name of the animal
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the animal
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The ratio of how many of this animal is initially spawned
        /// </summary>
        public int BatchSize { get; set; } = 4;

        /// <summary>
        /// The visual color of the animal
        /// </summary>
        public Color ColorCode { get { return _colorCode; } set { _colorCode = ColorCode; } }

        /// <summary>
        /// The manager of the node containing this animal
        /// </summary>
        public NodeManager Node { get; set; }

        /// <summary>
        /// The values of how the animal should behave
        /// </summary>
        public AnimalAttributes Attributes { get; set; }

        /// <summary>
        /// If this profile represents an animal or plant
        /// </summary>
        public bool IsAnimal { get { return Attributes.IsAnimal; } }

        /// <summary>
        /// Called when the color of the animal is changed
        /// </summary>
        public event ColorEvent OnColorChange;

        /// <summary>
        /// Called when the profile is switched between animal and plant
        /// </summary>
        public event ColorEvent OnAnimalChange;

        private Color _colorCode;

        /// <summary>
        /// Creates a blank profile with default attributes
        /// </summary>
        public AnimalProfile()
        {
            Attributes = new AnimalAttributes();
        }

        /// <summary>
        /// Destroys the node containing this animal
        /// </summary>
        public void DestroyNode()
        {
            Node.SelfDestruct();
        }

        /// <summary>
        /// Changes the red value of the color
        /// </summary>
        /// <param name="r">Value between 0 and 1</param>
        public void SetRColor(float r)
        {
            _colorCode.r = r;
            OnColorChange?.Invoke(this, new ColorChangeArgs() { NewColor = _colorCode });
        }

        /// <summary>
        /// Changes the green value of the color
        /// </summary>
        /// <param name="g">Value between 0 and 1</param>
        public void SetGColor(float g)
        {
            _colorCode.g = g;
            OnColorChange?.Invoke(this, new ColorChangeArgs() { NewColor = _colorCode });
        }

        /// <summary>
        /// Changes the blue value of the color
        /// </summary>
        /// <param name="b">Value between 0 and 1</param>
        public void SetBColor(float b)
        {
            _colorCode.b = b;
            OnColorChange?.Invoke(this, new ColorChangeArgs() { NewColor = _colorCode });
        }

        /// <summary>
        /// Toggles the profile between animal and plant
        /// </summary>
        /// <param name="newColor">The color repesenting the state of the profile</param>
        public void ToggleAnimal(Color newColor)
        {
            Attributes.IsAnimal = !Attributes.IsAnimal;
            OnAnimalChange?.Invoke(this, new ColorChangeArgs() { NewColor = newColor });
        }

        /// <summary>
        /// Creates a blank profile
        /// </summary>
        /// <param name="id">The ID of the profile</param>
        /// <returns></returns>
        public static AnimalProfile BlankProfile(int id)
        {
            AnimalProfile profile = new AnimalProfile();
            profile.ID = id;
            profile.Name = GetRandomName();
            profile._colorCode = new Color(1, 1, 1);
            return profile;
        }

        /// <summary>
        /// Returns a random name from a bag of already created names
        /// </summary>
        /// <returns></returns>
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