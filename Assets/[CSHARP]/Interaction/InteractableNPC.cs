using UnityEngine;
using Darklight.UnityExt.Editor;
using Darklight.UXML;
using UnityEngine.UIElements;
using Darklight.Game.Grid;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(OverlapGrid2D))]
public class InteractableNPC : Interactable, IInteract
{
    [Header("NPC Speech Bubble Settings")]
    [SerializeField] private float speechBubbleScalar = 1.5f;
    [SerializeField, ShowOnly] private UXML_WorldSpaceUI dialogueBubble;
    private NPCState stateBeforeTalkedTo = NPCState.NONE;
    public UXML_WorldSpaceUI DialogueBubble { get => dialogueBubble; set => dialogueBubble = value; }
    public OverlapGrid2D overlapGrid2D => GetComponent<OverlapGrid2D>();

    public void Start()
    {
        NPC_Controller controller = GetComponent<NPC_Controller>();

        // >> ON FIRST INTERACTION -------------------------------

        this.OnFirstInteraction += () =>
        {
            stateBeforeTalkedTo = controller.stateMachine.CurrentState;
        };

        // >> ON INTERACTION -------------------------------------
        this.OnInteraction += (string currentText) =>
        {
            dialogueBubble = ShowDialogueBubble(currentText);
            if (controller)
            {
                controller.stateMachine.GoToState(NPCState.SPEAK);
            }
        };

        // >> ON COMPLETED -------------------------------------
        this.OnCompleted += () =>
        {
            //UIManager.WorldSpaceUI.Hide();
            if (controller)
            {
                controller.stateMachine.GoToState(stateBeforeTalkedTo);
            }
        };
    }

    public UXML_WorldSpaceUI ShowDialogueBubble(string text)
    {
        OverlapGrid2D_Data data = overlapGrid2D.GetBestData();
        Vector3 position = data.worldPosition;
        Debug.Log($"Grid Position: {data.positionKey} | World Position: {data.worldPosition}");

        UXML_WorldSpaceUI worldSpaceUIDoc = UIManager.Instance.worldSpaceUI;
        worldSpaceUIDoc.transform.position = position;
        worldSpaceUIDoc.transform.localScale = data.coordinateSize * Vector3.one * speechBubbleScalar;
        worldSpaceUIDoc.ElementQuery<Label>("inky-label").text = text;

        worldSpaceUIDoc.TextureUpdate();
        return worldSpaceUIDoc;
    }

    public void HideDialogueBubble()
    {
        UIManager.Instance.worldSpaceUI.Hide();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractableNPC))]
public class InteractableNPCCustomEditor : InteractableCustomEditor
{
    public override void OnInspectorGUI()
    {
        InteractableNPC interactableNPC = (InteractableNPC)target;

        DrawDefaultInspector();

        if (interactableNPC.DialogueBubble != null && interactableNPC.DialogueBubble.isVisible)
        {
            if (GUILayout.Button("Hide Dialogue Bubble"))
            {
                interactableNPC.HideDialogueBubble();
            }
        }
        else
        {
            if (GUILayout.Button("Show Dialogue Bubble"))
            {
                interactableNPC.ShowDialogueBubble("Hello, I am an NPC.");
            }
        }
    }
}
#endif