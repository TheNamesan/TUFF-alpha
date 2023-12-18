using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    [System.Serializable]
    public class BranchActionContent
    {
        public bool condition = false;
        public ActionList actionList = new ActionList();

        public bool VerifyCondition()
        {
            return condition;
        }
    }
}