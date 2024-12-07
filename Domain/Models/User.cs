namespace Domain.Models
{
    public class User
    {
        public int Id { get; set; }

        private string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("UserName cannot be null or empty.", nameof(UserName));

                _userName = value;
            }
        }

        private string _passwordHash = string.Empty;
        public string PasswordHash
        {
            get => _passwordHash;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("PasswordHash cannot be null or empty.", nameof(PasswordHash));

                _passwordHash = value;
            }
        }

        public User(int id, string userName, string passwordHash)
        {
            if (id <= 0) throw new ArgumentException("Id must be a positive number.", nameof(id));
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("UserName cannot be null or empty.", nameof(userName));
            if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("PasswordHash cannot be null or empty.", nameof(passwordHash));

            Id = id;
            UserName = userName;
            PasswordHash = passwordHash;
        }

        public User() { }
    }
}
