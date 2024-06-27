using UnityEditor;
using UnityEngine;

namespace CollectItem.Editor
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;  // GUIを無効にしてReadOnlyにする
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;   // GUIを元に戻す
        }
    }
}