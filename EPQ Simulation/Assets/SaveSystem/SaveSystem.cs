using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace  EPQ.Data
{
    public static class SaveSystem
    {
        public static void Save<T>(T file, string fileName, string fileExtension)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + $"/{fileName}.{fileExtension}";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, file);
            stream.Close();
            Debug.Log($"Saved to {path}");
        }
        public static T Load<T>(string fileName, string fileExtension)
        {
            string path = Application.persistentDataPath + $"/{fileName}.{fileExtension}";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                T file = (T)formatter.Deserialize(stream);
                stream.Close();
                Debug.Log($"Loaded from {path}");
                return file;
            }
            else
            {
                Debug.LogError("File not found");
                return default(T);
            }
        }
        public static bool Exists(string fileName, string fileExtension)
        {
            string path = Application.persistentDataPath + $"/{fileName}.{fileExtension}";
            return File.Exists(path);
        }
    }
}