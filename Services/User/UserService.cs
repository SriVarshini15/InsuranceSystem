using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.UserDTOs;
using VehicleInsuranceSystem.Exceptions;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Services.Common;

namespace VehicleInsuranceSystem.Services.Users;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public UserService(ApplicationDbContext context, IMapper mapper, INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.UserId == userId);
        return user == null ? throw new ResourceNotFoundException("User not found") : _mapper.Map<UserDto>(user);
    }

    public async Task<PagedResultDto<UserDto>> GetAllUsersAsync(PaginationParamsDto paginationParams)
    {
        var query = _context.Users
            .Include(x => x.Role)
            .OrderBy(x => x.UserId)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<UserDto> UpdateUserAsync(int userId, UpdateUserDto updateDto)
    {
        var user = await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.UserId == userId);
        if (user == null)
        {
            throw new ResourceNotFoundException("User not found");
        }

        user.FirstName = updateDto.FirstName;
        user.LastName = updateDto.LastName;
        user.PhoneNumber = updateDto.PhoneNumber;
        user.Address = updateDto.Address;

        await _context.SaveChangesAsync();
        await _notificationService.NotifyPolicyEndorsementAsync(userId, "profile");

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
