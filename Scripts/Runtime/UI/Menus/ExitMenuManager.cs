using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class ExitMenuManager : MonoBehaviour
    {
        [Header("References")]
        public UIMenu uiMenu;

        public void OpenMenu()
        {
            uiMenu?.OpenMenu();
        }
        public void ExitButton()
        {
            Debug.Log("Exiting Game");
            Application.Quit();
        }
    }
}
