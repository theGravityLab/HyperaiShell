using Hyperai.Relations;
using System;

namespace HyperaiShell.Foundation.Services
{
    public class ForAttachmentUpdateScope<T> : IDisposable
    {
        private readonly IAttachmentService _service;
        private readonly T _instance;
        private readonly RelationModel _toWhom;

        public ForAttachmentUpdateScope(IAttachmentService service, T instance, RelationModel toWhom)
        {
            _service = service;
            _instance = instance;
            _toWhom = toWhom;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private bool isDisposed = false;

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed && isDisposing)
            {
                if (_instance != null)
                {
                    _service.Attach(_instance, _toWhom);
                }
                isDisposed = true;
            }
        }
    }
}