using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditorInternal;

namespace TUFF.TUFFEditor
{
    public class EventActionListWindow : EditorWindow
    {
        public static EventActionListWindow instance;
        public static List<EventAction> eventList;
        public static List<EventActionPD> mainEventListPDs;
        public static Object rootObject
        {
            get
            {
                if (commonEvent != null) return commonEvent;
                if (interactableObject != null) return interactableObject;
                if (battle != null) return battle;
                return null;
            }
        }
        public static string rootName
        {
            get
            {
                string name = "";
                if(rootObject != null) name += rootObject?.name;
                if (rootObject == interactableObject) name += $" / Trigger #{triggerIndex}";
                return name;
            }
        }

        public static CommonEvent commonEvent;
        public static InteractableObject interactableObject;
        public static Battle battle;
        public static InteractableEvent[] ioTriggerEvents;

        private static int triggerIndex = 0;
        private static int lowerPanelTargetIdx = -1;
        private static EventActionPD lowerPanelPD;
        private static List<EventAction> panelEventListTarget;
        private static List<EventActionPD> panelEventListEditorsTarget;
        private static int replaceTarget = -1;
        private static string addPanelTitle = "";

        private static Vector2 eventScrollPos;
        private static Vector2 downScrollPos;
        private static Vector2 upScrollPos;
        private static float scrollViewHeight;
        private static Rect boxLineArea;
        private static bool resizing = false;
        private static Texture2D lineTexture;

        private static readonly Vector2 windowMinSize = new Vector2(100f, 200f);
        private const float lineHeight = 10f;
        private const float linePositionLimit = 54f;
        private const float triggerButtonsMinHeight = 16f;
        private const float upperPanelMinHeight = 54f;
        private const float lowerPanelMinHeight = 54f;

        private static ReorderableList listLayout;
        private static ReorderableList list;
        private static bool eventDeleted = false;

        private static SerializedProperty listContentProperty;
        private static List<EventAction> listEventList;
        private static List<EventActionPD> listEventListPDs;
        private static string listSelectionPanelTitle;
        [Tooltip("(ParentPropertyPath, List<EventActionPD>)")]
        private static Dictionary<string, List<EventActionPD>> listParentsDictionary = new(); // (parent property path, list class)
        //private static Dictionary<SerializedProperty, List<EventActionPD>> listEventListPDDictionary = new(); // (parent property path, list class)
        [Tooltip("(ParentPropertyPath, ReorderableList)")]
        private static Dictionary<string, ReorderableList> listsDictionary = new();

        //private static List<EventAction> prevListEventList;
        //private static List<EventActionPD> prevListEventListPDs;
        //private static string prevListSelectionPanelTitle;

        private static SerializedObject selectedSerializedObject;
        private static SerializedProperty SelectedContentProperty { get {
                if (selectedSerializedObject != null)
                {
                    if (commonEvent != null)
                        return selectedSerializedObject.FindProperty("actionList.content");
                    if (interactableObject != null && triggerIndex >= 0)
                        return selectedSerializedObject.FindProperty("triggerEvents").GetArrayElementAtIndex(triggerIndex).FindPropertyRelative("actionList.content");
                    if (battle != null)
                        return selectedSerializedObject.FindProperty("battleEvents").GetArrayElementAtIndex(triggerIndex).FindPropertyRelative("actionList.content");
                }
                return null; 
        }}

