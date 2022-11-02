using System.Buffers.Text;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CapitalBroker
{
    public class Utils
    {
        public static byte[] stringToBytes(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            return bytes;
        }
        public static string encryptPassword(string encryptionKey, long timestamp, string password)
        {
            byte[] input = stringToBytes(password + "|" + timestamp);
            var keyBytes = stringToBytes(Convert.ToBase64String(input));
            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
            RSAParameters rsaParameters = new RSAParameters();
            rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);

            byte[] output = rsa.Encrypt(input, false);
            string cipherresult = Convert.ToBase64String(output);
            return cipherresult;
        }
    }
}
