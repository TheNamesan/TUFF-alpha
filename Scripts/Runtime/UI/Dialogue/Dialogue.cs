using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    public enum TextboxType
    {
        Normal = 0,
        Fixed = 1,
    }
    public enum FixedTextboxPosition
    {
        Bottom = 0,
        Top = 1,
        Center = 2
    }
    [System.Serializable]
    public class Dialogue
    {
        [Tooltip("The textbox prefab to use. Normal shows the standard square textbox above the character speaking. Fixed shows a transparent box that stretches across all the screen.")]
        public TextboxType textboxType;
        [Tooltip("FromScene: Spawns the textbox above an object on the scene.\nFromPersistentInstance: Spawns textboxes above GameObjects that are marked as 'DontDestroyOnLoad'.")]
        public FieldOriginType originType;
        [Tooltip("Reference to the GameObject who's speaking. The textbox appears above the GameObject.")]
        public Transform origin;
        [Tooltip("The offset position to add to the textbox's world position.")]
        public Vector2 positionOffset;
        [Tooltip("The persistent GameObject who’s speaking.")]
        public PersistentType persistentOrigin = PersistentType.AvatarController;
        [Tooltip("The position of the Fixed Textbox.")]
        public FixedTextboxPosition fixedTextboxPosition;
        [Tooltip("The base Voicebank to use for sentences.")]
        public Voicebank baseVoicebank;
        public const float defaultBaseTextSpeed = 50f;
        [Tooltip("The base Text Speed to use to display in sentences. If 0 or less, will use the default instead. Default: 40.")]
        public float baseTextSpeed = 0f;
        [Tooltip("If true, will override the Textbox prefab with a custom one.")]
        public bool overrideTextbox;
        [Tooltip("The custom textbox prefab to use.")]
        public GameObject customTextbox;
        public DialogueSentence[] sentences = new DialogueSentence[0];
        [Tooltip("If true, will change the display color of Dialogue Sentences inside the textbox.")]
        public bool overrideTextColor = false;
        [Tooltip("The text color to override with.")]
        public Color customColor = Color.white;
        [Tooltip("Unity Event to call when this Dialogue ends.")]
        public UnityEvent onDialogueEnd;

        public static void InvokeTextbox(Dialogue dialogue, EventAction commandCallback = null)
        {
            GameManager.instance.StartCoroutine(InvokeTextboxCoroutine(dialogue, commandCallback));
        }
        public static IEnumerator InvokeTextboxCoroutine(Dialogue dialogue, EventAction commandCallback = null)
        {
            Transform parent = UIController.instance.textboxesParent;
            if (parent == null) yield break;
            while (UIController.instance.textbox.inUse || UIController.instance.dimTextbox.inUse)
            {
                yield return null;
            }
            GameObject textbox;
            if (dialogue.overrideTextbox && dialogue.customTextbox != null)
            {
                textbox = Object.Instantiate(dialogue.customTextbox);
            }
            else if (dialogue.textboxType == TextboxType.Fixed)
            {
                textbox = Object.Instantiate(TUFFSettings.fixedTextbox);
            }
            else
            {
                textbox = Object.Instantiate(TUFFSettings.defaultTextbox);
            }
            RectTransform textboxRect = textbox.GetComponent<RectTransform>();
            textboxRect.SetParent(parent, false);
            DialogueManager textboxManager = textbox.GetComponent<DialogueManager>();
            textboxManager.dialogue = dialogue;
            Dialogue textboxDialogue = textboxManager.dialogue;
            if (textboxDialogue.textboxType == TextboxType.Fixed)
            {
                textboxRect.position = Vector3.zero;
            }
            else
            {
                if (textboxDialogue.originType == FieldOriginType.FromScene)
                {
                    textboxDialogue.origin = dialogue.origin;
                }
                else
                {
                    textboxDialogue.origin = LISAUtility.GetPersistentOrigin(textboxDialogue.persistentOrigin);
                }
            }

            if (commandCallback != null)
            {
                textboxManager.actionCallback = commandCallback;
            }
            textboxManager.StartDialogue();
        }

        public static IEnumerator PreloadTextboxes() // Make this a pool
        {
            yield break;
        }
    }
}
