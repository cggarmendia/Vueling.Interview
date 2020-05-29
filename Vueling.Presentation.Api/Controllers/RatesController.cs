using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using Ninject;
using Vueling.Common.Core.Log;
using Vueling.Presentation.Api.Filters;
using Vueling.Business.DomainServices.Modules.Executive;

namespace Vueling.Presentation.Api.Controllers
{
    [VuelingWebApiFilter]
    public class RatesController : ApiController
    {
        #region Public Properties

        [Inject]
        public IRatesDomainServices _ratesDomainServices { get; set; }

        #endregion

        // GET api/Rates
        public HttpResponseMessage Get()
        {
            Logger.AddLOGMsg("GET api/rates");
            var rates = _ratesDomainServices.GetAll();
            var json = JsonConvert.SerializeObject(rates);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }
    }
}
