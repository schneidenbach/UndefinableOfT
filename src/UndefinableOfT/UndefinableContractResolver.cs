using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace System
{
    /// <summary>
    /// A Contract Resolver implementation, that will not
    /// serialize undefined Settable properties.
    /// </summary>
    public class UndefinableContractResolver : DefaultContractResolver
    {
        public new static readonly UndefinableContractResolver Instance = new UndefinableContractResolver();

        private static readonly MethodInfo ShouldSerializeSettableBuilderMethodInfo = typeof(UndefinableContractResolver)
            .GetMethod(nameof(ShouldSerializeSettableBuilder), BindingFlags.Static | BindingFlags.NonPublic);

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var settableTypeParameter = ResolveOptionTypeParameter(property.PropertyType);
            
            if (settableTypeParameter != null)
            {
                property.ShouldSerialize = MakePredicateForSettableType(settableTypeParameter, property);
            }

            return property;
        }

        private static Type ResolveOptionTypeParameter(Type settableType)
        {
            var toCheck = settableType;
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (typeof(Undefinable<>) == cur)
                {
                    return toCheck.GetGenericArguments().Single();
                }
                toCheck = toCheck.BaseType;
            }

            return null;
            // throw new InvalidOperationException("The provided type " + settableType.FullName + " does not inherit from Settable<T>");
        }

        /// <summary>
        /// For each base type, a ShouldSerialize handler is
        /// generated. Using reflection at each serialization would be
        /// slow, so we use MethodInfo.MakeGenericMethod once and then
        /// return the cached Predicate.
        /// </summary>
        /// <param name="baseType">
        /// The type wrapped in a Settable&lt;&gt; property.
        /// </param>
        /// <param name="property"></param>
        /// <returns></returns>
        private Predicate<object> MakePredicateForSettableType(Type baseType, JsonProperty property)
        {
            var typedMethod = ShouldSerializeSettableBuilderMethodInfo.MakeGenericMethod(baseType);
 
            return (Predicate<object>)typedMethod.Invoke(null, new object[] { property });
        }

        /// <summary>
        /// Strongly typed Predicate factory, to be invoked only once for
        /// each type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Predicate<object> ShouldSerializeSettableBuilder<T>(JsonProperty property)
        {
            return o =>
            {
                var v = property.ValueProvider.GetValue(o);
                return ((Undefinable<T>) v).IsDefined;
            };
        }
    }
}