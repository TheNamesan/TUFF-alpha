using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class FollowerInstance : SceneCharacter
    {
        public static FollowerInstance instance;
        public static FollowerInstance player { get { return instance; } } // Tmp

        public static List<FollowerController> followerList = new();

        private void Awake()
        {
            if (instance != null)
            { Destroy(gameObject); }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public static void AddFollower(FollowerController newFollower)
        {
            if (!newFollower) return;
            if (!followerList.Contains(newFollower))
                followerList.Add(newFollower);
        }
        public static void RemoveFollower(FollowerController follower)
        {
            followerList.Remove(follower);
        }
        public static void ResetFollowers()
        {
            foreach (FollowerController follower in followerList)
            {
                if (!follower) continue;
                follower.ResetValues();
            }
        }
        //public static void RunFollowersLogic()
        //{
        //    foreach (FollowerController follower in followerList)
        //    {
        //        if (!follower) continue;
        //        follower.Logic();
        //    }
        //}
    }
}
