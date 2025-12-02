using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProtoBuf
{
    internal enum ProfilerType
    {
        Unknown,
        Read,
        Write,
        WriteObject,
        WriteMap,
        InteriorWriteMap,
        InteriorWriteMap2,
        WriteFieldHeader,
        StartSubItem,
        WriteAny,
        OuterStartSubItem,
        GetField,
        ValueTypeBoxing,
        SerializeImpl,
        TailWrite,
        PrintFieldName,
    }


    /// <summary>
    /// Official pair structure for the Secret Engine Framework.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    [Serializable]
    public struct Pair<T, K> : IEquatable<Pair<T, K>>
    {
        [ProtoMember(1)]
        public T first;
        [ProtoMember(2)]
        public K second;

        public static void UnityRegister()
        {

        }

      

        private static readonly EqualityComparer<T> firstComparer = EqualityComparer<T>.Default;
        private static readonly EqualityComparer<K> secondComparer = EqualityComparer<K>.Default;

        /// <summary>
        /// Constructs a new PairStruct object.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public Pair(T first, K second)
        {
            this.first = first;
            this.second = second;
        }

        /// <summary>
        /// Constructs a new PairStruct object.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public Pair(in T first, in K second)
        {
            this.first = first;
            this.second = second;
        }

        public override readonly int GetHashCode()
        {
            return firstComparer.GetHashCode(first) +
               secondComparer.GetHashCode(second);
        }

        public override readonly bool Equals(object obj)
        {
            throw new NotSupportedException("There shall be no boxing performed on this class.");
        }

        public readonly bool Equals(Pair<T, K> other)
        {
            return firstComparer.Equals(other.first, first) &&
                secondComparer.Equals(other.second, second);
        }

        public readonly bool Equals(in Pair<T, K> other)
        {
            return firstComparer.Equals(other.first, first) &&
               secondComparer.Equals(other.second, second);
        }

        public override readonly string ToString()
        {
            return "First=> " + first.ToString() + " Second=> " + second.ToString();
        }

        public static bool operator ==(Pair<T, K> first, Pair<T, K> second)
        {
            return firstComparer.Equals(first.first, second.first) &&
               secondComparer.Equals(first.second, second.second);
        }

        public static bool operator !=(Pair<T, K> first, Pair<T, K> second)
        {
            return !firstComparer.Equals(first.first, second.first) ||
               !secondComparer.Equals(first.second, second.second);
        }

        public T First { get => first; set => first = value; }
        public K Second { get => second; set => second = value; }
    }

    public static class ProtobufProfiler
    {
        public static bool IsProfiling = false;
        public static bool ProfileFieldLevel = false;

        public static bool IsDebugging = false;

        public static Action<string> beginProfile;
        public static Action endProfile;

        public static Action<string> loggingAction;

        private static Dictionary<Type, string> profilerWriteStrings = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerWriteMapStrings = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerInteriorWriteMapStrings = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerInteriorWriteMapStrings2 = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerInteriorWriteObject= new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerWriteFieldHeader = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerGetFieldHeader = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerValueTypeBoxingStrings = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerSerializeImplStrings = new Dictionary<Type, string>(512);
        private static Dictionary<Type, string> profilerTailWriteStrings = new Dictionary<Type, string>(512);
        private static Dictionary<FieldInfo, string> profilerFieldInfoStrings = new Dictionary<FieldInfo, string>(512);
        private static Dictionary<Pair<Type, string>, string> loggings = new Dictionary<Pair<Type,string>, string>(512);
        private static string GetProfilerString(Type type, ProfilerType write)
        {
            switch (write)
            {
                case ProfilerType.Write:
                    if (!profilerWriteStrings.TryGetValue(type, out string str))
                        profilerWriteStrings.Add(type, str = type.Name + ".Write");
                    return str;
                case ProfilerType.WriteMap:
                    if (!profilerWriteMapStrings.TryGetValue(type, out str))
                        profilerWriteMapStrings.Add(type, str = type.Name + ".WriteMap");
                    return str;
                case ProfilerType.InteriorWriteMap:
                    if (!profilerInteriorWriteMapStrings.TryGetValue(type, out str))
                        profilerInteriorWriteMapStrings.Add(type, str = type.Name + ".InteriorWriteMap");
                    return str;
                case ProfilerType.InteriorWriteMap2:
                    if (!profilerInteriorWriteMapStrings2.TryGetValue(type, out str))
                        profilerInteriorWriteMapStrings2.Add(type, str = type.Name + ".InteriorWriteMap2");
                    return str;
                case ProfilerType.WriteObject:
                    if (!profilerInteriorWriteObject.TryGetValue(type, out str))
                        profilerInteriorWriteObject.Add(type, str = type.Name + ".WriteObject");
                    return str;
                case ProfilerType.WriteFieldHeader:
                    if (!profilerWriteFieldHeader.TryGetValue(type, out str))
                        profilerWriteFieldHeader.Add(type, str = type.Name + ".WriteHeader");
                    return str;
                case ProfilerType.StartSubItem:
                    return "ProtoWriter.StartSubItem";
                case ProfilerType.WriteAny:
                    return "WriteAny";
                case ProfilerType.OuterStartSubItem:
                    return "OuterStartSubItem";
                case ProfilerType.GetField:
                    if (!profilerGetFieldHeader.TryGetValue(type, out str))
                        profilerGetFieldHeader.Add(type, str = type.Name + ".GetField");
                    return str;
                case ProfilerType.ValueTypeBoxing:
                    if (!profilerValueTypeBoxingStrings.TryGetValue(type, out str))
                        profilerValueTypeBoxingStrings.Add(type, str = type.Name + ".ValueTypeBoxing");
                    return str;
                case ProfilerType.SerializeImpl:
                    if (!profilerSerializeImplStrings.TryGetValue(type, out str))
                        profilerSerializeImplStrings.Add(type, str = type.Name + ".SerializeImpl");
                    return str;
                case ProfilerType.TailWrite:
                    if (!profilerTailWriteStrings.TryGetValue(type, out str))
                        profilerTailWriteStrings.Add(type, str = type.Name + ".TailWrite");
                    return str;
            }
            return null;
        }

        private static string GetProfilerString(FieldInfo fieldInfo)
        {
            if(!profilerFieldInfoStrings.TryGetValue(fieldInfo, out string value))
            {
                value = fieldInfo.DeclaringType.Name + "." + fieldInfo.Name;
                profilerFieldInfoStrings[fieldInfo] = value;
            }
            return value;

        }

        internal static void BeginProfile(string profileName)
        {
            if (ProtobufProfiler.IsProfiling && ProtobufProfiler.beginProfile != null)
                ProtobufProfiler.beginProfile(profileName);
        }

        internal static void BeginProfile(Type type, ProfilerType profilerType)
        {
            if (ProtobufProfiler.IsProfiling && ProtobufProfiler.beginProfile != null)
                ProtobufProfiler.beginProfile(ProtobufProfiler.GetProfilerString(type, profilerType));
        }

        internal static void BeginProfile(FieldInfo field)
        {
            if (ProtobufProfiler.IsProfiling && ProtobufProfiler.beginProfile != null)
                ProtobufProfiler.beginProfile(ProtobufProfiler.GetProfilerString(field));
        }

        internal static void EndProfiler()
        {
            if (ProtobufProfiler.IsProfiling && ProtobufProfiler.endProfile != null)
                ProtobufProfiler.endProfile();
        }

        internal static void Log(Type type, string log)
        {
            if (ProtobufProfiler.IsDebugging && ProtobufProfiler.loggingAction != null)
            {
                Pair<Type, string> pair = new Pair<Type, string>(type, log);
                if (!loggings.TryGetValue(pair, out string value))
                    loggings.Add(pair, value = (type.Name + log));
                loggingAction(value);
            }
            
        }
    }
}
