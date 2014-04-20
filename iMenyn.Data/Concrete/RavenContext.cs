using System;
using iMenyn.Data.Abstract;
using iMenyn.Data.Infrastructure.Index;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace iMenyn.Data.Concrete
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
            //_documentStore = new DocumentStore
            //{
            //    Url = "https://ibis.ravenhq.com/databases/Jesse-imenyn",
            //    ApiKey = "5dce1a3a-9a16-4163-bc8d-74afb1135f40"
            //};
            _documentStore = new DocumentStore
            {
                ConnectionStringName = "imenyn",
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