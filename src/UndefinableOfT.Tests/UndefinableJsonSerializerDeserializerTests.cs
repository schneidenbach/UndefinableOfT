using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace UndefinableOfT.Tests
{
    public class UndefinableJsonSerializerDeserializerTests
    {
        [Test]
        public void Serializes_correctly()
        {
            var testClass = new JsonSerializationTestClass();

            var jsonString = JsonConvert.SerializeObject(testClass, new JsonSerializerSettings
            {
                ContractResolver = new UndefinableContractResolver()
            });

            var jobject = JObject.Parse(jsonString);
            Assert.IsTrue(jobject.ContainsKey(nameof(JsonSerializationTestClass.NullString)));
            Assert.IsTrue(jobject.ContainsKey(nameof(JsonSerializationTestClass.StringWithValue)));
            Assert.IsFalse(jobject.ContainsKey(nameof(JsonSerializationTestClass.OptionalIntThatIsUndefinedAndExplicitlyNotSet)));
            Assert.IsFalse(jobject.ContainsKey(nameof(JsonSerializationTestClass.OptionalIntThatIsUndefined)));
            Assert.IsTrue(jobject.ContainsKey(nameof(JsonSerializationTestClass.OptionalIntThatIsNull)));
            Assert.IsTrue(jobject.ContainsKey(nameof(JsonSerializationTestClass.OptionalIntThatHasAValue)));
            Assert.IsFalse(jobject.ContainsKey(nameof(JsonSerializationTestClass.OptionalStringThatIsUndefinedAndExplicitlyNotSet)));
            Assert.IsFalse(jobject.ContainsKey(nameof(JsonSerializationTestClass.OptionalStringThatIsUndefined)));
            Assert.IsTrue(jobject.ContainsKey(nameof(JsonSerializationTestClass.OptionalStringThatIsNull)));
            Assert.IsTrue(jobject.ContainsKey(nameof(JsonSerializationTestClass.OptionalStringThatHasAValue)));

            Console.Write(jobject.ToString(Formatting.Indented));
        }

        [Test]
        public void Deserializes_correctly()
        {
            var json = @"
{
    ""NullString"": null,
    ""StringWithValue"": ""hi"",
    ""OptionalIntThatIsNull"": null,
    ""OptionalIntThatHasAValue"": 123,
    ""OptionalStringThatIsNull"": null,
    ""OptionalStringThatHasAValue"": ""hi again"",
    ""IntThatHasAValue"": 123
}";
            var deserialized = JsonConvert.DeserializeObject<JsonDeserializationTestClass>(json);
            Assert.AreEqual(deserialized.NullString, null);
            Assert.AreEqual(deserialized.StringWithValue, "hi");
            Assert.AreEqual(deserialized.OptionalIntThatIsUndefinedAndExplicitlyNotSet, Undefinable<int?>.Undefined);
            Assert.AreEqual(deserialized.OptionalIntThatIsUndefined, Undefinable<int?>.Undefined);
            Assert.AreEqual(deserialized.OptionalIntThatIsNull, new Undefinable<int?>(null));
            Assert.AreEqual(deserialized.OptionalIntThatHasAValue, new Undefinable<int?>(123));
            Assert.AreEqual(deserialized.OptionalStringThatIsUndefinedAndExplicitlyNotSet, Undefinable<string>.Undefined);
            Assert.AreEqual(deserialized.OptionalStringThatIsUndefined, Undefinable<string>.Undefined);
            Assert.AreEqual(deserialized.OptionalStringThatIsNull, new Undefinable<string>(null));
            Assert.AreEqual(deserialized.OptionalStringThatHasAValue, new Undefinable<string>("hi again"));
            Assert.AreEqual(deserialized.IntThatIsUndefined, Undefinable<int>.Undefined);
            Assert.AreNotEqual(deserialized.IntThatIsUndefined, 0);
            Assert.AreEqual(deserialized.IntThatIsUndefinedAndExplicitlyNotSet, Undefinable<int>.Undefined);
            Assert.AreNotEqual(deserialized.IntThatIsUndefinedAndExplicitlyNotSet, 0);
            Assert.AreEqual(deserialized.IntThatHasAValue, 123);
        }
    }
}