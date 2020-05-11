using Microsoft.EntityFrameworkCore;
using MultiCurrencyPocket.Data;
using MultiCurrencyPocket.Data.Models;
using MultiCurrencyPocket.Services.DataService.DTO;
using MultiCurrencyPocket.Services.Exceptions;
using MultiCurrencyPocket.Services.ExchangeRateServices;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MultiCurrencyPocket.Services.DataService
{
    public class PocketService : IPocketService
    {
        public PocketService(PocketDbContext dbContext, IExchangeRateService rateService)
        {
            this.DbContext = dbContext;
            this.RateService = rateService;
        }

        protected PocketDbContext DbContext { get; }
        protected IExchangeRateService RateService { get; }

        public async Task ConvertCurrencyAsync(CurrencyConvertionDTO request)
        {
            var holder = await DbContext.Holders.SingleOrDefaultAsync(CheckHolder(request));
            
            if (holder == null)
                throw new AccountNotFoundException();
            
            await DbContext.Entry(holder).Collection(h => h.Accounts).LoadAsync();
            
            var accounts =  holder.Accounts.Where(acc => (acc.Currency == request.SourceCurrency) || (acc.Currency == request.DestinationCurrency));

            var sourceAccount = accounts.First(a => a.Currency == request.SourceCurrency);
            var destAccount = accounts.First(a => a.Currency == request.DestinationCurrency);
            
            var convertRate = await RateService.GetRateAsync(request.SourceCurrency, request.DestinationCurrency);

            var debit = request.Sum * convertRate;

            sourceAccount.Debit -= request.Sum;
            destAccount.Debit += debit;

            try
            {
                DbContext.Update(sourceAccount);
                DbContext.Update(destAccount);
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException();
            }


        }

        public async Task<decimal> DepositCurrencyAccountAsync(DepositCurrencyDTO request)
        {
            decimal result;

            var currencyAccount = await DbContext.CurrencyAccounts
                .Include(nameof(CurrencyAccount.Holder))
                .FirstOrDefaultAsync(acc => acc.Number == request.CurrencyAccount && acc.Holder.PINCode == request.PinCode);

            if (currencyAccount == null)
                throw new AccountNotFoundException();

            currencyAccount.Debit += request.Sum;
            try
            {
                DbContext.Update(currencyAccount);
                await DbContext.SaveChangesAsync();
                result = currencyAccount.Debit;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException();
            }

            return result;
        }

        public async Task<decimal> DepositPocketAsync(DepositDTO request)
        {
            decimal result;
            try
            {
                var currencyAccount = DbContext.Holders.Include(nameof(PocketHolder.Accounts))
                     .SingleOrDefault(CheckHolder(request))
                     .Accounts
                     .Find(acc => acc.Currency == request.Currency);

                if (currencyAccount == null)
                    throw new AccountNotFoundException();

                currencyAccount.Debit += request.Sum;
                DbContext.Update(currencyAccount);
                await DbContext.SaveChangesAsync();
                result = currencyAccount.Debit;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException();
            }

            return result;
        }

        public async Task<StatusResultDTO> GetPocketStatusAsync(GetPocketStatusDTO request)
        {

            var holder = await DbContext
                .Holders
                .SingleOrDefaultAsync(h => h.MasterAccount == request.MasterAccount && h.PINCode == request.PinCode);

            if (holder == null)
                throw new AccountNotFoundException();

            await DbContext.Entry(holder).Collection(h => h.Accounts).LoadAsync();

            return new StatusResultDTO
            {
                Deposits = holder.Accounts.Select(a =>
                new DepositItemDTO
                {
                    AccountNumber = a.Number,
                    Currency = a.Currency,
                    Sum = a.Debit
                })
            };

        }

        public async Task<decimal> WithdrawCurrencyAccountAsync(WithdrawCurrencyDTO request)
        {
            var currencyAccount = await DbContext.CurrencyAccounts
                .Include(nameof(CurrencyAccount.Holder))
                .FirstOrDefaultAsync(acc => acc.Number == request.CurrencyAccount && acc.Holder.PINCode == request.PinCode);

            if (currencyAccount == null)
                throw new AccountNotFoundException();

            if (currencyAccount.Debit < request.Sum)
                throw new InsufficientFundException();

            currencyAccount.Debit -= request.Sum;
            
            try
            {
                DbContext.Update(currencyAccount);
                await DbContext.SaveChangesAsync();
                return currencyAccount.Debit;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException();
            }
        }

        public async Task<decimal> WithdrawPocketAsync(WithdrawDTO request)
        {
            decimal result;
            try
            {
                var currencyAccount = DbContext.Holders
                    .Include(nameof(PocketHolder.Accounts))
                    .SingleOrDefault(CheckHolder(request))
                    .Accounts
                    .Find(acc => acc.Currency == request.Currency);

                if (currencyAccount == null)
                    throw new AccountNotFoundException();
                
                if (currencyAccount.Debit < request.Sum)
                {
                    throw new InsufficientFundException();
                }

                currencyAccount.Debit -= request.Sum;
                DbContext.Update(currencyAccount);
                await DbContext.SaveChangesAsync();
                result = currencyAccount.Debit;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException();
            }

            return result;
        }

        protected static Expression<Func<PocketHolder, bool>> CheckHolder( MasterDTO request)
        {
            return x => x.MasterAccount == request.MasterAccount && x.PINCode == request.PinCode;
        }

        protected static Expression<Func<CurrencyAccount, bool>> CheckCurrencyAccountHolder(CurrencyDTO request)
        {
            return x => x.Number == request.CurrencyAccount && x.Holder.PINCode == request.PinCode;
        }
    }
}
