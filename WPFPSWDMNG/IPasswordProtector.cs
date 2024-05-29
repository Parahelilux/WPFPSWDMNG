using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPSWDMNG
{
    public interface IPasswordProtector
    {
        string EncryptPassword(string password);
        string DecryptPassword(string encryptedPassword);
    }
}
