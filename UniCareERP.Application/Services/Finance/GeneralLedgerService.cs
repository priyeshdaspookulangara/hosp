using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Finance
{
    public class GeneralLedgerService : IGeneralLedgerService
    {
        private readonly UniCareDbContext _context;
        private readonly ILogger<GeneralLedgerService> _logger;

        public GeneralLedgerService(UniCareDbContext context, ILogger<GeneralLedgerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RecordTransactionAsync(Guid accountId, DateTime date, decimal amount, string description)
        {
            var transaction = new GeneralLedgerTransaction
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                TransactionDate = date,
                Amount = amount,
                Description = description
            };

            try
            {
                _context.GeneralLedgerTransactions.Add(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Transaction recorded for account {accountId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording transaction for account {accountId}.");
            }
        }
    }
}
