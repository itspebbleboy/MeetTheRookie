using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public VisualElement root;
    public Button PlayButton;
    public Button OptionsButton;
    public Button QuitButton;
    public int selector;
    public List<Button> Buttons = new List<Button>();
    // Start is called before the first frame update
    void Start()
    {
        root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        PlayButton = root.Q<GroupBox>("Selectables").Q<Button>("Play");
        OptionsButton = root.Q<GroupBox>("Selectables").Q<Button>("Options");
        QuitButton = root.Q<GroupBox>("Selectables").Q<Button>("Quit");
        selector = 0;
        Buttons.Add(PlayButton);
        Buttons.Add(OptionsButton);
        Buttons.Add(QuitButton);
        Debug.Log(Buttons[0]);
        Buttons[selector].style.fontSize = 85;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && selector > 0)
        {
            Buttons[selector].style.fontSize = 70;
            selector -= 1;
            Buttons[selector].style.fontSize = 85;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && selector < 2)
        {
            Buttons[selector].style.fontSize = 70;
            selector += 1;
            Buttons[selector].style.fontSize = 85;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Select();
        }
    }
    void Select()
    {

    }
}
