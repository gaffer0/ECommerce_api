using EC_V2.Dtos;
using EC_V2.Dtos.ProfileDtos;

namespace EC_V2.Services.Interfaces
{
    public interface IProfileService
    {
        Task<ServiceResult<bool>> CreateVendorProfile(string userId, AddVendorProfileDto dto);
        Task<ServiceResult<bool>> CreateCustomerProfile(string userId, AddCustomerProfileDto dto);
        Task<ServiceResult<CustomerProfileDto>> GetCustomerProfile(string userId);
        Task<ServiceResult<VendorProfileDto>> GetVendorProfile(string userId);
        Task<ServiceResult<bool>> UpdateCustomerProfile(string userId, UpdateCustomerProfileDto dto);
        Task<ServiceResult<bool>> UpdateVendorProfile(string userId, UpdateVendorProfileDto dto);
    }
}
