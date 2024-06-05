using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Darklight.UnityExt.UXML
{
    public interface ISelectableUIElement<TElement> where TElement : VisualElement, new()
    {
        TElement Element { get; }
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
    // Base class for controlled elements that encapsulates common functionalities for UXML elements.
    [UxmlElement]
    public partial class SelectableVisualElement<TElement> : VisualElement, ISelectableUIElement<TElement> where TElement : VisualElement, new()
    {
        // Publicly accessible element instance.
        public TElement Element { get; protected set; }
        public Rect Rect => Element.worldBound;
        public Vector2 CenterPosition => Element.worldBound.center;
        public event Action OnSelect;
        public event Action OnClick;
        public SelectableVisualElement()
        {
            Element = this as TElement;
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


}
