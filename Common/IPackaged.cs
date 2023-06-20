using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p2pchat.Common
{
    public interface IPackaged
    {
        long id { get; set; }
        string typename { get; set; }
    }
}
