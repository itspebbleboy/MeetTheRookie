using System.Collections;
using System.Collections.Generic;
using Darklight.Game.Grid2D;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class PlayerUIHandler : OverlapGrid2D
{
    public UXML_WorldSpaceElement activeDialogueBubble { get; private set; } = null;
    public VisualTreeAsset visualTreeAsset;
    public PanelSettings panelSettings;



    UXML_WorldSpaceElement CreateDialogueBubbleAt(Vector3 worldPosition, float destroy_after = -1f)
    {
        if (activeDialogueBubble == null)
        {
            // Create a new dialogue bubble
            this.activeDialogueBubble = new GameObject("DialogueBubble").AddComponent<UXML_WorldSpaceElement>();
            activeDialogueBubble.transform.SetParent(this.transform);
            activeDialogueBubble.transform.position = worldPosition;
            activeDialogueBubble.Initialize(visualTreeAsset, panelSettings);
        }

        if (destroy_after >= 0)
        {
            Destroy(activeDialogueBubble.gameObject, destroy_after);
        }

        return activeDialogueBubble;
    }

    public void CreateBubbleAtBestPosition()
    {
        // Create a dialogue bubble at the best position
        OverlapData data = GetOverlapDataWithLowestWeightValue();
        Vector3 worldPosition = dataGrid.GetWorldSpacePosition(data.positionKey);
        CreateDialogueBubbleAt(worldPosition);
    }

    public void CreateChoices()
    {

    }

# if UNITY_EDITOR
    [CustomEditor(typeof(PlayerUIHandler))]
    public class PlayerUIHandlerEditor : OverlapGrid2DEditor
    {
        public override void OnInspectorGUI()
        {
            OverlapGrid2D overlapGridScript = (OverlapGrid2D)target;
            base.OnInspectorGUI();
            //DrawDefaultInspector();

            PlayerUIHandler playerUIHandler = (PlayerUIHandler)target;

            if (GUILayout.Button("Set To Default Values"))
            {
                overlapGridScript.Initialize();
            }

            if (GUILayout.Button("Create Bubble"))
            {
                playerUIHandler.CreateBubbleAtBestPosition();
            }
        }

        private void OnSceneGUI()
        {
            PlayerUIHandler playerUIHandler = (PlayerUIHandler)target;
            if (playerUIHandler.activeDialogueBubble != null)
            {
                Handles.Label(playerUIHandler.activeDialogueBubble.transform.position, "Active Dialogue Bubble");
            }

            OverlapGrid2D overlapGridScript = (OverlapGrid2D)target;
            overlapGridScript.Update();
            DisplayGrid2D(overlapGridScript.dataGrid);
        }
    }
}
#endif