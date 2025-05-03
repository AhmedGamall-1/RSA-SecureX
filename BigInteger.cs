using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSASecureX
{
    /// <summary>
    /// Represents an arbitrary-precision integer that can handle numbers with hundreds of digits.
    /// Numbers are stored as a list of digits in reverse order (least significant digit first)
    /// to make arithmetic operations easier to implement.
    /// </summary>
    public class BigInteger
    {
        private List<int> digits;  // Stores digits in reverse order (least significant digit first)
        private bool isNegative;   // Flag indicating if the number is negative

        /// <summary>
        /// Creates a new BigInteger from a string representation of a number.
        /// </summary>
        /// <param name="number">String representation of the number (can include negative sign)</param>
        /// <exception cref="ArgumentException">Thrown if input is null, empty, or contains invalid characters</exception>
        /// <remarks>
        /// Example: For input "-1234":
        /// - isNegative = true
        /// - digits = [4,3,2,1]
        /// </remarks>
        public BigInteger(string number)
        {
            if (string.IsNullOrEmpty(number))
                throw new ArgumentException("Number cannot be null or empty");

            isNegative = number[0] == '-';
            digits = new List<int>();

            // Process digits from right to left to store them in reverse order
            for (int i = number.Length - 1; i >= (isNegative ? 1 : 0); i--)
            {
                if (!char.IsDigit(number[i]))
                    throw new ArgumentException("Invalid number format");
                digits.Add(number[i] - '0');
            }
        }

        /// <summary>
        /// Creates a new BigInteger from a list of digits and a sign flag.
        /// </summary>
        /// <param name="digits">List of digits in reverse order (least significant first)</param>
        /// <param name="isNegative">Flag indicating if the number is negative</param>
        public BigInteger(List<int> digits, bool isNegative = false)
        {
            this.digits = new List<int>(digits);
            this.isNegative = isNegative;
        }

        /// <summary>
        /// Adds two BigInteger numbers.
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Sum of the two numbers</returns>
        /// <remarks>
        /// If numbers have same sign, adds their magnitudes and keeps the sign.
        /// If numbers have different signs, converts to subtraction.
        /// Example: (-5) + 3 = -(5 - 3) = -2
        /// </remarks>
        public static BigInteger Add(BigInteger a, BigInteger b)
        {
            if (a.isNegative == b.isNegative)
            {
                var result = AddDigits(a.digits, b.digits);
                return new BigInteger(result, a.isNegative);
            }
            else
            {
                if (a.isNegative)
                {
                    a.isNegative = false;
                    return Subtract(b, a);
                }
                else
                {
                    b.isNegative = false;
                    return Subtract(a, b);
                }
            }
        }

        /// <summary>
        /// Subtracts one BigInteger from another.
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Difference between the two numbers</returns>
        /// <remarks>
        /// If numbers have different signs, converts to addition.
        /// If numbers have same sign, compares magnitudes and subtracts accordingly.
        /// Example: 5 - (-3) = 5 + 3 = 8
        /// </remarks>
        public static BigInteger Subtract(BigInteger a, BigInteger b)
        {
            if (a.isNegative != b.isNegative)
            {
                b.isNegative = !b.isNegative;
                return Add(a, b);
            }

            bool resultIsNegative = false;
            List<int> result;

            if (CompareMagnitude(a.digits, b.digits) >= 0)
            {
                result = SubtractDigits(a.digits, b.digits);
                resultIsNegative = a.isNegative;
            }
            else
            {
                result = SubtractDigits(b.digits, a.digits);
                resultIsNegative = !a.isNegative;
            }

            return new BigInteger(result, resultIsNegative);
        }

        /// <summary>
        /// Multiplies two BigInteger numbers.
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Product of the two numbers</returns>
        /// <remarks>
        /// Uses standard multiplication algorithm, digit by digit.
        /// Result is negative if operands have different signs.
        /// Example: (-5) * 3 = -(5 * 3) = -15
        /// </remarks>
        public static BigInteger Multiply(BigInteger a, BigInteger b)
        {
            var result = MultiplyDigits(a.digits, b.digits);
            return new BigInteger(result, a.isNegative != b.isNegative);
        }

        /// <summary>
        /// Divides one BigInteger by another and returns both quotient and remainder.
        /// </summary>
        /// <param name="a">Dividend</param>
        /// <param name="b">Divisor</param>
        /// <returns>Tuple containing quotient and remainder</returns>
        /// <exception cref="DivideByZeroException">Thrown if divisor is zero</exception>
        /// <remarks>
        /// Uses repeated subtraction algorithm.
        /// Quotient is negative if operands have different signs.
        /// Remainder has same sign as dividend.
        /// </remarks>
        public static (BigInteger quotient, BigInteger remainder) Divide(BigInteger a, BigInteger b)
        {
            if (b.digits.All(d => d == 0))
                throw new DivideByZeroException();

            var (quotient, remainder) = DivideDigits(a.digits, b.digits);
            return (new BigInteger(quotient, a.isNegative != b.isNegative),
                    new BigInteger(remainder, a.isNegative));
        }

        /// <summary>
        /// Checks if the number is even.
        /// </summary>
        /// <returns>True if the number is even, false otherwise</returns>
        /// <remarks>
        /// Checks the least significant digit (digits[0]).
        /// Example: 1234 is even because 4 % 2 = 0
        /// </remarks>
        public bool IsEven()
        {
            return digits[0] % 2 == 0;
        }

        /// <summary>
        /// Converts the BigInteger to its string representation.
        /// </summary>
        /// <returns>String representation of the number</returns>
        /// <remarks>
        /// Example: For number stored as [4,3,2,1] with isNegative = true:
        /// Returns "-1234"
        /// </remarks>
        public override string ToString()
        {
            if (digits.Count == 0) return "0";
            var result = string.Join("", digits.AsEnumerable().Reverse());
            return isNegative ? "-" + result : result;
        }

        /// <summary>
        /// Converts a string to a BigInteger.
        /// </summary>
        /// <param name="text">Input string to convert</param>
        /// <returns>BigInteger representation of the string</returns>
        /// <remarks>
        /// Converts string to UTF-8 bytes, then to hexadecimal string.
        /// Example: "Hello" → [72,101,108,108,111] → "48656C6C6F" → BigInteger
        /// </remarks>
        public static BigInteger FromString(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            string number = BitConverter.ToString(bytes).Replace("-", "");
            return new BigInteger(number);
        }

        /// <summary>
        /// Converts the BigInteger back to its original string.
        /// </summary>
        /// <returns>Original string that was converted to this BigInteger</returns>
        /// <remarks>
        /// Converts the number to hex string, then to bytes, then to UTF-8 string.
        /// Example: BigInteger("48656C6C6F") → "Hello"
        /// </remarks>
        public string ToOriginalString()
        {
            if (digits.Count == 0) return "";
            var hexString = ToString();
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Adds two lists of digits representing numbers in reverse order.
        /// </summary>
        /// <param name="a">First number's digits</param>
        /// <param name="b">Second number's digits</param>
        /// <returns>Sum of the two numbers as a list of digits</returns>
        /// <remarks>
        /// Example: Adding 123 + 456
        /// a = [3,2,1], b = [6,5,4]
        /// Result = [9,7,5] (representing 579)
        /// </remarks>
        private static List<int> AddDigits(List<int> a, List<int> b)
        {
            var result = new List<int>();
            int carry = 0;
            int maxLength = Math.Max(a.Count, b.Count);

            for (int i = 0; i < maxLength || carry > 0; i++)
            {
                int sum = carry;
                if (i < a.Count) sum += a[i];
                if (i < b.Count) sum += b[i];

                result.Add(sum % 10);
                carry = sum / 10;
            }

            return result;
        }

        /// <summary>
        /// Subtracts one list of digits from another.
        /// </summary>
        /// <param name="a">First number's digits</param>
        /// <param name="b">Second number's digits</param>
        /// <returns>Difference of the two numbers as a list of digits</returns>
        /// <remarks>
        /// Example: Subtracting 456 - 123
        /// a = [6,5,4], b = [3,2,1]
        /// Result = [3,3,3] (representing 333)
        /// </remarks>
        private static List<int> SubtractDigits(List<int> a, List<int> b)
        {
            var result = new List<int>();
            int borrow = 0;

            for (int i = 0; i < a.Count; i++)
            {
                int diff = a[i] - borrow;
                if (i < b.Count) diff -= b[i];

                if (diff < 0)
                {
                    diff += 10;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result.Add(diff);
            }

            // Remove leading zeros
            while (result.Count > 1 && result[result.Count - 1] == 0)
                result.RemoveAt(result.Count - 1);

            return result;
        }

        /// <summary>
        /// Multiplies two lists of digits using standard multiplication algorithm.
        /// </summary>
        /// <param name="a">First number's digits</param>
        /// <param name="b">Second number's digits</param>
        /// <returns>Product of the two numbers as a list of digits</returns>
        /// <remarks>
        /// Example: Multiplying 12 * 34
        /// a = [2,1], b = [4,3]
        /// Result = [8,0,4,0] (representing 408)
        /// </remarks>
        private static List<int> MultiplyDigits(List<int> a, List<int> b)
        {
            var result = new List<int>(new int[a.Count + b.Count]);

            for (int i = 0; i < a.Count; i++)
            {
                int carry = 0;
                for (int j = 0; j < b.Count || carry > 0; j++)
                {
                    int product = result[i + j] + a[i] * (j < b.Count ? b[j] : 0) + carry;
                    result[i + j] = product % 10;
                    carry = product / 10;
                }
            }

            // Remove leading zeros
            while (result.Count > 1 && result[result.Count - 1] == 0)
                result.RemoveAt(result.Count - 1);

            return result;
        }

        /// <summary>
        /// Divides one list of digits by another using repeated subtraction.
        /// </summary>
        /// <param name="a">Dividend's digits</param>
        /// <param name="b">Divisor's digits</param>
        /// <returns>Tuple containing quotient and remainder as lists of digits</returns>
        /// <remarks>
        /// Example: Dividing 100 by 3
        /// a = [0,0,1], b = [3]
        /// Result: quotient = [3,3], remainder = [1] (representing 33 with remainder 1)
        /// </remarks>
        private static (List<int> quotient, List<int> remainder) DivideDigits(List<int> a, List<int> b)
        {
            if (CompareMagnitude(a, b) < 0)
                return (new List<int> { 0 }, new List<int>(a));

            var quotient = new List<int>();
            var remainder = new List<int>(a);

            while (CompareMagnitude(remainder, b) >= 0)
            {
                var (q, r) = DivideStep(remainder, b);
                quotient = AddDigits(quotient, q);
                remainder = r;
            }

            return (quotient, remainder);
        }

        /// <summary>
        /// Performs a single step of division by subtracting divisor from remainder.
        /// </summary>
        /// <param name="a">Current remainder</param>
        /// <param name="b">Divisor</param>
        /// <returns>Tuple containing quotient (1) and new remainder</returns>
        /// <remarks>
        /// This is a helper method for DivideDigits that performs one subtraction step.
        /// </remarks>
        private static (List<int> quotient, List<int> remainder) DivideStep(List<int> a, List<int> b)
        {
            var quotient = new List<int> { 1 };
            var remainder = SubtractDigits(a, b);
            return (quotient, remainder);
        }

        /// <summary>
        /// Compares the magnitudes of two lists of digits.
        /// </summary>
        /// <param name="a">First number's digits</param>
        /// <param name="b">Second number's digits</param>
        /// <returns>-1 if a < b, 0 if a == b, 1 if a > b</returns>
        /// <remarks>
        /// First compares lengths, then compares digits from most significant to least.
        /// Example: Comparing [3,2,1] and [4,3,2,1]
        /// Returns -1 because first number is shorter
        /// </remarks>
        private static int CompareMagnitude(List<int> a, List<int> b)
        {
            if (a.Count != b.Count)
                return a.Count.CompareTo(b.Count);

            for (int i = a.Count - 1; i >= 0; i--)
            {
                if (a[i] != b[i])
                    return a[i].CompareTo(b[i]);
            }

            return 0;
        }
    }
} 
