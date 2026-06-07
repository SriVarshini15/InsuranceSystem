using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.UserDTOs;

namespace VehicleInsuranceSystem.Interfaces;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(int userId);

    Task<PagedResultDto<UserDto>> GetAllUsersAsync(PaginationParamsDto paginationParams);

    Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateDto);

    Task<bool> DeleteUserAsync(int userId);
}
