using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bodardr.UI
{
    [CustomPropertyDrawer(typeof(DotweenAnim))]
    public class DotweenAnimInspector : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 2 + 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, null, property);

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            position = EditorGUI.PrefixLabel(position, label);
            EditorGUIUtility.labelWidth = labelWidth;

            position.height = 18;

            var totalWidth = position.width;
            var baseX = position.x;

            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var transitionRect = position;
            transitionRect.width = totalWidth - 240;
            position.x += transitionRect.width + 5;

            var valueRect = position;
            valueRect.width = 180;
            position.x += valueRect.width - 35;

            var timeRect = position;
            timeRect.width = 90;
            position.x += timeRect.width + 10;

            position.x = baseX;
            position.y += 18;

            var easeRect = position;
            easeRect.width = (totalWidth - 85) / 2;
            position.x += easeRect.width + 5;

            var loopsProperty = property.FindPropertyRelative(nameof(DotweenAnim.loops));
            var loopsRect = position;

            if (Mathf.Approximately(loopsProperty.intValue, 0))
                loopsRect.width = easeRect.width + 75;
            else
                loopsRect.width = 70;

            position.x += loopsRect.width + 10;

            var loopsTypeRect = position;
            loopsTypeRect.width = easeRect.width;

            EditorGUI.PropertyField(transitionRect,
                property.FindPropertyRelative(nameof(DotweenAnim.transformationType)),
                GUIContent.none);

            EditorGUIUtility.labelWidth = 35;
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(nameof(DotweenAnim.value)),
                new GUIContent("Value:") {tooltip = "Values from and to. X : FROM, Y : TO"});

            EditorGUIUtility.labelWidth = 55;
            EditorGUI.PropertyField(timeRect, property.FindPropertyRelative(nameof(DotweenAnim.duration)),
                new GUIContent("Time (s):"));

            EditorGUI.PropertyField(easeRect, property.FindPropertyRelative(nameof(DotweenAnim.ease)), GUIContent.none);
            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUIUtility.labelWidth = 40;
            EditorGUI.PropertyField(loopsRect, loopsProperty);

            if (!Mathf.Approximately(loopsProperty.intValue, 0))
                EditorGUI.PropertyField(loopsTypeRect, property.FindPropertyRelative(nameof(DotweenAnim.loopType)),
                    GUIContent.none);

            EditorGUI.indentLevel = indentLevel;
            EditorGUI.EndProperty();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var transformationType =
                new PropertyField(property.FindPropertyRelative(nameof(DotweenAnim.transformationType)));
            var duration = new PropertyField(property.FindPropertyRelative(nameof(DotweenAnim.duration)));
            var ease = new PropertyField(property.FindPropertyRelative(nameof(DotweenAnim.ease)));

            container.Add(transformationType);
            container.Add(duration);
            container.Add(ease);

            return container;
        }
    }
}