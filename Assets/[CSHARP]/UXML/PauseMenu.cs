using System;
using System.Collections.Generic;
using Darklight.UnityExt.Input;
using FMODUnity;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    private UIDocument flicker;
    private VisualElement MenuBase;
    private VisualElement SoundsMenu;
    private VisualElement ControlsMenu;
    private VisualElement GeneralMenu;
    private VisualElement CurrentMenu;
    private List<VisualElement> Menus = new List<VisualElement>();
    private bool menuselected;
    private int menunav;
    private List<VisualElement> Interactors = new List<VisualElement>();
    private VisualElement CurrentElement;
    private int optionsnav;
    // Start is called before the first frame update
    void Start()
    {  
        flicker = GetComponent<UIDocument>();
        flicker.enabled = false;
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
        UniversalInputManager.OnSecondaryInteract += OnSecondaryInteractAction;
    }
    void OpenPauseMenu()
    {
        flicker.enabled = true;
        Time.timeScale = 0;
        MenuBase = flicker.rootVisualElement.Q<VisualElement>("Base");
        SoundsMenu = MenuBase.Q<VisualElement>("Sounds");
        ControlsMenu = MenuBase.Q<VisualElement>("Controls");
        GeneralMenu = MenuBase.Q<VisualElement>("GeneralSettings");
        Menus.Add(GeneralMenu);
        Menus.Add(ControlsMenu);
        Menus.Add(SoundsMenu);
        menunav = 0;
        menuselected = false;
        CurrentMenu = Menus[menunav].Q<Label>("TabTitle");
        CurrentMenu.AddToClassList("Selected");
        SliderRemoveDefaultControls();
    }
    void ClosePauseMenu()
    {
        if (menuselected == true)
        {
            if (CurrentMenu == SoundsMenu)
            {
                Debug.Log("worky");
                CurrentElement.Q<Label>().RemoveFromClassList("Selected");
            }
            if (CurrentMenu == GeneralMenu)
            {
                CurrentElement.RemoveFromClassList("Selected");
            }
        }
        optionsnav = 0;
        Interactors.Clear();
        Menus.Clear();
        CurrentMenu.RemoveFromClassList("Selected");
        Time.timeScale = 1;
        flicker.enabled = false;
    }

    private void OnPrimaryInteractAction()
    {
        if (menuselected == true)
        {
            if (CurrentMenu.parent == GeneralMenu)
            {
                ButtonActivity(CurrentElement as Button);
            }
        }
        if (menuselected == false)
        {
            menuselected = true;
            CurrentMenu.RemoveFromClassList("Selected");
            if (CurrentMenu.parent == SoundsMenu)
            {
                GroupBox sliders = SoundsMenu.Q<GroupBox>();
                foreach (SliderInt slider in sliders.Children())
                {
                    Interactors.Add(slider);
                }
                CurrentElement = Interactors[optionsnav];
                CurrentElement.Q<Label>().AddToClassList("Selected");
            }
            if (CurrentMenu.parent == GeneralMenu)
            {
                GroupBox options = GeneralMenu.Q<GroupBox>();
                foreach (Button option in options.Children())
                {
                    Interactors.Add(option);
                }
                CurrentElement = Interactors[optionsnav];
                CurrentElement.AddToClassList("Selected");
            }
        }
    }
    private void OnSecondaryInteractAction()
    {
        if (menuselected == true)
        {
            Debug.Log("yomama");
            if (CurrentMenu.parent == GeneralMenu)
            {
                CurrentElement.RemoveFromClassList("Selected");
                Debug.Log("yomam2a");
            }
            if (CurrentMenu.parent == SoundsMenu)
            {
                CurrentElement.Q<Label>().RemoveFromClassList("Selected");
            }
            Interactors.Clear();
            menuselected = false;
            CurrentMenu = Menus[menunav].Q<Label>("TabTitle");
            CurrentMenu.AddToClassList("Selected");
            optionsnav = 0;
        }
    }

    private void OnMoveInputStartAction(Vector2 moveInput)
    {
        CurrentMenu = Menus[menunav].Q<Label>("TabTitle");
        Vector2 direction = new Vector2(moveInput.x, moveInput.y);
        if (menuselected == false)
        {
            if (direction.x < 0 && menunav > 0)
            {
                MenuLeft();
            }
            if (direction.x > 0 && menunav < 2)
            {
                MenuRight();
            }
        }
        if (menuselected == true)
        {
            if (CurrentMenu.parent == SoundsMenu)
            {
                if (direction.y > 0)
                {
                    SoundsUp();
                }
                if (direction.y < 0)
                {
                    SoundsDown();
                }
                if (direction.x > 0)
                {
                    SoundsRight();
                }
                if (direction.x < 0)
                {
                    SoundsLeft();
                }
            }
            if (CurrentMenu.parent == GeneralMenu)
            {
                if (direction.y > 0)
                {
                    GenUp();
                }
                if (direction.y < 0)
                {
                    GenDown();
                }

            }
        }
    }
    void MenuRight()
    {
        CurrentMenu.parent.style.display = DisplayStyle.None;
        CurrentMenu.RemoveFromClassList("Selected");
        menunav += 1;
        CurrentMenu = Menus[menunav];
        CurrentMenu.BringToFront();
        CurrentMenu = Menus[menunav].Q<Label>("TabTitle");
        CurrentMenu.AddToClassList("Selected");
    }
    void MenuLeft()
    {
        CurrentMenu.RemoveFromClassList("Selected");
        menunav -= 1;
        CurrentMenu = Menus[menunav];
        CurrentMenu.BringToFront();
        CurrentMenu.style.display = DisplayStyle.Flex;
        CurrentMenu = Menus[menunav].Q<Label>("TabTitle");
        CurrentMenu.AddToClassList("Selected");
    }

    void SoundsUp()
    {
        if (optionsnav > 0)
        {
            CurrentElement.Q<Label>().RemoveFromClassList("Selected");
            optionsnav -= 1;
            CurrentElement = Interactors[optionsnav];
            CurrentElement.Q<Label>().AddToClassList("Selected");
        }
        
    }
    void SoundsDown()
    {
        if (optionsnav < 2)
        {
            CurrentElement.Q<Label>().RemoveFromClassList("Selected");
            optionsnav += 1;
            CurrentElement = Interactors[optionsnav];
            CurrentElement.Q<Label>().AddToClassList("Selected");
        }
    }
    void SoundsRight()
    {
        SliderInt yomama = CurrentElement as SliderInt; 
        yomama.value += 10;
    }
    void SoundsLeft()
    {
        SliderInt yomama = CurrentElement as SliderInt; 
        yomama.value -= 10;
    }

    void GenUp()
    {
        if (optionsnav > 0)
        {
            CurrentElement.RemoveFromClassList("Selected");
            optionsnav -= 1;
            CurrentElement = Interactors[optionsnav];
            CurrentElement.AddToClassList("Selected");
        }
    }
    void GenDown()
    {
        if (optionsnav < 2)
        {
            CurrentElement.RemoveFromClassList("Selected");
            optionsnav += 1;
            CurrentElement = Interactors[optionsnav];
            CurrentElement.AddToClassList("Selected");
        }
    }

    void ButtonActivity(Button button)
    {
        if (button.name == "Resume")
        {
            ClosePauseMenu();
        }
        if (button.name == "MainMenu")
        {
            Debug.Log("MainMenuClicked");
        }
        if (button.name == "QuitGame")
        {
            Debug.Log("QuitButtonClicked");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (flicker.enabled == true)
            {
                ClosePauseMenu();
            }
            else if (flicker.enabled == false)
            {
                OpenPauseMenu();
            }
        }
    }
    void SliderRemoveDefaultControls()
    {
        EventCallback<NavigationMoveEvent> AbsorbEvent = evt => 
        {
            evt.StopImmediatePropagation();
        };

        SoundsMenu.Q<GroupBox>().Q<SliderInt>("Music").RegisterCallback(AbsorbEvent, TrickleDown.TrickleDown);
        SoundsMenu.Q<GroupBox>().Q<SliderInt>("Game").RegisterCallback(AbsorbEvent, TrickleDown.TrickleDown);
        SoundsMenu.Q<GroupBox>().Q<SliderInt>("Dialogue").RegisterCallback(AbsorbEvent, TrickleDown.TrickleDown);
    }
}
