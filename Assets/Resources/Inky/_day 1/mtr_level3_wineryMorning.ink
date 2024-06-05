// SCENE 3: The Winery
// --- scene 3 ---
=== level3 ===
Level 3
-> scene3_1

=== scene3_1 ===
// location : The Winery, outside

// Lupe and Misra arrive at the Winery. For a place having just closed a short time ago, it looks pretty run down. The whole building is in obvious disrepair with loose wood on the ground and vines creeping up the sides of the walls. Kind of creepy.
~ SetActiveQuestChain(Level3_Quests)
* [cut fence] -> fence
* [window] -> window
* [tracks] -> strange_footsteps
* [door] -> door_main_room
* [misra] -> Misra_Dialogue.3_1
// QUEST OBJECTIVE: What Happened at the Winery?
// Needed evidence to progress to inside the Winery: window evidence, fence evidence, footsteps evidence
= misra 
-> Misra_Dialogue.3_1
= door_main_room
{IsQuestComplete(discover_outside_clues):
    ~ SetSpeaker(Speaker.Lupe)
    In we go.
    ~ ChangeGameScene("scene3_2")
    -> DONE
- else:
    ~ SetSpeaker(Speaker.Lupe)
    This door must lead inside.
    -> DONE
}

= fence 
// Lupe walks over to the fence. A large tear had been ripped through the middle of the chain link fence. Just big enough for the average height of an adult.
{IsQuestComplete(talk_to_misra_quest):
    // ADD TO SYNTHESIS
    ~ SetSpeaker(Speaker.Lupe)
    Seems like the fence has been cut through. That gap looks just big enough for a person...
    ~ CompleteQuest(visit_fence)
    ~ DiscoverClue(evidence_fence)
    -> DONE

- else:
    ~ SetSpeaker(Speaker.Lupe)
    It's a torn fence.
    -> DONE
}

= window
{IsQuestComplete(talk_to_misra_quest):
    // Lupe approaches a window on the front of the Winery that has been shattered into a million pieces on the ground. Puzzling...
    // ADD TO SYNTHESIS
    ~ DiscoverClue(evidence_broken_window)
    ~ SetSpeaker(Speaker.Misra)
    Hm. I'd say this is a pretty solid piece of evidence.
    ~ SetSpeaker(Speaker.Lupe)
    Yeah, it's definitely been broken by force. But something about this seems off to me...
    -> DONE
    
- else:
    ~ SetSpeaker(Speaker.Lupe)
    Eek. Watch your step. 
    -> DONE
}


= strange_footsteps
// Lupe looks down at the ground and sees a series of strange footsteps that look humanoid...ish..
// ADD TO SYNTHESIS
{IsQuestComplete(talk_to_misra_quest):
    {IsQuestComplete(visit_footsteps):
        ~ SetSpeaker(Speaker.Misra)
        Maybe they didn't want to leave shoe prints? -> DONE 
    
    -else:
        ~ CompleteQuest(visit_footsteps)
        ~ DiscoverClue(evidence_footsteps)
        ~ SetSpeaker(Speaker.Lupe)
        ...What's this?
        ~ SetSpeaker(Speaker.Misra)
        Footprints at the scene of the crime. <i>Classic</i>.
        ~ SetSpeaker(Lupe)
        Who would be walking around barefoot? 
        -> DONE
    }
- else:
    ~ SetSpeaker(Speaker.Lupe)
    Those are some interesting footprints.
    -> DONE
}


===scene3_2===
//Lupe and Misra enter the Winery. Inside is in shambles, clearly ransacked and clearly abandoned (large equipment and wine barrels still there). Dried, spilled wine is smeared on the floor and on some equipment.

// Note, only the main room is accessible during this time. The door that leads to the office area is closed and locked, located on the back wall.

* [misra] -> Misra_Dialogue.3_2
* [wine barrels] -> wine_barrels
* [claw marks] -> claw_marks
* [damaged equipment] -> equipment
* [floor wine] -> floor_splatters
* [backroom door] -> door_office
* [winery front door] -> door_winery_exterior
* [handprint] -> handprint
* [inside_window] ->inside_window
= misra 
    -> Misra_Dialogue.3_2
    
= inside_window
    ~ SetSpeaker(Speaker.Lupe)
    Something about this broken window doesn't make sense to me...
    ~ CompleteQuest(visit_inside_window)
    -> DONE

= handprint
    ~ SetSpeaker(Speaker.Lupe)
     Well, someone didn't care about leaving behind evidence.
    ~ SetSpeaker(Speaker.Misra)
     Could we use that for prints?
     ~ SetSpeaker(Speaker.Lupe)
     Do you have a lab here in Kettle Rock?
    ~ SetSpeaker(Speaker.Misra) 
    ...
    Erm, no.
     ~ SetSpeaker(Speaker.Lupe)
     Then, no.
     ~ DiscoverClue(evidence_handprint)
     -> DONE


= claw_marks
    ~ SetSpeaker(Speaker.Lupe)
    What could've left those?
    ~ SetSpeaker(Speaker.Misra)
    Some kind of animal, maybe? Maybe a raccoon is our culprit.
    ~ SetSpeaker(Speaker.Lupe)
    Those are some big marks.
    ~ SetSpeaker(Speaker.Misra)
    A...large...raccoon..?
    ~ DiscoverClue(evidence_claw_marks)
    -> DONE

= floor_splatters
    ~ SetSpeaker(Speaker.Misra)
    I know what you're thinking! Don't worry, it's not blood. I'm sure you've already deduced this, but it's dried up wine from when this place was used.
    ~ CompleteQuest(visit_floor)
    -> DONE

= equipment
    ~ DiscoverClue(evidence_damages)
    // ADD TO SYNTHESIS
    ~ SetSpeaker(Speaker.Lupe)
    Some of this stuff looks like it's been ripped apart.
    -> DONE
    

= wine_barrels
    ~ CompleteQuest(visit_barrels)
    ~ SetSpeaker(Speaker.Misra)
    Back in the day, this place really did make good wine.
    ~ SetSpeaker(Speaker.Lupe)
    Is any of that wine left in there?
    ~ SetSpeaker(Speaker.Misra)
    Are you asking if I want to get a drink?
    ~ SetSpeaker(Speaker.Lupe)
    ...It would just be a motive, if anyone was looking to take some.
    ~ SetSpeaker(Speaker.Misra)
    Oh, no. They drained all of it when the Winery closed.
    ~ SetSpeaker(Speaker.Lupe)
    I don't drink when I'm on a case.
    ~ SetSpeaker(Speaker.Misra)
    I know, I know. Professional, as always.
    -> DONE

= door_office
    ~ CompleteQuest(visit_backroom_door)
    ~ SetSpeaker(Speaker.Lupe)
    Where does this lead?
    ~ SetSpeaker(Speaker.Misra)
    To one of the backrooms. It was locked when the Winery shut down, and is still locked. 
    ~ SetSpeaker(Speaker.Lupe)
    So they wouldn't have been able to access that room?
    ~ SetSpeaker(Speaker.Misra)
    Not unless they had a key.
    //DON'T GO INTO OFFICE
    -> DONE

= door_winery_exterior
    {IsQuestComplete(discover_inside_clues):
        ~ ChangeGameScene("scene4_1")
        -> DONE
    - else:
        ~ SetSpeaker(Speaker.Misra)
        We probably shouldn't leave just yet! More investigating to do!
        -> DONE
    }