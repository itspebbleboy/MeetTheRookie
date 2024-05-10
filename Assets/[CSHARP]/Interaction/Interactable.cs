using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.Game.Grid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Interactable : MonoBehaviour, IInteract
{
    private SpriteRenderer _spriteRenderer => GetComponentInChildren<SpriteRenderer>();
    private InkyStoryLoader _storyLoader => InkyStoryManager.Instance.storyLoader;
    private InkyKnotIterator _knotIterator;

    // ------------------- [[ SERIALIZED FIELDS ]] -------------------

    [Header("Interaction Settings")]
    [SerializeField] protected InkyStoryObject _storyParent;

    [DropdownAttribute("_storyParent.knotAndStitchKeys")]
    [SerializeField] protected string _interactionKey;

    [Header("Components")]
    [SerializeField] Sprite _sprite;

    [SerializeField] FMODUnity.EventReference _interactSound;

    [Header("State Flags")]
    [ShowOnly, SerializeField] bool _isTarget;
    [ShowOnly, SerializeField] bool _isActive;
    [ShowOnly, SerializeField] bool _isComplete;

    [Header("Colors")]
    [SerializeField] Color _defaultTint = Color.white;
    [SerializeField] Color _interactionTint = Color.yellow;

    // ------------------- [[ PUBLIC ACCESSORS ]] -------------------
    public InkyStoryObject storyParent { get => _storyParent; private set => _storyParent = value; }
    public string interactionKey { get => _interactionKey; private set => _interactionKey = value; }
    public InkyKnotIterator knotIterator { get => _knotIterator; private set => _knotIterator = value; }
    public bool isTarget { get => _isTarget; set => _isTarget = value; }
    public bool isActive { get => _isActive; set => _isActive = value; }
    public bool isComplete { get => _isComplete; set => _isComplete = value; }
    public event IInteract.OnFirstInteract OnFirstInteraction;
    public event IInteract.OnInteract OnInteraction;
    public event IInteract.OnComplete OnCompleted;

    // ====== [[ MONOBEHAVIOUR METHODS ]] =========================
    public void Awake()
    {
        Initialize();
    }

    // ====== [[ INITIALIZATION ]] ================================
    public virtual void Initialize()
    {
        _spriteRenderer.sprite = _sprite;
        _spriteRenderer.color = _defaultTint;

        if (_storyParent == null)
        {
            Debug.LogError("Story Parent is null. Please assign a valid InkyStory object.");
            return;
        }

        if (_interactionKey == null || _interactionKey == "")
        {
            Debug.LogError("Interaction Key is null. Please assign a valid knot or stitch key.");
            return;
        }

        _knotIterator = new InkyKnotIterator(_storyParent.CreateStory(), _interactionKey);
    }

    public virtual void Reset()
    {
        isComplete = false;
        _spriteRenderer.color = _interactionTint;
    }

    // ====== [[ TARGETING ]] ======================================
    public virtual void TargetSet()
    {
        isTarget = true;
        UIManager.Instance.ShowInteractionPromptInWorld(transform.position);
    }

    public virtual void TargetClear()
    {
        isTarget = false;
        UIManager.Instance.HideInteractPrompt();
    }

    public virtual void Interact()
    {
        // >> TEMPORARY COLOR CHANGE
        StartCoroutine(ColorChangeRoutine(_interactionTint, 0.25f));

        // >> CONTINUE KNOT
        if (knotIterator == null)
            knotIterator = new InkyKnotIterator(_storyParent.CreateStory(), _interactionKey);
        ContinueKnot();
    }

    public virtual void ContinueKnot()
    {
        if (!isActive)
        {
            OnFirstInteraction?.Invoke();
            FMODManager.Instance.PlayEvent(_interactSound);
        }

        isTarget = false;
        isActive = true;
        isComplete = false;

        knotIterator.ContinueKnot();

        if (knotIterator.CurrentState == InkyKnotIterator.State.END)
        {
            Complete();
            return;
        }

        OnInteraction?.Invoke(knotIterator.currentText);
    }

    public virtual void Complete()
    {
        isComplete = true;
        isActive = false;
        knotIterator = null;

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
public class InteractableCustomEditor : Editor
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

        base.OnInspectorGUI();


        GUILayout.Space(10);
        GUILayout.Label("Interactable Testing", EditorStyles.boldLabel);
        if (!_script.isTarget && GUILayout.Button("Target"))
        {
            _script.TargetSet();
        }

        if (_script.isTarget && GUILayout.Button("Clear Target"))
        {
            _script.TargetClear();
        }


        if (EditorGUI.EndChangeCheck())
        {
            _script.Awake();
            _serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif