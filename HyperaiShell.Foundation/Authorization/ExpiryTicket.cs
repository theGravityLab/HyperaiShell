using System;

namespace HyperaiShell.Foundation.Authorization
{
    public class ExpiryTicket : TicketBase
    {
        public DateTime ExpirationDate { get; private set; }

        public ExpiryTicket(string name, DateTime expiration) : base(name)
        {
            ExpirationDate = expiration;
        }

        public override bool Verify()
        {
            return ExpirationDate >= DateTime.Now;
        }
    }
}