using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class DatabaseElement : ScriptableObject
    {
        public int id { get => DatabaseLoader.FindID(this); }
        [TextArea(3, 8)] public string notes = "";
        public virtual string GetName()
        {
            return "";
        }
        public virtual string GetDescription()
        {
            return "";
        }
    }
}

