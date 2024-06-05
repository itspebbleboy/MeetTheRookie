=== scene4_2 ===
# Location: IdaHome and Goods, General Store
    * [talk_to_roy] -> Roy_Dialogue  
    * [store_window] -> store_window
    * [merch_shirt] -> merch_shirt
    * [merch_sticker] -> merch_sticker
    * [merch_pamphlet] -> pamphlet
        ->DONE
= pamphlet
    ~ DiscoverClue(merch_pamphlet)
    ~ SetSpeaker(Speaker.Lupe)
    Wow, it's a pamphlet w/ loads of info!
   -> DONE
= roy
    -> Roy_Dialogue
= store_window
    -> Roy_Dialogue.window
    
= merch_shirt
    -> Roy_Dialogue.merch_shirt
    
= merch_sticker
    -> Roy_Dialogue.merch_sticker

= front_door
    ~ SetSpeaker(Speaker.Lupe)
    Thanks again.
    ~ SetSpeaker(Speaker.Misra)
    Bye, Roy!
    ~ SetSpeaker(Speaker.Roy_Rodgerson)
     Take care!
        # SCENE CHANGE
        -> DONE
