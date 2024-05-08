using UnityEngine;
using System.Collections.Generic;
using Darklight.UnityExt.Input;
using System.Linq;
using Darklight.Game.Selectable;
using Darklight.UXML;
using Darklight.Selectable;
using System;





#if UNITY_EDITOR
using UnityEditor;
#endif


public class MainMenuController : UXML_UIDocumentObject
{
    SelectableVectorField<SelectableButton> selectableVectorField = new SelectableVectorField<SelectableButton>();
    [SerializeField] int selectablesCount = 0;
    bool lockSelection = false;

    public void Awake()
    {
        Initialize(preset);

        // Load the Selectable Elements
        selectableVectorField.Load(ElementQueryAll<SelectableButton>());
        selectableVectorField.Selectables.First().Select();

        // Listen to the input manager
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;

        SelectableButton quitButton = ElementQuery<SelectableButton>("quit-button");
        quitButton.OnClick += Quit;
    }

    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        SelectableButton buttonInDirection = selectableVectorField.getFromDir(directionInScreenSpace);
        Select(buttonInDirection);
    }

    void OnPrimaryInteractAction()
    {
        selectableVectorField.CurrentSelection?.Click();
    }


    private void OnDestroy()
    {
        UniversalInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
    }


    void Select(SelectableButton selectedButton)
    {
        if (selectedButton == null || lockSelection) return;

        SelectableButton previousButton = selectableVectorField.PreviousSelection;
        if (selectedButton != previousButton)
        {
            previousButton?.Deselect();
            selectedButton.Select();
            lockSelection = true;
            Invoke(nameof(UnlockSelection), 0.1f);
        }
    }

    void UnlockSelection()
    {
        lockSelection = false;
    }


    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MainMenuController))]
public class MainMenuControllerCustomEditor : Editor
{
    SerializedObject _serializedObject;
    MainMenuController _script;
    private void OnEnable()
    {
        _serializedObject = new SerializedObject(target);
        _script = (MainMenuController)target;
        _script.Awake();
    }

    private void OnDisable()
    {
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



