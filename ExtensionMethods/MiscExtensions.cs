using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ZetaColorEditor.Runtime.Colors;

namespace ExtensionMethods
{
    public static class MiscExtensions
    {
        public static SqlParameter ConfigureAsDecimal(this SqlParameter p, string name, byte precision, byte scale)
        {
            p.ParameterName = name;
            p.DbType = System.Data.DbType.Decimal;
            p.Precision = precision;
            p.Scale = scale;

            return p;
        }

        public static SqlParameter ConfigureAs(this SqlParameter p, string name, SqlDbType dbType)
        {
            p.ParameterName = name;
            p.SqlDbType = dbType;

            return p;
        }

        public static SqlParameter SetValue<T>(this SqlParameter p, T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                p.Value = DBNull.Value;
            }
            else if (value.Equals(value.GetTypeDefaultValue()))
            {
                p.Value = DBNull.Value;
            }
            else
            {
                p.Value = value;
            }

            return p;
        }
    }

    public static class ExtendArray
    {
        // Array array = Array.CreateInstance(typeof(string), 2);
        // array.SetValue("One", 0); array.SetValue("Two", 1);
        // Array arrayToClear = array.ClearAll();

        public static Array ClearAll(this Array clear)
        {
            if (clear != null) Array.Clear(clear, 0, clear.Length);
            return clear;
        }

        // int[] result = new[] { 1, 2, 3, 4 }.ClearAll<int>();

        public static T[] ClearAll<T>(this T[] arrayToClear)
        {
            if (arrayToClear != null)
                for (int i = arrayToClear.GetLowerBound(0); i <= arrayToClear.GetUpperBound(0); ++i)
                    arrayToClear[i] = default(T);
            return arrayToClear;
        }
    }

    public static class DateTimeExtensions
    {

        public static string DaySuffix(this DateTime source)
        {
            return ((source.Day % 10 == 1 && source.Day != 11) ? "st" : (source.Day % 10 == 2 && source.Day != 12) ? "nd" : (source.Day % 10 == 3 && source.Day != 13) ? "rd" : "th");
        }

    }

    public static class ExtendColor
    {
        public static Color WithAlpha(this Color color, double opacity)
        {
            byte op = (byte)(opacity*255);
            return Color.FromArgb(op, color.R, color.G, color.B);
        }

        public static Color LightenDramatically(this Color color)
        {
            return ControlPaint.Light(color);
        }

        public static Color DarkenDramatically(this Color color)
        {
            return ControlPaint.Dark(color);
        }

        public static Color ChangeColorBrightness(this Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public static HslColor DarkenUsingHue(this Color color, double darkenAmount)
        {
            HslColor hslColor = new HslColor((int)color.GetHue(), (int)color.GetSaturation(), (int)color.GetBrightness());
            hslColor.Light *= (int)darkenAmount; // 0 to 1
            return hslColor;
        }
    }

    public static class ExtendChar
    {
        /// <summary>
        /// The is digit.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsDigit(this char c)
        {
            return Char.IsDigit(c);
        }

        /// <summary>
        /// The is letter.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsLetter(this char c)
        {
            return Char.IsLetter(c);
        }

        /// <summary>
        /// The in.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="chars">The chars.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool In(this char c, params char[] chars)
        {
            foreach (var oc in chars) { if (c == oc) return true; }
            return false;
        }
        //Char.GetUnicodeCategory  Idea: Use this to determine implied type of text: Math symbols, language, arrows, dots, etc.

        public static string ToString(this char c)
        {
            return Char.ToString(c);
        }

        public static bool In(this char? c, params char[] chars)
        {
            foreach (var oc in chars) { if (c == oc) return true; }
            return false;
        }

    }

    public static class ExtendIDictionary
    {
        public static TValue GetValueOrDefault<TKey, TValue>
    (this IDictionary<TKey, TValue> dictionary,
     TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }
        public static TValue GetValueOrDefault<TKey, TValue>
    (this IDictionary<TKey, TValue> dictionary,
     TKey key,
     TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             TKey key,
             Func<TValue> defaultValueProvider)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value
                 : defaultValueProvider();
        }
    }
    public static class ExtendIEnumerable
    {
        /// <summary>
            /// Continues processing items in a collection until the end condition is true.
            /// </summary>
            /// <typeparam name="T">The type of the collection.</typeparam>
            /// <param name="collection">The collection to iterate.</param>
            /// <param name="endCondition">The condition that returns true if iteration should stop.</param>
            /// <returns>Iterator of sub-list.</returns>
        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> collection, Predicate<T> endCondition)
        {
            return collection.TakeWhile(item => !endCondition(item));
        }

        public static T Last<T>(this List<T> collection)
        {
            if (collection.Count == 0) return default(T);
            return collection[collection.Count - 1];
        }

        //foreach(var item in list.TakeUntil(i => i.Name == null);
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var value in values)
                action(value);
        }

        [Obsolete("Use RemoveWhere instead..")]
        public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                return Enumerable.Empty<T>();

            var list = source.ToList();
            list.RemoveAll(predicate);
            return list;
        }

        public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
                yield break;

            foreach (T t in source)
                if (!predicate(t))
                    yield return t;
        }

 
    }
    public static class ExtendInt
    {
        /// <summary>
        /// Convenient see if in list.  Good for enums.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="i_n"></param>
        /// <returns></returns>
        public static bool In(this int i, params int[] i_n)
        {
            foreach (int v in i_n) if (v == i) return true;
            return false;
        }

        public static bool In(this int? i, params int?[] i_n)
        {
            if (i == null) return false;
            foreach (int v in i_n) if (v == i) return true;
            return false;
        }

        /// <summary>
        /// Get tired of printing "1 sets" or "1 set(s)".  Looks sad.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="term"></param>
        /// <returns></returns>

        public static string Pluralize(this int i, string singularterm, string pluralterm = null, string singularprefix = null, string pluralprefix = null)
        {
            string phrase = i.ToString() + " ";
            if (singularterm != null && pluralterm == null) pluralterm = singularterm + "s";
            if (singularprefix == null) singularprefix = "";
            if (pluralprefix == null) pluralprefix = singularprefix;

            phrase = i != 1 ? $"{pluralprefix} {phrase} {pluralterm}" : $"{singularprefix} {phrase} {singularterm}";
            return phrase;
        }

        ///// <summary>
        ///// The between.
        ///// </summary>
        ///// <param name="i">The i.</param>
        ///// <param name="lo">The lo.</param>
        ///// <param name="hi">The hi.</param>
        ///// <returns>The <see cref="bool"/>.</returns>
        //public static bool Between(this int i, int lo, int hi, bool testforflippedlimits=true)
        //{
        //    if (testforflippedlimits && lo > hi)
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(lo), lo, $"ExtendInt:Between:lo {lo} cannot be greater than hi {hi}.");
        //    }
        //
        //    return i >= lo && i <= hi;
        //}
        //
        ///// <summary>
        ///// The between for pairs of ranges. assumes you meant OR between the tuples.  If any match, we return true.
        ///// </summary>
        ///// <param name="i">The i.</param>
        ///// <param name="lo">The lo.</param>
        ///// <param name="hi">The hi.</param>
        ///// <returns>The <see cref="bool"/>.</returns>
        //public static bool Between(this int i, params ValueTuple<int, int>[] range)
        //{
        //    foreach (var tuple in range)
        //    {
        //        if (i.Between(tuple.Item1, tuple.Item2)) return true;
        //    }
        //    return false;
        //}

        public static IWin32Window ToIWin32Window(this System.IntPtr handle)
        {
            return NativeWindow.FromHandle(handle);
        }
    }
    public static class ExtendKeys
    {
        /// <summary>
        /// The in clause for keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="keystomatch">The keystomatch.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool In(this Keys keys, params Keys[] keystomatch)
        {
            foreach (Keys key in keystomatch)
            {
                if (keys == key) return true;
            }

            return false;
        }

        public static bool In(this Keys? keys, params Keys[] keystomatch)
        {
            if (keys == null) return false; // Safety feature

            foreach (Keys key in keystomatch)
            {
                if (keys == key) return true;
            }

            return false;
        }

        public static bool IsLetter(this Keys? keys)
        {
            if (keys == null) return false; // Safety feature
                                            // A-Z        a-z
            return ((int)(((Keys)keys).ToChar())).Between(new[] { (65, 90), (97, 122) });
        }

        public static bool IsLetter(this Keys keys)
        {
            return IsLetter((Keys?)keys);
        }

        public static bool IsDateChar(this Keys? keys)
        {
            if (keys == null) return false; // Safety feature

            char c = ((Keys)keys).ToChar();
            int i = (int)c;
            //  0-9       -./
            return ((int)(((Keys)keys).ToChar())).Between(new[] { (48, 57), (45, 47) });
        }

        public static bool IsDateChar(this Keys keys)
        {
            return IsDateChar((Keys?)keys);
        }

        public static bool IsControlChar(this Keys keys)
        {
            char c = ((Keys)keys).ToChar();
            int i = (int)c;

            return (Char.IsControl(c));
        }
        /// <summary>
        /// Get the async key state.  Since KeyEventArgs.Control doesn't work at all.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="short"/>.</returns>
        /// <see cref="https://stackoverflow.com/questions/9595825/capture-specific-modifier-key"/>
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys key);

        /// <summary>
        /// The is being pressed.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsKeyBeingPressed(this Keys key)
        {
            // Keys.ControlKey, Keys.LControlKey, Keys.RControlKey, etyc.
            return GetAsyncKeyState(key) < 0;
        }

        /// <summary>
        /// The enum class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static class Enum<T> where T : struct, IConvertible
        {
            public static int Count
            {
                get
                {
                    if (!typeof(T).IsEnum)
                        throw new ArgumentException("T must be an enumerated type");

                    return Enum.GetNames(typeof(T)).Length;
                }
            }
        }

        // https://stackoverflow.com/questions/318777/c-sharp-how-to-translate-virtual-keycode-to-char
        [DllImport("user32.dll")]
        static extern int MapVirtualKey(uint uCode, uint uMapType);

        public static char ToChar(this Keys key)
        {
            int nonVirtualKey = MapVirtualKey((uint)key, 2);
            char mappedChar = Convert.ToChar(nonVirtualKey);

            return mappedChar;
        }

        public static Keys ToKeys(this char ch)
        {
            short vkey = VkKeyScan(ch);
            Keys retval = (Keys)(vkey & 0xff);
            int modifiers = vkey >> 8;
            if ((modifiers & 1) != 0) retval |= Keys.Shift;
            if ((modifiers & 2) != 0) retval |= Keys.Control;
            if ((modifiers & 4) != 0) retval |= Keys.Alt;
            return retval;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);
    }
    public static class ExtendObject
    {
        public static bool In(this object o, params object[] these)
        {
            if (o == null) return false;

            foreach (var that in these)
            {
                if (that is Array)
                {
                    foreach (var that2 in that as Array)
                    {
                        if (o.GetType() == that2.GetType() && o.ToString() == that2.ToString())
                        {
                            if (o is Int32 && that2 is Int32 && (int)o == (int)that2) return true;
                            {
                                return true;
                            }
                        }
                    }
                }

                if (o.Equals(that)) return true;
            }

            return false;
        }
        public static bool In(this object o, IEnumerable<object> these)
        {
            foreach (var that in these)
            {
                if (o == that) return true;
            }

            return false;
        }
    }
    public static class ExtendDataRow
    {
        public static T Get<T>(this DataRow row, string field)
        {
            return row.Get(field, default(T));
        }

        public static T Get<T>(this DataRow row, string field, T defaultValue)
        {
            var value = row[field];
            if (value == DBNull.Value)
                return defaultValue;
            return value.ConvertTo(defaultValue);
        }

        public static byte[] GetBytes(this DataRow row, string field)
        {
            return (row[field] as byte[]);
        }

        public static string GetString(this DataRow row, string field)
        {
            return row.GetString(field, null);
        }

        public static string GetString(this DataRow row, string field, string defaultValue)
        {
            var value = row[field];
            return (value is string ? (string)value : defaultValue);
        }

        public static Guid GetGuid(this DataRow row, string field)
        {

            var value = row[field];
            return (value is Guid ? (Guid)value : Guid.Empty);
        }

        public static DateTime GetDateTime(this DataRow row, string field, DateTime defaultValue)
        {
            var value = row[field];
            return (value is DateTime ? (DateTime)value : defaultValue);
        }

        public static Type GetType(this DataRow row, string field)
        {
            return row.GetType(field, null);
        }

        public static Type GetType(this DataRow row, string field, Type defaultValue)
        {
            var classType = row.GetString(field);
            if (classType.IsNotEmpty())
            {
                var type = Type.GetType(classType);
                if (type != null)
                    return type;
            }
            return defaultValue;
        }

        public static object GetTypeInstance(this DataRow row, string field)
        {
            return row.GetTypeInstance(field, null);
        }

        public static object GetTypeInstance(this DataRow row, string field, Type defaultValue)
        {
            var type = row.GetType(field, defaultValue);
            return (type != null ? Activator.CreateInstance(type) : null);
        }

        public static T GetTypeInstance<T>(this DataRow row, string field) where T : class
        {
            return (row.GetTypeInstance(field, null) as T);
        }

        public static T GetTypeInstanceSafe<T>(this DataRow row, string field, Type type) where T : class
        {
            var instance = (row.GetTypeInstance(field, null) as T);
            return (instance ?? Activator.CreateInstance(type) as T);
        }

        public static bool IsDBNull(this DataRow row, string field)
        {
            var value = row[field];
            return (value == DBNull.Value);
        }
    }
    public static class ExtendDateTIme
    {
        /// <summary>
        /// The day suffix text based on the day #.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string DaySuffix(this DateTime source)
        {
            return (source.Day % 10 == 1 && source.Day != 11) ? "st" : (source.Day % 10 == 2 && source.Day != 12) ? "nd" : (source.Day % 10 == 3 && source.Day != 13) ? "rd" : "th";
        }

        public static DateTime? ToDateTime(this string source)
        {
            DateTime gotDateTime;
            bool succeed = DateTime.TryParse(source, out gotDateTime);
            if (!succeed) return null;
            return gotDateTime;



        }

        public static bool IsVoid(this DateTime source)
        {
            if (source == DateTime.MaxValue || source == DateTime.MinValue) return true;
            if (source == new DateTime(1753, 1, 1)) return true; // SQL Server DATETIME Minimum
            if (source == new DateTime(9999, 12, 31)) return true; // SQL Server DATETIME Maximum
            if (source == new DateTime(9999, 12, 31, 23, 58, 59)) return true;
            if (source == new DateTime(9999, 12, 31, 23, 58, 59, 997)) return true;
            if (source == new DateTime(0001, 01, 01)) return true; // Null in DevExpress
            if (source == new DateTime(1582, 10, 15)) return true; // min in Informatica
            //if (source == new DateTime(-4712, 1, 1)) return true; // Min in Oracle
            if (source == new DateTime(9999, 01, 01)) return true; // Max in Oracle

            return false;
        }

        public static bool IsNotVoid(this DateTime source)
        {
            return !IsVoid(source);
        }

        public static int Age(this DateTime @this)
        {
            if (DateTime.Today.Month < @this.Month ||
                DateTime.Today.Month == @this.Month &&
                DateTime.Today.Day < @this.Day)
            {
                return DateTime.Today.Year - @this.Year - 1;
            }
            return DateTime.Today.Year - @this.Year;
        }
    }
    public static class ExtendDecimal
    {
        public static bool IsVoid(this Decimal d)
        {
            if (d == null) return true;
            if (d == Decimal.MaxValue || d == Decimal.MinValue) return true;
            return false;
        }

        public static bool IsNotVoid(this Decimal d)
        {
            return !IsVoid(d);
        }

        public static Decimal RoundOff(this Decimal d, int places)
        {
            return Decimal.Round(d, places, MidpointRounding.AwayFromZero);
        }
    }
    public static class ExtendException
    {
        public static IEnumerable<string> Messages(this Exception exception)
        {
            return exception != null ?
                    new List<string>(exception.InnerException.Messages()) { exception.Message } : Enumerable.Empty<string>();
        }
    }
    public static class ExtendFloat
    {
        /// <summary>
        /// The if int function.  It means it will give you the integer (32) value of a floating point number, as long as there is no decimal portion and it is in range.
        /// This is convenient for have a mix of functions, some that take floating point values (but always have no decimal) for points and others that require integer. 
        /// You can't have a "0.1" point position!  Points are by definition integers, unfortunately, DevExpress takes floating point.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>The <see cref="int"/>.</returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        //public static int IfInt(this float f)
        //{
        //    if (f.Precision() == 0)
        //    {
        //        var i = (Int128)f;
        //        if (i > Int32.MaxValue || i < Int32.MinValue)
        //        {
        //            throw new InvalidCastException($"IfInt() cannot convert any numbers outside the range of int32. Your number is {i.ToString()}.");
        //        }
        //        return (int)i;
        //    }
        //    else
        //    {
        //        throw new InvalidCastException($"IfInt() does not allow conversion of floats with actual fractional values to an int. Your input value was {f.ToString()}. ");
        //    }
        //}

        /// <summary>
        /// The precision (number of non-trailing-zeroes).  Rather unintuitive.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int Precision(this float f)
        {
            // https://stackoverflow.com/questions/9386672/finding-the-number-of-places-after-the-decimal-point-of-a-double
            var precision = 0;
            while (f * (float)Math.Pow(10, precision) != Math.Round(f * (float)Math.Pow(10, precision))) precision++;
            return precision;
        }
    }
    public static class ExtendGenerics
    {

        public static T Least<T>(this T n, T n_n) where T : IComparable
        {
            if (n.CompareTo(n_n) <= 0) return n;
            return n_n;
        }

        /// <summary>
        /// The least between all numbers. Note that T[] won't accept a comparison between only two objects.
        /// </summary>
        /// <param name="n">The i.</param>
        /// <param name="n_n">The i_n.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static T Least<T>(this T n, params T[] n_n) where T : IComparable
        {
            T running_min = n;
            foreach (T v in n_n) if (v.CompareTo(running_min) < 0) running_min = v;
            return running_min;
        }

        /// <summary>
        /// The greatest.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="n_n">The n_n.</param>
        /// <returns>The <see cref="T"/>.</returns>
        /// <typeparam name="T"></typeparam>
        public static T Greatest<T>(this T n, params T[] n_n) where T : IComparable
        {
            T running_max = n;
            foreach (T v in n_n) if (v.CompareTo(running_max) > 0) running_max = v;
            return running_max;

        }

        /// <summary>
        /// The between for pairs of ranges. assumes you meant OR between the tuples.  If any match, we return true.
        /// </summary>
        /// <param name="n">The i.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Between<T>(this T n, params ValueTuple<T, T>[] range) where T : IComparable
        {
            foreach (var tuple in range)
            {
                if (n.Between(tuple.Item1, tuple.Item2)) return true;
            }
            return false;
        }

        /// <summary>
        /// The between.
        /// </summary>
        /// <param name="n">The i.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Between<T>(this T n, T lo, T hi, bool testforflippedlimits = true) where T : IComparable
        {
            if (testforflippedlimits && lo.CompareTo(hi) > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lo), lo, $"ExtendInt:Between:lo {lo} cannot be greater than hi {hi}.");
            }

            return n.CompareTo(lo) >= 0 && n.CompareTo(hi) <= 0;
        }

        public static bool IsNotBetween<T>(this T n, T lo, T hi, bool testforflippedlimits = true) where T : IComparable
        {
            return !n.Between(lo, hi, testforflippedlimits);
        }

        public static T GetTypeDefaultValue<T>(this T value)
        {
            return default(T);
        }

    }
    public static class ExtendSqlDataReader
    {
        public static bool IsDBNull(this SqlDataReader reader, string colName)
        {
            int colPos = reader.GetOrdinal(colName);
            return reader.IsDBNull(colPos);
        }

        public static T Get<T>(this SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index)) return default;
            Type type = reader.GetFieldType(index);
            if (type != typeof(T))
            {
                bool b = false;

                switch (typeof(T).FullName)
                {
                    case "System.Boolean":
                        switch (type.Name)
                        {
                            case "Int32":
                                int i = (int)reader[index];
                                if (i == 1) b = true;
                                else if (i == 0) b = false;
                                else
                                {
                                    throw new InvalidCastException($"Cannot cast {i} to boolean");
                                }

                                return b.ConvertTo<T>();

                            case "Char":
                                char c = (char)reader[index];
                                if (c.In('Y', 'y', '1', 'T')) b = true;
                                else if (c.In('N', 'n', '0', 'F')) b = false;
                                else
                                {
                                    throw new InvalidCastException($"Cannot cast <{c}> to boolean");
                                }

                                return b.ConvertTo<T>();
                            default:
                                throw new InvalidCastException($"Cannot cast {type.Name} to boolean");

                        }
                }
            }
            return ((T)reader[index]);
        }
    }

    //public static class ExtendInt
    //{
    //    /// <summary>
    //    /// Convenient see if in list.  Good for enums.
    //    /// </summary>
    //    /// <param name="i"></param>
    //    /// <param name="i_n"></param>
    //    /// <returns></returns>
    //    public static bool In(this int i, params int[] i_n)
    //    {
    //        foreach (int v in i_n) if (v == i) return true;
    //        return false;
    //    }

    //    public static bool In(this int? i, params int?[] i_n)
    //    {
    //        if (i == null) return false;
    //        foreach (int v in i_n) if (v == i) return true;
    //        return false;
    //    }

    //    ///// <summary>
    //    ///// The between.
    //    ///// </summary>
    //    ///// <param name="i">The i.</param>
    //    ///// <param name="lo">The lo.</param>
    //    ///// <param name="hi">The hi.</param>
    //    ///// <returns>The <see cref="bool"/>.</returns>
    //    //public static bool Between(this int i, int lo, int hi, bool testforflippedlimits=true)
    //    //{
    //    //    if (testforflippedlimits && lo > hi)
    //    //    {
    //    //        throw new ArgumentOutOfRangeException(nameof(lo), lo, $"ExtendInt:Between:lo {lo} cannot be greater than hi {hi}.");
    //    //    }
    //    //
    //    //    return i >= lo && i <= hi;
    //    //}
    //    //
    //    ///// <summary>
    //    ///// The between for pairs of ranges. assumes you meant OR between the tuples.  If any match, we return true.
    //    ///// </summary>
    //    ///// <param name="i">The i.</param>
    //    ///// <param name="lo">The lo.</param>
    //    ///// <param name="hi">The hi.</param>
    //    ///// <returns>The <see cref="bool"/>.</returns>
    //    //public static bool Between(this int i, params ValueTuple<int, int>[] range)
    //    //{
    //    //    foreach (var tuple in range)
    //    //    {
    //    //        if (i.Between(tuple.Item1, tuple.Item2)) return true;
    //    //    }
    //    //    return false;
    //    //}

    //    public static IWin32Window ToIWin32Window(this System.IntPtr handle)
    //    {
    //        return NativeWindow.FromHandle(handle);
    //    }
    //}
    public static class CollectionExtensions
    {
        //http://www.codeproject.com/Articles/495453/LINQ-and-dictionaries
        public static Dictionary<TKey, TValue> Where<TKey, TValue>(
            this IDictionary<TKey, TValue> source, Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            return Enumerable.Where(source, predicate).ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
    public static class SqlParExtension
    {
        public static void ParseParameters(this SqlCommand cmd)
        {
            var rxPattern = @"(?<=\= |\=)@\w*";
            foreach (System.Text.RegularExpressions.Match item in System.Text.RegularExpressions.Regex.Matches(cmd.CommandText, rxPattern))
            {
                var sqlp = new SqlParameter(item.Value, null);
                cmd.Parameters.Add(sqlp);
            }
        }
    }

    /*
     *     public class ColorManagement
        {
            [DllImport ("shlwapi.dll")]
            public static extern int ColorRGBToHLS (int RGB, ref int H, ref int L, ref int S);

            /* "Portions of this code provided by Bob Powell. http://www.bobpowell.net" 
             *              https://web.archive.org/web/20110425154034/http://bobpowell.net/RGBHSB.htm
             *

    public static Color ColorHSLToRGB(double H, double S, double L)
    {
        int r = 0, g = 0, b = 0;
        double temp1, temp2;

        if (L == 0)
        {
            r = g = b = 0;
        }
        else
        {
            if (S == 0)
            {
                //r=g=b=(int)L; white turned to black. no good.
                r = g = b = 255;
            }
            else
            {
                temp2 = ((L <= 0.5) ? L * (1.0 + S) : L + S - (L * S));
                temp1 = 2.0 * L - temp2;
                double[] t3 = new double [] { H + 1.0 / 3.0, H, H - 1.0 / 3.0 };
                double[] clr = new double [] { 0, 0, 0 };

                for (int i = 0; i < 3; i++)
                {
                    if (t3[i] < 0)
                        t3[i] += 1.0;
                    if (t3[i] > 1)
                        t3[i] -= 1.0;
                    if (6.0 * t3[i] < 1.0)
                        clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                    else if (2.0 * t3[i] < 1.0)
                        clr[i] = temp2;
                    else if (3.0 * t3[i] < 2.0)
                        clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
                    else
                        clr[i] = temp1;
                }
                r = (int)(clr[0] * 255);
                g = (int)(clr[1] * 255);
                b = (int)(clr[2] * 255);


            }
        }
        if (r > 255) r = 255;
        if (g > 255) g = 255;
        if (b > 255) b = 255;
        return Color.FromArgb(r, g, b);
    }

    //̶—————————————————————————————————————————————————————————————
    public static Color GetGrayedOutColor(Color Color)
    {
        int r, g, b;

        r = Color.R / 2 + 105;
        g = Color.G / 2 + 113;
        b = Color.B / 2 + 107;

        if (r > 255) r = 255;
        if (g > 255) g = 255;
        if (b > 255) b = 255;

        return Color.FromArgb(r, g, b);
    }

    //̶—————————————————————————————————————————————————————————————
    public static Color GetBrightColor(Color Color)
    {
        double h, s, l;

        h = Color.GetHue() / 360.0;
        s = Color.GetSaturation();
        l = Color.GetBrightness();
        l *= 1.6;
        return ColorHSLToRGB(h, s, l);
    }

    //̶—————————————————————————————————————————————————————————————
    public static Color GetBrighterColor(Color Color)
    {
        double h, s, l;

        h = Color.GetHue() / 360.0;
        s = Color.GetSaturation();
        l = Color.GetBrightness();
        l *= 1.8;
        return ColorHSLToRGB(h, s, l);
    }

    //̶—————————————————————————————————————————————————————————————
    public static Color GetOppositeColor(Color Color)
    {
        double h, s, l;

        h = 360.0 - Color.GetHue();
        s = Color.GetSaturation();
        if (s < 20) s = 20;
        if (s > 345) s = 345;
        s = 360 - s;
        l = Color.GetBrightness();
        if (l < 20) l = 20;
        if (l > 345) l = 345;
        l = 360 - l;
        //l *= 1.8;
        return ColorHSLToRGB(h, s, l);
    }

    //̶—————————————————————————————————————————————————————————————
    public static Color GetGrayedOutColor2(Color Color)
    {
        double h, s, l;

        h = Color.GetHue() / 360.0;
        s = Color.GetSaturation();
        s = 0.001;
        l = Color.GetBrightness();
        return ColorHSLToRGB(h, s, l);
    }
}

public class VisibilityTester
{
    private delegate bool CallBackPtr(int hwnd, int lParam);
    private static CallBackPtr callBackPtr;

    /// The enumerated pointers of actually visible windows
    public static List<IntPtr> enumedwindowPtrs = new List<IntPtr> ();

    /// The enumerated rectangles of actually visible windows
    public static List<Rectangle> enumedwindowRects = new List<Rectangle> ();

    //̶—————————————————————————————————————————————————————————————
    /// Does a hit test for specified control (is point of control visible to user)
    /// the rectangle (usually Bounds) of the control
    /// the handle for the control
    /// the point to test (usually MousePosition)
    /// a control or window to exclude from hit test (means point is visible through this window)</param>
    /// boolean value indicating if p is visible for ctrlRect
    //̶—————————————————————————————————————————————————————————————
    public static bool HitTest(Rectangle ctrlRect, IntPtr ctrlHandle, Point p, IntPtr ExcludeWindow)
    {
        // clear results
        enumedwindowPtrs.Clear();
        enumedwindowRects.Clear();

        // Create callback and start enumeration
        callBackPtr = new CallBackPtr(EnumCallBack);
        EnumDesktopWindows(IntPtr.Zero, callBackPtr, 0);

        // Go from last to first window, and substract them from the ctrlRect area
        Region r = new Region (ctrlRect);

        bool StartClipping = false;
        for (int i = enumedwindowRects.Count - 1; i >= 0; i--)
        {
            if (StartClipping && enumedwindowPtrs[i] != ExcludeWindow)
            {
                r.Exclude(enumedwindowRects[i]);
            }

            if (enumedwindowPtrs[i] == ctrlHandle) StartClipping = true;
        }

        // return boolean indicating if point is visible to clipped (truly visible) window
        return r.IsVisible(p);
    }

    //̶—————————————————————————————————————————————————————————————
    /// Window enumeration callback
    //̶—————————————————————————————————————————————————————————————
    private static bool EnumCallBack(int hwnd, int lParam)
    {
        // If window is visible and not minimized (isiconic)
        if (IsWindow((IntPtr)hwnd) && IsWindowVisible((IntPtr)hwnd) && !IsIconic((IntPtr)hwnd))
        {
            // add the handle and windowrect to "found windows" collection
            enumedwindowPtrs.Add((IntPtr)hwnd);

            RECT rct;

            if (GetWindowRect((IntPtr)hwnd, out rct))
            {
                // add rect to list
                enumedwindowRects.Add(new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top));
            }
            else
            {
                // invalid, make empty rectangle
                enumedwindowRects.Add(new Rectangle(0, 0, 0, 0));
            }
        }

        return true;
    }


    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int EnumDesktopWindows(IntPtr hDesktop, CallBackPtr callPtr, int lPar);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner

        public override string ToString()
        {
            return Left + "," + Top + "," + Right + "," + Bottom;
        }
    }
*/
}
