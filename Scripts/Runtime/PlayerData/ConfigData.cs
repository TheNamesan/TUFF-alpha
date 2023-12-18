using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class ConfigData
    {
        public int refreshRate = 60;
        public bool fullscreen = true;
        public int resolutionWidth = 0;
        public int resolutionHeight = 0;
        [Range(0f, 1f)] public float globalMusicVolume = 1f;
        [Range(0f, 1f)] public float globalSFXVolume = 1f;
        [Range(0f, 1f)] public float globalAmbienceVolume = 1f;
        public int textSpeed = 0;
        public bool lost = false;

        public void SaveData()
        {
            SaveDataConverter.SaveConfigData(this);
        }

        public void LoadData()
        {
            ConfigData load = SaveDataConverter.LoadConfigData();
            if (load == null) return;
            refreshRate = load.refreshRate;
            fullscreen = load.fullscreen;
            resolutionWidth = load.resolutionWidth;
            resolutionHeight = load.resolutionHeight;
            globalMusicVolume = load.globalMusicVolume;
            globalSFXVolume = load.globalSFXVolume;
            globalAmbienceVolume = load.globalAmbienceVolume;
            textSpeed = load.textSpeed;
            lost = load.lost;
        }
        public static ConfigData GetDefaultData() // Set Best Settings for current device on first load here
        {
            ConfigData configData = new ConfigData();
            
            configData.fullscreen = true;
            var res = GameManager.instance.highestResolution;
            configData.resolutionWidth = res.width;
            configData.resolutionHeight = res.height;
            configData.refreshRate = res.refreshRate;
            configData.globalMusicVolume = 1f;
            configData.globalSFXVolume = 1f;
            configData.globalAmbienceVolume = 1f;
            configData.textSpeed = 0;
            configData.lost = false;
            return configData;
        }
    }
}
