using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.User
{
   
    public class InitializeSystem
    {
        [Key]
        public int Id { get; set; }
        public bool IsInit { get; set; }

        public InitializeSystem()
        {
            
        }
    }
}
