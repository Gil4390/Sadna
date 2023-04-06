namespace SadnaExpress.DomainLayer.User
{
    public class Member : User
    {
        protected string email;
        public string Email {get => email; set => email = value;}
        protected string firstName;
        public string FirstName { get => firstName; set => firstName = value;}
            
        protected string lastName;
        public string LastName { get => lastName; set => lastName = value;}
        protected string password;
        public string Password { get => password; set => password = value; }
        private bool loggedIn;
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }

        public Member(int id, string memail, string mfirstName, string mlastLame, string mpassword): base (id)
        {
            userId = id;
            email = memail;
            firstName = mfirstName;
            lastName = mfirstName;
            password = mpassword;
        }

        
        
    }
}