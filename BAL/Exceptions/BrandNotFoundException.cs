using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Exceptions
{
    public class BrandNotFoundException(Guid id):Exception($"Brand with ID : {id} Not Found")
    {
    }
}
