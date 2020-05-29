using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Vueling.Common.Core.Log;
using Vueling.Common.DomainServices;

namespace Vueling.Business.HttpClientHelper
{
    public interface IVuelingHttpClientHelper
    {
        IEnumerable<T> GetFromResource<T>(string url)
            where T : DtoBase;
    }

    public class VuelingHttpClientHelper : IVuelingHttpClientHelper
    {
        #region Public_Methods

        public IEnumerable<T> GetFromResource<T>(string url)
            where T : DtoBase
        {
            Logger.AddLOGMsg($"VuelingHttpClientHelper GET {url}");
            IEnumerable<T> result = new List<T>();
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                    result = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                Logger.AddLOGMsg($"VuelingHttpClientHelper GET {url}. StatusCode {response.StatusCode}");
            }
            catch (Exception ex) { 
                    Logger.AddLOGMsg($"VuelingHttpClientHelper GET {url}. Error {ex.Message}");
            }
            return result;
        }

        #endregion

    }
    
}
