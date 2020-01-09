using System;
using System.Collections.Generic;
using System.Text;

namespace HomeM8
{
    public class EstablishSharedSecretResponseModel : BaseResponseModel
    {
        public string ECDHPublicKeyBase64 { get; set; }

        public string ECDHSignedPublicKeyBase64_RSA { get; set; }
    }
}
