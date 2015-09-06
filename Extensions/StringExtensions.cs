using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Kogler.Framework
{
    public static class StringExtensions
    {
        #region << Public >>
        
        public static Color ToColor(this string color)
        {
            if (color[0] == '#') color = color.Substring(1);
            if (color.Length != 6 && color.Length != 8) throw new ArgumentOutOfRangeException("color");
            int start = 0;
            byte a;
            if (color.Length == 8)
            {
                a = byte.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                start = 2;
            }
            else a = 255;
            byte r = byte.Parse(color.Substring(start, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(color.Substring(start + 2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(color.Substring(start + 4, 2), NumberStyles.HexNumber);
            return new Color {A = a, R = r, B = b, G = g};
        }
        public static string CapitalizeWords(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            StringBuilder result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
                result[i] = char.IsWhiteSpace(result[i - 1]) ? char.ToUpper(result[i]) : char.ToLower(result[i]);
            return result.ToString();
        }
        
        public static ContentControl ToSearchContent(this string searchedString, string search,
                                                     Style defaultTextBlockStyle = null,
                                                     Style matchTextBlockStyle = null)
        {
            var cc = new ContentControl();
            if (string.IsNullOrEmpty(searchedString) || string.IsNullOrEmpty(search))
            {
                cc.Content = GetTextBlock(searchedString, defaultTextBlockStyle);
                return cc;
            }

            if (matchTextBlockStyle == null)
            {
                matchTextBlockStyle = new Style {TargetType = typeof (TextBlock)};
                matchTextBlockStyle.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
            }

            var sp = new StackPanel {Orientation = Orientation.Horizontal};
            int srcIndex = 0;
            do
            {
                int srcNewIndex = searchedString.ToLowerInvariant().IndexOf(search.ToLowerInvariant(), srcIndex, StringComparison.Ordinal);
                if (srcNewIndex != -1)
                {
                    sp.Children.Add(GetTextBlock(searchedString.Substring(srcIndex, srcNewIndex - srcIndex), defaultTextBlockStyle));
                    sp.Children.Add(GetTextBlock(searchedString.Substring(srcNewIndex, search.Length), matchTextBlockStyle));
                    srcIndex = srcNewIndex + search.Length;
                }
                else
                {
                    sp.Children.Add(GetTextBlock(searchedString.Substring(srcIndex), defaultTextBlockStyle));
                    srcIndex = -1;
                }
            } while (srcIndex != -1);
            cc.Content = sp;
            return cc;
        }

        private static TextBlock GetTextBlock(string text, Style style)
        {
            return new TextBlock {Text = text, Style = style};
        }

        public static TextBlock ToTextBlockText(this string me)
        {
            if (me == null) me = string.Empty;
            return new TextBlock {Text = me.Replace(@"\n\r", Environment.NewLine), TextWrapping = TextWrapping.Wrap};
        }

        /// <summary>
        /// Converts string to int array
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static int[] ToIntArray(this string value, char separator)
        {
            return value.Split(separator).Select(int.Parse).ToArray();
        }

        /// <summary>Use this function like string.Split but instead of a character to split on,
        /// use a maximum line width size. This is similar to a Word Wrap where no words will be split.</summary>
        /// Note if the a word is longer than the maxcharactes it will be trimmed from the start.
        /// <param name="me">The string to parse.</param>
        /// <param name="maxCharacters">The maximum size.</param>
        /// <remarks>This function will remove some white space at the end of a line, but allow for a blank line.</remarks>
        /// http://omegacoder.com/?p=661
        /// <returns>An array of strings.</returns>
        public static List<string> SplitOn(this string me, int maxCharacters)
        {
            List<string> lines = new List<string>();
            if (string.IsNullOrEmpty(me) == false)
            {
                const string targetGroup = "Line";
                string theRegex = string.Format(@"(?<{0}>.{{1,{1}}})(?:\W|$)", targetGroup, maxCharacters);

                MatchCollection matches = Regex.Matches(me, theRegex, RegexOptions.IgnoreCase
                                                                      | RegexOptions.Multiline
                                                                      | RegexOptions.ExplicitCapture
                                                                      | RegexOptions.CultureInvariant);
                if (matches.Count > 0)
                    lines.AddRange(from Match m in matches select m.Groups[targetGroup].Value);
            }
            return lines;
        }

        public static string RemoveNonNumeric(this string me)
        {
            if (!string.IsNullOrEmpty(me))
            {
                char[] result = new char[me.Length];
                int resultIndex = 0;
                foreach (char c in me.Where(char.IsNumber))
                    result[resultIndex++] = c;
                if (0 == resultIndex)
                    me = string.Empty;
                else if (result.Length != resultIndex)
                    me = new string(result, 0, resultIndex);
            }
            return me;
        }

        private static readonly Regex EmailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$");

        /// <summary>
        /// true, if is valid email address
        /// from http://www.davidhayden.com/blog/dave/
        /// archive/2006/11/30/ExtensionMethodsCSharp.aspx
        /// </summary>
        /// <param name="me">email address to test</param>
        /// <returns>true, if is valid email address</returns>
        public static bool IsValidEmailAddress(this string me)
        {
            return EmailRegex.IsMatch(me);
        }

        private static readonly Regex UrlRegex =
            new Regex(
                "^(https?://)"
                + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@
                + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
                + "|" // allows either IP or domain
                + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
                + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]" // second level domain
                + @"(\.[a-z]{2,6})?)" // first level domain- .com or .museum is optional
                + "(:[0-9]{1,5})?" // port number- :80
                + "((/?)|" // a slash isn't required if there is no file name
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$"
                );

        /// <summary>
        /// Checks if url is valid. 
        /// from http://www.osix.net/modules/article/?id=586 and changed to match http://localhost
        /// 
        /// complete (not only http) url regex can be found 
        /// at http://internet.ls-la.net/folklore/url-regexpr.html
        /// </summary>
        /// <returns></returns>
        public static bool IsValidUrl(this string me)
        {
            return UrlRegex.IsMatch(me);
        }

        /// <summary>
        /// Reverse the string
        /// from http://en.wikipedia.org/wiki/Extension_method
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string Reverse(this string me)
        {
            char[] chars = me.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// Like LINQ Take - Takes the first x characters
        public static string Take(this string me, int count, bool ellipsis = false)
        {
            int lengthToTake = Math.Min(count, me.Length);
            var cutDownString = me.Substring(0, lengthToTake);

            if (ellipsis && lengthToTake < me.Length)
                cutDownString += "...";

            return cutDownString;
        }

        // Like LINQ Skip - Skips the first x characters and returns the remaining string
        public static string Skip(this string me, int count)
        {
            int startIndex = Math.Min(count, me.Length);
            var cutDownString = me.Substring(startIndex - 1);

            return cutDownString;
        }

        /// <summary>
        /// Reduce string to shorter preview which is optionally ended by some string (...).
        /// </summary>
        /// <param name="me">string to reduce</param>
        /// <param name="count">Length of returned string including endings.</param>
        /// <param name="endings">optional edings of reduced text</param>
        /// <param name="atLeft">if true, takes characters on left side insted of right side.</param>
        /// <example>
        /// string description = "This is very long description of something";
        /// string preview = description.Reduce(20,"...");
        /// produce -> "This is very long..."
        /// </example>
        /// <returns></returns>
        public static string Reduce(this string me, int count, string endings, bool atLeft = false)
        {
            if (me == null) return null;
            if (count < endings.Length)
                throw new Exception("Failed to reduce to less then endings length.");
            int len = me.Length + endings.Length;
            if (count > len) return me; //it's too short to reduce
            return atLeft
                       ? $"{endings}{me.Substring(len - count)}"
                : $"{me.Substring(0, len - count)}{endings}";
        }

        /// <summary>
        /// remove white space, not line end
        /// Useful when parsing user input such phone,
        /// price int.Parse("1 000 000".RemoveSpaces(),.....
        /// </summary>
        /// <param name="me"></param>
        public static string RemoveSpaces(this string me)
        {
            return me.Replace(" ", "");
        }

        /// <summary>
        /// true, if the string can be parse as Double respective Int32
        /// Spaces are not considred.
        /// </summary>
        /// <param name="me">input string</param>
        /// <param name="floatpoint">true, if Double is considered,
        /// otherwhise Int32 is considered.</param>
        /// <returns>true, if the string contains only digits or float-point</returns>
        public static bool IsNumber(this string me, bool floatpoint)
        {
            int i;
            double d;
            string withoutWhiteSpace = me.RemoveSpaces();
            return floatpoint
                       ? double.TryParse(withoutWhiteSpace, NumberStyles.Any, Thread.CurrentThread.CurrentUICulture,
                                         out d)
                       : int.TryParse(withoutWhiteSpace, out i);
        }

        /// <summary>
        /// true, if the string contains only digits or float-point.
        /// Spaces are not considred.
        /// </summary>
        /// <param name="me">input string</param>
        /// <param name="floatpoint">true, if float-point is considered</param>
        /// <returns>true, if the string contains only digits or float-point</returns>
        public static bool IsNumberOnly(this string me, bool floatpoint)
        {
            me = me.Trim();
            return me.Length != 0 && me.Where(c => !char.IsDigit(c)).All(c => floatpoint && (c == '.' || c == ','));
        }

        /// <summary>
        /// Returns the given string truncated to the specified length, suffixed with an elipses (...)
        /// </summary>
        /// <param name="me"></param>
        /// <param name="length">Maximum length of return string</param>
        /// <returns></returns>
        public static string Truncate(this string me, int length)
        {
            return Truncate(me, length, "...");
        }

        /// <summary>
        /// Returns the given string truncated to the specified length, suffixed with the given value
        /// </summary>
        /// <param name="me"></param>
        /// <param name="length">Maximum length of return string</param>
        /// <param name="suffix">The value to suffix the return value with (if truncation is performed)</param>
        /// <returns></returns>
        public static string Truncate(this string me, int length, string suffix)
        {
            if (me == null) return "";
            if (me.Length <= length) return me;
            if (suffix == null) suffix = "...";
            return me.Substring(0, length - suffix.Length) + suffix;
        }

        /// <summary>
        /// Splits a given string into an array based on character line breaks
        /// </summary>
        /// <param name="me"></param>
        /// <returns>String array, each containing one line</returns>
        public static string[] ToLineArray(this string me)
        {
            return me == null ? new string[] {} : Regex.Split(me, "\r\n");
        }

        /// <summary>
        /// Splits a given string into a strongly-typed list based on character line breaks
        /// </summary>
        /// <param name="me"></param>
        /// <returns>Strongly-typed string list, each containing one line</returns>
        public static List<string> ToLineList(this string me)
        {
            var output = new List<string>();
            output.AddRange(me.ToLineArray());
            return output;
        }

        /// <summary>
        /// Replaces line breaks with self-closing HTML 'br' tags
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string ReplaceBreaksWithBr(this string me)
        {
            return string.Join("<br/>", me.ToLineArray());
        }

        /// <summary>
        /// Replaces any single apostrophes with two of the same
        /// </summary>
        /// <param name="me"></param>
        /// <returns>String</returns>
        public static string DoubleApostrophes(this string me)
        {
            return Regex.Replace(me, "'", "''");
        }

        private static readonly Regex StripHtmlRegex =
            new Regex(@"<(style|script)[^<>]*>.*?</\1>|</?[a-z][a-z0-9]*[^<>]*>|<!--.*?-->");

        /// <summary>
        /// Removes any HTML tags from the input string
        /// </summary>
        /// <param name="me"></param>
        /// <returns>String</returns>
        public static string StripHtml(this string me)
        {
            return StripHtmlRegex.Replace(me, "");
        }

        /// <summary>
        /// Return the leftmost number_of_characters characters from the string.
        /// </summary>
        /// <param name="me">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to return.</param>
        /// <returns>A string containing the leftmost characters in this string.</returns>
        public static string Left(this string me, int number_of_characters)
        {
            if (number_of_characters < 0) return me;
            return me.Length <= number_of_characters ? me : me.Substring(0, number_of_characters);
        }

        public static string LeftOf(this string me, string indexOf)
        {
            return me.Left(me.IndexOf(indexOf, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Return the rightmost number_of_characters characters from the string.
        /// </summary>
        /// <param name="me">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to return.</param>
        /// <returns>A string containing the rightmost characters in this string.</returns>
        public static string Right(this string me, int number_of_characters)
        {
            return me.Length <= number_of_characters ? me : me.Substring(me.Length - number_of_characters);
        }

        public static string Mid(this string me, int index, int count)
        {
            return me.Substring(index, count);
        }

        public static int ToInteger(this string me)
        {
            int integerValue;
            int.TryParse(me, out integerValue);
            return integerValue;
        }

        public static bool IsInteger(this string me)
        {
            Regex regularExpression = new Regex("^-[0-9]+$|^[0-9]+$");
            return regularExpression.Match(me).Success;
        }

        /// <summary>
        /// Return the string with the leftmost number_of_characters characters removed.
        /// </summary>
        /// <param name="me">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveLeft(this string me, int number_of_characters)
        {
            if (me.Length <= number_of_characters) return "";
            return me.Substring(number_of_characters);
        }

        /// <summary>
        /// Return the string with the rightmost number_of_characters characters removed.
        /// </summary>
        /// <param name="me">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveRight(this string me, int number_of_characters)
        {
            if (me.Length <= number_of_characters) return "";
            return me.Substring(0, me.Length - number_of_characters);
        }

        #region << Encrypt/Decrypt >>

        private static byte[] GetHashKey(string hashKey)
        {
            // Initialise
            var encoder = new UTF8Encoding();

            // Get the salt
            const string salt = "I am a nice little salt";
            byte[] saltBytes = encoder.GetBytes(salt);

            // Setup the hasher
            var rfc = new Rfc2898DeriveBytes(hashKey, saltBytes);

            // Return the key
            return rfc.GetBytes(16);
        }

        /// <summary>
        /// Encrypt this string and return the result as a string of hexadecimal characters.
        /// </summary>
        /// <param name="me">The string being extended and that will be encrypted.</param>
        /// <param name="password">The password to use then encrypting the string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string EncryptToString(this string me, string password)
        {
            return Encrypt(me, GetHashKey(password));
        }

        private static string Encrypt(string stringToEncrypt, byte[] key)
        {
            if (string.IsNullOrEmpty(stringToEncrypt))
                return string.Empty;

            // Initialise
            var encryptor = new AesManaged {Key = key, IV = key};

            // Set the key

            // create a memory stream
            using (var encryptionStream = new MemoryStream())
            {
                // Create the crypto stream
                using (
                    var encrypt = new CryptoStream(encryptionStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write)
                    )
                {
                    // Encrypt
                    byte[] utfD1 = Encoding.UTF8.GetBytes(stringToEncrypt);
                    encrypt.Write(utfD1, 0, utfD1.Length);
                    encrypt.FlushFinalBlock();
                    encrypt.Close();

                    // Return the encrypted data
                    return Convert.ToBase64String(encryptionStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Decrypt the encryption stored in this string of hexadecimal values.
        /// </summary>
        /// <param name="me">The hexadecimal string to decrypt.</param>
        /// <param name="password">The password to use then encrypting the string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string DecryptFromString(this string me, string password)
        {
            return Decrypt(me, GetHashKey(password));
        }

        private static string Decrypt(string encryptedString, byte[] key)
        {
            if (string.IsNullOrEmpty(encryptedString))
                return string.Empty;

            // Initialise
            var decryptor = new AesManaged();
            byte[] encryptedData = Convert.FromBase64String(encryptedString);

            // Set the key
            decryptor.Key = key;
            decryptor.IV = key;

            // create a memory stream
            using (var decryptionStream = new MemoryStream())
            {
                // Create the crypto stream
                using (
                    var decrypt = new CryptoStream(decryptionStream, decryptor.CreateDecryptor(), CryptoStreamMode.Write)
                    )
                {
                    // Encrypt
                    decrypt.Write(encryptedData, 0, encryptedData.Length);
                    decrypt.Flush();
                    decrypt.Close();

                    // Return the unencrypted data
                    byte[] decryptedData = decryptionStream.ToArray();
                    return Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);
                }
            }
        }

        #endregion

        /// <summary>
        /// Convert this string containing hexadecimal into a byte array.
        /// </summary>
        /// <param name="me">The hexadecimal string to convert.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] HexStringToBytes(this string me)
        {
            me = me.Replace(" ", "");
            int max_byte = (me.Length/2) - 1;
            var bytes = new byte[max_byte + 1];
            for (int i = 0; i <= max_byte; i++)
            {
                bytes[i] = byte.Parse(me.Substring(2*i, 2), NumberStyles.AllowHexSpecifier);
            }

            return bytes;
        }

        // "a string".IsNullOrEmpty() beats string.IsNullOrEmpty("a string")
        public static bool IsNullOrEmpty(this string me)
        {
            return string.IsNullOrEmpty(me);
        }

        /// <summary>
        /// Checks if string is in "version" format
        /// </summary>
        /// <param name="me">The hexadecimal string to convert.</param>
        /// <returns></returns>
        public static bool IsValidSectionedNumber(this string me)
        {
            // SectionedNumbers may have up to 10 sections of 5-or-less-digit numbers.
            string[] sections = me.Split('.');
            if (sections.Length > 10)
            {
                return false;
            }
            foreach (string t in sections)
            {
                if (t.Length < 1 || t.Length > 5)
                    return false;
                int number;
                if (int.TryParse(t, out number) == false)
                    return false;
            }
            return true;
        }

        private static readonly Regex NewLineRegex = new Regex(@"\r(?!\n)|(?<!\r)\n");

        public static string NormalizeNewLines(this string value)
        {
            // From: 
            // http://stackoverflow.com/questions/3022571/how-to-deal-with-the-new-line-character-in-the-silverlight-textbox
            return value == null ? null : NewLineRegex.Replace(value, Environment.NewLine);
        }

        public static int CountOf(this string value, char c)
        {
            return value == null ? 0 : value.Count(m_T => m_T == c);
        }

        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            return source.IndexOf(value, comparison) >= 0;
        }

        public static bool ContainsIgnoreCase(this string source, string value)
        {
            return source.Contains(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool StartsWithIgnoreCase(this string source, string value)
        {
            return source.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string PadCenter(this string me, int width, char c = ' ')
        {
            if (IsNullOrEmpty(me) || width <= me.Length) return me;
            int padding = width - me.Length;
            return me.PadLeft(me.Length + padding/2, c).PadRight(width, c);
        }

        #endregion
    }
}