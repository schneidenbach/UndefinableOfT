using System;

namespace UndefinableOfT.Tests
{
    public class JsonDeserializationTestClass
    {
        public string NullString { get; set; }
        public string StringWithValue { get; set; }
        public Undefinable<int?> OptionalIntThatIsUndefinedAndExplicitlyNotSet { get; set; }
        public Undefinable<int?> OptionalIntThatIsUndefined { get; set; }
        public Undefinable<int?> OptionalIntThatIsNull { get; set; }
        public Undefinable<int?> OptionalIntThatHasAValue { get; set; }
        
        public Undefinable<int> IntThatIsUndefinedAndExplicitlyNotSet { get; set; }
        public Undefinable<int> IntThatIsUndefined { get; set; }
        public Undefinable<int> IntThatHasAValue { get; set; }
        
        public Undefinable<string> OptionalStringThatIsUndefinedAndExplicitlyNotSet { get; set; }
        public Undefinable<string> OptionalStringThatIsUndefined { get; set; }
        public Undefinable<string> OptionalStringThatIsNull { get; set; }
        public Undefinable<string> OptionalStringThatHasAValue { get; set; }
    }
}