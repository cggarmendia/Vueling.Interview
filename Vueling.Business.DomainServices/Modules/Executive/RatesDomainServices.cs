using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Vueling.Business.DomainServices.Mapper;
using Vueling.Business.HttpClientHelper;
using Vueling.Common.Core.Log;
using Vueling.Common.DataAccess.EF.UnitOfWork;
using Vueling.Common.DomainServices;
using Vueling.Domain.Entities.Modules.Executives;

namespace Vueling.Business.DomainServices.Modules.Executive
{
    public interface IRatesDomainServices : IDtoDomainService<RatesDto>
    {
        IEnumerable<RatesDto> GetByFrom(string from);
        IEnumerable<string> HowToChangeFromOneCurrencyToAnother(string from, string to);
    }

    public class RatesDomainServices : DomainServiceBase<Rates>, IRatesDomainServices
    {
        #region Properties

        public IVuelingHttpClientHelper _vuelingHttpClientHelper { get; set; }

        public IUnitOfWorkManager _uowManager { get; set; }

        #endregion

        #region Ctor.

        public RatesDomainServices(IVuelingHttpClientHelper vuelingHttpClientHelper,
            IUnitOfWorkManager uowManager)
            : base()
        {
            _vuelingHttpClientHelper = vuelingHttpClientHelper;
            _uowManager = uowManager;
        }

        #endregion

        #region Public_Methods

        public IEnumerable<RatesDto> GetByFrom(string from)
        {
            Logger.AddLOGMsg($"Get By From: {from}");
            return GetDtosByFilters(u => u.From.Equals(from));
        }

        public RatesDto GetByIds(params object[] ids)
        {
            Logger.AddLOGMsg($"Get By Ids: {ids}");
            var domain = repository.GetByPKs(ids);
            return DomainServicesMapper.MapToRatesDto(domain);
        }

        public void Add(RatesDto dto)
        {
            var domain = new Rates();
            DomainServicesMapper.MapToRates(dto, domain);
            repository.Add(domain);
            Logger.AddLOGMsg($"Add Rates with From -> {dto.From} To -> {dto.To} Rate -> {dto.Rate}");
        }

        public void Update(RatesDto dto)
        {
            var domain = repository.GetByPKs(dto.From, dto.To);
            DomainServicesMapper.MapToRates(dto, domain);
            repository.Update(domain);
            Logger.AddLOGMsg($"Updated Rate with From -> {dto.From} To -> {dto.To} Rate -> {dto.Rate}");
        }

        public void Delete(RatesDto dto)
        {
            var domain = repository.GetByPKs(dto.From, dto.To);
            repository.Delete(domain);
        }

        public IEnumerable<RatesDto> GetAll()
        {
            Logger.AddLOGMsg("Get All Rates");
            CheckIfThereAreNewRates();
            return repository.GetAll().Select(DomainServicesMapper.MapToRatesDto).ToList();
        }

        public IEnumerable<string> HowToChangeFromOneCurrencyToAnother(string from, string to)
        {
            Logger.AddLOGMsg($"HowToChangeFromOneCurrencyToAnother From {from} To {to}");
            var howChangeToEuro = new List<string>();
            if (Exists(from, to))
                howChangeToEuro.Add(to);
            else if (ExistsFromOrTo(from) && ExistsFromOrTo(to))
                howChangeToEuro = ShortestPathDijkstra(from, to);
            return howChangeToEuro;
        }

        #endregion

        #region Private_Methods
        private RatesDto GetDtoByFilters(
            Expression<Func<Rates, bool>> filter = null,
            Func<IQueryable<Rates>, IOrderedQueryable<Rates>> orderBy = null,
            string includeProperties = null)
        {
            var domain = GetByFilters(filter, orderBy, includeProperties).SingleOrDefault();
            return DomainServicesMapper.MapToRatesDto(domain);
        }

        private IEnumerable<RatesDto> GetDtosByFilters(
            Expression<Func<Rates, bool>> filter = null,
            Func<IQueryable<Rates>, IOrderedQueryable<Rates>> orderBy = null,
            string includeProperties = null)
        {
            var domains = GetByFilters(filter, orderBy, includeProperties);
            return domains.Select(DomainServicesMapper.MapToRatesDto).ToList();
        }

        private void CheckIfThereAreNewRates()
        {
            var haveNew = false;
            var ratesFromResources = _vuelingHttpClientHelper
                .GetFromResource<RatesDto>("http://quiet-stone-2094.herokuapp.com/rates.json");
            foreach (var dto in ratesFromResources)
            {
                if (!Exists(dto.From, dto.To))
                {
                    Add(dto);
                    haveNew = true;
                }
                else
                {
                    if (GetByIds(dto.From, dto.To).Rate != dto.Rate)
                    {
                        Update(dto);
                        haveNew = true;
                    }
                }
            }
            if (haveNew)
                _uowManager.GetUoW().Commit();
        }

        private bool Exists(string from, string to)
        {
            return AsQueryable().Any(r => r.From.Equals(from) && r.To.Equals(to));
        }

        private bool ExistsFromOrTo(string fromOrTo)
        {
            return AsQueryable().Any(r => r.From.Equals(fromOrTo) || r.To.Equals(fromOrTo));
        }

        private List<string> ShortestPathDijkstra(string from, string to)
        {
            var howChangeToEuro = new List<string>();
            try
            {
                var graph = new Graph();
                var distinctRateFrom = GetAllDistinctFrom();
                foreach (var rateFrom in distinctRateFrom)
                {
                    var dtos = GetByFrom(rateFrom);
                    graph.add_vertex(rateFrom, dtos.ToDictionary(dto => dto.To, dto => 1));
                }
                howChangeToEuro = graph.shortest_path(from, to);
            }
            catch (Exception ex)
            {
                Logger.AddLOGMsg($"Error on Dijkstra. Error: {ex.Message}");
            }
            return howChangeToEuro;
        }

        private IEnumerable<string> GetAllDistinctFrom()
        {
            return AsQueryable().Select(r => r.From).Distinct();
        }
        #endregion
    }

    #region Dtos.

    public class RatesDto : DtoBase
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Rate { get; set; }
    }

    #endregion

    #region Dijkstra-Graph

    public class Graph
    {
        Dictionary<string, Dictionary<string, int>> vertices = new Dictionary<string, Dictionary<string, int>>();

        public void add_vertex(string name, Dictionary<string, int> edges)
        {
            vertices[name] = edges;
        }

        public List<string> shortest_path(string start, string finish)
        {
            var previous = new Dictionary<string, string>();
            var distances = new Dictionary<string, int>();
            var nodes = new List<string>();

            List<string> path = new List<string>();

            foreach (var vertex in vertices)
            {
                if (vertex.Key == start)
                {
                    distances[vertex.Key] = 0;
                }
                else
                {
                    distances[vertex.Key] = int.MaxValue;
                }

                nodes.Add(vertex.Key);
            }

            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest == finish)
                {
                    path = new List<string>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                foreach (var neighbor in vertices[smallest])
                {
                    if (distances.ContainsKey(neighbor.Key))
                    {
                        var alt = distances[smallest] + neighbor.Value;
                        if (alt < distances[neighbor.Key])
                        {
                            distances[neighbor.Key] = alt;
                            previous[neighbor.Key] = smallest;
                        }
                    }
                }
            }
            path.Reverse();
            return path;
        }
    }

    #endregion

}
