using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JewelleryStore.HelperMethods
{
    public class PasswordHelper
    {
        public static bool VerifyPassword(string passwordText, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(passwordText, passwordHash, false, BCrypt.Net.HashType.SHA256);
        }
        public static string HashPassword(string passwordText)
        {
            return BCrypt.Net.BCrypt.HashPassword(passwordText);
        }
    }
}
