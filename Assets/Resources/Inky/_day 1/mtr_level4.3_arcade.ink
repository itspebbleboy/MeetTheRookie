
=== scene4_3 ===
// FUNC SCENE CHANGE
# Location: Power Up Arcade
//+ {CompleteQuest(visited_arcade)}[front_door] -> scene4_1.main_street

//* [talk_to_misra] -> Misra_Dialogue
//* [talk_to_jenny] -> Jenny_Dialogue
//* [machines] -> arcade_machines
//* [exit_door] -> exit_scene

    -> DONE

= arcade_machines
    {IsQuestComplete(visited_machines):
        {"Mac Pan"... huh | "Donkey King"...that's...okay.. | "GalaxyBattles!" | "Sidewalk Fighter" | So weird...}
    - else:
     I don't recognize any of these games.
    ~ SetSpeaker(Speaker.Misra)
     Yeah...
     They're all knock offs of knock offs.
     They're cheaper that way.
     And twice the fun!
    -> DONE
    }
= teens
    -> Teens_Dialogue.teens
    
= jenny
    -> Teens_Dialogue.jenny
= calvin
    -> Teens_Dialogue.calvin
= josh 
    -> Teens_Dialogue.josh
    
= teens_roulette

* [Jenny] -> Jenny_Dialogue
* [Calvin] -> Calvin_Dialogue
* [Josh] -> Josh_Dialogue
* {HOSI_mentioned} [What's HOSI?] -> Jenny_Dialogue


* {IsQuestComplete(visited_jenny)} [Jenny] -> Jenny_Dialogue
* {IsQuestComplete(calvin_first_interact)} [Calvin] -> Calvin_Dialogue
* {IsQuestComplete(josh_first_interact)} [Josh] -> Josh_Dialogue
    
        
* {IsQuestComplete(visited_josh) && IsQuestComplete(visited_calvin) && IsQuestComplete(visited_jenny)} [Say I wanted to learn more about the Winery...anyone been talking about it lately?]
    -> DONE
    
= teens_suspect
    ~ StartQuest(suspects)
    *  -> Jenny_Dialogue
    * [Calvin] -> Calvin_Dialogue
    * [Josh] -> Josh_Dialogue
    
//    * {IsClueFound(jenny_suspects) && IsClueFound(josh_suspects) && IsClueFound(calvin_suspects)} [So. Anything else you wanna get off your chests?] -> exit_scene
    
= exit_scene
    ~ SetSpeaker(Speaker.Jenny)
     Unless the Rookie Sheriff wants to conceit their HO:SI score, no.
     We're done talking with you losers.
    ~ SetSpeaker(Speaker.Lupe)
    ~ CompleteQuest(complete_arcade)
    ~ CompleteQuest(suspects)
    -> DONE
