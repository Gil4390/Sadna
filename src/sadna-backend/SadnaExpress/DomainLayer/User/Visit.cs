using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.User
{
    public class Visit
    {
        [Key]
        public Guid UniqueID { get; set; }
        public Guid UserID { get; set; }

        // types of Role see getRole() function in user, member, promotedMember Class
        // 1. Guest -> אורחים
        // 2. Member -> משתמשים שאינם מנהלי או בעלי חנות
        // 3. PromotedMember,Store Manager -> מנהלי חנות שאינם בעלים של אף חנות
        // 4. PromotedMember,Store Owner -> בעלי חנות 
        // 5. SystemManager -> מנהל מערכת
        public string Role { get; set; }
        public string VisitDate { get; set; } // format: DD/MM/YYYY
    }
}
