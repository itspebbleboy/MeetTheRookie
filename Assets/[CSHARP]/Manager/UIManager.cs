using Darklight.UXML;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;





#if UNITY_EDITOR
using UnityEditor;
#endif

// the InputSystemProvider throws an error if a UIDocument is destroyed.
// This is a workaround to prevent the error from being thrown ( https://forum.unity.com/threads/case-1426900-error-destroy-may-not-be-called-from-edit-mode-is-shown-when-stopping-play.1279895/#post-8454926 )

/// <summary>
/// The UIManager is a singleton class that handles the creation and management of 
/// UIDocuments in the game.
/// </summary>
[ExecuteAlways]
public class UIManager : MonoBehaviourSingleton<UIManager>
{
    #region ======= [[ STATIC METHODS ]] ============================================= >>>>

    /// <summary>
    /// Creates a new UIDocumentObject from a given preset.
    /// </summary>
    /// <param name="preset"></param>
    /// <returns></returns>
    [EasyButtons.Button]
    public static TDocument CreateUIDocumentObject<TDocument>(UXML_UIDocumentPreset preset) where TDocument : UXML_UIDocumentObject
    {
        GameObject go = new GameObject($"UXMLUIDocument : {preset.name}");
        //go.hideFlags = HideFlags.NotEditable;
        TDocument uiDocument = go.AddComponent<TDocument>();
        uiDocument.Initialize(preset);
        return uiDocument;
    }

    /// <summary>
    /// Sets the position of a UI Toolkit element to correspond to a world position.
    /// Optionally centers the element on the screen position.
    /// </summary>
    /// <param name="element">The UI Toolkit element to position.</param>
    /// <param name="worldPosition">The world position to map to screen space.</param>
    /// <param name="center">Optional parameter to center the element at the screen position (default false).</param>
    public static void SetWorldToScreenPoint(VisualElement element, Vector3 worldPosition, bool center = false)
    {
        Camera cam = Camera.main;
        if (cam == null) throw new System.Exception("No main camera found.");

        // Convert world position to screen position
        Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
        screenPosition.y = cam.pixelHeight - screenPosition.y;  // UI Toolkit uses top-left origin
        screenPosition.z = 0;

        if (center)
        {
            // Adjust position to center the element
            screenPosition.x -= element.resolvedStyle.width / 2;
            screenPosition.y -= element.resolvedStyle.height / 2;
        }

        // Set positions using left and top in style
        element.style.left = screenPosition.x;
        element.style.top = screenPosition.y;

        Debug.Log($"Set Element Position: {screenPosition}");
    }


    // ---------------- [[ SCREEN SCALING ]] ---------------->
    private static int lastScreenWidth;
    private static int lastScreenHeight;

    /// <summary>
    /// Adjusts the size of the given VisualElement based on the current screen size.
    /// </summary>
    /// <param name="element">The VisualElement to adjust.</param>
    public static void ScaleElementToScreenSize(VisualElement element, float scale = 1f)
    {
        float maxDimension = Mathf.Max(lastScreenWidth, lastScreenHeight);

        // Adjust the size of the element based on the smaller dimension of the screen
        float newSize = maxDimension * scale;
        element.style.width = new Length(newSize, LengthUnit.Pixel);
        element.style.height = new Length(newSize, LengthUnit.Pixel);

        Debug.Log($"Screen Size: {lastScreenWidth} x {lastScreenHeight}, New Element Size: {newSize}");
    }

#if UNITY_EDITOR
    public static void CleanUpDocuments()
    {
        int count = 0;
        UIDocument[] allDocuments = Resources.FindObjectsOfTypeAll<UIDocument>();
        foreach (UIDocument doc in allDocuments)
        {
            DestroyImmediate(doc.gameObject);
            count++;
        }

        UXML_UIDocumentObject[] allObjects = Resources.FindObjectsOfTypeAll<UXML_UIDocumentObject>();
        foreach (UXML_UIDocumentObject obj in allObjects)
        {
            DestroyImmediate(obj.gameObject);
            count++;
        }

        UXML_RenderTextureObject[] allRenderTextures = Resources.FindObjectsOfTypeAll<UXML_RenderTextureObject>();
        foreach (UXML_RenderTextureObject obj in allRenderTextures)
        {
            DestroyImmediate(obj.gameObject);
            count++;
        }

        Debug.Log($"{count} UIDocuments have been destroyed.");
    }
#endif
    #endregion <<< ======= [[ STATIC METHODS ]] =======

    // ----- [[ UI CONTROLLERS ]] ------------------------------------>
    [Header("Main Menu Controller")]
    private MainMenuController _mainMenu;
    [SerializeField] UXML_UIDocumentPreset _mainMenuPreset;
    [SerializeField] SceneAsset _mainMenuScene;


    [Header("Game UI Controller")]
    private GameUIController _gameUI;
    [SerializeField] UXML_UIDocumentPreset _gameUIPreset;
    public GameUIController gameUIController
    {
        get
        {
            // Find the GameUIController if it exists
            if (_gameUI != null) return _gameUI;
            _gameUI = FindAnyObjectByType<GameUIController>();
            if (_gameUI != null) return _gameUI;

            // Create a new GameUIController if it doesn't
            _gameUI = CreateUIDocumentObject<GameUIController>(_gameUIPreset);
            return _gameUI;
        }
    }

