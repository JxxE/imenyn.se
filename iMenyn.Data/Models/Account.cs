using System;
using System.Security.Cryptography;
using System.Text;

namespace iMenyn.Data.Models
{
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsAdmin { get; set; }
        public bool Enabled { get; set; }
        public string Enterprise { get; set; }

        #region Password

        private const string ConstantSalt = "po8502jede13";
        protected string HashedPassword { get; private set; }

        private string _passwordSalt;
        private string PasswordSalt
        {
            get { return _passwordSalt ?? (_passwordSalt = Guid.NewGuid().ToString("N")); }
            set { _passwordSalt = value; }
        }

        public Account SetPassword(string pwd)
        {
            HashedPassword = GetHashedPassword(pwd);
            return this;
        }

        private string GetHashedPassword(string pwd)
        {
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(PasswordSalt + pwd + ConstantSalt));
                return Convert.ToBase64String(computedHash);
            }
        }

        public bool ValidatePassword(string maybePwd)
        {
            if (HashedPassword == null)
                return true;
            string maybe = GetHashedPassword(maybePwd);
            return HashedPassword == maybe;
        }
        #endregion
    }
}