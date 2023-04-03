namespace SadnaExpress.DomainLayer.User
{
    public class Member : User
    {
        protected string email;
        protected string firstName;
        protected string lastName;
        protected string password;

        public Member(int id, string memail, string mfirstName, string mlastLame, string mpassword): base (id)
        {
            userId = id;
            email = memail;
            firstName = mfirstName;
            lastName = mfirstName;
            password = mpassword;
        }

        public string Email
        {
            get => email;
            set => email = value;
        }
        public string FirstName
        {
            get => firstName;
            set => firstName = value;
        }
        public string LastName
        {
            get => lastName;
            set => lastName = value;
        }
        public string Password
        {
            get => password;
            set => password = value;
        }
    }
}