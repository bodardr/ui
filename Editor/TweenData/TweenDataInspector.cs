using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bodardr.UI
{
    [CustomPropertyDrawer(typeof(TweenData), true)]
    public class TweenDataInspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Packages/com.bodardr.ui/Editor/TweenData/TweenData.uxml");
            var ui = asset.Instantiate();

            var style = ui.style;

            style.flexShrink = 1;
            style.flexGrow = 0;
            style.marginLeft = style.marginRight = 4;
            style.marginBottom = style.marginTop = 8;

            ui.BindProperty(property);
            ui.Query<Label>().First().text = property.displayName;

            var moveType = ui.Query<EnumField>("moveType").First();
            var loopType = ui.Query<EnumField>("loopType").First();

            var tweenType = ui.Query<EnumField>("tweenType").First();
            tweenType.RegisterValueChangedCallback(val =>
            {
                moveType.style.display =
                    new StyleEnum<DisplayStyle>(
                        (TransformationType)(val.newValue ?? val.previousValue) == TransformationType.Move
                            ? DisplayStyle.Flex
                            : DisplayStyle.None);
                tweenType.value = val.newValue ?? val.previousValue;
            });

            var loops = ui.Query<FloatField>("loops").First();
            loops.RegisterValueChangedCallback(val =>
            {
                loopType.style.display =
                    new StyleEnum<DisplayStyle>(val.newValue != 0 ? DisplayStyle.Flex : DisplayStyle.None);
                loops.value = val.newValue;
            });
            return ui;
        }
    }
}