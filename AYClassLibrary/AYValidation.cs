using System;
using System.Text.RegularExpressions;

namespace AYClassLibrary
{
    public static class AYValidation
    {
        public static string AYCapitalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            input = input.ToLower().Trim();
            string result = string.Empty;
            if (input.Length==1)
            {
                result = input.ToUpper();
            }
            else
            {
                string[] inputArray = input.Split(" ");                

                foreach (var s in inputArray)
                {
                    result += s.Substring(0, 1).ToUpper() + s.Substring(1) + " ";
                }

            }

            return result;
        }


        public static string AYExtractDigits(string input)
        {
            if (input == null)
            {
                return input;
            }

            input = input.Trim();

            string result = Regex.Replace(input, @"[^\d]", ""); //change all non-digit chars to ""

            return result;
        }

        public static bool AYPostalCodeValidation(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
            if (input==" ")
            {
                return false;
            }

            Regex postalCodePattern = new Regex(@"^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z] ?\d[ABCEGHJ-NPRSTV-Z]\d$", RegexOptions.IgnoreCase);
            
            //the SPACE in the middle A1A 1A1 can both exist or not exist, so we use" ?"

            if (postalCodePattern.IsMatch(input) )
            {
                return true;
            }

            return false;
        }

        public static string AYPostalCodeFormat(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = input.ToUpper().Replace(" ", "");
            string result = input.Insert(3, " ");
            return result;
        }

        public static bool AYZipCodeValidation(ref string input) //ref changed the string that's used here
        {
            if (string.IsNullOrEmpty(input))
            {
                input = "";
                return true;
            }
            Regex zipCodePattern = new Regex(@"^\d{5}(?:[-\s]\d{4})?$", RegexOptions.IgnoreCase);

            if (zipCodePattern.IsMatch(input))
            {
                string inputDigits = AYExtractDigits(input);
                if (inputDigits.Length == 5)
                {
                    input = inputDigits;
                    return true;
                }
                else if (inputDigits.Length == 9)
                {
                    input = input.Insert(5, "-");
                    return true;
                }
            }
            
            return false;           
            
        }
    }
}