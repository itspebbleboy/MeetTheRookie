using Darklight.UnityExt.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UXML;
using Darklight.Game.Selectable;
using Darklight.Selectable;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handle the UI and Synthesis objects.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class SynthesisManager : UXML_UIDocumentObject
{
    protected Dictionary<string, VisualElement> synthesisItems = new Dictionary<string, VisualElement>();
    public SelectableVectorField<VisualElement> itemsSelection = new SelectableVectorField<VisualElement>();

    /// <summary>
    /// Our group for showing the objects visually.
    /// </summary>
    VisualElement objects;
    VisualElement synthesizeButton;

    public void Prepare() {
        InkyStoryManager.Instance.BindExternalFunction("playerAddItem", AddItem);
        InkyStoryManager.Instance.BindExternalFunction("playerRemoveItem", RemoveItem);
        InkyStoryManager.Instance.BindExternalFunction("playerHasItem", HasItem);

        document.rootVisualElement.visible = false;
        objects = document.rootVisualElement.Q("objects");

        synthesizeButton = ElementQuery<VisualElement>("synthesizeButton");
        itemsSelection.Add(synthesizeButton);
    }

    bool synthesisActive = false;
    void Start()
    {
        Invoke("Init", 0.1f);
    }
    ///oijqwdoijqwodijqwd
    void Init() {
        if (UniversalInputManager.Instance == null) { Debug.LogWarning("UniversalInputManager is not initialized"); return; }

        UniversalInputManager.OnMoveInputStarted += SelectMove;
        UniversalInputManager.OnPrimaryInteract += Select;
    }

    public void Show(bool visible) {
        synthesisActive = visible;
        document.rootVisualElement.visible = synthesisActive;
    }

    void SelectMove(Vector2 move)
    {
        move.y = -move.y;
        if (itemsSelection.CurrentSelection != null)
        {
            itemsSelection.CurrentSelection.RemoveFromClassList("highlight");
        }
        var selected = itemsSelection.getFromDir(move);
        if (selected != null) {
            selected.AddToClassList("highlight");
        }
    }

    HashSet<SynthesisObject> toSynthesize = new HashSet<SynthesisObject>();
    void Select()
    {
        if (itemsSelection.CurrentSelection != null)
        {
            var s = itemsSelection.CurrentSelection;
            if (s == synthesizeButton) {
                Synthesize();
                return;
            }

            if (s.ClassListContains("selected")) {
                s.RemoveFromClassList("selected");
                toSynthesize.Remove((SynthesisObject)s);
            } else if (toSynthesize.Count < 3) { // Don't allow us to select more than three.
                s.AddToClassList("selected");
                toSynthesize.Add((SynthesisObject)s);
            }
        }
    }

    void Synthesize() {
        List<string> args = new List<string>();

        foreach (var item in toSynthesize) {
            item.RemoveFromClassList("selected");
            args.Add(item.name);
        }
        toSynthesize.Clear();

        args = args.OrderBy(s => s).ToList();

        if (args.Count == 2) {
            args.Add("");
        }

        InkyStoryManager.Instance.RunExternalFunction("synthesize", args.ToArray());
    }

    [Obsolete("Synthesis is handled by Synthesize instead.")]
    public object CombineItems(object[] args) {
        if (args.Length != 2) {
            Debug.LogError("Could not get 2 items to combine from " + args);
            return null;
        }
        string a = (string)args[0];
        string b = (string)args[1];
        // Sort our combination items so we don't have to worry about multiple cases in our combine function:
        List<string> sortArr = new List<string> { a, b };
        sortArr.Sort();
        sortArr.Reverse();
        var final = sortArr.ToArray<object>();
        object newItem = InkyStoryManager.Instance.RunExternalFunction("combine", final);
        if (newItem.GetType() == typeof(string)) {

            RemoveItem(new[] { a });
            RemoveItem(new[] { b });
            return AddItem(new[] { newItem });
        } else {
            return newItem;
        }
    }

    public object AddItem(object[] args) {
        if (args.Length < 2) {
            Debug.LogError("Invalid number of args for AddItem: " + args.Length + " minimum of 2 needed.");
            return null;
        }
        string type = (string)args[0];
        VisualTreeAsset asset = (VisualTreeAsset)Resources.Load("Synthesis/" + type);
        var newObj = asset.Instantiate();
        
        newObj.name = (string)args[1];
        newObj.AddToClassList("synthesis-object");

        foreach (var child in newObj.Children()) {
            if (child.dataSource != null && child.dataSource is SynthesisBinding b) {
                var source = (SynthesisBinding)b.Clone();
                if (args.Length == 3) {
                    source.setValue((string)args[2]);
                }
                child.dataSource = source;
            }
        }
        objects.Add(newObj);
        itemsSelection.Add(newObj);
        return synthesisItems.TryAdd(name, newObj);
    }

    public object RemoveItem(object[] args) {
        if ((bool)HasItem(args)) {
            var name = (string)args[0];
            synthesisItems[name].RemoveFromHierarchy();
            itemsSelection.Remove(synthesisItems[name]);
            return synthesisItems.Remove(name);
        }
        return false;
    }

    public object HasItem(object[] args) {
        return synthesisItems.ContainsKey((string)args[0]);
    }

    [Obsolete("Dragging should not be used for synthesis items.")]
    public SynthesisObject OverlappingObject(VisualElement synthesisObj) {
        var rect = synthesisObj.worldBound;
        foreach (var obj in synthesisItems) {
            if (obj.Value != synthesisObj && obj.Value.worldBound.Overlaps(rect, true)) {
                //return obj.Value;
            }
        }
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SynthesisManager))]
public class SynthesisManagerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    SynthesisManager _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (SynthesisManager)target;
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
