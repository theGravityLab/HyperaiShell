using LiteDB;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HyperaiShell.App.Data
{
    public class LiteDbQueryProvider<T> : IQueryProvider
    {
        private readonly LiteCollection<T> _collection;

        public LiteDbQueryProvider(LiteCollection<T> collection)
        {
            _collection = collection;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}