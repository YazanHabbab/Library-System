using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_access.Models
{
    public class UpdatedUser
    {
        public string? Name { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}