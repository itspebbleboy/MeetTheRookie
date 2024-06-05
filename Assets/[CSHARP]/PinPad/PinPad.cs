using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Input;
using Darklight.UnityExt.UXML;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PinPad : MonoBehaviour
{
    public VisualElement groupbase;
    public Label Numbers;
    public VisualElement currentselected;
    public List<VisualElement> buttons = new List<VisualElement>();
    public VisualElement LFlasher;
    public VisualElement RFlasher;
    private int selector = 0;
    private string correctcode;
    public int Inputted = 0;
    public bool ispinpadcorrect;
    // Start is called before the first frame update
    void Awake()
    {
    }

    void Start()
    {
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract += OnPrimaryInteractAction;
        VisualElement root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        groupbase = root.Q<VisualElement>("Base");
        Numbers = root.Q<VisualElement>("Numbers").Q<Label>("Numbers");
        LFlasher = root.Q<VisualElement>("LFlasher");
        RFlasher = root.Q<VisualElement>("RFlasher");
        foreach (VisualElement row in groupbase.Children())
        { 
            foreach (VisualElement child in row.Children())
            {
                buttons.Add(child);
            }
        }
        correctcode = "100722";
        buttons.Remove(Numbers);
        currentselected = buttons[selector];
        currentselected.RemoveFromClassList("UnHovered");
        currentselected.AddToClassList("Hovered");
        groupbase.style.scale = new StyleScale(new Scale(new Vector2(1.4f, 1.4f)));
    }
        void OnMoveInputStartAction(Vector2 dir)
    {
        Vector2 directionInScreenSpace = new Vector2 (dir.x, -dir.y);
        if (directionInScreenSpace.x > 0)
        {
            if (selector < 11)
            {
            UnHover();
            selector += 1;
            currentselected = buttons[selector];
            Hover();
            }
        }
        if (directionInScreenSpace.y > 0)
        {
            if (selector < 9)
            {
            UnHover();
            selector += 3;
            currentselected = buttons[selector];
            Hover();
            }
        }
        if (directionInScreenSpace.x < 0)
        {
            if (selector > 0)
            {
            UnHover();
            selector -= 1;
            currentselected = buttons[selector];
            Hover();
            }
        }
        if (directionInScreenSpace.y < 0)
        {
            if (selector > 2)
            {
            UnHover();
            selector -= 3;
            currentselected = buttons[selector];
            Hover();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
    
    void OnPrimaryInteractAction()
    {
        StartCoroutine(Select());
    }
    IEnumerator Select()
    {
        currentselected.RemoveFromClassList("Hovered");
        currentselected.AddToClassList("Selected");
        if (currentselected.name != "Pound" && currentselected.name != "Star")
        {
            if (Inputted < 6)
            {
            Inputted += 1;
            Numbers.text += currentselected.name;
            }
        }
        if (currentselected.name == "Star")
        {
            Inputted = 0;
            Numbers.text = "";
        }
        if (currentselected.name == "Pound")
        {
            if (Numbers.text != correctcode)
            {
                StartCoroutine(Incorrect());
            }
            if (Numbers.text == correctcode)
            {
                StartCoroutine(Correct());
            }
        }
        yield return new WaitForSecondsRealtime(0.3f);
        currentselected.RemoveFromClassList("Selected");
        currentselected.AddToClassList("Hovered");
    }
    IEnumerator Correct()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        LFlasher.style.unityBackgroundImageTintColor = new StyleColor(new Color(1,1,1,1));
        ispinpadcorrect = true;

    }
    IEnumerator Incorrect()
    {
        RFlasher.style.unityBackgroundImageTintColor = new StyleColor(new Color(1,1,1,1));
        yield return new WaitForSecondsRealtime(0.2f);
        RFlasher.style.unityBackgroundImageTintColor = new StyleColor(new Color(.4156f,.4156f,.4156f,1));
        yield return new WaitForSecondsRealtime(0.2f);
        RFlasher.style.unityBackgroundImageTintColor = new StyleColor(new Color(1,1,1,1));
        yield return new WaitForSecondsRealtime(0.2f);
        RFlasher.style.unityBackgroundImageTintColor = new StyleColor(new Color(.4156f,.4156f,.4156f,1));
    }
    void UnHover()
    {
        currentselected.RemoveFromClassList("Selected");
        currentselected.RemoveFromClassList("Hovered");
        currentselected.AddToClassList("UnHovered");
    }
    void Hover()
    {
        currentselected.RemoveFromClassList("UnHovered");
        currentselected.AddToClassList("Hovered");
    }

    private void OnDestroy()
    {
        UniversalInputManager.OnMoveInputStarted -= OnMoveInputStartAction;
        UniversalInputManager.OnPrimaryInteract -= OnPrimaryInteractAction;
    }
    
}
