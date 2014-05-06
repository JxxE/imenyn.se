using Ninject;
using Raven.Client;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Concrete;
using iMenyn.Data.Concrete.Db;

namespace iMenyn.Data.Infrastructure
{
    public class DependencyManager
    {
        private readonly IKernel _ninjectKernel;

        public static IKernel NinjectKernel
        {
            get { return Instance._ninjectKernel; }
        }

        private readonly static DependencyManager Instance = new DependencyManager();

        private DependencyManager()
        {
            _ninjectKernel = new StandardKernel();
            AddBindings();
        }

        //Add all bindings here
        private void AddBindings()
        {
            _ninjectKernel.Bind<ILogger>().To<Logger>();

            BindDatabaseLayer();
        }

        private void BindDatabaseLayer()
        {
            // All IDb-bindings
            _ninjectKernel.Bind<IDbAccounts>().To<RavenDbAccounts>();
            _ninjectKernel.Bind<IDbEnterprises>().To<RavenDbEnterprises>();
            _ninjectKernel.Bind<IDbProducts>().To<RavenDbProducts>();
        }

        private static IRavenDbContext _ravenDbContext;

        public static void InjectRavenDbContext(IRavenDbContext ravenDbContext)
        {
            _ravenDbContext = ravenDbContext;
            NinjectKernel.Rebind<IRavenDbContext>().ToConstant(ravenDbContext);
            NinjectKernel.Rebind<IDb>().To<RavenDb>();
            NinjectKernel.Rebind<IDocumentStore>().ToConstant(ravenDbContext.DocumentStore);
        }

        public static T GetInstance<T>()
        {
            return NinjectKernel.Get<T>();
        }

        public static IDb Db
        {
            get { return GetInstance<IDb>(); }
        }

        public static IDocumentStore DocumentStore
        {
            get { return GetInstance<IDocumentStore>(); }
        }

        public static ILogger Logger
        {
            get { return GetInstance<ILogger>(); }
        }

        public static IAuthentication Authentication
        {
            get { return GetInstance<IAuthentication>(); }
        }
    }
}