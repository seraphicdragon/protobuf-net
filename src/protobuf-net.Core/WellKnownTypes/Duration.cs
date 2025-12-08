using ProtoBuf.Internal;
using ProtoBuf.Serializers;
using ProtoBuf.WellKnownTypes;
using System;
using System.Runtime.InteropServices;


namespace ProtoBuf.WellKnownTypes
{
    /// <summary>
    /// A Duration represents a signed, fixed-length span of time represented
    /// as a count of seconds and fractions of seconds at nanosecond
    /// resolution. It is independent of any calendar and concepts like "day"
    /// or "month". It is related to Timestamp in that the difference between
    /// two Timestamp values is a Duration and it can be added or subtracted
    /// from a Timestamp. 
    /// </summary>
    [ProtoContract(Name = ".google.protobuf.Duration", Serializer = typeof(PrimaryTypeProvider), Origin = "google/protobuf/duration.proto")]
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Duration
    {
        /// <summary>
        /// Signed seconds of the span of time.
        /// </summary>
        [ProtoMember(1, Name = "seconds", DataFormat = DataFormat.Default)]
        public long Seconds { get; }

        /// <summary>
        /// Signed fractions of a second at nanosecond resolution of the span of time.
        /// </summary>
        [ProtoMember(2, Name = "nanos", DataFormat = DataFormat.Default)]
        public int Nanoseconds { get; }

        /// <summary>Creates a new Duration with the supplied values</summary>
        public Duration(long seconds, int nanoseconds)
        {
            Seconds = seconds;
            Nanoseconds = nanoseconds;
        }

        /// <summary>Converts a TimeSpan to a Duration</summary>
        public Duration(TimeSpan value) : this(value.Ticks) { }

        internal Duration(long ticks)
        {
            Seconds = PrimaryTypeProvider.ToDurationSeconds(ticks, out var nanoseconds, false);
            Nanoseconds = nanoseconds;
        }

        /// <summary>Converts a Duration to a TimeSpan</summary>
        public TimeSpan AsTimeSpan() => TimeSpan.FromTicks(ToTicks());

        internal long ToTicks() => PrimaryTypeProvider.ToTicks(Seconds, Nanoseconds);

        /// <summary>Converts a Duration to a TimeSpan</summary>
        public static implicit operator TimeSpan(Duration value) => value.AsTimeSpan();
        /// <summary>Converts a TimeSpan to a Duration</summary>
        public static implicit operator Duration(TimeSpan value) => new Duration(value);

        /// <summary>
        /// Applies .proto rules to ensure that this value is in the expected ranges
        /// </summary>
        public Duration Normalize()
        {
            var seconds = Seconds;
            var nanos = Nanoseconds;
            PrimaryTypeProvider.NormalizeSecondsNanoseconds(ref seconds, ref nanos, false);
            return new Duration(seconds, nanos);
        }
    }
}
