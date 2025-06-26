using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BigIntegerProject
{
    public class RSA
    {
        private static BigInteger FastModularExponention(BigInteger baseNum, BigInteger exponent, BigInteger modulus)
        {
            BigInteger result = new BigInteger("1");
            BigInteger baseVal = baseNum;

            while (exponent.ToString() != "0")
            {
                if (!exponent.IsEven())
                {
                    result = BigInteger.Modulus(BigInteger.Multiply(result, baseVal), modulus);
                }

                baseVal = BigInteger.Modulus(BigInteger.Multiply(baseVal, baseVal), modulus);
                var (e, _) = BigInteger.Divide(exponent, new BigInteger("2"));
                exponent = e;
            }

            return result;
        }

        public static BigInteger EncryptDecrypt(BigInteger M, BigInteger e, BigInteger N)
        {
            BigInteger result = FastModularExponention(M, e, N);
            return result;
        }

       /* public static BigInteger Decrypt(BigInteger M, BigInteger d, BigInteger N)
        {
            BigInteger result = FastModularExponention(M, d, N);
            return N;
        }*/
    }
}