

=== thelton_intro_DAY2===

   ~ SetSpeaker(Speaker.Chief_Thelton)
   TODO SFX RING RING RING
    You've reached Chief Detective Inspector Thelton, Boise Precinct. 
    I'm not available right now. You know what to do!
    // We hear a generic voicemail beep.
    TODO SFX VOICEMAIL BEEP
    
    ~ SetSpeaker(Speaker.Lupe)
   Hey...it's Lupe. Had to change my route; tank was running low. I pit stopped outside of Kettle Rock, Idaho. Hoping to get back on the road soon...sorry, I'm a bit scatterbrained right now. Don't jump on me for being late. I'll debrief the Watchowski Case with you when I get back. Alright. Bye.
        ~ChangeGameScene("scene6_1")
        -> DONE

=== scene6_1 ===
// ------------- SCENE6.1 Outside MelOMarket
// PLAYER_PROMPT -> highlight gas pump

* [npc] -> marlowe
* [lupe's car] -> car
* [gas pump] -> gas_pump
* [front door] -> enter_store

= marlowe
 {IsQuestComplete(pay_for_gas):
        ~ SetSpeaker(Speaker.Marlowe)
        Timber. Heh heh. Hope you didn't need to be anywhere anytime soon.
        ~ SetSpeaker(Speaker.Lupe)
         ...yeah....should I...head down to the precinct to file a complaint?
        ~ SetSpeaker(Speaker.Marlowe)
       You read my mind! Eheheh.
        ~ CompleteQuest(look_at_tree)
        -> DONE
        
    - else :
        ~ SetSpeaker(Speaker.Marlowe)
        You're stuck at the pump too, eh? 
        Good luck getting that lazybones to help you. 
        Heh. Youth these days, am I right?
        -> DONE
    }

= car
{
    - IsQuestComplete(look_at_tree):
        ~ SetSpeaker(Speaker.Lupe)
        "Pump out of order, pay inside".
        Huh.
        Okay.
        Something tells me I'll be here longer than I think. I should probably try Thelton again at some point.
        TODO SFX CAR DOOR OPEN CLOSE AND STARTING
        ~ ChangeGameScene("scene7_1_DS")
        -> DONE
    - IsQuestComplete(pay_for_gas):
        That noise didn't sound good. I should probably see what's up.
    - else: 
        ~ SetSpeaker(Speaker.Lupe)
        I still gotta pay.
        -> DONE
    }

= gas_pump
 {IsQuestComplete(pay_for_gas):
        ~ SetSpeaker(Speaker.Lupe)
         {God, I feel weird. The Deja Vu is killing me right now. | Alright. Let's get this over with. | I hope it doesn't take too long.}
                 TODO SFX CAR DOOR OPEN CLOSE AND STARTING
                ~ ChangeGameScene("scene7_1_DS")
  

          -> DONE 
        
    - else:
        ~ SetSpeaker(Speaker.Lupe)
        {It's...out of order. | Guess I should pay inside.}
    }
    -> DONE

= enter_store
 ~ SetSpeaker(Speaker.Lupe)
    I guess I'll find someone to help me inside here.
    TODO SFX DOOR OPEN
    ~ ChangeGameScene("scene6_2")
    -> DONE
    
=== scene6_2 ===
//The gas station door chimes with a slightly out of tune jingle.
TODO SFX DOOR CLOSE

* [vending_machine] -> vending_machine
* [employee] -> employee
* [backrooms] -> door_to_backroom
* [exit door] -> door_to_outside

= vending_machine
    {Sugar-Flavored Snack Bites. Pickle Chips. Jerkied Ox Sticks...I think I'll pass.| Is this stuff FDA approved?}
    -> DONE

= employee
{IsQuestComplete(broken_cash_reg):
    ~ SetSpeaker(Speaker.Beth)
        {The register drawer is busted. | Register is still busted. | Stillllllll busted, buddy.}
-else:
    // -- DISCOVER CASHREG_BROKEN
    ~ SetSpeaker(Speaker.Lupe)
    Hey man. How're you?
    ~ SetSpeaker(Speaker.Beth) 
    I work at a gas station. How do you think I am?
    ~ SetSpeaker(Speaker.Lupe)
    ...The pump's broken. It says to pay inside?
    ~ SetSpeaker(Speaker.Beth) 
    Yeahhhhh. You can't pay right now. The register drawer is busted.
    ~ CompleteQuest(broken_cash_reg)
    ~ SetSpeaker(Speaker.Lupe)
    * Can you, uh, fix it? -> can_u_fix
    * ...maybe just try, uh...hitting it? -> hit_that_thang

}

= can_u_fix
~ SetSpeaker(Speaker.Beth) 
.
    That's kinda above my paygrade?
    ~ SetSpeaker(Speaker.Lupe)
    Whose paygrade is it <i>not</i> above?
    ~ SetSpeaker(Speaker.Beth) 
    My manager's in the back.
    ~DiscoverClue(broken_cash_reg)
    -> DONE

=hit_that_thang
~ SetSpeaker(Speaker.Beth) 
~ CompleteQuest(haggle)
.
...
Why would I do that?
~ SetSpeaker(Speaker.Lupe)
Well...I don't know...
It might work?
~ SetSpeaker(Speaker.Beth)
This is awkward.
~ SetSpeaker(Speaker.Lupe)
Can you just try--
~ SetSpeaker(Speaker.Beth)
Are you trying to get me to purposefully damage equipment?
~ SetSpeaker(Speaker.Lupe)
What, no, I just--
~ SetSpeaker(Speaker.Beth)
Are you trying to get me in trouble???
~ SetSpeaker(Speaker.Lupe)
* I'm really not. Hitting it might work. -> convince
* Forget it, sorry. -> forget

= convince
~ SetSpeaker(Speaker.Beth)
.
Why would that work???
~ SetSpeaker(Speaker.Lupe)
I just have a hunch.
~ SetSpeaker(Speaker.Beth)
A "hunch"??? 
I'm not gonna trust your "hunch"! 
That's weird.
~ SetSpeaker(Speaker.Lupe)
Ugh.
Look, your manager's in the back, right?
~ SetSpeaker(Speaker.Beth)
Uh, yeah.
~ SetSpeaker(Speaker.Lupe)
I'll be right back.
-> DONE

=forget
~ SetSpeaker(Speaker.Beth)
.
Uh, okay. 
Cough, cough, weird.
~ SetSpeaker(Speaker.Lupe)
...
Look, your manager's in the back, right?
~ SetSpeaker(Speaker.Beth)
Uh, yeah.
~ SetSpeaker(Speaker.Lupe)
I'll be right back.

-> DONE

=door_to_backroom
 ~ ChangeGameScene("scene6_3")
    -> DONE

=door_to_outside
{IsQuestComplete(pay_for_gas):
    ~ChangeGameScene("scene6_4")
    TODO SFX DOOR OPEN
    -> DONE
    -else:
        ~ SetSpeaker(Speaker.Lupe)
        I still need to pay.
        -> DONE
}


=== scene6_3 ===
TODO SFX DOOR CLOSE

+ [goop] -> goop
+ [mel] -> mel
+ [door to mainroom] -> door_to_mainroom

= mel
{IsClueFound(cashreg_fix) == false:
    ~ SetSpeaker(Speaker.Lupe)
    Hey. You're the manager, right?
    ~ SetSpeaker(Speaker.Mel)
    Yeah.
    I'm Mel.
    What are you doing back here?
    Did you need something?
    ~ SetSpeaker(Speaker.Lupe)
    The register is broken.
    ~ SetSpeaker(Speaker.Mel)
     That old piece of junk. 
    The drawer is just jammed, it just needs to be-
    ~ SetSpeaker(Speaker.Lupe)
    Whacked in the side a bit?
    ~ SetSpeaker(Speaker.Mel)
    Uh.
    Yeah.
    Exactly.
    How did you guess that?
    ~ SetSpeaker(Speaker.Lupe)
    Call it a hunch.
    ~ SetSpeaker(Speaker.Mel)
    Damn good hunch.
    You psychic or somethin?
    ~ SetSpeaker(Speaker.Lupe)
    I don't know.
    Lucky guess.
    ~ DiscoverClue(cashreg_fix)
    -> DONE
    
    -else:
        ~ SetSpeaker(Speaker.Mel)
        {Look, I'd like to help more, but I gotta get this stuff out. | This is <i>definitely</i> a code violation. | It almost looks like mold? Or jello. Gross, ain't it? | I love my job. I love my job. Ugh.}
        
        -> DONE
}

= goop
~ SetSpeaker(Speaker.Lupe)
That stuff looks, ah....not great.
~ SetSpeaker(Speaker.Mel)
I know.
It stinks, too.
I've been trying all morning to scrub this off.

* Have you tried bleach? -> bleach
* Any idea what it could be? -> any_idea

= bleach
~ SetSpeaker(Speaker.Mel)
. 
Bleach?
You think that will work?
~ SetSpeaker(Speaker.Lupe)
Worth a shot.
TODO implement animation where mel can clean it off
~ SetSpeaker(Speaker.Mel)
.
Well, I'll be darned.
Thanks, whoever you are.
How did you know that would help?
~ SetSpeaker(Speaker.Lupe)
I...
I'm not sure...
-> DONE

= any_idea
~ SetSpeaker(Speaker.Mel)
.
No clue.
It keeps coming out of my drains, clogging them up.
It's been annoying the hell outta me.

-> DONE

= door_to_mainroom
    ~ ChangeGameScene("scene6_4")
    TODO SFX DOOR OPEN
    -> DONE

=== scene6_4 ===
TODO SFX DOOR CLOSE
* [vending_machine] -> vending_machine
* [employee] -> employee
* [backrooms] -> door_to_backroom
* [exit door] -> door_to_outside

= intro_after_mel
~ SetSpeaker(Speaker.Lupe)
Geez, what is going on with me today?
Ugh, my head is killing me.
-> DONE

= vending_machine
~ SetSpeaker(Speaker.Lupe)
I think I'd rather lick ground than eat something out of here.
-> DONE

= employee
  {IsClueFound(cashreg_fix):
        ~ SetSpeaker(Speaker.Beth)
        I told you--
        ~ SetSpeaker(Speaker.Lupe)
        Mel said to hit it. In the side.
        // the employee whacks the side of the register.The Drawer pops open.
        TODO SFX CASH REGISTER FIX NOISE
        TODO ANIMATION REGISTER DRAWER
        ~ SetSpeaker(Speaker.Beth) 
        Cool. $76.45.
        ~ SetSpeaker(Speaker.Lupe)
        ~ CompleteQuest(pay_for_gas)
        {IsQuestComplete(haggle):
        ~ SetSpeaker(Speaker.Lupe)
        ...
        You do realize that's <i>exactly</i> what I told you to do, right?
        ~ SetSpeaker(Speaker.Beth)
        Yeah.
        But it's different if that's what my manager told you.
        If you said Mel told you to do it, I would have done it.
        ~ SetSpeaker(Speaker.Lupe)
        That's-
        Ugh, you know what?
        Nevermind.
            TODO SFX TREE CRASH
        Uh, what was that?
        ->DONE
        
    - else:
        Finally.
        ~ CompleteQuest(pay_for_gas)
        
        // TODO CRASHING TREE NOISE 
        // Lupe pays. Suddenly, a loud crashing noise is heard from outside.
        TODO SFX TREE CRASH
        What was that? 
        -> DONE
        }
    - else:
        ~ SetSpeaker(Speaker.Beth)
        {The register drawer is busted. | Register is still busted. | Stillllllll busted, buddy.}
    }

    
    
= door_to_backroom
    ~ ChangeGameScene("scene6_3")
    TODO SFX DOOR OPEN
    -> DONE

= door_to_outside
    ~ ChangeGameScene("scene6_5")
    TODO SFX DOOR OPEN
    -> DONE

=== scene6_5 ===
TODO DOOR CLOSE
// Lupe pays. Suddenly, a loud crashing noise is heard from outside.
~ SetSpeaker(Speaker.Lupe)
* [fallen tree] -> fallen_tree

= fallen_tree 
    // Lupe sees a tree has fallen and blocked the way out of town.
    -> scene1_1.marlowe
    -> DONE
    //* What the hell... -> npc





