using EmployeeManagementSystem.Application.DTOs;
using EmployeeManagementSystem.Domain.Entities;

namespace EmployeeManagementSystem.Application.Mappings;

/// <summary>
/// Extension methods for mapping Address entities to DTOs.
/// </summary>
public static class AddressMappingExtensions
{
    /// <summary>
    /// Maps an Address entity to an AddressResponseDto.
    /// </summary>
    /// <param name="address">The address entity to map.</param>
    /// <returns>The mapped AddressResponseDto.</returns>
    public static AddressResponseDto ToResponseDto(this Address address)
    {
        return new AddressResponseDto
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

    /// <summary>
    /// Maps a collection of Address entities to AddressResponseDto list.
    /// </summary>
    /// <param name="addresses">The address entities to map.</param>
    /// <returns>The mapped list of AddressResponseDto.</returns>
    public static IReadOnlyList<AddressResponseDto> ToResponseDtoList(this IEnumerable<Address> addresses)
    {
        return addresses.Select(a => a.ToResponseDto()).ToList();
    }
}
