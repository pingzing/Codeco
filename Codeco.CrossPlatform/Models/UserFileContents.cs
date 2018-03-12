using System;

namespace Codeco.CrossPlatform.Models
{
    public class UserFileContents
    {
        public Guid FileId { get; set; }        
        public string EncryptedUserKeyValues { get; set; }
        public string EncryptionSalt { get; set; }
        public string EncryptionIV { get; set; }
    }
}
