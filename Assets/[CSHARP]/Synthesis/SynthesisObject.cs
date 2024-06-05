using UnityEngine.UIElements;

[UxmlElement]
public partial class SynthesisObject : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SynthesisObject> { }

    public Label noteHeader;
    
    public SynthesisObject() {
        AddToClassList("synthesis-object");

        noteHeader = new Label();
        noteHeader.AddToClassList("label");
        Add(noteHeader);
    }
}
