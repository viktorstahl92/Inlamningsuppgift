using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Inlamningsuppgift.Entities
{
    public class UserEntity
    {

        [Key]
        public int UserId { get; set; }

        [Required, Column(TypeName = "nvarchar(40)")]
        public string FirstName { get; set; } = null!;

        [Required, Column(TypeName = "nvarchar(40)")]
        public string LastName { get; set; } = null!;

        [Required, Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; } = null!;

        [Required, Column(TypeName = "nvarchar(100)")]
        public string Address { get; set; } = null!;

        [Required, Column(TypeName = "nvarchar(50)")]
        public string City { get; set; } = null!;

        [Required, Column(TypeName = "nvarchar(12)")]
        public string PostalCode { get; set; } = null!;

        [Required]
        public byte[] PasswordHash { get; private set; }

        [Required]
        public byte[] Salt { get; private set; }

        public void CreateSecurePassword(string password)
        {
            using var hmac = new HMACSHA512();
            Salt = hmac.Key;
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            hmac.Clear();
        }

        public bool CompareSecurePassword(string password)
        {
            using (var hmac = new HMACSHA512(Salt))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != PasswordHash[i])
                        return false;
                }
            }

            return true;
        }

    }
}
