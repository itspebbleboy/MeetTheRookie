
=== Misra_Dialogue ===
#NPC tag
-> DONE

= default
Woah .. something scary is going on
I think you - you didnt set a proper stitch?
Maybe you did it on purpose
Idk to each their own , just take it away now. -> DONE

= 3_1
{IsClueFound(evidence_fence) && IsClueFound(evidence_broken_window) && IsClueFound(evidence_footsteps):
    ~ SetSpeaker(Speaker.Misra)
    Well, what do you make of it, Detective?
    ~ SetSpeaker(Speaker.Lupe)
    It all points to an intruder, yeah. Whoever made the noise complaint probably heard something legit.
    ~ SetSpeaker(Speaker.Misra)
    The plot thickens!! Shall we check inside? 
    ~ CompleteQuest(discover_outside_clues)
    -> DONE
    
- else:
    ~ SetSpeaker(Speaker.Misra)
    Here we are! Kettle Rock's pride and joy. Best wine north of California.
    ~ SetSpeaker(Speaker.Lupe)
    You said this place closed a few weeks ago? It looks like it's been abandoned for years.
    ~ SetSpeaker(Speaker.Misra)
    Well, it <i>has</i> been a bit neglected recently. But make no mistake, this is a piece of town history! 
    ~ SetSpeaker(Speaker.Lupe)
    Right... 
    ~ CompleteQuest(talk_to_misra_quest)
    -> DONE
}

= 3_2
{IsClueFound(evidence_claw_marks) && IsClueFound(evidence_damages):
    ~ SetSpeaker(Speaker.Misra)
    Thoughts?
    ~ SetSpeaker(Speaker.Lupe)
    It's obscure. Someone was definitely here, but I can't speak to motive. Who would want to break into an abandoned Winery?
    ~ SetSpeaker(Speaker.Misra)
    That sounds like it's time for some profiling! We need a suspect list.
    ~ SetSpeaker(Speaker.Lupe)
    Any leads on who it could be?
    ~ SetSpeaker(Speaker.Misra)
    Our best bet is questioning some locals. 
    ~ SetSpeaker(Speaker.Lupe)
    I agree.
    ~ SetSpeaker(Speaker.Misra)
    Copy that, Detective. Lead the way! 
    ~ SetSpeaker(Speaker.Lupe)
    I...don't know the way to Downtown.
    ~ SetSpeaker(Speaker.Misra)
    Oh, right! Yeah, of course you don't. Follow me.
    ~ CompleteQuest(discover_inside_clues)
    -> DONE
    
- else:
    ~ SetSpeaker(Speaker.Lupe)
    Wow. The inside is <i>so</i> much better than the outside.
    ~ SetSpeaker(Speaker.Misra)
    Never judge a book by it's cover! 
    ~ SetSpeaker(Speaker.Lupe)
    That was sarcasm, you know that, right?
    ~ SetSpeaker(Speaker.Misra)
    I do! The advice still applies.
    -> DONE
}


= 4_1
{IsQuestComplete(visited_misra) == false:
    ~ CompleteQuest(visited_misra)
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Here we are! 
    [Misra] Kettle Rock, Main Street. Heart of the Downtown. 
    [Misra] There's bound to be some locals around - where do you want to start?
    ~ SetSpeaker(Speaker.Lupe)
-else: 
    {IsQuestComplete(visited_roy):
        ~ SetSpeaker(Speaker.Misra)
        [Misra] I'm sorry if Roy seems like a bit of a downer.
        [Misra] He has no faith.
        ~ SetSpeaker(Speaker.Lupe)
        [Lupe] He seems like he's got a pretty good acceptance of the situation.
        [Lupe] From what I can tell.
        ~ SetSpeaker(Speaker.Misra)
        [Misra] Well, you've only been here a day...
        ~ SetSpeaker(Speaker.Lupe)
    }
}
{IsQuestComplete(visited_jenny) && IsQuestComplete(complete_arcade):
    ~ SetSpeaker(Speaker.Misra)
     [Misra] Those guys are the worst.
    ~ SetSpeaker(Speaker.Lupe)
     [Lupe] They're definitely hiding <i>something</i>.
    ~ SetSpeaker(Speaker.Misra)
     [Misra] Good luck getting anything out of them.
    ~ SetSpeaker(Speaker.Lupe)
     // Add to Synthesis: Who broke into the Winery?
}
    -> DONE

