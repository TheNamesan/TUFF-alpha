using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class FileSelectMenu : MonoBehaviour
    {
        public UIMenu uiMenu;
        public Transform buttonsParent;
        public SaveFileHUD saveFileHUDPrefab;
        public List<SaveFileHUD> elements = new();
        private void Awake()
        {
            if (uiMenu == null) uiMenu = GetComponent<UIMenu>();
            //if (uiMenu == null) uiMenu = GetComponent<UIMenu>();
            //if (uiMenu != null)
            //{
            //    if (uiMenu.UIElements == null) uiMenu.SetupUIElements();
            //    for (int i = 0; i < uiMenu.UIElements.Length; i++)
            //    {
            //        if (uiMenu.UIElements[i].Length > 0)
            //        {
            //            int index = i;
            //            var element = uiMenu.UIElements[i][0];
            //            element.useCustomSelectSFX = true;
            //            element.customSelectSFX = TUFFSettings.loadSFX;
            //            element.onSelect.AddListener(() => { SelectFile(index); });
            //        }
            //    }
            //}
            InitializeSaveFileHUDs();
        }
        private void InitializeSaveFileHUDs()
        {
            VerifyMenuArrays(0);
            int index = 0;
            foreach (Transform child in buttonsParent) // Add existing GameObjects to list
            {
                if (child.TryGetComponent(out SaveFileHUD existing))
                {
                    existing.Initialize(index);
                    elements.Add(existing);
                    index++;
                }
                
            }
            if (saveFileHUDPrefab)
            {
                for (; index < 16; index++) // Tmp 16 number
                {
                    var create = Instantiate(saveFileHUDPrefab, buttonsParent);
                    create.Initialize(index);
                    elements.Add(create);
                }
            }
            for (int i = 0; i < elements.Count; i++)
            {
                AddToMenu(i, elements[i].fileSelectButton);
            }
            if (uiMenu.UIElements == null) uiMenu.SetupUIElements();

            //if (uiMenu != null)
            //{
            //    if (uiMenu.UIElements == null) uiMenu.SetupUIElements();
            //    for (int i = 0; i < uiMenu.UIElements.Length; i++)
            //    {
            //        if (uiMenu.UIElements[i].Length > 0)
            //        {
            //            int index = i;
            //            var element = uiMenu.UIElements[i][0];
            //            element.useCustomSelectSFX = true;
            //            element.customSelectSFX = TUFFSettings.loadSFX;
            //            element.onSelect.AddListener(() => { SelectFile(index); });
            //        }
            //    }
            //}
        }
        private void VerifyMenuArrays(int rows)
        {
            if (uiMenu == null) return;
            if (uiMenu.UIElementContainers == null)
            {
                uiMenu.UIElementContainers = new UIElementContainer[rows];
            }
            if (rows >= uiMenu.UIElementContainers.Length) // If row is out of row count
            {
                var newArray = new UIElementContainer[rows + 1];
                System.Array.Copy(uiMenu.UIElementContainers, newArray, uiMenu.UIElementContainers.Length);
                uiMenu.UIElementContainers = newArray;
            }
            if (uiMenu.UIElementContainers[0] == null) // Check Index 0
                uiMenu.UIElementContainers[0] = new UIElementContainer();
        }
        protected void AddToMenu(int index, UIButton element)
        {
            if (uiMenu == null) return;
            if (index >= uiMenu.UIElementContainers.Length)
            {
                VerifyMenuArrays(index);
            }
            if (uiMenu.UIElementContainers[index] == null)
                uiMenu.UIElementContainers[index] = new UIElementContainer();
            uiMenu.UIElementContainers[index].UIElements.Add(element);

            element.useCustomSelectSFX = true;
            element.customSelectSFX = TUFFSettings.loadSFX;
            element.onSelect.AddListener(() => SelectFile(index));
            //element.onSelect.AddListener(() => { SelectFile(index); });
        }
        public void SelectFile(int file)
        {
            bool loaded = GameManager.instance.LoadSaveData(file);
            if (loaded) { Debug.Log($"Loaded File {file}"); StartCoroutine(Continue()); }
            else Debug.Log($"No file found at {file}");
        }
        private IEnumerator Continue()
        {
            UIController.instance.SetMenu(null);
            UIController.instance.fadeScreen.TriggerFadeOut(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(2f);
            PlayerData.instance.GetSceneData(out string sceneName, out Vector3 playerPosition, out FaceDirections playerFacing);
            SceneLoaderManager.instance.LoadSceneWithFadeIn(sceneName, 0.5f, playerPosition, playerFacing, true, true); //Replace with save data scene name and start position
        }
        private void OnEnable()
        {
            
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
