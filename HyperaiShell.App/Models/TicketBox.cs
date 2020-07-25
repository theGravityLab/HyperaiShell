using HyperaiShell.Foundation.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace HyperaiShell.App.Models
{
    public class TicketBox
    {
        public List<TicketBase> Tickets { get; set; } = new List<TicketBase>();

        public bool Check(string name)
        {
            IEnumerable<TicketBase> mani = Tickets.Where(x => x.Pattern.Match(name).Success);
            bool veri = false;
            LinkedList<TicketBase> diposedTickets = new LinkedList<TicketBase>();
            foreach (TicketBase ticket in mani)
            {
                veri = ticket.Verify() || veri; // 不可短路
                if (!veri)
                {
                    diposedTickets.AddLast(ticket);
                }
            }
            foreach (TicketBase ticket in diposedTickets)
            {
                Tickets.Remove(ticket);
            }
            return veri;
        }

        public void Put(TicketBase ticket)
        {
            Tickets.Add(ticket);
        }

        public void Remove(TicketBase ticket)
        {
            if (Tickets.Contains(ticket))
            {
                Tickets.Remove(ticket);
            }
        }

        public TicketBase FindSpecificTicket(string ticketName)
        {
            return Tickets.Where(x => x.Name == ticketName).FirstOrDefault();
        }

        public IEnumerable<TicketBase> GetTickets()
        {
            return Tickets;
        }
    }
}