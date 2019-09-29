using System;

namespace UndefinableOfT.Tests
{
    public class JsonSerializationTestClass
    {
        public string NullString { get; set; }
        public string StringWithValue { get; set; } = "hello";
        public Undefinable<int?> OptionalIntThatIsUndefinedAndExplicitlyNotSet { get; set; }
        public Undefinable<int?> OptionalIntThatIsUndefined { get; set; } = Undefinable<int?>.Undefined;
        public Undefinable<int?> OptionalIntThatIsNull { get; set; } = new Undefinable<int?>(null);
        public Undefinable<int?> OptionalIntThatHasAValue { get; set; } = new Undefinable<int?>(1234);
        public Undefinable<string> OptionalStringThatIsUndefinedAndExplicitlyNotSet { get; set; }
        public Undefinable<string> OptionalStringThatIsUndefined { get; set; } = Undefinable<string>.Undefined;
        public Undefinable<string> OptionalStringThatIsNull { get; set; } = new Undefinable<string>(null);
        public Undefinable<string> OptionalStringThatHasAValue { get; set; } = new Undefinable<string>("hello");
    }
}