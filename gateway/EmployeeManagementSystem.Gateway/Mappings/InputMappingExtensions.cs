using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Types.Inputs;

namespace EmployeeManagementSystem.Gateway.Mappings;

public static class InputMappingExtensions
{
    #region Address Mappings

    public static CreateAddressDto ToDto(this CreateAddressInput input)
    {
        return new CreateAddressDto
        {
            Address1 = input.Address1 ?? string.Empty,
            Address2 = input.Address2,
            AddressType = input.AddressType ?? AddressType.Business,
            Barangay = input.Barangay,
            City = input.City ?? string.Empty,
            Country = input.Country,
            IsCurrent = input.IsCurrent ?? false,
            IsPermanent = input.IsPermanent ?? false,
            Province = input.Province ?? string.Empty,
            ZipCode = input.ZipCode
        };
    }

    public static UpsertAddressDto ToDto(this UpsertAddressInput input)
    {
        return new UpsertAddressDto
        {
            DisplayId = input.DisplayId ?? 0,
            Address1 = input.Address1 ?? string.Empty,
            Address2 = input.Address2,
            AddressType = input.AddressType ?? AddressType.Business,
            Barangay = input.Barangay,
            City = input.City ?? string.Empty,
            Country = input.Country,
            IsCurrent = input.IsCurrent ?? false,
            IsPermanent = input.IsPermanent ?? false,
            Province = input.Province ?? string.Empty,
            ZipCode = input.ZipCode
        };
    }

    public static List<CreateAddressDto>? ToDtos(this List<CreateAddressInput>? inputs)
    {
        return inputs?.Select(i => i.ToDto()).ToList();
    }

    public static List<UpsertAddressDto>? ToDtos(this List<UpsertAddressInput>? inputs)
    {
        return inputs?.Select(i => i.ToDto()).ToList();
    }

    #endregion

    #region Contact Mappings

    public static CreateContactDto ToDto(this CreateContactInput input)
    {
        return new CreateContactDto
        {
            ContactType = input.ContactType ?? ContactType.Work,
            Email = input.Email,
            Fax = input.Fax,
            LandLine = input.LandLine,
            Mobile = input.Mobile
        };
    }

    public static UpsertContactDto ToDto(this UpsertContactInput input)
    {
        return new UpsertContactDto
        {
            DisplayId = input.DisplayId ?? 0,
            ContactType = input.ContactType ?? ContactType.Work,
            Email = input.Email,
            Fax = input.Fax,
            LandLine = input.LandLine,
            Mobile = input.Mobile
        };
    }

    public static List<CreateContactDto>? ToDtos(this List<CreateContactInput>? inputs)
    {
        return inputs?.Select(i => i.ToDto()).ToList();
    }

    public static List<UpsertContactDto>? ToDtos(this List<UpsertContactInput>? inputs)
    {
        return inputs?.Select(i => i.ToDto()).ToList();
    }

    #endregion

    #region Person Mappings

    public static CreatePersonDto ToDto(this CreatePersonInput input)
    {
        return new CreatePersonDto
        {
            FirstName = input.FirstName ?? string.Empty,
            MiddleName = input.MiddleName,
            LastName = input.LastName ?? string.Empty,
            DateOfBirth = input.DateOfBirth.HasValue ? new DateTimeOffset(input.DateOfBirth.Value) : DateTimeOffset.MinValue,
            Gender = input.Gender ?? Gender.Male,
            CivilStatus = input.CivilStatus ?? CivilStatus.Single,
            Addresses = input.Addresses?.ToDtos(),
            Contacts = input.Contacts?.ToDtos()
        };
    }

    public static UpdatePersonDto ToDto(this UpdatePersonInput input)
    {
        return new UpdatePersonDto
        {
            FirstName = input.FirstName ?? string.Empty,
            MiddleName = input.MiddleName,
            LastName = input.LastName ?? string.Empty,
            DateOfBirth = input.DateOfBirth.HasValue ? new DateTimeOffset(input.DateOfBirth.Value) : DateTimeOffset.MinValue,
            Gender = input.Gender ?? Gender.Male,
            CivilStatus = input.CivilStatus ?? CivilStatus.Single
        };
    }

    #endregion

    #region Employment Mappings

    public static CreateEmploymentSchoolDto ToDto(this CreateEmploymentSchoolInput input)
    {
        return new CreateEmploymentSchoolDto
        {
            SchoolDisplayId = input.SchoolDisplayId ?? 0,
            StartDate = input.StartDate.HasValue ? new DateTimeOffset(input.StartDate.Value) : null,
            EndDate = input.EndDate.HasValue ? new DateTimeOffset(input.EndDate.Value) : null,
            IsCurrent = input.IsCurrent ?? false
        };
    }

