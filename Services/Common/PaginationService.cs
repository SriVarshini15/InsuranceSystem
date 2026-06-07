using Microsoft.EntityFrameworkCore;
using VehicleInsuranceSystem.DTOs.CommonDTOs;

namespace VehicleInsuranceSystem.Services.Common;

public static class PaginationService
{
    public static async Task<PagedResultDto<T>> CreatePagedResultAsync<T>(
        IQueryable<T> query,
        PaginationParamsDto paginationParams)
    {
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParams.PageSize);
        var items = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PagedResultDto<T>(
            items,
            paginationParams.PageNumber,
            paginationParams.PageSize,
            totalCount,
            totalPages);
    }
}
