using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class SynthTestControls : MonoBehaviour
{
    public SynthesisItems[] SynthItems;
    public SynthesisItems CurrentItem;
    public SynthesisItems PreviousItem;
    public SynthesisItems[] SelectedItems;
    public SynthesisItems SynthesizeButton;
    public int selector = 0;
    public bool movable = false;
    public GameObject Combiner;
    void OnEnable()
    {
        SynthItems = FindObjectsByType<SynthesisItems>(FindObjectsSortMode.InstanceID);
        CurrentItem = SynthItems[selector];
        StartCoroutine(StartHover());
    }
    void Update()
    {
        if (movable == true)
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                Move("Left");
            }
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                Move("Right");
            }
        }
    }
    void Move(string dir)
    {
        PreviousItem = CurrentItem;
        if (dir == "Left")
        {
            if (selector != 0)
            {
                selector -= 1;
            } else {
                selector = SynthItems.Length - 1;
            }
        }
        if (dir == "Right")
        {
            if (selector != SynthItems.Length - 1)
            {
                selector += 1;
            } else {
                selector = 0;
            }
        }
        CurrentItem = SynthItems[selector];
        CurrentItem.OnHover();
        PreviousItem.OnHover();
        Debug.Log(CurrentItem.name);
    }

    private IEnumerator StartHover()
    {
        yield return new WaitForSeconds(0.5f);
        CurrentItem.OnHover();
        movable = true;
    }

    /*void ReadyToCombine()
    {
        SynthItems[SynthItems.Length] = SynthesizeButton;
        SynthesizeButton.gameObject.SetActive(true);
    }
    public void Combine()
    {
        foreach (SynthesisItems item in SelectedItems)
        {
            item.gameObject.SetActive(false);
        }   
        StartCoroutine(CombinationMaker());
    }
    IEnumerator CombinationMaker()
    {
        SynthesizeButton.gameObject.SetActive(false);
        Combiner.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        Combiner.SetActive(false);

    }*/
}
