using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EColors
{
    Clear,
    White,
    Black,
    Gray,
    Red,
    Pink,
    Orange,
    Yellow,
    Green,
    Blue,
    Indigo,
    Violet
}

public static class EditorColorExtensions
{
    public static Color GetColor(this EColors color)
    {
        switch (color)
        {
            case EColors.Clear: return new Color32(0, 0, 0, 0);
            case EColors.White: return new Color32(255, 255, 255, 255);
            case EColors.Black: return new Color32(0, 0, 0, 255);
            case EColors.Gray: return new Color32(128, 128, 128, 255);
            case EColors.Red: return new Color32(255, 0, 63, 255);
            case EColors.Pink: return new Color32(255, 152, 203, 255);
            case EColors.Orange: return new Color32(255, 128, 0, 255);
            case EColors.Yellow: return new Color32(255, 211, 0, 255);
            case EColors.Green: return new Color32(98, 211, 79, 255);
            case EColors.Blue: return new Color32(0, 135, 189, 255);
            case EColors.Indigo: return new Color32(75, 0, 130, 255);
            case EColors.Violet: return new Color32(128, 0, 255, 255);
            default: return new Color32(255, 255, 255, 255);
        }
    }
}