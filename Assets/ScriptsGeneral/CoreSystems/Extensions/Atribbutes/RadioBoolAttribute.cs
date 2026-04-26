using System;
using UnityEditor;
using UnityEngine;

namespace CoreSystems.Extensions.Attributes
{
    /// <summary>
    /// How to use:
    /// Colocar antes de uma das bools, e indicar o nome da sua contraparte
    /// Ex.:
    /// [RadioBool("bool2")]
    /// bool bool1;
    /// bool bool2;
    /// </summary>

    [AttributeUsage(AttributeTargets.Field)]
    public class RadioBoolAttribute : PropertyAttribute
    {
        public string counterpart;
        public byte flag = 0;

        public RadioBoolAttribute(string counterpart)
        {
            this.counterpart = counterpart;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RadioBoolAttribute))]
    public class RadioBoolDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            RadioBoolAttribute radioBoolAttribute = attribute as RadioBoolAttribute;
            SerializedProperty counterpatProperty = property.serializedObject.FindProperty(radioBoolAttribute.counterpart);

            if (counterpatProperty == null || counterpatProperty.propertyType != SerializedPropertyType.Boolean)
            {
                Debug.LogWarning("RadioBool attribute: Invalid boolean property name.");
                return;
            }

            bool bool1 = property.boolValue;
            bool bool2 = counterpatProperty.boolValue;

            if (!bool1 && !bool2)
            {
                radioBoolAttribute.flag = 0;
            }
            else if (bool1 && !bool2)
            {
                radioBoolAttribute.flag = 1;
            }
            else if (!bool1 && bool2)
            {
                radioBoolAttribute.flag = 2;
            }
            else
            {
                if (radioBoolAttribute.flag == 1)
                {
                    property.boolValue = false;
                    radioBoolAttribute.flag = 2;
                }
                else
                {
                    counterpatProperty.boolValue = false;
                    radioBoolAttribute.flag = 1;
                }
            }
        }
    }
#endif
}