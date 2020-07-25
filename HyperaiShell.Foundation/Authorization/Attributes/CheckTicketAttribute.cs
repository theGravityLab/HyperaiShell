using Hyperai.Units;
using Hyperai.Units.Attributes;
using HyperaiShell.Foundation.ModelExtensions;
using System.Collections.Generic;
using System.Linq;

namespace HyperaiShell.Foundation.Authorization.Attributes
{
    public class CheckTicketAttribute : FilterByAttribute
    {
        private const string message = "Permission Denied: ";

        public CheckTicketAttribute(string specificName) : base(new CheckTicketFilter(new string[] { specificName }), message + specificName)
        {
        }

        public CheckTicketAttribute(params string[] specificNames) : base(new CheckTicketFilter(specificNames), message + string.Join(',', specificNames))
        {
        }
    }

    internal class CheckTicketFilter : IFilter
    {
        private readonly IEnumerable<string> names;

        public CheckTicketFilter(IEnumerable<string> specificNames)
        {
            names = specificNames;
        }

        public bool Check(MessageContext context)
        {
            return names.Any(x => context.User.CheckPermission(x));
        }
    }
}