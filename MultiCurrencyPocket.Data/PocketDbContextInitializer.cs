using Microsoft.EntityFrameworkCore;
using MultiCurrencyPocket.Data.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MultiCurrencyPocket.Data
{
    public partial class PocketDbContext 
    {
        protected void SeedData(ModelBuilder modelBuilder) 
        {
            InitCurrencyData(modelBuilder);
            
            InitTestPocketHolders(modelBuilder);
            InitCurrencyAccounts(modelBuilder);
        }

        private void InitTestPocketHolders(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PocketHolder>()
                .HasIndex(nameof(PocketHolder.MasterAccount))
                .IsUnique();

            modelBuilder.Entity<PocketHolder>()
                .Property(p => p.Gender)
                .HasConversion(new EnumToNumberConverter<GenderCode, byte>());

            modelBuilder
                .Entity<PocketHolder>()
                .HasData(new[]
                { 
                    new PocketHolder { Id = 1, MasterAccount = "123450", PINCode = "1234", FirstName = "Мария", LastName = "Иванова", DateOfBirth = new DateTime(1984, 10, 15), Gender = GenderCode.Female, ExpirationDate = DateTime.Today.AddYears(5)},
                    new PocketHolder { Id = 2, MasterAccount = "555550", PINCode = "5555", FirstName = "Bacя", LastName = "Пупкин", DateOfBirth = new DateTime(1989, 4, 1), Gender = GenderCode.Male, ExpirationDate = DateTime.Today.AddYears(10)},
                    new PocketHolder { Id = 3, MasterAccount = "999990", PINCode = "9999", FirstName = "Тарас", LastName = "Поздний", DateOfBirth = new DateTime(1975, 5, 9), Gender = GenderCode.Male, ExpirationDate = DateTime.Today.AddMonths(-2)}
                });
        }

        private void InitCurrencyAccounts(ModelBuilder modelBuilder) 
        {
            modelBuilder
                .Entity<CurrencyAccount>()
                .HasIndex(nameof(CurrencyAccount.Number))
                .IsUnique();

            modelBuilder
                .Entity<CurrencyAccount>()
                .HasIndex(nameof(CurrencyAccount.PocketHolderId), nameof(CurrencyAccount.Currency))
                .IsUnique();

            modelBuilder.Entity<CurrencyAccount>()
                .HasData( new[] 
                {
                    new CurrencyAccount { Id = 1, Number = "123451", PocketHolderId = 1, Currency = "RUB", Debit = 1000m  },
                    new CurrencyAccount { Id = 2, Number = "123452", PocketHolderId = 1, Currency = "USD", Debit = 5000m  },

                    new CurrencyAccount { Id = 3, Number = "555551", PocketHolderId = 2, Currency = "RUB", Debit = 100000m  },
                    new CurrencyAccount { Id = 4, Number = "555552", PocketHolderId = 2, Currency = "USD", Debit = 5000m },
                    new CurrencyAccount { Id = 5, Number = "555553", PocketHolderId = 2, Currency = "EUR", Debit = 2000m },

                    new CurrencyAccount { Id = 6, Number = "999991", PocketHolderId = 3, Currency = "RUB", Debit = 100m  }
                }
                );

        }



        /*
           <Cube time='2020-05-08'>
            <Cube currency='USD' rate='1.0843'/>
            <Cube currency='JPY' rate='115.34'/>
            <Cube currency='BGN' rate='1.9558'/>
            <Cube currency='CZK' rate='27.251'/>
            <Cube currency='DKK' rate='7.4598'/>
            <Cube currency='GBP' rate='0.87535'/>
            <Cube currency='HUF' rate='349.38'/>
            <Cube currency='PLN' rate='4.5482'/>
            <Cube currency='RON' rate='4.8280'/>
            <Cube currency='SEK' rate='10.5875'/>
            <Cube currency='CHF' rate='1.0529'/>
            <Cube currency='ISK' rate='158.50'/>
            <Cube currency='NOK' rate='11.0695'/>
            <Cube currency='HRK' rate='7.5573'/>
            <Cube currency='RUB' rate='79.8383'/>
            <Cube currency='TRY' rate='7.7252'/>
            <Cube currency='AUD' rate='1.6613'/>
            <Cube currency='BRL' rate='6.3074'/>
            <Cube currency='CAD' rate='1.5118'/>
            <Cube currency='CNY' rate='7.6719'/>
            <Cube currency='HKD' rate='8.4052'/>
            <Cube currency='IDR' rate='16229.26'/>
            <Cube currency='ILS' rate='3.8031'/>
            <Cube currency='INR' rate='81.9615'/>
            <Cube currency='KRW' rate='1322.41'/>
            <Cube currency='MXN' rate='25.9023'/>
            <Cube currency='MYR' rate='4.6994'/>
            <Cube currency='NZD' rate='1.7668'/>
            <Cube currency='PHP' rate='54.681'/>
            <Cube currency='SGD' rate='1.5326'/>
            <Cube currency='THB' rate='34.958'/>
            <Cube currency='ZAR' rate='19.9970'/></Cube>
           </Cube>
         */
         //TODO: Load from csv
        protected void InitCurrencyData(ModelBuilder modelBuilder)         
        {
            //Init Currency
            modelBuilder.Entity<Currency>()
                .HasData(new[]
                {
                    new Currency { ShortCurrency = "EUR", Code =  978,  FullCurrencyName = "Euro" },
                    new Currency { ShortCurrency = "USD", Code = 840, FullCurrencyName = "U.S. Dollar" },
                    new Currency { ShortCurrency = "RUB", Code =  643, FullCurrencyName = "Russian Rouble" },
                    new Currency { ShortCurrency = "JPY", Code =  643, FullCurrencyName = "Japan Yen" },
                    new Currency { ShortCurrency = "BGN", Code = 975, FullCurrencyName = "Boulgarian lev" },
                    new Currency { ShortCurrency = "CZK" },
                    new Currency { ShortCurrency = "DKK" },
                    new Currency { ShortCurrency = "GBP", Code = 826, FullCurrencyName ="G.B. Pound" },
                    new Currency { ShortCurrency = "HUF", FullCurrencyName ="Hungary Forit" },
                    new Currency { ShortCurrency = "PLN", FullCurrencyName ="Polsky zloty" },
                    new Currency { ShortCurrency = "RON", FullCurrencyName ="Romanian ley" },
                    
                });
        }
    }
}