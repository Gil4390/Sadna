using System.Text.RegularExpressions;

namespace SadnaExpress.DomainLayer.User
{
    public class Registration : IRegistration
    {
        public bool ValidateStrongPassword(string pass)
        {
            // Strong password regex
            // The regular expression below checks that a password:
            //
            // Has minimum 8 characters in length. Adjust it by modifying {8,}
            // At least one uppercase English letter. You can remove this condition by removing (?=.*?[A-Z])
            // At least one lowercase English letter.  You can remove this condition by removing (?=.*?[a-z])
            // At least one digit. You can remove this condition by removing (?=.*?[0-9])
            // At least one special character,  You can remove this condition by removing (?=.*?[#?!@$%^&*-])
            
            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            return validateGuidRegex.IsMatch(pass);
        }

        public bool ValidateEmail(string email)
        {
            // This C# regular expression will match 99% of valid email addresses and will not pass validation for email addresses that have, for instance:
            //
            // Dots in the beginning
            // Multiple dots at the end
            // But at the same time it will allow part after @ to be IP address.
            Regex validateEmailRegex = new Regex("^\\S+@\\S+\\.\\S+$");
            return validateEmailRegex.IsMatch(email);
        }
    }
}