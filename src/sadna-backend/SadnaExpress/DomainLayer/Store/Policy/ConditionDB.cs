using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store.Policy
{
    public class ConditionDB
    {
        [Key]
        public Guid UniqueID { get; set; }
        public int ID { get; set; }
        public Guid StoreID { get; set; }
        public string EntityStr { get; set; }
        public string EntityName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTime Dt { get; set; }
        public string Op { get; set; }
        public int OpCond { get; set; }
    }
}
