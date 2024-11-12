using ESPService.Data;
using ESPService.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace ESPService.Repository;

public class DiscountCodeRepository(AppDbContext dbContext) : IDiscountCodeRepository
{
    protected IDbContextTransaction WithTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable) => dbContext.Database.BeginTransaction(isolationLevel);

    public async Task<string[]> GetAll()
    {
        using var _ = WithTransaction(IsolationLevel.ReadCommitted);
        return await dbContext.Codes
            .Where(code => code.UsedAt == null)
            .Select(e => e.Code).ToArrayAsync();
    }

    public async Task<DiscountCode> Insert(string code, bool checkIfExists)
    {
        using var txn = WithTransaction();
        if (checkIfExists && await dbContext.Codes.AnyAsync(c => c.Code == code && c.UsedAt == null))
        {
            return null;
        }

        var discountCode = new DiscountCode()
        {
            Code = code,
            UsedAt = null,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Codes.Add(discountCode);

        await dbContext.SaveChangesAsync();
        txn.Commit();

        dbContext.Entry(discountCode).State = EntityState.Detached;

        return discountCode;
    }

    public async Task<bool> UseCode(string code)
    {
        using var txn = WithTransaction();

        var result = await dbContext.Codes
            .Where(c => c.Code == code && c.UsedAt == null)
            .ExecuteUpdateAsync(e => e.SetProperty(f => f.UsedAt, DateTime.UtcNow));

        txn.Commit();

        return result > 0;

    }
}
