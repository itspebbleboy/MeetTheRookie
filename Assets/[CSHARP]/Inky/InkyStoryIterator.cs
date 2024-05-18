using System.Collections.Generic;
using Darklight.Console;
using Darklight.Utility;
using Ink.Runtime;
using UnityEngine;

/// <summary>
/// This class is responsible for iterating through the Ink story and handling the different states of the story.
/// It implements a simple state machine to track the current state of the story.
/// </summary>
public class InkyStoryIterator : StateMachine<InkyStoryIterator.State>
{
    public enum State { NULL, START, DIALOGUE, CHOICE, END }
    private const string Prefix = "[InkyKnot] >> ";
    private InkyStoryObject _storyObject;
    private Story story => _storyObject.story;
    private Dictionary<Choice, int> _choiceMap = new Dictionary<Choice, int>();

    // ------------------- [[ PUBLIC ACCESSORS ]] -------------------
    /// <summary>
    /// This is the active text that is currently being displayed in the story, for easy reference.
    /// </summary>
    public string CurrentText => story.currentText.Trim();

    // ------------------- [[ EVENTS ]] -------------------
    public delegate void OnDialogue(string currentText);
    public event OnDialogue OnKnotDialogue;

    // ------------------- [[ CONSTRUCTORS ]] -------------------
    public InkyStoryIterator(InkyStoryObject inkyStoryObject, State initialState = State.NULL) : base(initialState)
    {
        _storyObject = inkyStoryObject;
        //_storyObject.Initialize
        GoToState(initialState);
    }


    public void GoToKnotOrStitch(string knotName)
    {
        try
        {
            story.ChoosePathString(knotName);
        }
        catch (System.Exception e)
        {
            InkyStoryManager.Console.Log($"{Prefix} Error: {e.Message}", 0, LogSeverity.Error);
            Debug.LogError($"{Prefix} Error: {e.Message}, this");
        }
        finally
        {
            GoToState(State.START);
            InkyStoryManager.Console.Log($"{Prefix} Moved to Knot: {knotName}");
            Debug.Log($"{Prefix} Moved to Knot: {knotName}");
        }
    }

    public void ContinueStory()
    {

        // Check if null
        if (story == null || CurrentState == State.NULL)
        {
            InkyStoryManager.Console.Log($"{Prefix} Story is null", 0, LogSeverity.Error);
            Debug.LogError($"{Prefix} Error: Story is null");
            return;
        }

        // Check if end
        if (CurrentState == State.END)
        {
            InkyStoryManager.Console.Log($"{Prefix} Knot has ended", 0, LogSeverity.Warning);
            Debug.LogWarning($"{Prefix} Knot has ended");
            return;
        }

        // -- ( DIALOGUE STATE ) --------------- >>
        if (story.canContinue)
        {
            GoToState(State.DIALOGUE);
            story.Continue();

            // Invoke the Dialogue Event
            OnKnotDialogue?.Invoke(CurrentText);

            HandleTags();

            InkyStoryManager.Console.Log($"{Prefix} Continue Dialogue: {CurrentText}");
        }

        // -- ( CHOICE STATE ) --------------- >>
        else if (story.currentChoices.Count > 0)
        {
            GoToState(State.CHOICE);
            InkyStoryManager.Console.Log($"{Prefix} Choices: {story.currentChoices.Count}", 1);

            foreach (Choice choice in story.currentChoices)
            {
                _choiceMap.Add(choice, choice.index);
                InkyStoryManager.Console.Log($"{Prefix} Choice: {choice.text}", 1);
            }
        }

        // -- ( END STATE ) --------------- >>
        else
        {
            GoToState(State.END);
            InkyStoryManager.Console.Log($"{Prefix} End of Knot");
            Debug.Log($"{Prefix} End of Knot");
        }
        // Get Tags
        List<string> tags = story.currentTags;
        if (tags != null && tags.Count > 0)
        {
            foreach (string tag in tags)
            {
                InkyStoryManager.Console.Log($"{Prefix} Found Tag: {tag}", 3);
            }
        }
    }

    public void ChooseChoice(int choiceIndex)
    {
        Choice choice = story.currentChoices[choiceIndex];
        story.ChooseChoiceIndex(choice.index);
        _choiceMap.Clear();
        ContinueStory();
    }


    private void HandleTags()
    {
        List<string> currentTags = story.currentTags;
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                InkyStoryManager.Console.Log($"{Prefix} Tag is not formatted correctly: {tag}", 0, LogSeverity.Error);
            }
            else
            {
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                // handle the tag
                switch (tagKey)
                {
                    default:
                        InkyStoryManager.Console.Log($"{Prefix} Tag came in but is not currently being handled: {tag}", 0, LogSeverity.Warning);
                        break;
                }
            }

        }
    }


}

