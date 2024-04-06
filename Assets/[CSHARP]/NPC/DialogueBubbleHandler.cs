using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

[UxmlElement]
public partial class UXML_InkyLabel : Label
{
    public UXML_InkyLabel()
    {
        AddToClassList("inky-label");
    }

    public void SetText(string text)
    {
        this.text = text;
    }

    public void SetBubble(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}

public class DialogueBubbleHandler : MonoBehaviour
{
    UIDocument uiDocument;
    PanelSettings panelSettings;
    VisualElement root;
    UXML_InkyLabel inkyLabel;
    VisualElement bubble;

    [TextArea(3, 10)]
    public string dialogueText = "Hello, World!";

    public Sprite bubbleSprite;

    public MeshRenderer meshRenderer;
    public Material material;
    public RenderTexture renderTexture;

    public void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        panelSettings = uiDocument.panelSettings;
        inkyLabel = root.Q<UXML_InkyLabel>();
        bubble = root.Q<VisualElement>("BubbleSprite");
        root.Q<UXML_InkyLabel>().dataSource = this;
    }

    public void Update()
    {
        // Update the text only if it has changed
        if (inkyLabel.text != dialogueText)
        {
            // Set Values
            inkyLabel.SetText(dialogueText);
            bubble.style.backgroundImage = new StyleBackground(bubbleSprite);

            // Destroy old render texture
            if (panelSettings.targetTexture != null)
            {
                panelSettings.targetTexture.Release();

                // destroy the old render texture in play and edit mode
                if (Application.isPlaying)
                {
                    Destroy(panelSettings.targetTexture);
                }
                else
                {
                    DestroyImmediate(panelSettings.targetTexture);
                }
            }

            // create a new render texture
            panelSettings.targetTexture = new RenderTexture(512, 512, 24);

            // assign the render texture to the material
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = new Material(material);
            meshRenderer.sharedMaterial.mainTexture = panelSettings.targetTexture;
        }
    }


}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueBubbleHandler))]
public class DialogueBubbleEditor : Editor
{
    private void OnEnable()
    {
        DialogueBubbleHandler dialogueBubble = (DialogueBubbleHandler)target;
        dialogueBubble.Awake();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueBubbleHandler dialogueBubble = (DialogueBubbleHandler)target;
        if (GUILayout.Button("Update Text"))
        {
            dialogueBubble.Update();
        }
    }
}
#endif
