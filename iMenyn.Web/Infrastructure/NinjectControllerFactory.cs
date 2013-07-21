using System;
using System.Web.Mvc;
using Ninject;
using iMenyn.Data.Infrastructure;

namespace iMenyn.Web.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _ninjectKernel;

        public NinjectControllerFactory()
        {
            _ninjectKernel = DependencyManager.NinjectKernel;
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null
                       ? null
                       : (IController)_ninjectKernel.Get(controllerType);
        }
    }
}