using bank.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank.Interfaces
{
    public interface IStringcheck<Input,Output>
    {
        Output getAllData(string hello);

    }
}
