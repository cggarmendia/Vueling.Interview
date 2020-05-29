using System;
using Vueling.Business.DomainServices.Modules.Executive;
using Vueling.Domain.Entities.Modules.Executives;

namespace Vueling.Business.DomainServices.Mapper
{
    internal class DomainServicesMapper
    {
        #region Rates - RatesDto
        internal static RatesDto MapToRatesDto(Rates domain)
        {
            var dto = new RatesDto();
            if (domain != null)
                dto = new RatesDto()
                {
                    From = domain.From,
                    Rate = domain.Rate,
                    To = domain.To
                };
            return dto;
        }

        internal static void MapToRates(RatesDto dto, Rates domain = null)
        {
            if (domain == null)
                domain = new Rates();

            domain.From = dto.From;
            domain.Rate = dto.Rate;
            domain.To = dto.To;
        }
        #endregion
        
        #region Transactions - TransactionsDto
        internal static TransactionsDto MapToTransactionsDto(Transactions domain)
        {
            var dto = new TransactionsDto();
            if (domain != null)
                dto = new TransactionsDto()
                {
                    Sku = domain.Sku,
                    Amount = domain.Amount,
                    Currency = domain.Currency
                };
            return dto;
        }

        internal static void MapToTransactions(TransactionsDto dto, Transactions domain = null)
        {
            if (domain == null)
                domain = new Transactions();

            domain.Sku = dto.Sku;
            domain.Amount = dto.Amount;
            domain.Currency = dto.Currency;
        }
        #endregion
    
    }
}
