// ------------------------------------------------------------------ //
// MEET THE ROOKIE
//      - Scene 4.0 - 4.5
// Quest Objective of Scene: Who broke into the Old Winery?
// Current Synthesis Pages: Who broke into the Old Winery?
// AND The Town of Kettle Rock (for general town information)
// ---------------------------------------------- >>/^*
// ---- [[ LOCAL VARIABLES ]] ---- >>
VAR closed_signs = 0

// ============================================================
// ========================== SCENE 4.1 =======================
=== scene4_1 === 
# Location: Main Street
# Lupe and Misra find themselves on the Main Street of Kettle Rock, in front of a Fountain. 
# The General Store and Arcade are accessible.
# The Bar is not.
# Any store marked with a closed sign is not accessible.

~ SetActiveQuestChain(Level4_Quests)

+ [talk to misra] -> talk_to_misra
+ [go to gen store] -> door_idahome_and_goods
+ [go to arcade] -> door_powerup_arcade
+ [go to bar] -> door_the_rockin_kettle

+ [laundromat sign] -> laundromat_closed_sign
+ [einab sign] -> einab_closed_sign
+ [apartments sign] -> apartments_for_lease_sign
+ [diner closed sign] -> diner_closed_sign
+ [clothing store closed sign] -> clothing_store_closed_sign

+ [strange symbol] -> strange_symbol_on_fountain
+ [goop] -> goop_on_fountain

+ {closed_signs >= 5} ["The Heart of Kettle Rock" seems a bit...barren.]
-> DONE
= misra_cutscene
    ~ CompleteQuest(visited_misra)
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Here we are! 
    [Misra] Kettle Rock, Main Street. Heart of the Downtown. 
    [Misra] There's bound to be some locals around - where do you want to start?
    ~ SetSpeaker(Speaker.Lupe)
    ->DONE
= talk_to_misra
    -> Misra_Dialogue.4_1
= door_idahome_and_goods
    ~ ChangeGameScene("scene4_2")
    -> DONE
= door_powerup_arcade
    ~ ChangeGameScene("scene4_3")
    -> DONE

= strange_symbol_on_fountain
    {IsQuestComplete(visited_symbol):
    ~ SetSpeaker(Speaker.Misra)
    {It's probably just some historical symbol. | I don't think it's anything important. | We should move on. | Where to next?}
    ~ SetSpeaker(Speaker.Lupe)
    // Add to Synthesis - The Town of KR
    -> DONE
    - else:
    What's this?
    ~ SetSpeaker(Speaker.Misra)
    Hm. Looks like some sort of silly engravement.
    ~ SetSpeaker(Speaker.Lupe)
    ~  DiscoverClue(symbol_evidence)
        ~ CompleteQuest(visited_symbol)
    # ADD TO SYNTHESIS
    -> DONE
    }

= goop_on_fountain
    {IsQuestComplete(visited_goop):
        ~ SetSpeaker(Speaker.Lupe)
        {So weird...|It looks...squishy?| It smells kinda acidic. | Weird.}
        -> DONE
    - else: 
        ~ SetSpeaker(Speaker.Lupe)
        More of this stuff.
        ~ SetSpeaker(Speaker.Misra)
        You've seen it before?
        ~ SetSpeaker(Speaker.Lupe)
        Yeah. 
        It was at the Gas Station near the edge of town. 
         ~ CompleteQuest(visited_goop)
         ~ DiscoverClue(goop_evidence)
        // Add to Synthesis - The Town of KR
         -> DONE
    }

= door_the_rockin_kettle
    {IsQuestComplete(complete_gen_store) && IsQuestComplete(complete_arcade):
        ~ SetSpeaker(Speaker.Misra)
        Looks like the Bar is open! Shall we?
        ~ SetSpeaker(Speaker.Lupe)
        I shouldn't drink.
        I still need to drive back out tonight once the road is cleared.
        ~ SetSpeaker(Speaker.Misra)
        Right, about that...
        I forgot to tell you, but the earliest I can get someone out there to clear the tree is tomorrow morning.
        I'm really sorry.
        I know you had somewhere to be.
        ~ SetSpeaker(Speaker.Lupe)
        Ah...
        Well, that's not great.
        But I suppose there's nothing you can do about it.
        ~ SetSpeaker(Speaker.Misra)
        Think of it this way...we get more time to crack this case!
        But in the meantime, let's take a bit of a break...
        ~ SetSpeaker(Speaker.Lupe)
         ~ ChangeGameScene("scene4_4")
         ->DONE
        
    - else:
        ~ SetSpeaker(Speaker.Misra)
        Someone wants a drink, I see.
        The Rockin Kettle doesn't open until happy hour! 
        We can come back later.
        ~ SetSpeaker(Speaker.Lupe)
    -> DONE
    }
    

= laundromat_closed_sign
    ~ closed_signs ++ 
    ~ SetSpeaker(Speaker.Misra)
    The good ol' 24/7 Laundromat!
    ~ SetSpeaker(Speaker.Lupe)
    Doesn't seem very 24/7 to me.-> DONE

= einab_closed_sign
    ~ closed_signs ++ 
    ~ SetSpeaker(Speaker.Lupe)
    A lot of things seem to be closed...
    -> DONE

= apartments_for_lease_sign
    ~ closed_signs ++ 
    [Lupe] Hm..
        -> DONE
    

= diner_closed_sign
    ~ closed_signs ++ 
    Jeez, what <i>is</i> open?
    -> DONE

= clothing_store_closed_sign
   ~ SetSpeaker(Speaker.Lupe)
   ~ closed_signs ++ 
   Another closed business.
   ~ SetSpeaker(Speaker.Misra)
   Things have been rough lately.
        -> DONE
        

= heart_of_kettle
    ~ SetSpeaker(Speaker.Misra)
    It has slowed down.
    A bit.
    Usually it's busy and full of life,
    but things have been bumpy for a lot of locals lately,
    Not enough profit to keep them going.
    Breaks my heart a bit.
    // Add to Synthesis - The Town of KR
    -> DONE

