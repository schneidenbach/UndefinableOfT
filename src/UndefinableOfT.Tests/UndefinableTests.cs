using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace UndefinableOfT.Tests
{
    [TestFixture]
    public class UndefinableTests
    {
        [Test]
        public void Can_cast_to_proper_unwrapped_value()
        {
            var val = (int) new Undefinable<int>(0);
        }
        
        [Test]
        public void Undefined_int_that_is_undefined_resolves_undefined_values_correctly()
        {
            Undefinable<int> i = default;
            Assert.That(i.IsDefined, Is.False);
            Assert.Throws<InvalidOperationException>(() => { var a = i.Value; });
        }

        [Test]
        public void Undefinable_nullable_int_that_is_empty_is_defined()
        {
            Undefinable<int?> i = null;
            Assert.That(i.IsDefined, Is.True);
            Assert.That(i.Value, Is.Null);
        }

        [Test]
        public void Undefinable_int_with_a_value_resolves_equality_correctly()
        {
            Undefinable<int> i = 10;
            Assert.That(i.IsDefined, Is.True);
            Assert.That(i.Value, Is.EqualTo(10));
            Assert.That(10, Is.EqualTo(i));
            Assert.That(i, Is.EqualTo(new Undefinable<int>(10)));
            Assert.That(i, Is.EqualTo(new Undefinable<int?>(10)));
        }

        [Test]
        public void Undefinable_nullable_int_with_a_value_resolves_equality_correctly()
        {
            Undefinable<int?> i = 10;
            Assert.That(i.IsDefined, Is.True);
            Assert.That(i.Value, Is.EqualTo(10));
            Assert.That(10, Is.EqualTo(i));
            Assert.That(i, Is.EqualTo(new Undefinable<int?>(10)));
            Assert.That(i, Is.EqualTo(new Undefinable<int>(10)));
        }

        public class Tmp
        {
            public Undefinable<int> A { get; set; }
            public Undefinable<int> B { get; set; }
        }

        [Test]
        [TestCase("{A:10}", true, true, 10, false, false, null)]
        [TestCase("{A:10, B:null}", true, true, 10, true, false, null)]
        [TestCase("{}", false, false, null, false, false, null)]
        [TestCase("{A:10, B:5}", true, true, 10, true, true, 5)]
        [TestCase("{B:null}", false, false, null, true, false, null)]
        public void TestDeserialization(string json,
            bool aIsSet, bool aHasValue, int? aValue,
            bool bIsSet, bool bHasValue, int? bValue)
        {
            var o = JsonConvert.DeserializeObject<Tmp>(json);

            Assert.That(o.A.IsDefined, Is.EqualTo(aIsSet));
            if (aValue != null)
                Assert.That(o.A.Value, Is.EqualTo(aValue));

            Assert.That(o.B.IsDefined, Is.EqualTo(bIsSet));
            if (bValue != null)
                Assert.That(o.B.Value, Is.EqualTo(bValue));
        }

        public static object[] SerializationTests = {
            new object[] {new Undefinable<int>(1), "1"},
            new object[] {new Undefinable<int?>(null), "null"},
            new object[] {Undefinable<int>.Undefined, "null"},
            new object[] {new Undefinable<int>(10), "10"},
            new object[] {new Undefinable<int>(-10000), "-10000"},
            new object[] {Undefinable<bool>.Undefined, "null"},
            new object[] {new Undefinable<bool?>(null), "null"},
            new object[] {new Undefinable<bool>(true), "true"},
            new object[] {new Undefinable<bool>(false), "false"},
            new object[] {Undefinable<double>.Undefined, "null"},
            new object[] {new Undefinable<double?>(null), "null"},
            new object[] {new Undefinable<double>(0), "0.0"},
            new object[] {new Undefinable<double>(-10.53), "-10.53"},
        };

        [Test]
        [TestCaseSource(nameof(SerializationTests))]
        public void TestSerialization(Object v, string expectedJson)
        {
            var json = JsonConvert.SerializeObject(v);

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new UndefinableContractResolver()
        };

        
        public static object[] UndefinedSerializationTests = {
            new object[] {new Undefinable<int>(10), Undefinable<int>.Undefined, "{\"A\":10}"},
            new object[] {Undefinable<int>.Undefined, new Undefinable<int>(0),  "{\"B\":0}"},
            new object[] {Undefinable<int>.Undefined, Undefinable<int>.Undefined, "{}"}
        };
        
        public static object[] NullableUndefinedSerializationTests = {
            new object[] {new Undefinable<int?>(null), Undefinable<int?>.Undefined, "{\"A\":null}"}
        };

        [Test]
        [TestCaseSource(nameof(UndefinedSerializationTests))]
        public void TestUndefinedSerialization(Undefinable<int> a, Undefinable<int> b, String expectedJson)
        {
            var json = JsonConvert.SerializeObject(new { A = a, B = b }, settings);

            Assert.That(json, Is.EqualTo(expectedJson));
        }
        
        [Test]
        [TestCaseSource(nameof(NullableUndefinedSerializationTests))]
        public void TestUndefinedSerializationUsingNullables(Undefinable<int?> a, Undefinable<int?> b, String expectedJson)
        {
            var json = JsonConvert.SerializeObject(new { A = a, B = b }, settings);

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        public static object[] EqualityCases = {
            new object[] {Undefinable<int>.Undefined, Undefinable<int>.Undefined, true},
            new object[] {new Undefinable<int>(0), Undefinable<int>.Undefined, false},
            new object[] {new Undefinable<int>(0), new Undefinable<int>(10), false},
            new object[] {new Undefinable<int>(-1), new Undefinable<int>(-1), true},
            new object[] {new Undefinable<int>(0), new Undefinable<int>(0), true}
        };

        public static object[] NullableEqualityCases = {
            new object[] {Undefinable<int?>.Undefined, new Undefinable<int?>(null), false},
            new object[] {new Undefinable<int?>(null), new Undefinable<int?>(null), true},
            new object[] {new Undefinable<int?>(null), new Undefinable<int?>(10), false}
        };

        public static object[] NullableAndNonNullableEqualityCases = {
            new object[] {Undefinable<int?>.Undefined, new Undefinable<int>(0), false},
            new object[] {new Undefinable<int?>(null), new Undefinable<int>(0), false},
            new object[] {new Undefinable<int?>(123), new Undefinable<int>(123), true},
            new object[] {Undefinable<int?>.Undefined, Undefinable<int>.Undefined, true},
        };

        public static object[] NonNullableAndNullableEqualityCases = {
            new object[] {Undefinable<int>.Undefined, new Undefinable<int?>(null), false},
            new object[] {new Undefinable<int>(0), new Undefinable<int?>(null), false},
            new object[] {Undefinable<int>.Undefined, Undefinable<int?>.Undefined, true},
            new object[] {new Undefinable<int>(0), new Undefinable<int?>(0), true},
            new object[] {new Undefinable<int>(123), new Undefinable<int?>(123), true}
        };
       
        /*
        [Test]
        [TestCaseSource(nameof(NonNullableAndNullableEqualityCases))]
        public void TestNonNullableAndNullableEquality(Undefinable<int> a, Undefinable<int?> b, bool expected)
        {
            var actual = a == b;
            Assert.That(actual, Is.EqualTo(expected));
        }
        
        [Test]
        [TestCaseSource(nameof(NullableAndNonNullableEqualityCases))]
        public void TestNullableAndNonNullableEquality(Undefinable<int?> a, Undefinable<int> b, bool expected)
        {
            var actual = a == b;
            Assert.That(actual, Is.EqualTo(expected));
        }
        */

        [Test]
        [TestCaseSource(nameof(EqualityCases))]
        public void TestEquality(Undefinable<int> a, Undefinable<int> b, bool expected)
        {
            Assert.That(a == b, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(EqualityCases))]
        public void TestInequality(Undefinable<int> a, Undefinable<int> b, bool expected)
        {
            Assert.That(a != b, Is.EqualTo(!expected));
        }
        
        [Test]
        [TestCaseSource(nameof(NullableEqualityCases))]
        public void TestNullableEquality(Undefinable<int?> a, Undefinable<int?> b, bool expected)
        {
            Assert.That(a == b, Is.EqualTo(expected));
        }

        [Test]
        [TestCaseSource(nameof(NullableEqualityCases))]
        public void TestNullableInequality(Undefinable<int?> a, Undefinable<int?> b, bool expected)
        {
            Assert.That(a != b, Is.EqualTo(!expected));
        }
    }
}
