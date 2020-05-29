using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using Ninject;
using Vueling.Business.DomainServices.Modules.Executive;
using Vueling.Common.Core.Log;
using Vueling.Presentation.Api.Filters;

namespace Vueling.Presentation.Api.Controllers
{
    [VuelingWebApiFilter]
    public class TransactionsController : ApiController
    {
        #region Public Properties

        [Inject]
        public ITransactionsDomainServices _transactionsDomainServices { get; set; }

        #endregion

        // GET api/Transactions
        public HttpResponseMessage Get()
        {
            Logger.AddLOGMsg("GET api/transactions");

            var transactions = _transactionsDomainServices.GetAll();
            var json = JsonConvert.SerializeObject(transactions);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }
        
        // GET api/Transactions/GetBySku
        [HttpGet]
        public HttpResponseMessage GetBySku(string sku)
        {
            Logger.AddLOGMsg($"GET api/transactions/getBySku/{sku}");

            dynamic transactionsWithTotal = new ExpandoObject();
            var tuple = _transactionsDomainServices.GetBySku(sku);
            transactionsWithTotal.sku = sku;
            transactionsWithTotal.transactions = tuple.Item1;
            transactionsWithTotal.total = tuple.Item2;

            var json = JsonConvert.SerializeObject(transactionsWithTotal);
            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(json)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return resp;
        }
    }
}
