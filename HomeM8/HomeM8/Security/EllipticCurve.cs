using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HomeM8
{
    public class EllipticCurve
    {
        public EllipticCurve(bool fromRegister)
        {
            FromRegister = fromRegister;
        }

        public bool FromRegister { get; set; }

        public bool IsSucceeded { get; set; }

        public byte[] SharedSecret { get; set; }

        public EstablishSharedSecretResponseModel Response { get; set; }

        public KeyValuePair<byte[],EstablishSharedSecretResponseModel> KeyExchange(string username)
        {
            X9ECParameters x9EC = NistNamedCurves.GetByName("P-521");
            ECDomainParameters ecDomain = new ECDomainParameters(x9EC.Curve, x9EC.G, x9EC.N, x9EC.H, x9EC.GetSeed());
            AsymmetricCipherKeyPair aliceKeyPair = GenerateKeyPair(ecDomain);

            ECPublicKeyParameters alicePublicKey = (ECPublicKeyParameters)aliceKeyPair.Public;
            KeyValuePair<ECPublicKeyParameters, EstablishSharedSecretResponseModel> bobPublicKey = GetServerPublicKey(username, x9EC, alicePublicKey);

            if (bobPublicKey.Key == null)
            {
                Response = bobPublicKey.Value;
                return new KeyValuePair<byte[], EstablishSharedSecretResponseModel>(null, bobPublicKey.Value);
            }

            byte[] AESKey = GenerateAESKey(bobPublicKey.Key, aliceKeyPair.Private);

            IsSucceeded = AESKey != null && bobPublicKey.Value.responseVal == 0;

            if (IsSucceeded)
            {
                SharedSecret = AESKey;
                Response = bobPublicKey.Value;
            }

            return new KeyValuePair<byte[], EstablishSharedSecretResponseModel>(AESKey, bobPublicKey.Value);
        }

        private byte[] GenerateAESKey(ECPublicKeyParameters bobPublicKey, AsymmetricKeyParameter alicePrivateKey)
        {
            IBasicAgreement aKeyAgree = AgreementUtilities.GetBasicAgreement("ECDH");
            aKeyAgree.Init(alicePrivateKey);
            BigInteger sharedSecret = aKeyAgree.CalculateAgreement(bobPublicKey);
            byte[] sharedSecretBytes = sharedSecret.ToByteArray();

            IDigest digest = new Sha256Digest();
            byte[] symmetricKey = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(sharedSecretBytes, 0, sharedSecretBytes.Length);
            digest.DoFinal(symmetricKey, 0);

            return symmetricKey;
        }

        private KeyValuePair<ECPublicKeyParameters,EstablishSharedSecretResponseModel> GetServerPublicKey(string username,X9ECParameters x9EC, ECPublicKeyParameters clientPublicKey)
        {
            KeyValuePair<KeyCoords,EstablishSharedSecretResponseModel> bobCoords = GetServerCoords(username, clientPublicKey);
            if (bobCoords.Key == null)
            {
                return new KeyValuePair<ECPublicKeyParameters, EstablishSharedSecretResponseModel>(null, bobCoords.Value);
            }
            var point = x9EC.Curve.CreatePoint(bobCoords.Key.X, bobCoords.Key.Y);
            return new KeyValuePair<ECPublicKeyParameters, EstablishSharedSecretResponseModel>(new ECPublicKeyParameters("ECDH", point, SecObjectIdentifiers.SecP521r1), bobCoords.Value);
        }

        private KeyValuePair<KeyCoords, EstablishSharedSecretResponseModel> GetServerCoords(string username, ECPublicKeyParameters clientPublicKey)
        {
            var parameters = new
            {
                publicKey = GetXmlString(clientPublicKey)
            };

            var response = JsonConvert
                .DeserializeObject<EstablishSharedSecretResponseModel>(Helper
                .httpPostAsync($"{Utility.BaseURL}/api/user/establishsharedsecret?username={username}&fromRegister={FromRegister}&p={Guid.NewGuid().ToString("N")}", JsonConvert.SerializeObject(parameters)).Result);

            string responseXmlBase64 = default(string);

            if (response.responseVal == 0)
            {
                responseXmlBase64 = response.ECDHPublicKeyBase64;

                var responseSignedXmlBase64 = response.ECDHSignedPublicKeyBase64_RSA;

                var validation = Utility.VerifyDataRSA(responseXmlBase64, responseSignedXmlBase64);

                if (validation)
                {
                    var responseXmlArray = Convert.FromBase64String(response.ECDHPublicKeyBase64);


                    var responseXml = new UTF8Encoding().GetString(responseXmlArray);

                    XmlDocument doc = new XmlDocument();

                    doc.LoadXml(responseXml);

                    XmlElement root = doc.DocumentElement;

                    XmlNodeList elemList = doc.DocumentElement.GetElementsByTagName("PublicKey");

                    return new KeyValuePair<KeyCoords, EstablishSharedSecretResponseModel>(new KeyCoords
                    {
                        X = new BigInteger(elemList[0].FirstChild.Attributes["Value"].Value),
                        Y = new BigInteger(elemList[0].LastChild.Attributes["Value"].Value)
                    }, response);
                }
                else
                {
                    return new KeyValuePair<KeyCoords, EstablishSharedSecretResponseModel>(null, response);
                }
            }
            return new KeyValuePair<KeyCoords, EstablishSharedSecretResponseModel>(null, response);
        }

        private string GetXmlString(ECPublicKeyParameters clientPublicKey)
        {
            string publicKeyXmlTemplate = @"<ECDHKeyValue xmlns=""http://www.w3.org/2001/04/xmldsig-more#""> <DomainParameters> <NamedCurve URN=""urn:oid:1.3.132.0.35"" /> </DomainParameters> <PublicKey> <X Value=""X_VALUE"" xsi:type=""PrimeFieldElemType"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" /> <Y Value=""Y_VALUE"" xsi:type=""PrimeFieldElemType"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" /> </PublicKey> </ECDHKeyValue>";
            string xml = publicKeyXmlTemplate;
            xml = xml.Replace("X_VALUE", clientPublicKey.Q.AffineXCoord.ToBigInteger().ToString());
            xml = xml.Replace("Y_VALUE", clientPublicKey.Q.AffineYCoord.ToBigInteger().ToString());
            return xml;
        }

        private AsymmetricCipherKeyPair GenerateKeyPair(ECDomainParameters ecDomain)
        {
            ECKeyPairGenerator g = (ECKeyPairGenerator)GeneratorUtilities.GetKeyPairGenerator("ECDH");
            g.Init(new ECKeyGenerationParameters(ecDomain, new SecureRandom()));

            AsymmetricCipherKeyPair aliceKeyPair = g.GenerateKeyPair();
            return aliceKeyPair;
        }

        internal class KeyCoords
        {
            public BigInteger X { get; set; }
            public BigInteger Y { get; set; }
        }
    }
}
