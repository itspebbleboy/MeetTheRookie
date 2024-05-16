
=== Misra_Dialogue ===
#NPC tag
-> DONE

= default
Woah .. something scary is going on
I think you - you didnt set a proper stitch?
Maybe you did it on purpose
Idk to each their own , just take it away now. -> DONE

= 4_1
{IsQuestComplete(visited_misra) == false:
    ~ CompleteQuest(visited_misra)
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Here we are! 
    [Misra] Kettle Rock, Main Street. Heart of the Downtown. 
    [Misra] There's bound to be some locals around - where do you want to start?
    ~ SetSpeaker(Speaker.Lupe)
-else: 
    {IsQuestComplete(visited_gen_store):
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
{IsQuestComplete(visited_gen_store) && IsQuestComplete(visited_arcade):
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

