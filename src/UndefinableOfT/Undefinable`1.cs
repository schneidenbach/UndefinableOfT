using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UndefinableOfT;

// ReSharper disable PossibleInvalidOperationException
// ReSharper disable NonReadonlyMemberInGetHashCode

// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    /// Represents values that can be either undefined or an instance of the type (or null).
    /// </summary>
    [JsonConverter(typeof(UndefinableJsonConverter))]
    public struct Undefinable<T>
    {
        /// <summary>
        /// Returns the default value for this struct, which represents an undefined value.
        /// </summary>
        public static Undefinable<T> Undefined => default;

        private T _value;
        private bool _isDefined;

        /// <summary>
        /// Instantiates the Undefined class with a value.
        /// </summary>
        /// <param name="value"></param>
        public Undefinable(T value)
        {
            _isDefined = true;
            _value = value;
        }

        /// <summary>
        /// True if a value has been set, even if it is null.
        /// </summary>
        public bool IsDefined
        {
            get { return _isDefined; }
        }

        /// <summary>
        /// Gets the value of the current <see cref="Undefinable{T}"/>. 
        /// </summary>
        public T Value
        {
            get
            {
                if (_isDefined)
                {
                    return _value;
                }

                throw new InvalidOperationException("The value is undefined.");
            }
        }

        /// <summary>
        /// Conversion from value type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Undefinable<T>(T value)
        {
            return new Undefinable<T>
            {
                _isDefined = true,
                _value = value
            };
        }

        
        /// <summary>
        /// Used to unwrap the value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator T(Undefinable<T> value)
        {
            return value.Value;
        }
        

        /// <summary>
        /// Gets the wrapped value or the default of T.
        /// </summary>
        /// <returns></returns>
        public T GetValueOrDefault()
        {
            return _isDefined ? _value : default;
        }

        public T GetValueOrDefault(T defaultValue)
        {
            return _isDefined ? _value : defaultValue;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="other"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="other">Another object to compare to. </param>
        public override bool Equals(object other)
        {
            if (other == null)
            {
                return this._value == null;
            }
            
            if (other is Undefinable<T> anotherUndefined)
            {
                if (!this._isDefined && !anotherUndefined._isDefined)
                {
                    return true;
                }
            }

            var otherType = other.GetType();
            if (otherType.IsGenericType && otherType.GetGenericTypeDefinition() == typeof(Undefinable<>))
            {
                //special case for types where one may be nullable and the other not
                //e.g. we want Undefined<int?> and Undefined<int> to use the same equality comparison
                var otherTypeArgument = otherType.GetGenericArguments().Single();
                var thisTypeArgument = typeof(T);

                if (TypeExtensions.DoTypesResolveToSameCoreType(otherTypeArgument, thisTypeArgument))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    if (!this._isDefined && !(bool) otherType.GetField(nameof(_isDefined),
                                BindingFlags.Instance | BindingFlags.NonPublic)
                            .GetValue(other))
                    {
                        return true;
                    }
                    
                    // ReSharper disable once PossibleNullReferenceException
                    var otherValue = otherType
                        .GetField(nameof(_value), BindingFlags.Instance | BindingFlags.NonPublic).GetValue(other);

                    if (otherValue == null && _value == null)
                    {
                        return true;
                    }
                    return _value.Equals(otherValue);
                }
            }

            //special case for structs - otherwise it would be equal since value would be set to something
            if (!typeof(T).IsClass)
            {
                if (!_isDefined)
                {
                    return false;
                }
            }
            
            return _value.Equals(other);
        }

        /// <summary>
        /// Implementation of the equals operator.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool operator ==(Undefinable<T> t1, Undefinable<T> t2)
        {
            // undefined equals undefined
            if (!t1._isDefined && !t2.IsDefined) return true;

            // undefined != everything else
            if (t1._isDefined ^ t2._isDefined) return false;

            if (t1._value == null && t2._value == null)
            {
                return true;
            }

            if (t1._value == null)
            {
                return t2._value.Equals(t1._value);
            }

            // if both are values, compare them
            return t1._value.Equals(t2._value);
        }

        /// <summary>
        /// Implementation of the equals operator.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool operator ==(Undefinable<T> t1, T t2)
        {
            if (!t1._isDefined)
            {
                return false;
            }

            return t1.Equals(t2);
        }

        /// <summary>
        /// Implementation of the inequality operator.
        /// </summary>
        /// <returns></returns>
        public static bool operator !=(Undefinable<T> t1, T t2)
        {
            return !(t1 == t2);
        }

        /// <summary>
        /// Implementation of the equals operator.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool operator ==(T t2, Undefinable<T> t1)
        {
            if (!t1._isDefined)
            {
                return false;
            }

            return t1.Equals(t2);
        }

        /// <summary>
        /// Implementation of the inequality operator.
        /// </summary>
        /// <returns></returns>
        public static bool operator !=(T t2, Undefinable<T> t1)
        {
            return !(t1 == t2);
        }

        /// <summary>
        /// Implementation of the inequality operator.
        /// </summary>
        /// <returns></returns>
        public static bool operator !=(Undefinable<T> t1, Undefinable<T> t2)
        {
            return !(t1 == t2);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            if (!_isDefined)
            {
                return -1;
            }
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns a text representation of
        /// the value, or an empty string if no value
        /// is present.
        /// </summary>
        public override string ToString()
        {
            return _value?.ToString() ?? string.Empty;
        }

    }
}