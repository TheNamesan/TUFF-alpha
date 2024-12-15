using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    public class FontLoader : MonoBehaviour
    {
        public TMP_Text text;

        private void Awake()
        {
            if (!text) text = GetComponent<TMP_Text>();
        }
    }
}

