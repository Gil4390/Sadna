using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.User
{
    public class Founder
    {
        [Key]
        public Guid StoreId { get; set; }
        public string PromotedMemberEmail { get; set; }
    }
}
