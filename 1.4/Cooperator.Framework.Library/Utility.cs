/*-
*       Copyright (c) 2006-2007 Eugenio Serrano, Daniel Calvin.
*       All rights reserved.
*
*       Redistribution and use in source and binary forms, with or without
*       modification, are permitted provided that the following conditions
*       are met:
*       1. Redistributions of source code must retain the above copyright
*          notice, this list of conditions and the following disclaimer.
*       2. Redistributions in binary form must reproduce the above copyright
*          notice, this list of conditions and the following disclaimer in the
*          documentation and/or other materials provided with the distribution.
*       3. Neither the name of copyright holders nor the names of its
*          contributors may be used to endorse or promote products derived
*          from this software without specific prior written permission.
*
*       THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
*       "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
*       TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
*       PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL COPYRIGHT HOLDERS OR CONTRIBUTORS
*       BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
*       CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
*       SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
*       INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
*       CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
*       ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
*       POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Cooperator.Framework.Library
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Utility
    {
        /// <summary>
        /// This class connot be instantiated
        /// </summary>
        private Utility()
        {
        }

        #region "NumbersToWords"


        private readonly static string[] _numbersInWords = {
            "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve",
            "diez", "once", "doce", "trece", "catorce", "quince", "dieciseis", "diecisiete", "dieciocho", "diecinueve",
            "veinte", "veintiuno", "veintidos", "veintitres", "veinticuatro", "veinticinco", "veintiseis", "veintisiete", "veintiocho", "veintinueve",
            "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa",
			"ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos"
		};

        private const string _centWord = "centavo(s)";
        private const string _oneMillionWord = "un millon";
        private const string _millionsWord = "millones";
        private const string _oneThousandWord = "un mil";
        private const string _thousandWord = "mil";
        private const string _hundredWord = "cien";
        private const string _oneHundredWord = "ciento un";
        private const string _zeroWithWord = "cero con";
        private const string _withWord = "con";
        private const string _andWord = "y";

        /// <summary>
        /// Converts numbers to words
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string NumberToWords(decimal number)
        {

            if (number > 999999999)
            {
                return "Numero demasiado grande";
            }

            StringBuilder Words;
            string FormattedNumberString;
            int decimalSeparatorLocation;
            int millionsPart;
            int thousandsPart;
            int hundredsPart;
            int decimalsPart;
            int hundreds;
            int tens;
            int units;
            int ActualNumber = 0;

            Words = new StringBuilder();
            FormattedNumberString = number.ToString("000000000.00");
            char DecimalSeparator = Convert.ToChar((Convert.ToString(1.1)).Trim().Substring(1, 1));
            decimalSeparatorLocation = FormattedNumberString.IndexOf(DecimalSeparator);

            millionsPart = Convert.ToInt32(FormattedNumberString.Substring(0, 3));
            thousandsPart = Convert.ToInt32(FormattedNumberString.Substring(3, 3));
            hundredsPart = Convert.ToInt32(FormattedNumberString.Substring(6, 3));
            decimalsPart = Convert.ToInt32(FormattedNumberString.Substring(decimalSeparatorLocation + 1, 2));
            for (int NumberPart = 1; NumberPart <= 4; NumberPart++)
            {
                switch (NumberPart)
                {
                    case 1:
                        {
                            ActualNumber = millionsPart;
                            if (millionsPart == 1)
                            {
                                Words.Append(_oneMillionWord);
                                Words.Append(' ');
                                continue;
                            }
                            break;
                        }
                    case 2:
                        {
                            ActualNumber = thousandsPart;
                            if (millionsPart != 1 && millionsPart != 0)
                            {
                                Words.Append(_millionsWord);
                                Words.Append(' ');
                            }
                            if (thousandsPart == 1)
                            {
                                Words.Append(_oneThousandWord);
                                Words.Append(' ');
                                continue;
                            }
                            break;
                        }
                    case 3:
                        {
                            ActualNumber = hundredsPart;
                            if (thousandsPart != 1 && thousandsPart != 0)
                            {
                                Words.Append(_thousandWord);
                                Words.Append(' ');
                            }
                            break;
                        }
                    case 4:
                        {
                            ActualNumber = decimalsPart;
                            if (decimalsPart != 0)
                            {
                                if (millionsPart == 0 && thousandsPart == 0 && hundredsPart == 0)
                                {
                                    Words.Append(_zeroWithWord);
                                    Words.Append(' ');
                                }
                                else
                                {
                                    Words.Append(_withWord);
                                    Words.Append(' ');
                                }
                            }
                            break;
                        }
                }

                hundreds = (int)(ActualNumber / 100);
                tens = (int)(ActualNumber - hundreds * 100) / 10;
                units = (int)(ActualNumber - (hundreds * 100 + tens * 10));
                if (ActualNumber == 0) continue;

                if (ActualNumber == 100)
                {
                    Words.Append(_hundredWord);
                    Words.Append(' ');
                    continue;
                }
                else
                {
                    if (ActualNumber == 101 && NumberPart != 3)
                    {
                        Words.Append(_oneHundredWord);
                        Words.Append(' ');
                        continue;
                    }
                    else
                    {
                        if (ActualNumber > 100)
                        {
                            Words.Append(_numbersInWords[hundreds + 35]);
                            Words.Append(' ');
                        }
                    }
                }
                if (tens < 3 && tens != 0)
                {
                    Words.Append(_numbersInWords[tens * 10 + units - 1]);
                    Words.Append(' ');
                }
                else
                {
                    if (tens > 2)
                    {
                        Words.Append(_numbersInWords[tens + 26]);
                        Words.Append(' ');
                        if (units == 0)
                        {
                            continue;
                        }
                        Words.Append(_andWord);
                        Words.Append(' ');
                        Words.Append(_numbersInWords[units - 1]);
                        Words.Append(' ');
                    }
                    else
                    {
                        if (tens == 0 && units != 0)
                        {
                            Words.Append(_numbersInWords[units - 1]);
                            Words.Append(' ');
                        }
                    }
                }
            } // end for

            if (decimalsPart != 0)
            {
                Words.Append(_centWord);
            }

            // Resolve particular problems here.
            Words.Replace("uno mil", "un mil");

            return Words.ToString().Trim();
        }

        # endregion


        #region "RandomPasswords"


        /// <summary>
        /// 
        /// </summary>
        public static class RandomPassword
        {


            // Define default min and max password lengths.
            private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
            private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

            // Define supported password characters divided into groups.
            // You can add (or remove) characters to (from) these groups.
            private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
            private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            private static string PASSWORD_CHARS_NUMERIC = "23456789";
            private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

            /// <summary>
            /// Generates a random password.
            /// </summary>
            /// <returns>
            /// Randomly generated password.
            /// </returns>
            /// <remarks>
            /// The length of the generated password will be determined at
            /// random. It will be no shorter than the minimum default and
            /// no longer than maximum default.
            /// </remarks>
            public static string Generate()
            {
                return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                                DEFAULT_MAX_PASSWORD_LENGTH);
            }

            /// <summary>
            /// Generates a random password of the exact length.
            /// </summary>
            /// <param name="length">
            /// Exact password length.
            /// </param>
            /// <returns>
            /// Randomly generated password.
            /// </returns>
            public static string Generate(int length)
            {
                return Generate(length, length);
            }

            /// <summary>
            /// Generates a random password.
            /// </summary>
            /// <param name="minLength">
            /// Minimum password length.
            /// </param>
            /// <param name="maxLength">
            /// Maximum password length.
            /// </param>
            /// <returns>
            /// Randomly generated password.
            /// </returns>
            /// <remarks>
            /// The length of the generated password will be determined at
            /// random and it will fall with the range determined by the
            /// function parameters.
            /// </remarks>
            public static string Generate(int minLength,
                                          int maxLength)
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported password characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the password strength.
                char[][] charGroups = new char[][] 
        {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray(),
            PASSWORD_CHARS_SPECIAL.ToCharArray()
        };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold password characters.
                char[] password = null;

                // Allocate appropriate memory for the password.
                if (minLength < maxLength)
                    password = new char[random.Next(minLength, maxLength + 1)];
                else
                    password = new char[minLength];

                // Index of the next character to be added to password.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < password.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0,
                                                             lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the password.
                    password[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] =
                                                  charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert password characters into a string and return the result.
                return new string(password);
            }
        }



        #endregion


        #region "IsEmailValid"

        /// <summary>
        /// Chequea si el formato del mail es valido.
        /// </summary>
        /// <param name="emailAddress">La direccion de correo que se quiere verificar</param>
        /// <returns>Verdadero o Falso si es una direccion valida o no</returns>
        public static bool IsEmailValid(string emailAddress)
        {
            string sEmailRegex = "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$";
            System.Text.RegularExpressions.Regex oRegex = new System.Text.RegularExpressions.Regex(sEmailRegex);
            if (!((oRegex.IsMatch(emailAddress))))
                return false;
            else
                return true;
        }


        #endregion


        #region "GetDriveSerialNumber""

        /// <summary>
        /// Devuelve el numero de serie de la unidad especificada
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns></returns>
        public static string GetDriveSerialNumber(string driveLetter)
        {
            System.Management.ManagementObject disk = new System.Management.ManagementObject("win32_logicaldisk.deviceid=\"" + driveLetter + ":\"");
            disk.Get();

            if (disk["VolumeSerialNumber"] != null)
                return disk["VolumeSerialNumber"].ToString();
            else
                return "No Information Avalaible";
        }

        #endregion
    }
}

