using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Vueling.Business.DomainServices.Mapper;
using Vueling.Business.HttpClientHelper;
using Vueling.Common.Core.Log;
using Vueling.Common.DataAccess.Contracts;
using Vueling.Common.DataAccess.EF.UnitOfWork;
using Vueling.Common.DomainServices;
using Vueling.Domain.Entities.Modules.Executives;

namespace Vueling.Business.DomainServices.Modules.Executive
{
    public interface ITransactionsDomainServices : IDtoDomainService<TransactionsDto>
    {
        Tuple<IEnumerable<TransactionsDto>, decimal> GetBySku(string sku);
    }

    public class TransactionsDomainServices : DomainServiceBase<Transactions>, ITransactionsDomainServices
    {
        #region Properties
        public IVuelingHttpClientHelper _vuelingHttpClientHelper { get; set; }

        public IRatesDomainServices _ratesDomainServices { get; set; }

        private readonly ISqlBulkLogic _sqlBulkLogic;

        public const string TableName = "Transactions";
        #endregion

        #region Ctor.
        public TransactionsDomainServices(IVuelingHttpClientHelper vuelingHttpClientHelper,
            IRatesDomainServices ratesDomainServices,
            ISqlBulkLogic sqlBulkLogic)
            : base()
        {
            _vuelingHttpClientHelper = vuelingHttpClientHelper;
            _ratesDomainServices = ratesDomainServices;
            _sqlBulkLogic = sqlBulkLogic;
        }
        #endregion

        #region Public_Methods

        public Tuple<IEnumerable<TransactionsDto>, decimal> GetBySku(string sku)
        {
            Logger.AddLOGMsg($"GET api/transactions/getBySku/{sku}");
            var dtosWithCurrencyEuro = new List<TransactionsDto>();
            var total = 0M;

            IEnumerable<TransactionsDto> dtosFromResource = new List<TransactionsDto>();
            IEnumerable<TransactionsDto> dtos = new List<TransactionsDto>();
            dtosFromResource = CheckIfThereAreNewTransactions();
            if (dtosFromResource.Any()) { 
                Task.Run(() => RefreshTransactionsFromTheResource(dtosFromResource));
                dtos = dtosFromResource.Where(t => t.Sku.Equals(sku)).ToList();
            }
            else
                dtos = AsQueryable().Where(t => t.Sku.Equals(sku)).Select(DomainServicesMapper.MapToTransactionsDto).ToList();
            
            if (dtos.Any())
            {
                dtosWithCurrencyEuro = ChangeCurrencyToEuroToAllDtos(dtos);
                total = dtosWithCurrencyEuro.Sum(t => t.Amount);
            }
            return new Tuple<IEnumerable<TransactionsDto>, decimal>(dtosWithCurrencyEuro, total);
        }

        public TransactionsDto GetByIds(params object[] ids)
        {
            Logger.AddLOGMsg($"Get By Ids: {ids}");
            var domain = repository.GetByPKs(ids);
            return DomainServicesMapper.MapToTransactionsDto(domain);
        }

        public void Add(TransactionsDto dto)
        {
            var domain = new Transactions();
            DomainServicesMapper.MapToTransactions(dto, domain);
            repository.Add(domain);
            Logger.AddLOGMsg($"Add Transaction with Amount -> {dto.Amount} Sku -> {dto.Sku} Currency -> {dto.Currency}");
        }

        public void Update(TransactionsDto dto)
        {
            var domain = repository.GetByPKs(dto.Sku, dto.Currency);
            DomainServicesMapper.MapToTransactions(dto, domain);
            repository.Update(domain);
            Logger.AddLOGMsg($"Updated Transaction with Amount -> {dto.Amount} Sku -> {dto.Sku} Currency -> {dto.Currency}");
        }

        public void Delete(TransactionsDto dto)
        {
            var domain = repository.GetByPKs(dto.Sku, dto.Currency);
            repository.Delete(domain);
        }

        public IEnumerable<TransactionsDto> GetAll()
        {
            Logger.AddLOGMsg("Get All Transactions");
            IEnumerable<TransactionsDto> dtos = new List<TransactionsDto>();
            dtos = CheckIfThereAreNewTransactions();
            if (dtos.Any())
                Task.Run(() => RefreshTransactionsFromTheResource(dtos));
            else
                dtos = repository.GetAll().Select(DomainServicesMapper.MapToTransactionsDto).ToList();
            return dtos;
        }

        private void RefreshTransactionsFromTheResource(IEnumerable<TransactionsDto> dtos)
        {
            Logger.AddLOGMsg("Deleted All Transactions");
            using (var context = GetNewContext())
            {
                _sqlBulkLogic.BulkDelete(TableName);

                var domains = new List<Transactions>();
                foreach (var dto in dtos)
                {
                    var domain = new Transactions();
                    DomainServicesMapper.MapToTransactions(dto, domain);
                    domains.Add(domain);
                }

                _sqlBulkLogic.BulkInsert(domains, TableName);
            }
            Logger.AddLOGMsg("Saved New Transactions");
        }

        #endregion

        #region Private_Methods
        private TransactionsDto GetDtoByFilters(
            Expression<Func<Transactions, bool>> filter = null,
            Func<IQueryable<Transactions>, IOrderedQueryable<Transactions>> orderBy = null,
            string includeProperties = null)
        {
            var domain = GetByFilters(filter, orderBy, includeProperties).SingleOrDefault();
            return DomainServicesMapper.MapToTransactionsDto(domain);
        }

        private IEnumerable<TransactionsDto> GetDtosByFilters(
            Expression<Func<Transactions, bool>> filter = null,
            Func<IQueryable<Transactions>, IOrderedQueryable<Transactions>> orderBy = null,
            string includeProperties = null)
        {
            var domains = GetByFilters(filter, orderBy, includeProperties);
            return domains.Select(DomainServicesMapper.MapToTransactionsDto).ToList();
        }

        private IEnumerable<TransactionsDto> CheckIfThereAreNewTransactions()
        {
            var transactionsFromResources = _vuelingHttpClientHelper
                .GetFromResource<TransactionsDto>("http://quiet-stone-2094.herokuapp.com/transactions.json");
            return transactionsFromResources;
        }

        private List<TransactionsDto> ChangeCurrencyToEuroToAllDtos(IEnumerable<TransactionsDto> dtos)
        {
            return dtos.Select(ChangeCurrencyToEuro).ToList();
        }

        private TransactionsDto ChangeCurrencyToEuro(TransactionsDto dto)
        {
            Logger.AddLOGMsg($"ChangeCurrencyToEuro. Currency: {dto.Currency}");
            if (dto.Currency != "EUR")
            {
                var howChangeToEuro = _ratesDomainServices.HowToChangeFromOneCurrencyToAnother(dto.Currency, "EUR");
                var from = dto.Currency;
                var amountTemp = dto.Amount;
                foreach (var currency in howChangeToEuro)
                {
                    var rateDto = _ratesDomainServices.GetByIds(from, currency);
                    from = currency;
                    amountTemp = amountTemp * rateDto.Rate;
                }
                if (howChangeToEuro.Any())
                {
                    dto.Currency = "EUR";
                    dto.Amount = decimal.Round(amountTemp, 2, MidpointRounding.AwayFromZero);
                }
            }
            return dto;
        }

        #endregion
    }

    #region Dtos.

    public class TransactionsDto : DtoBase
    {
        public string Sku { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    #endregion
}
