using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SadnaExpress.DomainLayer.Store.Policy
{
    public class PolicyDB
    {
        [Key]
        public Guid UniqueID { get; set; }

        public int ID { get; set; }
        public string Discriminator { get; set; }

        public Guid StoreId { get; set; }

        public string simple_level { get; set; }

        public int simple_percent { get; set; }
        public DateTime simple_startDate { get; set; }
        public DateTime simple_endDate { get; set; }

        public bool activated { get; set; }

        public string complex_op { get; set; }

        public int[] complex_policys { get; set; }


    }
}
