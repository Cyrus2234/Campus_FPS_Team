using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SeperatorAttribute))]
public class SeperatorDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        SeperatorAttribute attr = attribute as SeperatorAttribute;
        Rect rect = new Rect(position.xMin, position.yMin + attr.Spacing, position.width, attr.Height);

        EditorGUI.DrawRect(rect, attr._Color.GetColor());
    }

    public override float GetHeight()
    {
        SeperatorAttribute attr = attribute as SeperatorAttribute;
        // top spacer -- lines height -- bottom spacer
        return attr.Spacing + attr.Height + attr.Spacing;
    }
}