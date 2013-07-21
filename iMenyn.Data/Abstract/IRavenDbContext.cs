using Raven.Client;

namespace iMenyn.Data.Abstract
{
    public interface IRavenDbContext
    {
        IDocumentStore DocumentStore { get; }
    }
}
