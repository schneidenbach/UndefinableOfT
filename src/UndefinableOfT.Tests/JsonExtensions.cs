using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace UndefinableOfT.Tests
{
    public static class JsonExtensions
    {
        public static bool ContainsKey(this JObject jObject, string key)
        {
            return ((IDictionary<string, JToken>) jObject).ContainsKey(key);
        }
    }
}