using System;
using System.Collections.Generic;
using Darklight.UnityExt;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Utility;
using Ink.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.Inky
{
    /// <summary>
    ///  Singleton class for handling the data from Ink Stories and decrypting them into interpretable game data.
    /// </summary>
    public class InkyStoryManager : MonoBehaviourSingleton<InkyStoryManager>, IUnityEditorListener
    {
        [SerializeField]
        InkyStoryObject _globalStoryObject;

        [SerializeField, ShowOnly]
        List<string> _speakerList;

        [SerializeField, ShowOnly]
        List<string> _globalKnots;

        // ------------------------ [[ GLOBAL STORY OBJECT ]] ------------------------ >>

        public static InkyStoryObject GlobalStoryObject
        {
            get { return Instance._globalStoryObject; }
        }

        /// <summary>
        /// List of all the speakers in the Inky Story.
        /// </summary>
        public static List<string> SpeakerList
        {
            get { return Instance._speakerList; }
        }

        /// <summary>
        /// List of all of the knot names in the Inky Story.
        /// </summary>
        public static List<string> GlobalKnots
        {
            get { return Instance._globalKnots; }
        }

        // ----------- [[ STORY ITERATOR ]] ------------ >>
        public static InkyStoryIterator Iterator { get; private set; }

        public delegate void StoryInitialized(Story story);
        public event StoryInitialized OnStoryInitialized;

        #region ----- [[ SPEAKER HANDLING ]] ------------------------ >>
        [Tooltip("The current speaker in the story.")]
        [ShowOnly, SerializeField]
        string _currentSpeaker;
        public string CurrentSpeaker => _currentSpeaker;

        public delegate void SpeakerSet(string speaker);
        public event SpeakerSet OnSpeakerSet;

        /// <summary>
        /// This is the external function that is called from the Ink story to set the current speaker.
        /// </summary>
        public object SetSpeaker(object[] args)
        {
            string speaker = (string)args[0];
            _currentSpeaker = speaker;
            OnSpeakerSet?.Invoke(speaker);
            Debug.Log($"{Prefix} >> Set Speaker: {speaker}");
            return false;
        }

        /// <summary>
        /// This is the forceful way to set the speaker value.
        /// </summary>
        /// <param name="speaker"></param>
        public void SetSpeaker(string speaker)
        {
            _currentSpeaker = speaker;
            OnSpeakerSet?.Invoke(speaker);
        }

        #endregion


        // ------------------------ [[ METHODS ]] ------------------------ >>
        public void OnEditorReloaded()
        {
            Initialize();
        }

        public override void Initialize()
        {
            if (_globalStoryObject == null)
                return;
            _globalStoryObject.Initialize(); // << INITIALIZE STORY DATA >>

            // << GET VARIABLES >>
            _speakerList = _globalStoryObject.GetVariableByName("Speaker").ToStringList();
            //Debug.Log($"{Prefix} >> Speaker List Count : {SpeakerList.Count}");

            _globalKnots = _globalStoryObject.KnotNames;
            //Debug.Log($"{Prefix} >> Global Knots Count : {GlobalKnots.Count}");

            // << OBSERVE VARIABLES >>
            _globalStoryObject.StoryValue.ObserveVariable(
                "CURRENT_SPEAKER",
                (string varName, object newValue) =>
                {
                    if (_currentSpeaker == newValue.ToString())
                        return;
                    _currentSpeaker = newValue.ToString();
                    OnSpeakerSet?.Invoke(_currentSpeaker);
                    Debug.Log($"{Prefix} >> Current Speaker: {_currentSpeaker}");
                }
            );

            // << INITIALIZE STORY ITERATOR >>
            Iterator = new InkyStoryIterator(_globalStoryObject);

            Debug.Log($"{Prefix} >> Initialized Inky Story Manager.");
            OnStoryInitialized?.Invoke(_globalStoryObject.StoryValue);
        }

        object QuestStarted(object[] args)
        {
            Debug.Log("Quest Started! >> " + args[0].ToString());
            return false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(InkyStoryManager))]
    public class InkyStoryManagerCustomEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        InkyStoryManager _script;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (InkyStoryManager)target;
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Initialize"))
            {
                _script.Initialize();
            }
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}
