
=== Irene_Dialogue ===

{IsQuestComplete(irene_intro):
    -> Irene_Dialogue.irene_questions
- else:
    ~ SetSpeaker(Speaker.Irene)
     Is that Ritam Misra I spy?
    ~ SetSpeaker(Speaker.Misra)
    [Misra] Hi, Irene!
    ~ SetSpeaker(Speaker.Irene)
     Damn, it's been a sec! 
     How're you?
     Who's your broody looking pal?
    ~ SetSpeaker(Speaker.Lupe)
     Detective Lupe.
    ~ SetSpeaker(Speaker.Irene)
     Ooh, a <i>Detective</i>.
     Like Sherlock Holmes.
     Or Nancy Drew.
     Or Scooby-Doo.
    ~ SetSpeaker(Speaker.Lupe)
     ...
     Yes.
     Like Scooby-Doo.
    ~ SetSpeaker(Speaker.Irene)
     A Detective, in my bar!
     Feel free to look around.
     Let me know if you need anything.
    ~ CompleteQuest(irene_intro)
    -> DONE
}



= irene_questions
    {IsQuestComplete(irene_convo_1) && IsQuestComplete(irene_convo_2): 
        {What's up? | Whatchya need? | Anything else? | Whatchya need? }
        -> DONE
     
    - else: 
    * {IsClueFound(goats_mentioned)} [People have been mentioning goats a lot.] -> irene_goats
    * {memorial_plaque_visited} [Someone's drawn on your memorial plaque over there.] -> irene_plaque
    * {sacrifice_mentioned} [Sorry, a sacrifice?] -> irene_sacrifice
    * [How long have you lived in Kettle Rock?] -> irene_kettle_rock
    * {KR_irene && closed_shops_irene} [What do you know about the Old Winery?] -> irene_winery
    * [Despite all the closed shops and stores, this place is still open...] -> irene_still_standing
    }

= irene_goats
    ~ SetSpeaker(Speaker.Lupe)
     I get the feeling they're not talking about the animal. 
    ~ SetSpeaker(Speaker.Irene)
     Oh.
     No, they're not.
     Misra, you haven't filled them in on this?
    ~ SetSpeaker(Speaker.Misra)
     ...
     Like you said, it's disrespectful.
     I don't like talking about it.
    ~ SetSpeaker(Speaker.Lupe)
     If it's important to the case, I should know.
    ~ SetSpeaker(Speaker.Misra)
    ...
    ~ SetSpeaker(Speaker.Irene)
     It's what they call them.
     The people who disappeared in the tragedy. 
     "Goats".
    ~ SetSpeaker(Speaker.Lupe)
     Why?
    ~ SetSpeaker(Speaker.Misra)
     Irene...
    ~ SetSpeaker(Speaker.Irene)
     Come on, Misra.
     It's just silly town folklore meant to scare kids and give people a reason to gossip.
     They call them "Goats" because people think they sacrificied themselves.
     Like, you know, a sacrificial lamb. 
     But less cute. 
     So, goat.
    ~ DiscoverClue(sacrifice_mentioned)
    // Add to Synthesis: The Town of KR
    -> DONE

= irene_plaque
    ~ SetSpeaker(Speaker.Irene)
     Damn it! 
     People keep doing that.
     It's disrespectful. 
     I'll clean it off when I close up.
    ~ SetSpeaker(Speaker.Lupe)
     It says something about "goats"?
    ~ SetSpeaker(Speaker.Irene)
     Yeah, that's usually the message...
    ~ DiscoverClue(goats_mentioned)
    -> DONE
    
= irene_sacrifice
    ~ SetSpeaker(Speaker.Misra)
     It's nothing.
     Like Irene said, stupid gossip.
    ~ SetSpeaker(Speaker.Lupe)
     I'm intrigued.
    ~ SetSpeaker(Speaker.Irene)
     Of course!
     Who better to be interested in a small town mystery than a Detective?
    ~ SetSpeaker(Speaker.Misra)
     That's not the case we're working on, though.
    ~ SetSpeaker(Speaker.Lupe)
     Humor me.
    ~ SetSpeaker(Speaker.Irene)
     I'm sure Misra's told you about Kettle Rock's <i> rocky </i> past?
    ~ SetSpeaker(Speaker.Lupe)
     That part, yes.
    ~ SetSpeaker(Speaker.Irene)
     Well, legend says that in order to save the town, in desperation, a group of locals turned to darker alternatives.
     I don't know if they were dancin' with the Devil,
     or one of the many demons that want to sink their fangs into humanity's naviety,
     but they were in kahoots with something of that nature.
     And one day in 1940, when it came time to pay up and face the music...
     They <i>vanished</i>.
    ~ SetSpeaker(Speaker.Lupe)
     ...
    ~ SetSpeaker(Speaker.Misra)
     ...
    ~ SetSpeaker(Speaker.Irene)
     SpoOooOooOky.
    ~ SetSpeaker(Speaker.Lupe)
     I feel like you've given that speech before
    ~ SetSpeaker(Speaker.Irene)
     Hey, I gotta keep the drunks entertained somehow.
    ~ CompleteQuest(irene_convo_1)
     // Add to Synthesis: The Town of KR
    -> DONE

= irene_kettle_rock
    ~ SetSpeaker(Speaker.Irene)
     I moved here when I was 13!
     I actually when to highschool with Misra.
    ~ SetSpeaker(Speaker.Misra)
     Class of '86!
    ~ SetSpeaker(Speaker.Irene)
     That's right!
     Misra's a total sweetheart. 
     But I'm sure you know that already.
    ~ DiscoverClue(KR_irene)
    -> DONE

= irene_winery
    ~ SetSpeaker(Speaker.Irene)
     Sheesh, that old shack on the hill?
     More trouble than it's worth, in my opinion.
     Why?
    ~ SetSpeaker(Speaker.Misra)
     There was a break in, it looks like.
     Last night.
     We're trying to find probable cause.
    ~ SetSpeaker(Speaker.Irene)
     Heck, what are you talking to me for?
     You should be talking to Jenkins over there.
    ~ SetSpeaker(Speaker.Lupe)
     That's Jenkins?
     He's completely passed out.
    ~ SetSpeaker(Speaker.Irene)
     Yeah, he's been taking it rough.
     He worked at the place for years.
    ~ SetSpeaker(Speaker.Lupe)
     Would he have any reason to go up there and trash the place?
    ~ SetSpeaker(Speaker.Irene)
     Hell, I don't know. But I can tell you he's been here every night for the past week. Hasn't budged from his Whiskey Neat.
     But why don't you ask him yourselves?
     Give him a good shove, and he'll come to.
     I've done it a dozen times when he passes out like this.
    ~ CompleteQuest(jenkins_wakes_up)
    ~ CompleteQuest(irene_convo_2)
    -> DONE

= irene_still_standing
    ~ SetSpeaker(Speaker.Irene)
     Yeahhhhh.
     It's a little concerning that the bar is one of 
     the businesses that is still able to stay afloat.
     People need somewhere to forget about their troubles, I guess.
     To be honest, I've been debating closing up shop myself.
    ~ SetSpeaker(Speaker.Misra)
     What?
    ~ SetSpeaker(Speaker.Irene)
     Yeah; Roy offered to buy me out. 
     Says he'll pay me up front for the place, too.
    ~ SetSpeaker(Speaker.Misra)
     Roy's trying to push you out?
    ~ SetSpeaker(Speaker.Irene)
     I wouldn't say "push"; 
     He's just trying to help.
     I'm not the only person he's made this offer too.
     He bought out the Diner, I'm pretty sure.
    ~ SetSpeaker(Speaker.Misra)
     Why would he do that? 
     That's just hurting the town more.
     Causing more people to leave.
    ~ SetSpeaker(Speaker.Irene)
     I couldn't say.
    ~ DiscoverClue(closed_shops_irene)
    // Add to Synthesis: the Town of KR
    -> DONE
    
}



