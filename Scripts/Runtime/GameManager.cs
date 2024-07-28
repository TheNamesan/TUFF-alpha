using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using System.Linq;
using DG.Tweening;

namespace TUFF
{
    public class GameManager : MonoBehaviour
    {
        public PlayerData playerData = new PlayerData();
        public ConfigData configData = new ConfigData();
        [Header("References")]
        public GameObject gameCanvasPrefab;
        public InputManager inputManager;
        public DatabaseLoader databaseLoader;
        public SceneLoaderManager sceneLoaderManager;
        public CommonEventManager commonEventManager;
        public bool stopPlaytime = false;
        [Tooltip("Index of the last opened or loaded save file ingame.")]
        public int lastLoadedFile = -1;

        private bool m_holdingSkip = false;


        private static bool m_disablePlayerInput = false;
        public static bool disablePlayerInput
        {
            get { return m_disablePlayerInput; }
        }
        private static bool m_disableUIInput = true;
        public static bool disableUIInput
        {
            get { return m_disableUIInput; }
        }
        public bool DPI = false;
        public bool DUI = false;
        public bool DPAM = false;
        public bool DUIAM = false;

        public Resolution[] supportedResolutions;
        public Resolution highestResolution { get => m_highestResolution; }
        private Resolution m_highestResolution;

        public int[] frameRates = new int[] { 60, 75, 120, 144, 240, 300 };

        public UnityEvent<bool> onPlayerInputToggle = new();

        public static bool gameOver = false;

        #region Singleton
        public static GameManager instance
        {
            get { 
                if (m_instance == null)
                {
                    m_instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
                    m_instance?.InitializeGame();
                }
                return m_instance;
            }
            set { m_instance = value; }
        }
        private static GameManager m_instance;
        
        private void Awake()
        {
            if (instance != null)
            {
                if (instance != this) Destroy(gameObject);
            }
            //else
            //{
            //    instance = this;
            //    InitializeGame();
            //}
        }

        private void InitializeGame()
        {
            DontDestroyOnLoad(gameObject);
            Instantiate(gameCanvasPrefab);
            LocalizationSettings.Instance.GetInitializationOperation();
            LoadInitialData();
            //LocalizationSettings.Instance.GetInitializationOperation(); //Start Localization
            StartCoroutine(Dialogue.PreloadTextboxes());
            DOTween.Init();
        }
        #endregion

        private void LoadInitialData()
        {
            //Load Config
            GetUserScreenResolutions();
            configData.LoadData();
            SetGameFullscreen(configData.fullscreen);
            SetGameResolution(configData.resolutionWidth, configData.resolutionHeight, configData.refreshRate);
            UpdateGlobalVolume();

            databaseLoader.InitializeDatabase();
            //Load Dummy Save Data
            playerData.StartPlayerData();
            playerData.AddToParty(0); //dummy test
        }
        public void SavePlayerData(int fileIndex)
        {
            lastLoadedFile = fileIndex;
            configData.lastOpenedFile = fileIndex;
            playerData.SaveData(fileIndex);
            configData.SaveData();
        }
        public bool LoadSaveData(int fileIndex)
        {
            bool loaded = false;
            playerData.StartPlayerData();
            playerData.AddToParty(0); //test
            var load = PlayerData.LoadData(fileIndex);
            if (load != null)
            {
                loaded = true;
                load.CheckListSizes();
                load.CheckUnitRefs();

                playerData = load;
                lastLoadedFile = fileIndex;
                configData.lastOpenedFile = fileIndex;
                configData.SaveData();
            }
            if (loaded) Debug.Log($"Loaded File {fileIndex}");
            return loaded;
        }
        private void Start()
        {
            
            /*inv = GetComponent<Inventory>();
            for (int i = 0; i < Party.Count; i++)
            {
                Party[i] = Instantiate(Party[i]);
            }
            UI = UIController.instance;
            UI.RefreshParty(Party);
            UI.RefreshDosh(inv.dosh);*/
        }

        public void Update()
        {
            playerData.Update();
            DPI = m_disablePlayerInput;
            DUI = m_disableUIInput;
            DPAM = !inputManager.playerActionMap.enabled;
            DUIAM = !inputManager.uiActionMap.enabled;
            FastForwardEvents();
        }

        private void FastForwardEvents()
        {
            if (TUFF.CommonEventManager.interactableEventPlaying && !BattleManager.instance.InBattle)
            {
                if (TUFF.UIController.instance.skipButtonHold)
                {
                    Time.timeScale = 10f;
                    m_holdingSkip = true;
                }
                else
                {
                    Time.timeScale = 1f;
                    m_holdingSkip = false;
                }
            }
            else if (m_holdingSkip)
            {
                Time.timeScale = 1f;
                m_holdingSkip = false;
            }
        }

