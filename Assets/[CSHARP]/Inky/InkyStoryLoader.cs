using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Class to load all Ink stories from the Resources/Inky directory.
/// </summary>
public class InkyStoryLoader : MonoBehaviour
{
    // ------ [[ PRIVATE FIELDS ]] ------ >>
    private Dictionary<string, InkyStoryObject> _storyObjectDict = new Dictionary<string, InkyStoryObject>();

    // ------ [[ SERIALIZED FIELDS ]] ------ >>
    [SerializeField] InkyStoryObject[] _storyObjects = new InkyStoryObject[0];


    // ------ [[ PUBLIC ACCESSORS ]] ------ >>
    public bool allStoriesLoaded { get; private set; }


    // ------ [[ PUBLIC METHODS ]] ------ >>
    /// <summary>
    /// Loads all Ink story files found in the Resources/Inky directory.
    /// </summary>
    public void LoadAllStories()
    {
        // Load all text assets from the Inky resources directory
        Dictionary<string, InkyStoryObject> stories = new Dictionary<string, InkyStoryObject>();
        UnityEngine.Object[] storyAssets = Resources.LoadAll("Inky/", typeof(TextAsset));
        foreach (TextAsset storyAsset in storyAssets)
        {
            InkyStoryObject inkyStory = ScriptableObject.CreateInstance<InkyStoryObject>();
            inkyStory.Initialize(storyAsset);
            SaveInkyStoryObject(storyAsset);

            stories.Add(storyAsset.name, inkyStory);
        }

        _storyObjectDict = stories;
        _storyObjects = stories.Values.ToArray();
        allStoriesLoaded = true;
    }

    public InkyStoryObject GetStoryObject(string storyName)
    {
        if (_storyObjectDict.ContainsKey(storyName))
        {
            return _storyObjectDict[storyName];
        }
        else
        {
            Debug.LogError($"InkyStoryLoader: Story '{storyName}' not found.");
            return null;
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Saves an Inky story object to the Resources/Inky/StoryObjects directory.
    /// </summary>
    /// <param name="textAsset">
    ///     The TextAsset containing the Ink story. Typically, this is a generated .json file.
    /// </param>
    /// <returns>
    ///     The <see cref="InkyStoryObject"/> that was saved. 
    /// </returns>
    public InkyStoryObject SaveInkyStoryObject(TextAsset textAsset)
    {
        string path = $"Assets/Resources/Inky/StoryObjects/{textAsset.name}.asset";
        InkyStoryObject inkyStory = AssetDatabase.LoadAssetAtPath<InkyStoryObject>(path);

        if (inkyStory == null)
        {
            inkyStory = ScriptableObject.CreateInstance<InkyStoryObject>();
            AssetDatabase.CreateAsset(inkyStory, path);
        }

        inkyStory.Initialize(textAsset);
        EditorUtility.SetDirty(inkyStory);
        AssetDatabase.SaveAssets();

        return inkyStory;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InkyStoryLoader))]
public class InkyStoryLoaderCustomEditor : Editor
{
    SerializedObject _serializedObject;
    InkyStoryLoader _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (InkyStoryLoader)target;
        _script.LoadAllStories();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
