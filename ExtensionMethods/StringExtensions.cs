using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.VisualBasic;

using Xunit;

namespace ExtensionMethods
{
    public static class StringExtensions
    {

        /*
                TTTTTTTTTTTTTTTTTTTTTTT                                                                         ffffffffffffffff                                                            
                T:::::::::::::::::::::T                                                                        f::::::::::::::::f                                                           
                T:::::::::::::::::::::T                                                                       f::::::::::::::::::f                                                          
                T:::::TT:::::::TT:::::T                                                                       f::::::fffffff:::::f                                                          
                TTTTTT  T:::::T  TTTTTTrrrrr   rrrrrrrrr   aaaaaaaaaaaaa  nnnn  nnnnnnnn        ssssssssss    f:::::f       ffffffooooooooooo   rrrrr   rrrrrrrrr      mmmmmmm    mmmmmmm   
                        T:::::T        r::::rrr:::::::::r  a::::::::::::a n:::nn::::::::nn    ss::::::::::s   f:::::f           oo:::::::::::oo r::::rrr:::::::::r   mm:::::::m  m:::::::mm 
                        T:::::T        r:::::::::::::::::r aaaaaaaaa:::::an::::::::::::::nn ss:::::::::::::s f:::::::ffffff    o:::::::::::::::or:::::::::::::::::r m::::::::::mm::::::::::m
                        T:::::T        rr::::::rrrrr::::::r         a::::ann:::::::::::::::ns::::::ssss:::::sf::::::::::::f    o:::::ooooo:::::orr::::::rrrrr::::::rm::::::::::::::::::::::m
                        T:::::T         r:::::r     r:::::r  aaaaaaa:::::a  n:::::nnnn:::::n s:::::s  ssssss f::::::::::::f    o::::o     o::::o r:::::r     r:::::rm:::::mmm::::::mmm:::::m
                        T:::::T         r:::::r     rrrrrrraa::::::::::::a  n::::n    n::::n   s::::::s      f:::::::ffffff    o::::o     o::::o r:::::r     rrrrrrrm::::m   m::::m   m::::m
                        T:::::T         r:::::r           a::::aaaa::::::a  n::::n    n::::n      s::::::s    f:::::f          o::::o     o::::o r:::::r            m::::m   m::::m   m::::m
                        T:::::T         r:::::r          a::::a    a:::::a  n::::n    n::::nssssss   s:::::s  f:::::f          o::::o     o::::o r:::::r            m::::m   m::::m   m::::m
                      TT:::::::TT       r:::::r          a::::a    a:::::a  n::::n    n::::ns:::::ssss::::::sf:::::::f         o:::::ooooo:::::o r:::::r            m::::m   m::::m   m::::m
                      T:::::::::T       r:::::r          a:::::aaaa::::::a  n::::n    n::::ns::::::::::::::s f:::::::f         o:::::::::::::::o r:::::r            m::::m   m::::m   m::::m
                      T:::::::::T       r:::::r           a::::::::::aa:::a n::::n    n::::n s:::::::::::ss  f:::::::f          oo:::::::::::oo  r:::::r            m::::m   m::::m   m::::m
                      TTTTTTTTTTT       rrrrrrr            aaaaaaaaaa  aaaa nnnnnn    nnnnnn  sssssssssss    fffffffff            ooooooooooo    rrrrrrr            mmmmmm   mmmmmm   mmmmmm          
         */
        public static int? ToInt(this string input)
        {
            int o;
            if (!int.TryParse(input, out o)) return null;
            return o;
        }

        public static int ToInt(this string input, int valueifnull)
        {
            int o;
            if (!int.TryParse(input, out o)) return valueifnull;
            return o;
        }

