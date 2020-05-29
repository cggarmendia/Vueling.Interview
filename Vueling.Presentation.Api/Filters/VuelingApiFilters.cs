using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Ninject;
using Vueling.Common.DataAccess.EF.UnitOfWork;

namespace Vueling.Presentation.Api.Filters
{
    public class VuelingWebApiFilter : ActionFilterAttribute
    {
        [Inject]
        public IUnitOfWorkManager uowManager { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var uow = uowManager.GetUoW();

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var uow = uowManager.GetUoW();

            uow.Dispose();

            base.OnActionExecuted(actionExecutedContext);
        }
    }
    
}