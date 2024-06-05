using UnityEngine;
using UnityEngine.UIElements;
using Darklight.UnityExt.UXML;

[UxmlElement]
public partial class SpeechBubble : ControlledLabel
{
    public new class UxmlFactory : UxmlFactory<SpeechBubble> { }

    [UxmlAttribute]
    public Sprite bubbleSprite
    {
        get { return this.style.backgroundImage.value.sprite; }
        set { SetBackgroundSprite(value); }
    }

    public SpeechBubble()
    {
        SetBackgroundSprite(bubbleSprite);
        fontSize = GetDynamicFontSize();
    }

    public void SetBackgroundSprite(Sprite sprite)
    {
        if (sprite == null)
            this.style.backgroundImage = null;
        else
            this.style.backgroundImage = new StyleBackground(sprite);
    }
}