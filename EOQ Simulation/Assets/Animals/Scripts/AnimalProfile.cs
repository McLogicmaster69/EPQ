using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ.Animals
{
    public class AnimalProfile
    {
        public string Name;

        public static AnimalProfile BlankProfile(int id)
        {
            AnimalProfile profile = new AnimalProfile();
            profile.Name = $"Animal {id}";
            return profile;
        }
    }
}