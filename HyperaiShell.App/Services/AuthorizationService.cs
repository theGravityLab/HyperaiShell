﻿using Hyperai.Relations;
using HyperaiShell.App.Models;
using HyperaiShell.Foundation.Authorization;
using HyperaiShell.Foundation.ModelExtensions;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace HyperaiShell.App.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly string _daddy;
        public AuthorizationService(IConfiguration configuration)
        {
            _daddy = configuration["Application:Daddy"];

        }

        public bool CheckTicket(RelationModel model, string specificName)
        {
            if (specificName == "whosyourdaddy" && _daddy != null && _daddy == model.Identity.ToString())
            {
                return true;
            }

            TicketBox ticketBox = model.Retrieve<TicketBox>();
            if (ticketBox == null)
            {
                return false;
            }
            else
            {
                bool pass = ticketBox.Check(specificName);
                model.Attach(ticketBox);
                return pass;
            }
        }

        public void PutTicket(RelationModel model, TicketBase ticket)
        {
            TicketBox ticketBox = model.Retrieve<TicketBox>();
            if (ticketBox == null)
            {
                ticketBox = new TicketBox();
            }
            ticketBox.Put(ticket);
            model.Attach(ticketBox);
        }

        public void RemoveTicket(RelationModel model, string name)
        {
            TicketBox ticketBox = model.Retrieve<TicketBox>();
            if (ticketBox != null)
            {
                ticketBox.Remove(name);
            }
        }

        public IEnumerable<TicketBase> GetTickets(RelationModel model)
        {
            TicketBox ticketBox = model.Retrieve<TicketBox>();
            if (ticketBox != null)
            {
                return ticketBox.GetTickets();
            }
            return Enumerable.Empty<TicketBase>();
        }
    }
}