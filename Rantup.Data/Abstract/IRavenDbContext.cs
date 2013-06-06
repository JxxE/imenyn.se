using Raven.Client;

namespace Rantup.Data.Abstract
{
    public interface IRavenDbContext
    {
        IDocumentStore DocumentStore { get; }
    }
}
