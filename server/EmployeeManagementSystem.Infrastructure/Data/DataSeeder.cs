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
    private const int PersonCount = 5000;
    private static readonly Random Random = new(42); // Fixed seed for reproducibility

    // Filipino first names
    private static readonly string[] MaleFirstNames =
    [
        "Juan", "Pedro", "Jose", "Carlos", "Miguel", "Antonio", "Roberto", "Ricardo", "Eduardo", "Fernando",
        "Rafael", "Manuel", "Francisco", "Alejandro", "Gabriel", "Luis", "Marco", "Paolo", "Andres", "Ramon",
        "Ernesto", "Alfredo", "Vicente", "Domingo", "Reynaldo", "Armando", "Leonardo", "Rolando", "Benjamin", "Romeo",
        "Danilo", "Orlando", "Virgilio", "Mariano", "Nestor", "Rodolfo", "Arturo", "Renato", "Felix", "Sergio",
        "Jaime", "Teodoro", "Gregorio", "Ismael", "Leandro", "Marcelo", "Noel", "Oscar", "Patricio", "Quintin",
        "Rodel", "Salvador", "Tomas", "Ulysses", "Victor", "William", "Xavier", "Yosef", "Zeno", "Adrian",
        "Bryan", "Christian", "Dennis", "Elmer", "Francis", "Gerald", "Harold", "Ivan", "Jerome", "Kenneth",
        "Larry", "Michael", "Nathan", "Oliver", "Patrick", "Quincy", "Ronald", "Stephen", "Timothy", "Vincent"
    ];

    private static readonly string[] FemaleFirstNames =
    [
        "Maria", "Ana", "Rosa", "Elena", "Carmen", "Lucia", "Teresa", "Josefina", "Guadalupe", "Patricia",
        "Gloria", "Esperanza", "Dolores", "Beatriz", "Cristina", "Diana", "Estela", "Francisca", "Gabriela", "Helena",
        "Isabel", "Julia", "Katherine", "Leonora", "Margarita", "Norma", "Olivia", "Pilar", "Regina", "Sofia",
        "Valentina", "Yolanda", "Zenaida", "Adelaida", "Barbara", "Cecilia", "Daniela", "Evelyn", "Felicia", "Geraldine",
        "Herminia", "Imelda", "Jasmine", "Karen", "Lourdes", "Maribel", "Natividad", "Ophelia", "Paulina", "Queenie",
        "Rosalinda", "Stephanie", "Theresa", "Ursula", "Veronica", "Wendy", "Ximena", "Yvonne", "Zoe", "Angela",
        "Bernadette", "Charlene", "Desiree", "Emily", "Florence", "Grace", "Hannah", "Irene", "Jennifer", "Katrina",
        "Liza", "Michelle", "Nicole", "Paula", "Rachel", "Sarah", "Trisha", "Uma", "Vivian", "Wanda"
    ];

    private static readonly string[] LastNames =
    [
        "Santos", "Reyes", "Cruz", "Bautista", "Garcia", "Mendoza", "Torres", "Gonzales", "Fernandez", "Lopez",
        "Martinez", "Rodriguez", "Hernandez", "Ramos", "Aquino", "Villanueva", "Castro", "Pascual", "Dela Cruz", "Flores",
        "Rivera", "Morales", "Perez", "Sanchez", "Jimenez", "Romero", "Diaz", "Alvarez", "Vargas", "Castillo",
        "Gutierrez", "Ortega", "Salazar", "Mercado", "Soriano", "Tan", "Lim", "Chua", "Uy", "Go",
        "Sy", "Co", "Ang", "Ong", "Yap", "Chan", "Lee", "Wong", "Ng", "Tiu",
        "Aguilar", "Domingo", "Francisco", "Ignacio", "Laurel", "Magsaysay", "Osmeña", "Quezon", "Roxas", "Bonifacio",
        "Mabini", "Luna", "Del Pilar", "Jacinto", "Silang", "Tupas", "Legaspi", "Urdaneta", "Salcedo", "Lacson",
        "Escudero", "Cayetano", "Ejercito", "Marcos", "Arroyo", "Estrada", "Duterte", "Pacquiao", "Sotto", "Villar",
        "Pangilinan", "Binay", "Poe", "Robredo", "Moreno", "Isko", "Lacson", "Defensor", "Abalos", "Remulla",
        "Gatchalian", "Tolentino", "Recto", "Angara", "Drilon", "Enrile", "Honasan", "Trillanes", "Hontiveros", "Villanueva"
    ];

    private static readonly string[] MiddleNames =
    [
        "Cruz", "Reyes", "Santos", "Garcia", "Lopez", "Aquino", "Ramos", "Castro", "Martinez", "Gonzales",
        "Fernandez", "Rodriguez", "Hernandez", "Flores", "Rivera", "Morales", "Perez", "Sanchez", "Jimenez", "Romero"
    ];

    private static readonly string[] Cities =
    [
        "Manila", "Quezon City", "Makati", "Pasig", "Taguig", "Mandaluyong", "San Juan", "Marikina",
        "Parañaque", "Las Piñas", "Muntinlupa", "Caloocan", "Malabon", "Navotas", "Valenzuela", "Pasay"
    ];

    private static readonly string[] Barangays =
    [
        "Poblacion", "San Antonio", "San Jose", "Santo Niño", "Santa Cruz", "Concepcion", "Bagumbayan", "Kapitolyo",
        "Ugong", "Pinagkaisahan", "Manggahan", "Rosario", "Rizal", "Malanday", "Parang", "Tumana",
        "Industrial Valley", "Santa Elena", "Wack-Wack", "Addition Hills", "Hagdang Bato", "Plainview", "Barangka", "Marikina Heights"
    ];

    private static readonly string[] StreetNames =
    [
        "Rizal", "Mabini", "Bonifacio", "Aguinaldo", "Quezon", "Roxas", "Osmena", "Laurel",
        "Magsaysay", "Garcia", "Macapagal", "Ramos", "Estrada", "Arroyo", "Aquino", "Marcos",
        "Luna", "Del Pilar", "Jacinto", "Silang", "Katipunan", "EDSA", "Shaw", "Ortigas"
    ];

    private static readonly string[] StreetTypes = ["St.", "Ave.", "Blvd.", "Road", "Drive", "Lane", "Highway", "Circle"];

    private static readonly string[] EmailDomains = ["email.com", "gmail.com", "yahoo.com", "outlook.com", "hotmail.com", "mail.com"];

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
        List<School> schools = CreateSchools();
        await context.Schools.AddRangeAsync(schools);

        // Create Positions
        List<Position> positions = CreatePositions();
        await context.Positions.AddRangeAsync(positions);

        // Create Salary Grades
        List<SalaryGrade> salaryGrades = CreateSalaryGrades();
        await context.SalaryGrades.AddRangeAsync(salaryGrades);

        // Create Items - generate enough items for all employments
        List<Item> items = CreateItems();
        await context.Items.AddRangeAsync(items);

        // Create Persons with Addresses and Contacts in batches
        List<Person> persons = CreatePersons();

        // Add persons in batches to avoid memory issues
        const int batchSize = 500;
        for (int i = 0; i < persons.Count; i += batchSize)
        {
            List<Person> batch = persons.Skip(i).Take(batchSize).ToList();
            await context.Persons.AddRangeAsync(batch);
            _ = await context.SaveChangesAsync();
        }

        // Create Employments in batches
        List<Employment> employments = CreateEmployments(persons, positions, salaryGrades, items);
        for (int i = 0; i < employments.Count; i += batchSize)
        {
            List<Employment> batch = employments.Skip(i).Take(batchSize).ToList();
            await context.Employments.AddRangeAsync(batch);
            _ = await context.SaveChangesAsync();
        }

        // Create Employment-School relationships in batches
        List<EmploymentSchool> employmentSchools = CreateEmploymentSchools(employments, schools);
        for (int i = 0; i < employmentSchools.Count; i += batchSize)
        {
            List<EmploymentSchool> batch = employmentSchools.Skip(i).Take(batchSize).ToList();
            await context.EmploymentSchools.AddRangeAsync(batch);
            _ = await context.SaveChangesAsync();
        }
    }

    private static string GetRandomMobileNumber()
    {
        string[] prefixes = new[] { "0917", "0918", "0919", "0920", "0921", "0927", "0928", "0929", "0939", "0949" };
        return $"{prefixes[Random.Next(prefixes.Length)]}{Random.Next(1000000, 9999999)}";
    }

    private static string GetRandomLandline()
    {
        return $"02{Random.Next(10000000, 99999999)}";
    }

    private static string GetRandomZipCode()
    {
        return Random.Next(1000, 1999).ToString();
    }

    private static DateOnly GetRandomBirthDate()
    {
        // Generate birth dates for people aged 22-65
        int minYear = DateTime.UtcNow.Year - 65;
        int maxYear = DateTime.UtcNow.Year - 22;
        int year = Random.Next(minYear, maxYear + 1);
        int month = Random.Next(1, 13);
        int day = Random.Next(1, DateTime.DaysInMonth(year, month) + 1);
        return new DateOnly(year, month, day);
    }

    private static DateOnly GetRandomAppointmentDate(DateOnly birthDate)
    {
        // Appointment should be at least 22 years after birth (minimum working age)
        int minYear = birthDate.Year + 22;
        int maxYear = DateTime.UtcNow.Year;
        if (minYear > maxYear)
        {
            minYear = maxYear;
        }

        int year = Random.Next(minYear, maxYear + 1);
        int month = Random.Next(1, 13);
        int day = Random.Next(1, Math.Min(28, DateTime.DaysInMonth(year, month)) + 1);
        return new DateOnly(year, month, day);
    }

    private static List<Address> CreateRandomAddresses(int count)
    {
        List<Address> addresses = [];
        AddressType[] addressTypes = Enum.GetValues<AddressType>();

        for (int i = 0; i < count; i++)
        {
            string city = Cities[Random.Next(Cities.Length)];
            addresses.Add(new Address
            {
                Address1 = $"{Random.Next(1, 9999)} {StreetNames[Random.Next(StreetNames.Length)]} {StreetTypes[Random.Next(StreetTypes.Length)]}",
                Address2 = Random.Next(100) < 30 ? $"Unit {Random.Next(1, 100)}" : null,
                Barangay = Barangays[Random.Next(Barangays.Length)],
                City = city,
                Province = "Metro Manila",
                Country = "Philippines",
                ZipCode = GetRandomZipCode(),
                AddressType = i == 0 ? AddressType.Home : addressTypes[Random.Next(addressTypes.Length)],
                IsCurrent = i == 0,
                IsPermanent = i == 0 || Random.Next(100) < 50,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            });
        }

        return addresses;
    }

    private static List<Contact> CreateRandomContacts(string firstName, string lastName, int count)
    {
        List<Contact> contacts = [];
        ContactType[] contactTypes = Enum.GetValues<ContactType>();
        string normalizedFirst = firstName.ToLower().Replace(" ", "");
        string normalizedLast = lastName.ToLower().Replace(" ", "");

        for (int i = 0; i < count; i++)
        {
            ContactType contactType = i == 0 ? ContactType.Personal : contactTypes[Random.Next(contactTypes.Length)];
            int emailVariant = Random.Next(4);
            string email = emailVariant switch
            {
                0 => $"{normalizedFirst}.{normalizedLast}{Random.Next(100)}@{EmailDomains[Random.Next(EmailDomains.Length)]}",
                1 => $"{normalizedFirst}{normalizedLast[0]}{Random.Next(1000)}@{EmailDomains[Random.Next(EmailDomains.Length)]}",
                2 => $"{normalizedFirst[0]}{normalizedLast}{Random.Next(100)}@{EmailDomains[Random.Next(EmailDomains.Length)]}",
                _ => $"{normalizedFirst}_{normalizedLast}@{EmailDomains[Random.Next(EmailDomains.Length)]}"
            };

            contacts.Add(new Contact
            {
                Mobile = GetRandomMobileNumber(),
                LandLine = Random.Next(100) < 40 ? GetRandomLandline() : null,
                Email = email,
                ContactType = contactType,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            });
        }

        return contacts;
    }

    private static List<School> CreateSchools()
    {
        string[] schoolTypes = new[] { "Elementary School", "High School", "National High School", "Science High School", "Integrated School", "Central School" };
        List<School> schools = [];

        // Create 50 schools to have a good distribution for 5000 employees
        for (int i = 0; i < 50; i++)
        {
            string city = Cities[i % Cities.Length];
            string schoolType = schoolTypes[Random.Next(schoolTypes.Length)];
            string schoolName = $"{city} {schoolType} {(i / Cities.Length) + 1}";

            schools.Add(new School
            {
                SchoolName = schoolName,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses =
                [
                    new Address
                    {
                        Address1 = $"{Random.Next(1, 999)} {StreetNames[Random.Next(StreetNames.Length)]} {StreetTypes[Random.Next(StreetTypes.Length)]}",
                        Address2 = Random.Next(100) < 30 ? $"Building {(char)('A' + Random.Next(5))}" : null,
                        Barangay = Barangays[Random.Next(Barangays.Length)],
                        City = city,
                        Province = "Metro Manila",
                        Country = "Philippines",
                        ZipCode = GetRandomZipCode(),
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
                        Mobile = GetRandomMobileNumber(),
                        LandLine = GetRandomLandline(),
                        Email = $"{schoolName.ToLower().Replace(" ", ".").Replace("-", "")}@deped.gov.ph",
                        ContactType = ContactType.Work,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    }
                ]
            });
        }

        return schools;
    }

    private static List<Position> CreatePositions()
    {
        return
        [
            new Position { TitleName = "Teacher I", Description = "Entry-level teaching position for elementary and secondary schools", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Teacher II", Description = "Teaching position with 3+ years of experience", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Teacher III", Description = "Senior teaching position with 5+ years of experience", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Master Teacher I", Description = "Master teacher position for curriculum development", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Master Teacher II", Description = "Senior master teacher position", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Head Teacher I", Description = "Department head position", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Head Teacher II", Description = "Senior department head position", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Principal I", Description = "School principal for small schools", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Principal II", Description = "School principal for medium schools", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow },
            new Position { TitleName = "Administrative Officer III", Description = "Administrative support position", IsActive = true, CreatedBy = SystemUser, CreatedOn = DateTime.UtcNow }
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
        List<Item> items = [];
        (string, string)[] itemTypes = new[]
        {
            ("TCH1", "Teaching Item - Teacher I"),
            ("TCH2", "Teaching Item - Teacher II"),
            ("TCH3", "Teaching Item - Teacher III"),
            ("MT1", "Master Teacher Item I"),
            ("MT2", "Master Teacher Item II"),
            ("HT1", "Head Teacher Item I"),
            ("HT2", "Head Teacher Item II"),
            ("PRIN1", "Principal Item I"),
            ("PRIN2", "Principal Item II"),
            ("AO3", "Administrative Officer Item")
        };

        // Create enough items for 5000 employees with some buffer
        int itemNumber = 1;
        foreach ((string? code, string? description) in itemTypes)
        {
            // Distribute items based on typical organizational structure
            int count = code switch
            {
                "TCH1" => 2000, // Most employees are Teacher I
                "TCH2" => 1200,
                "TCH3" => 800,
                "MT1" => 300,
                "MT2" => 200,
                "HT1" => 200,
                "HT2" => 100,
                "PRIN1" => 80,
                "PRIN2" => 50,
                "AO3" => 200,
                _ => 100
            };

            for (int i = 1; i <= count; i++)
            {
                items.Add(new Item
                {
                    ItemName = $"OSEC-DECSB-{code}-{i:D4}",
                    Description = $"{description} #{i}",
                    IsActive = true,
                    CreatedBy = SystemUser,
                    CreatedOn = DateTime.UtcNow
                });
                itemNumber++;
            }
        }

        return items;
    }

    private static List<Person> CreatePersons()
    {
        List<Person> persons = [];
        _ = Enum.GetValues<Gender>();
        CivilStatus[] civilStatuses = Enum.GetValues<CivilStatus>();

        for (int i = 0; i < PersonCount; i++)
        {
            bool isMale = Random.Next(100) < 50;
            Gender gender = isMale ? Gender.Male : Gender.Female;
            string firstName = isMale
                ? MaleFirstNames[Random.Next(MaleFirstNames.Length)]
                : FemaleFirstNames[Random.Next(FemaleFirstNames.Length)];
            string lastName = LastNames[Random.Next(LastNames.Length)];
            string middleName = MiddleNames[Random.Next(MiddleNames.Length)];

            // Random number of addresses (1-4)
            int addressCount = Random.Next(1, 5);
            // Random number of contacts (1-4)
            int contactCount = Random.Next(1, 5);

            persons.Add(new Person
            {
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
                DateOfBirth = GetRandomBirthDate(),
                Gender = gender,
                CivilStatus = civilStatuses[Random.Next(civilStatuses.Length)],
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow,
                Addresses = CreateRandomAddresses(addressCount),
                Contacts = CreateRandomContacts(firstName, lastName, contactCount)
            });
        }

        return persons;
    }

    private static List<Employment> CreateEmployments(
        List<Person> persons,
        List<Position> positions,
        List<SalaryGrade> salaryGrades,
        List<Item> items)
    {
        List<Employment> employments = [];
        _ = Enum.GetValues<AppointmentStatus>();
        _ = Enum.GetValues<EmploymentStatus>();
        Eligibility[] eligibilities = Enum.GetValues<Eligibility>();

        // Position distribution weights (index matches positions list)
        // Teacher I: 40%, Teacher II: 24%, Teacher III: 16%, MT1: 6%, MT2: 4%, HT1: 4%, HT2: 2%, Prin1: 1.6%, Prin2: 1%, AO3: 1.4%
        int[] positionWeights = new[] { 40, 24, 16, 6, 4, 4, 2, 2, 1, 1 };
        int totalWeight = positionWeights.Sum();

        // Salary grade mapping per position index
        Dictionary<int, int[]> positionToSalaryGrade = new()
        {
            { 0, new[] { 0, 1, 2 } },      // Teacher I -> SG 11 Step 1-3
            { 1, new[] { 3, 4 } },          // Teacher II -> SG 12 Step 1-2
            { 2, new[] { 5 } },             // Teacher III -> SG 13
            { 3, new[] { 8 } },             // Master Teacher I -> SG 18
            { 4, new[] { 9 } },             // Master Teacher II -> SG 19
            { 5, new[] { 6 } },             // Head Teacher I -> SG 14
            { 6, new[] { 7 } },             // Head Teacher II -> SG 15
            { 7, new[] { 9 } },             // Principal I -> SG 19
            { 8, new[] { 10 } },            // Principal II -> SG 20
            { 9, new[] { 11 } }             // Administrative Officer III -> SG 9
        };

        // Item category mapping based on item code patterns
        // Items are ordered: TCH1 (0-1999), TCH2 (2000-3199), TCH3 (3200-3999), MT1 (4000-4299), MT2 (4300-4499), 
        // HT1 (4500-4699), HT2 (4700-4799), PRIN1 (4800-4879), PRIN2 (4880-4929), AO3 (4930-5129)
        Dictionary<int, (int start, int count)> positionToItemRange = new()
        {
            { 0, (0, 2000) },       // Teacher I
            { 1, (2000, 1200) },    // Teacher II
            { 2, (3200, 800) },     // Teacher III
            { 3, (4000, 300) },     // Master Teacher I
            { 4, (4300, 200) },     // Master Teacher II
            { 5, (4500, 200) },     // Head Teacher I
            { 6, (4700, 100) },     // Head Teacher II
            { 7, (4800, 80) },      // Principal I
            { 8, (4880, 50) },      // Principal II
            { 9, (4930, 200) }      // Administrative Officer III
        };
        _ = new Dictionary<int, int>();

        for (int i = 0; i < persons.Count; i++)
        {
            Person person = persons[i];

            // Select position based on weighted distribution
            int randomWeight = Random.Next(totalWeight);
            int positionIndex = 0;
            int cumulativeWeight = 0;
            for (int j = 0; j < positionWeights.Length; j++)
            {
                cumulativeWeight += positionWeights[j];
                if (randomWeight < cumulativeWeight)
                {
                    positionIndex = j;
                    break;
                }
            }

            // Get salary grade for this position
            int[] salaryGradeOptions = positionToSalaryGrade[positionIndex];
            int salaryGradeIndex = salaryGradeOptions[Random.Next(salaryGradeOptions.Length)];

            // Get item for this position
            (int itemStart, int itemCount) = positionToItemRange[positionIndex];
            int itemIndex = itemStart + (i % itemCount);
            if (itemIndex >= items.Count)
            {
                itemIndex = items.Count - 1;
            }

            DateOnly birthDate = person.DateOfBirth;
            DateOnly appointmentDate = GetRandomAppointmentDate(birthDate);

            // Determine employment status based on years of service
            int yearsOfService = DateTime.UtcNow.Year - appointmentDate.Year;
            EmploymentStatus employmentStatus = yearsOfService >= 2 ? EmploymentStatus.Permanent : EmploymentStatus.Regular;
            AppointmentStatus appointmentStatus = yearsOfService >= 3 ? AppointmentStatus.Promotion : AppointmentStatus.Original;

            employments.Add(new Employment
            {
                DepEdId = $"DEPED-{appointmentDate.Year}-{i + 1:D6}",
                PSIPOPItemNumber = $"PSIPOP-{i + 1:D5}",
                TINId = $"{Random.Next(100, 999)}-{Random.Next(100, 999)}-{Random.Next(100, 999)}-{Random.Next(0, 999):D3}",
                GSISId = $"{Random.Next(1000000000, int.MaxValue)}",
                PhilHealthId = $"{Random.Next(10, 99)}-{Random.Next(100000000, 999999999)}-{Random.Next(0, 9)}",
                DateOfOriginalAppointment = appointmentDate,
                AppointmentStatus = appointmentStatus,
                EmploymentStatus = employmentStatus,
                Eligibility = eligibilities[Random.Next(eligibilities.Length)],
                IsActive = true,
                PersonId = person.Id,
                PositionId = positions[positionIndex].Id,
                SalaryGradeId = salaryGrades[salaryGradeIndex].Id,
                ItemId = items[itemIndex].Id,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            });
        }

        return employments;
    }

    private static List<EmploymentSchool> CreateEmploymentSchools(List<Employment> employments, List<School> schools)
    {
        List<EmploymentSchool> employmentSchools = [];

        foreach (Employment employment in employments)
        {
            // Randomly assign to a school
            int schoolIndex = Random.Next(schools.Count);
            DateOnly startDate = employment.DateOfOriginalAppointment ?? DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5));

            // Some employees might have worked at a previous school
            if (Random.Next(100) < 20) // 20% chance of having previous school
            {
                int previousSchoolIndex = (schoolIndex + 1) % schools.Count;
                DateOnly previousStartDate = startDate;
                DateOnly previousEndDate = startDate.AddYears(Random.Next(1, 4));

                if (previousEndDate < DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    employmentSchools.Add(new EmploymentSchool
                    {
                        EmploymentId = employment.Id,
                        SchoolId = schools[previousSchoolIndex].Id,
                        StartDate = previousStartDate,
                        EndDate = previousEndDate,
                        IsCurrent = false,
                        IsActive = true,
                        CreatedBy = SystemUser,
                        CreatedOn = DateTime.UtcNow
                    });

                    startDate = previousEndDate;
                }
            }

            // Current school assignment
            employmentSchools.Add(new EmploymentSchool
            {
                EmploymentId = employment.Id,
                SchoolId = schools[schoolIndex].Id,
                StartDate = startDate,
                IsCurrent = true,
                IsActive = true,
                CreatedBy = SystemUser,
                CreatedOn = DateTime.UtcNow
            });
        }

        return employmentSchools;
    }
}
