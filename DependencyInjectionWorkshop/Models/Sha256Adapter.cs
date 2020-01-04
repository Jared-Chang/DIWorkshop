using System.Linq;
using System.Text;

namespace DependencyInjectionWorkshop.Models
{
    public interface IHash
    {
        string ComputeHash(string password);
    }

    public class Sha256Adapter : IHash
    {
        public Sha256Adapter()
        {
        }

        public string ComputeHash(string password)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hashedPassword = crypt.ComputeHash(Encoding.UTF8.GetBytes(password))
                .Aggregate(new StringBuilder(), (builder, theByte) => builder.Append(theByte))
                .ToString();

            return hashedPassword;
        }
    }
}