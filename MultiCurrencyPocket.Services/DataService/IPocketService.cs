using MultiCurrencyPocket.Services.DataService.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiCurrencyPocket.Services.DataService
{
    public interface IPocketService
    {
        
        public Task<decimal> DepositPocketAsync(DepositDTO request);

        public Task<decimal> DepositCurrencyAccountAsync(DepositCurrencyDTO request);

        public Task<decimal> WithdrawPocketAsync(WithdrawDTO request);

        public Task<decimal> WithdrawCurrencyAccountAsync(WithdrawCurrencyDTO request);

        public Task ConvertCurrencyAsync(CurrencyConvertionDTO request);

        //public StatusResultDTO GetPocketStatus(GetPocketStatusDTO request);
        Task<StatusResultDTO> GetPocketStatusAsync(GetPocketStatusDTO request);
    }
}
