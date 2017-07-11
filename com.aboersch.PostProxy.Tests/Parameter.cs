using System;
using System.Collections.Generic;

namespace com.aboersch.PostProxy.Tests
{
    class Parameter : IEquatable<Parameter>
    {
        public Parameter(string key, object rawValue, string stringValue)
        {
            Key = key;
            RawValue = rawValue;
            StringValue = stringValue;
        }

        public Parameter(string key, object rawValue) : this(key, rawValue, rawValue.ToString()) { }

        public string Key { get; }
        public string StringValue { get; }
        public object RawValue { get; }
        public string QueryParameter => $"{Key}={StringValue}";

        public override bool Equals(object obj)
        {
            return Equals(obj as Parameter);
        }

        public bool Equals(Parameter other)
        {
            return other != null &&
                   Key == other.Key &&
                   StringValue == other.StringValue;
        }

        public override int GetHashCode()
        {
            var hashCode = 206514262;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Key);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(StringValue);
            return hashCode;
        }

        public static bool operator ==(Parameter parameter1, Parameter parameter2)
        {
            return EqualityComparer<Parameter>.Default.Equals(parameter1, parameter2);
        }

        public static bool operator !=(Parameter parameter1, Parameter parameter2)
        {
            return !(parameter1 == parameter2);
        }
    }
}
