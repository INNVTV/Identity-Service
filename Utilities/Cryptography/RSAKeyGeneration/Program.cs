using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RSAKeyGeneration
{
    class Program
    {
        private const int keySize = 2048;

        static void Main(string[] args)
        {
            // Generate Private/Public Key Pair
            var rsaKeys = GenerateRsaKeys();

            // Print XML String Formats:
            Console.WriteLine("============================");
            Console.WriteLine("   XML STRING FORMAT(S):");
            Console.WriteLine("============================");
            Console.WriteLine();
            Console.WriteLine("PUBLIC_KEY:");
            Console.WriteLine();
            Console.WriteLine(rsaKeys.PublicKeyXmlString);
            Console.WriteLine();

            Console.WriteLine("PRIVATE_KEY:");
            Console.WriteLine();
            Console.WriteLine(rsaKeys.PrivateAndPublicKeysXmlString);
            Console.WriteLine();


            // Print PEM Formats:
            var pemPublicKey = ExportPublicKey(rsaKeys.PublicKeyParameters);
            var pemPrivateKey = ExportPrivateKey(rsaKeys.PrivateKeyParameters);

            Console.WriteLine("============================");
            Console.WriteLine("   PEM FORMAT(S):");
            Console.WriteLine("============================");
            Console.WriteLine();
            Console.WriteLine(pemPublicKey);
            Console.WriteLine();
            Console.WriteLine(pemPrivateKey);

            Console.ReadLine();
        }

        private static RsaKeyGenerationResult GenerateRsaKeys()
        {
            //Uses: using System.Security.Cryptography;

            RSACryptoServiceProvider myRSA = new RSACryptoServiceProvider(keySize);
            //RSAParameters publicKey = myRSA.ExportParameters(true);

            #region RSA Algorithm, XML Structure and Properties

            /* RSA Algorithm and  XML Structure
            -------------------------------
            RSA Algorithm
            To generate a key pair, you start by creating two large prime numbers named p and q.
            These numbers are multiplied and the result is called n.
            Because p and q are both prime numbers, the only factors of n are 1, p, q, and n.
            -------------------------------


            <RSAKeyValue>
               <Modulus>…</Modulus>
               <Exponent>…</Exponent>
               <P>…</P> 
               <Q>…</Q>
               <DP>…</DP>
               <DQ>…</DQ>
               <InverseQ>…</InverseQ>
               <D>…</D>
            </RSAKeyValue>

            SUMMARY OF FIELDS

            D	        ==          d, the private exponent	privateExponent
            DP	        ==          d mod (p - 1)	exponent1
            DQ	        ==          d mod (q - 1)	exponent2
            Exponent    ==	        e, the public exponent	publicExponent
            InverseQ    ==	        (InverseQ)(q) = 1 mod p	coefficient
            Modulus     ==	        n	modulus
            P           ==  	    p	prime1
            Q	        ==          q	prime2


            The security of RSA derives from the fact that, given the public key { e, n }, it is computationally infeasible to calculate d, either directly or by factoring n into p and q. Therefore, any part of the key related to d, p, or q must be kept secret. If you call

            ExportParameters and ask for only the public key information, this is why you will receive only Exponent and Modulus. The other fields are available only if you have access to the private key, and you request it.

            */

            #endregion

            // Note: Requires the RSACryptoServiceProviderExtensions.cs file on project root (ToXMLString(true/flase) does not work in .Net Core so we have an extention method that parses pub/priv without boolean flag)

            var result = new RsaKeyGenerationResult
            {
                PrivateKeyParameters = myRSA.ExportParameters(true),
                PublicKeyParameters = myRSA.ExportParameters(false),

                PublicKeyXmlString = myRSA.ToXmlRsaString_PublicOnly(),
                PrivateAndPublicKeysXmlString = myRSA.ToXmlRsaString_Full(),

                //PrivateBase64Key = "",
                //PublicBase64Key = ""

            };

            return result;

        }

        public class RsaKeyGenerationResult
        {
            //public string PublicBase64Key { get; set; } //<-- <Modulus> - ????
            //public string PrivateBase64Key { get; set; } //-- <D> - ????

            // Used by signing and verification functions
            public RSAParameters PrivateKeyParameters { get; set; }
            public RSAParameters PublicKeyParameters { get; set; }

            public string PublicKeyXmlString { get; set; } //<-- Just the <Modulus> and <Exponent>
            public string PrivateAndPublicKeysXmlString { get; set; } //<-- All XML fields
        }


        #region PEM Format Exports

        private static string ExportPrivateKey(RSAParameters parameters)
        {
            var results = new StringBuilder();

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, parameters.D);
                    EncodeIntegerBigEndian(innerWriter, parameters.P);
                    EncodeIntegerBigEndian(innerWriter, parameters.Q);
                    EncodeIntegerBigEndian(innerWriter, parameters.DP);
                    EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                results.Append("-----BEGIN RSA PRIVATE KEY-----");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    results.Append(base64, i, Math.Min(64, base64.Length - i));
                }
                results.Append("-----END RSA PRIVATE KEY-----");
            }

            return results.ToString();
        }

        private static string ExportPublicKey(RSAParameters parameters)
        {
            var results = new StringBuilder();

            //var parameters = csp.ExportParameters(false);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    innerWriter.Write((byte)0x30); // SEQUENCE
                    EncodeLength(innerWriter, 13);
                    innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                    var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                    EncodeLength(innerWriter, rsaEncryptionOid.Length);
                    innerWriter.Write(rsaEncryptionOid);
                    innerWriter.Write((byte)0x05); // NULL
                    EncodeLength(innerWriter, 0);
                    innerWriter.Write((byte)0x03); // BIT STRING
                    using (var bitStringStream = new MemoryStream())
                    {
                        var bitStringWriter = new BinaryWriter(bitStringStream);
                        bitStringWriter.Write((byte)0x00); // # of unused bits
                        bitStringWriter.Write((byte)0x30); // SEQUENCE
                        using (var paramsStream = new MemoryStream())
                        {
                            var paramsWriter = new BinaryWriter(paramsStream);
                            EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                            EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                            var paramsLength = (int)paramsStream.Length;
                            EncodeLength(bitStringWriter, paramsLength);
                            bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                        }
                        var bitStringLength = (int)bitStringStream.Length;
                        EncodeLength(innerWriter, bitStringLength);
                        innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                    }
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                results.Append("-----BEGIN PUBLIC KEY-----");
                for (var i = 0; i < base64.Length; i += 64)
                {
                    results.Append(base64, i, Math.Min(64, base64.Length - i));
                }
                results.Append("-----END PUBLIC KEY-----");

                return results.ToString();
            }
        }

        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        #endregion

    }
}
