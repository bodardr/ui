using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


[Serializable]
public class InteractionElement
{
    public Interaction interaction;
    public int weight;
}

[CustomPropertyDrawer(typeof(InteractionElement))]
public class InteractionElementDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var interactionField = new PropertyField(property.FindPropertyRelative("interaction"));
        var weightField = new PropertyField(property.FindPropertyRelative("weight"));

        container.Add(interactionField);
        container.Add(weightField);

        return container;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var interactionRect = new Rect(position.x, position.y, position.width - 35, position.height);
        var weightRect = new Rect(position.x + position.width - 30, position.y, 30, position.height);

        EditorGUI.PropertyField(interactionRect, property.FindPropertyRelative("interaction"), GUIContent.none);
        EditorGUI.PropertyField(weightRect, property.FindPropertyRelative("weight"), GUIContent.none);
        EditorGUI.indentLevel = indentLevel;

        EditorGUI.EndProperty();
    }
}