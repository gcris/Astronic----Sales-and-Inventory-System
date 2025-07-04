using CommonLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.Interfaces
{
    public interface ICryptologyRepository
    {
        string EncryptString(CryptographyDtos cryptographyDtos);
        string DecryptString(CryptographyDtos cryptographyDtos);
    }
}
