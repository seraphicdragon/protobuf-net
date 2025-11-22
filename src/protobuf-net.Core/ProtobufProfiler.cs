using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
    public static class ProtobufProfiler
    {
        public static bool IsProfiling = false;
        public static Action<string> beginProfile;
        public static Action endProfile;

        private static Dictionary<Type, string> profilerWriteStrings = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerWriteMapStrings = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerInteriorWriteMapStrings = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerInteriorWriteMapStrings2 = new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerInteriorWriteObject= new Dictionary<Type, string>(512);

        private static Dictionary<Type, string> profilerWriteFieldHeader = new Dictionary<Type, string>(512);


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
            }
            return null;
        }

        internal static void BeginProfile(Type type, ProfilerType profilerType)
        {
            if (ProtobufProfiler.IsProfiling && ProtobufProfiler.beginProfile != null)
                ProtobufProfiler.beginProfile(ProtobufProfiler.GetProfilerString(type, profilerType));
        }

        internal static void EndProfiler()
        {
            if (ProtobufProfiler.IsProfiling && ProtobufProfiler.endProfile != null)
                ProtobufProfiler.endProfile();
        }
    }
}
