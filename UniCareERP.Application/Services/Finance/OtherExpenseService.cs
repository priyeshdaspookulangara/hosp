using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniCareERP.Application.DTOs.Finance;
using UniCareERP.Domain.Entities.Finance;
using UniCareERP.Infrastructure.Data;

namespace UniCareERP.Application.Services.Finance
{
    public class OtherExpenseService : IOtherExpenseService
    {
        private readonly UniCareDbContext _context;
        private readonly IGeneralLedgerService _generalLedgerService;
        private readonly ILogger<OtherExpenseService> _logger;

        public OtherExpenseService(UniCareDbContext context, IGeneralLedgerService generalLedgerService, ILogger<OtherExpenseService> logger)
        {
            _context = context;
            _generalLedgerService = generalLedgerService;
            _logger = logger;
        }

        public async Task<OtherExpenseDto> CreateExpenseAsync(CreateOtherExpenseDto createDto)
        {
            var debitAccount = await _context.GeneralLedgerAccounts.FindAsync(createDto.GeneralLedgerAccountId);
            var creditAccount = await _context.GeneralLedgerAccounts.FindAsync(createDto.CreditAccountId);

            if (debitAccount == null || creditAccount == null)
            {
                _logger.LogError("Debit or Credit account not found.");
                return null;
            }

            var expense = new OtherExpense
            {
                Description = createDto.Description,
                Amount = createDto.Amount,
                ExpenseDate = createDto.ExpenseDate,
                GeneralLedgerAccountId = createDto.GeneralLedgerAccountId
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.OtherExpenses.Add(expense);
                    await _context.SaveChangesAsync();

                    await _generalLedgerService.PostTransactionAsync(creditAccount, debitAccount, expense.Amount, $"Other Expense: {expense.Description}");

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating other expense.");
                    return null;
                }
            }

            return new OtherExpenseDto
            {
                Id = expense.Id,
                Description = expense.Description,
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate,
                GeneralLedgerAccountId = expense.GeneralLedgerAccountId,
                GeneralLedgerAccountName = debitAccount.AccountName
            };
        }

        public async Task<IEnumerable<OtherExpenseDto>> GetAllExpensesAsync()
        {
            return await _context.OtherExpenses
                .Include(e => e.GeneralLedgerAccount)
                .Select(e => new OtherExpenseDto
                {
                    Id = e.Id,
                    Description = e.Description,
                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate,
                    GeneralLedgerAccountId = e.GeneralLedgerAccountId,
                    GeneralLedgerAccountName = e.GeneralLedgerAccount.AccountName
                })
                .ToListAsync();
        }
    }
}
