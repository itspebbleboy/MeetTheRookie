using UnityEngine;
using Darklight.UnityExt.Input;
using System.Linq;
using Darklight.UnityExt.UXML;
using Darklight.UnityExt.Utility;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This is a Basic Representation of a Menu Controller that handles the selection of buttons.
/// </summary>
public class BasicMenuController : UXML_UIDocumentObject
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
        selectablesCount = selectableVectorField.Selectables.Count();

        // Listen to the input manager
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
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



