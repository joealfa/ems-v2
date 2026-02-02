using EmployeeManagementSystem.ApiClient.Generated;
using EmployeeManagementSystem.Gateway.Types.Inputs;

namespace EmployeeManagementSystem.Gateway.Mappings;

public static class InputMappingExtensions
{
    #region Address Mappings

    extension(CreateAddressInput input)
    {
        public CreateAddressDto ToDto()
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
    }

    extension(UpsertAddressInput input)
    {
        public UpsertAddressDto ToDto()
        {
            return new UpsertAddressDto
            {
                DisplayId = input.DisplayId,
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
    }

    extension(List<CreateAddressInput>? inputs)
    {
        public List<CreateAddressDto>? ToDtos()
        {
            return inputs?.Select(i => i.ToDto()).ToList();
        }
    }

    extension(List<UpsertAddressInput>? inputs)
    {
        public List<UpsertAddressDto>? ToDtos()
        {
            return inputs?.Select(i => i.ToDto()).ToList();
        }
    }

    #endregion

    #region Contact Mappings

    extension(CreateContactInput input)
    {
        public CreateContactDto ToDto()
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
    }

    extension(UpsertContactInput input)
    {
        public UpsertContactDto ToDto()
        {
            return new UpsertContactDto
            {
                DisplayId = input.DisplayId,
                ContactType = input.ContactType ?? ContactType.Work,
                Email = input.Email,
                Fax = input.Fax,
                LandLine = input.LandLine,
                Mobile = input.Mobile
            };
        }
    }

    extension(List<CreateContactInput>? inputs)
    {
        public List<CreateContactDto>? ToDtos()
        {
            return inputs?.Select(i => i.ToDto()).ToList();
        }
    }

    extension(List<UpsertContactInput>? inputs)
    {
        public List<UpsertContactDto>? ToDtos()
        {
            return inputs?.Select(i => i.ToDto()).ToList();
        }
    }

    #endregion

    #region Person Mappings

    extension(CreatePersonInput input)
    {
        public CreatePersonDto ToDto()
        {
            return new CreatePersonDto
            {
                FirstName = input.FirstName ?? string.Empty,
                MiddleName = input.MiddleName,
                LastName = input.LastName ?? string.Empty,
                DateOfBirth = input.DateOfBirth.HasValue ? new DateTimeOffset(input.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)) : DateTimeOffset.MinValue,
                Gender = input.Gender ?? Gender.Male,
                CivilStatus = input.CivilStatus ?? CivilStatus.Single,
                Addresses = input.Addresses?.ToDtos(),
                Contacts = input.Contacts?.ToDtos()
            };
        }
    }

    extension(UpdatePersonInput input)
    {
        public UpdatePersonDto ToDto()
        {
            return new UpdatePersonDto
            {
                FirstName = input.FirstName ?? string.Empty,
                MiddleName = input.MiddleName,
                LastName = input.LastName ?? string.Empty,
                DateOfBirth = input.DateOfBirth.HasValue ? new DateTimeOffset(input.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)) : DateTimeOffset.MinValue,
                Gender = input.Gender ?? Gender.Male,
                CivilStatus = input.CivilStatus ?? CivilStatus.Single,
                Addresses = input.Addresses?.ToDtos(),
                Contacts = input.Contacts?.ToDtos()
            };
        }
    }

    #endregion

    #region Employment Mappings

