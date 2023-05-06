using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer.SModels
{
    public class SMember
    {
        private Guid id;
        private string email;
        private string firstName;
        private string lastName;
        private bool loggedIn;
        private List<string> permissions;

        public SMember(Member member)
        {
            this.id = member.UserId;
            this.email = member.Email;
            this.firstName = member.FirstName;
            this.lastName = member.LastName;
            this.loggedIn = member.LoggedIn;


            this.permissions = new List<string>();

            if (member is PromotedMember)
            {
                PromotedMember pmember = (PromotedMember)member;

                foreach (Guid storeID in pmember.Permission.Keys)
                {
                    if (!storeID.Equals(Guid.Empty))
                    {
                        this.permissions.Add(storeID.ToString() + ": ");
                    }
                    this.permissions.AddRange(pmember.Permission[storeID]);
                }

            }
        }

        public Guid Id { get => id; set => id = value; }
        public string Email { get => email; set => email = value; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }
        public List<string> Permissions { get => permissions; set => permissions = value; }
    }
}
