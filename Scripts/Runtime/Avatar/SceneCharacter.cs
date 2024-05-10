using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class SceneCharacter : MonoBehaviour
    {
        public OverworldCharacterController controller;
        public MoveRouteHandler moveRouteHandler;
        public CharacterAnimationHandler charAnim;
        public GameObject lightSource;
        public bool LightSourceEnabled { get {
                if (!lightSource) return false;
                return lightSource.activeInHierarchy;
            } }
        public virtual void RestoreState()
        {
            charAnim?.RestoreState(controller);
        }
        public virtual void EnableLightSource(bool enable)
        {
            if (lightSource) lightSource.SetActive(enable);
        }
        public virtual void ChangeAnimationPack(AnimationPack animationPack)
        {
            charAnim?.LoadAnimationPack(animationPack);
        }
        public virtual void PlayAnimation(AnimationClip animationClip)
        {
            charAnim?.PlayAnimation(animationClip);
        }
        public virtual void ChangeSprite(Sprite sprite)
        {
            charAnim?.ChangeSprite(sprite);
        }
    }
}

