
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
    -> DONE
}

= 3_2
{IsClueFound(evidence_claw_marks) && IsClueFound(evidence_damages):
    ~ SetSpeaker(Speaker.Misra)
    Thoughts?
    ~ SetSpeaker(Speaker.Lupe)
    It's obscure. 
    Someone was definitely here, but I can't speak to motive. 
    Who would want to break into an abandoned Winery?
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
    Oh, right! Yeah, of course you don't. 
    I can give you directions in the car.
    ~ CompleteQuest(discover_inside_clues)
    TODO SFX once fade to black, car leave noises
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
~ SetSpeaker(Speaker.Misra)
Where to next, Detective?
    -> DONE

