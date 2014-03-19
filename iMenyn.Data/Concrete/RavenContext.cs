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
            _documentStore = new DocumentStore
            {
                //DefaultDatabase = "iMenyn",
                //Url = "http://localhost:8080"
                //Url = "https://ibis.ravenhq.com/databases/AppHarbor_2a23d4e2-9ba7-458d-82f1-ca1c0afa53b5",
                //ApiKey = "aeabbbbe-139e-4312-85d8-6a9986b30f47"
                Url = "https://ibis.ravenhq.com/databases/Jesse-imenyn",
                ApiKey = "5dce1a3a-9a16-4163-bc8d-74afb1135f40"
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