    [Header("Synthesis Manager")]
    private SynthesisManager _synthesisManager;
    [SerializeField] UXML_UIDocumentPreset _synthesisUIPreset;
    public SynthesisManager synthesisManager
    {
        get
        {
            // Find the SynthesisManager if it exists
            if (_synthesisManager != null) return _synthesisManager;
            _synthesisManager = FindAnyObjectByType<SynthesisManager>();
            if (_synthesisManager != null) return _synthesisManager;

            // Create a new SynthesisManager if it doesn't
            _synthesisManager = CreateUIDocumentObject<SynthesisManager>(_synthesisUIPreset);
            return _synthesisManager;
        }
    }

    [Header("Speech Bubble")]
    [SerializeField] UXML_UIDocumentPreset _speechBubblePreset;
    [ShowOnly] public UXML_RenderTextureObject speechBubble;


    [Header("Interact Icon")]
    [SerializeField] UXML_UIDocumentPreset _interactIconPreset;
    [ShowOnly] public UXML_RenderTextureObject interactIcon;


    // ----- [[ PUBLIC FIELDS ]] ------------------------------------>
    [Header("Render Texture")]
    public Material UXML_RenderTextureMaterial;
    public RenderTexture UXML_RenderTexture;

    // ----- [[ UNITY METHODS ]] ------------------------------------>
    public override void Awake()
    {
        base.Awake(); // << Update the Singleton instance

        //CleanUpDocuments(); // << Clean up hidden documents
        /*
        if (SceneManager.GetActiveScene().name == _mainMenuScene.name)
        {
            _mainMenu = CreateUIDocumentObject<MainMenuController>(_mainMenuPreset);
            _mainMenu.Initialize(_mainMenuPreset);
        }
        */
        //gameUIController?.Initialize(_gameUIPreset);
    }

    void Update()
    {
        UpdateScreenSize();
    }

    // ----- [[ PUBLIC METHODS ]] ------------------------------------>
    public void ShowInteractIcon(Vector3 worldPosition, float scale = 1)
    {
        if (interactIcon == null)
            interactIcon = CreateRenderTextureObject(_interactIconPreset);
        interactIcon.transform.position = worldPosition;
        interactIcon.SetLocalScale(scale);

        //VisualElement icon = interactIcon.ElementQuery<VisualElement>();
        //ScaleElementToScreenSize(icon, scale);
    }

    public void RemoveInteractIcon()
    {
        if (interactIcon == null) return;
        if (Application.isPlaying)
            interactIcon.Destroy();
        else
            DestroyImmediate(interactIcon.gameObject);
        interactIcon = null;
    }

    public void CreateSpeechBubble(Vector3 worldPosition, string text, float scale = 1f)
    {
        if (speechBubble == null)
            speechBubble = CreateRenderTextureObject(_speechBubblePreset);
        speechBubble.transform.position = worldPosition;
        speechBubble.SetLocalScale(scale);

        // Set the text of the speech bubble
        speechBubble.ElementQuery<SpeechBubble>().text = text;

        Debug.Log($"Created Speech Bubble at {worldPosition} ||| {text}");
    }

    public void DestroySpeechBubble()
    {
        if (speechBubble != null)
        {
            if (Application.isPlaying)
                Destroy(speechBubble.gameObject);
            else
                DestroyImmediate(speechBubble.gameObject);
        }
        speechBubble = null;

    }



    // ----- [[ PRIVATE METHODS ]] ------------------------------------>
    /// <summary>
    /// Creates a new GameObject with a UXML_RenderTextureObject component.
    /// This allows for the rendering of a UXML Element to a In-World RenderTexture.
    /// </summary>
    /// <param name="preset">
    ///     The UXML_UIDocumentPreset to use for the RenderTextureObject.
    /// </param>
    /// <returns></returns>
    UXML_RenderTextureObject CreateRenderTextureObject(UXML_UIDocumentPreset preset)
    {
        string name = $"UXMLRenderTexture : unknown";
        if (preset != null) name = $"UXMLRenderTexture : {preset.name}";
        GameObject go = new GameObject(name);

        //go.hideFlags = HideFlags.NotEditable;
        UXML_RenderTextureObject renderTextureObject = go.AddComponent<UXML_RenderTextureObject>();
        renderTextureObject.Initialize(preset, null, UXML_RenderTextureMaterial, UXML_RenderTexture);
        renderTextureObject.TextureUpdate();
        return renderTextureObject;
    }

    void UpdateScreenSize()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            Debug.Log($"Screen Size Updated: {lastScreenWidth} x {lastScreenHeight}");
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIManager))]
    public class UIManagerCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        UIManager _script;
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (UIManager)target;
            _script.Awake();
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (GUILayout.Button("Clean Up Documents"))
            {
                UIManager.CleanUpDocuments();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif

}
