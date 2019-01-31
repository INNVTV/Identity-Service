using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace RSAKeyGeneration
{
    public static class RSACryptoServiceProviderExtensions
    {
        public static void FromXmlRsaString_Full(this RSACryptoServiceProvider rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

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

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                        case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
                        case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
                        case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
                        case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
                        case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        public static void FromXmlRsaString_PublicOnly(this RSACryptoServiceProvider rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

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

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        public static string ToXmlRsaString_Full(this RSACryptoServiceProvider rsa)
        {
            RSAParameters parameters = rsa.ExportParameters(true);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(parameters.Modulus),
                Convert.ToBase64String(parameters.Exponent),
                Convert.ToBase64String(parameters.P),
                Convert.ToBase64String(parameters.Q),
                Convert.ToBase64String(parameters.DP),
                Convert.ToBase64String(parameters.DQ),
                Convert.ToBase64String(parameters.InverseQ),
                Convert.ToBase64String(parameters.D));
        }

        public static string ToXmlRsaString_PublicOnly(this RSACryptoServiceProvider rsa)
        {
            RSAParameters parameters = rsa.ExportParameters(true);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(parameters.Modulus),
                Convert.ToBase64String(parameters.Exponent));
        }
    }
}
