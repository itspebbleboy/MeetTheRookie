using UnityEngine;
using UnityEngine.UIElements;

using Darklight.UnityExt.Utility;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Darklight.UnityExt.UXML
{
    [UxmlElement]
    public partial class ControlledLabel : VisualElement
    {
        public class ControlledSizeLabelFactory : UxmlFactory<ControlledLabel> { }

        private VisualElement _container;
        private Label _label;
        private float _rollingTextPercentValue;
        private int _currentIndex;

        [UxmlAttribute, ShowOnly]
        public Vector2 screenSize
        {
            get { return ScreenInfoUtility.ScreenSize; }
            set { }
        }

        [UxmlAttribute]
        public int fontSize
        {
            get
            {
                return (int)_label.style.fontSize.value.value;
            }
            set { _label.style.fontSize = value; }
        }

        [UxmlAttribute, MinMaxSlider(12, 128)]
        public Vector2Int fontSizeRange;

        [UxmlAttribute, TextArea(3, 10)]
        public string fullText =
            "New UXML Element Controlled Label. This is a test string to see how the text wraps around the bubble. Hopefully it works well.";

        [UxmlAttribute, ShowOnly]
        public string text
        {
            get { return _label.text; }
            set { _label.text = value; }
        }

        [UxmlAttribute, Range(0, 1)]
        public float rollingTextPercentage
        {
            get { return _rollingTextPercentValue; }
            set
            {
                _rollingTextPercentValue = value;
                _currentIndex = Mathf.FloorToInt(fullText.Length * _rollingTextPercentValue);
                SetTextToIndex(_currentIndex);
            }
        }

        [UxmlAttribute]
        public TextAnchor textAlign
        {
            get { return _label.style.unityTextAlign.value;}
            set { _label.style.unityTextAlign = value;}
        }

        public ControlledLabel()
        {
            _container = new VisualElement();
            _container.style.overflow = Overflow.Hidden;
            _container.style.flexWrap = Wrap.Wrap;

            _label = new Label();
            _label.style.alignSelf = Align.Auto;
            _label.style.fontSize = GetDynamicFontSize();

            _container.Add(_label);
            Add(_container);
        }

        public void Initialize(string fullText)
        {
            this.fullText = fullText;
            SetTextToIndex(0);
        }

        public int GetDynamicFontSize()
        {
            screenSize = ScreenInfoUtility.ScreenSize;

            // Get the font size based on the screen size
            int fontSizeMin = (int)fontSizeRange.x;
            int fontSizeMax = (int)fontSizeRange.y;

            // Divide the max font size by the aspect ratio
            float fontSizeByAspectRatio = fontSizeMax / ScreenInfoUtility.ScreenAspectRatio;

            // Clamp the font size to the set range
            int result = (int)Mathf.Clamp(fontSizeByAspectRatio, fontSizeRange.x, fontSizeRange.y);
            return result;
        }

        public void RollingTextStep()
        {
            SetTextToIndex(_currentIndex + 1);
        }

        void SetTextToIndex(int index)
        {
            _currentIndex = Mathf.Min(index, fullText.Length);
            this.text = fullText.Substring(0, _currentIndex);
        }
    }
}
