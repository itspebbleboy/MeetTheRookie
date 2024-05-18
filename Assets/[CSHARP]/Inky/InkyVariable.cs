using UnityEngine;
using Ink.Runtime;
using Darklight.UnityExt.Editor;

[System.Serializable]
public class InkyVariable
{
    [SerializeField, ShowOnly] private string _key;
    // Hide in Inspector because Unity cannot display 'object' directly
    [SerializeField, HideInInspector] private object _value;
    // For inspector display and possibly for serialization   
    [SerializeField, ShowOnly] private string _valueAsString;

    public string Key { get => _key; set => _key = value; }
    public object Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateValueAsString();
        }
    }

    // Constructor for general use
    public InkyVariable(string key, object value)
    {
        _key = key;
        Value = value;
    }

    // Updates the string representation for the Inspector
    private void UpdateValueAsString()
    {
        if (_value is InkList inkList)
        {
            _valueAsString = inkList.ToString().Trim();  // Custom handling for InkList
        }
        else
        {
            _valueAsString = _value?.ToString() ?? "null";  // General handling for other types
        }
    }

    public override string ToString()
    {
        return $"{_key} = {_valueAsString}";
    }
}
