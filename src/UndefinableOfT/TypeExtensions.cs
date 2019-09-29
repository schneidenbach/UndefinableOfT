using System;

namespace UndefinableOfT
{
    internal static class TypeExtensions
    {
        public static bool DoTypesResolveToSameCoreType(Type type1, Type type2)
        {
            if (type1.IsNullableType())
            {
                type1 = type1.GetCoreType();
            }

            if (type2.IsNullableType())
            {
                type2 = type2.GetCoreType();
            }

            return type1 == type2;
        }
        
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        public static Type GetCoreType(this Type type)
        {
            if (type.IsNullableType())
            {
                type = type.GenericTypeArguments[0];
            }

            if (type.IsEnum)
            {
                return Enum.GetUnderlyingType(type);
            }

            return type;
        }
    }
}