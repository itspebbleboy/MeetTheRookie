// ------------------------------------------------------------------ //
//      MEET THE ROOKIE
//      - Scene 1.0 - 1.5
// ---------------------------------------------- >>/^*

VAR gas_pumps = 0
LIST Clues_1 = (GAS_PUMP_BROKEN), (CASHREG_BROKEN), (CASHREG_FIX)
LIST QuestChain_1 = (FIRST_INTERACT), (PAY_FOR_GAS)

=== level1 ===
* [Scene1_0] -> scene1_0
* [Scene1_1] -> scene1_1
* [Scene1_2] -> scene1_2
* [Scene1_3] -> scene1_3
-> DONE



// == ( SCENE 1 ) ================================= >>
=== scene1_0 ===

* [thelton] -> thelton

= thelton
    ~ SetSpeaker(Speaker.Chief_Thelton)
    You've reached Chief Detective Inspector Thelton, Boise Precinct. I'm not available right now. You know what to do.
    // We hear a generic voicemail beep.
    
    ~ SetSpeaker(Speaker.Lupe)
    Hey, it's Lupe. Had to change my route; tank was running low. I pit stopped outside of Kettle Rock, Idaho. Should be back on the road soon--don't jump on me for being late. I'll debrief the Watchowski Case with you when I get back. Alright. Bye.
        -> scene1_1

// ------------- SCENE1.1 Outside MelOMarket
=== scene1_1 ===
// FUNC_SCENE_CHANGE
// PLAYER_PROMPT -> highlight gas pump
~ SetActiveQuestChain(QuestChain_1)
~ StartQuest(pay_for_gas)