        public void DisablePlayerInput(bool input)
        {
            bool prev = m_disablePlayerInput;
            m_disablePlayerInput = input;
            Debug.Log($"Disable Player Input: {input}");
            if (input != prev)
            {
                if (input) inputManager.playerInputHandler?.StopInput();
                else inputManager.playerInputHandler?.ResumeInput();
            }
            onPlayerInputToggle.Invoke(!input);
        }

        public void DisableActionMaps(bool input)
        {
            if (input)
            {
                inputManager.uiActionMap.Disable();
                inputManager.playerActionMap.Disable();
            }
            else
            {
                inputManager.playerActionMap.Enable();
                inputManager.uiActionMap.Enable();
            }
        }

        public void DisableUIInput(bool input)
        {
            m_disableUIInput = input;
            //if (input) inputManager.uiActionMap.Disable();
            //else inputManager.uiActionMap.Enable();
        }

        public void DisablePlayerAndUIInput(bool input)
        {
            DisablePlayerInput(input);
            DisableUIInput(input);
        }

        public bool DealDamageToParty(int input)
        {
            /*for (int i = 0; i < Party.Count; i++)
            {
                DealDamage(Party[i], input);
            }*/
            return false;
        }

        public void GameOver()
        {
            if (!gameOver)
            {
                StartCoroutine(SetGameOver());
            }
        }

        public void ChangeTimeScale(float value)
        {
            Time.timeScale = value;
        }
        public void SetGameFullscreen(bool fullscreen)
        {
            Screen.fullScreenMode = fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Debug.Log("FullScreenMode: " + fullscreen);
            if (configData.fullscreen) SetGameResolution(0, 0);
        }
        public void SetGameResolution(int width, int height, int refreshRate = -1)
        {
            if (refreshRate < 0) refreshRate = configData.refreshRate;
            Vector2Int targetRes = new Vector2Int(width, height);
            if(configData.fullscreen)
            {
                targetRes = new Vector2Int(m_highestResolution.width, m_highestResolution.height);
            }
            Screen.SetResolution(targetRes.x, targetRes.y,
                configData.fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed,
                refreshRate);
            Application.targetFrameRate = refreshRate;
            Debug.Log("Resolution: " + targetRes);
            Debug.Log("Refresh Rate: " + refreshRate);
        }
        public void UpdateGlobalVolume()
        {
            if (AudioManager.instance != null) // CHANGE THIS TO GET REFERENCE!!!!
                AudioManager.instance.UpdateVolumeFromConfig();
        }

        private void GetUserScreenResolutions()
        {
            supportedResolutions = Screen.resolutions;
            Resolution[] screenResolutions = Screen.resolutions;
            //string test = "I am bot";
            //test.GetHashCode();
            Debug.Log($"All resolutions: {screenResolutions.Length}");
            Resolution[] uniqueResolutions = Enumerable.Distinct(screenResolutions, new ResolutionEqualityComparer()).ToArray();
            Debug.Log($"Unique resolutions: {uniqueResolutions.Length}");
            supportedResolutions = uniqueResolutions;

            //for (int i=0; i < supportedResolutions.Length; i++)
            //{
            //    Debug.Log(supportedResolutions[i].width + "" +
            //        ", " + supportedResolutions[i].height +
            //        ", " + supportedResolutions[i].refreshRate);
            //}

            m_highestResolution = supportedResolutions[supportedResolutions.Length - 1];
        }
        public void ExpandSupportedResolutions(Resolution add)
        {
            System.Array.Resize(ref supportedResolutions, supportedResolutions.Length + 1);
            supportedResolutions[supportedResolutions.Length - 1] = add;
            if (add.height > m_highestResolution.height)
                m_highestResolution = add;
        }
        public int GetMaxUserRefreshRate()
        {
            if (supportedResolutions.Length <= 0) GetUserScreenResolutions();
            int maxFrameRate = 0;
            for (int i = 0; i < supportedResolutions.Length; i++)
            {
                var res = supportedResolutions[i];
                if (res.refreshRate > maxFrameRate)
                    maxFrameRate = res.refreshRate;
            }
            return maxFrameRate;
        }
        public void StartNewGame()
        {
            playerData.StartPlayerData();
            playerData.AssignNewGameData();
            playerData.AddToParty(0); //test
        }
        IEnumerator SetGameOver()
        {
            gameOver = true;
            Debug.Log("Game Over");
            ChangeTimeScale(0f);
            CommonEventManager.StopEvents();
            UIController.instance.SetMenu(null);
            AudioManager.instance.StopAmbience();
            AudioManager.instance.PlayGameOverMusic();
            UIController.instance.UIFadeScreen.FadeOut(1f);
            yield return new WaitForSecondsRealtime(1f);
            ChangeTimeScale(1);
            SceneLoaderManager.instance.LoadSceneWithFadeIn("GameOver", 0.5f, disableActionMap: true, enablePlayerInputAction: false) ;
        }

        public void TestBattle(Battle battle)
        {
            BattleManager.instance.TestBattle(battle);
            AudioManager.instance.PlaySFX(TUFFSettings.equipSFX);
        }
    }
}
