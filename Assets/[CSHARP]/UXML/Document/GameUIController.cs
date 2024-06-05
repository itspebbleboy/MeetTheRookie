using System.Collections;

using Darklight.UnityExt.UXML;

using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Input;
using System.Linq;
using System.Collections.Generic;
using Ink.Runtime;
using Darklight.UnityExt.Inky;
using System;








#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This handles all of the Game UI elements like interactions and speech bubbles.
/// </summary>
public class GameUIController : UXML_UIDocumentObject
{
    const string INTERACT_PROMPT_TAG = "interact-icon";
    const string SPEECH_BUBBLE_TAG = "speech-bubble";

    SelectableVectorField<SelectableButton> selectableVectorField = new SelectableVectorField<SelectableButton>();
    private Dictionary<SelectableButton, Action> buttonHandlers = new Dictionary<SelectableButton, Action>();

    VisualElement _header;
    VisualElement _body;
    VisualElement _footer;
    GroupBox _choiceBox;
    bool lockSelection = false;


    public void Awake()
    {
        Initialize(preset);



        // Listen to the input manager
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
    }

    public void Start()
    {
        _body = ElementQuery<VisualElement>("body");
        _header = ElementQuery<VisualElement>("header");
        _footer = ElementQuery<VisualElement>("footer");
    }

    public void LoadChoices(List<Choice> choices)
    {
        _choiceBox = ElementQuery<GroupBox>("ChoiceBox");
        _choiceBox.Clear();
        selectableVectorField.Clear();
        buttonHandlers.Clear();

        foreach (Choice choice in choices)
        {
            SelectableButton button = new SelectableButton();
            button.text = choice.text;
            Action handler = () => SelectChoice(choice);
            button.OnClick += handler;
            _choiceBox.Add(button);
            buttonHandlers[button] = handler;
        }

        // Load the Selectable Elements
        selectableVectorField.Load(ElementQueryAll<SelectableButton>());
        selectableVectorField.Selectables.First().Select();
    }

    public void SelectChoice(Choice choice)
    {
        InkyStoryManager.Iterator.ChooseChoice(choice);

        // Remove OnClick event handlers for each button
        foreach (SelectableButton button in selectableVectorField.Selectables)
        {
            if (buttonHandlers.TryGetValue(button, out var handler))
            {
                button.OnClick -= handler;
            }
        }

        _choiceBox.Clear();
    }



    void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2(dir.x, -dir.y); // inverted y for screen space
        SelectableButton buttonInDirection = selectableVectorField.getFromDir(directionInScreenSpace);
        Select(buttonInDirection);
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



    void OnPrimaryInteractAction()
    {
        selectableVectorField.CurrentSelection?.Click();
    }

}
