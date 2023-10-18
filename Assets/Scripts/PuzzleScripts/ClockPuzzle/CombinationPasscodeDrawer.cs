using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CombinationPasscode))]
public class CombinationPasscodeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int labelWidth = 50; // adjust as needed
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        int slotWidth = (int)(position.width - labelWidth) / 4;

        // Define the rectangles for each slot
        Rect slot1 = new Rect(position.x, position.y, slotWidth, position.height);
        Rect slot2 = new Rect(slot1.xMax, position.y, slotWidth, position.height);
        Rect slot3 = new Rect(slot2.xMax, position.y, slotWidth, position.height);
        Rect slot4 = new Rect(slot3.xMax, position.y, slotWidth, position.height);

        // Retrieve properties
        SerializedProperty digit1 = property.FindPropertyRelative("digit1");
        SerializedProperty digit2 = property.FindPropertyRelative("digit2");
        SerializedProperty digit3 = property.FindPropertyRelative("digit3");
        SerializedProperty digit4 = property.FindPropertyRelative("digit4");

        // Draw fields and clamp values to range
        digit1.intValue = Mathf.Clamp(EditorGUI.IntField(slot1, digit1.intValue), 0, 9);
        digit2.intValue = Mathf.Clamp(EditorGUI.IntField(slot2, digit2.intValue), 0, 9);
        digit3.intValue = Mathf.Clamp(EditorGUI.IntField(slot3, digit3.intValue), 0, 9);
        digit4.intValue = Mathf.Clamp(EditorGUI.IntField(slot4, digit4.intValue), 0, 9);


        EditorGUI.EndProperty();
    }

}