        public static bool? ToBool(this string input)
        {
            bool b;
            if (!bool.TryParse(input, out b)) return null;
            return b;
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuid(this string value)
        {
            return new Guid(value);
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value and returns Guid.Empty if conversion fails.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuidSave(this string value)
        {
            return value.ToGuidSave(Guid.Empty);
        }

        /// <summary>
        /// 	Convert the provided string to a Guid value and returns the provided default value if the conversion fails.
        /// </summary>
        /// <param name = "value">The original string value.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>The Guid</returns>
        public static Guid ToGuidSave(this string value, Guid defaultValue)
        {
            if (value.IsEmpty())
                return defaultValue;

            try
            {
                return value.ToGuid();
            }
            catch { }

            return defaultValue;
        }

        /// <summary>
        /// 	Loads the string into a LINQ to XML XDocument
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML document object model (XDocument)</returns>
        public static XDocument ToXDocument(this string xml)
        {
            return XDocument.Parse(xml);
        }

        /// <summary>
        /// 	Loads the string into a XML DOM object (XmlDocument)
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML document object model (XmlDocument)</returns>
        public static XmlDocument ToXmlDOM(this string xml)
        {
            var document = new XmlDocument();
            document.LoadXml(xml);
            return document;
        }

        /// <summary>
        /// 	Loads the string into a XML XPath DOM (XPathDocument)
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML XPath document object model (XPathNavigator)</returns>
        public static XPathNavigator ToXPath(this string xml)
        {
            var document = new XPathDocument(new StringReader(xml));
            return document.CreateNavigator();
        }

        /// <summary>
        ///     Loads the string into a LINQ to XML XElement
        /// </summary>
        /// <param name = "xml">The XML string.</param>
        /// <returns>The XML element object model (XElement)</returns>
        public static XElement ToXElement(this string xml)
        {
            return XElement.Parse(xml);
        }

        /// <summary>
        /// Cause I keep forgetting this
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Stream ToStream(this string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// 	Converts the string to a byte-array using the default encoding
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <returns>The created byte array</returns>
        public static byte[] ToBytes(this string value)
        {
            return value.ToBytes(null);
        }

        /// <summary>
        /// 	Converts the string to a byte-array using the supplied encoding
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "encoding">The encoding to be used.</param>
        /// <returns>The created byte array</returns>
        /// <example>
        /// 	<code>
        /// 		var value = "Hello World";
        /// 		var ansiBytes = value.ToBytes(Encoding.GetEncoding(1252)); // 1252 = ANSI
        /// 		var utf8Bytes = value.ToBytes(Encoding.UTF8);
        /// 	</code>
        /// </example>
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            encoding = (encoding ?? Encoding.Default);
            return encoding.GetBytes(value);
        }


        /// <summary>
        /// Convert a byte array into a hexadecimal string representation.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String BytesToHexString(this byte[] bytes, bool stripspaces = true)
        {
            string result = "";
            foreach (byte b in bytes)
            {
                result += " " + b.ToString("X").PadLeft(2, '0');
            }
            if (result.Length > 0) result = result.Substring(1);

            return result.RemoveChar(' ');
        }

        //todo: xml documentation requires
        //todo: unit test required
        public static byte[] GetBytes(this string data)
        {
            return Encoding.Default.GetBytes(data);
        }

        public static byte[] GetBytes(this string data, Encoding encoding)
        {
            return encoding.GetBytes(data);
        }

        /// <summary>
        /// Convert this string containing hexadecimal into a byte array.
        /// </summary>
        /// <param name="str">The hexadecimal string to convert.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] HexStringToBytes(this String str)
        {
            str = str.Replace(" ", "");
            int max_byte = str.Length / 2 - 1;
            var bytes = new byte[max_byte + 1];
            for (int i = 0; i <= max_byte; i++)
            {
                bytes[i] = byte.Parse(str.Substring(2 * i, 2), NumberStyles.AllowHexSpecifier);
            }

            return bytes;
        }

        /// <summary>
        /// 	Encodes the input value to a Base64 string using the default encoding.
        /// </summary>
        /// <param name = "value">The input value.</param>
        /// <returns>The Base 64 encoded string</returns>
        public static string EncodeBase64(this string value)
        {
            return value.EncodeBase64(null);
        }

        /// <summary>
        /// 	Encodes the input value to a Base64 string using the supplied encoding.
        /// </summary>
        /// <param name = "value">The input value.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <returns>The Base 64 encoded string</returns>
        public static string EncodeBase64(this string value, Encoding encoding)
        {
            encoding = (encoding ?? Encoding.UTF8);
            var bytes = encoding.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 	Decodes a Base 64 encoded value to a string using the default encoding.
        /// </summary>
        /// <param name = "encodedValue">The Base 64 encoded value.</param>
        /// <returns>The decoded string</returns>
        public static string DecodeBase64(this string encodedValue)
        {
            return encodedValue.DecodeBase64(null);
        }

        /// <summary>
        /// 	Decodes a Base 64 encoded value to a string using the supplied encoding.
        /// </summary>
        /// <param name = "encodedValue">The Base 64 encoded value.</param>
        /// <param name = "encoding">The encoding.</param>
        /// <returns>The decoded string</returns>
        public static string DecodeBase64(this string encodedValue, Encoding encoding)
        {
            encoding = (encoding ?? Encoding.UTF8);
            var bytes = Convert.FromBase64String(encodedValue);
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Encodes the email address so that the link is still valid, but the email address is useless for email harvsters.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        public static string EncodeEmailAddress(this string emailAddress)
        {
            int i;
            string repl;
            string tempHtmlEncode = emailAddress;
            for (i = tempHtmlEncode.Length; i >= 1; i--)
            {
                int acode = Convert.ToInt32(tempHtmlEncode[i - 1]);
                if (acode == 32)
                {
                    repl = " ";
                }
                else if (acode == 34)
                {
                    repl = "\"";
                }
                else if (acode == 38)
                {
                    repl = "&";
                }
                else if (acode == 60)
                {
                    repl = "<";
                }
                else if (acode == 62)
                {
                    repl = ">";
                }
                else if (acode >= 32 && acode <= 127)
                {
                    repl = "&#" + Convert.ToString(acode) + ";";
                }
                else
                {
                    repl = "&#" + Convert.ToString(acode) + ";";
                }
                if (repl.Length > 0)
                {
                    tempHtmlEncode = tempHtmlEncode.Substring(0, i - 1) +
                                     repl + tempHtmlEncode.Substring(i);
                }
            }
            return tempHtmlEncode;
        }

        /// <summary>
        /// Centers a charters in this string, padding in both, left and right, by specified Unicode character,
        /// for a specified total lenght.
        /// </summary>
        /// <param name="value">Instance value.</param>
        /// <param name="width">The number of characters in the resulting string, 
        /// equal to the number of original characters plus any additional padding characters.
        /// </param>
        /// <param name="padChar">A Unicode padding character.</param>
        /// <param name="truncate">Should get only the substring of specified width if string width is 
        /// more than the specified width.</param>
        /// <returns>A new string that is equivalent to this instance, 
        /// but center-aligned with as many paddingChar characters as needed to create a 
        /// length of width paramether.</returns>
        public static string PadBoth(this string value, int width, char padChar, bool truncate = false)
        {
            int diff = width - value.Length;
            if (diff == 0 || diff < 0 && !(truncate))
            {
                return value;
            }
            else if (diff < 0)
            {
                return value.Substring(0, width);
            }
            else
            {
                return value.PadLeft(width - diff / 2, padChar).PadRight(width, padChar);
            }
        }

        /// <summary>
        /// 	Ensures that a string starts with a given prefix.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        /// <param name = "prefix">The prefix value to check for.</param>
        /// <returns>The string value including the prefix</returns>
        /// <example>
        /// 	<code>
        /// 		var extension = "txt";
        /// 		var fileName = string.Concat(file.Name, extension.EnsureStartsWith("."));
        /// 	</code>
        /// </example>
        public static string EnsureStartsWith(this string value, string prefix)
        {
            return value.StartsWith(prefix) ? value : string.Concat(prefix, value);
        }

        /// <summary>
        /// 	Ensures that a string ends with a given suffix.
        /// </summary>
        /// <param name = "value">The string value to check.</param>
        /// <param name = "suffix">The suffix value to check for.</param>
        /// <returns>The string value including the suffix</returns>
        /// <example>
        /// 	<code>
        /// 		var url = "http://www.pgk.de";
        /// 		url = url.EnsureEndsWith("/"));
        /// 	</code>
        /// </example>
        public static string EnsureEndsWith(this string value, string suffix)
        {
            return value.EndsWith(suffix) ? value : string.Concat(value, suffix);
        }

        public static string NullIfEmpty(this string s)
        {
            if (s == null) return null;
            if (s.IsEmptyOrWhiteSpace()) return null;
            return s;
        }

        public static T NullIf<T>(this T s, bool iftrue)
        {
            if (iftrue) return default;
            return s;
        }

        /// <summary>
        /// Formats the value with the parameters using string.Format.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "parameters">The parameters.</param>
        /// <returns></returns>
        public static string FormatWith(this string value, params object[] parameters)
        {
            return string.Format(value, parameters);
        }

        /// <summary>
        /// The to sentence case. Injects space.
        /// </summary>
        /// <param name="s">The str.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string FromPascalToSentenceCase(this string s)  // PascalCase => 
        {
            return Regex.Replace(s, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }

        /// <summary>
        /// The from pascal to title case. Injects space.
        /// </summary>
        /// <param name="s">The str.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string FromPascalToTitleCase(this string s)  // PascalCase to 
        {
            // TODO: Unless multiple uppercase in a row?
            return Regex.Replace(s, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToUpper(m.Value[1])}");
        }

        /// <summary>Uppercase First Letter</summary>
        /// <param name = "value">The string value to process</param>
        public static string ToUpperFirstLetter(this string value)
        {
            if (value.IsEmptyOrWhiteSpace()) return string.Empty;

            char[] valueChars = value.ToCharArray();
            valueChars[0] = char.ToUpper(valueChars[0]);

            return new string(valueChars);
        }

        /// <summary>
        /// The lower first.  So turn a nameof(DetailRow) into "detailRow", the prefix for our cell instances.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string LowerFirst(this string s)
        {
            if (s.IsNotVoid() && s.Length >= 0) // 
                return Char.ToLower(s[0]) + s.Substring(1);
            else
                return s;
        }

        // PascalCase
        // camelCase
        // Title Case
        // Sentence case
        // UPPER CASE
        // lower case
        // UPPERCASE_WITH_UNDERSCORES
        // lowercase_with_underscores
        // kebab-case

        /// <summary>Convert text's case to a title case</summary>
        /// <remarks>UppperCase characters is the source string after the first of each word are lowered, unless the word is exactly 2 characters</remarks>
        public static string ToTitleCase(this string value)
        {
            return ToTitleCase(value, CultureInfo.CurrentCulture);
        }

        /// <summary>Convert text's case to a title case</summary>
        /// <remarks>UppperCase characters is the source string after the first of each word are lowered, unless the word is exactly 2 characters</remarks>
        public static string ToTitleCase(this string value, CultureInfo culture)
        {
            return culture.TextInfo.ToTitleCase(value);
        }

        public static string ToRegExCompat(this string value)
        {
            return Regex.Escape(value);
        }

        public static string ToSingleLine(this string value)
        {
            return value.Replace("\r\n", "<nl>", "\n", "<nl>");
        }

        public static string ToSingleLine(this string[] values)
        {
            return values.Join("\r\n").ToSingleLine();
        }

        public static Dictionary<string, AcronymDef> upperCaseTheseStrings = null;

        public enum NameTypeEnum
        {
            Organizations,           // DAV  Also include stop words as lowercase
            Persons,              // III, II, IV.   lower case "de", "la"
            Addresses,              // P.O. BOX    AFB
            Activities,          // Similar to Organizations with stop words, even may have "!"  more eclectic
            UNDEFINED
        }

        /// <summary>
        /// TODO: Deal with multi-character delimiters, and nested brackets (enclosures)
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static string ToSmartTitleCase(this string inputText, NameTypeEnum nameTypeEnum, string chardelimitersasstring = @",@.,(){}/""#<>- ;:\~|!?[]=+<>")
        {
            var delimiters = chardelimitersasstring.ToCharArray();
            if (inputText.IsVoid()) return inputText;

            bool madeACaseChange = false;

            if (upperCaseTheseStrings == null)
            {
                upperCaseTheseStrings = new Dictionary<string, AcronymDef>(100);
                using (SqlConnection connection = new SqlConnection("Server=softdev;Integrated Security=SSPI;Initial Catalog=Reference Data;"))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(
                        " SELECT UPPER(acronym) [key], forPurposeOf, acronym, max(expansion) expansion, MAX(CAST(MustExpandInLiveInput as int)) MustExpandInLiveInput from [Ambiguous Acronyms] where (DoNotUpperCase IS NULL OR DoNotUpperCase = 0) and acronym collate Latin1_General_CS_AS = UPPER(acronym) " +
                        " GROUP BY acronym, forPurposeOf " +
                        " UNION " +
                        " SELECT UPPER(acronym) [key], forPurposeOf, acronym, max(expansion) expansion, MAX(CAST(MustExpandInLiveInput as int)) MustExpandInLiveInput from [Acronyms] where (DoNotUpperCase IS NULL OR DoNotUpperCase = 0) and acronym collate Latin1_General_CS_AS = UPPER(acronym) " +
                        " GROUP BY acronym, forPurposeOf "
                        , connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // while there is another record present
                        while (reader.Read())
                        {
                            var upperCasedAcronym = reader.Get<string>(0);
                            var purpose = reader.Get<string>(1);
                            var titleCasedAcronym = reader.Get<string>(2);
                            var expansion = reader.Get<string>(3);
                            var mustExpand = reader.Get<bool>(4);
                            var key = upperCasedAcronym + "/" + purpose;
                            var acronymDef = new AcronymDef() {key = key, purpose = purpose, acronymInTitleCase = titleCasedAcronym, expansion = expansion, forceExpansion = mustExpand };
                            upperCaseTheseStrings.Add(key, acronymDef);  // The key is UPPER, but the value is mixed, like "PhD"
                        }
                    }
                }
            }

            IEnumerable<SplitLineItem> inputTextWordDefs = inputText.SplitAndKeep(delimiters);
            var inputTextWordsAsArray = inputTextWordDefs.ToArray(); // So we can alter words by index.
            List<string> purposeFilters = new List<string>(3);

            // Based on the business class of string we are working with, these are the upper-case names we want to look at.

            switch (nameTypeEnum)
            {
                case NameTypeEnum.Addresses:
                    purposeFilters.AddRange(new[] { "States", "Units", "Military States", "Provinces", "Addresses" });
                    break;
                case NameTypeEnum.Persons:
                    purposeFilters.AddRange(new[] { "Persons" }); // Titles, like MD, DDR, BS, BA, III, II, IV, I, but not David Darcie
                    break;
                case NameTypeEnum.Organizations:
                    purposeFilters.AddRange(new[] { "Organizations", "States", "Units" });  // DAV
                    break;
            }

            foreach (var purposeFilter in purposeFilters)
            {
                foreach (var inputTextWordDef in inputTextWordDefs)
                {
                    if (upperCaseTheseStrings.ContainsKey(inputTextWordDef.word.ToUpper() + "/" + purposeFilter))
                    {
                        string titledCaseWord = upperCaseTheseStrings[inputTextWordDef.word.ToUpper() + "/" + purposeFilter].acronymInTitleCase;
                        bool mustExpand = upperCaseTheseStrings[inputTextWordDef.word.ToUpper() + "/" + purposeFilter].forceExpansion;
                        string expandedTitleCasedWord = upperCaseTheseStrings[inputTextWordDef.word.ToUpper() + "/" + purposeFilter].expansion;

                        // Test for organization type name.  We don't want to capture "DAV" as we type "David" and have its case jump around

                        if (inputTextWordDef.word.ForceToTitleCase() != titledCaseWord || mustExpand)
                        {
                            madeACaseChange = true;
                            if (mustExpand)
                            {
                                inputTextWordsAsArray[inputTextWordDef.pos].word = expandedTitleCasedWord;
                            }
                            else
                            {
                                inputTextWordsAsArray[inputTextWordDef.pos].word = titledCaseWord; // Correct
                            }

                            inputTextWordsAsArray[inputTextWordDef.pos].changedCase = true;
                        }
                    }

                    for (int i = 0; i < inputTextWordDef.prevnestedphrases.Length; i++)
                    {
                        var nestedPhrase = inputTextWordDef.prevnestedphrases[i];
                        if (upperCaseTheseStrings.ContainsKey(nestedPhrase.ToUpper() + "/" + purposeFilter))
                        {
                            // Check again; for addresses we need to detect "P.O. BOX"

                            string titleCasedPhrase = upperCaseTheseStrings[nestedPhrase.ToUpper() + "/" + purposeFilter].acronymInTitleCase;
                            bool mustExpand = upperCaseTheseStrings[nestedPhrase.ToUpper() + "/" + purposeFilter].forceExpansion;
                            string expandedTitleCasedPhrase = upperCaseTheseStrings[nestedPhrase.ToUpper() + "/" + purposeFilter].expansion;

                            if (nestedPhrase.ForceToTitleCase() != titleCasedPhrase || mustExpand)
                            {
                                madeACaseChange = true;
                                var nestedPhraseWords = nestedPhrase.Split(delimiters);
                                var titleCasedWordsFromNestedPhrase = titleCasedPhrase.Split(delimiters);

                                if (mustExpand)
                                {

                                    for (int j = 0; j < nestedPhraseWords.Length; j++)
                                    {
                                        int idx = inputTextWordDef.pos - (nestedPhraseWords.Length - j) + 1;
                                        if (j == 0)
                                        {
                                            inputTextWordsAsArray[idx].word = expandedTitleCasedPhrase;
                                            inputTextWordsAsArray[idx].changedCase = true;
                                        }
                                        else
                                        {
                                            inputTextWordsAsArray[idx].dropfromoutput = true;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < nestedPhraseWords.Length; j++)
                                    {
                                        int idx = inputTextWordDef.pos - (nestedPhraseWords.Length - j) + 1;
                                        var originalWord = inputTextWordsAsArray[idx].word;
                                        var titleCasedWordFromNestedPhrase = titleCasedWordsFromNestedPhrase[idx];
                                        if (originalWord != titleCasedWordFromNestedPhrase)
                                        {
                                            madeACaseChange = true;
                                            inputTextWordsAsArray[idx].word = titleCasedWordFromNestedPhrase; // Did separator change, too??
                                            inputTextWordsAsArray[inputTextWordDef.pos].changedCase = true;
                                        }
                                    }
                                }

                                break; // Replaced a nested preset of words - get out of loop before we corrupt
                            }
                        }
                    }
                }
            }

            titledText.Clear();

            foreach (var inputTextWordDef in inputTextWordsAsArray)
            {
                if (inputTextWordDef.dropfromoutput) continue; // Drop word from output.
                var localword = inputTextWordDef.word;
                if (!inputTextWordDef.changedCase)
                {
                    localword = localword.ForceToTitleCase();
                }

                titledText.Append(localword);
                titledText.Append(inputTextWordDef.separator);
            }

            return titledText.ToString();
        }

        public static string ForceToTitleCase(this string s)
        {
            return s.ToLower().ToTitleCase(); // Full uppercase words won't shift into title case for some reason.
        }

        /// <summary>
        /// Add space on every upper character
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The adjusted string.</returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string SpaceOnUpper(this string value)
        {
            return Regex.Replace(value, "([A-Z])(?=[a-z])|(?<=[a-z])([A-Z]|[0-9]+)", " $1$2").TrimStart();
        }

        /// <summary>
        /// 	Checks whether the string is empty and returns a default value in case.
        /// </summary>
        /// <param name = "value">The string to check.</param>
        /// <param name = "defaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfEmpty(this string value, string defaultValue)
        {
            return (value.IsNotEmpty() ? value : defaultValue);
        }

        /// <summary>
        /// Options to match the template with the original string
        /// </summary>
        public enum ComparsionTemplateOptions
        {
            /// <summary>
            /// Free template comparsion
            /// </summary>
            Default,

            /// <summary>
            /// Template compared from beginning of input string
            /// </summary>
            FromStart,

            /// <summary>
            /// Template compared with the end of input string
            /// </summary>
            AtTheEnd,

            /// <summary>
            /// Template compared whole with input string
            /// </summary>
            Whole,
        }

        public static string ToPlural(this string singular)
        {
            // Multiple words in the form A of B : Apply the plural to the first word only (A)
            int index = singular.LastIndexOf(" of ");
            if (index > 0) return (singular.Substring(0, index)) + singular.Remove(0, index).ToPlural();

            // single Word rules
            //sibilant ending rule
            if (singular.EndsWith("sh")) return singular + "es";
            if (singular.EndsWith("ch")) return singular + "es";
            if (singular.EndsWith("us")) return singular + "es";
            if (singular.EndsWith("ss")) return singular + "es";
            //-ies rule
            if (singular.EndsWith("y")) return singular.Remove(singular.Length - 1, 1) + "ies";
            // -oes rule
            if (singular.EndsWith("o")) return singular.Remove(singular.Length - 1, 1) + "oes";
            // -s suffix rule
            return singular + "s";
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s)
        {
            return s.ToHtmlSafe(false, false);
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <param name="all">Whether to make all characters entities or just those needed.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s, bool all)
        {
            return s.ToHtmlSafe(all, false);
        }

        /// <summary>
        /// Makes the current instance HTML safe.
        /// </summary>
        /// <param name="s">The current instance.</param>
        /// <param name="all">Whether to make all characters entities or just those needed.</param>
        /// <param name="replace">Whether or not to encode spaces and line breaks.</param>
        /// <returns>An HTML safe string.</returns>
        public static string ToHtmlSafe(this string s, bool all, bool replace)
        {
            if (s.IsEmptyOrWhiteSpace())
                return string.Empty;
            var entities = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 28, 29, 30, 31, 34, 39, 38, 60, 62, 123, 124, 125, 126, 127, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 215, 247, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 8704, 8706, 8707, 8709, 8711, 8712, 8713, 8715, 8719, 8721, 8722, 8727, 8730, 8733, 8734, 8736, 8743, 8744, 8745, 8746, 8747, 8756, 8764, 8773, 8776, 8800, 8801, 8804, 8805, 8834, 8835, 8836, 8838, 8839, 8853, 8855, 8869, 8901, 913, 914, 915, 916, 917, 918, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929, 931, 932, 933, 934, 935, 936, 937, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965, 966, 967, 968, 969, 977, 978, 982, 338, 339, 352, 353, 376, 402, 710, 732, 8194, 8195, 8201, 8204, 8205, 8206, 8207, 8211, 8212, 8216, 8217, 8218, 8220, 8221, 8222, 8224, 8225, 8226, 8230, 8240, 8242, 8243, 8249, 8250, 8254, 8364, 8482, 8592, 8593, 8594, 8595, 8596, 8629, 8968, 8969, 8970, 8971, 9674, 9824, 9827, 9829, 9830 };
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (all || entities.Contains(c))
                    sb.Append("&#" + ((int)c) + ";");
                else
                    sb.Append(c);
            }

            return replace ? sb.Replace("", "<br />").Replace("\n", "<br />").Replace(" ", "&nbsp;").ToString() : sb.ToString();
        }

        /// <summary>
        /// Convert a input string to a byte array and compute the hash.
        /// </summary>
        /// <param name="s">Input string.</param>
        /// <returns>Hexadecimal string.</returns>
        public static string ToMd5Hash(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] originalBytes = ASCIIEncoding.Default.GetBytes(s);
                byte[] encodedBytes = md5.ComputeHash(originalBytes);
                return BitConverter.ToString(encodedBytes).Replace("-", string.Empty);
            }
        }

        /// <summary>
        ///     Parse a string to a enum item if that string exists in the enum otherwise return the default enum item.
        /// </summary>
        /// <typeparam name="TEnum">The Enum type.</typeparam>
        /// <param name="dataToMatch">The data will use to convert into give enum</param>
        /// <param name="ignorecase">Whether the enum parser will ignore the given data's case or not.</param>
        /// <returns>Converted enum.</returns>
        /// <example>
        /// 	<code>
        /// 		public enum EnumTwo {  None, One,}
        /// 		object[] items = new object[] { "One".ParseStringToEnum<EnumTwo>(), "Two".ParseStringToEnum<EnumTwo>() };
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static TEnum ParseStringToEnum<TEnum>(this string dataToMatch, bool ignorecase = default(bool))
                where TEnum : struct
        {
            return dataToMatch.IsItemInEnum<TEnum>()() ? default(TEnum) : (TEnum)Enum.Parse(typeof(TEnum), dataToMatch, default(bool));
        }

        /// <summary>
        /// Encrypt this string into a byte array.
        /// </summary>
        /// <param name="plain_text">The string being extended and that will be encrypted.</param>
        /// <param name="password">The password to use then encrypting the string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] EncryptToByteArray(this String plain_text, String password)
        {
            var ascii_encoder = new ASCIIEncoding();
            byte[] plain_bytes = ascii_encoder.GetBytes(plain_text);
            return CryptBytes(password, plain_bytes, true);
        }

        /// <summary>
        /// Decrypt the encryption stored in this byte array.
        /// </summary>
        /// <param name="encrypted_bytes">The byte array to decrypt.</param>
        /// <param name="password">The password to use when decrypting.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String DecryptFromByteArray(this byte[] encrypted_bytes, String password)
        {
            byte[] decrypted_bytes = CryptBytes(password, encrypted_bytes, false);
            var ascii_encoder = new ASCIIEncoding();
            return new String(ascii_encoder.GetChars(decrypted_bytes));
        }

        /// <summary>
        /// Encrypt this string and return the result as a string of hexadecimal characters.
        /// </summary>
        /// <param name="plain_text">The string being extended and that will be encrypted.</param>
        /// <param name="password">The password to use then encrypting the string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String EncryptToString(this String plain_text, String password)
        {
            return plain_text.EncryptToByteArray(password).BytesToHexString();
        }

        /// <summary>
        /// Decrypt the encryption stored in this string of hexadecimal values.
        /// </summary>
        /// <param name="encrypted_bytes_string">The hexadecimal string to decrypt.</param>
        /// <param name="password">The password to use then encrypting the string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String DecryptFromString(this String encrypted_bytes_string, String password)
        {
            return encrypted_bytes_string.HexStringToBytes().DecryptFromByteArray(password);
        }

        /// <summary>
        /// Encrypt or decrypt a byte array using the TripleDESCryptoServiceProvider crypto provider and Rfc2898DeriveBytes to build the key and initialization vector.
        /// </summary>
        /// <param name="password">The password string to use in encrypting or decrypting.</param>
        /// <param name="in_bytes">The array of bytes to encrypt.</param>
        /// <param name="encrypt">True to encrypt, False to decrypt.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static byte[] CryptBytes(String password, byte[] in_bytes, bool encrypt)
        {
            // Make a triple DES service provider.
            var des_provider = new TripleDESCryptoServiceProvider();

            // Find a valid key size for this provider.
            int key_size_bits = 0;
            for (int i = 1024; i >= 1; i--)
            {
                if (des_provider.ValidKeySize(i))
                {
                    key_size_bits = i;
                    break;
                }
            }

            // Get the block size for this provider.
            int block_size_bits = des_provider.BlockSize;

            // Generate the key and initialization vector.
            byte[] key = null;
            byte[] iv = null;
            byte[] salt = { 0x10, 0x20, 0x12, 0x23, 0x37, 0xA4, 0xC5, 0xA6, 0xF1, 0xF0, 0xEE, 0x21, 0x22, 0x45 };
            MakeKeyAndIV(password, salt, key_size_bits, block_size_bits, ref key, ref iv);

            // Make the encryptor or decryptor.
            ICryptoTransform crypto_transform = encrypt
                                                ? des_provider.CreateEncryptor(key, iv)
                                                : des_provider.CreateDecryptor(key, iv);

            // Create the output stream.
            var out_stream = new MemoryStream();

            // Attach a crypto stream to the output stream.
            var crypto_stream = new CryptoStream(out_stream,
                                             crypto_transform, CryptoStreamMode.Write);

            // Write the bytes into the CryptoStream.
            crypto_stream.Write(in_bytes, 0, in_bytes.Length);
            try
            {
                crypto_stream.FlushFinalBlock();
            }
            catch (CryptographicException)
            {
                // Ignore this one. The password is bad.
            }

            // Save the result.
            byte[] result = out_stream.ToArray();

            // Close the stream.
            try
            {
                crypto_stream.Close();
            }
            catch (CryptographicException)
            {
                // Ignore this one. The password is bad.
            }
            out_stream.Close();

            return result;
        }

        /// <summary>
        /// Returns a default value if the string is null or empty.
        /// </summary>
        /// <param name="s">Original String</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string DefaultIfNullOrEmpty(this string s, string defaultValue)
        {
            return String.IsNullOrEmpty(s) ? defaultValue : s;
        }

        /// <summary>
        /// Throws an <see cref="System.ArgumentException"/> if the string value is empty.
        /// </summary>
        /// <param name="obj">The value to test.</param>
        /// <param name="message">The message to display if the value is null.</param>
        /// <param name="name">The name of the parameter being tested.</param>
        public static string ExceptionIfNullOrEmpty(this string obj, string message, string name)
        {
            if (String.IsNullOrEmpty(obj))
                throw new ArgumentException(message, name);
            return obj;
        }


        /*
          
          
          
          
                                         TTTTTTTTTTTTTTTTTTTTTTT                                          tttt          
                                        T:::::::::::::::::::::T                                       ttt:::t          
                                        T:::::::::::::::::::::T                                       t:::::t          
                                        T:::::TT:::::::TT:::::T                                       t:::::t          
                                        TTTTTT  T:::::T  TTTTTTeeeeeeeeeeee        ssssssssss   ttttttt:::::ttttttt    
                                                T:::::T      ee::::::::::::ee    ss::::::::::s  t:::::::::::::::::t    
                                                T:::::T     e::::::eeeee:::::eess:::::::::::::s t:::::::::::::::::t    
                                                T:::::T    e::::::e     e:::::es::::::ssss:::::stttttt:::::::tttttt    
                                                T:::::T    e:::::::eeeee::::::e s:::::s  ssssss       t:::::t          
                                                T:::::T    e:::::::::::::::::e    s::::::s            t:::::t          
                                                T:::::T    e::::::eeeeeeeeeee        s::::::s         t:::::t          
                                                T:::::T    e:::::::e           ssssss   s:::::s       t:::::t    tttttt
                                              TT:::::::TT  e::::::::e          s:::::ssss::::::s      t::::::tttt:::::t
                                              T:::::::::T   e::::::::eeeeeeee  s::::::::::::::s       tt::::::::::::::t
                                              T:::::::::T    ee:::::::::::::e   s:::::::::::ss          tt:::::::::::tt
                                              TTTTTTTTTTT      eeeeeeeeeeeeee    sssssssssss              ttttttttttt 

         
         
         
         
         
         
         
         */

        /// <summary>
        /// 	Tests whether the contents of a string is a numeric value
        /// </summary>
        /// <param name = "value">String to check</param>
        /// <returns>
        /// 	Boolean indicating whether or not the string contents are numeric
        /// </returns>
        /// <remarks>
        /// 	Contributed by Kenneth Scott
        /// </remarks>
        public static bool IsNumeric(this string value)
        {
            float output;
            return float.TryParse(value, out output);
        }

        /// <summary>
        /// Sees if string is made up of hex?  Not tested with multiple characters.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        /// <summary>
        /// The is unicode.
        /// </summary>
        /// <param name="s">The value.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsUnicode(this string s)
        {
            int asciiBytesCount = System.Text.Encoding.ASCII.GetByteCount(s);
            int unicodBytesCount = System.Text.Encoding.UTF8.GetByteCount(s);

            if (asciiBytesCount != unicodBytesCount)
            {
                return true;
            }
            return false;
        }

        public static bool IsHex(this string s)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static bool IsEmpty(this string value)
        {
            return ((value == null) || (value.Length == 0));
        }

        public static bool IsNotEmpty(this string value)
        {
            return (value.IsEmpty() == false);
        }

        /// <summary>Finds out if the specified string contains null, empty or consists only of white-space characters</summary>
        /// <param name = "value">The input string</param>
        public static bool IsEmptyOrWhiteSpace(this string value)
        {
            return (value.IsEmpty() || value.All(t => char.IsWhiteSpace(t)));
        }

        /// <summary>Determines whether the specified string is not null, empty or consists only of white-space characters</summary>
        /// <param name = "value">The string value to check</param>
        public static bool IsNotEmptyOrWhiteSpace(this string value)
        {
            return (value.IsEmptyOrWhiteSpace() == false);
        }

        /// <summary>Checks whether the string is null, empty or consists only of white-space characters and returns a default value in case</summary>
        /// <param name = "value">The string to check</param>
        /// <param name = "defaultValue">The default value</param>
        /// <returns>Either the string or the default value</returns>
        public static string IfEmptyOrWhiteSpace(this string value, string defaultValue)
        {
            return (value.IsEmptyOrWhiteSpace() ? defaultValue : value);
        }

        public static bool StartsWithVowel(this string s)
        {
            return s[0].In('A', 'E', 'I', 'O', 'U') || s[0].In('a', 'e', 'i', 'o', 'u');
        }

        public static bool IsVoid(this string input)
        {
            return input == null || (String.IsNullOrEmpty(input)) || (input != null && String.IsNullOrEmpty(input)) || (input.Trim() != null && String.IsNullOrEmpty(input.Trim()));
        }

        public static bool IsNotVoid(this string input)
        {
            return !(input == null || input?.Length == 0 || (input != null && String.IsNullOrEmpty(input)) || (input.Trim() != null && String.IsNullOrEmpty(input.Trim())));
        }


        /// <summary>
        /// Returns true if strings are equals, without consideration to case (<see cref="StringComparison.InvariantCultureIgnoreCase"/>)
        /// </summary>
        public static bool EquivalentTo(this string s, string whateverCaseString)
        {
            return string.Equals(s, whateverCaseString, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 	Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var isMatching = s.IsMatchingTo(@"^\d+$");
        /// 	</code>
        /// </example>
        public static bool IsMatchingTo(this string value, string regexPattern)
        {
            return IsMatchingTo(value, regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var isMatching = s.IsMatchingTo(@"^\d+$");
        /// 	</code>
        /// </example>
        public static bool IsMatchingTo(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.IsMatch(value, regexPattern, options);
        }

        private static bool StringContainsEquivalence(string inputValue, string comparisonValue)
        {
            return (inputValue.IsNotEmptyOrWhiteSpace() && inputValue.Contains(comparisonValue, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool BothStringsAreEmpty(string inputValue, string comparisonValue)
        {
            return (inputValue.IsEmptyOrWhiteSpace() && comparisonValue.IsEmptyOrWhiteSpace());
        }

        public static bool? Like(this string input, params string[] patterns)
        {
            if (input == null) return null;
            foreach (var pattern in patterns)
                if (Regex.Match(input, pattern, RegexOptions.IgnoreCase).Success)
                {
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Simple like support.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Like(this string s, string pattern)
        {
            Regex regex = new Regex(pattern);
            Match match = regex.Match(s);
            return match.Success;
        }

        /// <summary>
        /// Wildcard comparison for any pattern
        /// </summary>
        /// <param name="value">The current <see cref="System.String"/> object</param>
        /// <param name="patterns">The array of string patterns</param>
        /// <returns></returns>
        public static bool IsLikeAny(this string value, params string[] patterns)
        {
            return patterns.Any(p => value.IsLike(p));
        }

        /// <summary>
        /// Wildcard comparison
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsLike(this string input, string pattern)
        {
            if (input == pattern) return true;

            if (pattern[0] == '*' && pattern.Length > 1)
            {
                for (int index = 0; index < input.Length; index++)
                {
                    if (input.Substring(index).IsLike(pattern.Substring(1)))
                        return true;
                }
            }
            else if (pattern[0] == '*')
            {
                return true;
            }
            else if (pattern[0] == input[0])
            {
                return input.Substring(1).IsLike(pattern.Substring(1));
            }
            return false;
        }

        /// <summary>
        ///     To check whether the data is defined in the given enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum will use to check, the data defined.</typeparam>
        /// <param name="dataToCheck">To match against enum.</param>
        /// <returns>Anonoymous method for the condition.</returns>
        /// <remarks>
        /// 	Contributed by Mohammad Rahman, http://mohammad-rahman.blogspot.com/
        /// </remarks>
        public static Func<bool> IsItemInEnum<TEnum>(this string dataToCheck)
            where TEnum : struct
        {
            return () => { return string.IsNullOrEmpty(dataToCheck) || !Enum.IsDefined(typeof(TEnum), dataToCheck); };
        }

        /// <summary>
        /// Used to be "In" but Contains fits current standards.
        /// </summary>
        /// <param name="input">The str.</param>
        /// <param name="instrings">The instrings.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Contains(this string input, params string[] instrings)
        {
            foreach (string ls in instrings)
            {
                if (input == ls)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="cs">The cs.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Contains(this string s, params char[] cs)
        {
            return (s.Any<char>(c => cs.Contains(c)));
        }

        public static bool Contains(this string[] inputs, StringComparison stringComparison, params string[] search)
        {
            return search.Intersect(inputs, StringComparison2StringComparer(stringComparison)).Any();
        }


        public static bool Contains(this string input, StringComparison stringComparison, params string[] search)
        {
            return search.Contains(input, StringComparison2StringComparer(stringComparison));
        }

        /// <summary>
        /// 	Determines whether the comparison value strig is contained within the input value string
        /// </summary>
        /// <param name = "inputValue">The input value.</param>
        /// <param name = "comparisonValue">The comparison value.</param>
        /// <param name = "comparisonType">Type of the comparison to allow case sensitive or insensitive comparison.</param>
        /// <returns>
        /// 	<c>true</c> if input value contains the specified value, otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string inputValue, string comparisonValue, StringComparison comparisonType)
        {
            return (inputValue.IndexOf(comparisonValue, comparisonType) != -1);
        }

        /// <summary>
        /// 	Determines whether the comparison value string is contained within the input value string without any
        ///     consideration about the case (<see cref="StringComparison.InvariantCultureIgnoreCase"/>).
        /// </summary>
        /// <param name = "inputValue">The input value.</param>
        /// <param name = "comparisonValue">The comparison value.  Case insensitive</param>
        /// <returns>
        /// 	<c>true</c> if input value contains the specified value (case insensitive), otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsEquivalenceTo(this string inputValue, string comparisonValue)
        {
            return BothStringsAreEmpty(inputValue, comparisonValue) || StringContainsEquivalence(inputValue, comparisonValue);
        }

        /// <summary>
        /// Determines whether the string contains any of the provided values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string value, params string[] values)
        {
            return value.ContainsAny(StringComparison.CurrentCulture, values);
        }

        /// <summary>
        /// Determines whether the string contains any of the provided values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string value, StringComparison comparisonType, params string[] values)
        {
            return values.Any(v => value.IndexOf(v, comparisonType) > -1);

        }

        /// <summary>
        ///     A string extension method that query if '@this' contains all values.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <returns>true if it contains all values, otherwise false.</returns>
        public static bool ContainsAll(this string @this, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.IndexOf(value) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     A string extension method that query if this object contains the given @this.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="values">A variable-length parameters list containing values.</param>
        /// <returns>true if it contains all values, otherwise false.</returns>
        public static bool ContainsAll(this string @this, StringComparison comparisonType, params string[] values)
        {
            foreach (string value in values)
            {
                if (@this.IndexOf(value, comparisonType) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the string is equal to any of the provided values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="comparisonType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool EqualsAny(this string value, StringComparison comparisonType, params string[] values)
        {
            return values.Any(v => value.Equals(v, comparisonType));
        }
        
        private static StringComparer StringComparison2StringComparer(StringComparison stringComparison)
        {
            StringComparer sc = null;
            switch (stringComparison)
            {
                case StringComparison.CurrentCulture:
                    sc = StringComparer.CurrentCulture;
                    break;
                case StringComparison.CurrentCultureIgnoreCase:
                    sc = StringComparer.CurrentCultureIgnoreCase;
                    break;
                case StringComparison.InvariantCulture:
                    sc = StringComparer.InvariantCulture;
                    break;
                case StringComparison.InvariantCultureIgnoreCase:
                    sc = StringComparer.InvariantCultureIgnoreCase;
                    break;
                case StringComparison.Ordinal:
                    sc = StringComparer.Ordinal;
                    break;
                case StringComparison.OrdinalIgnoreCase:
                    sc = StringComparer.OrdinalIgnoreCase;
                    break;
                default:
                    throw new Exception("Unrecognized StringComparison");
            }
            return sc;
        }

        /*







                             MMMMMMMM               MMMMMMMM                                                                                                             
                             M:::::::M             M:::::::M                                                                                                             
                             M::::::::M           M::::::::M                                                                                                             
                             M:::::::::M         M:::::::::M                                                                                                             
                             M::::::::::M       M::::::::::M    eeeeeeeeeeee    aaaaaaaaaaaaa      ssssssssss   uuuuuu    uuuuuu rrrrr   rrrrrrrrr       eeeeeeeeeeee    
                             M:::::::::::M     M:::::::::::M  ee::::::::::::ee  a::::::::::::a   ss::::::::::s  u::::u    u::::u r::::rrr:::::::::r    ee::::::::::::ee  
                             M:::::::M::::M   M::::M:::::::M e::::::eeeee:::::eeaaaaaaaaa:::::ass:::::::::::::s u::::u    u::::u r:::::::::::::::::r  e::::::eeeee:::::ee
                             M::::::M M::::M M::::M M::::::Me::::::e     e:::::e         a::::as::::::ssss:::::su::::u    u::::u rr::::::rrrrr::::::re::::::e     e:::::e
                             M::::::M  M::::M::::M  M::::::Me:::::::eeeee::::::e  aaaaaaa:::::a s:::::s  ssssss u::::u    u::::u  r:::::r     r:::::re:::::::eeeee::::::e
                             M::::::M   M:::::::M   M::::::Me:::::::::::::::::e aa::::::::::::a   s::::::s      u::::u    u::::u  r:::::r     rrrrrrre:::::::::::::::::e 
                             M::::::M    M:::::M    M::::::Me::::::eeeeeeeeeee a::::aaaa::::::a      s::::::s   u::::u    u::::u  r:::::r            e::::::eeeeeeeeeee  
                             M::::::M     MMMMM     M::::::Me:::::::e         a::::a    a:::::assssss   s:::::s u:::::uuuu:::::u  r:::::r            e:::::::e           
                             M::::::M               M::::::Me::::::::e        a::::a    a:::::as:::::ssss::::::su:::::::::::::::uur:::::r            e::::::::e          
                             M::::::M               M::::::M e::::::::eeeeeeeea:::::aaaa::::::as::::::::::::::s  u:::::::::::::::ur:::::r             e::::::::eeeeeeee  
                             M::::::M               M::::::M  ee:::::::::::::e a::::::::::aa:::as:::::::::::ss    uu::::::::uu:::ur:::::r              ee:::::::::::::e  
                             MMMMMMMM               MMMMMMMM    eeeeeeeeeeeeee  aaaaaaaaaa  aaaa sssssssssss        uuuuuuuu  uuuurrrrrrr                eeeeeeeeeeeeee  

         
         
         
         
         
         
         
         
         
         */

        public static int LastIndex(this string s)
        {
            return (s.Length - 1);
        }

        /// <summary>
        /// The width in points.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="font">The font.</param>
        /// <returns>The <see cref="float"/>.</returns>
        public static float WidthInPoints(this string s, Font font)
        {
            // https://msdn.microsoft.com/en-us/library/y4xdbe66.aspx

            // The Size, in pixels, of text drawn on a single line with the
            // specified font. You can manipulate how the text is drawn by using
            // one of the DrawText overloads that takes a TextFormatFlags
            // parameter. For example, the default behavior of the TextRenderer
            // is to add padding to the bounding rectangle of the drawn text to
            // accommodate overhanging glyphs. If you need to draw a line of
            // text without these extra spaces you should use the versions of
            // DrawText and MeasureText that take a Size and TextFormatFlags
            // parameter. For an example, see
            // MeasureText(IDeviceContext, String, Font, Size, TextFormatFlags).
            // SelectObject(hDC, yourFont); // The device context and your font,
            // respectively GetTextExtentPoint32
            Size size = System.Windows.Forms.TextRenderer.MeasureText(s, font);
            //int ascent = font.FontFamily.GetCellAscent(FontStyle.Regular);
            //int descent = font.FontFamily.GetCellDescent(FontStyle.Regular);
            //int height = font.FontFamily.GetEmHeight(FontStyle.Regular);
            //int lineSpacing = font.FontFamily.GetLineSpacing(FontStyle.Regular);
            //float pointSize = font.SizeInPoints;
            return size.Width;
        }

        /// <summary>
        /// Calculates the SHA1 hash of the supplied string and returns a base 64 string.
        /// </summary>
        /// <param name="stringToHash">String that must be hashed.</param>
        /// <returns>The hashed string or null if hashing failed.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToHash or key is null or empty.</exception>
        public static string GetSHA1Hash(this string stringToHash)
        {
            if (string.IsNullOrEmpty(stringToHash)) return null;
            //{
            //    throw new ArgumentException("An empty string value cannot be hashed.");
            //}

            Byte[] data = Encoding.UTF8.GetBytes(stringToHash);
            Byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(data);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int LevenshteinDistanceFrom(this string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        private static int HowManyMatchingCharsFromStart(string inputleft, string inputright)
        {
            inputleft = (inputleft ?? string.Empty).ToLower();
            inputright = (inputright ?? string.Empty).ToLower();
            int matching = 0;
            for (int i = 0; i < Math.Min(inputleft.Length, inputright.Length); i++)
            {
                if (!char.Equals(inputleft[i], inputright[i]))
                    break;

                matching++;
            }
            return matching;
        }

        public static int ContainsHowMany(this string input, string what)
        {
            return (input.Length - input.Replace(what, "").Length) / what.Length;
        }


        /*







                               RRRRRRRRRRRRRRRRR                                           lllllll                                                          
                               R::::::::::::::::R                                          l:::::l                                                          
                               R::::::RRRRRR:::::R                                         l:::::l                                                          
                               RR:::::R     R:::::R                                        l:::::l                                                          
                                 R::::R     R:::::R    eeeeeeeeeeee    ppppp   ppppppppp    l::::l   aaaaaaaaaaaaa      cccccccccccccccc    eeeeeeeeeeee    
                                 R::::R     R:::::R  ee::::::::::::ee  p::::ppp:::::::::p   l::::l   a::::::::::::a   cc:::::::::::::::c  ee::::::::::::ee  
                                 R::::RRRRRR:::::R  e::::::eeeee:::::eep:::::::::::::::::p  l::::l   aaaaaaaaa:::::a c:::::::::::::::::c e::::::eeeee:::::ee
                                 R:::::::::::::RR  e::::::e     e:::::epp::::::ppppp::::::p l::::l            a::::ac:::::::cccccc:::::ce::::::e     e:::::e
                                 R::::RRRRRR:::::R e:::::::eeeee::::::e p:::::p     p:::::p l::::l     aaaaaaa:::::ac::::::c     ccccccce:::::::eeeee::::::e
                                 R::::R     R:::::Re:::::::::::::::::e  p:::::p     p:::::p l::::l   aa::::::::::::ac:::::c             e:::::::::::::::::e 
                                 R::::R     R:::::Re::::::eeeeeeeeeee   p:::::p     p:::::p l::::l  a::::aaaa::::::ac:::::c             e::::::eeeeeeeeeee  
                                 R::::R     R:::::Re:::::::e            p:::::p    p::::::p l::::l a::::a    a:::::ac::::::c     ccccccce:::::::e           
                               RR:::::R     R:::::Re::::::::e           p:::::ppppp:::::::pl::::::la::::a    a:::::ac:::::::cccccc:::::ce::::::::e          
                               R::::::R     R:::::R e::::::::eeeeeeee   p::::::::::::::::p l::::::la:::::aaaa::::::a c:::::::::::::::::c e::::::::eeeeeeee  
                               R::::::R     R:::::R  ee:::::::::::::e   p::::::::::::::pp  l::::::l a::::::::::aa:::a cc:::::::::::::::c  ee:::::::::::::e  
                               RRRRRRRR     RRRRRRR    eeeeeeeeeeeeee   p::::::pppppppp    llllllll  aaaaaaaaaa  aaaa   cccccccccccccccc    eeeeeeeeeeeeee  
                                                                        p:::::p                                                                             
                                                                        p:::::p                                                                             
                                                                       p:::::::p                                                                            
                                                                       p:::::::p                                                                            
                                                                       p:::::::p                                                                            
                                                                       ppppppppp                                                                            







*/
                /// <summary>
                /// Run multiple replaces in one.
                /// </summary>
                /// <param name="input"></param>
                /// <param name="pairs">Number of strings has to be divisible by 2 or will error.</param>
                /// <returns></returns>

                public static string Replace(this string input, params string[] pairs)
                {
                    string newval = input;
                    for (int i = 0; i < pairs.Length; i+= 2)
                    {
                        newval = newval.Replace(pairs[i], pairs[i + 1]);
                    }
                    return newval;
                }
                /// <summary>
                /// Keep replacing until all items are replaced.  So "x       y" => "x y"
                /// </summary>
                /// <param name="input"></param>
                /// <param name="pairs">Number of strings has to be divisible by 2 or will error.</param>
                /// <returns></returns>
                public static string ReplaceRecurse(this string input, string[] pairs)
                {
                    string newval = input;
                    for (int i = 0; i < pairs.Length; i += 2)
                    {
                        string midval = newval;

                        do 
                        {
                            midval = newval.Replace(pairs[i], pairs[i + 1]);
                        }
                        while (midval != newval);

                    }
                    return newval;
                }

                /// <summary>
                /// Totally nuts:  If replace a with b, then c with d, you can get new replacements.  There is a case of infinite loop.
                /// </summary>
                /// <param name="input"></param>
                /// <param name="pairs">Number of strings has to be divisible by 2 or will error.</param>
                /// <returns></returns>
                public static string ReplaceRecurseAllAgain(this string input, string[] pairs)
                {
                    string newval = input;
                    List<string> alreadyreachedbefore = new List<string>(10);
                    bool ReplacementHappened = false;

                    do // Check them all over again.
                    {
                        ReplacementHappened = false;

                        for (int i = 0; i < pairs.Length; i += 2)
                        {
                            string midval = newval;

                            do
                            {
                                midval = newval.Replace(pairs[i], pairs[i + 1]);
                                if (midval != newval)
                                {
                                    ReplacementHappened = true;
                                }

                                newval = midval;
                            }
                            while (midval != newval);  // Something changed, try it again.
                        }
                        if (alreadyreachedbefore.Contains(newval))
                        {
                            return newval; // Break if detect infinite loop.
                        }
                        alreadyreachedbefore.Add(newval);

                    }
                    while (ReplacementHappened); // At least one replacement in the entire array of replacements did something.

                    return newval;
                }

                /// <summary>
                /// Use a list of patterns and replacements.
                /// </summary>
                /// <param name="input"></param>
                /// <param name="pairs">Number of strings has to be divisible by 2 or will error.</param>
                /// <returns></returns>
                public static string ReplaceRegex(this string input, params string[] pairs)
                {
                    string newval = input;
                    for (int i = 0; i < pairs.Length; i += 2)
                    {
                        newval = Regex.Replace(newval, pairs[i], pairs[i + 1]);
                    }
                    return newval;
                }

                /// <summary>
                /// Replace all values in string
                /// </summary>
                /// <param name="value">The input string.</param>
                /// <param name="oldValues">List of old values, which must be replaced</param>
                /// <param name="replacePredicate">Function for replacement old values</param>
                /// <returns>Returns new string with the replaced values</returns>
                /// <example>
                /// 	<code>
                ///         var str = "White Red Blue Green Yellow Black Gray";
                ///         var achromaticColors = new[] {"White", "Black", "Gray"};
                ///         str = str.ReplaceAll(achromaticColors, v => "[" + v + "]");
                ///         // str == "[White] Red Blue Green Yellow [Black] [Gray]"
                /// 	</code>
                /// </example>
                /// <remarks>
                /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
                /// </remarks>
                public static string ReplaceAll(this string value, IEnumerable<string> oldValues, Func<string, string> replacePredicate)
                {
                    var sbStr = new StringBuilder(value);
                    foreach (var oldValue in oldValues)
                    {
                        var newValue = replacePredicate(oldValue);
                        sbStr.Replace(oldValue, newValue);
                    }

                    return sbStr.ToString();
                }

                /// <summary>
                /// Replace all values in string
                /// </summary>
                /// <param name="value">The input string.</param>
                /// <param name="oldValues">List of old values, which must be replaced</param>
                /// <param name="newValue">New value for all old values</param>
                /// <returns>Returns new string with the replaced values</returns>
                /// <example>
                /// 	<code>
                ///         var str = "White Red Blue Green Yellow Black Gray";
                ///         var achromaticColors = new[] {"White", "Black", "Gray"};
                ///         str = str.ReplaceAll(achromaticColors, "[AchromaticColor]");
                ///         // str == "[AchromaticColor] Red Blue Green Yellow [AchromaticColor] [AchromaticColor]"
                /// 	</code>
                /// </example>
                /// <remarks>
                /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
                /// </remarks>
                public static string ReplaceAll(this string value, IEnumerable<string> oldValues, string newValue)
                {
                    var sbStr = new StringBuilder(value);
                    foreach (var oldValue in oldValues)
                        sbStr.Replace(oldValue, newValue);

                    return sbStr.ToString();
                }

                /// <summary>
                /// Replace all values in string
                /// </summary>
                /// <param name="value">The input string.</param>
                /// <param name="oldValues">List of old values, which must be replaced</param>
                /// <param name="newValues">List of new values</param>
                /// <returns>Returns new string with the replaced values</returns>
                /// <example>
                /// 	<code>
                ///         var str = "White Red Blue Green Yellow Black Gray";
                ///         var achromaticColors = new[] {"White", "Black", "Gray"};
                ///         var exquisiteColors = new[] {"FloralWhite", "Bistre", "DavyGrey"};
                ///         str = str.ReplaceAll(achromaticColors, exquisiteColors);
                ///         // str == "FloralWhite Red Blue Green Yellow Bistre DavyGrey"
                /// 	</code>
                /// </example>
                /// <remarks>
                /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
                /// </remarks> 
                public static string ReplaceAll(this string value, IEnumerable<string> oldValues, IEnumerable<string> newValues)
                {
                    var sbStr = new StringBuilder(value);
                    var newValueEnum = newValues.GetEnumerator();
                    foreach (var old in oldValues)
                    {
                        if (!newValueEnum.MoveNext())
                            throw new ArgumentOutOfRangeException("newValues", "newValues sequence is shorter than oldValues sequence");
                        sbStr.Replace(old, newValueEnum.Current);
                    }
                    if (newValueEnum.MoveNext())
                        throw new ArgumentOutOfRangeException("newValues", "newValues sequence is longer than oldValues sequence");

                    return sbStr.ToString();
                }

                /// <summary>
                /// The replace with no white space collapse. Meaning, if there ls "Joe.Dirt. is stupid", replacing all '.' will make it "Joe Dirt is stupid".  Very common-sense.
                /// </summary>
                /// <param name="s">The s.</param>
                /// <param name="replacethischar">The replacethischar.</param>
                /// <returns>The <see cref="string"/>.</returns>
                public static string ReplaceWithNoWhiteSpaceCollapse(this string s, char replacethischar)
                {
                    // So, with a string like "X.R. 333", we want to avoid collapsing the "." to a null, but wa also don't want to introduce multiple spaces
                    return s.Replace(replacethischar, ' ').Replace("  ", " ");
                }

                public static string Restore(this string input, string newval, string matchval)
                {
                    return input.Replace(matchval, newval);
                }

                /// <summary>
                /// Restore is a Replace Reversal.  So you can pass in the same list of values, left to right, but it puts the newvalue where the oldvalue, 
                /// </summary>
                /// <param name="input"></param>
                /// <param name="pairs">Number of strings has to be divisible by 2 or will error.</param>
                /// <returns></returns>
                public static string Restore(this string input, params string[] pairs)
                {
                    string newval = input;
                    for (int i = pairs.Length - 1; i >= 0; i -= 2)
                    {
                        newval = newval.Replace(pairs[i], pairs[i - 1]);
                    }
                    return newval;
                }

        /// <summary>
        /// 	Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "replaceValue">The replacement value.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue)
        {
            return ReplaceWith(value, regexPattern, replaceValue, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "replaceValue">The replacement value.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, string replaceValue, RegexOptions options)
        {
            return Regex.Replace(value, regexPattern, replaceValue, options);
        }

        /// <summary>
        /// 	Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "evaluator">The replacement method / lambda expression.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, MatchEvaluator evaluator)
        {
            return ReplaceWith(value, regexPattern, RegexOptions.None, evaluator);
        }

        /// <summary>
        /// 	Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <param name = "evaluator">The replacement method / lambda expression.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// 	</code>
        /// </example>
        public static string ReplaceWith(this string value, string regexPattern, RegexOptions options, MatchEvaluator evaluator)
        {
            return Regex.Replace(value, regexPattern, evaluator, options);
        }



        /*
                                                                                                                           





                                                                                                                                                   dddddddd
                                EEEEEEEEEEEEEEEEEEEEEE                             tttt                                                            d::::::d
                                E::::::::::::::::::::E                          ttt:::t                                                            d::::::d
                                E::::::::::::::::::::E                          t:::::t                                                            d::::::d
                                EE::::::EEEEEEEEE::::E                          t:::::t                                                            d:::::d 
                                  E:::::E       EEEEEExxxxxxx      xxxxxxxttttttt:::::ttttttt        eeeeeeeeeeee    nnnn  nnnnnnnn        ddddddddd:::::d 
                                  E:::::E              x:::::x    x:::::x t:::::::::::::::::t      ee::::::::::::ee  n:::nn::::::::nn    dd::::::::::::::d 
                                  E::::::EEEEEEEEEE     x:::::x  x:::::x  t:::::::::::::::::t     e::::::eeeee:::::een::::::::::::::nn  d::::::::::::::::d 
                                  E:::::::::::::::E      x:::::xx:::::x   tttttt:::::::tttttt    e::::::e     e:::::enn:::::::::::::::nd:::::::ddddd:::::d 
                                  E:::::::::::::::E       x::::::::::x          t:::::t          e:::::::eeeee::::::e  n:::::nnnn:::::nd::::::d    d:::::d 
                                  E::::::EEEEEEEEEE        x::::::::x           t:::::t          e:::::::::::::::::e   n::::n    n::::nd:::::d     d:::::d 
                                  E:::::E                  x::::::::x           t:::::t          e::::::eeeeeeeeeee    n::::n    n::::nd:::::d     d:::::d 
                                  E:::::E       EEEEEE    x::::::::::x          t:::::t    tttttte:::::::e             n::::n    n::::nd:::::d     d:::::d 
                                EE::::::EEEEEEEE:::::E   x:::::xx:::::x         t::::::tttt:::::te::::::::e            n::::n    n::::nd::::::ddddd::::::dd
                                E::::::::::::::::::::E  x:::::x  x:::::x        tt::::::::::::::t e::::::::eeeeeeee    n::::n    n::::n d:::::::::::::::::d
                                E::::::::::::::::::::E x:::::x    x:::::x         tt:::::::::::tt  ee:::::::::::::e    n::::n    n::::n  d:::::::::ddd::::d
                                EEEEEEEEEEEEEEEEEEEEEExxxxxxx      xxxxxxx          ttttttttttt      eeeeeeeeeeeeee    nnnnnn    nnnnnn   ddddddddd   ddddd
                                                                                                                           
                                                                                                                           
                                                                                                                           
                                                                                                                           
                                                                                                                           
                                                                                                                           
                                                                                                                           
*/
        public static string AppendWithSeparator(this string input, string newfield, string sep = ", ")
        {
            if (newfield == null || newfield.IsVoid())
            {
                return input;
            }
            // We allow adding blanks, though.
            if (input.Contains(newfield))
            {
                return input;
            }
            // No sense adding the same value
            if (input.IsNotVoid()) input = input + sep + newfield;
            else input = newfield;
            return input;
        }

        public static string InsertBefore(this string target, string beforeThisString, string whatToInsert)
        {
            int posofstr = target.IndexOf(beforeThisString);
            if (posofstr == -1) throw new Exception("InsertAfter: Did not find string to insert before.");
            return target.Insert(posofstr, whatToInsert);
        }

        public static string Prepend(this string s, string inputstring, bool pushaspaceifstringpop = true)
        {
            if (s.IsVoid()) return inputstring;
            if (inputstring.IsVoid()) return s;
            return s.Insert(0, inputstring + (pushaspaceifstringpop ? " " : ""));
        }

        /// <summary>
        /// 	Concatenates the specified string value with the passed additional strings.
        /// </summary>
        /// <param name = "value">The original value.</param>
        /// <param name = "values">The additional string values to be concatenated.</param>
        /// <returns>The concatenated string.</returns>
        public static string ConcatWith(this string value, params string[] values)
        {
            return string.Concat(value, string.Concat(values));
        }

        /// <summary>
        /// Joins  the values of a string array if the values are not null or empty.
        /// </summary>
        /// <param name="objs">The string array used for joining.</param>
        /// <param name="separator">The separator to use in the joined string.</param>
        /// <returns></returns>
        public static string JoinNotNullOrEmpty(this string[] objs, string separator)
        {
            var items = new List<string>();
            foreach (string s in objs)
            {
                if (!String.IsNullOrEmpty(s))
                    items.Add(s);
            }
            return String.Join(separator, items.ToArray());
        }

        /// <summary>
        /// 	A generic version of System.String.Join()
        /// </summary>
        /// <typeparam name = "T">
        /// 	The type of the array to join
        /// </typeparam>
        /// <param name = "separator">
        /// 	The separator to appear between each element
        /// </param>
        /// <param name = "value">
        /// 	An array of values
        /// </param>
        /// <returns>
        /// 	The join.
        /// </returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Join<T>(string separator, T[] value)
        {
            if (value == null || value.Length == 0)
                return string.Empty;
            if (separator == null)
                separator = string.Empty;
            Converter<T, string> converter = o => o.ToString();
            return string.Join(separator, Array.ConvertAll(value, converter));
        }


        /*







                            RRRRRRRRRRRRRRRRR                                                                                                        
                            R::::::::::::::::R                                                                                                       
                            R::::::RRRRRR:::::R                                                                                                      
                            RR:::::R     R:::::R                                                                                                     
                              R::::R     R:::::R    eeeeeeeeeeee       mmmmmmm    mmmmmmm      ooooooooooo vvvvvvv           vvvvvvv eeeeeeeeeeee    
                              R::::R     R:::::R  ee::::::::::::ee   mm:::::::m  m:::::::mm  oo:::::::::::oov:::::v         v:::::vee::::::::::::ee  
                              R::::RRRRRR:::::R  e::::::eeeee:::::eem::::::::::mm::::::::::mo:::::::::::::::ov:::::v       v:::::ve::::::eeeee:::::ee
                              R:::::::::::::RR  e::::::e     e:::::em::::::::::::::::::::::mo:::::ooooo:::::o v:::::v     v:::::ve::::::e     e:::::e
                              R::::RRRRRR:::::R e:::::::eeeee::::::em:::::mmm::::::mmm:::::mo::::o     o::::o  v:::::v   v:::::v e:::::::eeeee::::::e
                              R::::R     R:::::Re:::::::::::::::::e m::::m   m::::m   m::::mo::::o     o::::o   v:::::v v:::::v  e:::::::::::::::::e 
                              R::::R     R:::::Re::::::eeeeeeeeeee  m::::m   m::::m   m::::mo::::o     o::::o    v:::::v:::::v   e::::::eeeeeeeeeee  
                              R::::R     R:::::Re:::::::e           m::::m   m::::m   m::::mo::::o     o::::o     v:::::::::v    e:::::::e           
                            RR:::::R     R:::::Re::::::::e          m::::m   m::::m   m::::mo:::::ooooo:::::o      v:::::::v     e::::::::e          
                            R::::::R     R:::::R e::::::::eeeeeeee  m::::m   m::::m   m::::mo:::::::::::::::o       v:::::v       e::::::::eeeeeeee  
                            R::::::R     R:::::R  ee:::::::::::::e  m::::m   m::::m   m::::m oo:::::::::::oo         v:::v         ee:::::::::::::e  
                            RRRRRRRR     RRRRRRR    eeeeeeeeeeeeee  mmmmmm   mmmmmm   mmmmmm   ooooooooooo            vvv            eeeeeeeeeeeeee  







        */
        /// <summary>
        /// Strip out any occurrences of a list of characters
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="c">The c.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string Strip(this string s, params char[] cs)
        {
            foreach (var c in cs)
                s = s.Replace(c.ToString(), "");
            return s;
        }

        /// <summary>
        /// The strip last word.  Helpful for moving up a folder hierarchy.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="wordsepchars">The chars.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string StripLastWord(this string s, params char[] wordsepchars)
        {
            if (s.IsVoid()) return s;
            var lastword = s.LastWord(wordsepchars);
            if (lastword.Length == 0) return s;
            return s.TrimRightN(lastword.Length + 1); // Make sure you trim off what ever character was found.
        }

        /// <summary>
        /// The strip if starts with.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="c">The c.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string StripIfStartsWith(this string s, char c)
        {
            if (s.IsNotVoid() && s.Length > 0 && Char.ToLower(s[0]) == Char.ToLower(c))
                return s.Substring(1);
            else
                return s;
        }

        /// <summary>
        /// The strip if starts with.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="rs">The rs.</param>
        /// <param name="throwExceptionIfNotStart">The throwExceptionIfNotStart.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string StripIfStartsWith(this string s, string rs, Exception throwExceptionIfNotStart = null)
        {
            if (s.IsNotVoid() && s.Length >= rs.Length && s.StartsWith(rs, StringComparison.CurrentCultureIgnoreCase))
                return s.Substring(rs.Length);
            else
            {
                if (throwExceptionIfNotStart != null)
                    throw throwExceptionIfNotStart;
                return s;
            }
        }

        public static string StripIfEndsWith(this string s, char c)
        {
            if (s.IsNotVoid() && s.Length > 0 && Char.ToLower(s[s.LastIndex()]) == Char.ToLower(c))
                return s.Substring(1);
            else
                return s;
        }

        public static string StripIfEndsWith(this string s, string rs, Exception throwExceptionIfNotEnd = null)
        {
            if (s.IsNotVoid() && s.Length >= rs.Length && s.EndsWith(rs, StringComparison.CurrentCultureIgnoreCase))
                return s.Substring(0, s.Length - rs.Length);
            else
            {
                if (throwExceptionIfNotEnd != null)
                    throw throwExceptionIfNotEnd;
                return s;
            }
        }

        /// <summary>
        /// Simple helper function cause i hate converting characters to strings. "Remove" runs into an int!!!
        /// </summary>
        /// <param name="input"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RemoveChar(this string input, char c)
        {
            if (input == null) return null;
            return input.Replace(c.ToString(), "");
        }

        /// <summary>
        /// Return the string with the leftmost number_of_characters characters removed.
        /// </summary>
        /// <param name="str">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String RemoveLeft(this String str, int number_of_characters)
        {
            if (str.Length <= number_of_characters) return "";
            return str.Substring(number_of_characters);
        }

        /// <summary>
        /// Return the string with the rightmost number_of_characters characters removed.
        /// </summary>
        /// <param name="str">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String RemoveRight(this String str, int number_of_characters)
        {
            if (str.Length <= number_of_characters) return "";
            return str.Substring(0, str.Length - number_of_characters);
        }

        /// <summary>
        /// 	Remove any instance of the given character from the current string.
        /// </summary>
        /// <param name = "value">
        /// 	The input.
        /// </param>
        /// <param name = "removeCharc">
        /// 	The remove char.
        /// </param>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Remove(this string value, params char[] removeCharc)
        {
            var result = value;
            if (!string.IsNullOrEmpty(result) && removeCharc != null)
                Array.ForEach(removeCharc, c => result = result.Remove(c.ToString()));

            return result;

        }

        /// <summary>
        /// Remove any instance of the given string pattern from the current string.
        /// </summary>
        /// <param name="value">The input.</param>
        /// <param name="strings">The strings.</param>
        /// <returns></returns>
        /// <remarks>
        /// Contributed by Michael T, http://about.me/MichaelTran
        /// </remarks>
        public static string Remove(this string value, params string[] strings)
        {
            return strings.Aggregate(value, (current, c) => current.Replace(c, string.Empty));
        }
        /// <summary>
        /// Removed all special characters from the string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The adjusted string.</returns>
        /// <remarks>
        /// 	Contributed by Michael T, http://about.me/MichaelTran, James C, http://www.noveltheory.com
        /// 	This implementation is roughly equal to the original in speed, and should be more robust, internationally.
        /// </remarks>

        public static string RemoveAllSpecialCharacters(this string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (var c in value.Where(c => Char.IsLetterOrDigit(c)))
                sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        /// Removed all special characters from the string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>The adjusted string.</returns>
        [Obsolete("Please use RemoveAllSpecialCharacters instead")]
        public static string AdjustInput(this string value)
        {
            return string.Join(null, Regex.Split(value, "[^a-zA-Z0-9]"));
        }

        public static string TrimC(this string @string, string string2Trim)
        {
            if (@string == null || string2Trim == null)
            {
                return @string;
            }

            if (@string.Right(string2Trim.Length) == string2Trim)
            {
                return @string.Left(@string.Length - string2Trim.Length);
            }

            return @string;
        }

        public static string TrimC(this string input, char? character2Trim)
        {
            if (input == null || character2Trim == null)
            {
                return input;
            }

            if (input.RightC() == character2Trim)
            {
                return input.Left(input.Length - 1);
            }

            return input;
        }

        public static string TrimLeftN(this string input, int nofChar2Remove)
        {
            Assert.True(nofChar2Remove >= 0);

            if (input == null)
                return null;

            if (input.IsVoid() || input.Length <= nofChar2Remove)
                return string.Empty;

            return input.Substring(nofChar2Remove);
        }

        public static string TrimTrailingSpace(this string input)
        {
            return Strings.RTrim(input);
        }

        /// <summary>
        /// <summary>
        /// 	Trims the text to a provided maximum length.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "maxLength">Maximum length.</param>
        /// <returns></returns>
        /// <remarks>
        /// 	Proposed by Rene Schulte
        /// </remarks>
        public static string TrimToMaxLength(this string value, int maxLength)
        {
            return (value == null || value.Length <= maxLength ? value : value.Substring(0, maxLength));
        }

        /// <summary>
        /// 	Trims the text to a provided maximum length and adds a suffix if required.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "maxLength">Maximum length.</param>
        /// <param name = "suffix">The suffix.</param>
        /// <returns></returns>
        /// <remarks>
        /// 	Proposed by Rene Schulte
        /// </remarks>
        public static string TrimToMaxLength(this string value, int maxLength, string suffix)
        {
            return (value == null || value.Length <= maxLength ? value : string.Concat(value.Substring(0, maxLength), suffix));
        }

        /// <summary>
        /// Truncates a string with optional Elipses added
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <param name="useElipses"></param>
        /// <returns></returns>
        public static string Truncate(this string input, int length, bool useElipses = false)
        {
            if (length==0) return string.Empty;
            int e = useElipses ? 3 : 0;
            if (length - e <= 0) throw new InvalidOperationException(string.Format("Length must be greater than {0}.", e));

            if (string.IsNullOrEmpty(input) || input.Length <= length) return input;

            return input.Substring(0, length - e) + new String('.', e);
        }

        public static string TrimSentence(this string s)
        {
            if (s == null)
            {
                return s;
            }

            return s.Trim().ReplaceWith("\\s+", " ");
        }

        public static string TrimRightN(this string input, int nofChar2Remove)
        {
            Assert.True(nofChar2Remove >= 0);

            if (input == null)
                return null;

            if (input.IsVoid() || input.Length <= nofChar2Remove)
                return string.Empty;

            return input.Substring(0, input.Length - nofChar2Remove);
        }
        /*










                                            EEEEEEEEEEEEEEEEEEEEEE                             tttt                                                                         tttt          
                                            E::::::::::::::::::::E                          ttt:::t                                                                      ttt:::t          
                                            E::::::::::::::::::::E                          t:::::t                                                                      t:::::t          
                                            EE::::::EEEEEEEEE::::E                          t:::::t                                                                      t:::::t          
                                              E:::::E       EEEEEExxxxxxx      xxxxxxxttttttt:::::ttttttt   rrrrr   rrrrrrrrr   aaaaaaaaaaaaa      ccccccccccccccccttttttt:::::ttttttt    
                                              E:::::E              x:::::x    x:::::x t:::::::::::::::::t   r::::rrr:::::::::r  a::::::::::::a   cc:::::::::::::::ct:::::::::::::::::t    
                                              E::::::EEEEEEEEEE     x:::::x  x:::::x  t:::::::::::::::::t   r:::::::::::::::::r aaaaaaaaa:::::a c:::::::::::::::::ct:::::::::::::::::t    
                                              E:::::::::::::::E      x:::::xx:::::x   tttttt:::::::tttttt   rr::::::rrrrr::::::r         a::::ac:::::::cccccc:::::ctttttt:::::::tttttt    
                                              E:::::::::::::::E       x::::::::::x          t:::::t          r:::::r     r:::::r  aaaaaaa:::::ac::::::c     ccccccc      t:::::t          
                                              E::::::EEEEEEEEEE        x::::::::x           t:::::t          r:::::r     rrrrrrraa::::::::::::ac:::::c                   t:::::t          
                                              E:::::E                  x::::::::x           t:::::t          r:::::r           a::::aaaa::::::ac:::::c                   t:::::t          
                                              E:::::E       EEEEEE    x::::::::::x          t:::::t    ttttttr:::::r          a::::a    a:::::ac::::::c     ccccccc      t:::::t    tttttt
                                            EE::::::EEEEEEEE:::::E   x:::::xx:::::x         t::::::tttt:::::tr:::::r          a::::a    a:::::ac:::::::cccccc:::::c      t::::::tttt:::::t
                                            E::::::::::::::::::::E  x:::::x  x:::::x        tt::::::::::::::tr:::::r          a:::::aaaa::::::a c:::::::::::::::::c      tt::::::::::::::t
                                            E::::::::::::::::::::E x:::::x    x:::::x         tt:::::::::::ttr:::::r           a::::::::::aa:::a cc:::::::::::::::c        tt:::::::::::tt
                                            EEEEEEEEEEEEEEEEEEEEEExxxxxxx      xxxxxxx          ttttttttttt  rrrrrrr            aaaaaaaaaa  aaaa   cccccccccccccccc          ttttttttttt  






                */



        public static string Left(this string input, int headLength)
        {
            if (input == null) return "";
            if (headLength >= input.Length)
                return input;
            return input.Substring(0, headLength);
        }

        /// <summary>
        /// Get the leftmost character.  "Left" returns a string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The <see cref="char"/>.</returns>
        public static char? LeftChar(this string s)
        {
            if (s.IsNotVoid() && s.Length > 0)
                return s[0];
            else
                return null;
        }

        /// <summary>
        /// From a SQL background so I like the Right function.
        /// </summary>
        /// <param name="input">The s.</param>
        /// <param name="tailLength">The tailLength.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string Right(this string input, int tailLength)
        {
            if (input == null) return "";
            if (tailLength >= input.Length)
                return input;
            return input.Substring(input.Length - tailLength);
        }

        /// <summary>
        /// Or "LastCharacter"
        /// </summary>
        /// <param name="input">The s.</param>
        /// <returns>The <see cref="char"/>.</returns>
        public static char? RightC(this string input)
        {
            if (input == null) return null;
            switch (input.Length)
            {
                case 0:
                    return null;
                default:
                    return input[input.Length - 1];
            }
        }

        /// <summary>
        /// The right char.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The <see cref="char"/>.</returns>
        public static char? RightChar(this string s)
        {
            if (s.IsNotVoid() && s.Length > 0)
                return s[s.Length - 1];
            else
                return null;
        }

        /// <summary>
        /// The last word, based on a list of given separators.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string LastWord(this string s, params char[] wordsepchars)
        {
            var words = s.Split(wordsepchars);
            if (words.Length > 0)
            {
                return words[words.Length - 1];
            }
            return "";
        }

        /// <summary>
        /// The last word based on a space as a separator.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string LastWord(this string s)
        {
            var words = s.Split(' ');
            if (words.Length > 0)
            {
                return words[words.Length - 1];
            }
            return "";
        }

        public static string GetAfter(this string @this, string value)
        {
            if (@this.IndexOf(value) == -1)
            {
                return "";
            }
            return @this.Substring(@this.IndexOf(value) + value.Length);
        }

        public static string GetBetween(this string @this, string before, string after)
        {
            int beforeStartIndex = @this.IndexOf(before);
            int startIndex = beforeStartIndex + before.Length;
            int afterStartIndex = @this.IndexOf(after, startIndex);

            if (beforeStartIndex == -1 || afterStartIndex == -1)
            {
                return "";
            }

            return @this.Substring(startIndex, afterStartIndex - startIndex);
        }

        public static string GetBefore(this string @this, string value)
        {
            if (@this.IndexOf(value) == -1)
            {
                return "";
            }
            return @this.Substring(0, @this.IndexOf(value));
        }

        /// <summary>
        /// Extract arguments from string by template
        /// </summary>
        /// <param name="value">The input string. For example, "My name is Aleksey".</param>
        /// <param name="template">Template with arguments in the format {№}. For example, "My name is {0} {1}.".</param>
        /// <param name="compareTemplateOptions">Template options for compare with input string.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <returns>Returns parsed arguments.</returns>
        /// <example>
        /// 	<code>
        /// 		var str = "My name is Aleksey Nagovitsyn. I'm from Russia.";
        /// 		var args = str.ExtractArguments(@"My name is {1} {0}. I'm from {2}.");
        ///         // args[i] is [Nagovitsyn, Aleksey, Russia]
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static IEnumerable<string> ExtractArguments(this string value, string template,
                                                           ComparsionTemplateOptions compareTemplateOptions = _defaultComparsionTemplateOptions,
                                                           RegexOptions regexOptions = _defaultRegexOptions)
        {
            return ExtractGroupArguments(value, template, compareTemplateOptions, regexOptions).Select(g => g.Value);
        }

        /// <summary>
        /// Extract arguments as regex groups from string by template
        /// </summary>
        /// <param name="value">The input string. For example, "My name is Aleksey".</param>
        /// <param name="template">Template with arguments in the format {№}. For example, "My name is {0} {1}.".</param>
        /// <param name="compareTemplateOptions">Template options for compare with input string.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <returns>Returns parsed arguments as regex groups.</returns>
        /// <example>
        /// 	<code>
        /// 		var str = "My name is Aleksey Nagovitsyn. I'm from Russia.";
        /// 		var groupArgs = str.ExtractGroupArguments(@"My name is {1} {0}. I'm from {2}.");
        ///         // groupArgs[i].Value is [Nagovitsyn, Aleksey, Russia]
        /// 	</code>
        /// </example>
        /// <remarks>
        /// 	Contributed by nagits, http://about.me/AlekseyNagovitsyn
        /// </remarks>
        public static IEnumerable<Group> ExtractGroupArguments(this string value, string template,
                                                               ComparsionTemplateOptions compareTemplateOptions = _defaultComparsionTemplateOptions,
                                                               RegexOptions regexOptions = _defaultRegexOptions)
        {
            var pattern = GetRegexPattern(template, compareTemplateOptions);
            var regex = new Regex(pattern, regexOptions);
            var match = regex.Match(value);

            return Enumerable.Cast<Group>(match.Groups).Skip(1);
        }

        private static string[] FetchAllWordsBefore(string s, char[] delims, int index)
        {
            StringBuilder builddelimitedword = new StringBuilder(40);
            StringBuilder buildundelimitedword = new StringBuilder(40);
            List<string> priorwords = new List<string>(10);
            for (int i = index - 1; i >= 0; i--)
            {
                char c = s[i];
                builddelimitedword.Insert(0, c);

                if (c.In(delims) || c == ' ')
                {
                    buildundelimitedword.Insert(0, c);

                    if (!priorwords.Contains(builddelimitedword.ToString()))
                    {
                        priorwords.Add(builddelimitedword.ToString());
                    }
                    //if (!priorwords.Contains(buildundelimitedword.ToString()))
                    //{
                    //    priorwords.Add(buildundelimitedword.ToString());
                    //}
                }
            }

            if (!priorwords.Contains(builddelimitedword.ToString()))
            {
                priorwords.Add(builddelimitedword.ToString());
            }
            //if (!priorwords.Contains(buildundelimitedword.ToString()))
            //{
            //    priorwords.Add(buildundelimitedword.ToString());
            //}

            return priorwords.ToArray();
        }

        /// <summary>
        /// 	Extracts all digits from a string.
        /// </summary>
        /// <param name = "value">String containing digits to extract</param>
        /// <returns>
        /// 	All digits contained within the input string
        /// </returns>
        /// <remarks>
        /// 	Contributed by Kenneth Scott
        /// </remarks>

        public static string ExtractDigits(this string value)
        {
            return value.Where(Char.IsDigit).Aggregate(new StringBuilder(value.Length), (sb, c) => sb.Append(c)).ToString();
        }

        /// <summary>Returns the right part of the string from index.</summary>
        /// <param name="value">The original value.</param>
        /// <param name="index">The start index for substringing.</param>
        /// <returns>The right part.</returns>
        public static string SubstringFrom(this string value, int index)
        {
            return index < 0 ? value : value.Substring(index, value.Length - index);
        }

        /// <summary>
        /// 	Uses regular expressions to determine all matches of a given regex pattern.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <returns>A collection of all matches</returns>
        public static MatchCollection GetMatches(this string value, string regexPattern)
        {
            return GetMatches(value, regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to determine all matches of a given regex pattern.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>A collection of all matches</returns>
        public static MatchCollection GetMatches(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.Matches(value, regexPattern, options);
        }

        /// <summary>
        /// 	Uses regular expressions to determine all matches of a given regex pattern and returns them as string enumeration.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <returns>An enumeration of matching strings</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		foreach(var number in s.GetMatchingValues(@"\d")) {
        /// 		Console.WriteLine(number);
        /// 		}
        /// 	</code>
        /// </example>
        public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern)
        {
            return GetMatchingValues(value, regexPattern, RegexOptions.None);
        }

        /// <summary>
        /// 	Uses regular expressions to determine all matches of a given regex pattern and returns them as string enumeration.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>An enumeration of matching strings</returns>
        /// <example>
        /// 	<code>
        /// 		var s = "12345";
        /// 		foreach(var number in s.GetMatchingValues(@"\d")) {
        /// 		Console.WriteLine(number);
        /// 		}
        /// 	</code>
        /// </example>
        public static IEnumerable<string> GetMatchingValues(this string value, string regexPattern, RegexOptions options)
        {
            foreach (Match match in GetMatches(value, regexPattern, options))
            {
                if (match.Success) yield return match.Value;
            }
        }


        /// <summary>
        /// 	Splits the given string into words and returns a string array.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <returns>The splitted string array</returns>
        public static string[] GetWords(this string value)
        {
            return value.Split(@"\W");
        }

        /// <summary>
        /// 	Gets the nth "word" of a given string, where "words" are substrings separated by a given separator
        /// </summary>
        /// <param name = "value">The string from which the word should be retrieved.</param>
        /// <param name = "index">Index of the word (0-based).</param>
        /// <returns>
        /// 	The word at position n of the string.
        /// 	Trying to retrieve a word at a position lower than 0 or at a position where no word exists results in an exception.
        /// </returns>
        /// <remarks>
        /// 	Originally contributed by MMathews
        /// </remarks>
        public static string GetWordByIndex(this string value, int index)
        {
            var words = value.GetWords();

            if ((index < 0) || (index > words.Length - 1))
                throw new IndexOutOfRangeException("The word number is out of range.");

            return words[index];
        }

        private const RegexOptions _defaultRegexOptions = RegexOptions.None;
        private const ComparsionTemplateOptions _defaultComparsionTemplateOptions = ComparsionTemplateOptions.Default;
        private static readonly string[] _reservedRegexOperators = new[] { @"\", "^", "$", "*", "+", "?", ".", "(", ")" };

        private static string GetRegexPattern(string template, ComparsionTemplateOptions compareTemplateOptions)
        {
            template = template.ReplaceAll(_reservedRegexOperators, v => @"\" + v);

            bool comparingFromStart = compareTemplateOptions == ComparsionTemplateOptions.FromStart ||
                                  compareTemplateOptions == ComparsionTemplateOptions.Whole;
            bool comparingAtTheEnd = compareTemplateOptions == ComparsionTemplateOptions.AtTheEnd ||
                                 compareTemplateOptions == ComparsionTemplateOptions.Whole;
            var pattern = new StringBuilder();

            if (comparingFromStart)
                pattern.Append("^");

            pattern.Append(Regex.Replace(template, @"\{[0-9]+\}",
                                            delegate (Match m)
                                            {
                                                var argNum = m.ToString().Replace("{", "").Replace("}", "");
                                                return String.Format("(?<{0}>.*?)", int.Parse(argNum) + 1);
                                            }
                          ));

            if (comparingAtTheEnd || (template.LastOrDefault() == '}' && compareTemplateOptions == ComparsionTemplateOptions.Default))
                pattern.Append("$");

            return pattern.ToString();
        }


/*


                                                                                                                                           
                                                                                                                                           
                               SSSSSSSSSSSSSSS                    lllllll   iiii          tttt                    OOOOOOOOO                              tttt          
                             SS:::::::::::::::S                   l:::::l  i::::i      ttt:::t                  OO:::::::::OO                         ttt:::t          
                            S:::::SSSSSS::::::S                   l:::::l   iiii       t:::::t                OO:::::::::::::OO                       t:::::t          
                            S:::::S     SSSSSSS                   l:::::l              t:::::t               O:::::::OOO:::::::O                      t:::::t          
                            S:::::S           ppppp   ppppppppp    l::::l iiiiiiittttttt:::::ttttttt         O::::::O   O::::::Ouuuuuu    uuuuuuttttttt:::::ttttttt    
                            S:::::S           p::::ppp:::::::::p   l::::l i:::::it:::::::::::::::::t         O:::::O     O:::::Ou::::u    u::::ut:::::::::::::::::t    
                             S::::SSSS        p:::::::::::::::::p  l::::l  i::::it:::::::::::::::::t         O:::::O     O:::::Ou::::u    u::::ut:::::::::::::::::t    
                              SS::::::SSSSS   pp::::::ppppp::::::p l::::l  i::::itttttt:::::::tttttt         O:::::O     O:::::Ou::::u    u::::utttttt:::::::tttttt    
                                SSS::::::::SS  p:::::p     p:::::p l::::l  i::::i      t:::::t               O:::::O     O:::::Ou::::u    u::::u      t:::::t          
                                   SSSSSS::::S p:::::p     p:::::p l::::l  i::::i      t:::::t               O:::::O     O:::::Ou::::u    u::::u      t:::::t          
                                        S:::::Sp:::::p     p:::::p l::::l  i::::i      t:::::t               O:::::O     O:::::Ou::::u    u::::u      t:::::t          
                                        S:::::Sp:::::p    p::::::p l::::l  i::::i      t:::::t    tttttt     O::::::O   O::::::Ou:::::uuuu:::::u      t:::::t    tttttt
                            SSSSSSS     S:::::Sp:::::ppppp:::::::pl::::::li::::::i     t::::::tttt:::::t     O:::::::OOO:::::::Ou:::::::::::::::uu    t::::::tttt:::::t
                            S::::::SSSSSS:::::Sp::::::::::::::::p l::::::li::::::i     tt::::::::::::::t      OO:::::::::::::OO  u:::::::::::::::u    tt::::::::::::::t
                            S:::::::::::::::SS p::::::::::::::pp  l::::::li::::::i       tt:::::::::::tt        OO:::::::::OO     uu::::::::uu:::u      tt:::::::::::tt
                             SSSSSSSSSSSSSSS   p::::::pppppppp    lllllllliiiiiiii         ttttttttttt            OOOOOOOOO         uuuuuuuu  uuuu        ttttttttttt  
                                               p:::::p                                                                                                                 
                                               p:::::p                                                                                                                 
                                              p:::::::p                                                                                                                
                                              p:::::::p                                                                                                                
                                              p:::::::p                                                                                                                
                                              ppppppppp                                                                                                                
                                                                                                                                                                       


*/

        public static IEnumerable<SplitLineItem> SplitAndKeep(this string s, char[] delims)
        {
            int start = 0, index;
            bool isfirst = true;
            int pos = 0;
            string word = "", separator = "";
            while ((index = s.IndexOfAny(delims, start)) != -1)
            {
                if (index - start > 0)
                {
                    word = s.Substring(start, index - start);
                }
                else
                {
                    word = "";
                }

                separator = s.Substring(index, 1);

                yield return new SplitLineItem() { word = word, separator = separator, isfirst = isfirst, islast = false, pos = pos, prevnestedphrases = FetchAllWordsBefore(s, delims, index) };
                isfirst = false;
                pos++;

                start = index + 1;
            }

            if (start < s.Length)
            {
                yield return new SplitLineItem() { word = s.Substring(start), separator = "", isfirst = isfirst, islast = true, pos = pos, prevnestedphrases = FetchAllWordsBefore(s, delims, s.Length) };
            }
        }

        /// <summary>
        /// Invention: I need to wrap on a console monitor.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charsperline"></param>
        /// <returns></returns>
        public static string[] Split(this string input, int charsperline)
        {
            SortedList<int, string>lines = new SortedList<int, string>(19);
            int lineno = 1;
            while (input.Length > 0)
            {
                string line = input.Substring(0, charsperline.Least(input.Length));
                input = input.Substring(line.Length);
                lines.Add(lineno, line);
                lineno++;
            }
            return lines.Sort().Values.ToArray<string>();
        }
        /// <summary>
        /// Uses regular expressions to split a string into parts.
        /// </summary>
        /// <param name = "value">The input string.</param>
        /// <param name = "regexPattern">The regular expression pattern.</param>
        /// <param name = "options">The regular expression options.</param>
        /// <returns>The splitted string array</returns>
        public static string[] Split(this string input, string sep)
        {
            string[] seps = { sep };
            if (input == null)
            {
                return string.Empty.Split(",").ToArray();
            }

            return input.Split(seps, StringSplitOptions.None);
        }

        public static string[] Split(this string value, string regexPattern, RegexOptions options)
        {
            return Regex.Split(value, regexPattern, options);
        }
        /// <summary>
        /// Use the password to generate key bytes and an initialization vector with Rfc2898DeriveBytes.
        /// </summary>
        /// <param name="password">The input password to use in generating the bytes.</param>
        /// <param name="salt">The input salt bytes to use in generating the bytes.</param>
        /// <param name="key_size_bits">The input size of the key to generate.</param>
        /// <param name="block_size_bits">The input block size used by the crypto provider.</param>
        /// <param name="key">The output key bytes to generate.</param>
        /// <param name="iv">The output initialization vector to generate.</param>
        /// <remarks></remarks>
        private static void MakeKeyAndIV(String password, byte[] salt, int key_size_bits, int block_size_bits,
                                         ref byte[] key, ref byte[] iv)
        {
            var derive_bytes =
            new Rfc2898DeriveBytes(password, salt, 1234);

            key = derive_bytes.GetBytes(key_size_bits / 8);
            iv = derive_bytes.GetBytes(block_size_bits / 8);
        }

        public static string Repeat(this string input, int times)
        {
            return string.Concat(Enumerable.Repeat(input, times));
        }
        
        public static string Repeat(this char repeatingchar, int times)
        {
            return new string(repeatingchar, times);
        }

        /// <summary>
        /// The reverse.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string Reverse(this string s)
        {
            if (s == null)
            {
                return null;
            }

            char[] charArray = s.ToCharArray();
            int len = s.Length - 1;

            for (int i = 0; i < len; i++, len--)
            {
                charArray[i] ^= charArray[len];
                charArray[len] ^= charArray[i];
                charArray[i] ^= charArray[len];
            }

            return new string(charArray);
        }

        private static StringBuilder titledText = new StringBuilder(120);

        public class SplitLineItem
        {
            public string word { get; set; }
            public string originalword { get; set; }
            public string separator;
            public bool isfirst;
            public bool islast;
            public int pos;
            public bool changedCase;
            public bool dropfromoutput;
            public string[] prevnestedphrases;
        }

        public class AcronymDef
        {
            public string key;
            public string purpose;
            public string acronymInTitleCase;
            public string expansion;
            public bool forceExpansion;
        }

    }

}
