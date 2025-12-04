using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Exceptions
{
    internal class ProductNotFoundException(Guid id):NotFoundException(id)
    {
    }

    public class NotFoundException(Guid id):Exception($"Product With Id : {id} Not Found");
}
