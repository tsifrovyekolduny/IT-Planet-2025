#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RenameAttribute))]
public class RenameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var renameAttr = (RenameAttribute)attribute;
        label.text = renameAttr.DisplayName;
        EditorGUI.PropertyField(position, property, label);
    }
}
#endif