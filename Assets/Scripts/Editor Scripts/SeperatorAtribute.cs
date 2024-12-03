using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, Inherited = true)]
public class SeperatorAttribute : PropertyAttribute
{
    public readonly float Height;
    public readonly float Spacing;
    public readonly EColors _Color;

    public SeperatorAttribute()
    {
        Height = 1.0f;
        Spacing = 10.0f;
        _Color = EColors.Gray;
    }

    public SeperatorAttribute(float _height = 1, float _spacing = 10, EColors _color = EColors.White)
    {
        Height = _height;
        Spacing = _spacing;
        _Color = _color;
    }

}