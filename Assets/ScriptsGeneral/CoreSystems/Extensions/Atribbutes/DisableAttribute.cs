using UnityEditor;
using UnityEngine;

namespace CoreSystems.Extensions.Attributes
{
    public class DisableAttribute : PropertyAttribute
    {
        public string conditionPropertyName1;
        public string conditionPropertyName2;
        public string selectedOperator;
        public bool simpleOperation;

        public DisableAttribute(string conditionPropertyName)
        {
            this.conditionPropertyName1 = conditionPropertyName;
            this.selectedOperator = "NONE";
            this.simpleOperation = false;
        }

        public DisableAttribute()
        {
            this.simpleOperation = true;
        }

        public DisableAttribute(string conditionPropertyName1, string conditionPropertyName2, string selectedOperator = "AND")
        {
            this.conditionPropertyName1 = conditionPropertyName1;
            this.conditionPropertyName2 = conditionPropertyName2;
            this.selectedOperator = selectedOperator;
            this.simpleOperation = false;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisableAttribute))]
    public class DisableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DisableAttribute disableAttribute = attribute as DisableAttribute;
            bool isSimpleOperation = disableAttribute.simpleOperation;
            string selectedOperator = disableAttribute.selectedOperator;

            SerializedProperty conditionProperty1 = property.serializedObject.FindProperty(disableAttribute.conditionPropertyName1);
            SerializedProperty conditionProperty2 = property.serializedObject.FindProperty(disableAttribute.conditionPropertyName2);

            bool disable = false;

            if (isSimpleOperation)
            {
                disable = true;
            }
            else
            {
                if (conditionProperty1 != null && conditionProperty1.propertyType == SerializedPropertyType.Boolean)
                {
                    if (selectedOperator.ToUpper() == "NONE")
                    {
                        disable = !conditionProperty1.boolValue;
                    }
                    else if (conditionProperty2 != null && conditionProperty2.propertyType == SerializedPropertyType.Boolean)
                    {
                        if (selectedOperator.ToUpper() == "AND")
                        {
                            disable = !conditionProperty1.boolValue && !conditionProperty2.boolValue;
                        }
                        else if (selectedOperator.ToUpper() == "OR")
                        {
                            disable = !conditionProperty1.boolValue || !conditionProperty2.boolValue;
                        }
                    }
                }
            }

            EditorGUI.BeginDisabledGroup(disable);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
#endif
}
