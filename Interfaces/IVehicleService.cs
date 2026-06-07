using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.VehicleDTOs;

namespace VehicleInsuranceSystem.Interfaces;

public interface IVehicleService
{
    Task<VehicleDto> AddVehicleAsync(int userId, CreateVehicleDto createVehicleDto);

    Task<VehicleDto?> GetVehicleByIdAsync(int vehicleId);

    Task<PagedResultDto<VehicleDto>> GetVehiclesByUserAsync(int userId, PaginationParamsDto paginationParams);

    Task<VehicleDto> UpdateVehicleAsync(int vehicleId, UpdateVehicleDto updateVehicleDto);

    Task<bool> DeleteVehicleAsync(int vehicleId);
}
