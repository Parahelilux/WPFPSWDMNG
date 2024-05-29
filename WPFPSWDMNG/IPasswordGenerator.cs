using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPSWDMNG
{
    public interface IPasswordGenerator
    {
        string Generate(int length);
    }
}
