using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Dtos
{
    public class CryptographyDtos
    {
        public string ToEncryptString { get; set; }
        public string CipherString { get; set; }
        public bool UseHashing { get; set; }
    }
}
