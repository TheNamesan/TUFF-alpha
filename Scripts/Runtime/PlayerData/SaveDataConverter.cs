using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TUFF
{
    public static class SaveDataConverter
    {
        public static readonly string SAVE_PATH = Application.dataPath + @"/PlayerData/"; 
        public const string SAVE_FILE_NAME = @"save";
        public const string SAVE_FILE_EXT = @".sav";
        public const string CONFIG_FILE = @"config.cfg";
        
        public static void SavePlayerData(PlayerData playerData, int file)
        {
            CheckSavePathExists();
            string path = SAVE_PATH + SAVE_FILE_NAME + file + SAVE_FILE_EXT;
            SavePlayerDataToPath(playerData, path);
            Debug.Log($"Saved File #{file}");
        }
        public static void SavePlayerDataToPath(PlayerData playerData, string path)
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(path, json);
        }
        public static void SaveConfigData(ConfigData config)
        {
            CheckSavePathExists();
            string json = JsonUtility.ToJson(config, true);
            File.WriteAllText(SAVE_PATH + CONFIG_FILE, json);
            Debug.Log("Saved Config");
        }
        public static PlayerData LoadPlayerData(int file)
        {
            CheckSavePathExists();
            string path = SAVE_PATH + SAVE_FILE_NAME + file + SAVE_FILE_EXT;
            if (CheckSaveExistsAtIndex(file))
            {
                PlayerData load = LoadPlayerDataFromPath(path);
                Debug.Log($"Loaded File #{file}");
                return load;
            }
            return null;
        }
        public static PlayerData LoadPlayerDataFromPath(string path)
        {
            string fileString = File.ReadAllText(path);
            PlayerData load = JsonUtility.FromJson<PlayerData>(fileString);
            return load;
        }
        public static bool CheckSaveExistsAtIndex(int index)
        {
            CheckSavePathExists();
            string path = SAVE_PATH + SAVE_FILE_NAME + index + SAVE_FILE_EXT;
            return File.Exists(path);
        }

        public static ConfigData LoadConfigData()
        {
            CheckSavePathExists();
            if (File.Exists(SAVE_PATH + CONFIG_FILE))
            {
                string fileString = File.ReadAllText(SAVE_PATH + CONFIG_FILE);
                ConfigData load = JsonUtility.FromJson<ConfigData>(fileString);
                Debug.Log("Loaded Config");
                return load;
            }
            ConfigData newConfig = ConfigData.GetDefaultData();
            SaveConfigData(newConfig);
            return newConfig;
        }

        private static void CheckSavePathExists()
        {
            if (!Directory.Exists(SAVE_PATH)) Directory.CreateDirectory(SAVE_PATH);
        }
    }
}
