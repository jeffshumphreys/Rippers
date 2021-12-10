// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Int128.cs">
//   Copyright (c) 2013 Alexander Logger. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

//using BigMath.Utils;

namespace SharedFunctionality
{
    /// <summary>
    ///     Represents a 256-bit signed integer.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
    public struct Int256 : IComparable<Int256>, IComparable, IEquatable<Int256>, IFormattable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(0)]
        private ulong _d;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(8)]
        private ulong _c;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(16)]
        private ulong _b;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(32)]
        private ulong _a;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return "0x" + ToString("X1"); }
        }


        private const ulong NegativeSignMask = 0x1UL << 63;


        /// <summary>
        ///     Gets a value that represents the number 0 (zero).
        /// </summary>
        public static Int256 Zero = GetZero();


        /// <summary>
        ///     Represents the largest possible value of an Int256.
        /// </summary>
        public static Int256 MaxValue = GetMaxValue();


        /// <summary>
        ///     Represents the smallest possible value of an Int256.
        /// </summary>
        public static Int256 MinValue = GetMinValue();


        private static Int256 GetMaxValue()
        {
            return new Int256(long.MaxValue, ulong.MaxValue, ulong.MaxValue, ulong.MaxValue);
        }


        private static Int256 GetMinValue()
        {
            return -GetMaxValue();
        }


        private static Int256 GetZero()
        {
            return new Int256();
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(byte value)
        {
            _a = 0;
            _b = 0;
            _c = 0;
            _d = value;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public Int256(bool value)
        {
            _a = 0;
            _b = 0;
            _c = 0;
            _d = (ulong)(value ? 1 : 0);
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(char value)
        {
            _a = 0;
            _b = 0;
            _c = 0;
            _d = value;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(decimal value)
        {
            bool isNegative = value < 0;
            uint[] bits = decimal.GetBits(value).ConvertAll(i => (uint)i);
            uint scale = (bits[3] >> 16) & 0x1F;
            if (scale > 0)
            {
                uint[] quotient;
                uint[] reminder;
                MathUtils.DivModUnsigned(bits, new[] { 10U * scale }, out quotient, out reminder);


                bits = quotient;
            }


            _a = 0;
            _b = 0;
            _c = bits[2];
            _d = bits[0] | (ulong)bits[1] << 32;


            if (isNegative)
            {
                Negate();
            }
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(double value)
            : this((decimal)value)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(float value)
            : this((decimal)value)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(short value)
            : this((int)value)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(int value)
            : this((long)value)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(long value)
        {
            _a = _b = _c = unchecked((ulong)(value < 0 ? ~0 : 0));
            _d = (ulong)value;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(sbyte value)
            : this((long)value)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(ushort value)
        {
            _a = 0;
            _b = 0;
            _c = 0;
            _d = value;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(uint value)
        {
            _a = 0;
            _b = 0;
            _c = 0;
            _d = value;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(ulong value)
        {
            _a = 0;
            _b = 0;
            _c = 0;
            _d = value;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(Guid value)
        {
            var int256 = value.ToByteArray().ToInt256(0);
            _a = int256.A;
            _b = int256.B;
            _c = int256.C;
            _d = int256.D;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int256(Int128 value)
        {
            ulong[] values = value.ToUIn64Array();
            _a = _b = unchecked((ulong)(value.Sign < 0 ? ~0 : 0));
            _c = values[1];
            _d = values[0];
        }


        public Int256(ulong a, ulong b, ulong c, ulong d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Int256" /> struct.
        /// </summary>
        /// <param name="sign">The sign.</param>
        /// <param name="ints">The ints.</param>
        public Int256(int sign, uint[] ints)
        {
            if (ints == null)
            {
                throw new ArgumentNullException("ints");
            }


            var value = new ulong[4];
            for (int i = 0; i < ints.Length && i < 8; i++)
            {
                Buffer.BlockCopy(ints[i].ToBytes(), 0, value, i * 4, 4);
            }


            _a = value[3];
            _b = value[2];
            _c = value[1];
            _d = value[0];


            if (sign < 0 && (_d > 0 || _c > 0 || _b > 0 || _a > 0))
            {
                // We use here two's complement numbers representation,
                // hence such operations for negative numbers.
                Negate();
                _a |= NegativeSignMask; // Ensure negative sign.
            }
        }


        /// <summary>
        ///     Higher 64 bits of the higher 128 bits.
        /// </summary>
        public ulong A
        {
            get { return _a; }
        }


        /// <summary>
        ///     Lower 64 bits of the higher 128 bits.
        /// </summary>
        public ulong B
        {
            get { return _b; }
        }


        /// <summary>
        ///     Higher 64 bits of the lower 128 bits.
        /// </summary>
        public ulong C
        {
            get { return _c; }
        }


        /// <summary>
        ///     Lower 64 bits of the lower 128 bits.
        /// </summary>
        public ulong D
        {
            get { return _d; }
        }


        /// <summary>
        ///     Gets a number that indicates the sign (negative, positive, or zero) of the current Int256 object.
        /// </summary>
        /// <value>A number that indicates the sign of the Int256 object</value>
        public int Sign
        {
            get
            {
                if (_a == 0 && _b == 0 && _c == 0 && _d == 0)
                {
                    return 0;
                }


                return ((_a & NegativeSignMask) == 0) ? 1 : -1;
            }
        }


        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _a.GetHashCode() ^ _b.GetHashCode() ^ _c.GetHashCode() ^ _d.GetHashCode();
        }


        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        ///     true if obj has the same value as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }


        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified Int64 value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///     true if obj has the same value as this instance; otherwise, false.
        /// </returns>
        public bool Equals(Int256 obj)
        {
            return _a == obj._a && _b == obj._b && _c == obj._c && _d == obj._d;
        }


        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(null, null);
        }


        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format. Only x, X, g, G, d, D are supported.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            if (!string.IsNullOrEmpty(format))
            {
                char ch = format[0];
                if ((ch == 'x') || (ch == 'X'))
                {
                    int min;
                    int.TryParse(format.Substring(1).Trim(), out min);
                    return this.ToBytes(false).ToHexString(ch == 'X', min, trimZeros: true);
                }


                if (((ch != 'G') && (ch != 'g')) && ((ch != 'D') && (ch != 'd')))
                {
                    throw new NotSupportedException("Not supported format: " + format);
                }
            }


            return ToString((NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo)));
        }


        private string ToString(NumberFormatInfo info)
        {
            if (Sign == 0)
            {
                return "0";
            }


            var sb = new StringBuilder();
            var ten = new Int256(10);
            Int256 current = Sign < 0 ? -this : this;
            while (true)
            {
                Int256 r;
                current = DivRem(current, ten, out r);
                if (r._d > 0 || current.Sign != 0 || (sb.Length == 0))
                {
                    sb.Insert(0, (char)('0' + r._d));
                }
                if (current.Sign == 0)
                {
                    break;
                }
            }


            string s = sb.ToString();
            if ((Sign < 0) && (s != "0"))
            {
                return info.NegativeSign + s;
            }


            return s;
        }


        /// <summary>
        ///     Converts the numeric value to an equivalent object. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="conversionType">The target conversion type.</param>
        /// <param name="provider">An object that supplies culture-specific information about the conversion.</param>
        /// <param name="asLittleEndian">As little endian.</param>
        /// <param name="value">
        ///     When this method returns, contains the value that is equivalent to the numeric value, if the
        ///     conversion succeeded, or is null if the conversion failed. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if this value was converted successfully; otherwise, false.</returns>
        public bool TryConvert(Type conversionType, IFormatProvider provider, bool asLittleEndian, out object value)
        {
            if (conversionType == typeof(bool))
            {
                value = (bool)this;
                return true;
            }


            if (conversionType == typeof(byte))
            {
                value = (byte)this;
                return true;
            }


            if (conversionType == typeof(char))
            {
                value = (char)this;
                return true;
            }


            if (conversionType == typeof(decimal))
            {
                value = (decimal)this;
                return true;
            }


            if (conversionType == typeof(double))
            {
                value = (double)this;
                return true;
            }


            if (conversionType == typeof(short))
            {
                value = (short)this;
                return true;
            }


            if (conversionType == typeof(int))
            {
                value = (int)this;
                return true;
            }


            if (conversionType == typeof(long))
            {
                value = (long)this;
                return true;
            }


            if (conversionType == typeof(sbyte))
            {
                value = (sbyte)this;
                return true;
            }


            if (conversionType == typeof(float))
            {
                value = (float)this;
                return true;
            }


            if (conversionType == typeof(string))
            {
                value = ToString(null, provider);
                return true;
            }


            if (conversionType == typeof(ushort))
            {
                value = (ushort)this;
                return true;
            }


            if (conversionType == typeof(uint))
            {
                value = (uint)this;
                return true;
            }


            if (conversionType == typeof(ulong))
            {
                value = (ulong)this;
                return true;
            }


            if (conversionType == typeof(byte[]))
            {
                value = this.ToBytes(asLittleEndian);
                return true;
            }


            if (conversionType == typeof(Guid))
            {
                value = new Guid(this.ToBytes(asLittleEndian));
                return true;
            }


            if (conversionType == typeof(Int128))
            {
                value = (Int128)this;
                return true;
            }


            value = null;
            return false;
        }


        /// <summary>
        ///     Converts the string representation of a number to its Int256 equivalent.
        /// </summary>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <returns>
        ///     A value that is equivalent to the number specified in the value parameter.
        /// </returns>
        public static Int256 Parse(string value)
        {
            return Parse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
        }


        /// <summary>
        ///     Converts the string representation of a number in a specified style format to its Int256 equivalent.
        /// </summary>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of the enumeration values that specify the permitted format of value.</param>
        /// <returns>
        ///     A value that is equivalent to the number specified in the value parameter.
        /// </returns>
        public static Int256 Parse(string value, NumberStyles style)
        {
            return Parse(value, style, NumberFormatInfo.CurrentInfo);
        }


        /// <summary>
        ///     Converts the string representation of a number in a culture-specific format to its Int256 equivalent.
        /// </summary>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about value.</param>
        /// <returns>
        ///     A value that is equivalent to the number specified in the value parameter.
        /// </returns>
        public static Int256 Parse(string value, IFormatProvider provider)
        {
            return Parse(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }


        /// <summary>
        ///     Converts the string representation of a number in a specified style and culture-specific format to its Int256
        ///     equivalent.
        /// </summary>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of the enumeration values that specify the permitted format of value.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about value.</param>
        /// <returns>A value that is equivalent to the number specified in the value parameter.</returns>
        public static Int256 Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            Int256 result;
            if (!TryParse(value, style, provider, out result))
            {
                throw new ArgumentException(null, "value");
            }


            return result;
        }


        /// <summary>
        ///     Tries to convert the string representation of a number to its Int256 equivalent, and returns a value that indicates
        ///     whether the conversion succeeded..
        /// </summary>
        /// <param name="value">The string representation of a number.</param>
        /// <param name="result">
        ///     When this method returns, contains the Int256 equivalent to the number that is contained in value,
        ///     or Int256.Zero if the conversion failed. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     true if the value parameter was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string value, out Int256 result)
        {
            return TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }


        /// <summary>
        ///     Tries to convert the string representation of a number in a specified style and culture-specific format to its
        ///     Int256 equivalent, and returns a value that indicates whether the conversion succeeded..
        /// </summary>
        /// <param name="value">
        ///     The string representation of a number. The string is interpreted using the style specified by
        ///     style.
        /// </param>
        /// <param name="style">
        ///     A bitwise combination of enumeration values that indicates the style elements that can be present
        ///     in value. A typical value to specify is NumberStyles.Integer.
        /// </param>
        /// <param name="provider">An object that supplies culture-specific formatting information about value.</param>
        /// <param name="result">
        ///     When this method returns, contains the Int256 equivalent to the number that is contained in value,
        ///     or Int256.Zero if the conversion failed. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out Int256 result)
        {
            result = Zero;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }


            if (value.StartsWith("x", StringComparison.OrdinalIgnoreCase))
            {
                style |= NumberStyles.AllowHexSpecifier;
                value = value.Substring(1);
            }
            else if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                style |= NumberStyles.AllowHexSpecifier;
                value = value.Substring(2);
            }


            if ((style & NumberStyles.AllowHexSpecifier) == NumberStyles.AllowHexSpecifier)
            {
                return TryParseHex(value, out result);
            }


            return TryParseNum(value, out result);
        }


        private static bool TryParseHex(string value, out Int256 result)
        {
            if (value.Length > 64)
            {
                throw new OverflowException();
            }


            result = Zero;
            int pos = 0;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                char ch = value[i];
                ulong bch;
                if ((ch >= '0') && (ch <= '9'))
                {
                    bch = (ulong)(ch - '0');
                }
                else if ((ch >= 'A') && (ch <= 'F'))
                {
                    bch = (ulong)(ch - 'A' + 10);
                }
                else if ((ch >= 'a') && (ch <= 'f'))
                {
                    bch = (ulong)(ch - 'a' + 10);
                }
                else
                {
                    return false;
                }


                if (pos < 64)
                {
                    result._d |= bch << pos;
                }
                else if (pos < 128)
                {
                    result._c |= bch << pos;
                }
                else if (pos < 192)
                {
                    result._b |= bch << pos;
                }
                else if (pos < 256)
                {
                    result._a |= bch << pos;
                }
                pos += 4;
            }
            return true;
        }


        private static bool TryParseNum(string value, out Int256 result)
        {
            result = Zero;
            foreach (char ch in value)
            {
                byte b;
                if ((ch >= '0') && (ch <= '9'))
                {
                    b = (byte)(ch - '0');
                }
                else
                {
                    return false;
                }


                result = 10 * result;
                result += b;
            }
            return true;
        }


        /// <summary>
        ///     Converts the value of this instance to an <see cref="T:System.Object" /> of the specified
        ///     <see cref="T:System.Type" /> that has an equivalent value, using the specified culture-specific formatting
        ///     information.
        /// </summary>
        /// <param name="conversionType">The <see cref="T:System.Type" /> to which the value of this instance is converted.</param>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider" /> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <param name="asLittleEndian">As little endian.</param>
        /// <returns>
        ///     An <see cref="T:System.Object" /> instance of type <paramref name="conversionType" /> whose value is equivalent to
        ///     the value of this instance.
        /// </returns>
        public object ToType(Type conversionType, IFormatProvider provider, bool asLittleEndian)
        {
            object value;
            if (TryConvert(conversionType, provider, asLittleEndian, out value))
            {
                return value;
            }


            throw new InvalidCastException();
        }


        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
        ///     Meaning Less than zero This instance is less than <paramref name="obj" />. Zero This instance is equal to
        ///     <paramref name="obj" />. Greater than zero This instance is greater than <paramref name="obj" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="obj" /> is not the same type as this instance.
        /// </exception>
        int IComparable.CompareTo(object obj)
        {
            return Compare(this, obj);
        }


        /// <summary>
        ///     Compares two Int256 values and returns an integer that indicates whether the first value is less than, equal to, or
        ///     greater than the second value.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>A signed integer that indicates the relative values of left and right, as shown in the following table.</returns>
        public static int Compare(Int256 left, object right)
        {
            if (right is Int256)
            {
                return Compare(left, (Int256)right);
            }


            // NOTE: this could be optimized type per type
            if (right is bool)
            {
                return Compare(left, new Int256((bool)right));
            }


            if (right is byte)
            {
                return Compare(left, new Int256((byte)right));
            }


            if (right is char)
            {
                return Compare(left, new Int256((char)right));
            }


            if (right is decimal)
            {
                return Compare(left, new Int256((decimal)right));
            }


            if (right is double)
            {
                return Compare(left, new Int256((double)right));
            }


            if (right is short)
            {
                return Compare(left, new Int256((short)right));
            }


            if (right is int)
            {
                return Compare(left, new Int256((int)right));
            }


            if (right is long)
            {
                return Compare(left, new Int256((long)right));
            }


            if (right is sbyte)
            {
                return Compare(left, new Int256((sbyte)right));
            }


            if (right is float)
            {
                return Compare(left, new Int256((float)right));
            }


            if (right is ushort)
            {
                return Compare(left, new Int256((ushort)right));
            }


            if (right is uint)
            {
                return Compare(left, new Int256((uint)right));
            }


            if (right is ulong)
            {
                return Compare(left, new Int256((ulong)right));
            }


            var bytes = right as byte[];
            if ((bytes != null) && (bytes.Length == 32))
            {
                // TODO: ensure endian.
                return Compare(left, bytes.ToInt256(0));
            }


            if (right is Guid)
            {
                return Compare(left, new Int256((Guid)right));
            }


            throw new ArgumentException();
        }


        /// <summary>
        ///     Compares two 256-bit signed integer values and returns an integer that indicates whether the first value is less
        ///     than, equal to, or greater than the second value.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>
        ///     A signed number indicating the relative values of this instance and value.
        /// </returns>
        public static int Compare(Int256 left, Int256 right)
        {
            int leftSign = left.Sign;
            int rightSign = right.Sign;


            if (leftSign == 0 && rightSign == 0)
            {
                return 0;
            }


            if (leftSign >= 0 && rightSign < 0)
            {
                return 1;
            }


            if (leftSign < 0 && rightSign >= 0)
            {
                return -1;
            }


            if (left._a != right._a)
            {
                return left._a.CompareTo(right._a);
            }
            if (left._b != right._b)
            {
                return left._b.CompareTo(right._b);
            }
            if (left._c != right._c)
            {
                return left._c.CompareTo(right._c);
            }


            return left._d.CompareTo(right._d);
        }


        /// <summary>
        ///     Compares this instance to a specified 256-bit signed integer and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An integer to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and value.</returns>
        public int CompareTo(Int256 value)
        {
            return Compare(this, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Not()
        {
            _a = ~_a;
            _b = ~_b;
            _c = ~_c;
            _d = ~_d;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Negate()
        {
            Not();
            this++;
        }


        /// <summary>
        ///     Negates a specified Int256 value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The result of the value parameter multiplied by negative one (-1).</returns>
        public static Int256 Negate(Int256 value)
        {
            value.Negate();
            return value;
        }


        /// <summary>
        ///     Gets the absolute value this object.
        /// </summary>
        /// <returns>The absolute value.</returns>
        public Int256 ToAbs()
        {
            return Abs(this);
        }


        /// <summary>
        ///     Gets the absolute value of an Int256 object.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>
        ///     The absolute value.
        /// </returns>
        public static Int256 Abs(Int256 value)
        {
            if (value.Sign < 0)
            {
                return -value;
            }


            return value;
        }


        /// <summary>
        ///     Adds two Int256 values and returns the result.
        /// </summary>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        /// <returns>The sum of left and right.</returns>
        public static Int256 Add(Int256 left, Int256 right)
        {
            return left + right;
        }


        /// <summary>
        ///     Subtracts one Int256 value from another and returns the result.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        /// <returns>The result of subtracting right from left.</returns>
        public static Int256 Subtract(Int256 left, Int256 right)
        {
            return left - right;
        }


        /// <summary>
        ///     Divides one Int256 value by another and returns the result.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <returns>The quotient of the division.</returns>
        public static Int256 Divide(Int256 dividend, Int256 divisor)
        {
            Int256 integer;
            return DivRem(dividend, divisor, out integer);
        }


        /// <summary>
        ///     Performs integer division on two Int256 values and returns the remainder.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <returns>The remainder after dividing dividend by divisor.</returns>
        public static Int256 Remainder(Int256 dividend, Int256 divisor)
        {
            Int256 remainder;
            DivRem(dividend, divisor, out remainder);
            return remainder;
        }


        /// <summary>
        ///     Divides one Int256 value by another, returns the result, and returns the remainder in an output parameter.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <param name="remainder">
        ///     When this method returns, contains an Int256 value that represents the remainder from the
        ///     division. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     The quotient of the division.
        /// </returns>
        public static Int256 DivRem(Int256 dividend, Int256 divisor, out Int256 remainder)
        {
            if (divisor == 0)
            {
                throw new DivideByZeroException();
            }
            int dividendSign = dividend.Sign;
            dividend = dividendSign < 0 ? -dividend : dividend;
            int divisorSign = divisor.Sign;
            divisor = divisorSign < 0 ? -divisor : divisor;


            uint[] quotient;
            uint[] rem;
            MathUtils.DivModUnsigned(dividend.ToUIn32Array(), divisor.ToUIn32Array(), out quotient, out rem);
            remainder = new Int256(1, rem);
            return new Int256(dividendSign * divisorSign, quotient);
        }


        /// <summary>
        ///     Converts an Int256 value to an unsigned long array.
        /// </summary>
        /// <returns>
        ///     The value of the current Int256 object converted to an array of unsigned integers.
        /// </returns>
        public ulong[] ToUIn64Array()
        {
            return new[] { _d, _c, _b, _a };
        }


        /// <summary>
        ///     Converts an Int256 value to an unsigned integer array.
        /// </summary>
        /// <returns>The value of the current Int256 object converted to an array of unsigned integers.</returns>
        public uint[] ToUIn32Array()
        {
            var ints = new uint[8];
            ulong[] ulongs = ToUIn64Array();
            Buffer.BlockCopy(ulongs, 0, ints, 0, 32);
            return ints;
        }


        /// <summary>
        ///     Returns the product of two Int256 values.
        /// </summary>
        /// <param name="left">The first number to multiply.</param>
        /// <param name="right">The second number to multiply.</param>
        /// <returns>The product of the left and right parameters.</returns>
        public static Int256 Multiply(Int256 left, Int256 right)
        {
            int leftSign = left.Sign;
            left = leftSign < 0 ? -left : left;
            int rightSign = right.Sign;
            right = rightSign < 0 ? -right : right;


            uint[] xInts = left.ToUIn32Array();
            uint[] yInts = right.ToUIn32Array();
            var mulInts = new uint[16];


            for (int i = 0; i < xInts.Length; i++)
            {
                int index = i;
                ulong remainder = 0;
                foreach (uint yi in yInts)
                {
                    remainder = remainder + (ulong)xInts[i] * yi + mulInts[index];
                    mulInts[index++] = (uint)remainder;
                    remainder = remainder >> 32;
                }


                while (remainder != 0)
                {
                    remainder += mulInts[index];
                    mulInts[index++] = (uint)remainder;
                    remainder = remainder >> 32;
                }
            }
            return new Int256(leftSign * rightSign, mulInts);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Boolean" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(bool value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Byte" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(byte value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Char" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(char value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="System.Decimal" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Int256(decimal value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="System.Double" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Int256(double value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Int16" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(short value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Int32" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(int value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Int64" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(long value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.SByte" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(sbyte value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="System.Single" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Int256(float value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.UInt16" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(ushort value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.UInt32" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(uint value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.UInt64" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int256(ulong value)
        {
            return new Int256(value);
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Boolean" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator bool(Int256 value)
        {
            return value.Sign != 0;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Byte" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator byte(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            if ((value < byte.MinValue) || (value > byte.MaxValue))
            {
                throw new OverflowException();
            }


            return (byte)value._d;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Char" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator char(Int256 value)
        {
            if (value.Sign == 0)
            {
                return (char)0;
            }


            if ((value < char.MinValue) || (value > char.MaxValue))
            {
                throw new OverflowException();
            }


            return (char)(ushort)value._d;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Decimal" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator decimal(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            if ((value < (Int256)decimal.MinValue) || (value > (Int256)decimal.MaxValue))
            {
                throw new OverflowException();
            }


            return new decimal((int)(value._d & 0xFFFFFFFF), (int)(value._d >> 32), (int)(value._c & 0xFFFFFFFF), value.Sign < 0, 0);
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Double" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator double(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            double d;
            NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
            if (!double.TryParse(value.ToString(nfi), NumberStyles.Number, nfi, out d))
            {
                throw new OverflowException();
            }


            return d;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Single" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator float(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            float f;
            NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
            if (!float.TryParse(value.ToString(nfi), NumberStyles.Number, nfi, out f))
            {
                throw new OverflowException();
            }


            return f;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Int16" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator short(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            if ((value < short.MinValue) || (value > short.MaxValue))
            {
                throw new OverflowException();
            }


            return (short)value._d;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Int32" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator int(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            if ((value < int.MinValue) || (value > int.MaxValue))
            {
                throw new OverflowException();
            }


            return ((int)value._d);
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.Int64" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator long(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            if ((value < long.MinValue) || (value > long.MaxValue))
            {
                throw new OverflowException();
            }


            return (long)value._d;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.UInt32" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator uint(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            if ((value < uint.MinValue) || (value > uint.MaxValue))
            {
                throw new OverflowException();
            }


            return (uint)value._d;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.UInt16" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator ushort(Int256 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }


            if ((value < ushort.MinValue) || (value > ushort.MaxValue))
            {
                throw new OverflowException();
            }


            return (ushort)value._d;
        }


        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int256" /> to <see cref="System.UInt64" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator ulong(Int256 value)
        {
            if ((value < ushort.MinValue) || (value > ushort.MaxValue))
            {
                throw new OverflowException();
            }


            return value._d;
        }


        /// <summary>
        ///     Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator >(Int256 left, Int256 right)
        {
            return Compare(left, right) > 0;
        }


        /// <summary>
        ///     Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator <(Int256 left, Int256 right)
        {
            return Compare(left, right) < 0;
        }


        /// <summary>
        ///     Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator >=(Int256 left, Int256 right)
        {
            return Compare(left, right) >= 0;
        }


        /// <summary>
        ///     Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator <=(Int256 left, Int256 right)
        {
            return Compare(left, right) <= 0;
        }


        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Int256 left, Int256 right)
        {
            return Compare(left, right) != 0;
        }


        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Int256 left, Int256 right)
        {
            return Compare(left, right) == 0;
        }


        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int256 operator +(Int256 value)
        {
            return value;
        }


        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int256 operator -(Int256 value)
        {
            return Negate(value);
        }


        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int256 operator +(Int256 left, Int256 right)
        {
            left._a += right._a;
            left._b += right._b;
            if (left._b < right._b)
            {
                left._a++;
            }
            left._c += right._c;
            if (left._c < right._c)
            {
                left._b++;
                if (left._b < left._b - 1)
                {
                    left._a++;
                }
            }
            left._d += right._d;
            if (left._d < right._d)
            {
                left._c++;
                if (left._c < left._c - 1)
                {
                    left._b++;
                    if (left._b < left._b - 1)
                    {
                        left._a++;
                    }
                }
            }


            return left;
        }


        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int256 operator -(Int256 left, Int256 right)
        {
            return left + -right;
        }


        /// <summary>
        ///     Implements the operator %.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int256 operator %(Int256 dividend, Int256 divisor)
        {
            return Remainder(dividend, divisor);
        }


        /// <summary>
        ///     Implements the operator /.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int256 operator /(Int256 dividend, Int256 divisor)
        {
            return Divide(dividend, divisor);
        }


        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int256 operator *(Int256 left, Int256 right)
        {
            return Multiply(left, right);
        }


        /// <summary>
        ///     Implements the operator &gt;&gt;.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>The result of the operator.</returns>
        public static Int256 operator >>(Int256 value, int shift)
        {
            if (shift == 0)
            {
                return value;
            }


            ulong[] bits = MathUtils.ShiftRight(value.ToUIn64Array(), shift);
            value._a = bits[0];
            value._b = bits[1];
            value._c = bits[2];
            value._d = bits[3];


            return value;
        }


        /// <summary>
        ///     Implements the operator &lt;&lt;.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>The result of the operator.</returns>
        public static Int256 operator <<(Int256 value, int shift)
        {
            if (shift == 0)
            {
                return value;
            }


            ulong[] bits = MathUtils.ShiftLeft(value.ToUIn64Array(), shift);
            value._a = bits[0];
            value._b = bits[1];
            value._c = bits[2];
            value._d = bits[3];


            return value;
        }


        /// <summary>
        ///     Implements the operator |.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Int256 operator |(Int256 left, Int256 right)
        {
            if (left == 0)
            {
                return right;
            }


            if (right == 0)
            {
                return left;
            }


            left._a |= right._a;
            left._b |= right._b;
            left._c |= right._c;
            left._d |= right._d;
            return left;
        }


        /// <summary>
        ///     Implements the operator &amp;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Int256 operator &(Int256 left, Int256 right)
        {
            if (left == 0 || right == 0)
            {
                return Zero;
            }


            left._a &= right._a;
            left._b &= right._b;
            left._c &= right._c;
            left._d &= right._d;
            return left;
        }


        /// <summary>
        ///     Implements the operator ~.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Int256 operator ~(Int256 value)
        {
            return new Int256(~value._a, ~value._b, ~value._c, ~value._d);
        }


        /// <summary>
        ///     Implements the operator ++.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Int256 operator ++(Int256 value)
        {
            return value + 1;
        }


        /// <summary>
        ///     Implements the operator --.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Int256 operator --(Int256 value)
        {
            return value - 1;
        }
    }

    /// <summary>
    ///     Represents a 128-bit signed integer.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 16)]
    [Serializable]
    public struct Int128 : IComparable<Int128>, IComparable, IEquatable<Int128>, IFormattable
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(0)]
        private ulong _lo;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [FieldOffset(8)]
        private ulong _hi;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return "0x" + ToString("X1"); }
        }

        private const ulong NegativeSignMask = 0x1UL << 63;

        /// <summary>
        ///     Gets a value that represents the number 0 (zero).
        /// </summary>
        public static Int128 Zero = GetZero();

        /// <summary>
        ///     Represents the largest possible value of an Int128.
        /// </summary>
        public static Int128 MaxValue = GetMaxValue();

        /// <summary>
        ///     Represents the smallest possible value of an Int128.
        /// </summary>
        public static Int128 MinValue = GetMinValue();

        private static Int128 GetMaxValue()
        {
            return new Int128(long.MaxValue, ulong.MaxValue);
        }

        private static Int128 GetMinValue()
        {
            return -GetMaxValue();
        }

        private static Int128 GetZero()
        {
            return new Int128();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(byte value)
        {
            _hi = 0;
            _lo = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public Int128(bool value)
        {
            _hi = 0;
            _lo = (ulong)(value ? 1 : 0);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(char value)
        {
            _hi = 0;
            _lo = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(decimal value)
        {
            bool isNegative = value < 0;
            uint[] bits = decimal.GetBits(value).ConvertAll(i => (uint)i);
            uint scale = (bits[3] >> 16) & 0x1F;
            if (scale > 0)
            {
                uint[] quotient;
                uint[] reminder;
                MathUtils.DivModUnsigned(bits, new[] { 10U * scale }, out quotient, out reminder);

                bits = quotient;
            }

            _hi = bits[2];
            _lo = bits[0] | (ulong)bits[1] << 32;

            if (isNegative)
            {
                Negate();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(double value)
            : this((decimal)value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(float value)
            : this((decimal)value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(short value)
            : this((int)value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(int value)
            : this((long)value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(long value)
        {
            _hi = unchecked((ulong)(value < 0 ? ~0 : 0));
            _lo = (ulong)value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(sbyte value)
            : this((long)value)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(ushort value)
        {
            _hi = 0;
            _lo = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(uint value)
        {
            _hi = 0;
            _lo = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(ulong value)
        {
            _hi = 0;
            _lo = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(Guid value)
        {
            var int128 = value.ToByteArray().ToInt128(0);
            _hi = int128.High;
            _lo = int128.Low;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Int128(Int256 value)
        {
            ulong[] values = value.ToUIn64Array();
            _hi = values[1];
            _lo = values[0];
        }

        public Int128(ulong hi, ulong lo)
        {
            _hi = hi;
            _lo = lo;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Int128" /> struct.
        /// </summary>
        /// <param name="sign">The sign.</param>
        /// <param name="ints">The ints.</param>
        public Int128(int sign, uint[] ints)
        {
            if (ints == null)
            {
                throw new ArgumentNullException("ints");
            }

            var value = new ulong[2];
            for (int i = 0; i < ints.Length && i < 4; i++)
            {
                Buffer.BlockCopy(ints[i].ToBytes(), 0, value, i * 4, 4);
            }

            _hi = value[1];
            _lo = value[0];

            if (sign < 0 && (_hi > 0 || _lo > 0))
            {
                // We use here two's complement numbers representation,
                // hence such operations for negative numbers.
                Negate();
                _hi |= NegativeSignMask; // Ensure negative sign.
            }
        }

        /// <summary>
        ///     Higher 64 bits.
        /// </summary>
        public ulong High
        {
            get { return _hi; }
        }

        /// <summary>
        ///     Lower 64 bits.
        /// </summary>
        public ulong Low
        {
            get { return _lo; }
        }

        /// <summary>
        ///     Gets a number that indicates the sign (negative, positive, or zero) of the current Int128 object.
        /// </summary>
        /// <value>A number that indicates the sign of the Int128 object</value>
        public int Sign
        {
            get
            {
                if (_hi == 0 && _lo == 0)
                {
                    return 0;
                }

                return ((_hi & NegativeSignMask) == 0) ? 1 : -1;
            }
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return _hi.GetHashCode() ^ _lo.GetHashCode();
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        ///     true if obj has the same value as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified Int64 value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        ///     true if obj has the same value as this instance; otherwise, false.
        /// </returns>
        public bool Equals(Int128 obj)
        {
            return _hi == obj._hi && _lo == obj._lo;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format. Only x, X, g, G, d, D are supported.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            if (!string.IsNullOrEmpty(format))
            {
                char ch = format[0];
                if ((ch == 'x') || (ch == 'X'))
                {
                    int min;
                    int.TryParse(format.Substring(1).Trim(), out min);
                    return this.ToBytes(false).ToHexString(ch == 'X', min, trimZeros: true);
                }

                if (((ch != 'G') && (ch != 'g')) && ((ch != 'D') && (ch != 'd')))
                {
                    throw new NotSupportedException("Not supported format: " + format);
                }
            }

            return ToString((NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo)));
        }

        private string ToString(NumberFormatInfo info)
        {
            if (Sign == 0)
            {
                return "0";
            }

            var sb = new StringBuilder();
            var ten = new Int128(10);
            Int128 current = Sign < 0 ? -this : this;
            Int128 r;
            while (true)
            {
                current = DivRem(current, ten, out r);
                if (r._lo > 0 || current.Sign != 0 || (sb.Length == 0))
                {
                    sb.Insert(0, (char)('0' + r._lo));
                }
                if (current.Sign == 0)
                {
                    break;
                }
            }

            string s = sb.ToString();
            if ((Sign < 0) && (s != "0"))
            {
                return info.NegativeSign + s;
            }

            return s;
        }

        /// <summary>
        ///     Converts the numeric value to an equivalent object. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="conversionType">The target conversion type.</param>
        /// <param name="provider">An object that supplies culture-specific information about the conversion.</param>
        /// <param name="asLittleEndian">As little endian.</param>
        /// <param name="value">
        ///     When this method returns, contains the value that is equivalent to the numeric value, if the
        ///     conversion succeeded, or is null if the conversion failed. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if this value was converted successfully; otherwise, false.</returns>
        public bool TryConvert(Type conversionType, IFormatProvider provider, bool asLittleEndian, out object value)
        {
            if (conversionType == typeof(bool))
            {
                value = (bool)this;
                return true;
            }

            if (conversionType == typeof(byte))
            {
                value = (byte)this;
                return true;
            }

            if (conversionType == typeof(char))
            {
                value = (char)this;
                return true;
            }

            if (conversionType == typeof(decimal))
            {
                value = (decimal)this;
                return true;
            }

            if (conversionType == typeof(double))
            {
                value = (double)this;
                return true;
            }

            if (conversionType == typeof(short))
            {
                value = (short)this;
                return true;
            }

            if (conversionType == typeof(int))
            {
                value = (int)this;
                return true;
            }

            if (conversionType == typeof(long))
            {
                value = (long)this;
                return true;
            }

            if (conversionType == typeof(sbyte))
            {
                value = (sbyte)this;
                return true;
            }

            if (conversionType == typeof(float))
            {
                value = (float)this;
                return true;
            }

            if (conversionType == typeof(string))
            {
                value = ToString(null, provider);
                return true;
            }

            if (conversionType == typeof(ushort))
            {
                value = (ushort)this;
                return true;
            }

            if (conversionType == typeof(uint))
            {
                value = (uint)this;
                return true;
            }

            if (conversionType == typeof(ulong))
            {
                value = (ulong)this;
                return true;
            }

            if (conversionType == typeof(byte[]))
            {
                value = this.ToBytes(asLittleEndian);
                return true;
            }

            if (conversionType == typeof(Guid))
            {
                value = new Guid(this.ToBytes(asLittleEndian));
                return true;
            }

            if (conversionType == typeof(Int256))
            {
                value = (Int256)this;
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        ///     Converts the string representation of a number to its Int128 equivalent.
        /// </summary>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <returns>
        ///     A value that is equivalent to the number specified in the value parameter.
        /// </returns>
        public static Int128 Parse(string value)
        {
            return Parse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///     Converts the string representation of a number in a specified style format to its Int128 equivalent.
        /// </summary>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of the enumeration values that specify the permitted format of value.</param>
        /// <returns>
        ///     A value that is equivalent to the number specified in the value parameter.
        /// </returns>
        public static Int128 Parse(string value, NumberStyles style)
        {
            return Parse(value, style, NumberFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///     Converts the string representation of a number in a culture-specific format to its Int128 equivalent.
        /// </summary>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about value.</param>
        /// <returns>
        ///     A value that is equivalent to the number specified in the value parameter.
        /// </returns>
        public static Int128 Parse(string value, IFormatProvider provider)
        {
            return Parse(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
        }

        /// <summary>
        ///     Converts the string representation of a number in a specified style and culture-specific format to its Int128
        ///     equivalent.
        /// </summary>
        /// <param name="value">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of the enumeration values that specify the permitted format of value.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about value.</param>
        /// <returns>A value that is equivalent to the number specified in the value parameter.</returns>
        public static Int128 Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            Int128 result;
            if (!TryParse(value, style, provider, out result))
            {
                throw new ArgumentException(null, "value");
            }

            return result;
        }

        /// <summary>
        ///     Tries to convert the string representation of a number to its Int128 equivalent, and returns a value that indicates
        ///     whether the conversion succeeded..
        /// </summary>
        /// <param name="value">The string representation of a number.</param>
        /// <param name="result">
        ///     When this method returns, contains the Int128 equivalent to the number that is contained in value,
        ///     or Int128.Zero if the conversion failed. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     true if the value parameter was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string value, out Int128 result)
        {
            return TryParse(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
        }

        /// <summary>
        ///     Tries to convert the string representation of a number in a specified style and culture-specific format to its
        ///     Int128 equivalent, and returns a value that indicates whether the conversion succeeded..
        /// </summary>
        /// <param name="value">
        ///     The string representation of a number. The string is interpreted using the style specified by
        ///     style.
        /// </param>
        /// <param name="style">
        ///     A bitwise combination of enumeration values that indicates the style elements that can be present
        ///     in value. A typical value to specify is NumberStyles.Integer.
        /// </param>
        /// <param name="provider">An object that supplies culture-specific formatting information about value.</param>
        /// <param name="result">
        ///     When this method returns, contains the Int128 equivalent to the number that is contained in value,
        ///     or Int128.Zero if the conversion failed. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out Int128 result)
        {
            result = Zero;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value.StartsWith("x", StringComparison.OrdinalIgnoreCase))
            {
                style |= NumberStyles.AllowHexSpecifier;
                value = value.Substring(1);
            }
            else if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                style |= NumberStyles.AllowHexSpecifier;
                value = value.Substring(2);
            }

            if ((style & NumberStyles.AllowHexSpecifier) == NumberStyles.AllowHexSpecifier)
            {
                return TryParseHex(value, out result);
            }

            return TryParseNum(value, out result);
        }

        private static bool TryParseHex(string value, out Int128 result)
        {
            if (value.Length > 32)
            {
                throw new OverflowException();
            }

            result = Zero;
            bool hi = false;
            int pos = 0;
            for (int i = value.Length - 1; i >= 0; i--)
            {
                char ch = value[i];
                ulong b;
                if ((ch >= '0') && (ch <= '9'))
                {
                    b = (ulong)(ch - '0');
                }
                else if ((ch >= 'A') && (ch <= 'F'))
                {
                    b = (ulong)(ch - 'A' + 10);
                }
                else if ((ch >= 'a') && (ch <= 'f'))
                {
                    b = (ulong)(ch - 'a' + 10);
                }
                else
                {
                    return false;
                }

                if (hi)
                {
                    result._hi |= b << pos;
                    pos += 4;
                }
                else
                {
                    result._lo |= b << pos;
                    pos += 4;
                    if (pos == 64)
                    {
                        pos = 0;
                        hi = true;
                    }
                }
            }
            return true;
        }

        private static bool TryParseNum(string value, out Int128 result)
        {
            result = Zero;
            bool isFirstChar = true;
            sbyte sign;
            sign = 1;
            foreach (char ch in value)
            {
                sbyte b;
                if (ch == '+' && isFirstChar)
                {
                    isFirstChar = false;
                    continue;
                }
                if (ch == '-' && isFirstChar)
                {
                    isFirstChar = false;
                    sign = -1;
                    continue;
                }
                if ((ch >= '0') && (ch <= '9'))
                {
                    b = (sbyte)(ch - '0');
                }
                else
                {
                    return false;
                }

                result = 10 * result;
                result += (b * sign);
                sign = 1;
                isFirstChar = false;
            }
            return true;
        }

        /// <summary>
        ///     Converts the value of this instance to an <see cref="T:System.Object" /> of the specified
        ///     <see cref="T:System.Type" /> that has an equivalent value, using the specified culture-specific formatting
        ///     information.
        /// </summary>
        /// <param name="conversionType">The <see cref="T:System.Type" /> to which the value of this instance is converted.</param>
        /// <param name="provider">
        ///     An <see cref="T:System.IFormatProvider" /> interface implementation that supplies
        ///     culture-specific formatting information.
        /// </param>
        /// <param name="asLittleEndian">As little endian.</param>
        /// <returns>
        ///     An <see cref="T:System.Object" /> instance of type <paramref name="conversionType" /> whose value is equivalent to
        ///     the value of this instance.
        /// </returns>
        public object ToType(Type conversionType, IFormatProvider provider, bool asLittleEndian)
        {
            object value;
            if (TryConvert(conversionType, provider, asLittleEndian, out value))
            {
                return value;
            }

            throw new InvalidCastException();
        }

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
        ///     Meaning Less than zero This instance is less than <paramref name="obj" />. Zero This instance is equal to
        ///     <paramref name="obj" />. Greater than zero This instance is greater than <paramref name="obj" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="obj" /> is not the same type as this instance.
        /// </exception>
        int IComparable.CompareTo(object obj)
        {
            return Compare(this, obj);
        }

        /// <summary>
        ///     Compares two Int128 values and returns an integer that indicates whether the first value is less than, equal to, or
        ///     greater than the second value.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>A signed integer that indicates the relative values of left and right, as shown in the following table.</returns>
        public static int Compare(Int128 left, object right)
        {
            if (right is Int128)
            {
                return Compare(left, (Int128)right);
            }

            // NOTE: this could be optimized type per type
            if (right is bool)
            {
                return Compare(left, new Int128((bool)right));
            }

            if (right is byte)
            {
                return Compare(left, new Int128((byte)right));
            }

            if (right is char)
            {
                return Compare(left, new Int128((char)right));
            }

            if (right is decimal)
            {
                return Compare(left, new Int128((decimal)right));
            }

            if (right is double)
            {
                return Compare(left, new Int128((double)right));
            }

            if (right is short)
            {
                return Compare(left, new Int128((short)right));
            }

            if (right is int)
            {
                return Compare(left, new Int128((int)right));
            }

            if (right is long)
            {
                return Compare(left, new Int128((long)right));
            }

            if (right is sbyte)
            {
                return Compare(left, new Int128((sbyte)right));
            }

            if (right is float)
            {
                return Compare(left, new Int128((float)right));
            }

            if (right is ushort)
            {
                return Compare(left, new Int128((ushort)right));
            }

            if (right is uint)
            {
                return Compare(left, new Int128((uint)right));
            }

            if (right is ulong)
            {
                return Compare(left, new Int128((ulong)right));
            }

            var bytes = right as byte[];
            if ((bytes != null) && (bytes.Length == 16))
            {
                // TODO: ensure endian.
                return Compare(left, bytes.ToInt128(0));
            }

            if (right is Guid)
            {
                return Compare(left, new Int128((Guid)right));
            }

            throw new ArgumentException();
        }

        /// <summary>
        ///     Compares two 128-bit signed integer values and returns an integer that indicates whether the first value is less
        ///     than, equal to, or greater than the second value.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>
        ///     A signed number indicating the relative values of this instance and value.
        /// </returns>
        public static int Compare(Int128 left, Int128 right)
        {
            int leftSign = left.Sign;
            int rightSign = right.Sign;

            if (leftSign == 0 && rightSign == 0)
            {
                return 0;
            }

            if (leftSign >= 0 && rightSign < 0)
            {
                return 1;
            }

            if (leftSign < 0 && rightSign >= 0)
            {
                return -1;
            }

            if (left._hi != right._hi)
            {
                return left._hi.CompareTo(right._hi);
            }

            return left._lo.CompareTo(right._lo);
        }

        /// <summary>
        ///     Compares this instance to a specified 128-bit signed integer and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An integer to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and value.</returns>
        public int CompareTo(Int128 value)
        {
            return Compare(this, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Not()
        {
            _hi = ~_hi;
            _lo = ~_lo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Negate()
        {
            Not();
            this++;
        }

        /// <summary>
        ///     Negates a specified Int128 value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The result of the value parameter multiplied by negative one (-1).</returns>
        public static Int128 Negate(Int128 value)
        {
            value.Negate();
            return value;
        }

        /// <summary>
        ///     Gets the absolute value this object.
        /// </summary>
        /// <returns>The absolute value.</returns>
        public Int128 ToAbs()
        {
            return Abs(this);
        }

        /// <summary>
        ///     Gets the absolute value of an Int128 object.
        /// </summary>
        /// <param name="value">A number.</param>
        /// <returns>
        ///     The absolute value.
        /// </returns>
        public static Int128 Abs(Int128 value)
        {
            if (value.Sign < 0)
            {
                return -value;
            }

            return value;
        }

        /// <summary>
        ///     Adds two Int128 values and returns the result.
        /// </summary>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        /// <returns>The sum of left and right.</returns>
        public static Int128 Add(Int128 left, Int128 right)
        {
            return left + right;
        }

        /// <summary>
        ///     Subtracts one Int128 value from another and returns the result.
        /// </summary>
        /// <param name="left">The value to subtract from (the minuend).</param>
        /// <param name="right">The value to subtract (the subtrahend).</param>
        /// <returns>The result of subtracting right from left.</returns>
        public static Int128 Subtract(Int128 left, Int128 right)
        {
            return left - right;
        }

        /// <summary>
        ///     Divides one Int128 value by another and returns the result.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <returns>The quotient of the division.</returns>
        public static Int128 Divide(Int128 dividend, Int128 divisor)
        {
            Int128 integer;
            return DivRem(dividend, divisor, out integer);
        }

        /// <summary>
        ///     Divides one Int128 value by another, returns the result, and returns the remainder in an output parameter.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <param name="remainder">
        ///     When this method returns, contains an Int128 value that represents the remainder from the
        ///     division. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     The quotient of the division.
        /// </returns>
        public static Int128 DivRem(Int128 dividend, Int128 divisor, out Int128 remainder)
        {
            if (divisor == 0)
            {
                throw new DivideByZeroException();
            }
            int dividendSign = dividend.Sign;
            dividend = dividendSign < 0 ? -dividend : dividend;
            int divisorSign = divisor.Sign;
            divisor = divisorSign < 0 ? -divisor : divisor;

            uint[] quotient;
            uint[] rem;
            MathUtils.DivModUnsigned(dividend.ToUIn32Array(), divisor.ToUIn32Array(), out quotient, out rem);
            remainder = new Int128(1, rem);
            return new Int128(dividendSign * divisorSign, quotient);
        }

        /// <summary>
        ///     Performs integer division on two Int128 values and returns the remainder.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <returns>The remainder after dividing dividend by divisor.</returns>
        public static Int128 Remainder(Int128 dividend, Int128 divisor)
        {
            Int128 remainder;
            DivRem(dividend, divisor, out remainder);
            return remainder;
        }

        /// <summary>
        ///     Converts an Int128 value to an unsigned long array.
        /// </summary>
        /// <returns>
        ///     The value of the current Int128 object converted to an array of unsigned integers.
        /// </returns>
        public ulong[] ToUIn64Array()
        {
            return new[] { _lo, _hi };
        }

        /// <summary>
        ///     Converts an Int128 value to an unsigned integer array.
        /// </summary>
        /// <returns>The value of the current Int128 object converted to an array of unsigned integers.</returns>
        public uint[] ToUIn32Array()
        {
            var ints = new uint[4];
            ulong[] ulongs = ToUIn64Array();
            Buffer.BlockCopy(ulongs, 0, ints, 0, 16);
            return ints;
        }

        /// <summary>
        ///     Returns the product of two Int128 values.
        /// </summary>
        /// <param name="left">The first number to multiply.</param>
        /// <param name="right">The second number to multiply.</param>
        /// <returns>The product of the left and right parameters.</returns>
        public static Int128 Multiply(Int128 left, Int128 right)
        {
            int leftSign = left.Sign;
            left = leftSign < 0 ? -left : left;
            int rightSign = right.Sign;
            right = rightSign < 0 ? -right : right;

            uint[] xInts = left.ToUIn32Array();
            uint[] yInts = right.ToUIn32Array();
            var mulInts = new uint[8];

            for (int i = 0; i < xInts.Length; i++)
            {
                int index = i;
                ulong remainder = 0;
                foreach (uint yi in yInts)
                {
                    remainder = remainder + (ulong)xInts[i] * yi + mulInts[index];
                    mulInts[index++] = (uint)remainder;
                    remainder = remainder >> 32;
                }

                while (remainder != 0)
                {
                    remainder += mulInts[index];
                    mulInts[index++] = (uint)remainder;
                    remainder = remainder >> 32;
                }
            }
            return new Int128(leftSign * rightSign, mulInts);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Boolean" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(bool value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Byte" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(byte value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Char" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(char value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="System.Decimal" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Int128(decimal value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="System.Double" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Int128(double value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Int16" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(short value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Int32" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(int value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.Int64" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(long value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.SByte" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(sbyte value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="System.Single" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Int128(float value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.UInt16" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(ushort value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.UInt32" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(uint value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="System.UInt64" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Int128(ulong value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Int256" /> to <see cref="Int128" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Int128(Int256 value)
        {
            return new Int128(value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Int128" /> to <see cref="Int256" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator Int256(Int128 value)
        {
            return new Int256(value);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Boolean" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator bool(Int128 value)
        {
            return value.Sign != 0;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Byte" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator byte(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value < byte.MinValue) || (value > byte.MaxValue))
            {
                throw new OverflowException();
            }

            return (byte)value._lo;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Char" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator char(Int128 value)
        {
            if (value.Sign == 0)
            {
                return (char)0;
            }

            if ((value < char.MinValue) || (value > char.MaxValue))
            {
                throw new OverflowException();
            }

            return (char)(ushort)value._lo;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Decimal" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator decimal(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            return new decimal((int)(value._lo & 0xFFFFFFFF), (int)(value._lo >> 32), (int)(value._hi & 0xFFFFFFFF), value.Sign < 0, 0);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Double" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator double(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            double d;
            NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
            if (!double.TryParse(value.ToString(nfi), NumberStyles.Number, nfi, out d))
            {
                throw new OverflowException();
            }

            return d;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Single" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator float(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            float f;
            NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
            if (!float.TryParse(value.ToString(nfi), NumberStyles.Number, nfi, out f))
            {
                throw new OverflowException();
            }

            return f;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Int16" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator short(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value < short.MinValue) || (value > short.MaxValue))
            {
                throw new OverflowException();
            }

            return (short)value._lo;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Int32" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator int(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value < int.MinValue) || (value > int.MaxValue))
            {
                throw new OverflowException();
            }

            return (int)value._lo;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.Int64" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator long(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value < long.MinValue) || (value > long.MaxValue))
            {
                throw new OverflowException();
            }

            return (long)value._lo;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.UInt32" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator uint(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value < uint.MinValue) || (value > uint.MaxValue))
            {
                throw new OverflowException();
            }

            return (uint)value._lo;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.UInt16" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator ushort(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value < ushort.MinValue) || (value > ushort.MaxValue))
            {
                throw new OverflowException();
            }

            return (ushort)value._lo;
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Int128" /> to <see cref="System.UInt64" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static explicit operator ulong(Int128 value)
        {
            if (value.Sign == 0)
            {
                return 0;
            }

            if ((value < ulong.MinValue) || (value > ulong.MaxValue))
            {
                throw new OverflowException();
            }

            return value._lo;
        }

        /// <summary>
        ///     Implements the operator &gt;.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator >(Int128 left, Int128 right)
        {
            return Compare(left, right) > 0;
        }

        /// <summary>
        ///     Implements the operator &lt;.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator <(Int128 left, Int128 right)
        {
            return Compare(left, right) < 0;
        }

        /// <summary>
        ///     Implements the operator &gt;=.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator >=(Int128 left, Int128 right)
        {
            return Compare(left, right) >= 0;
        }

        /// <summary>
        ///     Implements the operator &lt;=.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator <=(Int128 left, Int128 right)
        {
            return Compare(left, right) <= 0;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Int128 left, Int128 right)
        {
            return Compare(left, right) != 0;
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Int128 left, Int128 right)
        {
            return Compare(left, right) == 0;
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int128 operator +(Int128 value)
        {
            return value;
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int128 operator -(Int128 value)
        {
            return Negate(value);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int128 operator +(Int128 left, Int128 right)
        {
            left._hi += right._hi;
            left._lo += right._lo;

            if (left._lo < right._lo)
            {
                left._hi++;
            }

            return left;
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int128 operator -(Int128 left, Int128 right)
        {
            return left + -right;
        }


        /// <summary>
        ///     Implements the operator %.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int128 operator %(Int128 dividend, Int128 divisor)
        {
            return Remainder(dividend, divisor);
        }

        /// <summary>
        ///     Implements the operator /.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int128 operator /(Int128 dividend, Int128 divisor)
        {
            return Divide(dividend, divisor);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="left">The x.</param>
        /// <param name="right">The y.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Int128 operator *(Int128 left, Int128 right)
        {
            return Multiply(left, right);
        }

        /// <summary>
        ///     Implements the operator &gt;&gt;.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>The result of the operator.</returns>
        public static Int128 operator >>(Int128 value, int shift)
        {
            if (shift == 0)
            {
                return value;
            }

            ulong[] bits = MathUtils.ShiftRight(value.ToUIn64Array(), shift);
            value._hi = bits[0];
            value._lo = bits[1];

            return value;
        }

        /// <summary>
        ///     Implements the operator &lt;&lt;.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>The result of the operator.</returns>
        public static Int128 operator <<(Int128 value, int shift)
        {
            if (shift == 0)
            {
                return value;
            }

            ulong[] bits = MathUtils.ShiftRight(value.ToUIn64Array(), shift);
            value._hi = bits[0];
            value._lo = bits[1];

            return value;
        }

        /// <summary>
        ///     Implements the operator |.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Int128 operator |(Int128 left, Int128 right)
        {
            if (left == 0)
            {
                return right;
            }

            if (right == 0)
            {
                return left;
            }

            Int128 result = left;
            result._hi |= right._hi;
            result._lo |= right._lo;
            return result;
        }

        /// <summary>
        ///     Implements the operator &amp;.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Int128 operator &(Int128 left, Int128 right)
        {
            if (left == 0 || right == 0)
            {
                return Zero;
            }

            Int128 result = left;
            result._hi &= right._hi;
            result._lo &= right._lo;
            return result;
        }

        /// <summary>
        ///     Implements the operator ~.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Int128 operator ~(Int128 value)
        {
            return new Int128(~value._hi, ~value._lo);
        }

        /// <summary>
        ///     Implements the operator ++.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Int128 operator ++(Int128 value)
        {
            return value + 1;
        }

        /// <summary>
        ///     Implements the operator --.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Int128 operator --(Int128 value)
        {
            return value - 1;
        }
    }

    /// <summary>
    ///     Bit converter methods which support explicit endian.
    /// </summary>
    public static class ExtendedBitConverter
    {
        /// <summary>
        ///     Indicates the byte order ("endianness") in which data is stored in this computer architecture.
        /// </summary>
        public static readonly bool IsLittleEndian = BitConverter.IsLittleEndian;


        #region Int16
        /// <summary>
        ///     Converts <see cref="short" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this short value, bool? asLittleEndian = null)
        {
            var buffer = new byte[2];
            value.ToBytes(buffer, 0, asLittleEndian);
            return buffer;
        }


        /// <summary>
        ///     Converts <see cref="short" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="buffer">Buffer at least 2 bytes.</param>
        /// <param name="offset">The starting position within <paramref name="buffer" />.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBytes(this short value, byte[] buffer, int offset = 0, bool? asLittleEndian = null)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }


            if (asLittleEndian.HasValue ? asLittleEndian.Value : IsLittleEndian)
            {
                buffer[offset] = (byte)value;
                buffer[offset + 1] = (byte)(value >> 8);
            }
            else
            {
                buffer[offset] = (byte)(value >> 8);
                buffer[offset + 1] = (byte)(value);
            }
        }


        /// <summary>
        ///     Converts array of bytes to <see cref="short" />.
        /// </summary>
        /// <param name="bytes">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="bytes" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <returns><see cref="short" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ToInt16(this byte[] bytes, int offset = 0, bool? asLittleEndian = null)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            if (bytes.Length <= offset)
            {
                throw new InvalidOperationException("Array length must be greater than offset.");
            }
            bool ale = GetIsLittleEndian(asLittleEndian);
            EnsureLength(ref bytes, 2, offset, ale);


            return (short)(ale ? bytes[offset] | bytes[offset + 1] << 8 : bytes[offset] << 8 | bytes[offset + 1]);
        }
        #endregion


        #region UInt16
        /// <summary>
        ///     Converts <see cref="ushort" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this ushort value, bool? asLittleEndian = null)
        {
            return unchecked((short)value).ToBytes(asLittleEndian);
        }


        /// <summary>
        ///     Converts <see cref="ushort" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="buffer">Buffer at least 2 bytes.</param>
        /// <param name="offset">The starting position within <paramref name="buffer" />.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBytes(this ushort value, byte[] buffer, int offset = 0, bool? asLittleEndian = null)
        {
            unchecked((short)value).ToBytes(buffer, offset, asLittleEndian);
        }


        /// <summary>
        ///     Converts array of bytes to <see cref="ushort" />.
        /// </summary>
        /// <param name="bytes">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="bytes" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <returns><see cref="ushort" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUInt16(this byte[] bytes, int offset = 0, bool? asLittleEndian = null)
        {
            return (ushort)bytes.ToInt16(offset, asLittleEndian);
        }
        #endregion


        #region Int32
        /// <summary>
        ///     Converts <see cref="int" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this int value, bool? asLittleEndian = null)
        {
            var buffer = new byte[4];
            value.ToBytes(buffer, 0, asLittleEndian);
            return buffer;
        }


        /// <summary>
        ///     Converts <see cref="int" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="buffer">Buffer at least 4 bytes.</param>
        /// <param name="offset">The starting position within <paramref name="buffer" />.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBytes(this int value, byte[] buffer, int offset = 0, bool? asLittleEndian = null)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }


            if (asLittleEndian.HasValue ? asLittleEndian.Value : IsLittleEndian)
            {
                buffer[offset] = (byte)value;
                buffer[offset + 1] = (byte)(value >> 8);
                buffer[offset + 2] = (byte)(value >> 16);
                buffer[offset + 3] = (byte)(value >> 24);
            }
            else
            {
                buffer[offset] = (byte)(value >> 24);
                buffer[offset + 1] = (byte)(value >> 16);
                buffer[offset + 2] = (byte)(value >> 8);
                buffer[offset + 3] = (byte)value;
            }
        }


        /// <summary>
        ///     Converts array of bytes to <see cref="int" />.
        /// </summary>
        /// <param name="bytes">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="bytes" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <returns><see cref="int" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32(this byte[] bytes, int offset = 0, bool? asLittleEndian = null)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            if (bytes.Length <= offset)
            {
                throw new InvalidOperationException("Array length must be greater than offset.");
            }
            bool ale = GetIsLittleEndian(asLittleEndian);
            EnsureLength(ref bytes, 4, offset, ale);


            return (ale)
                ? bytes[offset] | bytes[offset + 1] << 8 | bytes[offset + 2] << 16 | bytes[offset + 3] << 24
                : bytes[offset] << 24 | bytes[offset + 1] << 16 | bytes[offset + 2] << 8 | bytes[offset + 3];
        }
        #endregion


        private static void EnsureLength(ref byte[] bytes, int minLength, int offset, bool ale)
        {
            int bytesLength = bytes.Length - offset;
            if (bytesLength < minLength)
            {
                var b = new byte[minLength];
                Buffer.BlockCopy(bytes, offset, b, ale ? 0 : minLength - bytesLength, bytesLength);
                bytes = b;
            }
        }


        private static bool GetIsLittleEndian(bool? asLittleEndian)
        {
            return asLittleEndian.HasValue ? asLittleEndian.Value : IsLittleEndian;
        }


        #region UInt32
        /// <summary>
        ///     Converts <see cref="uint" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this uint value, bool? asLittleEndian = null)
        {
            return unchecked((int)value).ToBytes(asLittleEndian);
        }


        /// <summary>
        ///     Converts <see cref="uint" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="buffer">Buffer at least 4 bytes.</param>
        /// <param name="offset">The starting position within <paramref name="buffer" />.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBytes(this uint value, byte[] buffer, int offset = 0, bool? asLittleEndian = null)
        {
            unchecked((int)value).ToBytes(buffer, offset, asLittleEndian);
        }


        /// <summary>
        ///     Converts array of bytes to <see cref="uint" />.
        /// </summary>
        /// <param name="bytes">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="bytes" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <returns><see cref="uint" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(this byte[] bytes, int offset = 0, bool? asLittleEndian = null)
        {
            return (uint)bytes.ToInt32(offset, asLittleEndian);
        }
        #endregion


        #region Int64
        /// <summary>
        ///     Converts <see cref="long" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this long value, bool? asLittleEndian = null)
        {
            var buffer = new byte[8];
            value.ToBytes(buffer, 0, asLittleEndian);
            return buffer;
        }


        /// <summary>
        ///     Converts <see cref="long" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="buffer">Buffer at least 8 bytes.</param>
        /// <param name="offset">The starting position within <paramref name="buffer" />.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBytes(this long value, byte[] buffer, int offset = 0, bool? asLittleEndian = null)
        {
            if (asLittleEndian.HasValue ? asLittleEndian.Value : IsLittleEndian)
            {
                buffer[offset] = (byte)value;
                buffer[offset + 1] = (byte)(value >> 8);
                buffer[offset + 2] = (byte)(value >> 16);
                buffer[offset + 3] = (byte)(value >> 24);
                buffer[offset + 4] = (byte)(value >> 32);
                buffer[offset + 5] = (byte)(value >> 40);
                buffer[offset + 6] = (byte)(value >> 48);
                buffer[offset + 7] = (byte)(value >> 56);
            }
            else
            {
                buffer[offset] = (byte)(value >> 56);
                buffer[offset + 1] = (byte)(value >> 48);
                buffer[offset + 2] = (byte)(value >> 40);
                buffer[offset + 3] = (byte)(value >> 32);
                buffer[offset + 4] = (byte)(value >> 24);
                buffer[offset + 5] = (byte)(value >> 16);
                buffer[offset + 6] = (byte)(value >> 8);
                buffer[offset + 7] = (byte)value;
            }
        }


        /// <summary>
        ///     Converts array of bytes to <see cref="long" />.
        /// </summary>
        /// <param name="bytes">An array of bytes. </param>
        /// <param name="offset">The starting position within <paramref name="bytes" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <returns><see cref="long" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToInt64(this byte[] bytes, int offset = 0, bool? asLittleEndian = null)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            if (bytes.Length <= offset)
            {
                throw new InvalidOperationException("Array length must be greater than offset.");
            }
            bool ale = GetIsLittleEndian(asLittleEndian);
            EnsureLength(ref bytes, 8, offset, ale);


            return ale
                ? bytes[offset] | (long)bytes[offset + 1] << 8 | (long)bytes[offset + 2] << 16 | (long)bytes[offset + 3] << 24 | (long)bytes[offset + 4] << 32 |
                    (long)bytes[offset + 5] << 40 | (long)bytes[offset + 6] << 48 | (long)bytes[offset + 7] << 56
                : (long)bytes[offset] << 56 | (long)bytes[offset + 1] << 48 | (long)bytes[offset + 2] << 40 | (long)bytes[offset + 3] << 32 |
                    (long)bytes[offset + 4] << 24 | (long)bytes[offset + 5] << 16 | (long)bytes[offset + 6] << 8 | bytes[offset + 7];
        }
        #endregion


        #region UInt64
        /// <summary>
        ///     Converts <see cref="ulong" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        /// <returns>Array of bytes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this ulong value, bool? asLittleEndian = null)
        {
            return unchecked((long)value).ToBytes(asLittleEndian);
        }


        /// <summary>
        ///     Converts <see cref="ulong" /> to array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="buffer">Buffer at least 8 bytes.</param>
        /// <param name="offset">The starting position within <paramref name="buffer" />.</param>
        /// <param name="asLittleEndian">Convert to little endian.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBytes(this ulong value, byte[] buffer, int offset = 0, bool? asLittleEndian = null)
        {
            unchecked((long)value).ToBytes(buffer, offset, asLittleEndian);
        }


        /// <summary>
        ///     Converts array of bytes to <see cref="ulong" />.
        /// </summary>
        /// <param name="bytes">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="bytes" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <returns><see cref="ulong" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToUInt64(this byte[] bytes, int offset = 0, bool? asLittleEndian = null)
        {
            return (ulong)bytes.ToInt64(offset, asLittleEndian);
        }
        #endregion


        #region Int128
        /// <summary>
        ///     Converts an <see cref="Int128" /> value to an array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="buffer">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="buffer" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        public static void ToBytes(this Int128 value, byte[] buffer, int offset = 0, bool? asLittleEndian = null)
        {
            bool ale = GetIsLittleEndian(asLittleEndian);
            value.Low.ToBytes(buffer, ale ? offset : offset + 8, ale);
            value.High.ToBytes(buffer, ale ? offset + 8 : offset, ale);
        }


        /// <summary>
        ///     Converts an <see cref="Int128" /> value to a byte array.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <param name="trimZeros">Trim zero bytes from left or right, depending on endian.</param>
        /// <returns>Array of bytes.</returns>
        public static byte[] ToBytes(this Int128 value, bool? asLittleEndian = null, bool trimZeros = false)
        {
            var buffer = new byte[16];
            value.ToBytes(buffer, 0, asLittleEndian);


            if (trimZeros)
            {
                buffer = buffer.TrimZeros(asLittleEndian);
            }


            return buffer;
        }


        /// <summary>
        ///     Converts array of bytes to <see cref="Int128" />.
        /// </summary>
        /// <param name="bytes">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="bytes" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <returns><see cref="Int128" /> value.</returns>
        public static Int128 ToInt128(this byte[] bytes, int offset = 0, bool? asLittleEndian = null)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            if (bytes.Length <= offset)
            {
                throw new InvalidOperationException("Array length must be greater than offset.");
            }
            bool ale = GetIsLittleEndian(asLittleEndian);
            EnsureLength(ref bytes, 16, offset, ale);


            return new Int128(bytes.ToUInt64(ale ? offset + 8 : offset, ale), bytes.ToUInt64(ale ? offset : offset + 8, ale));
        }
        #endregion


        #region Int256
        /// <summary>
        ///     Converts an <see cref="Int256" /> value to an array of bytes.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="buffer">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="buffer" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        public static void ToBytes(this Int256 value, byte[] buffer, int offset = 0, bool? asLittleEndian = null)
        {
            bool ale = GetIsLittleEndian(asLittleEndian);


            value.D.ToBytes(buffer, ale ? offset : offset + 24, ale);
            value.C.ToBytes(buffer, ale ? offset + 8 : offset + 16, ale);
            value.B.ToBytes(buffer, ale ? offset + 16 : offset + 8, ale);
            value.A.ToBytes(buffer, ale ? offset + 24 : offset, ale);
        }


        /// <summary>
        ///     Converts an <see cref="Int256" /> value to a byte array.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <param name="trimZeros">Trim zero bytes from left or right, depending on endian.</param>
        /// <returns>Array of bytes.</returns>
        public static byte[] ToBytes(this Int256 value, bool? asLittleEndian = null, bool trimZeros = false)
        {
            var buffer = new byte[32];
            value.ToBytes(buffer, 0, asLittleEndian);


            if (trimZeros)
            {
                buffer = buffer.TrimZeros(asLittleEndian);
            }


            return buffer;
        }


        /// <summary>
        ///     Converts array of bytes to <see cref="Int256" />.
        /// </summary>
        /// <param name="bytes">An array of bytes.</param>
        /// <param name="offset">The starting position within <paramref name="bytes" />.</param>
        /// <param name="asLittleEndian">Convert from little endian.</param>
        /// <returns><see cref="Int256" /> value.</returns>
        public static Int256 ToInt256(this byte[] bytes, int offset = 0, bool? asLittleEndian = null)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            if (bytes.Length <= offset)
            {
                throw new InvalidOperationException("Array length must be greater than offset.");
            }
            bool ale = GetIsLittleEndian(asLittleEndian);
            EnsureLength(ref bytes, 32, offset, ale);


            ulong a = bytes.ToUInt64(ale ? offset + 24 : offset, ale);
            ulong b = bytes.ToUInt64(ale ? offset + 16 : offset + 8, ale);
            ulong c = bytes.ToUInt64(ale ? offset + 8 : offset + 16, ale);
            ulong d = bytes.ToUInt64(ale ? offset : offset + 24, ale);


            return new Int256(a, b, c, d);
        }
        #endregion
    }

    public static class MathUtils
    {
        private const ulong Base = 0x100000000;


        /// <summary>
        ///     Bitwise shift array of <see cref="ulong" />.
        /// </summary>
        /// <param name="values">Bits to shift. Lower bits have lower index in array.</param>
        /// <param name="shift">Shift amount in bits. Negative for left shift, positive for right shift.</param>
        /// <returns>Shifted values.</returns>
        public static ulong[] Shift(ulong[] values, int shift)
        {
            if (shift == 0)
            {
                return values;
            }
            return shift < 0 ? ShiftLeft(values, -shift) : ShiftRight(values, shift);
        }


        /// <summary>
        ///     Bitwise right shift.
        /// </summary>
        /// <param name="values">Bits to shift. Lower bits have lower index in array.</param>
        /// <param name="shift">Shift amount in bits.</param>
        /// <returns>Shifted values.</returns>
        public static ulong[] ShiftRight(ulong[] values, int shift)
        {
            if (shift < 0)
            {
                return ShiftLeft(values, -shift);
            }


            const int valueLength = sizeof(ulong) * 8;
            int length = values.Length;


            shift = shift % (length * valueLength);


            int shiftOffset = shift / valueLength;
            int bshift = shift % valueLength;


            var shifted = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                int ishift = i - shiftOffset;
                if (ishift < 0)
                {
                    continue;
                }
                shifted[ishift] |= values[i] >> bshift;
                if (bshift > 0 && i + 1 < length)
                {
                    shifted[ishift] |= values[i + 1] << valueLength - bshift;
                }
            }


            return shifted;
        }


        /// <summary>
        ///     Bitwise right shift.
        /// </summary>
        /// <param name="values">Bits to shift. Lower bits have lower index in array.</param>
        /// <param name="shift">Shift amount in bits.</param>
        /// <returns>Shifted values.</returns>
        public static ulong[] ShiftLeft(ulong[] values, int shift)
        {
            if (shift < 0)
            {
                return ShiftRight(values, -shift);
            }


            const int valueLength = sizeof(ulong) * 8;
            int length = values.Length;


            shift = shift % (length * valueLength);


            int shiftOffset = shift / valueLength;
            int bshift = shift % valueLength;


            var shifted = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                int ishift = i + shiftOffset;
                if (ishift >= length)
                {
                    continue;
                }
                shifted[ishift] |= values[i] << bshift;
                if (bshift > 0 && i - 1 >= 0)
                {
                    shifted[ishift] |= values[i - 1] >> valueLength - bshift;
                }
            }


            return shifted;
        }


        public static void GetPrimeMultipliers(this Int128 pq, out Int128 p, out Int128 q)
        {
            var pq256 = (Int256)pq;
            Int256 p256, q256;
            pq256.GetPrimeMultipliers(out p256, out q256);
            p = (Int128)p256;
            q = (Int128)q256;
        }


        public static void GetPrimeMultipliers(this Int256 pq, out Int256 p, out Int256 q)
        {
            p = PollardRho(pq);
            q = pq / p;
            if (p > q)
            {
                Int256 t = p;
                p = q;
                q = t;
            }
        }


        public static Int256 PollardRho(Int256 number)
        {
            var func = new Func<Int256, Int256, Int256>((param, mod) => ((param * param + 1) % mod));


            Int256 x = 2, y = 2, z;
            do
            {
                x = func(x, number);
                y = func(func(y, number), number);
                z = GCD(x > y ? x - y : y - x, number);
            } while (z <= 1);
            return z;
        }


        public static Int128 GCD(this Int128 a, Int128 b)
        {
            while (true)
            {
                if (b == 0)
                {
                    return a;
                }
                Int128 a1 = a;
                a = b;
                b = a1 % b;
            }
        }


        public static Int256 GCD(this Int256 a, Int256 b)
        {
            while (true)
            {
                if (b == 0)
                {
                    return a;
                }
                Int256 a1 = a;
                a = b;
                b = a1 % b;
            }
        }


        private static int GetNormalizeShift(uint value)
        {
            int shift = 0;


            if ((value & 0xFFFF0000) == 0)
            {
                value <<= 16;
                shift += 16;
            }
            if ((value & 0xFF000000) == 0)
            {
                value <<= 8;
                shift += 8;
            }
            if ((value & 0xF0000000) == 0)
            {
                value <<= 4;
                shift += 4;
            }
            if ((value & 0xC0000000) == 0)
            {
                value <<= 2;
                shift += 2;
            }
            if ((value & 0x80000000) == 0)
            {
                value <<= 1;
                shift += 1;
            }


            return shift;
        }


        private static void Normalize(uint[] u, int l, uint[] un, int shift)
        {
            uint carry = 0;
            int i;
            if (shift > 0)
            {
                int rshift = 32 - shift;
                for (i = 0; i < l; i++)
                {
                    uint ui = u[i];
                    un[i] = (ui << shift) | carry;
                    carry = ui >> rshift;
                }
            }
            else
            {
                for (i = 0; i < l; i++)
                {
                    un[i] = u[i];
                }
            }


            while (i < un.Length)
            {
                un[i++] = 0;
            }


            if (carry != 0)
            {
                un[l] = carry;
            }
        }


        private static void Unnormalize(uint[] un, out uint[] r, int shift)
        {
            int length = un.Length;
            r = new uint[length];


            if (shift > 0)
            {
                int lshift = 32 - shift;
                uint carry = 0;
                for (int i = length - 1; i >= 0; i--)
                {
                    uint uni = un[i];
                    r[i] = (uni >> shift) | carry;
                    carry = (uni << lshift);
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    r[i] = un[i];
                }
            }
        }


        private static int GetLength(uint[] uints)
        {
            int index = uints.Length - 1;
            while ((index >= 0) && (uints[index] == 0))
            {
                index--;
            }
            index = index < 0 ? 0 : index;
            return index + 1;
        }


        private static uint[] TrimZeros(uint[] uints)
        {
            var trimmed = new uint[GetLength(uints)];
            Buffer.BlockCopy(uints, 0, trimmed, 0, trimmed.Length * 4);
            return trimmed;
        }


        public static void DivModUnsigned(uint[] u, uint[] v, out uint[] q, out uint[] r)
        {
            int m = GetLength(u);
            int n = GetLength(v);


            if (n <= 1)
            {
                //  Divide by single digit
                //
                ulong rem = 0;
                uint v0 = v[0];
                q = new uint[m];
                r = new uint[1];


                for (int j = m - 1; j >= 0; j--)
                {
                    rem *= Base;
                    rem += u[j];


                    ulong div = rem / v0;
                    rem -= div * v0;
                    q[j] = (uint)div;
                }
                r[0] = (uint)rem;
            }
            else if (m >= n)
            {
                int shift = GetNormalizeShift(v[n - 1]);


                var un = new uint[m + 1];
                var vn = new uint[n];


                Normalize(u, m, un, shift);
                Normalize(v, n, vn, shift);


                q = new uint[m - n + 1];
                r = null;


                //  Main division loop
                //
                for (int j = m - n; j >= 0; j--)
                {
                    ulong rr, qq;
                    int i;


                    rr = Base * un[j + n] + un[j + n - 1];
                    qq = rr / vn[n - 1];
                    rr -= qq * vn[n - 1];


                    for (; ; )
                    {
                        // Estimate too big ?
                        //
                        if ((qq >= Base) || (qq * vn[n - 2] > (rr * Base + un[j + n - 2])))
                        {
                            qq--;
                            rr += vn[n - 1];
                            if (rr < Base)
                            {
                                continue;
                            }
                        }
                        break;
                    }




                    //  Multiply and subtract
                    //
                    long b = 0;
                    long t = 0;
                    for (i = 0; i < n; i++)
                    {
                        ulong p = vn[i] * qq;
                        t = un[i + j] - (long)(uint)p - b;
                        un[i + j] = (uint)t;
                        p >>= 32;
                        t >>= 32;
                        b = (long)p - t;
                    }
                    t = un[j + n] - b;
                    un[j + n] = (uint)t;


                    //  Store the calculated value
                    //
                    q[j] = (uint)qq;


                    //  Add back vn[0..n] to un[j..j+n]
                    //
                    if (t < 0)
                    {
                        q[j]--;
                        ulong c = 0;
                        for (i = 0; i < n; i++)
                        {
                            c = (ulong)vn[i] + un[j + i] + c;
                            un[j + i] = (uint)c;
                            c >>= 32;
                        }
                        c += un[j + n];
                        un[j + n] = (uint)c;
                    }
                }


                Unnormalize(un, out r, shift);
            }
            else
            {
                q = new uint[] { 0 };
                r = u;
            }


            q = TrimZeros(q);
            r = TrimZeros(r);
        }
    }

    /// <summary>
    ///     Utils for the <see cref="Array" /> class.
    /// </summary>
    public static class ArrayUtils
    {
        private static readonly byte[] CharToByteLookupTable =
        {
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e,
            0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            0xff, 0xff, 0xff, 0xff
        };


        private static readonly char[][] LookupTableUpper;
        private static readonly char[][] LookupTableLower;


        static ArrayUtils()
        {
            LookupTableLower = new char[256][];
            LookupTableUpper = new char[256][];
            for (int i = 0; i < 256; i++)
            {
                LookupTableLower[i] = i.ToString("x2").ToCharArray();
                LookupTableUpper[i] = i.ToString("X2").ToCharArray();
            }
        }


        /// <summary>
        ///     Converts an array of one type to an array of another type.
        /// </summary>
        /// <returns>
        ///     An array of the target type containing the converted elements from the source array.
        /// </returns>
        /// <param name="array">The one-dimensional, zero-based <see cref="T:System.Array" /> to convert to a target type.</param>
        /// <param name="convert">A <see cref="Func{TInput, TOutput}" /> that converts each element from one type to another type.</param>
        /// <typeparam name="TInput">The type of the elements of the source array.</typeparam>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="array" /> is null.-or-<paramref name="convert" /> is
        ///     null.
        /// </exception>
        public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Func<TInput, TOutput> convert)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (convert == null)
            {
                throw new ArgumentNullException("convert");
            }
            var outputArray = new TOutput[array.Length];
            for (int index = 0; index < array.Length; ++index)
            {
                outputArray[index] = convert(array[index]);
            }
            return outputArray;
        }


        /// <summary>
        ///     Get length of serial non zero items.
        /// </summary>
        /// <param name="bytes">Array of bytes.</param>
        /// <param name="asLittleEndian">True - skip all zero items from high. False - skip all zero items from low.</param>
        /// <returns>Length of serial non zero items.</returns>
        public static int GetNonZeroLength(this byte[] bytes, bool? asLittleEndian = null)
        {
            bool ale = GetIsLittleEndian(asLittleEndian);


            if (ale)
            {
                int index = bytes.Length - 1;
                while ((index >= 0) && (bytes[index] == 0))
                {
                    index--;
                }
                index = index < 0 ? 0 : index;
                return index + 1;
            }
            else
            {
                int index = 0;
                while ((index < bytes.Length) && (bytes[index] == 0))
                {
                    index++;
                }
                index = index >= bytes.Length ? bytes.Length - 1 : index;
                return bytes.Length - index;
            }
        }


        /// <summary>
        ///     Trim zero items.
        /// </summary>
        /// <param name="bytes">Array of bytes.</param>
        /// <param name="asLittleEndian">True - trim from high, False - trim from low.</param>
        /// <returns>Trimmed array of bytes.</returns>
        public static byte[] TrimZeros(this byte[] bytes, bool? asLittleEndian = null)
        {
            bool ale = GetIsLittleEndian(asLittleEndian);


            int length = GetNonZeroLength(bytes, ale);


            var trimmed = new byte[length];
            Buffer.BlockCopy(bytes, ale ? 0 : bytes.Length - length, trimmed, 0, length);
            return trimmed;
        }


        public static byte[] Combine(byte[] first, byte[] second)
        {
            var ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }


        public static byte[] Combine(byte[] first, byte[] second, byte[] third)
        {
            var ret = new byte[first.Length + second.Length + third.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length, third.Length);
            return ret;
        }


        public static byte[] Combine(params byte[][] arrays)
        {
            var ret = new byte[arrays.Sum(x => x.Length)];
            int offset = 0;
            foreach (byte[] data in arrays)
            {
                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }
            return ret;
        }


        public static void Split<T>(this T[] array, int index, out T[] first, out T[] second)
        {
            first = array.Take(index).ToArray();
            second = array.Skip(index).ToArray();
        }


        public static void SplitMidPoint<T>(this T[] array, out T[] first, out T[] second)
        {
            Split(array, array.Length / 2, out first, out second);
        }


        public static byte[] RewriteWithValue(this byte[] bytes, byte value, int offset, int length)
        {
            if (offset + length > bytes.Length)
            {
                throw new InvalidOperationException("Offset + length must be less of equal of the bytes length.");
            }
            var tbytes = (byte[])bytes.Clone();
            for (int i = offset; i < offset + length; i++)
            {
                tbytes[i] = value;
            }
            return tbytes;
        }


        /// <summary>
        ///     Converts array of bytes to hexadecimal string.
        /// </summary>
        /// <param name="bytes">Bytes.</param>
        /// <param name="caps">Capitalize chars.</param>
        /// <param name="min">Minimum string length. 0 if there is no minimum length.</param>
        /// <param name="spaceEveryByte">Space every byte.</param>
        /// <param name="trimZeros">Trim zeros in the result string.</param>
        /// <returns>Hexadecimal string representation of the bytes array.</returns>
        public static unsafe string ToHexString(this byte[] bytes, bool caps = true, int min = 0, bool spaceEveryByte = false, bool trimZeros = false)
        {
            if (bytes.Length == 0)
            {
                return string.Empty;
            }


            int strLength = min;

            int bim = 0;
            if (trimZeros)
            {
                bim = bytes.Length - 1;
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] > 0)
                    {
                        bim = i;
                        int l = (bytes.Length - i) * 2;
                        strLength = (l <= min) ? min : l;
                        break;
                    }
                }
            }
            else
            {
                strLength = bytes.Length * 2;
                strLength = strLength < min ? min : strLength;
            }


            if (strLength == 0)
            {
                return "0";
            }


            int step = 0;
            if (spaceEveryByte)
            {
                strLength += (strLength / 2 - 1);
                step = 1;
            }


            var chars = new char[strLength];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = '0';
            }


            if (spaceEveryByte)
            {
                for (int i = 2; i < chars.Length; i += 3)
                {
                    chars[i] = ' ';
                }
            }


            char[][] lookupTable = caps ? LookupTableUpper : LookupTableLower;
            int bi = bytes.Length - 1;
            int ci = strLength - 1;
            while (bi >= bim)
            {
                char[] chb = lookupTable[bytes[bi--]];
                chars[ci--] = chb[1];
                chars[ci--] = chb[0];
                ci -= step;
            }


            int offset = 0;
            if (trimZeros && strLength > min)
            {
                for (int i = 0; i < chars.Length; i++)
                {
                    char c = chars[i];
                    if (c != '0' && c != ' ')
                    {
                        offset = i;
                        break;
                    }
                }
            }


            string str;
            fixed (char* charPtr = chars)
            {
                str = new string(charPtr + offset);
            }
            return str;
        }


        /// <summary>
        ///     Converts string of hex numbers to array of bytes.
        /// </summary>
        /// <param name="hexString">String value.</param>
        /// <returns>Array of bytes.</returns>
        public static byte[] HexToBytes(this string hexString)
        {
            byte[] bytes;
            if (String.IsNullOrWhiteSpace(hexString))
            {
                bytes = new byte[0];
            }
            else
            {
                int stringLength = hexString.Length;
                int characterIndex = (hexString.StartsWith("0x", StringComparison.Ordinal)) ? 2 : 0;
                // Does the string define leading HEX indicator '0x'. Adjust starting index accordingly.               
                int numberOfCharacters = stringLength - characterIndex;


                bool addLeadingZero = false;
                if (0 != (numberOfCharacters % 2))
                {
                    addLeadingZero = true;


                    numberOfCharacters += 1; // Leading '0' has been striped from the string presentation.
                }


                bytes = new byte[numberOfCharacters / 2]; // Initialize our byte array to hold the converted string.


                int writeIndex = 0;
                if (addLeadingZero)
                {
                    bytes[writeIndex++] = CharToByteLookupTable[hexString[characterIndex]];
                    characterIndex += 1;
                }


                while (characterIndex < hexString.Length)
                {
                    int hi = CharToByteLookupTable[hexString[characterIndex++]];
                    int lo = CharToByteLookupTable[hexString[characterIndex++]];


                    bytes[writeIndex++] = (byte)(hi << 4 | lo);
                }
            }


            return bytes;
        }


        private static bool GetIsLittleEndian(bool? asLittleEndian)
        {
            return asLittleEndian.HasValue ? asLittleEndian.Value : BitConverter.IsLittleEndian;
        }
    }
}
