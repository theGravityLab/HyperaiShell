using System;
using System.Collections.Generic;
using Ac682.Extensions.Logging.Console;
using Hyperai.Relations;

namespace HyperaiShell.App.Logging.ConsoleFormatters
{
    public class RelationFormatter: IObjectLoggingFormatter
    {
        public bool IsTypeAvailable(Type type)
        {
            return type.IsAssignableTo(typeof(RelationModel));
        }

        public IEnumerable<ColoredUnit> Format(object obj, Type type)
        {
            return new[]
            {
                obj switch
                {
                    Friend friend => new ColoredUnit(friend.Nickname, foreground: ConsoleColor.DarkMagenta),
                    Member member => new ColoredUnit(member.DisplayName, foreground: ConsoleColor.DarkMagenta),
                    Group group => new ColoredUnit(group.Name, foreground: ConsoleColor.DarkMagenta),
                    _ => new ColoredUnit(((RelationModel) obj).Identity.ToString())
                },
                new ColoredUnit("(", foreground: ConsoleColor.DarkGray),
                new ColoredUnit(((RelationModel) obj).Identifier, foreground:ConsoleColor.Red),
                new ColoredUnit(")", foreground: ConsoleColor.DarkGray)
            };
        }
    }
}
