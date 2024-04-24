using UnityEngine;
using static Darklight.UnityExt.CustomInspectorGUI;

public enum TempType { BASIC, NPC }
public class InkyInteractable : Interactable
{
    [SerializeField] public TempType tempType;
    [SerializeField] private string inkKnotName;
    public InkyKnotIterator knotIterator;



    [Space(10), Header("Materials")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material activeMaterial;
    [SerializeField] private Material defaultMaterial;

    [Header("Scene Change")]
    [SerializeField] private string sceneName = "";

    [ShowOnly] public string currentText;
    [ShowOnly] public InkyKnotIterator.State currentKnotState = InkyKnotIterator.State.NULL;

    protected override void Initialize()
    {
    }

    public void Update()
    {
        if (isComplete || knotIterator == null) return;
        currentKnotState = knotIterator.CurrentState;
        currentText = knotIterator.currentText;
        if (knotIterator.CurrentState == InkyKnotIterator.State.END)
        {
            Complete();
        }


        // Set MAterial
        if (meshRenderer)
        {
            if (isActive)
            {
                meshRenderer.material = activeMaterial;
            }
            else
            {
                meshRenderer.material = defaultMaterial;
            }
        }

    }


    public override void Interact()
    {
        //Debug.Log("Interacting with InkyInteractable");

        // Move the story to this knot
        if (knotIterator == null)
        {
            knotIterator = InkyStoryManager.Instance.CreateKnotIterator(inkKnotName);
        }

        // State Handler
        switch (knotIterator.CurrentState)
        {
            case InkyKnotIterator.State.START:
            case InkyKnotIterator.State.DIALOGUE:
                knotIterator.ContinueKnot();
                break;

            case InkyKnotIterator.State.CHOICE:
                // TODO : Implement choice selection using input
                knotIterator.ChooseChoice(0);
                break;
            case InkyKnotIterator.State.END:
                Complete();
                break;
            default:
                break;
        }

        // Call the base interact method to invoke the OnInteraction event
        if (knotIterator.CurrentState != InkyKnotIterator.State.END
        && knotIterator.CurrentState != InkyKnotIterator.State.NULL)
        {
            base.Interact();
        }
    }

    public override void Complete()
    {
        base.Complete();
        Debug.Log("Completing Interaction Knot: " + inkKnotName); // Invoke the OnInteractionCompleted event

        if (sceneName != "")
        {
            SceneManager.Instance.ChangeSceneTo(sceneName);
        }

    }

    public override void OnDestroy()
    {
        //throw new System.NotImplementedException();
    }
}