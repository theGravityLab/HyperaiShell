using Hyperai.Relations;
using HyperaiShell.App.Models;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace HyperaiShell.App.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public AttachmentService(IRepository repository, ILogger<AttachmentService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void Attach<T>(T ins, RelationModel toWhom)
        {
            string typeName = typeof(T).FullName;
            Attachment first = _repository.Query<Attachment>().Where(x => x.Target == toWhom.Identifier && x.TypeName == typeName).FirstOrDefault();
            if (first != null)
            {
                first.Object = ins;
                _repository.Update(first);
            }
            else
            {
                first = new Attachment();
                first.Target = toWhom.Identifier;
                first.TypeName = typeName;
                first.Object = ins;
                _repository.Store(first);
            }
        }

        public void Detach<T>(RelationModel toWhom)
        {
            string typeName = typeof(T).FullName;
            Attachment first = _repository.Query<Attachment>().Where(x => x.Target == toWhom.Identifier && x.TypeName == typeName).FirstOrDefault();
            if (first != null)
            {
                _repository.Delete<Attachment>(first.Id);
            }
        }

        public T Retrieve<T>(RelationModel fromWhom)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            string typeName = typeof(T).FullName;
            T ins = (T)_repository.Query<Attachment>().Where(x => x.Target == fromWhom.Identifier && x.TypeName == typeName).FirstOrDefault()?.Object;
            watch.Stop();
            _logger.LogDebug("Data query and deserilization took {} milliseconds for {}@{}", watch.ElapsedMilliseconds, typeof(T).Name, fromWhom.Identity);
            return ins;
        }

        public ForAttachmentUpdateScope<T> For<T>(RelationModel model, out T ins, Func<T> generator = null)
        {
            T t = Retrieve<T>(model) ?? (generator ?? new Func<T>(() => default(T)))();
            ForAttachmentUpdateScope<T> scope = new ForAttachmentUpdateScope<T>(this, t, model);
            ins = t;
            return scope;
        }
    }
}