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
                //DefaultDatabase = "Rantup",
                //Url = "http://localhost:8080"
                //Url = "https://ibis.ravenhq.com/databases/AppHarbor_2a23d4e2-9ba7-458d-82f1-ca1c0afa53b5",
                //ApiKey = "aeabbbbe-139e-4312-85d8-6a9986b30f47"
                Url = "https://ec2-eu4.cloudbird.net/databases/8c7359fa-4d38-412e-90a5-984077a133c6.iMenyn",
                ApiKey = "c3b5f4ec-8edc-42c6-ab7e-dd8e892d0ca8"
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