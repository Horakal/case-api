using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiCase.Domain.Commons
{
    public class RefreshToken : Entity<Guid>
    {
        public RefreshToken()
        { }

        //Token, UserId, ExpiresAt, IsRevoked
        public string Token { get; private set; }

        public Guid UserId { get; private set; }

        public bool IsRevoked { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public RefreshToken(string token, Guid userId, bool isRevoked, DateTime expiresAt)
        {
            Token = token;
            UserId = userId;
            ExpiresAt = expiresAt;
            IsRevoked = isRevoked;
        }

        public bool IsExpired()
        {
            return this.ExpiresAt < DateTime.UtcNow;
        }
    }
}