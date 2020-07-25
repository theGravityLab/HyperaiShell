using LiteDB;
using System;
using System.Linq;

namespace HyperaiShell.App.Data
{
    public class SearchingTypeNameBinder : ITypeNameBinder
    {
        public string GetName(Type type)
        {
            return $"{type.FullName},{type.Assembly.GetName().Name}";
        }

        public Type GetType(string name)
        {
            string typeName = name.Substring(0, name.IndexOf(','));
            string assName = name.Substring(typeName.Length + 1);
            return AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == assName).GetType(typeName);
        }
    }
}