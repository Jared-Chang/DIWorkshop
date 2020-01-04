using System.Linq;
using System.Text;

namespace DependencyInjectionWorkshop.Models
{
    public class Sha256Adapter
    {
        public Sha256Adapter()
        {
        }

        public string HashedPassword(string password)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hashedPassword = crypt.ComputeHash(Encoding.UTF8.GetBytes(password))
                .Aggregate(new StringBuilder(), (builder, theByte) => builder.Append(theByte))
                .ToString();

            return hashedPassword;
        }
    }
}