    extension(CreateEmploymentSchoolInput input)
    {
        public CreateEmploymentSchoolDto ToDto()
        {
            return new CreateEmploymentSchoolDto
            {
                SchoolDisplayId = input.SchoolDisplayId ?? 0,
                StartDate = input.StartDate.HasValue ? new DateTimeOffset(input.StartDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)) : null,
                EndDate = input.EndDate.HasValue ? new DateTimeOffset(input.EndDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)) : null,
                IsCurrent = input.IsCurrent ?? false
            };
        }
    }

    extension(List<CreateEmploymentSchoolInput>? inputs)
    {
        public List<CreateEmploymentSchoolDto>? ToDtos()
        {
            return inputs?.Select(i => i.ToDto()).ToList();
        }
    }

    extension(CreateEmploymentInput input)
    {
        public CreateEmploymentDto ToDto()
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
                DateOfOriginalAppointment = input.DateOfOriginalAppointment.HasValue ? new DateTimeOffset(input.DateOfOriginalAppointment.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)) : null,
                PsipopItemNumber = input.PsipopItemNumber,
                DepEdId = input.DepEdId,
                GsisId = input.GsisId,
                PhilHealthId = input.PhilHealthId,
                TinId = input.TinId,
                Schools = input.Schools?.ToDtos()
            };
        }
    }

    extension(UpdateEmploymentInput input)
    {
        public UpdateEmploymentDto ToDto()
        {
            return new UpdateEmploymentDto
            {
                PositionDisplayId = input.PositionDisplayId ?? 0,
                SalaryGradeDisplayId = input.SalaryGradeDisplayId ?? 0,
                ItemDisplayId = input.ItemDisplayId ?? 0,
                EmploymentStatus = input.EmploymentStatus ?? EmploymentStatus.Regular,
                AppointmentStatus = input.AppointmentStatus ?? AppointmentStatus.Original,
                Eligibility = input.Eligibility ?? Eligibility.LET,
                DateOfOriginalAppointment = input.DateOfOriginalAppointment.HasValue ? new DateTimeOffset(input.DateOfOriginalAppointment.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)) : null,
                PsipopItemNumber = input.PsipopItemNumber,
                DepEdId = input.DepEdId,
                GsisId = input.GsisId,
                PhilHealthId = input.PhilHealthId,
                TinId = input.TinId,
                IsActive = input.IsActive ?? true,
                Schools = input.Schools?.ToDtos()
            };
        }
    }

    extension(UpsertEmploymentSchoolInput input)
    {
        public UpsertEmploymentSchoolDto ToDto()
        {
            return new UpsertEmploymentSchoolDto
            {
                DisplayId = input.DisplayId,
                SchoolDisplayId = input.SchoolDisplayId ?? 0,
                StartDate = input.StartDate.HasValue ? new DateTimeOffset(input.StartDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)) : null,
                EndDate = input.EndDate.HasValue ? new DateTimeOffset(input.EndDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)) : null,
                IsCurrent = input.IsCurrent ?? true
            };
        }
    }

    extension(List<UpsertEmploymentSchoolInput>? inputs)
    {
        public List<UpsertEmploymentSchoolDto>? ToDtos()
        {
            return inputs?.Select(i => i.ToDto()).ToList();
        }
    }

    #endregion

    #region School Mappings

    extension(CreateSchoolInput input)
    {
        public CreateSchoolDto ToDto()
        {
            return new CreateSchoolDto
            {
                SchoolName = input.SchoolName ?? string.Empty,
                Addresses = input.Addresses?.ToDtos(),
                Contacts = input.Contacts?.ToDtos()
            };
        }
    }

    extension(UpdateSchoolInput input)
    {
        public UpdateSchoolDto ToDto()
        {
            return new UpdateSchoolDto
            {
                SchoolName = input.SchoolName,
                Addresses = input.Addresses?.ToDtos(),
                Contacts = input.Contacts?.ToDtos(),
                IsActive = input.IsActive ?? true
            };
        }
    }

    #endregion

    #region Position Mappings

    extension(CreatePositionInput input)
    {
        public CreatePositionDto ToDto()
        {
            return new CreatePositionDto
            {
                TitleName = input.TitleName ?? string.Empty,
                Description = input.Description
            };
        }
    }

    extension(UpdatePositionInput input)
    {
        public UpdatePositionDto ToDto()
        {
            return new UpdatePositionDto
            {
                TitleName = input.TitleName,
                Description = input.Description,
                IsActive = input.IsActive ?? true
            };
        }
    }

    #endregion

    #region SalaryGrade Mappings

    extension(CreateSalaryGradeInput input)
    {
        public CreateSalaryGradeDto ToDto()
        {
            return new CreateSalaryGradeDto
            {
                SalaryGradeName = input.SalaryGradeName ?? string.Empty,
                Description = input.Description,
                Step = input.Step ?? 0,
                MonthlySalary = (double)(input.MonthlySalary ?? 0)
            };
        }
    }

    extension(UpdateSalaryGradeInput input)
    {
        public UpdateSalaryGradeDto ToDto()
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
    }

    #endregion

    #region Item Mappings

    extension(CreateItemInput input)
    {
        public CreateItemDto ToDto()
        {
            return new CreateItemDto
            {
                ItemName = input.ItemName ?? string.Empty,
                Description = input.Description
            };
        }
    }

    extension(UpdateItemInput input)
    {
        public UpdateItemDto ToDto()
        {
            return new UpdateItemDto
            {
                ItemName = input.ItemName,
                Description = input.Description,
                IsActive = input.IsActive ?? true
            };
        }
    }

    #endregion
}
