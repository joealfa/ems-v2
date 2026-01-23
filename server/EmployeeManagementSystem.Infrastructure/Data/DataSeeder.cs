using EmployeeManagementSystem.Domain.Entities;
using EmployeeManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Infrastructure.Data;

/// <summary>
/// Provides mock data seeding for the database.
/// </summary>
public static class DataSeeder
{
    private const string SystemUser = "System";

    /// <summary>
    /// Seeds the database with initial mock data.
    /// </summary>
    /// <param name="context">The database context.</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Only seed if the database is empty
        if (await context.Schools.AnyAsync())
        {
            return;
        }

        // Create Schools
        var schools = CreateSchools();
        await context.Schools.AddRangeAsync(schools);

        // Create Positions
        var positions = CreatePositions();
        await context.Positions.AddRangeAsync(positions);

        // Create Salary Grades
        var salaryGrades = CreateSalaryGrades();
        await context.SalaryGrades.AddRangeAsync(salaryGrades);

        // Create Items
        var items = CreateItems();
        await context.Items.AddRangeAsync(items);

        // Create Persons with Addresses and Contacts
        var persons = CreatePersons();
        await context.Persons.AddRangeAsync(persons);

        // Save to get IDs assigned
        await context.SaveChangesAsync();

        // Create Employments
        var employments = CreateEmployments(persons, positions, salaryGrades, items);
        await context.Employments.AddRangeAsync(employments);

        await context.SaveChangesAsync();

        // Create Employment-School relationships
        var employmentSchools = CreateEmploymentSchools(employments, schools);
        await context.EmploymentSchools.AddRangeAsync(employmentSchools);

