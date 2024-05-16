using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Interactable : OverlapGrid2D, IInteract
{
    private SpriteRenderer _spriteRenderer => GetComponentInChildren<SpriteRenderer>();

    // private access to knots for dropdown
    private List<string> _sceneKnots
    {
        get
        {
            if (_storyObject == null) return new List<string>();
            return InkyStoryObject.GetAllKnots(_storyObject.Story);
        }
    }

    // private access to stitches for dropdown
    private List<string> _interactionStitches
    {
        get
        {
            if (_storyObject == null) return new List<string>();
            if (_sceneKnot == null || _sceneKnot == "") return new List<string>();
            return InkyStoryObject.GetAllStitchesInKnot(_storyObject.Story, _sceneKnot);
        }
    }

    // ------------------- [[ SERIALIZED FIELDS ]] -------------------

    //[HorizontalLine(color: EColor.Gray)]
    [Header("Interactable")]
    [SerializeField] Sprite _sprite;

    [Tooltip("The parent InkyStoryObject that this interactable belongs to. This is equivalent to a 'Level' of the game.")]
    [SerializeField] protected InkyStoryObject _storyObject;

    [DropdownAttribute("_sceneKnots")]
    public string _sceneKnot;

    [DropdownAttribute("_interactionStitches")]
    public string _interactionStitch;
    protected InkyStoryIterator _storyIterator;

    [Header("State Flags")]
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive;
    [ShowOnly, SerializeField] bool _isComplete;

    [Header("Colors")]
    [SerializeField] Color _defaultTint = Color.white;
    [SerializeField] Color _interactionTint = Color.yellow;

    // ------------------- [[ PUBLIC ACCESSORS ]] -------------------
    public InkyStoryObject storyObject { get => _storyObject; private set => _storyObject = value; }
    public string interactionKey { get => _interactionStitch; private set => _interactionStitch = value; }
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }
    public string currentText => _storyIterator.CurrentText;

    public event IInteract.OnFirstInteract OnFirstInteraction;
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // ------------------- [[ PUBLIC METHODS ]] ------------------- >>
    public override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public virtual void Initialize()
    {
        // Prioritize the initial sprite that is set in the sprite renderer
        // Its assumed that the sprtie renderer has a null sprite when the interactable is first created
        if (_spriteRenderer.sprite == null)
            _spriteRenderer.sprite = _sprite;
        else
            _sprite = _spriteRenderer.sprite;

        _spriteRenderer.color = _defaultTint;

        if (_storyObject == null)
        {
            Debug.LogError($"INTERACTABLE ( {name} ) >> Story Parent is null. Please assign a valid InkyStory object.", this);
            return;
        }
    }

    public virtual void Reset()
    {
        isTarget = false;
        isActive = false;
        isComplete = false;
        _spriteRenderer.color = _interactionTint;
    }

    // ====== [[ TARGETING ]] ======================================
    public virtual void TargetSet()
    {
        isTarget = true;
        OverlapGrid2D_Data targetData = GetBestData();
        UIManager.Instance.ShowInteractIcon(targetData.worldPosition, targetData.cellSize);
    }

    public virtual void TargetClear()
    {
        isTarget = false;
        UIManager.Instance.RemoveInteractIcon();
    }

    // ====== [[ INTERACTION ]] ======================================
    public virtual void Interact()
    {
        // First Interaction
        if (!isActive)
        {
            TargetClear();

            isActive = true;
            isComplete = false;

            _storyIterator = new InkyStoryIterator(storyObject, InkyStoryIterator.State.NULL);
            _storyIterator.GoToKnotOrStitch(_interactionStitch);

            // >> TEMPORARY COLOR CHANGE
            StartCoroutine(ColorChangeRoutine(_interactionTint, 0.25f));

            OnFirstInteraction?.Invoke();
            Debug.Log($"INTERACT :: {name} >> First Interaction");
            return;
        }

        // Last Interaction
        if (_storyIterator.CurrentState == InkyStoryIterator.State.END)
        {
            Complete();
            Debug.Log($"INTERACT :: {name} >> Complete");
            return;
        }

        // Continue the interaction
        _storyIterator.ContinueKnot();
        OnInteraction?.Invoke(_storyIterator.CurrentText);
        Debug.Log($"INTERACT :: {name} >> Continue Interaction");
    }

    public virtual void Complete()
    {
        isActive = false;
        isTarget = false;
        isComplete = true;
        _storyIterator = null;

        OnCompleted?.Invoke();
    }

    public virtual void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }

    private IEnumerator ColorChangeRoutine(Color newColor, float duration)
    {
        if (_spriteRenderer == null) yield break;
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = newColor;

        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Interactable), true)]
public class InteractableCustomEditor : OverlapGrid2DEditor
{
    SerializedObject _serializedObject;
    Interactable _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (Interactable)target;
        _script.Awake();
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();


        GUILayout.Space(10);
        GUILayout.Label("Interactable Testing", EditorStyles.boldLabel);

        if (!_script.isTarget)
        {
            if (GUILayout.Button("Set Target"))
                _script.TargetSet();

            if (_script.isActive)
            {
                if (GUILayout.Button("Continue Interaction"))
                    _script.Interact();

                if (GUILayout.Button("Complete Interaction"))
                    _script.Complete();

                if (GUILayout.Button("Reset Interaction"))
                    _script.Reset();
            }
        }
        else
        {
            if (GUILayout.Button("Clear Target"))
                _script.TargetClear();

            if (!_script.isActive)
            {
                if (GUILayout.Button("First Interact"))
                    _script.Interact();
            }
        }


        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _script.Awake();
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif