using Hyperai.Relations;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HyperaiShell.Foundation.ModelExtensions
{
    public static class AttachmentExtensions
    {
        private static readonly IAttachmentService service;

        static AttachmentExtensions()
        {
            service = Shared.Application.Provider.GetRequiredService<IAttachmentService>();
        }

        public static void Attach<T>(this RelationModel model, T ins)
        {
            service.Attach<T>(ins, model);
        }

        public static void Detach<T>(this RelationModel model)
        {
            service.Detach<T>(model);
        }

        public static T Retrieve<T>(this RelationModel model, Func<T> generator = null)
        {
            generator = generator ?? new Func<T>(() => default(T));
            T t = service.Retrieve<T>(model);
            if (t != null)
            {
                return t;
            }
            else
            {
                t = generator();
                if (t == null)
                {
                    return default(T);
                }
                else
                {
                    service.Attach(t, model);
                    return t;
                }
            }
        }

        public static ForAttachmentUpdateScope<T> For<T>(this RelationModel model, out T ins, Func<T> generator = null)
        {
            return service.For<T>(model, out ins, generator);
        }
    }
}