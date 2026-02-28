using EC_V2.Dtos;
using EC_V2.Models;
using EC_V2.Repositories.Interfaces;
using EC_V2.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EC_V2.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProfileService> _logger;
        private readonly UserManager<AppUser> _userManager;
        public ProfileService(IUnitOfWork unitOfWork, ILogger<ProfileService> logger, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<ServiceResult<bool>> CreateCustomerProfile(string userId, AddCustomerProfileDto dto)
        {
            _logger.LogInformation("Creating customer profile for user {UserId}", userId);

            var existingProfile = await _unitOfWork.CustomerProfile.GetByUserIdAsync(userId);
            if (existingProfile != null)
                return new ServiceResult<bool> { Success = false, Error = "Customer profile already exists" };

            var profile = new CustomerProfile
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                PhoneNumber = dto.PhoneNumber
            };

            await _unitOfWork.CustomerProfile.Add(profile);
            var result = await _unitOfWork.SaveChangesAsync();

            if (result > 0)
            {
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.AddToRoleAsync(user!, "Customer");
                _logger.LogInformation("Customer profile created for user {UserId}", userId);
                return new ServiceResult<bool> { Success = true, Data = true };
            }

            _logger.LogError("Failed to create customer profile for user {UserId}", userId);
            return new ServiceResult<bool> { Success = false, Error = "Failed to create customer profile" };
        }
        public async Task<ServiceResult<bool>> CreateVendorProfile(string userId, AddVendorProfileDto dto)
        {
            _logger.LogInformation("Creating vendor profile for user {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ServiceResult<bool> { Success = false, Error = "User not found" };


            var existingProfile = await _unitOfWork.VendorProfile.GetByUserIdAsync(userId);
            if (existingProfile != null)
                return new ServiceResult<bool> { Success = false, Error = "Vendor profile already exists" };
            var profile = new VendorProfile
            {
                UserId = userId,
                StoreName = dto.StoreName,
                Description = dto.Description,
                TaxNumber = dto.TaxNumber
            };
            await _unitOfWork.VendorProfile.Add(profile);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                await _userManager.AddToRoleAsync(user!, "Vendor");
                _logger.LogInformation("Vendor profile created for user {UserId}", userId);
                return new ServiceResult<bool> { Success = true, Data = true };
            }
            _logger.LogError("Failed to create vendor profile for user {UserId}", userId);
            return new ServiceResult<bool> { Success = false, Error = "Failed to create vendor profile" };
        }

        public async Task<ServiceResult<CustomerProfileDto>> GetCustomerProfile(string userId)
        {
            var profile = await _unitOfWork.CustomerProfile.GetByUserIdAsync(userId);
            if (profile == null)
                return new ServiceResult<CustomerProfileDto> { Success = false, Error = "Customer profile not found" };
            var dto = new CustomerProfileDto
            {
                ShippingAddress = profile.ShippingAddress,
                PhoneNumber = profile.PhoneNumber
            };
            return new ServiceResult<CustomerProfileDto> { Success = true, Data = dto };
        }
        public async Task<ServiceResult<VendorProfileDto>> GetVendorProfile(string userId)
        {
            var profile = await _unitOfWork.VendorProfile.GetByUserIdAsync(userId);
            if (profile == null)
                return new ServiceResult<VendorProfileDto> { Success = false, Error = "Vendor profile not found" };
            var dto = new VendorProfileDto
            {
                StoreName = profile.StoreName,
                Description = profile.Description,
                TaxNumber = profile.TaxNumber
            };
            return new ServiceResult<VendorProfileDto> { Success = true, Data = dto };
        }
        public async Task<ServiceResult<bool>> UpdateCustomerProfile(string userId, UpdateCustomerProfileDto dto)
        {
            var profile = await _unitOfWork.CustomerProfile.GetByUserIdAsync(userId);
            if (profile == null)
                return new ServiceResult<bool> { Success = false, Error = "Customer profile not found" };
            profile.ShippingAddress = dto.ShippingAddress;
            profile.PhoneNumber = dto.PhoneNumber;
            _unitOfWork.CustomerProfile.Update(profile);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Customer profile updated for user {UserId}", userId);
                return new ServiceResult<bool> { Success = true, Data = true };
            }
            _logger.LogError("Failed to update customer profile for user {UserId}", userId);
            return new ServiceResult<bool> { Success = false, Error = "Failed to update customer profile" };
        }
        public async Task<ServiceResult<bool>> UpdateVendorProfile(string userId, UpdateVendorProfileDto dto)
        {
            var profile = await _unitOfWork.VendorProfile.GetByUserIdAsync(userId);
            if (profile == null)
                return new ServiceResult<bool> { Success = false, Error = "Vendor profile not found" };
            profile.StoreName = dto.StoreName;
            profile.Description = dto.Description;
            _unitOfWork.VendorProfile.Update(profile);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Vendor profile updated for user {UserId}", userId);
                return new ServiceResult<bool> { Success = true, Data = true };
            }
            _logger.LogError("Failed to update vendor profile for user {UserId}", userId);
            return new ServiceResult<bool> { Success = false, Error = "Failed to update vendor profile" };


        }






    }

}