    public static List<CreateEmploymentSchoolDto>? ToDtos(this List<CreateEmploymentSchoolInput>? inputs)
    {
        return inputs?.Select(i => i.ToDto()).ToList();
    }

    public static CreateEmploymentDto ToDto(this CreateEmploymentInput input)
    {
        return new CreateEmploymentDto
        {
            PersonDisplayId = input.PersonDisplayId ?? 0,
            PositionDisplayId = input.PositionDisplayId ?? 0,
            SalaryGradeDisplayId = input.SalaryGradeDisplayId ?? 0,
            ItemDisplayId = input.ItemDisplayId ?? 0,
            EmploymentStatus = input.EmploymentStatus ?? EmploymentStatus.Regular,
            AppointmentStatus = input.AppointmentStatus ?? AppointmentStatus.Original,
            Eligibility = input.Eligibility ?? Eligibility.LET,
            DateOfOriginalAppointment = input.DateOfOriginalAppointment.HasValue ? new DateTimeOffset(input.DateOfOriginalAppointment.Value) : null,
            PsipopItemNumber = input.PsipopItemNumber,
            DepEdId = input.DepEdId,
            GsisId = input.GsisId,
            PhilHealthId = input.PhilHealthId,
            TinId = input.TinId,
            Schools = input.Schools?.ToDtos()
        };
    }

    public static UpdateEmploymentDto ToDto(this UpdateEmploymentInput input)
    {
        return new UpdateEmploymentDto
        {
            PositionDisplayId = input.PositionDisplayId ?? 0,
            SalaryGradeDisplayId = input.SalaryGradeDisplayId ?? 0,
            ItemDisplayId = input.ItemDisplayId ?? 0,
            EmploymentStatus = input.EmploymentStatus ?? EmploymentStatus.Regular,
            AppointmentStatus = input.AppointmentStatus ?? AppointmentStatus.Original,
            Eligibility = input.Eligibility ?? Eligibility.LET,
            DateOfOriginalAppointment = input.DateOfOriginalAppointment.HasValue ? new DateTimeOffset(input.DateOfOriginalAppointment.Value) : null,
            PsipopItemNumber = input.PsipopItemNumber,
            DepEdId = input.DepEdId,
            GsisId = input.GsisId,
            PhilHealthId = input.PhilHealthId,
            TinId = input.TinId,
            IsActive = input.IsActive ?? true
        };
    }

    #endregion

    #region School Mappings

    public static CreateSchoolDto ToDto(this CreateSchoolInput input)
    {
        return new CreateSchoolDto
        {
            SchoolName = input.SchoolName ?? string.Empty,
            Addresses = input.Addresses?.ToDtos(),
            Contacts = input.Contacts?.ToDtos()
        };
    }

    public static UpdateSchoolDto ToDto(this UpdateSchoolInput input)
    {
        return new UpdateSchoolDto
        {
            SchoolName = input.SchoolName,
            Addresses = input.Addresses?.ToDtos(),
            Contacts = input.Contacts?.ToDtos(),
            IsActive = input.IsActive ?? true
        };
    }

    #endregion

    #region Position Mappings

    public static CreatePositionDto ToDto(this CreatePositionInput input)
    {
        return new CreatePositionDto
        {
            TitleName = input.TitleName ?? string.Empty,
            Description = input.Description
        };
    }

    public static UpdatePositionDto ToDto(this UpdatePositionInput input)
    {
        return new UpdatePositionDto
        {
            TitleName = input.TitleName,
            Description = input.Description,
            IsActive = input.IsActive ?? true
        };
    }

    #endregion

    #region SalaryGrade Mappings

    public static CreateSalaryGradeDto ToDto(this CreateSalaryGradeInput input)
    {
        return new CreateSalaryGradeDto
        {
            SalaryGradeName = input.SalaryGradeName ?? string.Empty,
            Description = input.Description,
            Step = input.Step ?? 0,
            MonthlySalary = (double)(input.MonthlySalary ?? 0)
        };
    }

    public static UpdateSalaryGradeDto ToDto(this UpdateSalaryGradeInput input)
    {
        return new UpdateSalaryGradeDto
        {
            SalaryGradeName = input.SalaryGradeName,
            Description = input.Description,
            Step = input.Step ?? 0,
            MonthlySalary = (double)(input.MonthlySalary ?? 0),
            IsActive = input.IsActive ?? true
        };
    }

    #endregion

    #region Item Mappings

    public static CreateItemDto ToDto(this CreateItemInput input)
    {
        return new CreateItemDto
        {
            ItemName = input.ItemName ?? string.Empty,
            Description = input.Description
        };
    }

    public static UpdateItemDto ToDto(this UpdateItemInput input)
    {
        return new UpdateItemDto
        {
            ItemName = input.ItemName,
            Description = input.Description,
            IsActive = input.IsActive ?? true
        };
    }

    #endregion
}
