using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.User
{
    public class Macs
    {
        [Key]
        public Guid id { get; set; }
        public string mac { get; set; }
    }
}