        public static void ShowWindow(int index)
        {
            instance = GetWindow<EventActionListWindow>("Event Action List");
            //ResetReferences();
            triggerIndex = index;
            instance.AdjustSize();
        }
        [MenuItem("TUFF/Event Action List Window")]
        public static void ShowWindow()
        {
            ShowWindow(0);
        }
        private void AdjustSize()
        {
            scrollViewHeight = (position.height * 0.5f);
            boxLineArea = new Rect(0, scrollViewHeight, position.width, lineHeight);
            lineTexture = EditorGUIUtility.whiteTexture;
            minSize = windowMinSize;
        }
        private void OnInspectorUpdate()
        {
            Repaint();
        }
        private void OnGUI()
        {
            GetReferences();
            if (eventList == null)
            {
                GUILayout.Label("Event List is missing.", EditorStyles.boldLabel);
                return;
            }
            ShowTriggerButtons();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Clamp(position.width * 0.4f, 140f, Mathf.Infinity);
            UpperPanelGUI(position);
            UpdateScrollView(position);
            LowerPanelGUI(position, panelEventListTarget, panelEventListEditorsTarget);
            EditorGUIUtility.labelWidth = labelWidth;
        }
        private static void UpperPanelGUI(Rect position)
        {
            float scrollHeight = scrollViewHeight - linePositionLimit - triggerButtonsMinHeight;
            upScrollPos = EditorGUILayout.BeginScrollView(upScrollPos, GUILayout.Height(scrollHeight));
            GUILayout.BeginVertical();
            ShowTitle($"{rootName} Event List Content");

            //DisplayEventListContentGUI(position, eventList, mainEventListPDs, rootName);
            var controlRect = EditorGUILayout.GetControlRect();
            
            DisplayEventListContent(controlRect, eventList, mainEventListPDs, rootName, SelectedContentProperty, SelectedContentProperty.propertyPath);
            float height = GetDisplayEventListContentHeight();
            GUILayout.Space(height);
            EditorGUILayout.LabelField("Height: " + height);

            //Debug.Log("===");
            //// test
            //foreach(var content in listsDictionary)
            //{
            //    Debug.Log(content.Value.GetHeight());
            //}

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void LowerPanelGUI(Rect position, List<EventAction> eventList, List<EventActionPD> eventListPDs)
        {
            downScrollPos = GUILayout.BeginScrollView(downScrollPos, GUILayout.Height(position.height - scrollViewHeight));
            if (eventDeleted) DisplaySelectionPanel(eventList, mainEventListPDs, rootName, contentProperty: listContentProperty);
            EditorGUILayout.BeginVertical();

            GUILayout.Label($"Lower Panel: {lowerPanelTargetIdx}");
            if (lowerPanelPD != null)
            {
                ShowTitle($"{rootName} #{lowerPanelTargetIdx}: {lowerPanelPD.GetEventName()}");
                lowerPanelPD.PanelGUI();
            }
            else
            {
                EventActionSelectionWindow.ShowPanelContent(eventList, eventListPDs, addPanelTitle);
            }

            EditorGUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        private static void UpdateScrollView(Rect position)
        {
            Rect mouseValidArea = new Rect(boxLineArea.x, boxLineArea.y - 2, boxLineArea.width, boxLineArea.height + 4);
            EditorGUIUtility.AddCursorRect(mouseValidArea, MouseCursor.ResizeVertical);
            if (mouseValidArea.Contains(Event.current.mousePosition))
            {
                GUI.DrawTexture(boxLineArea, lineTexture, ScaleMode.StretchToFill, true, 0, Color.white, 0, 0);
                if (Event.current.type == EventType.MouseDown) resizing = true;
            }
            else GUI.DrawTexture(boxLineArea, lineTexture, ScaleMode.StretchToFill, true, 0, Color.grey, 0, 0);
            if (resizing)
            {
                scrollViewHeight = Event.current.mousePosition.y;
            }
            scrollViewHeight = Mathf.Clamp(scrollViewHeight, linePositionLimit + triggerButtonsMinHeight + upperPanelMinHeight, position.height - linePositionLimit - lowerPanelMinHeight);
            if (Event.current.type == EventType.MouseUp)
            {
                resizing = false;
            }
            boxLineArea.Set(boxLineArea.x, scrollViewHeight, position.width, boxLineArea.height);
        }
        /// <summary>
        /// Draws the Action List in EditorGUI mode.
        /// </summary>
        /// <param name="position">Reference to Rect variable</param>
        /// <param name="eventList">Reference to Action List Content</param>
        /// <param name="eventListPDs">Reference to Action List Content Drawers</param>
        /// <param name="selectionPanelTitle">Display name in lower panel</param>
        /// <param name="contentProperty">Optional. Content Property to Update. Set to null to update the main property (SelectedContentProperty)</param>
        public static void DisplayEventListContent(Rect position, List<EventAction> eventList, List<EventActionPD> eventListPDs, string selectionPanelTitle, SerializedProperty contentProperty, string parentPropertyPath)
        {
            if (!listParentsDictionary.ContainsKey(parentPropertyPath))
                listParentsDictionary.Add(parentPropertyPath, eventListPDs);

            EditorGUI.LabelField(position, $"Event Count: {eventList.Count} || {listsDictionary.Count} || { listParentsDictionary.Count }" );
            position.y += 20f;
            EditorGUI.LabelField(position, $"Event Editor Count: {eventListPDs.Count}");
            position.y += 20f;

            //prevListEventList = listEventList;
            //prevListEventListPDs = listEventListPDs;
            //prevListSelectionPanelTitle = listSelectionPanelTitle;

            var prevList = list;
            listEventList = eventList;
            listEventListPDs = eventListPDs;
            listSelectionPanelTitle = selectionPanelTitle;
            
            //if (list == null) GetList(ref list, eventList);
            if (listsDictionary.ContainsKey(parentPropertyPath))
            {
                list = listsDictionary[parentPropertyPath];
            }
            else
            {
                GetList(ref list, eventList);
                listsDictionary.Add(parentPropertyPath, list);
            }
            var curList = list;

            //UpdateListContentProperty(contentProperty);
            Undo.RecordObject(rootObject, "Reordered event in Event List");
            EditorGUI.BeginChangeCheck();
            curList.DoList(position);
            if (curList != null) { position.y += curList.GetHeight(); };
            if (EditorGUI.EndChangeCheck())
            {
                list = null;
                MarkDirty();
            }
            DisplayButtons(position, eventList, eventListPDs, selectionPanelTitle, contentProperty);
            position.y += 40f;

            //listEventList = prevListEventList;
            //listEventListPDs = prevListEventListPDs;
            //listSelectionPanelTitle = prevListSelectionPanelTitle;

            if (list == null)
            {
                ResetList();
                return;
            }
            list = prevList;
        }
        public static float GetDisplayEventListContentHeight(ReorderableList list)
        {
            float listHeight = (list != null ? list.GetHeight() : 0f);
            return 80f + listHeight; 
        }
        public static float GetDisplayEventListContentHeight()
        {
            if (listsDictionary.ContainsKey(SelectedContentProperty.propertyPath))
            {
                list = listsDictionary[SelectedContentProperty.propertyPath];
            }
            return GetDisplayEventListContentHeight(list);
        }
        public static float GetListHeight(string contentPath)
        {
            var prevList = list;
            if (listsDictionary.ContainsKey(contentPath))
            {
                list = listsDictionary[contentPath];
            }
            else { Debug.Log("bruh"); return 0f; }
            float height = GetDisplayEventListContentHeight(list);
            list = prevList;
            return height;
        }
        /// <summary>
        /// Draws the Action List in EditorGUILayout mode.
        /// </summary>
        /// <param name="position">Reference to Rect variable</param>
        /// <param name="eventList">Reference to Action List Content</param>
        /// <param name="eventListPDs">Reference to Action List Content Drawers</param>
        /// <param name="selectionPanelTitle">Display name in lower panel</param>
        /// <param name="contentProperty">Optional. Content Property to Update. Set to null to update the main property (SelectedContentProperty)</param>
        public static void DisplayEventListContentGUI(Rect position, List<EventAction> eventList, List<EventActionPD> eventListPDs, string selectionPanelTitle, SerializedProperty contentProperty = null)
        {
            GUILayout.Label($"Event Count: {eventList.Count}");
            GUILayout.Label($"Event Editor Count: {eventListPDs.Count}");

            //prevListEventList = listEventList;
            //prevListEventListPDs = listEventListPDs;
            //prevListSelectionPanelTitle = listSelectionPanelTitle;

            listEventList = eventList;
            listEventListPDs = eventListPDs;
            listSelectionPanelTitle = selectionPanelTitle;

            Undo.RecordObject(rootObject, "Reordered event in Event List");
            if (listLayout == null) { GetList(ref listLayout, eventList); }
            UpdateListContentProperty(contentProperty);
            EditorGUI.BeginChangeCheck();
            listLayout.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                listLayout = null;
                MarkDirty();
            }
            DisplayButtonsLayout(position, eventList, eventListPDs, selectionPanelTitle, contentProperty);

            //listEventList = prevListEventList;
            //listEventListPDs = prevListEventListPDs;
            //listSelectionPanelTitle = prevListSelectionPanelTitle;

            if (listLayout == null)
            {
                ResetList();
            }
        }

        private static void ResetList()
        {
            list = null;
            listLayout = null;
            listEventList = null;
            listEventListPDs = null;
            listSelectionPanelTitle = null;
            //prevListEventList = null;
            //prevListEventListPDs = null;
            //prevListSelectionPanelTitle = null;
            mainEventListPDs = null;
        }

        private static void CommandDefaultButtons(ref Rect rect, List<EventAction> eventList, List<EventActionPD> eventListPDs, int i, string selectionPanelTitle, ref bool markListDirty, SerializedProperty contentProperty = null)
        {
            float orgX = rect.x;
            float orgWidth = rect.width;
            float orgHeight = rect.height;
            float width = orgWidth * 0.6f;
            rect.height = 20f;
            rect.width = width;

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.alignment = TextAnchor.MiddleLeft;
            btnStyle.fontStyle = FontStyle.Bold;
            GUIContent btn = new GUIContent($"{i}: {eventList[i].eventName} ({ eventList[i].GetType() }) ", "Modify event command content."); //Modify Button
            if (GUI.Button(rect, btn, btnStyle))
            {
                lowerPanelTargetIdx = i;
                OpenWindowOfElement(eventListPDs[i], i);
            }
            int buttons = 3;
            float buttonWidth = orgWidth * (0.4f / buttons);
            rect.x += width;
            width = buttonWidth;
            rect.width = width;
            GUIContent change = new GUIContent("Swap Action", "Replace this event action."); //Change Button
            if (GUI.Button(rect, change))
            {
                DisplaySelectionPanel(eventList, eventListPDs, selectionPanelTitle, i, contentProperty);
            }
            rect.x += width;
            width = buttonWidth;
            rect.width = width;
            GUIContent duplicate = new GUIContent("Duplicate", "Duplicate this event action."); //Duplicate Button
            if (GUI.Button(rect, duplicate))
            {
                replaceTarget = -1;
                UpdateListContentProperty(contentProperty);
                AddEvent(LISAUtility.Copy(eventList[i]) as EventAction, eventList, eventListPDs, i + 1);
                DisplaySelectionPanel(eventList, mainEventListPDs, selectionPanelTitle, contentProperty: contentProperty);
                markListDirty = true;
            }
            rect.x += width;
            width = buttonWidth;
            rect.width = width;
            GUIContent remove = new GUIContent("Remove", "Remove this event action."); //Remove Button
            if (GUI.Button(rect, remove))
            {
                RemoveEvent(i, eventList, eventListPDs, contentProperty);
                markListDirty = true;
            }
            rect.width = orgWidth;
            rect.height = orgHeight;
            rect.x = orgX;
        }
        public static void DisplayButtons(Rect position, List<EventAction> eventList, List<EventActionPD> eventListPDs, string title, SerializedProperty contentProperty = null)
        {
            GUIContent add = new GUIContent("+", "Add an event action.");
            if (GUI.Button(position, add))
            {
                DisplaySelectionPanel(eventList, eventListPDs, title, contentProperty: contentProperty);
            }
            GUIContent remove = new GUIContent("-", "Remove an event action.");
            position.y += 20f;
            if (GUI.Button(position, remove))
            {
                if (eventList.Count > 0)
                {
                    int removeIdx = eventList.Count - 1;
                    RemoveEvent(removeIdx, eventList, eventListPDs, contentProperty);
                    DisplaySelectionPanel(eventList, eventListPDs, title);
                }
            }
            position.y += 20f;
        }
        private static void DisplayButtonsLayout(Rect position, List<EventAction> eventList, List<EventActionPD> eventListPDs, string title, SerializedProperty contentProperty = null)
        {
            GUILayout.Space(18);
            GUILayout.BeginVertical();
            GUIContent add = new GUIContent("+", "Add an event action.");
            if (GUILayout.Button(add, GUILayout.Width(position.width - 6f)))
            {
                DisplaySelectionPanel(eventList, eventListPDs, title, contentProperty: contentProperty);
            }
            GUIContent remove = new GUIContent("-", "Remove an event action.");
            if (GUILayout.Button(remove, GUILayout.Width(position.width - 6f)))
            {
                if (eventList.Count > 0)
                {
                    int removeIdx = eventList.Count - 1;
                    RemoveEvent(removeIdx, eventList, eventListPDs, contentProperty);
                    DisplaySelectionPanel(eventList, eventListPDs, title);
                }
            }
            GUILayout.EndVertical();
        }
        private static void ShowTitle(string title)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private static void ShowTriggersLabel()
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{rootName} Events", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private static void ShowTriggerButtons()
        {
            eventScrollPos = EditorGUILayout.BeginScrollView(eventScrollPos, GUILayout.Height(triggerButtonsMinHeight + linePositionLimit));
            EditorGUILayout.BeginVertical();
            ShowTriggersLabel();
            EditorGUILayout.BeginHorizontal("box");
            DrawTriggerButtons();
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void DrawTriggerButtons()
        {
            int length = 0;
            if (interactableObject != null) length = ioTriggerEvents.Length;
            else if (battle != null) length = battle.battleEvents.Length; // tmp

            for (int i = 0; i < length; i++)
            {
                var idx = new GUIContent($"{i}");

                if (i == triggerIndex) GUI.backgroundColor = Color.gray;
                else GUI.backgroundColor = Color.white;
                if (GUILayout.Button(idx, EditorStyles.miniButton, GUILayout.Width(50f)))
                {
                    ResetReferences();
                    triggerIndex = i;
                    GetReferences();
                }
            }
        }
        private static void DisplaySelectionPanel(List<EventAction> eventList, List<EventActionPD> eventListEditors, string targetTitle = "", int replaceIdx = -1, SerializedProperty contentProperty = null)
        {
            lowerPanelTargetIdx = -1;
            lowerPanelPD = null;
            panelEventListTarget = eventList;
            panelEventListEditorsTarget = eventListEditors;
            replaceTarget = replaceIdx;
            addPanelTitle = targetTitle;
            if (replaceIdx >= 0) addPanelTitle = $"{targetTitle} Replace Event {replaceIdx}";
            UpdateListContentProperty(contentProperty);
        }

        private static void UpdateListContentProperty(SerializedProperty contentProperty)
        {
            if (contentProperty == null) listContentProperty = SelectedContentProperty;
            else listContentProperty = contentProperty;
            //Debug.Log("=UPD: " + listContentProperty.propertyPath);
        }

        private static void OpenWindowOfElement(EventActionPD eventActionPD, int i)
        {
            lowerPanelTargetIdx = i;
            lowerPanelPD = eventActionPD;
        }
        private static void GetList(ref ReorderableList list, List<EventAction> actionListContent)
        {
            list = new ReorderableList(actionListContent, typeof(EventAction), true, false, false, false);
            list.drawElementCallback = DrawListItems;
            list.elementHeightCallback = GetElementHeight;
        }
        private static float GetElementHeight(int index)
        {
            if (eventDeleted) return 0f;
            
            if (list != null)
            {
                var property = list.serializedProperty; //NOTE: Don't use serializedProperty to get list parentProperty (assigning it won't allow the list to move).
                if (property != null)
                {
                    Debug.Log("There's properrty");
                    if (property.arraySize <= 0 || index >= property.arraySize) { return 20f; }//list.GetHeight();
                    var element = property.GetArrayElementAtIndex(index);
                    if (listParentsDictionary.ContainsKey(element.propertyPath))
                    {
                        var prevList = list;
                        var prevActionListPDs = listEventListPDs;
                        list = listsDictionary[element.propertyPath];
                        listEventListPDs = listParentsDictionary[element.propertyPath];
                        float height = list.GetHeight();
                        list = prevList;
                        listEventListPDs = prevActionListPDs;
                        return height;
                    }
                }
            }
            //if (property == null) return 100f;

            var actionListPDs = listEventListPDs;

            if (index >= actionListPDs.Count) return 0f;
            if (actionListPDs.Count <= 0 || actionListPDs[index] == null) return 0f;
            float value = 20f + actionListPDs[index].GetSummaryHeight();
            //listEventListPDs = prevActionListPDs;
            return value;
        }
        private static void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            //int i = index;
            var prevActionList = listEventList;
            var prevActionListPDs = listEventListPDs;
            var prevListContentProperty = listContentProperty;

            var actionList = listEventList;
            var actionListPDs = listEventListPDs;
            var curListContentProperty = listContentProperty;

            var prevGUIColor = GUI.color;
            if (eventDeleted) return;
            if (actionList.Count <= 0) return;
            if (index < 0 || index >= actionList.Count) { Debug.LogWarning($"Index {index} is invalid! Count: {actionList.Count}"); return; }
            if (actionList[index] == null) { Debug.LogWarning($"Index {index} is empty! Count: {actionList.Count}"); return; }

            GUI.color = actionList[index].eventColor;
            rect.height = 20f;
            bool markListDirty = false;
            CommandDefaultButtons(ref rect, actionList, actionListPDs, index, listSelectionPanelTitle, ref markListDirty, curListContentProperty);
            rect.y += 20f;
            if (markListDirty)
            {
                GUI.color = prevGUIColor;
                eventDeleted = true;
                return;
            }
            if (actionListPDs.Count > 0 && index < actionListPDs.Count && actionListPDs[index] != null) 
                actionListPDs[index].SummaryGUI(rect);
            GUI.color = prevGUIColor;

            listEventList = prevActionList;
            listEventListPDs = prevActionListPDs;
            listContentProperty = prevListContentProperty;
        }
        private static void GetReferences()
        {
            eventDeleted = false;
            //Debug.Log(Selection.activeObject);
            if (Selection.activeObject == null)
            {
                ResetReferences();
                return;
            }
            var actObj = Selection.activeObject;
            if (actObj is CommonEvent)
            {
                if (commonEvent != Selection.activeObject)
                {
                    ResetReferences();
                }
                commonEvent = (CommonEvent)Selection.activeObject;
                eventList = commonEvent.actionList.content;
                selectedSerializedObject = new SerializedObject(commonEvent);
            }
            else if (actObj is GameObject gameObj)
            {
                var getIO = gameObj.GetComponent<InteractableObject>();
                var getBattle = gameObj.GetComponent<Battle>();
                if (getIO != null)
                {
                    if (interactableObject != getIO)
                    {
                        ResetReferences();
                    }
                    
                    interactableObject = getIO;
                    ioTriggerEvents = interactableObject.triggerEvents;
                    if (ioTriggerEvents.Length <= 0)
                    {
                        //triggerIndex = 0;
                        ResetReferences();
                        return;
                    }
                    var evts = ioTriggerEvents;
                    if (evts.Length <= 0)
                    {
                        ResetReferences();
                        return;
                    }
                    if (triggerIndex >= evts.Length) triggerIndex = evts.Length - 1;
                    eventList = evts[triggerIndex].actionList.content;
                    selectedSerializedObject = new SerializedObject(interactableObject);
                }
                else if (getBattle != null)
                {
                    if (battle != getBattle)
                    {
                        //triggerIndex = 0;
                        ResetReferences();
                        //return;
                    }
                    battle = getBattle;
                    var evts = battle.battleEvents;
                    if (evts.Length <= 0)
                    {
                        ResetReferences();
                        return;
                    }
                    if (triggerIndex >= evts.Length) triggerIndex = evts.Length - 1;
                    eventList = evts[triggerIndex].actionList.content; // [0] is tmp
                    selectedSerializedObject = new SerializedObject(battle);
                }
            }


            // Common Event
            //Selection.activeTransform.GetComponent<InteractableObject>();
            // Common Event End
            if (rootObject == null)
            {
                ResetReferences();
                return;
            }
            //iotriggerEvents = interactableObject.content;

            //if (triggerEvents.Length == 0)
            //{
            //    ResetReferences();
            //    return;
            //}
            //contentName = $"{interactableObject.name} Trigger #{triggerIndex}";

            //contentName = $"{commonEvent.name} Trigger #{triggerIndex}";
            
            if (mainEventListPDs == null)
            {
                mainEventListPDs = new List<EventActionPD>();
                UpdatePDs(SelectedContentProperty, eventList, mainEventListPDs);
            }
            if (panelEventListTarget == null)
            {
                DisplaySelectionPanel(eventList, mainEventListPDs, rootName);
            }
            if (mainEventListPDs.Count != eventList.Count)
            {
                UpdatePDs(SelectedContentProperty, eventList, mainEventListPDs);
            }
        }
        public static void ResetReferences()
        {
            eventList = null;
            lowerPanelTargetIdx = -1;
            selectedSerializedObject = null;
            mainEventListPDs = null;
            lowerPanelTargetIdx = -1;
            lowerPanelPD = null;
            panelEventListTarget = null;
            panelEventListEditorsTarget = null;
            replaceTarget = -1;
            addPanelTitle = "";
            //triggerIndex = 0;

            list = null;
            listLayout = null;
            listEventList = null;
            listEventListPDs = null;
            listSelectionPanelTitle = null;

            listsDictionary.Clear();
            listParentsDictionary.Clear();
            //prevListEventList = null;
            //prevListEventListPDs = null;
            //prevListSelectionPanelTitle = null;

            commonEvent = null;
            interactableObject = null;
            battle = null;

            upScrollPos = new Vector2();

            //assetCreated = false;

            //referencesReset = true;
        }
        public static void UpdatePDs(SerializedProperty contentProperty, List<EventAction> eventList, List<EventActionPD> eventListPDs, bool lol = false)
        {
            if (eventList != null)
            {
                var serializedObject = contentProperty.serializedObject;
                //serializedObject.Update();
                if (eventListPDs == null) eventListPDs = new List<EventActionPD>();
                else eventListPDs.Clear();

                if (lol) Debug.Log(serializedObject.targetObject.name + ": " + contentProperty.propertyPath);
                if (lol) Debug.Log("BEFORE: " + contentProperty.arraySize);
                serializedObject.Update();
                if (lol) Debug.Log("AFTER: " + contentProperty.arraySize);

                for (int i = 0; i < eventList.Count; i++)
                {
                    EventActionPD pd = CreatePD(contentProperty.GetArrayElementAtIndex(i));
                    eventListPDs.Add(pd);
                }
                serializedObject.Update();
            }
        }

        private static EventActionPD CreatePD(SerializedProperty property)
        {
            property.serializedObject.Update();
            var contentIndexProperty = property;
            var pd = LISAEditorUtility.GetPropertyDrawer(contentIndexProperty) as EventActionPD;
            pd.targetProperty = contentIndexProperty;
            return pd;
        }
        private static void MarkDirty()
        {
            EditorUtility.SetDirty(rootObject);
            //var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            //if (prefabStage != null)
            //{
            //    if (prefabStage.IsPartOfPrefabContents(interactableObject.gameObject))
            //        PrefabUtility.RecordPrefabInstancePropertyModifications(interactableObject);
            //}
        }
        public static void AddEvent(EventAction eventAction, List<EventAction> eventList, List<EventActionPD> eventListPDs, int insertAt = -1)
        {
            if (eventAction == null) return;
            Debug.Log(listContentProperty.propertyPath);
            Undo.RecordObject(rootObject, "Added event in Event List");
            eventAction.OnInstantiate();
            if (replaceTarget >= 0)
            {
                insertAt = replaceTarget;
                eventList.RemoveAt(replaceTarget);
            }
            if (insertAt >= 0)
            {
                eventList.Insert(insertAt, eventAction);
            }
            else eventList.Add(eventAction);

            ResetList();
            UpdatePDs(listContentProperty, eventList, eventListPDs, true);

            var pd = CreatePD(listContentProperty.GetArrayElementAtIndex(eventList.Count - 1));
            pd.OnEditorInstantiate();

            MarkDirty();
        }
        public static void RemoveEvent(int index, List<EventAction> eventList, List<EventActionPD> eventListPDs, SerializedProperty contentProperty = null)
        {
            Undo.RecordObject(rootObject, "Removed event in Event List");
            eventList.RemoveAt(index);

            UpdateListContentProperty(contentProperty);
            UpdatePDs(listContentProperty, eventList, eventListPDs, true);
            MarkDirty();
        }
        private void OnEnable()
        {
            Undo.undoRedoPerformed += () => { UpdatePDs(SelectedContentProperty, eventList, mainEventListPDs); };
            EditorApplication.playModeStateChanged += ModeChange;
            AdjustSize();
        }
        private void OnDisable()
        {
            Undo.undoRedoPerformed += () => { UpdatePDs(SelectedContentProperty, eventList, mainEventListPDs); };
            EditorApplication.playModeStateChanged -= ModeChange;
            ResetReferences();
        }
        private static void ModeChange(PlayModeStateChange playModeState)
        {
            ResetReferences();
        }
    }
}
