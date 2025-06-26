using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigIntegerProject
{
    public class BigInteger
    {
        private List<int> digits;
        private bool isNegative;

        public BigInteger(string number)
        {
            if (string.IsNullOrEmpty(number))
                throw new ArgumentException("Number cannot be null or empty");

            isNegative = number[0] == '-';
            digits = new List<int>();

            int startIndex = isNegative ? 1 : 0;
            for (int i = startIndex; i < number.Length; i++)
            {
                if (!char.IsDigit(number[i]))
                    throw new ArgumentException("Invalid number format");
                digits.Add(number[i] - '0');
            }
            RemoveLeadingZeros();
        }
        public BigInteger(List<int> digits, bool isNegative = false)
        {
            this.digits = new List<int>(digits);
            this.isNegative = isNegative;
            RemoveLeadingZeros();
        }
        private void RemoveLeadingZeros()
        {
            while (digits.Count > 1 && digits[0] == 0)
                digits.RemoveAt(0);
        }
        public static BigInteger Add(BigInteger a, BigInteger b)
        {
            if (a.isNegative == b.isNegative)
            {
                int n = Math.Max(a.digits.Count, b.digits.Count);

                a.digits.InsertRange(0, Enumerable.Repeat(0, n - a.digits.Count));  // bet7ot esfar 3and el rakam elli el digits beta3to a2al
                b.digits.InsertRange(0, Enumerable.Repeat(0, n - b.digits.Count));  // 3shan yeb2o nafs 3adad el digits

                List<int> resultDigits = new List<int>(Enumerable.Repeat(0, n));  // ba3mel list b n esfar
                int carry = 0;

                for (int i = n - 1; i >= 0; i--)
                {
                    int sum = a.digits[i] + b.digits[i] + carry;
                    resultDigits[i] = sum % 10;  // law el rakam akbar men 10 betraga3 el unit  
                    carry = sum / 10; // w tesib el 1 fel carry
                }

                if (carry > 0)
                    resultDigits.Insert(0, carry); // 3shan law fih carry fel a5er tezawedo 

                return new BigInteger(resultDigits, a.isNegative);
            }
            else
            {
                if (a.isNegative)
                {
                    var result = Subtract(b, a);
                    return result;
                }
                else
                {
                    var result = Subtract(a, b);
                    return result;
                }
            }
        }
        public static BigInteger Subtract(BigInteger a, BigInteger b)
        {
            if (a.isNegative != b.isNegative)
            {
                BigInteger bInverse = new BigInteger(b.digits, !b.isNegative);
                return Add(a, bInverse);
            }

            int compareResult = CompareMagnitude(a.digits, b.digits); // law absolute(a) as8ar men absolute(b)
            bool resultIsNegative = (compareResult < 0) != a.isNegative; // haye3kes el sign

            if (compareResult < 0) // swap a and b if b akbar men a
            {
                BigInteger temp = a;
                a = b;
                b = temp;
            }

            int n = Math.Max(a.digits.Count, b.digits.Count);

            a.digits.InsertRange(0, Enumerable.Repeat(0, n - a.digits.Count)); // beyzawed esfar fel digits el a2al
            b.digits.InsertRange(0, Enumerable.Repeat(0, n - b.digits.Count)); // 3shan yeb2o el 2 nafs 3adad el digits

            List<int> resultDigits = new List<int>(Enumerable.Repeat(0, n)); // initialize list b esfar 3shan a subtract 3aleha
            int borrow = 0;

            for (int i = n - 1; i >= 0; i--)
            {
                int diff = a.digits[i] - b.digits[i] - borrow; 
                if (diff < 0)
                {
                    diff += 10; 
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }
                resultDigits[i] = diff;
            }

            while (resultDigits.Count > 1 && resultDigits[0] == 0)
                resultDigits.RemoveAt(0);

            return new BigInteger(resultDigits, resultIsNegative);
        }
        public static BigInteger Multiply(BigInteger a, BigInteger b)
        {
            var resultDigits = Karatsuba(a.digits, b.digits);
            bool resultIsNegative = a.isNegative != b.isNegative && !IsZero(resultDigits); // hatetla3 negative law sign a 3aks b w mafish wa7da fihom b zero
            return new BigInteger(resultDigits, resultIsNegative);
        }
        private static bool IsZero(List<int> digits)
        {
            return digits.Count == 1 && digits[0] == 0;
        }
        public static (BigInteger quotient, BigInteger remainder) Divide(BigInteger a, BigInteger b)
        {
            if (CompareMagnitude(a.digits, b.digits) < 0)
                return (new BigInteger(new List<int> { 0 }), a);

            var two = new BigInteger("2");
            var (q, r) = Divide(a, Multiply(b, two));

            q = Multiply(q, two);

            if (CompareMagnitude(r.digits, b.digits) < 0)
            {
                return (q, r);
            }
            else
            {
                return (Add(q, new BigInteger("1")), Subtract(r, b));
            }
        }
        public static BigInteger Modulus(BigInteger a, BigInteger b)
        {
            var (_, remainder) = Divide(a, b);
            return remainder;
        }
        public bool IsEven()
        {
            return digits.Count > 0 && digits[digits.Count - 1] % 2 == 0;
        }
        public override string ToString()
        {
            if (digits.Count == 0 || (digits.Count == 1 && digits[0] == 0)) return "0";
            var result = string.Join("", digits);
            return isNegative ? "-" + result : result;
        }
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
        private static List<int> Karatsuba(List<int> a, List<int> b)
        {
            a = Reverse(a); 
            b = Reverse(b);

            List<int> result = KaratsubaImplementation(a, b);
            return Reverse(result);
        }
        private static List<int> KaratsubaImplementation(List<int> a, List<int> b)
        {
            Trim(a); // benshil el zeros elli na7yet el unit (yemin)
            Trim(b); // 3shan lama tet2eleb tani maraga3sh el esfar

            if (a.Count == 0 || b.Count == 0) // base case
                return new List<int> { 0 };
            if (a.Count == 1 && b.Count == 1) // base case
                return SimpleMultiply(a[0], b[0]);

            if (a.Count <= 32 && b.Count <= 32)
            {
                return SimpleMultiplyLists(a, b); // law 3adad el digits a2al men 32 ba3mel el multiplication el 3adeya
            }

            int m = Math.Max(a.Count, b.Count) / 2; // 3shan a2sem el rakam nosen ad ba3d

            // get range 3shan a2asem el arkam
            var aLow = a.GetRange(0, Math.Min(m, a.Count)); 
            var aHigh = a.GetRange(Math.Min(m, a.Count), Math.Max(0, a.Count - m));
            var bLow = b.GetRange(0, Math.Min(m, b.Count));
            var bHigh = b.GetRange(Math.Min(m, b.Count), Math.Max(0, b.Count - m));

            // (aLow + aHigh) * (bLow + bHigh) = aLow.bLow (z0) + aLow.bHigh + bLow.aHigh +aHigh.bHigh (z2)
            // z1 = (aLow + aHigh) * (bLow + bHigh) - z0 - z2
            var z0 = KaratsubaImplementation(aLow, bLow);
            var z2 = KaratsubaImplementation(aHigh, bHigh);
            var aSum = AddDigitsInternal(aLow, aHigh);
            var bSum = AddDigitsInternal(bLow, bHigh);
            var z1 = SubtractDigitsInternal(SubtractDigitsInternal
                (KaratsubaImplementation(aSum, bSum), z2), z0);

            var result = AddDigitsInternal(ShiftLeft(z2, 2 * m), 
                AddDigitsInternal(ShiftLeft(z1, m), z0)); 
            Trim(result);
            return result;
        }
        private static List<int> AddDigitsInternal(List<int> a, List<int> b)
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
        private static List<int> SubtractDigitsInternal(List<int> a, List<int> b)
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

            while (result.Count > 1 && result[result.Count - 1] == 0)
                result.RemoveAt(result.Count - 1);

            return result;
        }
        private static List<int> SimpleMultiplyLists(List<int> a, List<int> b)
        {
            int[] resultArray = new int[a.Count + b.Count];

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] == 0) continue;

                int carry = 0;
                int j = 0;

                for (; j + 3 < b.Count; j += 4)
                {
                    int sum1 = resultArray[i + j] + a[i] * b[j] + carry;
                    resultArray[i + j] = sum1 % 10;
                    carry = sum1 / 10;

                    int sum2 = resultArray[i + j + 1] + a[i] * b[j + 1] + carry;
                    resultArray[i + j + 1] = sum2 % 10;
                    carry = sum2 / 10;

                    int sum3 = resultArray[i + j + 2] + a[i] * b[j + 2] + carry;
                    resultArray[i + j + 2] = sum3 % 10;
                    carry = sum3 / 10;

                    int sum4 = resultArray[i + j + 3] + a[i] * b[j + 3] + carry;
                    resultArray[i + j + 3] = sum4 % 10;
                    carry = sum4 / 10;
                }

                for (; j < b.Count; j++)
                {
                    int sum = resultArray[i + j] + a[i] * b[j] + carry;
                    resultArray[i + j] = sum % 10;
                    carry = sum / 10;
                }

                if (carry > 0)
                    resultArray[i + j] = carry;
            }

            var result = new List<int>(resultArray);
            Trim(result);
            return result;
        }
        private static List<int> SimpleMultiply(int a, int b)
        {
            int product = a * b;
            var res = new List<int> { product % 10 };
            if (product >= 10) res.Add(product / 10);
            return res;
        }

        private static List<int> ShiftLeft(List<int> digits, int n)
        {
            var result = new List<int>(new int[n]);
            result.AddRange(digits);
            return result;
        }

        private static void Trim(List<int> digits)
        {
            while (digits.Count > 1 && digits[digits.Count - 1] == 0)
                digits.RemoveAt(digits.Count - 1);
        }

        private static int CompareMagnitude(List<int> a, List<int> b)
        {
            if (a.Count != b.Count)
                return a.Count.CompareTo(b.Count);

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i])
                    return a[i].CompareTo(b[i]);
            }

            return 0;
        }
        private static List<int> Reverse(List<int> digits)
        {
            var reversed = new List<int>(digits);
            reversed.Reverse();
            return reversed;
        }
        
    }
}