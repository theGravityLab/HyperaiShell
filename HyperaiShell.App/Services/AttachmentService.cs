﻿using Hyperai.Relations;
using HyperaiShell.App.Models;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.Logging;
using System;

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
                first = new Attachment
                {
                    Target = toWhom.Identifier,
                    TypeName = typeName,
                    Object = ins
                };
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
            string typeName = typeof(T).FullName;
            T ins = (T)_repository.Query<Attachment>().Where(x => x.Target == fromWhom.Identifier && x.TypeName == typeName).FirstOrDefault()?.Object;
            return ins;
        }

        public ForAttachmentUpdateScope<T> For<T>(RelationModel model, out T ins, Func<T> generator = null)
        {
            T t = Retrieve<T>(model) ?? (generator ?? new Func<T>(() => default))();
            ForAttachmentUpdateScope<T> scope = new ForAttachmentUpdateScope<T>(this, t, model);
            ins = t;
            return scope;
        }
    }
}