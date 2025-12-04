using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace BAL.Exceptions
{
    public class GeneralNotFoundException : Exception
    {
        public GeneralNotFoundException(Guid id, string entityName)
            : base($"{entityName} with Id: {id} was not found.")
        { }
    }
}

