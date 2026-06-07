using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using VehicleInsuranceSystem.Data;
using VehicleInsuranceSystem.DTOs.CommonDTOs;
using VehicleInsuranceSystem.DTOs.VehicleDTOs;
using VehicleInsuranceSystem.Exceptions;
using VehicleInsuranceSystem.Interfaces;
using VehicleInsuranceSystem.Models.Vehicles;
using VehicleInsuranceSystem.Services.Common;

namespace VehicleInsuranceSystem.Services.Vehicles;

public class VehicleService : IVehicleService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public VehicleService(ApplicationDbContext context, IMapper mapper, INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<VehicleDto> AddVehicleAsync(int userId, CreateVehicleDto createVehicleDto)
    {
        var vehicle = _mapper.Map<Vehicle>(createVehicleDto);
        vehicle.UserId = userId;

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        await _notificationService.NotifyPolicyEndorsementAsync(vehicle.UserId, "vehicle");

        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(int vehicleId)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        return vehicle == null ? null : _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<PagedResultDto<VehicleDto>> GetVehiclesByUserAsync(int userId, PaginationParamsDto paginationParams)
    {
        var query = _context.Vehicles
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.VehicleId)
            .ProjectTo<VehicleDto>(_mapper.ConfigurationProvider);

        return await PaginationService.CreatePagedResultAsync(query, paginationParams);
    }

    public async Task<VehicleDto> UpdateVehicleAsync(int vehicleId, UpdateVehicleDto updateVehicleDto)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle == null)
        {
            throw new ResourceNotFoundException("Vehicle not found");
        }

        vehicle.Manufacturer = updateVehicleDto.Manufacturer;
        vehicle.Model = updateVehicleDto.Model;

        await _context.SaveChangesAsync();

        return _mapper.Map<VehicleDto>(vehicle);
    }

    public async Task<bool> DeleteVehicleAsync(int vehicleId)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle == null)
        {
            return false;
        }

        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
        return true;
    }
}
