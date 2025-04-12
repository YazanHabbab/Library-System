using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_access.Models
{
    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsAvailable { get; set; }
    }
}