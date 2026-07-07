#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpriteSearchAttribute))]
public class SpriteSearchDrawer : PropertyDrawer
{
    private const float ButtonWidth = 80f;
    private const float Space = 4f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        SpriteSearchAttribute spriteSearch = (SpriteSearchAttribute)attribute;

        Rect fieldRect = new Rect(
            position.x,
            position.y,
            position.width - ButtonWidth - Space,
            position.height
        );

        Rect buttonRect = new Rect(
            fieldRect.xMax + Space,
            position.y,
            ButtonWidth,
            position.height
        );

        EditorGUI.PropertyField(fieldRect, property, label);

        if (GUI.Button(buttonRect, "âÊëúåüçı"))
        {
            SpriteSearchWindow.Open(
                spriteSearch.FolderPath,
                property.serializedObject.targetObject,
                property.propertyPath
            );
        }
    }
}
#endif