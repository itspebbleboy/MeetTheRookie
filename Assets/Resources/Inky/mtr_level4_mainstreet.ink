// ------------------------------------------------------------------ //
// MEET THE ROOKIE
//      - Scene 4.0 - 4.5
// Quest Objective of Scene: Who broke into the Old Winery?
// Current Synthesis Pages: Who broke into the Old Winery?
// AND The Town of Kettle Rock (for general town information)
//
//
// Last Edited By : Sky 5/15
// **** HEAVILY MODIFIED VERSION - 5/15 Build - in Unity Only
//      + missing 4.2 onward
//
// **** NOTES:
// ---------------------------------------------- >>/^*
INCLUDE mtr_global.ink

// ---- [[ LOCAL VARIABLES ]] ---- >>
VAR closed_signs = 0


// Debug Knot for Inky Testing
-> debug_level4
=== debug_level4 ===
* [Scene 4_1 - Main Street] -> scene4_1


// ===================================== SCENE 4_1 ======================= //
=== scene4_1 === 
# Location: Main Street
# Lupe and Misra find themselves on the Main Street of Kettle Rock, in front of a Fountain. 
# The General Store and Arcade are accessible.
# The Bar is not.
# Any store marked with a closed sign is not accessible.

~ SetActiveQuestChain(Level4_Quests)

+ [talk to misra] -> talk_to_misra

+ [go to gen store] -> idahome_and_goods
+ [go to arcade] -> powerup_arcade
+ [go to bar] -> the_rockin_kettle

+ [laundromat sign] -> laundromat_closed_sign
+ [einab sign] -> einab_closed_sign
+ [apartments sign] -> apartments_for_lease_sign
+ [diner closed sign] -> diner_closed_sign
+ [clothing store closed sign] -> clothing_store_closed_sign

+ [strange symbol] -> strange_symbol_on_fountain
+ [goop] -> goop_on_fountain

+ {closed_signs >= 5} ["The Heart of Kettle Rock" seems a bit...barren.]
-> DONE

= talk_to_misra
    -> Misra_Dialogue

= idahome_and_goods
    -> DONE
    

= powerup_arcade
    -> DONE

= strange_symbol_on_fountain
    {IsQuestComplete(visited_symbol):
    ~ SetSpeaker(Speaker.Misra)
    [Misra]{It's probably just some historical symbol. | I don't think it's anything important. | We should move on. | Where to next?}
    ~ SetSpeaker(Speaker.Lupe)
    // Add to Synthesis - The Town of KR
    -> DONE
    - else:
    [Lupe] What's this?
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Hm. Looks like some sort of silly engravement.
    ~ SetSpeaker(Speaker.Lupe)
    ~  DiscoverClue(symbol_evidence)
        ~ CompleteQuest(visited_symbol)
    # ADD TO SYNTHESIS
    -> DONE
    }

= goop_on_fountain
    {IsQuestComplete(visited_goop):
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] {So weird...|It looks...squishy?| It smells kinda acidic. | Weird.}
        -> DONE
    - else: 
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] More of this stuff.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] You've seen it before?
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] Yeah. 
        [Lupe] It was at the Gas Station near the edge of town. 
     ~ CompleteQuest(visited_goop)
     ~ DiscoverClue(goop_evidence)
    // Add to Synthesis - The Town of KR

     -> DONE
    }


   

= the_rockin_kettle
    {IsQuestComplete(visited_gen_store) && IsQuestComplete(visited_arcade):
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Looks like the Bar is open! Shall we?
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] I shouldn't drink.
        [Lupe] I still need to drive back out tonight once the road is cleared.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Right, about that...
        [Misra] I forgot to tell you, but the earliest I can get someone out there to clear the tree is tomorrow morning.
        [Misra] I'm really sorry.
        [Misra] I know you had somewhere to be.
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] Ah...
        [Lupe] Well, that's not great.
        [Lupe] But I suppose there's nothing you can do about it.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Think of it this way...we get more time to crack this case!
        [Misra] But in the meantime, let's take a bit of a break...
        ~ SetSpeaker(Speaker.Lupe)
        //-> scene4_4
        
    - else:
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Someone wants a drink, I see.
        [Misra] The Rockin Kettle doesn't open until happy hour! 
        [Misra] We can come back later.
        ~ SetSpeaker(Speaker.Lupe)
    -> DONE
    }
    

= laundromat_closed_sign
    ~ closed_signs ++ 
    ~ SetSpeaker(Speaker.Misra)
    [Misra] The good ol' 24/7 Laundromat!
    ~ SetSpeaker(Speaker.Lupe)
    [Lupe] Doesn't seem very 24/7 to me.
        -> DONE

= einab_closed_sign
    ~ closed_signs ++ 
    [Lupe] A lot of things seem to be closed...
    -> DONE

= apartments_for_lease_sign
    ~ closed_signs ++ 
    [Lupe] Hm..
        -> DONE
    

= diner_closed_sign
    ~ closed_signs ++ 
    [Lupe] Jeez, what <i>is</i> open?
    -> DONE

= clothing_store_closed_sign
   ~ closed_signs ++ 
   [Lupe] Another closed business.
   ~ SetSpeaker(Speaker.Misra)
   [Misra] Things have been rough lately.
   ~ SetSpeaker(Speaker.Lupe)
        -> DONE
        

= heart_of_kettle
        ~ SetSpeaker(Speaker.Misra)
        [Misra] It has slowed down.
        [Misra] A bit.
        [Misra] Usually it's busy and full of life,
        [Misra] but things have been bumpy for a lot of locals lately,
        [Misra] Not enough profit to keep them going.
        [Misra] Breaks my heart a bit.
        ~ SetSpeaker(Speaker.Lupe)
        // Add to Synthesis - The Town of KR
        -> DONE

