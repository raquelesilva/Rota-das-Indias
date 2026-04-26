using System;
using UnityEditor;
using UnityEngine;

namespace CoreSystems.Extensions.Attributes
{
    /// <summary>
    /// How to use:
    /// [HorizontalLine]
    /// [HorizontalLine(color: height: 5)]
    /// [HorizontalLine(color: EColor.Cyan)]
    /// [HorizontalLine(color: EColor.Cyan, height: 5)]
    /// </summary>

    [AttributeUsage(AttributeTargets.Field)]
    public class HorizontalLineAttribute : PropertyAttribute
    {
        public const float DefaultHeight = 1.0f;
        public const EColor DefaultColor = EColor.Gray;

        public float Height { get; private set; }
        public EColor Color { get; private set; }

        public HorizontalLineAttribute(float height = DefaultHeight, EColor color = DefaultColor)
        {
            Height = height;
            Color = color;
        }
    }

    public enum EColor
    {
        Black,
        White,
        Gray,
        Red,
        Green,
        Blue,
        Cyan,
        Magenta,
        Yellow,
        Clear
    }

    public static class EColorExtentions
    {
        public static Color GetColor(this EColor color)
        {
            return color switch
            {
                EColor.Black => Color.black,
                EColor.White => Color.white,
                EColor.Gray => Color.gray,
                EColor.Red => Color.red,
                EColor.Green => Color.green,
                EColor.Blue => Color.blue,
                EColor.Cyan => Color.cyan,
                EColor.Magenta => Color.magenta,
                EColor.Yellow => Color.yellow,
                EColor.Clear => Color.clear,
                _ => Color.gray,
            };
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
    public class HorizontalLineDecoratorDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            HorizontalLineAttribute lineAttr = (HorizontalLineAttribute)attribute;
            return EditorGUIUtility.singleLineHeight + lineAttr.Height;
        }

        public override void OnGUI(Rect position)
        {
            Rect rect = EditorGUI.IndentedRect(position);
            rect.y += EditorGUIUtility.singleLineHeight / 3.0f;
            HorizontalLineAttribute lineAttr = (HorizontalLineAttribute)attribute;
            rect.height = lineAttr.Height;
            EditorGUI.DrawRect(rect, lineAttr.Color.GetColor());
        }
    }
#endif
}