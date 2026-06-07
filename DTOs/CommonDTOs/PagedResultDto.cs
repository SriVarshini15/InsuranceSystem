namespace VehicleInsuranceSystem.DTOs.CommonDTOs;

public record PagedResultDto<T>(
    IEnumerable<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
)
{
    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
}
