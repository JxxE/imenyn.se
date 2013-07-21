using Ninject;
using iMenyn.Data.Abstract;
using iMenyn.Data.Concrete;

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

        private static IRavenDbContext _ravenDbContext;

        public static void InjectRavenDbContext(IRavenDbContext ravenDbContext)
        {
            _ravenDbContext = ravenDbContext;
            NinjectKernel.Rebind<IRavenDbContext>().ToConstant(ravenDbContext);
            NinjectKernel.Rebind<IRepository>().To<Repository>();
        }

        //Add all bindings here
        private void AddBindings()
        {
            _ninjectKernel.Bind<IRepository>().To<Repository>();
            _ninjectKernel.Bind<IRavenDbContext>().ToConstant(RavenContext.Instance);
        }

        public static T GetInstance<T>()
        {
            return NinjectKernel.Get<T>();
        }

        public static IRepository Repository
        {
            get { return GetInstance<IRepository>(); }
        }

        public static IAuthentication Authentication
        {
            get { return GetInstance<IAuthentication>(); }
        }
    }
}