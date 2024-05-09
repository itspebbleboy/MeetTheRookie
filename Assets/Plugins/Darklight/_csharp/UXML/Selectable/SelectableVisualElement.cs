using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.Selectable
{
    interface ISelectable
    {
        public event Action OnSelect;
        public event Action OnClick;
        void Select();
        void Click();
        void Deselect();
    }

    #region ---- [[ SELECTABLE VISUAL ELEMENT ]] ----
    /// <summary>
    /// Base class for controlled elements that encapsulates common functionalities for UXML elements.
    /// </summary>
    /// <typeparam name="TElement">The Type of the </typeparam>
    [UxmlElement]
    public partial class SelectableVisualElement<TElement> : VisualElement, ISelectable where TElement : VisualElement, new()
    {
        // Publicly accessible element instance.
        public TElement Element { get; private set; }
        public Rect Rect => Element.worldBound;
        public Vector2 CenterPosition => Element.worldBound.center;
        public event Action OnSelect;
        public event Action OnClick;
        public SelectableVisualElement()
        {
            Element = new TElement();
            this.Add(Element);
            this.focusable = true;
        }

        public virtual void Select()
        {
            this.AddToClassList("selected");
            this.Focus();
            OnSelect?.Invoke();
        }

        public virtual void Click()
        {
            this.RemoveFromClassList("selected");
            this.AddToClassList("clicked");
            OnClick?.Invoke();
        }

        public virtual void Deselect()
        {
            this.RemoveFromClassList("selected");
            this.RemoveFromClassList("clicked");
        }

        /// <summary>
        /// Allows implicit conversion from SelectableElement to the generic class, typeof(TElement) : VisualElement.
        /// </summary>
        /// <param name="wrapper"></param>
        public static implicit operator TElement(SelectableVisualElement<TElement> wrapper)
        {
            return wrapper.Element;
        }
    }
    #endregion

    #region ---- [[ SELECTABLE BUTTON ]] ----
    // Specific implementation for a Button element.
    [UxmlElement]
    public partial class SelectableButton : SelectableVisualElement<Button>, ISelectable
    {
        public Button Button => Element as Button;

        [UxmlAttribute]
        public string Text
        {
            get => Button.text;
            set => Button.text = value;
        }

        public SelectableButton()
        {
            Text = "selectable-button";
            Button.clickable.clicked += ClickAction;
        }

        private void ClickAction()
        {
            Click();
            Button.clickable.clicked -= ClickAction;
        }
    }
    #endregion

    #region ---- [[ SELECTABLE SCENE CHANGE BUTTON ]] ----
    [UxmlElement]
    public partial class SelectableSceneChangeButton : SelectableButton
    {
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
