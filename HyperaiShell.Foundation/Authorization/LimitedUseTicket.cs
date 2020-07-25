namespace HyperaiShell.Foundation.Authorization
{
    public class LimitedUseTicket : TicketBase
    {
        public int Count { get; private set; }

        public LimitedUseTicket(string name, int count) : base(name)
        {
            Count = count;
        }

        public override bool Verify()
        {
            return Count-- > 0;
        }
    }
}