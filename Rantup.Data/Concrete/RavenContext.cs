using System;
using Rantup.Data.Abstract;
using Rantup.Data.Infrastructure.Index;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Rantup.Data.Concrete
{
    public class RavenContext : IRavenDbContext
    {
        private readonly static Lazy<RavenContext> _instance = new Lazy<RavenContext>(() => new RavenContext());

        private static IDocumentStore _documentStore;

        public static RavenContext Instance { get { return _instance.Value; } }

        public IDocumentStore DocumentStore
        {
            get
            {
                return _documentStore;
            }
        }

        private RavenContext()
        {
            InitDocumentStore();
        }

        private void InitDocumentStore()
        {
            _documentStore = new DocumentStore
            {
                DefaultDatabase = "Rantup",
                Url = "http://localhost:8080"
            };
            _documentStore.Conventions.IdentityPartsSeparator = "-";
            _documentStore.Initialize();
        }

        public void CreateAllIndexes()
        {
            IndexCreation.CreateIndexes(typeof(Enterprises).Assembly, _documentStore);
        }
    }
}