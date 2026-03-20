using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.scr.EditorPanel
{
    public static class UITypeMapper
    {
        public static readonly Dictionary<Type, Type> ValueTypeToFieldType = new Dictionary<Type, Type>
        {
            { typeof(int), typeof(IntegerField) },
            { typeof(long), typeof(LongField) },
            { typeof(float), typeof(FloatField) },
            { typeof(double), typeof(DoubleField) },
            { typeof(bool), typeof(Toggle) },
            { typeof(string), typeof(TextField) },
            { typeof(Vector2), typeof(Vector2Field) },
            { typeof(Vector2Int), typeof(Vector2IntField) },
            { typeof(Vector3), typeof(Vector3Field) },
            { typeof(Vector3Int), typeof(Vector3IntField) },
            { typeof(Vector4), typeof(Vector4Field) },
            { typeof(Rect), typeof(RectField) },
            { typeof(RectInt), typeof(RectIntField) },
            { typeof(Bounds), typeof(BoundsField) },
            { typeof(BoundsInt), typeof(BoundsIntField) },
            { typeof(Color), typeof(ColorField) },
            { typeof(AnimationCurve), typeof(CurveField) },
            { typeof(Gradient), typeof(GradientField) },
            { typeof(UnityEngine.Object), typeof(ObjectField) },
            { typeof(Enum), typeof(EnumField) },
            { typeof(LayerMask), typeof(LayerField) }
        };

        public static Type GetFieldTypeForValueType(Type valueType)
        {
            if (valueType == null) return typeof(TextField);

            // 处理可空类型
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                valueType = Nullable.GetUnderlyingType(valueType);
            }

            // 直接匹配
            if (ValueTypeToFieldType.TryGetValue(valueType, out Type fieldType))
                return fieldType;

            // 枚举处理
            if (valueType.IsEnum)
            {
                return valueType.IsDefined(typeof(FlagsAttribute), false)
                    ? typeof(EnumFlagsField)
                    : typeof(EnumField);
            }

            // UnityEngine.Object派生类型
            if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
                return typeof(ObjectField);

            // 处理数组和列表
            if (valueType.IsArray || (valueType.IsGenericType &&
                (valueType.GetGenericTypeDefinition() == typeof(List<>) ||
                 valueType.GetGenericTypeDefinition() == typeof(IList<>))))
            {
                return typeof(TextField); // 暂时用TextField显示，后续可以扩展
            }

            // 处理复杂类型（自定义类）
            if (valueType.IsClass && valueType != typeof(string))
            {
                return typeof(Foldout); // 使用折叠面板显示复杂类型
            }

            return typeof(TextField);
        }
    }
}
