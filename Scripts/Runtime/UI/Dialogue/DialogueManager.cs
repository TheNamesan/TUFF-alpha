using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;

namespace TUFF
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Text")]
        public Dialogue dialogue;
        public int currentSentence = 0;
        public bool textboxInitiated = false;
        public bool inUse = false;

        [Header("References")]
        public TextMeshProUGUI text;
        public TMP_Typewriter typewriter;
        public BoxTransitionHandler transition;
        public AdjustToPreferredTextSize textSizeAdjuster;
        public AdjustToOtherRect adjustToOtherRect;
        public RectTransform continuePrompt;

        public CanvasScaler parentCanvasScaler;
        private LayoutGroup rootLayout;
        private RectTransform parentRT;
        private UIMenu uiMenu;
        [System.NonSerialized]
        public EventAction actionCallback = null;
        private static bool m_continuedDialogue = false; // Change this to be a brief period of buffer time before a dialogue is called? (For things like SFXs)


        [Header("Input")]
        public float skipTimeBuffer = 0.03f;

        List<string> sentences = new();
        public Camera cam { get => UIController.instance.cameraCanvas.worldCamera; }

        [SerializeField] public float skipTime = 0;
        public bool autoContinue = false;
        private float m_autoEndDelay = 0.5f;
        private IEnumerator autoContinueCoroutine;

        Vector3 bottomLeftCorner;
        Vector3 upperLeftCorner;
        Vector3 upperRightCorner;
        Vector3 bottomRightCorner;

        Vector3 canvasBottomLeftCorner;
        Vector3 canvasUpperLeftCorner;
        Vector3 canvasUpperRightCorner;
        Vector3 canvasBottomRightCorner;

        public RectTransform rect { get => transform as RectTransform; }
        private RectTransform parentRect { get => transform.parent as RectTransform; }

        public static List<DialogueManager> openBoxes = new(); // tmp // Change this to be a brief period of buffer time before a dialogue is called? (For things like SFXs)
        private static readonly Vector2 DEFAULT_OFFSET = new Vector2(0, 1f);

        private Color baseColor = Color.white; //tmp, change to get color from TUFFSettings;

        private void Awake()
        {
            if (transition == null) transition = GetComponent<BoxTransitionHandler>();
        }
        private void OnEnable()
        {
            DisplayContinuePrompt(false);
        }
        void InitialValues()
        {
            inUse = true;
            StartCoroutine(InitialValuesCoroutine());
        }
        private IEnumerator InitialValuesCoroutine()
        {
            if (uiMenu == null)
                uiMenu = GetComponent<UIMenu>();
            if (rootLayout == null)
                rootLayout = GetComponent<LayoutGroup>();
            parentCanvasScaler = LISAUtility.GetCanvasScalerRoot(transform);

            BattleManager.instance.hud.ShowWindowsDynamic(false);
            UIController.instance.SetMenu(uiMenu);
            LISAUtility.SetPivot(rect, new Vector2(rect.pivot.x, 0f));
            parentRT = transform.parent as RectTransform;
            currentSentence = 0;
            text.enabled = true;
            skipTime = 0;
            sentences = new List<string>();
            foreach (var sentence in dialogue.sentences)
            {
                string text = "";
                if (sentence.sentenceTextType == SentenceTextType.Localized)
                {
                    text = LISAUtility.GetLocalizedDialogueText(sentence.key);
                }
                else if (sentence.sentenceTextType == SentenceTextType.Simple)
                    text = sentence.text;
                sentences.Add(text);
            }
            if (!m_continuedDialogue) transition?.Appear();
            SetPosition();
            SetVoicebank();
            openBoxes.Add(this);
            yield return new WaitForEndOfFrame(); // Small buffer to avoid textboxes from skipping
                                                  // the first sentence if displayed after a menu is closed.
            textboxInitiated = true;
            yield break;
        }
        private void SetPosition()
        {
            Vector3 originPos = Vector3.zero;
            if (dialogue.origin != null) originPos = dialogue.origin.position;
            if (dialogue.textboxType == TextboxType.Normal)
            {
                LISAUtility.SetPivot(rect, new Vector2(rect.pivot.x, 0f));
                float orthoSize = Mathf.Max(cam.orthographicSize, 0.00001f);
                transform.localScale = Vector3.one * (6 / orthoSize);
                Vector2 canvasResolution = parentCanvasScaler.referenceResolution;
                Vector2 screenPosition = cam.WorldToScreenPoint(originPos + (Vector3)dialogue.positionOffset + (Vector3)DEFAULT_OFFSET);
                screenPosition = new Vector2(screenPosition.x * canvasResolution.x / Screen.width, screenPosition.y * canvasResolution.y / Screen.height);
                Vector2 canvasPosition = screenPosition - canvasResolution * 0.5f;
                rect.localPosition = canvasPosition;
                
                if (dialogue.origin == null) rect.localPosition = dialogue.positionOffset * 100f;
                AdjustOutOfBounds();
                LISAUtility.SetPivot(rect, new Vector2(rect.pivot.x, 0.5f));
            }
            else if (dialogue.textboxType == TextboxType.Fixed)
            {
                if (dialogue.fixedTextboxPosition == FixedTextboxPosition.Bottom)
                {
                    rect.pivot = new Vector2(0.5f, 0);
                    rect.anchorMin = new Vector2(0.5f, 0);
                    rect.anchorMax = new Vector2(0.5f, 0);
                    rect.localPosition = new Vector2(0, -parentRT.rect.height * 0.5f);
                }
                else if (dialogue.fixedTextboxPosition == FixedTextboxPosition.Top)
                {
                    rect.pivot = new Vector2(0.5f, 1);
                    rect.anchorMin = new Vector2(0.5f, 1);
                    rect.anchorMax = new Vector2(0.5f, 1);
                    rect.localPosition = new Vector2(0, parentRT.rect.height * 0.5f);
                }
                else if (dialogue.fixedTextboxPosition == FixedTextboxPosition.Center)
                {
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.localPosition = new Vector2(0, 0f);
                }
                //ForceRebuild();
            }
        }
        public void SetTextColor()
        {
            if (dialogue == null) return;
            Color textColor = baseColor;
            if (dialogue.overrideTextColor) textColor = dialogue.customColor;
            if (currentSentence < dialogue.sentences.Length)
            {
                var targetSentence = dialogue.sentences[currentSentence];
                if (targetSentence.overrideTextColor)
                    textColor = targetSentence.customColor;
            }
            text.color = textColor;
        }

        private void AdjustOutOfBounds()
        {
            Vector2 bounds = parentRT.rect.size * 0.5f;
            Vector2 halfSizeDelta = rect.sizeDelta * 0.5f * rect.localScale;
            transform.localPosition = new Vector3(
                Mathf.Clamp(transform.localPosition.x, -bounds.x + halfSizeDelta.x, bounds.x - halfSizeDelta.x),
                Mathf.Clamp(transform.localPosition.y, -bounds.y + halfSizeDelta.y, bounds.y - halfSizeDelta.y),
                transform.localPosition.z);
        }

        void SetVoicebank()
        {
            if (dialogue.baseVoicebank != null)
            {
                typewriter.clip = dialogue.baseVoicebank.clip;
                typewriter.pitch = dialogue.baseVoicebank.pitch;
                typewriter.pitchVariation = dialogue.baseVoicebank.pitchVariation;
            }
            if (dialogue.sentences.Length <= 0) return;
            if (dialogue.sentences[currentSentence].voicebank != null)
            {
                typewriter.clip = dialogue.sentences[currentSentence].voicebank.clip;
                typewriter.pitch = dialogue.sentences[currentSentence].voicebank.pitch;
                typewriter.pitchVariation = dialogue.sentences[currentSentence].voicebank.pitchVariation;
            }
        }

        private float SetTextSpeed()
        {
            if (dialogue.sentences[currentSentence].textSpeed > 0) return dialogue.sentences[currentSentence].textSpeed;
            else if (dialogue.baseTextSpeed > 0) return dialogue.baseTextSpeed;
            else return Dialogue.defaultBaseTextSpeed;
        }
        private void AdjustText()
        {
            textSizeAdjuster?.Adjust();
            adjustToOtherRect?.Adjust();
        }

        //private void ForceRebuild()
        //{
        //    StartCoroutine(RebuildCoroutine());
        //    //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        //    //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform); //called two times on purpose
        //} 

        private IEnumerator RebuildCoroutine()
        {
            if (rootLayout == null) yield break;
            //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            //rootLayout.enabled = false;
            yield return new WaitForEndOfFrame();
            //rootLayout.enabled = true;
        }

        private void Update()
        {
            SkipTimeTimer();

            if (textboxInitiated)
            {
                if (UIController.instance.skipButtonHold)
                {
                    if (skipTime == 0)
                    {
                        AdvanceText();
                        skipTime = skipTimeBuffer;
                    }
                }
                if (UIController.instance.actionButtonDown)
                {
                    if (!UIController.instance.skipButtonHold)
                        AdvanceText();
                }
                if (!IsTextPlaying() && autoContinue)
                {
                    AdvanceText();
                }
            }
        }

        private void SkipTimeTimer()
        {
            if (skipTime != 0)
            {
                skipTime -= Time.deltaTime;
                if (skipTime <= 0)
                {
                    skipTime = 0;
                }
            }
        }
        private bool IsTextPlaying()
        {
            return text.maxVisibleCharacters < text.GetParsedText().Length;
        }
        private void AdvanceText()
        {
            if (IsTextPlaying())
            {
                typewriter.Skip();
            }
            else
            { 
                if (autoContinue)
                {
                    if (autoContinueCoroutine == null)
                    {
                        autoContinueCoroutine = AdvanceTextCoroutine();
                        StartCoroutine(autoContinueCoroutine);
                    }
                }
                else DisplayNextSentence(); 
            }
        }
        private IEnumerator AdvanceTextCoroutine()
        {
            yield return new WaitForSeconds(m_autoEndDelay);
            DisplayNextSentence();
            autoContinueCoroutine = null;
        }

        public void StartDialogue()
        {
            InitialValues();
            
            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            if (currentSentence >= sentences.Count)
            {
                if (textboxInitiated)
                {
                    if (!ChoicesIsNext()) CloseTextbox(); // Change this to buffer
                    else {
                        if (actionCallback != null) actionCallback.EndEvent();
                        textboxInitiated = false;
                    }
                }
                return;
            }
            DisplayContinuePrompt(false);
            SetVoicebank();
            SetTextColor();
            autoContinue = false;
            var savedTags = new List<TUFFTextParser.TagData>();
            string text = sentences[currentSentence];
            TMP_Text typewriterText = typewriter.textUI;
            if (typewriterText)
            {
                typewriterText.text = text;
                typewriterText.ForceMeshUpdate();
                string parsedText = typewriterText.GetParsedText();
                savedTags = TUFFTextParser.GetTags(parsedText);
            }
            text = TUFFTextParser.ParseText(text);
            if (savedTags.Exists(e => e.type == TUFFTextParser.TextTagType.AutoContinue) || (currentSentence >= sentences.Count - 1 && ChoicesIsNext()))
            {
                autoContinue = true;
            }

            typewriter.Play(text, SetTextSpeed(), savedTags, () => { DisplayContinuePrompt(true); });
            currentSentence++;
            AdjustText();
            SetPosition();
        }

        private void DisplayContinuePrompt(bool display)
        {
            if (continuePrompt != null)
            {
                if (display && autoContinue) return;
                continuePrompt.gameObject.SetActive(display);
            }
        }
        public void CloseTextbox()
        {
            StartCoroutine(EndDialogue());
        }
        protected IEnumerator EndDialogue()
        {
            textboxInitiated = false;
            m_continuedDialogue = false;
            dialogue.onDialogueEnd?.Invoke();
            if (uiMenu != null) UIController.instance.SetMenu(null);
            BattleManager.instance.hud.ShowWindowsDynamic(true);
            if (actionCallback != null) actionCallback.EndEvent();
            if (transition != null)
            {
                if (DialogueIsNext()) // Change this to buffer
                {
                    m_continuedDialogue = true;
                }
                else
                {
                    text.enabled = false;
                    DisplayContinuePrompt(false);
                    transition?.Dissapear();
                    while (transition.state == BoxTransitionState.Dissapearing)
                    {
                        yield return null;
                    }
                }
            }
            openBoxes.RemoveAt(openBoxes.IndexOf(this));
            inUse = false;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        private bool DialogueIsNext() // Change this to buffer
        {
            if (actionCallback == null) return false;
            int index = actionCallback.parent.GetActionIndex(actionCallback);
            if (index < 0 || index >= actionCallback.parent.content.Count - 1) return false;
            return actionCallback.parent.content[index + 1] is ShowDialogueAction;
        }
        private bool ChoicesIsNext() // Change this to buffer
        {
            if (actionCallback == null) return false;
            int index = actionCallback.parent.GetActionIndex(actionCallback);
            if (index < 0 || index >= actionCallback.parent.content.Count - 1) return false;
            return actionCallback.parent.content[index + 1] is ShowChoicesAction;
        }
    }
}
