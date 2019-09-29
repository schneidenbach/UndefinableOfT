using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace UndefinableOfT
{
    public class UndefinableJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Undefinable<>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            var innerType = objectType.GetGenericArguments()?.FirstOrDefault() ?? throw new InvalidOperationException("No inner type found.");
            var noneMethod = MakeStaticGenericMethodInfo(nameof(None), innerType);
            var someMethod = MakeStaticGenericMethodInfo(nameof(Some), innerType);

            if (reader.TokenType == JsonToken.Null)
            {
                return someMethod.Invoke(null, new object[] { null });
            }

            var innerValue = serializer.Deserialize(reader, innerType);

            if (innerValue == null)
            {
                return noneMethod.Invoke(null, new object[] { });
            }

            return someMethod.Invoke(noneMethod, new[] { innerValue });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var innerType = value.GetType()?.GetGenericArguments()?.FirstOrDefault() ?? throw new InvalidOperationException("No inner type found.");
            var hasValueMethod = MakeStaticGenericMethodInfo(nameof(HasValue), innerType);
            var getValueMethod = MakeStaticGenericMethodInfo(nameof(GetValue), innerType);

            var hasValue = (bool)hasValueMethod.Invoke(null, new[] { value });

            if (!hasValue)
            {
                writer.WriteNull();
                return;
            }

            Nullable<int> asdf = null;
            var innerValue = getValueMethod.Invoke(null, new[] { value });
            serializer.Serialize(writer, innerValue);
        }

        private MethodInfo MakeStaticGenericMethodInfo(string name, params Type[] typeArguments)
        {
            return GetType()
                ?.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)
                ?.MakeGenericMethod(typeArguments)
                ?? throw new InvalidOperationException($"Could not make generic MethodInfo for method '{name}'.");
        }

        private static bool HasValue<T>(Undefinable<T> option) => option.IsDefined;
        private static T GetValue<T>(Undefinable<T> option) => option.GetValueOrDefault(default(T));
        private static Undefinable<T> None<T>() => Undefinable<T>.Undefined;
        private static Undefinable<T> Some<T>(T value) => new Undefinable<T>(value);
    }
}