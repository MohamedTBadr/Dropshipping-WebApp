using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException(Guid id)
            : base($"Category with ID '{id}' was not found.")
        {
        }
    }
}
