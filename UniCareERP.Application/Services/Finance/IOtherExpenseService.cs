using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniCareERP.Application.DTOs.Finance;

namespace UniCareERP.Application.Services.Finance
{
    public interface IOtherExpenseService
    {
        Task<OtherExpenseDto> CreateExpenseAsync(CreateOtherExpenseDto createDto);
        Task<IEnumerable<OtherExpenseDto>> GetAllExpensesAsync();
    }
}
