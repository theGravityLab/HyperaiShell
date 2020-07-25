using Hyperai.Events;
using HyperaiShell.Foundation.Bots;
using System.Threading.Tasks;

namespace HyperaiShell.Foundation.Services
{
    public interface IBotService
    {
        IBotCollectionBuilder Builder { get; }

        Task PushAsync(GenericEventArgs args);
    }
}