        await context.SaveChangesAsync();
    }

    private static List<School> CreateSchools()
    {
        return
        [
            new School
            {
                SchoolName = "Marikina Elementary School",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "123 Education Street",
                        Address2 = "Building A",
                        Barangay = "Concepcion Uno",
                        City = "Marikina",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1800",
                        AddressType = AddressType.Business,
                        IsCurrent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09171234567",
                        LandLine = "028123456",
                        Email = "marinakina.es@deped.gov.ph",
                        ContactType = ContactType.Work,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new School
            {
                SchoolName = "Quezon City Science High School",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "456 Science Avenue",
                        Barangay = "Diliman",
                        City = "Quezon City",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1100",
                        AddressType = AddressType.Business,
                        IsCurrent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09189876543",
                        Email = "qcshs@deped.gov.ph",
                        ContactType = ContactType.Work,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new School
            {
                SchoolName = "Manila National High School",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "789 National Road",
                        Barangay = "Ermita",
                        City = "Manila",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1000",
                        AddressType = AddressType.Business,
                        IsCurrent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09201112233",
                        LandLine = "028765432",
                        Email = "mnhs@deped.gov.ph",
                        ContactType = ContactType.Work,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new School
            {
                SchoolName = "Pasig Central Elementary School",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "321 Central Lane",
                        Barangay = "Kapitolyo",
                        City = "Pasig",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1600",
                        AddressType = AddressType.Business,
                        IsCurrent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09173334455",
                        Email = "pces@deped.gov.ph",
                        ContactType = ContactType.Work,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new School
            {
                SchoolName = "Taguig Integrated School",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "555 BGC Boulevard",
                        Barangay = "Fort Bonifacio",
                        City = "Taguig",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1630",
                        AddressType = AddressType.Business,
                        IsCurrent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09185556677",
                        Email = "tis@deped.gov.ph",
                        ContactType = ContactType.Work,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            }
        ];
    }

    private static List<Position> CreatePositions()
    {
        return
        [
            new Position
            {
                TitleName = "Teacher I",
                Description = "Entry-level teaching position for elementary and secondary schools",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Teacher II",
                Description = "Teaching position with 3+ years of experience",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Teacher III",
                Description = "Senior teaching position with 5+ years of experience",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Master Teacher I",
                Description = "Master teacher position for curriculum development",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Master Teacher II",
                Description = "Senior master teacher position",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Head Teacher I",
                Description = "Department head position",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Head Teacher II",
                Description = "Senior department head position",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Principal I",
                Description = "School principal for small schools",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Principal II",
                Description = "School principal for medium schools",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            new Position
            {
                TitleName = "Administrative Officer III",
                Description = "Administrative support position",
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            }
        ];
    }

    private static List<SalaryGrade> CreateSalaryGrades()
    {
        return
        [
            new SalaryGrade { SalaryGradeName = "SG 11", Description = "Salary Grade 11 - Teacher I", Step = 1, MonthlySalary = 27000m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 11", Description = "Salary Grade 11 - Teacher I", Step = 2, MonthlySalary = 27500m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 11", Description = "Salary Grade 11 - Teacher I", Step = 3, MonthlySalary = 28000m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 12", Description = "Salary Grade 12 - Teacher II", Step = 1, MonthlySalary = 29165m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 12", Description = "Salary Grade 12 - Teacher II", Step = 2, MonthlySalary = 29700m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 13", Description = "Salary Grade 13 - Teacher III", Step = 1, MonthlySalary = 31320m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 14", Description = "Salary Grade 14 - Head Teacher I", Step = 1, MonthlySalary = 33843m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 15", Description = "Salary Grade 15 - Head Teacher II", Step = 1, MonthlySalary = 36619m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 18", Description = "Salary Grade 18 - Master Teacher I", Step = 1, MonthlySalary = 46725m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 19", Description = "Salary Grade 19 - Master Teacher II / Principal I", Step = 1, MonthlySalary = 51357m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 20", Description = "Salary Grade 20 - Principal II", Step = 1, MonthlySalary = 57347m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new SalaryGrade { SalaryGradeName = "SG 9", Description = "Salary Grade 9 - Administrative Officer III", Step = 1, MonthlySalary = 23877m, IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow }
        ];
    }

    private static List<Item> CreateItems()
    {
        return
        [
            new Item { ItemName = "OSEC-DECSB-TCH1-1", Description = "Teaching Item 1 - Elementary", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-TCH1-2", Description = "Teaching Item 2 - Elementary", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-TCH1-3", Description = "Teaching Item 3 - Elementary", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-TCH2-1", Description = "Teaching Item 1 - Secondary", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-TCH2-2", Description = "Teaching Item 2 - Secondary", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-TCH3-1", Description = "Teaching Item - Senior", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-MT1-1", Description = "Master Teacher Item 1", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-MT2-1", Description = "Master Teacher Item 2", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-HT1-1", Description = "Head Teacher Item 1", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-HT2-1", Description = "Head Teacher Item 2", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-PRIN1-1", Description = "Principal Item 1", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-PRIN2-1", Description = "Principal Item 2", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Item { ItemName = "OSEC-DECSB-AO3-1", Description = "Administrative Officer Item", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow }
        ];
    }

    private static List<Person> CreatePersons()
    {
        return
        [
            new Person
            {
                FirstName = "Maria",
                LastName = "Santos",
                MiddleName = "Cruz",
                DateOfBirth = new DateOnly(1985, 5, 15),
                Gender = Gender.Female,
                CivilStatus = CivilStatus.Married,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "456 Sampaguita St.",
                        Barangay = "San Antonio",
                        City = "Makati",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1200",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09171234568",
                        Email = "maria.santos@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    },
                    new Contact
                    {
                        Email = "m.santos@deped.gov.ph",
                        ContactType = ContactType.Work,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Juan",
                LastName = "Dela Cruz",
                MiddleName = "Reyes",
                DateOfBirth = new DateOnly(1990, 8, 20),
                Gender = Gender.Male,
                CivilStatus = CivilStatus.Single,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "789 Mabini Ave.",
                        Barangay = "Commonwealth",
                        City = "Quezon City",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1121",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09189876544",
                        Email = "juan.delacruz@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Ana",
                LastName = "Reyes",
                MiddleName = "Garcia",
                DateOfBirth = new DateOnly(1988, 3, 10),
                Gender = Gender.Female,
                CivilStatus = CivilStatus.Married,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "123 Rizal Blvd.",
                        Barangay = "Poblacion",
                        City = "Pasig",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1600",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09201234567",
                        Email = "ana.reyes@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Pedro",
                LastName = "Gonzales",
                MiddleName = "Lopez",
                DateOfBirth = new DateOnly(1982, 11, 25),
                Gender = Gender.Male,
                CivilStatus = CivilStatus.Married,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "567 Bonifacio St.",
                        Barangay = "Barangka",
                        City = "Marikina",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1800",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09173456789",
                        LandLine = "028654321",
                        Email = "pedro.gonzales@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Elena",
                LastName = "Villanueva",
                MiddleName = "Martinez",
                DateOfBirth = new DateOnly(1979, 7, 8),
                Gender = Gender.Female,
                CivilStatus = CivilStatus.Widow,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "234 Aguinaldo Highway",
                        Barangay = "Baclaran",
                        City = "Parañaque",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1700",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09185678901",
                        Email = "elena.villanueva@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Carlos",
                LastName = "Mendoza",
                MiddleName = "Aquino",
                DateOfBirth = new DateOnly(1992, 1, 30),
                Gender = Gender.Male,
                CivilStatus = CivilStatus.Single,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "890 Luna St.",
                        Barangay = "Tondo",
                        City = "Manila",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1000",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09209012345",
                        Email = "carlos.mendoza@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Rosa",
                LastName = "Fernandez",
                MiddleName = "Castro",
                DateOfBirth = new DateOnly(1987, 9, 12),
                Gender = Gender.Female,
                CivilStatus = CivilStatus.Married,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "432 Katipunan Ave.",
                        Barangay = "Loyola Heights",
                        City = "Quezon City",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1108",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09172345678",
                        Email = "rosa.fernandez@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Miguel",
                LastName = "Torres",
                MiddleName = "Ramos",
                DateOfBirth = new DateOnly(1984, 4, 18),
                Gender = Gender.Male,
                CivilStatus = CivilStatus.Married,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "765 Shaw Blvd.",
                        Barangay = "Wack-Wack",
                        City = "Mandaluyong",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1550",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09183456789",
                        Email = "miguel.torres@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Lucia",
                LastName = "Bautista",
                MiddleName = "Santos",
                DateOfBirth = new DateOnly(1991, 6, 22),
                Gender = Gender.Female,
                CivilStatus = CivilStatus.Single,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "321 C5 Road",
                        Barangay = "Signal Village",
                        City = "Taguig",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1630",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09204567890",
                        Email = "lucia.bautista@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            },
            new Person
            {
                FirstName = "Roberto",
                LastName = "Garcia",
                MiddleName = "Pascual",
                DateOfBirth = new DateOnly(1980, 12, 5),
                Gender = Gender.Male,
                CivilStatus = CivilStatus.Married,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = "654 EDSA",
                        Barangay = "Cubao",
                        City = "Quezon City",
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = "1109",
                        AddressType = AddressType.Home,
                        IsCurrent = true,
                        IsPermanent = true,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ],
                Contacts =
                [
                    new Contact
                    {
                        Mobile = "09175678901",
                        LandLine = "027891234",
                        Email = "roberto.garcia@email.com",
                        ContactType = ContactType.Personal,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            }
        ];
    }

    private static List<Employment> CreateEmployments(
        List<Person> persons,
        List<Position> positions,
        List<SalaryGrade> salaryGrades,
        List<Item> items)
    {
        var employments = new List<Employment>
        {
            // Maria Santos - Master Teacher II
            new Employment
            {
                DepEdId = "DEPED-2010-001234",
                PSIPOPItemNumber = "PSIPOP-001",
                TINId = "123-456-789-000",
                GSISId = "1234567890",
                PhilHealthId = "12-345678901-2",
                DateOfOriginalAppointment = new DateOnly(2010, 6, 1),
                AppointmentStatus = AppointmentStatus.Promotion,
                EmploymentStatus = EmploymentStatus.Permanent,
                Eligibility = Eligibility.LET,
                IsActive = true,
                PersonId = persons[0].Id,
                PositionId = positions[4].Id, // Master Teacher II
                SalaryGradeId = salaryGrades[9].Id, // SG 19
                ItemId = items[7].Id, // MT2-1
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Juan Dela Cruz - Teacher I
            new Employment
            {
                DepEdId = "DEPED-2020-005678",
                PSIPOPItemNumber = "PSIPOP-002",
                TINId = "234-567-890-001",
                GSISId = "2345678901",
                PhilHealthId = "23-456789012-3",
                DateOfOriginalAppointment = new DateOnly(2020, 7, 15),
                AppointmentStatus = AppointmentStatus.Original,
                EmploymentStatus = EmploymentStatus.Regular,
                Eligibility = Eligibility.LET,
                IsActive = true,
                PersonId = persons[1].Id,
                PositionId = positions[0].Id, // Teacher I
                SalaryGradeId = salaryGrades[0].Id, // SG 11 Step 1
                ItemId = items[0].Id, // TCH1-1
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Ana Reyes - Teacher III
            new Employment
            {
                DepEdId = "DEPED-2015-003456",
                PSIPOPItemNumber = "PSIPOP-003",
                TINId = "345-678-901-002",
                GSISId = "3456789012",
                PhilHealthId = "34-567890123-4",
                DateOfOriginalAppointment = new DateOnly(2015, 3, 1),
                AppointmentStatus = AppointmentStatus.Promotion,
                EmploymentStatus = EmploymentStatus.Permanent,
                Eligibility = Eligibility.LET,
                IsActive = true,
                PersonId = persons[2].Id,
                PositionId = positions[2].Id, // Teacher III
                SalaryGradeId = salaryGrades[5].Id, // SG 13
                ItemId = items[5].Id, // TCH3-1
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Pedro Gonzales - Principal II
            new Employment
            {
                DepEdId = "DEPED-2005-001111",
                PSIPOPItemNumber = "PSIPOP-004",
                TINId = "456-789-012-003",
                GSISId = "4567890123",
                PhilHealthId = "45-678901234-5",
                DateOfOriginalAppointment = new DateOnly(2005, 8, 1),
                AppointmentStatus = AppointmentStatus.Promotion,
                EmploymentStatus = EmploymentStatus.Permanent,
                Eligibility = Eligibility.CivilServiceProfessional,
                IsActive = true,
                PersonId = persons[3].Id,
                PositionId = positions[8].Id, // Principal II
                SalaryGradeId = salaryGrades[10].Id, // SG 20
                ItemId = items[11].Id, // PRIN2-1
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Elena Villanueva - Head Teacher II
            new Employment
            {
                DepEdId = "DEPED-2008-002222",
                PSIPOPItemNumber = "PSIPOP-005",
                TINId = "567-890-123-004",
                GSISId = "5678901234",
                PhilHealthId = "56-789012345-6",
                DateOfOriginalAppointment = new DateOnly(2008, 1, 15),
                AppointmentStatus = AppointmentStatus.Promotion,
                EmploymentStatus = EmploymentStatus.Permanent,
                Eligibility = Eligibility.LET,
                IsActive = true,
                PersonId = persons[4].Id,
                PositionId = positions[6].Id, // Head Teacher II
                SalaryGradeId = salaryGrades[7].Id, // SG 15
                ItemId = items[9].Id, // HT2-1
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Carlos Mendoza - Teacher I
            new Employment
            {
                DepEdId = "DEPED-2022-007890",
                PSIPOPItemNumber = "PSIPOP-006",
                TINId = "678-901-234-005",
                GSISId = "6789012345",
                PhilHealthId = "67-890123456-7",
                DateOfOriginalAppointment = new DateOnly(2022, 9, 1),
                AppointmentStatus = AppointmentStatus.Original,
                EmploymentStatus = EmploymentStatus.Regular,
                Eligibility = Eligibility.LET,
                IsActive = true,
                PersonId = persons[5].Id,
                PositionId = positions[0].Id, // Teacher I
                SalaryGradeId = salaryGrades[0].Id, // SG 11 Step 1
                ItemId = items[1].Id, // TCH1-2
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Rosa Fernandez - Teacher II
            new Employment
            {
                DepEdId = "DEPED-2018-004567",
                PSIPOPItemNumber = "PSIPOP-007",
                TINId = "789-012-345-006",
                GSISId = "7890123456",
                PhilHealthId = "78-901234567-8",
                DateOfOriginalAppointment = new DateOnly(2018, 6, 15),
                AppointmentStatus = AppointmentStatus.Promotion,
                EmploymentStatus = EmploymentStatus.Permanent,
                Eligibility = Eligibility.LET,
                IsActive = true,
                PersonId = persons[6].Id,
                PositionId = positions[1].Id, // Teacher II
                SalaryGradeId = salaryGrades[3].Id, // SG 12 Step 1
                ItemId = items[3].Id, // TCH2-1
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Miguel Torres - Master Teacher I
            new Employment
            {
                DepEdId = "DEPED-2012-003333",
                PSIPOPItemNumber = "PSIPOP-008",
                TINId = "890-123-456-007",
                GSISId = "8901234567",
                PhilHealthId = "89-012345678-9",
                DateOfOriginalAppointment = new DateOnly(2012, 4, 1),
                AppointmentStatus = AppointmentStatus.Promotion,
                EmploymentStatus = EmploymentStatus.Permanent,
                Eligibility = Eligibility.LET,
                IsActive = true,
                PersonId = persons[7].Id,
                PositionId = positions[3].Id, // Master Teacher I
                SalaryGradeId = salaryGrades[8].Id, // SG 18
                ItemId = items[6].Id, // MT1-1
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Lucia Bautista - Teacher I
            new Employment
            {
                DepEdId = "DEPED-2021-006789",
                PSIPOPItemNumber = "PSIPOP-009",
                TINId = "901-234-567-008",
                GSISId = "9012345678",
                PhilHealthId = "90-123456789-0",
                DateOfOriginalAppointment = new DateOnly(2021, 8, 15),
                AppointmentStatus = AppointmentStatus.Original,
                EmploymentStatus = EmploymentStatus.Regular,
                Eligibility = Eligibility.LET,
                IsActive = true,
                PersonId = persons[8].Id,
                PositionId = positions[0].Id, // Teacher I
                SalaryGradeId = salaryGrades[1].Id, // SG 11 Step 2
                ItemId = items[2].Id, // TCH1-3
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },

            // Roberto Garcia - Administrative Officer III
            new Employment
            {
                DepEdId = "DEPED-2006-001000",
                PSIPOPItemNumber = "PSIPOP-010",
                TINId = "012-345-678-009",
                GSISId = "0123456789",
                PhilHealthId = "01-234567890-1",
                DateOfOriginalAppointment = new DateOnly(2006, 2, 1),
                AppointmentStatus = AppointmentStatus.Original,
                EmploymentStatus = EmploymentStatus.Permanent,
                Eligibility = Eligibility.CivilServiceSubProfessional,
                IsActive = true,
                PersonId = persons[9].Id,
                PositionId = positions[9].Id, // Administrative Officer III
                SalaryGradeId = salaryGrades[11].Id, // SG 9
                ItemId = items[12].Id, // AO3-1
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            }
        };

        return employments;
    }

    private static List<EmploymentSchool> CreateEmploymentSchools(List<Employment> employments, List<School> schools)
    {
        return
        [
            // Maria Santos at Quezon City Science High School
            new EmploymentSchool
            {
                EmploymentId = employments[0].Id,
                SchoolId = schools[1].Id,
                StartDate = new DateOnly(2018, 6, 1),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Juan Dela Cruz at Marikina Elementary School
            new EmploymentSchool
            {
                EmploymentId = employments[1].Id,
                SchoolId = schools[0].Id,
                StartDate = new DateOnly(2020, 7, 15),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Ana Reyes at Manila National High School
            new EmploymentSchool
            {
                EmploymentId = employments[2].Id,
                SchoolId = schools[2].Id,
                StartDate = new DateOnly(2019, 6, 1),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Pedro Gonzales at Pasig Central Elementary School (Principal)
            new EmploymentSchool
            {
                EmploymentId = employments[3].Id,
                SchoolId = schools[3].Id,
                StartDate = new DateOnly(2015, 1, 1),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Elena Villanueva at Taguig Integrated School
            new EmploymentSchool
            {
                EmploymentId = employments[4].Id,
                SchoolId = schools[4].Id,
                StartDate = new DateOnly(2016, 6, 1),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Carlos Mendoza at Marikina Elementary School
            new EmploymentSchool
            {
                EmploymentId = employments[5].Id,
                SchoolId = schools[0].Id,
                StartDate = new DateOnly(2022, 9, 1),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Rosa Fernandez at Quezon City Science High School
            new EmploymentSchool
            {
                EmploymentId = employments[6].Id,
                SchoolId = schools[1].Id,
                StartDate = new DateOnly(2018, 6, 15),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Miguel Torres at Manila National High School
            new EmploymentSchool
            {
                EmploymentId = employments[7].Id,
                SchoolId = schools[2].Id,
                StartDate = new DateOnly(2017, 1, 1),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Lucia Bautista at Taguig Integrated School
            new EmploymentSchool
            {
                EmploymentId = employments[8].Id,
                SchoolId = schools[4].Id,
                StartDate = new DateOnly(2021, 8, 15),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            },
            // Roberto Garcia at Pasig Central Elementary School (Admin)
            new EmploymentSchool
            {
                EmploymentId = employments[9].Id,
                SchoolId = schools[3].Id,
                StartDate = new DateOnly(2010, 3, 1),
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            }
        ];
    }
}
