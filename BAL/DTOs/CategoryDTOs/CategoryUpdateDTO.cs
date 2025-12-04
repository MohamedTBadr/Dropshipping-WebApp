using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BAL.DTOs.CategoryDTOs
{
    public class CategoryUpdateDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
