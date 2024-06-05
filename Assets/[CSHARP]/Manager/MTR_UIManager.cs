using System.Collections;
using Darklight.UnityExt.Editor;
using Darklight.UnityExt.Inky;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.UXML;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Ink.Runtime;


#if UNITY_EDITOR
using UnityEditor;
#endif

// the InputSystemProvider throws an error if a UIDocument is destroyed.
// This is a workaround to prevent the error from being thrown ( https://forum.unity.com/threads/case-1426900-error-destroy-may-not-be-called-from-edit-mode-is-shown-when-stopping-play.1279895/#post-8454926 )

/// <summary>
/// The UIManager is a singleton class that handles the creation and management of
/// UIDocuments in the game.
/// </summary>
public class MTR_UIManager : MonoBehaviourSingleton<MTR_UIManager>
{
    #region ======= [[ STATIC METHODS ]] ============================================= >>>>

    /// <summary>
    /// Creates a new UIDocumentObject from a given preset.
    /// </summary>
    /// <param name="preset"></param>
    /// <returns></returns>
    public static TDocument CreateUIDocumentObject<TDocument>(UXML_UIDocumentPreset preset)
        where TDocument : UXML_UIDocumentObject
    {
        GameObject go = new GameObject($"UXML_UIDocument : {preset.name}");
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
    public static void SetWorldToScreenPoint(
        VisualElement element,
        Vector3 worldPosition,
        bool center = false
    )
    {
        Camera cam = Camera.main;
        if (cam == null)
            throw new System.Exception("No main camera found.");

        // Convert world position to screen position
        Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
        screenPosition.y = cam.pixelHeight - screenPosition.y; // UI Toolkit uses top-left origin
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

    #endregion <<< ======= [[ STATIC METHODS ]] =======

    // ----- [[ PRIVATE FIELDS ]] ------------------------------------>
    [SerializeField, ShowOnly]
    Vector2 _screenSize;

    [SerializeField, ShowOnly]
    float _screenAspectRatio;

    // ----- [[ UI CONTROLLERS ]] ------------------------------------>
    [HorizontalLine(color: EColor.Gray)]
    [Header("Main Menu Controller")]
    [SerializeField]
    UXML_UIDocumentPreset _mainMenuPreset;

    [SerializeField]
    SceneObject _mainMenuScene;
    private MainMenuController _mainMenuController;

    [Header("Game UI Controller")]
    private GameUIController _gameUI;

    [SerializeField]
    UXML_UIDocumentPreset _gameUIPreset;
    public GameUIController gameUIController
    {
        get
        {
            // Find the GameUIController if it exists
            if (_gameUI != null)
                return _gameUI;
            _gameUI = FindAnyObjectByType<GameUIController>();
            if (_gameUI != null)
                return _gameUI;

            // Create a new GameUIController if it doesn't
            _gameUI = CreateUIDocumentObject<GameUIController>(_gameUIPreset);
            _gameUI.transform.SetParent(transform);
            return _gameUI;
        }
    }

    [Header("Synthesis Manager")]
    private SynthesisManager _synthesisManager;

    [SerializeField]
    UXML_UIDocumentPreset _synthesisUIPreset;
    public SynthesisManager synthesisManager
    {
        get
        {
            // Find the SynthesisManager if it exists
            if (_synthesisManager != null)
                return _synthesisManager;
            _synthesisManager = FindAnyObjectByType<SynthesisManager>();
            if (_synthesisManager != null)
                return _synthesisManager;

            // Create a new SynthesisManager if it doesn't
            _synthesisManager = CreateUIDocumentObject<SynthesisManager>(_synthesisUIPreset);
            return _synthesisManager;
        }
    }

    [Header("Speech Bubble")]
    [SerializeField]
    UXML_UIDocumentPreset _speechBubblePreset;

    [ShowOnly]
    public UXML_RenderTextureObject speechBubbleObject;

    [MinMaxSlider(24, 128)]
    public Vector2Int speechBubbleFontSizeRange = new Vector2Int(64, 128);

    [SerializeField]
    Sprite LTick_SpeechBubble;

    [SerializeField]
    Sprite RTick_SpeechBubble;

    public void CreateNewSpeechBubble(string text)
    {
        if (speechBubbleObject != null)
        {
            DestroySpeechBubble();
        }

        // Return if the application is not playing
        if (!Application.isPlaying) { return; }

        // Create a new Bubble
        speechBubbleObject = CreateUXMLRenderTextureObject(_speechBubblePreset);
        (Vector3, Vector2Int) bubbleData = GetSpeakerSpeechBubblePositionAndDirection();
        speechBubbleObject.transform.position = bubbleData.Item1;
        Vector2Int bubbleDirection = bubbleData.Item2;
        Sprite bubbleSprite = bubbleDirection == Vector2Int.left ? RTick_SpeechBubble : LTick_SpeechBubble;
        Debug.Log($"{Prefix} :: Created Speech Bubble || direction {bubbleDirection}");

        SpeechBubble speechBubble = speechBubbleObject.ElementQuery<SpeechBubble>();
        speechBubble.fontSizeRange = speechBubbleFontSizeRange;
        speechBubble.fontSize = speechBubble.GetDynamicFontSize();
        speechBubble.SetBackgroundSprite(bubbleSprite);

        StartCoroutine(SpeechBubbleRollingTextRoutine(text, 0.025f));
    }

    IEnumerator SpeechBubbleRollingTextRoutine(string fullText, float interval)
    {
        SpeechBubble speechBubble = speechBubbleObject.ElementQuery<SpeechBubble>();
        speechBubble.Initialize(fullText);

        while (true)
        {
            for (int i = 0; i < speechBubble.fullText.Length; i++)
            {
                speechBubble.RollingTextStep();
                yield return new WaitForSeconds(interval);
            }
            yield return null;
        }
    }

    public void DestroySpeechBubble()
    {
        if (speechBubbleObject != null)
        {
            if (Application.isPlaying)
                Destroy(speechBubbleObject.gameObject);
            else
                DestroyImmediate(speechBubbleObject.gameObject);
        }
        speechBubbleObject = null;
    }

    (Vector3, Vector2Int) GetSpeakerSpeechBubblePositionAndDirection()
    {
        string currentSpeaker = InkyStoryManager.Instance.CurrentSpeaker;
        Vector3 bubblePosition = Vector3.zero;
        Vector2Int bubbleDirection = Vector2Int.zero;

        // Set the Camera Target to the Player
        if (currentSpeaker.Contains("Lupe"))
        {
            PlayerInteractor playerInteractor = FindFirstObjectByType<PlayerInteractor>();
            if (playerInteractor == null)
            {
                Debug.LogError($"{Prefix} Could not find PlayerInteractor");
                return (bubblePosition, bubbleDirection);
            }

            bubblePosition = playerInteractor.GetBestOverlapGridData().worldPosition;
            if (bubblePosition.x <= playerInteractor.transform.position.x)
            {
                bubbleDirection = Vector2Int.left;
            }
            else if (bubblePosition.x > playerInteractor.transform.position.x)
            {
                bubbleDirection = Vector2Int.right;
            }
            return (bubblePosition, bubbleDirection);
        }

        // Set the Camera Target to a NPC
        NPC_Interactable[] interactables = FindObjectsByType<NPC_Interactable>(
            FindObjectsSortMode.None
        );
        foreach (NPC_Interactable interactable in interactables)
        {
            if (interactable.speakerTag.Contains(currentSpeaker))
            {
                bubblePosition = interactable.GetBestOverlapGridData().worldPosition;
                bubblePosition.z = interactable.transform.position.z; // Set the Z position to the NPC's Z position
                if (bubblePosition.x <= interactable.transform.position.x)
                {
                    bubbleDirection = Vector2Int.left;
                }
                else if (bubblePosition.x > interactable.transform.position.x)
                {
                    bubbleDirection = Vector2Int.right;
                }
                return (bubblePosition, bubbleDirection);
            }
        }

        Debug.LogError($"{Prefix} Could not find Speaker: {currentSpeaker}");
        return (bubblePosition, bubbleDirection);
    }




    #region ------ [[ INTERACT ICON ]] ------------------------
    [Header("Interact Icon")]
    [SerializeField]
    UXML_UIDocumentPreset _interactIconPreset;

    [ShowOnly]
    public UXML_RenderTextureObject interactIcon;

    public void ShowInteractIcon(Vector3 worldPosition, float scale = 1)
    {
        if (interactIcon == null)
            interactIcon = CreateUXMLRenderTextureObject(_interactIconPreset);
        interactIcon.transform.position = worldPosition;
        interactIcon.SetLocalScale(scale);

        //VisualElement icon = interactIcon.ElementQuery<VisualElement>();
        //ScaleElementToScreenSize(icon, scale);
    }

    public void RemoveInteractIcon()
    {
        if (interactIcon == null)
            return;
        if (Application.isPlaying)
            interactIcon.Destroy();
        else
            DestroyImmediate(interactIcon.gameObject);
        interactIcon = null;
    }
    #endregion

    // ----- [[ PUBLIC FIELDS ]] ------------------------------------>
    [Header("Render Texture")]
    public Material UXML_RenderTextureMaterial;
    public RenderTexture UXML_RenderTexture;

    // ----- [[ UNITY METHODS ]] ------------------------------------>
    public override void Initialize() { }

    // ----- [[ PRIVATE METHODS ]] ------------------------------------>
    /// <summary>
    /// Creates a new GameObject with a UXML_RenderTextureObject component.
    /// This allows for the rendering of a UXML Element to a In-World RenderTexture.
    /// </summary>
    /// <param name="preset">
    ///     The UXML_UIDocumentPreset to use for the RenderTextureObject.
    /// </param>
    /// <returns></returns>
    ///
    UXML_RenderTextureObject CreateUXMLRenderTextureObject(UXML_UIDocumentPreset preset)
    {
        string name = $"UXMLRenderTexture : unknown";
        if (preset != null)
            name = $"UXMLRenderTexture : {preset.name}";
        GameObject go = new GameObject(name);

        //go.hideFlags = HideFlags.NotEditable;
        UXML_RenderTextureObject renderTextureObject = go.AddComponent<UXML_RenderTextureObject>();
        renderTextureObject.Initialize(
            preset,
            null,
            UXML_RenderTextureMaterial,
            UXML_RenderTexture
        );
        renderTextureObject.TextureUpdate();
        return renderTextureObject;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MTR_UIManager))]
    public class UIManagerCustomEditor : Editor
    {
        SerializedObject _serializedObject;
        MTR_UIManager _script;
        SerializedProperty _screenSizeProperty;
        SerializedProperty _screenAspectRatioProperty;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _script = (MTR_UIManager)target;

            _screenSizeProperty = _serializedObject.FindProperty("_screenSize");
            _screenAspectRatioProperty = _serializedObject.FindProperty("_screenAspectRatio");
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.Update();

            // Set the Screen Size Property value
            _screenSizeProperty.vector2Value = ScreenInfoUtility.ScreenSize;
            _screenAspectRatioProperty.floatValue = ScreenInfoUtility.ScreenAspectRatio;

            base.OnInspectorGUI();

            _serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
