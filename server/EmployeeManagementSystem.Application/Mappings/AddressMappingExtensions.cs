using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Address entities to DTOs.
/// </summary>
public static class AddressMappingExtensions
{
    /// <summary>
    /// Extension method for address.
    /// </summary>
    /// <param name="address"></param>
    extension(Address address)
    {
        /// <summary>
        /// Maps an Address entity to an AddressResponseDto.
        /// </summary>
        /// <returns></returns>
        public AddressResponseDto ToResponseDto()
        {
            return new()
            {
                DisplayId = address.DisplayId,
                Address1 = address.Address1,
                Address2 = address.Address2,
                Barangay = address.Barangay,
                City = address.City,
                Province = address.Province,
                Country = address.Country,
                ZipCode = address.ZipCode,
                IsCurrent = address.IsCurrent,
                IsPermanent = address.IsPermanent,
                IsActive = address.IsActive,
                AddressType = address.AddressType,
                FullAddress = address.FullAddress,
                CreatedOn = address.CreatedOn,
                CreatedBy = address.CreatedBy,
                ModifiedOn = address.ModifiedOn,
                ModifiedBy = address.ModifiedBy
            };
        }
    }

    /// <summary>
    /// Extension method for addresses.
    /// </summary>
    /// <param name="addresses"></param>
    extension(IEnumerable<Address> addresses)
    {
        /// <summary>
        /// Maps a collection of Address entities to a list of AddressResponseDto.
        /// </summary>
        /// <returns>The list of mapped AddressResponseDto.</returns>
        public IReadOnlyList<AddressResponseDto> ToResponseDtoList()
        {
            return [.. addresses.Select(a => a.ToResponseDto())];
        }
    }
}
