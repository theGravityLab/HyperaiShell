using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HyperaiShell.App.Data
{
    public class LiteDbQueryable<T> : IQueryable<T>
    {
        public readonly ILiteQueryable<T> _queryable;

        public LiteDbQueryable(ILiteQueryable<T> queryable)
        {
            _queryable = queryable;
            Expression = Expression.Constant(queryable);
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; private set; }

        public IQueryProvider Provider => throw new NotImplementedException();

        public IEnumerator<T> GetEnumerator()
        {
            return _queryable.ToEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryable.ToEnumerable().GetEnumerator();
        }
    }
}