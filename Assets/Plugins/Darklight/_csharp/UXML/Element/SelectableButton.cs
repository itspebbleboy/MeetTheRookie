using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.UXML.Element
{
    #region ---- [[ SELECTABLE BUTTON ]] ----
    // Specific implementation for a Button element.
    [UxmlElement]
    public partial class SelectableButton : SelectableVisualElement<Button>
    {
        public class SelectableButtonFactory : UxmlFactory<SelectableButton> { }

        [UxmlAttribute]
        public string text
        {
            get { return Element.text; }
            set { Element.text = value; }
        }

        public SelectableButton()
        {
            Element = (Button)this;

            if (Element == null)
            {
                Element = new Button();
                Add(Element);
            }
            Element.text = "selectable-button";
            Element.clickable.clicked += ClickAction;
        }

        private void ClickAction()
        {
            Click();
            Element.clickable.clicked -= ClickAction;
        }
    }
    #endregion

    #region ---- [[ SELECTABLE SCENE CHANGE BUTTON ]] ----
    [UxmlElement]
    public partial class SelectableSceneChangeButton : SelectableButton
    {
        public class SelectableSceneChangeButtonFactory : UxmlFactory<SelectableSceneChangeButton> { }

        [UxmlAttribute]
        public SceneAsset scene;
        public SelectableSceneChangeButton()
        {
            OnClick += ChangeScene;
        }

        private void ChangeScene()
        {
            if (scene != null)
            {
                SceneManager.LoadScene(scene.name);
                OnClick -= ChangeScene; // Ensure this is only called once
            }
        }
    }
    #endregion
}