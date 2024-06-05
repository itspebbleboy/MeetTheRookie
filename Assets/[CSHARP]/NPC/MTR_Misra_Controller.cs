using System.Collections.Generic;
using UnityEngine;
using Darklight.Utility;
using Darklight.UnityExt.Editor;
using System;

[RequireComponent(typeof(NPC_Animator), typeof(NPC_Interactable))]

public class MTR_Misra_Controller : NPC_Controller {
    public override void Start()
    {
        base.Start();

        // ADD MISRA-SPECIFIC STATES
        GrabbedState grabbedState = new(NPCState.GRABBED, new object[] { this.stateMachine });
        StruggleState struggleState = new(NPCState.STRUGGLE, new object[] { this.stateMachine });
        DraggedState draggedState = new(NPCState.DRAGGED, new object[] { this.stateMachine });

        stateMachine.AddState(grabbedState);
        stateMachine.AddState(struggleState);
        stateMachine.AddState(draggedState);
    }
}