* [npc] -> marlowe
* [lupe's car] -> car
* [gas pump] -> gas_pump
* [front door] -> enter_store

= gas_pump
    ~ CompleteQuest(first_interact)
    ~ DiscoverClue(broken_gas_pump)
    ~ gas_pumps += 1
    {IsQuestComplete(pay_for_gas):
        ~ SetSpeaker(Speaker.Lupe)
        "Sorry I was so late to the debrief boss, I had to go report a suspicious fallen tree." 
        Ugh. Guess I'll be more than a little late...
        Thelton's gonna kill me. Gah. Let's blow this popsicle stand.  -> DONE 
    - else:
        ~ SetSpeaker(Speaker.Lupe)
        {gas_pumps == 1} "Out of order. Pay inside." Of course. Just my luck. -> DONE
        {gas_pumps == 2} Let's get this over with. Sooner I pay, sooner I can get back on the road, sooner Thelton won't bite my head off. -> DONE
        {gas_pumps == 3} ... Why am I still looking at this gas pump? -> DONE
        {gas_pumps < 6} ... -> DONE
        {gas_pumps == 7} .. This gas pump is kind of freaky. -> DONE
        {gas_pumps > 7} I should really go pay for gas. -> DONE
    }

= car
    {IsQuestComplete(pay_for_gas): 
        ~ SetSpeaker(Speaker.Lupe)
        "Sorry I was so late to the debrief boss, I had to go report a suspicious fallen tree." Ugh. Guess I'll be more than a little late...Thelton's gonna kill me.
        -> DONE //{goto("precinct")} 
    - else: 
        ~ SetSpeaker(Speaker.Lupe)
        Still gotta pay.
        -> DONE
    }


= marlowe
    {IsQuestComplete(PAY_FOR_GAS):
        ~ SetSpeaker(Speaker.Unknown)
        Timber. Heh heh. Hope you didn't need to be anywhere anytime soon.
        ~ SetSpeaker(Speaker.Lupe)
        ...great. Any way I can get this fixed?
        ~ SetSpeaker(Speaker.Unknown)
        Got a chainsaw on ya? 
        Heheheheh. 
        Ah, don't look so grumpy. 
        If you put in a complaint with the local Police I'm sure you'll be out of here in no time.
        -> DONE
    - else :
        ~ SetSpeaker(Speaker.Unknown)
        You're stuck at the pump too, eh? Good luck getting that lazybones to help you. Heh. Youth these days, am I right?
        -> DONE
    }

= enter_store
    ~ SetSpeaker(Speaker.Lupe)
    I guess I'll find someone to help me inside here.
    -> DONE

// ------------- SCENE1.2 MelOMarket Store
=== scene1_2 ===
//The gas station door chimes with a slightly out of tune jingle.
//Lupe hears the employee mutter a very unattentive "Welcome to MelOMart".
* [vending_machine] -> vending_machine
* [employee] -> employee
* [backrooms] -> door_to_backroom
* [exit door] -> door_to_outside

= vending_machine
    {Sugar-Flavored Snack Bites. Pickle Chips. Jerkied Ox Sticks...I think I'll pass.| Is this stuff FDA approved? | I though Pop Pops Gum went out of production in the 90s? Weird.}
    -> DONE

= employee
{IsClueFound(broken_cash_reg):
    {IsClueFound(cashreg_fix):
        ~ SetSpeaker(Speaker.Beth)
        I told you--
        ~ SetSpeaker(Speaker.Lupe)
        Mel said to hit it. In the side.
        // the employee whacks the side of the register.The Drawer pops open.
        ~ SetSpeaker(Speaker.Beth) 
        Cool. $76.45.
        ~ SetSpeaker(Speaker.Lupe)
        Finally
        ~ CompleteQuest(pay_for_gas)
        -> DONE
    - else:
        ~ SetSpeaker(Speaker.Beth)
        {The register drawer is busted. | Register is still busted. | Stillllllll busted, buddy.}
    }
-else:
    // -- DISCOVER CASHREG_BROKEN
    ~ SetSpeaker(Speaker.Lupe)
    Hey man. How're you?
    ~ SetSpeaker(Speaker.Beth) 
    I work at a gas station. How do you think I am?
    ~ SetSpeaker(Speaker.Lupe)
    ...The pump's broken. It says to pay inside?
    ~ SetSpeaker(Speaker.Beth) 
    Yeahhhhh. You can't pay right now. The register drawer is busted
    ~ SetSpeaker(Speaker.Lupe)
    Can you, uh, fix it?
    ~ SetSpeaker(Speaker.Beth) 
    That's kinda above my paygrade?
    ~ SetSpeaker(Speaker.Lupe)
    Whose paygrade is it <i>not</i> above?
    ~ SetSpeaker(Speaker.Beth) 
    My manager's in the back.
    ~ DiscoverClue(broken_cash_reg)
    -> DONE
}

= door_to_backroom
{IsClueFound(broken_cash_reg):
    -> scene1_3
    - else:
        ~ SetSpeaker(Speaker.Lupe)
        I've got no reason to poke around. -> DONE
}

= door_to_outside
{IsQuestComplete(pay_for_gas):
    -> DONE
    -else:
        ~ SetSpeaker(Speaker.Lupe)
        Still gotta pay.
        -> DONE
}

// ------------- SCENE1.3 Breakroom ---- >>
=== scene1_3 ===
// Lupe pushes through the door to the backroom of the gas station. The room is a small break room. The presumed manager stands at the sink, scrubbing goop.

= mel
{IsClueFound(broken_cash_reg) == false:
    ~ SetSpeaker(Speaker.Lupe)
    Hey. Are you the manager?
    ~ SetSpeaker(Speaker.Mel)
    Mel's the name. What are you doing back here?
    ~ SetSpeaker(Speaker.Lupe)
    I need to pay for gas. The cash register is broken.
    ~ SetSpeaker(Speaker.Mel)
     That old piece of junk. 
    The drawer is just jammed, it just needs to be whacked in the side a bit
    ~ SetSpeaker(Speaker.Lupe)
    Thanks.
    ~ DiscoverClue(cashreg_fix)
    -> DONE
-else:
    ~ SetSpeaker(Speaker.Mel)
    {Look, I'd like to help more, but I gotta get this stuff out. | This is <i>definitely</i> a code violation. | It almost looks like mold? Or jello. Gross, ain't it? | I love my job. I love my job. Ugh.}
}

= goop
    ~ SetSpeaker(Speaker.Lupe)
        Uh. What is that?
    ~ SetSpeaker(Speaker.Mel)
        No idea, it was here when I unlocked this morning. 
        Darn stuff won't scrub off. ->DONE

// ------------- SCENE1.4 Tree Falls
=== scene1_4 ===

// Lupe pays. Suddenly, a loud crashing noise is heard from outside.
~ SetSpeaker(Speaker.Lupe)
What was that? 
* [fallen tree] -> fallen_tree

    
= fallen_tree 
    // Lupe sees a tree has fallen and blocked the way out of town.
    -> DONE
    //* What the hell... -> npc
  
