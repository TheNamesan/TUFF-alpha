using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ReturnToTitleMenuManager : MonoBehaviour
    {
        public string titleScreenSceneName = "TitleScreen";

        [Header("References")]
        public UIMenu uiMenu;

        public void OpenMenu()
        {
            uiMenu?.OpenMenu();
        }
        public void ToTitleButton()
        {
            StartCoroutine(ToTitle());
        }

        IEnumerator ToTitle()
        {
            GameManager.instance.DisableUIInput(true);
            UIController.instance.FadeOutUI(1f);
            AudioManager.instance.FadeOutVolume(1f);
            yield return new WaitForSeconds(1.5f);
            UIController.instance.CloseAllMenus();
            SceneLoaderManager.instance.LoadSceneWithFadeIn(titleScreenSceneName, 1f);
            GameManager.instance.DisableUIInput(false);
        }
    }
}
