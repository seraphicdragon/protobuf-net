using ProtoBuf.Meta;
using ProtoBuf.Serializers;
using ProtoBuf.WellKnownTypes;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ProtoBuf.Internal
{

    /*public class PrimaryTypeProviderInt : IMeasuringSerializer<int>, IValueChecker<int>
    {
        public int Read(ref ProtoReader.State state, int value) => state.ReadInt32();
        public void Write(ref ProtoWriter.State state, int value) => state.WriteInt32(value);
        public SerializerFeatures Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        public int Measure(ISerializationContext context, WireType wireType, int value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureInt32(value),
                WireType.SignedVarint => ProtoWriter.MeasureUInt32(ProtoWriter.Zig(value)),
                _ => -1,
            };
        public bool IsNull(int value) => false;

        public bool HasNonTrivialValue(int value) => value != 0;

        public static readonly PrimaryTypeProviderInt Instance = new PrimaryTypeProviderInt();
    }*/

    public class PrimaryTypeProvider :
        IMeasuringSerializer<string>,
 
        IMeasuringSerializer<long>,
        IMeasuringSerializer<bool>,
        IMeasuringSerializer<float>,
        IMeasuringSerializer<double>,
        IMeasuringSerializer<byte[]>,
        IMeasuringSerializer<ArraySegment<byte>>,
        IMeasuringSerializer<Memory<byte>>,
        IMeasuringSerializer<ReadOnlyMemory<byte>>,
        IMeasuringSerializer<byte>,
        IMeasuringSerializer<ushort>,
        IMeasuringSerializer<uint>,
        IMeasuringSerializer<ulong>,
        IMeasuringSerializer<sbyte>,
        IMeasuringSerializer<short>,
        IMeasuringSerializer<char>,
        IMeasuringSerializer<Uri>,
        IMeasuringSerializer<Type>,
        IMeasuringSerializer<IntPtr>,
        IMeasuringSerializer<UIntPtr>,

        IFactory<string>,
        IFactory<byte[]>,

        IMeasuringSerializer<int?>,
        IMeasuringSerializer<long?>,
        IMeasuringSerializer<bool?>,
        IMeasuringSerializer<float?>,
        IMeasuringSerializer<double?>,
        IMeasuringSerializer<byte?>,
        IMeasuringSerializer<ushort?>,
        IMeasuringSerializer<uint?>,
        IMeasuringSerializer<ulong?>,
        IMeasuringSerializer<sbyte?>,
        IMeasuringSerializer<short?>,
        IMeasuringSerializer<char?>,
        IMeasuringSerializer<IntPtr?>,
        IMeasuringSerializer<UIntPtr?>,

        IValueChecker<string>,
        IValueChecker<long>,
        IValueChecker<bool>,
        IValueChecker<float>,
        IValueChecker<double>,
        IValueChecker<byte[]>,
        IValueChecker<byte>,
        IValueChecker<ushort>,
        IValueChecker<uint>,
        IValueChecker<ulong>,
        IValueChecker<sbyte>,
        IValueChecker<short>,
        IValueChecker<char>,
        IValueChecker<IntPtr>,
        IValueChecker<UIntPtr>,
        IValueChecker<Uri>,
        IValueChecker<Type>,

        IValueChecker<int?>,
        IValueChecker<long?>,
        IValueChecker<bool?>,
        IValueChecker<float?>,
        IValueChecker<double?>,
        IValueChecker<byte?>,
        IValueChecker<ushort?>,
        IValueChecker<uint?>,
        IValueChecker<ulong?>,
        IValueChecker<sbyte?>,
        IValueChecker<short?>,
        IValueChecker<char?>,
        IValueChecker<IntPtr?>,
        IValueChecker<UIntPtr?>,
        ISerializer<Guid>, ISerializer<Guid?>,
        ISerializer<PrimaryTypeProvider.ScaledTicks>,
        ISerializer<TimeSpan>, ISerializer<TimeSpan?>,
        ISerializer<DateTime>, ISerializer<DateTime?>,
        ISerializer<decimal>, ISerializer<decimal?>,
        ISerializer<Duration>, ISerializer<Duration?>,
        ISerializer<Empty>, ISerializer<Empty?>,
        ISerializer<Timestamp>, ISerializer<Timestamp?>,
        IMeasuringSerializer<int>, IValueChecker<int>
    {
        public int Read(ref ProtoReader.State state, int value) => state.ReadInt32();
        public void Write(ref ProtoWriter.State state, int value) => state.WriteInt32(value);
        public SerializerFeatures Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        public int Measure(ISerializationContext context, WireType wireType, int value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureInt32(value),
                WireType.SignedVarint => ProtoWriter.MeasureUInt32(ProtoWriter.Zig(value)),
                _ => -1,
            };
        public bool IsNull(int value) => false;

        public bool HasNonTrivialValue(int value) => value != 0;
        SerializerFeatures ISerializer<Timestamp>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessage;
        SerializerFeatures ISerializer<Timestamp?>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessage;
        Timestamp ISerializer<Timestamp>.Read(ref ProtoReader.State state, Timestamp value)
        {
            var duration = new Duration(value.Seconds, value.Nanoseconds);
            duration = ReadDuration(ref state, duration);
            return new Timestamp(duration.Seconds, duration.Nanoseconds);
        }

        internal static Timestamp ReadTimestamp(ref ProtoReader.State state, Timestamp value)
        {
            var duration = new Duration(value.Seconds, value.Nanoseconds);
            duration = ReadDuration(ref state, duration);
            return new Timestamp(duration.Seconds, duration.Nanoseconds);
        }

        void ISerializer<Timestamp>.Write(ref ProtoWriter.State state, Timestamp value)
            => WriteSecondsNanos(ref state, value.Seconds, value.Nanoseconds, true);

        internal static void WriteTimestamp(ref ProtoWriter.State state, Timestamp value)
            => WriteSecondsNanos(ref state, value.Seconds, value.Nanoseconds, true);

        Timestamp? ISerializer<Timestamp?>.Read(ref ProtoReader.State state, Timestamp? value)
            => ((ISerializer<Timestamp>)this).Read(ref state, value.GetValueOrDefault());
        void ISerializer<Timestamp?>.Write(ref ProtoWriter.State state, Timestamp? value)
            => ((ISerializer<Timestamp>)this).Write(ref state, value.Value);

        SerializerFeatures ISerializer<Empty>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessage;
        SerializerFeatures ISerializer<Empty?>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessage;
        Empty ISerializer<Empty>.Read(ref ProtoReader.State state, Empty value)
        {
            state.SkipAllFields();
            return value;
        }

        void ISerializer<Empty>.Write(ref ProtoWriter.State state, Empty value) { }

        Empty? ISerializer<Empty?>.Read(ref ProtoReader.State state, Empty? value)
            => ((ISerializer<Empty>)this).Read(ref state, value.GetValueOrDefault());
        void ISerializer<Empty?>.Write(ref ProtoWriter.State state, Empty? value)
            => ((ISerializer<Empty>)this).Write(ref state, value.Value);
        SerializerFeatures ISerializer<Duration>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessage;
        SerializerFeatures ISerializer<Duration?>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessage;

        Duration? ISerializer<Duration?>.Read(ref ProtoReader.State state, Duration? value)
            => ((ISerializer<Duration>)this).Read(ref state, value.GetValueOrDefault());
        void ISerializer<Duration?>.Write(ref ProtoWriter.State state, Duration? value)
            => ((ISerializer<Duration>)this).Write(ref state, value.Value);

        Duration ISerializer<Duration>.Read(ref ProtoReader.State state, Duration value)
            => ReadDuration(ref state, value);

        internal static Duration ReadDuration(ref ProtoReader.State state, Duration value)
        {
            if (state.WireType == WireType.String && state.RemainingInCurrent >= 20)
            {
                if (TryReadDurationFast(ref state, ref value)) return value;
            }
            return ReadDurationFallback(ref state, value);
        }

        private static bool TryReadDurationFast(ref ProtoReader.State state, ref Duration value)
        {
            int offset = state.OffsetInCurrent;
            var span = state.Span;
            int prefixLength = state.ParseVarintUInt32(span, offset, out var len);
            offset += prefixLength;
            if (len == 0) return true;

            if ((prefixLength + len) > state.RemainingInCurrent) return false; // don't have entire submessage

            if (span[offset] != (1 << 3)) return false; // expected field 1
            var msgOffset = 1 + ProtoReader.State.TryParseUInt64Varint(span, 1 + offset, out var seconds);
            var nanos = value.Nanoseconds;
            if (msgOffset < len)
            {
                if (span[msgOffset++ + offset] != (2 << 3)) return false; // expected field 2
                msgOffset += ProtoReader.State.TryParseUInt64Varint(span, msgOffset + offset, out var tmp);
                nanos = (int)(long)tmp;
            }
            if (msgOffset != len) return false; // expected no more fields
            state.Skip(prefixLength + (int)len);
            state.Advance(prefixLength + len);

            value = new Duration((long)seconds, nanos);
            return true;
        }

        private static Duration ReadDurationFallback(ref ProtoReader.State state, Duration value)
        {
            var seconds = value.Seconds;
            var nanos = value.Nanoseconds;
            int fieldNumber;

            while ((fieldNumber = state.ReadFieldHeader()) > 0)
            {
                switch (fieldNumber)
                {
                    case 1:
                        seconds = state.ReadInt64();
                        break;
                    case 2:
                        nanos = state.ReadInt32();
                        break;
                    default:
                        state.SkipField();
                        break;
                }
            }
            return new Duration(seconds, nanos);
        }

        void ISerializer<Duration>.Write(ref ProtoWriter.State state, Duration value)
            => WriteSecondsNanos(ref state, value.Seconds, value.Nanoseconds, false);

        internal static void WriteDuration(ref ProtoWriter.State state, Duration value)
            => WriteSecondsNanos(ref state, value.Seconds, value.Nanoseconds, false);

        internal static long ToDurationSeconds(long ticks, out int nanos, bool isTimestamp)
        {
            nanos = (int)(((ticks % TimeSpan.TicksPerSecond) * 1000000)
                / TimeSpan.TicksPerMillisecond);
            var seconds = ticks / TimeSpan.TicksPerSecond;
            NormalizeSecondsNanoseconds(ref seconds, ref nanos, isTimestamp);
            return seconds;
        }

        internal static long ToTicks(long seconds, int nanos)
        {
            long ticks = checked((seconds * TimeSpan.TicksPerSecond)
                + (nanos * TimeSpan.TicksPerMillisecond / 1000000));
            return ticks;
        }

        internal static void NormalizeSecondsNanoseconds(ref long seconds, ref int nanos, bool isTimestamp)
        {
            const int SECOND_NANOS = 1000000000;
            // normalize to -999,999,999 to +999,999,999 inclusive
            seconds += nanos / SECOND_NANOS;
            nanos %= SECOND_NANOS;

            if (isTimestamp)
            {
                if (nanos < 0)
                {   // from Timestamp.proto:
                    // "Negative second values with fractions must still have
                    // non -negative nanos values that count forward in time."
                    seconds--;
                    nanos += SECOND_NANOS;
                }
            }
            else
            {
                // from Duration.Proto
                // Durations less than one second are represented with a 0
                // `seconds` field and a positive or negative `nanos` field. For durations
                // of one second or more, a non-zero value for the `nanos` field must be
                // of the same sign as the `seconds` field.

                if (nanos < 0) // and we already know < 1s, because of first lines
                {
                    // can we save space by encoding it as a positive?
                    if (seconds >= 0)
                    {
                        // for 0 and 1, this has the effect of making the nanos +ve, which
                        // is probably cheaper; for > 1, it enforces the "same sign" requirement
                        seconds--;
                        nanos += SECOND_NANOS;
                    }
                }
                if (nanos > 0 && seconds < 0)
                {
                    nanos -= SECOND_NANOS;
                    seconds++;
                }
            }
        }
        private static void WriteSecondsNanos(ref ProtoWriter.State state, long seconds, int nanos, bool isTimestamp)
        {
            NormalizeSecondsNanoseconds(ref seconds, ref nanos, isTimestamp);
            if (seconds != 0)
            {
                state.WriteFieldHeader(1, WireType.Varint);
                state.WriteInt64(seconds);
            }
            if (nanos != 0)
            {
                state.WriteFieldHeader(2, WireType.Varint);
                state.WriteInt32(nanos);
            }
        }

        SerializerFeatures ISerializer<decimal>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessageWrappedAtRoot;
        SerializerFeatures ISerializer<decimal?>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessageWrappedAtRoot;
        private const int FieldDecimalLow = 0x01, FieldDecimalHigh = 0x02, FieldDecimalSignScale = 0x03;

        decimal? ISerializer<decimal?>.Read(ref ProtoReader.State state, decimal? value)
            => ((ISerializer<decimal>)this).Read(ref state, value.GetValueOrDefault());
        void ISerializer<decimal?>.Write(ref ProtoWriter.State state, decimal? value)
            => ((ISerializer<decimal>)this).Write(ref state, value.Value);

        decimal ISerializer<decimal>.Read(ref ProtoReader.State state, decimal value)
        {
            ulong low = 0;
            uint high = 0;
            uint signScale = 0;
            int fieldNumber;
            while ((fieldNumber = state.ReadFieldHeader()) > 0)
            {
                switch (fieldNumber)
                {
                    case FieldDecimalLow: low = state.ReadUInt64(); break;
                    case FieldDecimalHigh: high = state.ReadUInt32(); break;
                    case FieldDecimalSignScale: signScale = state.ReadUInt32(); break;
                    default: state.SkipField(); break;
                }
            }
            int lo = (int)(low & 0xFFFFFFFFL),
               mid = (int)((low >> 32) & 0xFFFFFFFFL),
               hi = (int)high;
            bool isNeg = (signScale & 0x0001) == 0x0001;
            byte scale = (byte)((signScale & 0x01FE) >> 1);
            return new decimal(lo, mid, hi, isNeg, scale);
        }

        void ISerializer<decimal>.Write(ref ProtoWriter.State state, decimal value)
        {
            ulong low;
            uint high, signScale;
            if (s_decimalOptimized) // the JIT should remove the non-preferred implementation, at least on modern runtimes
            {
                var dec = new DecimalAccessor(value);
                ulong a = ((ulong)dec.Mid) << 32, b = ((ulong)dec.Lo) & 0xFFFFFFFFL;
                low = a | b;
                high = (uint)dec.Hi;
                signScale = (uint)(((dec.Flags >> 15) & 0x01FE) | ((dec.Flags >> 31) & 0x0001));
            }
            else
            {
                int[] bits = decimal.GetBits(value);
                ulong a = ((ulong)bits[1]) << 32, b = ((ulong)bits[0]) & 0xFFFFFFFFL;
                low = a | b;
                high = (uint)bits[2];
                signScale = (uint)(((bits[3] >> 15) & 0x01FE) | ((bits[3] >> 31) & 0x0001));
            }

            if (low != 0)
            {
                state.WriteFieldHeader(FieldDecimalLow, WireType.Varint);
                state.WriteUInt64(low);
            }
            if (high != 0)
            {
                state.WriteFieldHeader(FieldDecimalHigh, WireType.Varint);
                state.WriteUInt32(high);
            }
            if (signScale != 0)
            {
                state.WriteFieldHeader(FieldDecimalSignScale, WireType.Varint);
                state.WriteUInt32(signScale);
            }
        }

        private static
#if !DEBUG
            readonly
#endif
            bool s_decimalOptimized = VerifyDecimalLayout();

        internal static bool DecimalOptimized
        {
            get => s_decimalOptimized;
#if DEBUG
            set => s_decimalOptimized = value && VerifyDecimalLayout();
#endif
        }

        private static bool VerifyDecimalLayout()
        {
            try
            {
                // test against example taken from https://docs.microsoft.com/en-us/dotnet/api/system.decimal.getbits?view=netframework-4.8
                //     1.0000000000000000000000000000    001C0000  204FCE5E  3E250261  10000000
                var value = 1.0000000000000000000000000000M;
                var layout = new DecimalAccessor(value);
                if (layout.Lo == 0x10000000
                    & layout.Mid == 0x3E250261
                    & layout.Hi == 0x204FCE5E
                    & layout.Flags == 0x001C0000)
                {
                    // and double-check against GetBits itself
                    var bits = decimal.GetBits(value);
                    if (bits.Length == 4)
                    {
                        return layout.Lo == bits[0]
                            & layout.Mid == bits[1]
                            & layout.Hi == bits[2]
                            & layout.Flags == bits[3];
                    }
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Provides access to the inner fields of a decimal.
        /// Similar to decimal.GetBits(), but faster and avoids the int[] allocation
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private readonly struct DecimalAccessor
        {
            [FieldOffset(0)]
            public readonly int Flags;
            [FieldOffset(4)]
            public readonly int Hi;
            [FieldOffset(8)]
            public readonly int Lo;
            [FieldOffset(12)]
            public readonly int Mid;

            [FieldOffset(0)]
            public readonly decimal Decimal;

            public DecimalAccessor(decimal value)
            {
                this = default;
                Decimal = value;
            }
        }
        SerializerFeatures ISerializer<DateTime>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessageWrappedAtRoot;
        SerializerFeatures ISerializer<DateTime?>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessageWrappedAtRoot;

        SerializerFeatures ISerializer<TimeSpan>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessageWrappedAtRoot;
        SerializerFeatures ISerializer<TimeSpan?>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessageWrappedAtRoot;

        TimeSpan? ISerializer<TimeSpan?>.Read(ref ProtoReader.State state, TimeSpan? value)
            => ((ISerializer<TimeSpan>)this).Read(ref state, value.GetValueOrDefault());
        void ISerializer<TimeSpan?>.Write(ref ProtoWriter.State state, TimeSpan? value)
            => ((ISerializer<TimeSpan>)this).Write(ref state, value.Value);

        DateTime? ISerializer<DateTime?>.Read(ref ProtoReader.State state, DateTime? value)
            => ((ISerializer<DateTime>)this).Read(ref state, value.GetValueOrDefault());
        void ISerializer<DateTime?>.Write(ref ProtoWriter.State state, DateTime? value)
            => ((ISerializer<DateTime>)this).Write(ref state, value.Value);

        TimeSpan ISerializer<TimeSpan>.Read(ref ProtoReader.State state, TimeSpan value)
            => ((ISerializer<ScaledTicks>)this).Read(ref state, default).ToTimeSpan();

        void ISerializer<TimeSpan>.Write(ref ProtoWriter.State state, TimeSpan value)
            => ((ISerializer<ScaledTicks>)this).Write(ref state, new ScaledTicks(value, DateTimeKind.Unspecified));

        DateTime ISerializer<DateTime>.Read(ref ProtoReader.State state, DateTime value)
            => ((ISerializer<ScaledTicks>)this).Read(ref state, default).ToDateTime();

        void ISerializer<DateTime>.Write(ref ProtoWriter.State state, DateTime value)
        {
            var includeKind = state.Model.HasOption(TypeModel.TypeModelOptions.IncludeDateTimeKind);
            ((ISerializer<ScaledTicks>)this).Write(ref state, ScaledTicks.Create(value, includeKind));
        }

        void ISerializer<ScaledTicks>.Write(ref ProtoWriter.State state, ScaledTicks value)
        {
            if (value.Value != 0)
            {
                state.WriteFieldHeader(ScaledTicks.FieldTimeSpanValue, WireType.SignedVarint);
                state.WriteInt64(value.Value);
            }
            if (value.Scale != TimeSpanScale.Days)
            {
                state.WriteFieldHeader(ScaledTicks.FieldTimeSpanScale, WireType.Varint);
                state.WriteInt32((int)value.Scale);
            }
            if (value.Kind != DateTimeKind.Unspecified)
            {
                state.WriteFieldHeader(ScaledTicks.FieldTimeSpanKind, WireType.Varint);
                state.WriteInt32((int)value.Kind);
            }
        }

        SerializerFeatures ISerializer<ScaledTicks>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessage;
        ScaledTicks ISerializer<ScaledTicks>.Read(ref ProtoReader.State state, ScaledTicks _)
        {
            int fieldNumber;
            TimeSpanScale scale = TimeSpanScale.Days;
            long value = 0;
            var kind = DateTimeKind.Unspecified;
            while ((fieldNumber = state.ReadFieldHeader()) > 0)
            {
                switch (fieldNumber)
                {
                    case ScaledTicks.FieldTimeSpanScale:
                        scale = (TimeSpanScale)state.ReadInt32();
                        break;
                    case ScaledTicks.FieldTimeSpanValue:
                        state.Assert(WireType.SignedVarint);
                        value = state.ReadInt64();
                        break;
                    case ScaledTicks.FieldTimeSpanKind:
                        kind = (DateTimeKind)state.ReadInt32();
                        switch (kind)
                        {
                            case DateTimeKind.Unspecified:
                            case DateTimeKind.Utc:
                            case DateTimeKind.Local:
                                break; // fine
                            default:
                                ThrowHelper.ThrowProtoException("Invalid date/time kind: " + kind.ToString());
                                break;
                        }
                        break;
                    default:
                        state.SkipField();
                        break;
                }
            }
            return new ScaledTicks(value, scale, kind);
        }

        [StructLayout(LayoutKind.Auto)]
        [ProtoContract(Name = ".bcl.TimeSpan")]
        internal readonly struct ScaledTicks
        {
            [ProtoMember(1, DataFormat = DataFormat.ZigZag, Name = "value")]
            public long Value { get; }
            [ProtoMember(2, Name = "scale")]
            public TimeSpanScale Scale { get; }
            [ProtoMember(3, Name = "kind")]
            public DateTimeKind Kind { get; }
            public ScaledTicks(long value, TimeSpanScale scale, DateTimeKind kind)
            {
                Value = value;
                Scale = scale;
                Kind = kind;
            }

            public static ScaledTicks Create(DateTime value, bool includeKind)
            {
                if (value == DateTime.MinValue) return new ScaledTicks(-1, TimeSpanScale.MinMax, DateTimeKind.Unspecified);
                if (value == DateTime.MaxValue) return new ScaledTicks(1, TimeSpanScale.MinMax, DateTimeKind.Unspecified);
                var kind = includeKind ? value.Kind : DateTimeKind.Unspecified;
                return new ScaledTicks(value - BclHelpers.EpochOrigin[(int)kind], kind);
            }

            public DateTime ToDateTime()
            {
                long tickDelta;
                switch (Scale)
                {
                    case TimeSpanScale.Days:
                        tickDelta = Value * TimeSpan.TicksPerDay;
                        break;
                    case TimeSpanScale.Hours:
                        tickDelta = Value * TimeSpan.TicksPerHour;
                        break;
                    case TimeSpanScale.Minutes:
                        tickDelta = Value * TimeSpan.TicksPerMinute;
                        break;
                    case TimeSpanScale.Seconds:
                        tickDelta = Value * TimeSpan.TicksPerSecond;
                        break;
                    case TimeSpanScale.Milliseconds:
                        tickDelta = Value * TimeSpan.TicksPerMillisecond;
                        break;
                    case TimeSpanScale.Ticks:
                        tickDelta = Value;
                        break;
                    case TimeSpanScale.MinMax:
                        switch (Value)
                        {
                            case 1: return DateTime.MaxValue;
                            case -1: return DateTime.MinValue;
                            default:
                                ThrowHelper.ThrowProtoException("Unknown min/max value: " + Value.ToString());
                                return default;
                        }
                    default:
                        ThrowHelper.ThrowProtoException("Unknown timescale: " + Scale.ToString());
                        return default;
                }
                return BclHelpers.EpochOrigin[(int)Kind].AddTicks(tickDelta);
            }

            internal ScaledTicks(TimeSpan timeSpan, DateTimeKind kind)
            {
                TimeSpanScale scale;
                long value = timeSpan.Ticks;
                if (timeSpan == TimeSpan.MaxValue)
                {
                    value = 1;
                    scale = TimeSpanScale.MinMax;
                }
                else if (timeSpan == TimeSpan.MinValue)
                {
                    value = -1;
                    scale = TimeSpanScale.MinMax;
                }
                else if (value % TimeSpan.TicksPerDay == 0)
                {
                    scale = TimeSpanScale.Days;
                    value /= TimeSpan.TicksPerDay;
                }
                else if (value % TimeSpan.TicksPerHour == 0)
                {
                    scale = TimeSpanScale.Hours;
                    value /= TimeSpan.TicksPerHour;
                }
                else if (value % TimeSpan.TicksPerMinute == 0)
                {
                    scale = TimeSpanScale.Minutes;
                    value /= TimeSpan.TicksPerMinute;
                }
                else if (value % TimeSpan.TicksPerSecond == 0)
                {
                    scale = TimeSpanScale.Seconds;
                    value /= TimeSpan.TicksPerSecond;
                }
                else if (value % TimeSpan.TicksPerMillisecond == 0)
                {
                    scale = TimeSpanScale.Milliseconds;
                    value /= TimeSpan.TicksPerMillisecond;
                }
                else
                {
                    scale = TimeSpanScale.Ticks;
                }

                Kind = kind;
                Value = value;
                Scale = scale;
            }


            public TimeSpan ToTimeSpan()
            {
                switch (Scale)
                {
                    case TimeSpanScale.Days:
                        return TimeSpan.FromDays(Value);
                    case TimeSpanScale.Hours:
                        return TimeSpan.FromHours(Value);
                    case TimeSpanScale.Minutes:
                        return TimeSpan.FromMinutes(Value);
                    case TimeSpanScale.Seconds:
                        return TimeSpan.FromSeconds(Value);
                    case TimeSpanScale.Milliseconds:
                        return TimeSpan.FromMilliseconds(Value);
                    case TimeSpanScale.Ticks:
                        return TimeSpan.FromTicks(Value);
                    case TimeSpanScale.MinMax:
                        switch (Value)
                        {
                            case 1: return TimeSpan.MaxValue;
                            case -1: return TimeSpan.MinValue;
                            default:
                                ThrowHelper.ThrowProtoException("Unknown min/max value: " + Value.ToString());
                                return default;
                        }
                    default:
                        ThrowHelper.ThrowProtoException("Unknown timescale: " + Scale.ToString());
                        return default;
                }
            }

            internal const int FieldTimeSpanValue = 0x01, FieldTimeSpanScale = 0x02, FieldTimeSpanKind = 0x03;
        }
        SerializerFeatures ISerializer<Guid>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessageWrappedAtRoot;
        SerializerFeatures ISerializer<Guid?>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryMessageWrappedAtRoot;
        private static
#if !DEBUG
            readonly
#endif
            bool s_guidOptimized = VerifyGuidLayout();

        internal static bool GuidOptimized
        {
            get => s_guidOptimized;
#if DEBUG
            set => s_guidOptimized = value && VerifyGuidLayout();
#endif
        }

        private const int FieldGuidLow = 1, FieldGuidHigh = 2;
        Guid? ISerializer<Guid?>.Read(ref ProtoReader.State state, Guid? value)
            => ((ISerializer<Guid>)this).Read(ref state, value.GetValueOrDefault());
        void ISerializer<Guid?>.Write(ref ProtoWriter.State state, Guid? value)
            => ((ISerializer<Guid>)this).Write(ref state, value.Value);

        Guid ISerializer<Guid>.Read(ref ProtoReader.State state, Guid value)
        {
            ulong low = 0, high = 0;
            int fieldNumber;
            while ((fieldNumber = state.ReadFieldHeader()) > 0)
            {
                switch (fieldNumber)
                {
                    case FieldGuidLow: low = state.ReadUInt64(); break;
                    case FieldGuidHigh: high = state.ReadUInt64(); break;
                    default: state.SkipField(); break;
                }
            }

            if (low == 0 & high == 0) return default;
            if (s_guidOptimized)
            {
                var acc = new GuidAccessor(low, high);
                return acc.Guid;
            }
            else
            {
                uint a = (uint)(low >> 32), b = (uint)low, c = (uint)(high >> 32), d = (uint)high;
                return new Guid((int)b, (short)a, (short)(a >> 16),
                    (byte)d, (byte)(d >> 8), (byte)(d >> 16), (byte)(d >> 24),
                    (byte)c, (byte)(c >> 8), (byte)(c >> 16), (byte)(c >> 24));
            }
        }

        void ISerializer<Guid>.Write(ref ProtoWriter.State state, Guid value)
        {
            if (value == Guid.Empty) { }
            else if (s_guidOptimized)
            {
                var obj = new GuidAccessor(value);
                state.WriteFieldHeader(FieldGuidLow, WireType.Fixed64);
                state.WriteUInt64(obj.Low);
                state.WriteFieldHeader(FieldGuidHigh, WireType.Fixed64);
                state.WriteUInt64(obj.High);
            }
            else
            {
                byte[] blob = value.ToByteArray();
                state.WriteFieldHeader(FieldGuidLow, WireType.Fixed64);
                state.WriteBytes(new ReadOnlyMemory<byte>(blob, 0, 8));
                state.WriteFieldHeader(FieldGuidHigh, WireType.Fixed64);
                state.WriteBytes(new ReadOnlyMemory<byte>(blob, 8, 8));
            }
        }

        /// <summary>
        /// Provides access to the inner fields of a Guid.
        /// Similar to Guid.ToByteArray(), but faster and avoids the byte[] allocation
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private readonly struct GuidAccessor
        {
            [FieldOffset(0)]
            public readonly Guid Guid;

            [FieldOffset(0)]
            public readonly ulong Low;

            [FieldOffset(8)]
            public readonly ulong High;

            public GuidAccessor(Guid value)
            {
                Low = High = default;
                Guid = value;
            }

            public GuidAccessor(ulong low, ulong high)
            {
                Guid = default;
                Low = low;
                High = high;
            }
        }
        private static bool VerifyGuidLayout()
        {
            try
            {
                if (!Guid.TryParse("12345678-2345-3456-4567-56789a6789ab", out var guid))
                    return false;

                var obj = new GuidAccessor(guid);
                var low = obj.Low;
                var high = obj.High;

                // check it the fast way against our known sentinels
                if (low != 0x3456234512345678 | high != 0xAB89679A78566745) return false;

                // and do it "for real"
                var expected = guid.ToByteArray();
                for (int i = 0; i < 8; i++)
                {
                    if (expected[i] != (byte)(low >> (8 * i))) return false;
                }
                for (int i = 0; i < 8; i++)
                {
                    if (expected[i + 8] != (byte)(high >> (8 * i))) return false;
                }
                return true;
            }
            catch { }
            return false;
        }

        string ISerializer<string>.Read(ref ProtoReader.State state, string value) => state.ReadString();
        void ISerializer<string>.Write(ref ProtoWriter.State state, string value) => state.WriteString(value);
        SerializerFeatures ISerializer<string>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<string>.Measure(ISerializationContext context, WireType wireType, string value)
            => wireType switch
            {
                WireType.String => ProtoWriter.UTF8.GetByteCount(value),
                _ => -1,
            };

      


        byte[] ISerializer<byte[]>.Read(ref ProtoReader.State state, byte[] value) => state.AppendBytes(value);
        void ISerializer<byte[]>.Write(ref ProtoWriter.State state, byte[] value) => state.WriteBytes(value);
        SerializerFeatures ISerializer<byte[]>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<byte[]>.Measure(ISerializationContext context, WireType wireType, byte[] value)
            => wireType switch
            {
                WireType.String => value.Length,
                _ => -1,
            };

        ArraySegment<byte> ISerializer<ArraySegment<byte>>.Read(ref ProtoReader.State state, ArraySegment<byte> value) => state.AppendBytes(value);
        void ISerializer<ArraySegment<byte>>.Write(ref ProtoWriter.State state, ArraySegment<byte> value) => state.WriteBytes(value);
        SerializerFeatures ISerializer<ArraySegment<byte>>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryScalar;

        int IMeasuringSerializer<ArraySegment<byte>>.Measure(ISerializationContext context, WireType wireType, ArraySegment<byte> value)
            => wireType switch
            {
                WireType.String => value.Count,
                _ => -1,
            };

        Memory<byte> ISerializer<Memory<byte>>.Read(ref ProtoReader.State state, Memory<byte> value) => state.AppendBytes(value);
        void ISerializer<Memory<byte>>.Write(ref ProtoWriter.State state, Memory<byte> value) => state.WriteBytes(value);
        SerializerFeatures ISerializer<Memory<byte>>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryScalar;

        int IMeasuringSerializer<Memory<byte>>.Measure(ISerializationContext context, WireType wireType, Memory<byte> value)
            => wireType switch
            {
                WireType.String => value.Length,
                _ => -1,
            };

        ReadOnlyMemory<byte> ISerializer<ReadOnlyMemory<byte>>.Read(ref ProtoReader.State state, ReadOnlyMemory<byte> value) => state.AppendBytes(value);
        void ISerializer<ReadOnlyMemory<byte>>.Write(ref ProtoWriter.State state, ReadOnlyMemory<byte> value) => state.WriteBytes(value);
        SerializerFeatures ISerializer<ReadOnlyMemory<byte>>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryScalar;

        int IMeasuringSerializer<ReadOnlyMemory<byte>>.Measure(ISerializationContext context, WireType wireType, ReadOnlyMemory<byte> value)
            => wireType switch
            {
                WireType.String => value.Length,
                _ => -1,
            };

        byte ISerializer<byte>.Read(ref ProtoReader.State state, byte value) => state.ReadByte();
        void ISerializer<byte>.Write(ref ProtoWriter.State state, byte value) => state.WriteByte(value);
        SerializerFeatures ISerializer<byte>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<byte>.Measure(ISerializationContext context, WireType wireType, byte value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureUInt32(value),
                _ => -1,
            };

        ushort ISerializer<ushort>.Read(ref ProtoReader.State state, ushort value) => state.ReadUInt16();
        void ISerializer<ushort>.Write(ref ProtoWriter.State state, ushort value) => state.WriteUInt16(value);
        SerializerFeatures ISerializer<ushort>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<ushort>.Measure(ISerializationContext context, WireType wireType, ushort value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureUInt32(value),
                _ => -1,
            };

        uint ISerializer<uint>.Read(ref ProtoReader.State state, uint value) => state.ReadUInt32();
        void ISerializer<uint>.Write(ref ProtoWriter.State state, uint value) => state.WriteUInt32(value);
        SerializerFeatures ISerializer<uint>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<uint>.Measure(ISerializationContext context, WireType wireType, uint value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureUInt32(value),
                _ => -1,
            };

        ulong ISerializer<ulong>.Read(ref ProtoReader.State state, ulong value) => state.ReadUInt64();
        void ISerializer<ulong>.Write(ref ProtoWriter.State state, ulong value) => state.WriteUInt64(value);
        SerializerFeatures ISerializer<ulong>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<ulong>.Measure(ISerializationContext context, WireType wireType, ulong value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureUInt64(value),
                _ => -1,
            };

        long ISerializer<long>.Read(ref ProtoReader.State state, long value) => state.ReadInt64();
        void ISerializer<long>.Write(ref ProtoWriter.State state, long value) => state.WriteInt64(value);
        SerializerFeatures ISerializer<long>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<long>.Measure(ISerializationContext context, WireType wireType, long value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureUInt64((ulong)value),
                WireType.SignedVarint => ProtoWriter.MeasureUInt64(ProtoWriter.Zig(value)),
                _ => -1,
            };

        bool ISerializer<bool>.Read(ref ProtoReader.State state, bool value) => state.ReadBoolean();
        void ISerializer<bool>.Write(ref ProtoWriter.State state, bool value) => state.WriteBoolean(value);
        SerializerFeatures ISerializer<bool>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<bool>.Measure(ISerializationContext context, WireType wireType, bool value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => 1,
                _ => -1,
            };

        float ISerializer<float>.Read(ref ProtoReader.State state, float value) => state.ReadSingle();
        void ISerializer<float>.Write(ref ProtoWriter.State state, float value) => state.WriteSingle(value);
        SerializerFeatures ISerializer<float>.Features => SerializerFeatures.WireTypeFixed32 | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<float>.Measure(ISerializationContext context, WireType wireType, float value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                _ => -1,
            };

        double ISerializer<double>.Read(ref ProtoReader.State state, double value) => state.ReadDouble();
        void ISerializer<double>.Write(ref ProtoWriter.State state, double value) => state.WriteDouble(value);
        SerializerFeatures ISerializer<double>.Features => SerializerFeatures.WireTypeFixed64 | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<double>.Measure(ISerializationContext context, WireType wireType, double value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                _ => -1,
            };

        sbyte ISerializer<sbyte>.Read(ref ProtoReader.State state, sbyte value) => state.ReadSByte();
        void ISerializer<sbyte>.Write(ref ProtoWriter.State state, sbyte value) => state.WriteSByte(value);
        SerializerFeatures ISerializer<sbyte>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<sbyte>.Measure(ISerializationContext context, WireType wireType, sbyte value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureInt32(value),
                WireType.SignedVarint => ProtoWriter.MeasureUInt32(ProtoWriter.Zig(value)),
                _ => -1,
            };

        short ISerializer<short>.Read(ref ProtoReader.State state, short value) => state.ReadInt16();
        void ISerializer<short>.Write(ref ProtoWriter.State state, short value) => state.WriteInt16(value);
        SerializerFeatures ISerializer<short>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<short>.Measure(ISerializationContext context, WireType wireType, short value)
            => wireType switch
            {
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.Varint => ProtoWriter.MeasureInt32(value),
                WireType.SignedVarint => ProtoWriter.MeasureUInt32(ProtoWriter.Zig(value)),
                _ => -1,
            };

        Uri ISerializer<Uri>.Read(ref ProtoReader.State state, Uri value)
        {
            var uri = state.ReadString();
            return string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);
        }
        void ISerializer<Uri>.Write(ref ProtoWriter.State state, Uri value) => state.WriteString(value.OriginalString);
        SerializerFeatures ISerializer<Uri>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<Uri>.Measure(ISerializationContext context, WireType wireType, Uri value)
            => wireType switch
            {
                WireType.String => ProtoWriter.UTF8.GetByteCount(value.OriginalString),
                _ => -1,
            };

        char ISerializer<char>.Read(ref ProtoReader.State state, char value) => (char)state.ReadUInt16();
        void ISerializer<char>.Write(ref ProtoWriter.State state, char value) => state.WriteUInt16(value);
        SerializerFeatures ISerializer<char>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<char>.Measure(ISerializationContext context, WireType wireType, char value)
            => wireType switch {
            WireType.Fixed32 => 4,
            WireType.Fixed64 => 8,
            WireType.Varint => ProtoWriter.MeasureUInt32(value),
            _ => -1,
        };

        string IFactory<string>.Create(ISerializationContext context) => "";

        byte[] IFactory<byte[]>.Create(ISerializationContext context) => Array.Empty<byte>();

        SerializerFeatures ISerializer<int?>.Features => ((ISerializer<int>)this).Features;
        void ISerializer<int?>.Write(ref ProtoWriter.State state, int? value) => ((ISerializer<int>)this).Write(ref state, value.Value);
        int? ISerializer<int?>.Read(ref ProtoReader.State state, int? value) => ((ISerializer<int>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<short?>.Features => ((ISerializer<short>)this).Features;
        void ISerializer<short?>.Write(ref ProtoWriter.State state, short? value) => ((ISerializer<short>)this).Write(ref state, value.Value);
        short? ISerializer<short?>.Read(ref ProtoReader.State state, short? value) => ((ISerializer<short>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<long?>.Features => ((ISerializer<long>)this).Features;
        void ISerializer<long?>.Write(ref ProtoWriter.State state, long? value) => ((ISerializer<long>)this).Write(ref state, value.Value);
        long? ISerializer<long?>.Read(ref ProtoReader.State state, long? value) => ((ISerializer<long>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<sbyte?>.Features => ((ISerializer<sbyte>)this).Features;
        void ISerializer<sbyte?>.Write(ref ProtoWriter.State state, sbyte? value) => ((ISerializer<sbyte>)this).Write(ref state, value.Value);
        sbyte? ISerializer<sbyte?>.Read(ref ProtoReader.State state, sbyte? value) => ((ISerializer<sbyte>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<uint?>.Features => ((ISerializer<uint>)this).Features;
        void ISerializer<uint?>.Write(ref ProtoWriter.State state, uint? value) => ((ISerializer<uint>)this).Write(ref state, value.Value);
        uint? ISerializer<uint?>.Read(ref ProtoReader.State state, uint? value) => ((ISerializer<uint>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<ushort?>.Features => ((ISerializer<ushort>)this).Features;
        void ISerializer<ushort?>.Write(ref ProtoWriter.State state, ushort? value) => ((ISerializer<ushort>)this).Write(ref state, value.Value);
        ushort? ISerializer<ushort?>.Read(ref ProtoReader.State state, ushort? value) => ((ISerializer<ushort>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<ulong?>.Features => ((ISerializer<ulong>)this).Features;
        void ISerializer<ulong?>.Write(ref ProtoWriter.State state, ulong? value) => ((ISerializer<ulong>)this).Write(ref state, value.Value);
        ulong? ISerializer<ulong?>.Read(ref ProtoReader.State state, ulong? value) => ((ISerializer<ulong>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<byte?>.Features => ((ISerializer<byte>)this).Features;
        void ISerializer<byte?>.Write(ref ProtoWriter.State state, byte? value) => ((ISerializer<byte>)this).Write(ref state, value.Value);
        byte? ISerializer<byte?>.Read(ref ProtoReader.State state, byte? value) => ((ISerializer<byte>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<char?>.Features => ((ISerializer<char>)this).Features;
        void ISerializer<char?>.Write(ref ProtoWriter.State state, char? value) => ((ISerializer<char>)this).Write(ref state, value.Value);
        char? ISerializer<char?>.Read(ref ProtoReader.State state, char? value) => ((ISerializer<char>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<bool?>.Features => ((ISerializer<bool>)this).Features;
        void ISerializer<bool?>.Write(ref ProtoWriter.State state, bool? value) => ((ISerializer<bool>)this).Write(ref state, value.Value);
        bool? ISerializer<bool?>.Read(ref ProtoReader.State state, bool? value) => ((ISerializer<bool>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<float?>.Features => ((ISerializer<float>)this).Features;
        void ISerializer<float?>.Write(ref ProtoWriter.State state, float? value) => ((ISerializer<float>)this).Write(ref state, value.Value);
        float? ISerializer<float?>.Read(ref ProtoReader.State state, float? value) => ((ISerializer<float>)this).Read(ref state, value.GetValueOrDefault());

        SerializerFeatures ISerializer<double?>.Features => ((ISerializer<double>)this).Features;
        void ISerializer<double?>.Write(ref ProtoWriter.State state, double? value) => ((ISerializer<double>)this).Write(ref state, value.Value);
        double? ISerializer<double?>.Read(ref ProtoReader.State state, double? value) => ((ISerializer<double>)this).Read(ref state, value.GetValueOrDefault());

        Type ISerializer<Type>.Read(ref ProtoReader.State state, Type value) => state.ReadType();
        void ISerializer<Type>.Write(ref ProtoWriter.State state, Type value) => state.WriteType(value);
        SerializerFeatures ISerializer<Type>.Features => SerializerFeatures.WireTypeString | SerializerFeatures.CategoryScalar;
        int IMeasuringSerializer<Type>.Measure(ISerializationContext context, WireType wireType, Type value)
            => wireType switch
            {
                WireType.String => Encoding.UTF8.GetByteCount(TypeModel.SerializeType(context?.Model, value)),
                _ => -1,
            };


        bool IValueChecker<string>.HasNonTrivialValue(string value) => value is not null; //  note: we write "" (when found), for compat
        bool IValueChecker<Uri>.HasNonTrivialValue(Uri value) => value?.OriginalString is not null; //  note: we write "" (when found), for compat
        bool IValueChecker<Type>.HasNonTrivialValue(Type value) => value is not null;
        bool IValueChecker<byte[]>.HasNonTrivialValue(byte[] value) => value is not null;  //  note: we write [] (when found), for compat
        bool IValueChecker<sbyte>.HasNonTrivialValue(sbyte value) => value != 0;
        bool IValueChecker<short>.HasNonTrivialValue(short value) => value != 0;
        bool IValueChecker<long>.HasNonTrivialValue(long value) => value != 0;
        bool IValueChecker<byte>.HasNonTrivialValue(byte value) => value != 0;
        bool IValueChecker<ushort>.HasNonTrivialValue(ushort value) => value != 0;
        bool IValueChecker<uint>.HasNonTrivialValue(uint value) => value != 0;
        bool IValueChecker<ulong>.HasNonTrivialValue(ulong value) => value != 0;
        bool IValueChecker<char>.HasNonTrivialValue(char value) => value != 0;
        bool IValueChecker<bool>.HasNonTrivialValue(bool value) => value;
        bool IValueChecker<float>.HasNonTrivialValue(float value) => value != 0;
        bool IValueChecker<double>.HasNonTrivialValue(double value) => value != 0;
        bool IValueChecker<sbyte>.IsNull(sbyte value) => false;
        bool IValueChecker<short>.IsNull(short value) => false;
        bool IValueChecker<long>.IsNull(long value) => false;
        bool IValueChecker<byte>.IsNull(byte value) => false;
        bool IValueChecker<ushort>.IsNull(ushort value) => false;
        bool IValueChecker<uint>.IsNull(uint value) => false;
        bool IValueChecker<ulong>.IsNull(ulong value) => false;
        bool IValueChecker<char>.IsNull(char value) => false;
        bool IValueChecker<bool>.IsNull(bool value) => false;
        bool IValueChecker<float>.IsNull(float value) => false;
        bool IValueChecker<double>.IsNull(double value) => false;
        bool IValueChecker<string>.IsNull(string value) => value is null;
        bool IValueChecker<byte[]>.IsNull(byte[] value) => value is null;
        bool IValueChecker<Uri>.IsNull(Uri value) => value is null;
        bool IValueChecker<Type>.IsNull(Type value) => value is null;

        bool IValueChecker<int?>.HasNonTrivialValue(int? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<int?>.IsNull(int? value) => !value.HasValue;
        bool IValueChecker<uint?>.HasNonTrivialValue(uint? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<uint?>.IsNull(uint? value) => !value.HasValue;
        bool IValueChecker<short?>.HasNonTrivialValue(short? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<short?>.IsNull(short? value) => !value.HasValue;
        bool IValueChecker<ushort?>.HasNonTrivialValue(ushort? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<ushort?>.IsNull(ushort? value) => !value.HasValue;
        bool IValueChecker<long?>.HasNonTrivialValue(long? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<long?>.IsNull(long? value) => !value.HasValue;
        bool IValueChecker<ulong?>.HasNonTrivialValue(ulong? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<ulong?>.IsNull(ulong? value) => !value.HasValue;
        bool IValueChecker<float?>.HasNonTrivialValue(float? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<float?>.IsNull(float? value) => !value.HasValue;
        bool IValueChecker<double?>.HasNonTrivialValue(double? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<double?>.IsNull(double? value) => !value.HasValue;
        bool IValueChecker<byte?>.HasNonTrivialValue(byte? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<byte?>.IsNull(byte? value) => !value.HasValue;
        bool IValueChecker<sbyte?>.HasNonTrivialValue(sbyte? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<sbyte?>.IsNull(sbyte? value) => !value.HasValue;
        bool IValueChecker<bool?>.HasNonTrivialValue(bool? value) => value.GetValueOrDefault();
        bool IValueChecker<bool?>.IsNull(bool? value) => !value.HasValue;
        bool IValueChecker<char?>.HasNonTrivialValue(char? value) => value.GetValueOrDefault() != 0;
        bool IValueChecker<char?>.IsNull(char? value) => !value.HasValue;

        int IMeasuringSerializer<int?>.Measure(ISerializationContext context, WireType wireType, int? value)
            => ((IMeasuringSerializer<int>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<long?>.Measure(ISerializationContext context, WireType wireType, long? value)
            => ((IMeasuringSerializer<long>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<float?>.Measure(ISerializationContext context, WireType wireType, float? value)
            => ((IMeasuringSerializer<float>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<double?>.Measure(ISerializationContext context, WireType wireType, double? value)
            => ((IMeasuringSerializer<double>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<byte?>.Measure(ISerializationContext context, WireType wireType, byte? value)
            => ((IMeasuringSerializer<byte>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<ushort?>.Measure(ISerializationContext context, WireType wireType, ushort? value)
            => ((IMeasuringSerializer<ushort>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<uint?>.Measure(ISerializationContext context, WireType wireType, uint? value)
            => ((IMeasuringSerializer<uint>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<ulong?>.Measure(ISerializationContext context, WireType wireType, ulong? value)
            => ((IMeasuringSerializer<ulong>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<sbyte?>.Measure(ISerializationContext context, WireType wireType, sbyte? value)
            => ((IMeasuringSerializer<sbyte>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<short?>.Measure(ISerializationContext context, WireType wireType, short? value)
            => ((IMeasuringSerializer<short>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<char?>.Measure(ISerializationContext context, WireType wireType, char? value)
            => ((IMeasuringSerializer<char>)this).Measure(context, wireType, value.Value);
        int IMeasuringSerializer<bool?>.Measure(ISerializationContext context, WireType wireType, bool? value)
            => ((IMeasuringSerializer<bool>)this).Measure(context, wireType, value.Value);


        SerializerFeatures ISerializer<IntPtr>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        SerializerFeatures ISerializer<UIntPtr>.Features => SerializerFeatures.WireTypeVarint | SerializerFeatures.CategoryScalar;
        SerializerFeatures ISerializer<IntPtr?>.Features => ((ISerializer<IntPtr>)this).Features;
        SerializerFeatures ISerializer<UIntPtr?>.Features => ((ISerializer<UIntPtr>)this).Features;

        void ISerializer<IntPtr?>.Write(ref ProtoWriter.State state, IntPtr? value) => ((ISerializer<IntPtr>)this).Write(ref state, value.Value);
        IntPtr? ISerializer<IntPtr?>.Read(ref ProtoReader.State state, IntPtr? value) => ((ISerializer<IntPtr>)this).Read(ref state, value.GetValueOrDefault());
        void ISerializer<UIntPtr?>.Write(ref ProtoWriter.State state, UIntPtr? value) => ((ISerializer<UIntPtr>)this).Write(ref state, value.Value);
        UIntPtr? ISerializer<UIntPtr?>.Read(ref ProtoReader.State state, UIntPtr? value) => ((ISerializer<UIntPtr>)this).Read(ref state, value.GetValueOrDefault());

        void ISerializer<IntPtr>.Write(ref ProtoWriter.State state, IntPtr value) => state.WriteIntPtr(value);
        IntPtr ISerializer<IntPtr>.Read(ref ProtoReader.State state, IntPtr value) => state.ReadIntPtr();
        void ISerializer<UIntPtr>.Write(ref ProtoWriter.State state, UIntPtr value) => state.WriteUIntPtr(value);
        UIntPtr ISerializer<UIntPtr>.Read(ref ProtoReader.State state, UIntPtr value) => state.ReadUIntPtr();

        int IMeasuringSerializer<IntPtr>.Measure(ISerializationContext context, WireType wireType, IntPtr value)
            => ((IMeasuringSerializer<long>)this).Measure(context, wireType, value.ToInt64());

        int IMeasuringSerializer<UIntPtr>.Measure(ISerializationContext context, WireType wireType, UIntPtr value)
            => ((IMeasuringSerializer<ulong>)this).Measure(context, wireType, value.ToUInt64());

        int IMeasuringSerializer<IntPtr?>.Measure(ISerializationContext context, WireType wireType, IntPtr? value)
            => ((IMeasuringSerializer<long>)this).Measure(context, wireType, value.Value.ToInt64());
        int IMeasuringSerializer<UIntPtr?>.Measure(ISerializationContext context, WireType wireType, UIntPtr? value)
            => ((IMeasuringSerializer<ulong>)this).Measure(context, wireType, value.Value.ToUInt64());

        bool IValueChecker<IntPtr>.HasNonTrivialValue(IntPtr value) => value != IntPtr.Zero;
        bool IValueChecker<IntPtr>.IsNull(IntPtr value) => false;

        bool IValueChecker<UIntPtr>.HasNonTrivialValue(UIntPtr value) => value != UIntPtr.Zero;
        bool IValueChecker<UIntPtr>.IsNull(UIntPtr value) => false;

        bool IValueChecker<IntPtr?>.HasNonTrivialValue(IntPtr? value) => value.GetValueOrDefault() != IntPtr.Zero;
        bool IValueChecker<IntPtr?>.IsNull(IntPtr? value) => !value.HasValue;

        bool IValueChecker<UIntPtr?>.HasNonTrivialValue(UIntPtr? value) => value.GetValueOrDefault() != UIntPtr.Zero;
        bool IValueChecker<UIntPtr?>.IsNull(UIntPtr? value) => !value.HasValue;

    }
}
