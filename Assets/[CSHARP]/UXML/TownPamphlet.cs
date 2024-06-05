using System;
using System.Collections;
using System.Collections.Generic;
using Darklight.UnityExt.Input;
using UnityEngine;
using UnityEngine.UIElements;

public class TownPamphlet : MonoBehaviour
{
    public List<VisualElement> Pages = new List<VisualElement>();
    public VisualElement pamphlet;
    public VisualElement currentselected;
    public int page;
    // Start is called before the first frame update
    void Start()
    {
        UniversalInputManager.OnMoveInputStarted += OnMoveInputStartAction;
        VisualElement root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        pamphlet = root.Q<VisualElement>("Pamphlet");
        foreach (VisualElement page in pamphlet.Children())
        {
            Pages.Add(page);
            page.AddToClassList("Unselected");
        }
        currentselected = Pages[page];
        currentselected.RemoveFromClassList("Unselected");
    }

    private void OnMoveInputStartAction(Vector2 moveInput)
    {
        Vector2 direction = new Vector2(moveInput.x, moveInput.y);
        if (direction.x > 0)
        {
            if (page < 3)
            {
            currentselected.AddToClassList("Unselected");
            page += 1;
            currentselected = Pages[page];
            currentselected.RemoveFromClassList("Unselected");
            }
        }
        if (direction.x < 0)
        {
            if (page > 0)
            {
            currentselected.AddToClassList("Unselected");
            page -= 1;
            currentselected = Pages[page];
            currentselected.RemoveFromClassList("Unselected");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
