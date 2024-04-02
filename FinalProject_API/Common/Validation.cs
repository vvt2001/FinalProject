namespace FinalProject_API.Common
{
    public class PasswordValidator
    {
        public int MinLength { get; set; } = 6;
        public bool RequireDigit { get; set; } = false;
        public bool RequireLowercase { get; set; } = false;
        public bool RequireUppercase { get; set; } = false;
        public bool RequireNonLetterOrDigit { get; set; } = false;
        public PasswordValidator()
        {

        }
        public PasswordValidator(int minLength, bool requireDigit, bool requireLowercase, bool requireUppercase, bool requireNonLetterOrDigit)
        {
            MinLength = minLength;
            RequireDigit = requireDigit;
            RequireLowercase = requireLowercase;
            RequireUppercase = requireUppercase;
            RequireNonLetterOrDigit = requireNonLetterOrDigit;
        }

        public bool Validate(string password)
        {
            // Check minimum length
            if (password.Length < MinLength)
            {
                return false;
            }

            // Check for digit if required
            if (RequireDigit && !ContainsDigit(password))
            {
                return false;
            }

            // Check for lowercase letter if required
            if (RequireLowercase && !ContainsLowercase(password))
            {
                return false;
            }

            // Check for uppercase letter if required
            if (RequireUppercase && !ContainsUppercase(password))
            {
                return false;
            }

            // Check for non-letter or digit character if required
            if (RequireNonLetterOrDigit && !ContainsNonLetterOrDigit(password))
            {
                return false;
            }

            return true;
        }

        static bool ContainsDigit(string value)
        {
            return System.Linq.Enumerable.Any(value, char.IsDigit);
        }

        static bool ContainsLowercase(string value)
        {
            return System.Linq.Enumerable.Any(value, char.IsLower);
        }

        static bool ContainsUppercase(string value)
        {
            return System.Linq.Enumerable.Any(value, char.IsUpper);
        }

        static bool ContainsNonLetterOrDigit(string value)
        {
            return System.Linq.Enumerable.Any(value, c => !char.IsLetterOrDigit(c));
        }
    }
}

