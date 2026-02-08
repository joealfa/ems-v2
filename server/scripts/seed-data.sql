-- ============================================================================
-- EMS-v2 Mock Data Seed Script
-- Equivalent to DataSeeder.cs — generates 5,000 persons with related data
-- Run directly on SQL Server against the EMS database
-- ============================================================================

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Safety check: only seed if ALL tables are empty
IF EXISTS (SELECT 1 FROM Schools) OR
   EXISTS (SELECT 1 FROM Positions) OR
   EXISTS (SELECT 1 FROM SalaryGrades) OR
   EXISTS (SELECT 1 FROM Items) OR
   EXISTS (SELECT 1 FROM Persons) OR
   EXISTS (SELECT 1 FROM Employments) OR
   EXISTS (SELECT 1 FROM EmploymentSchools)
BEGIN
    PRINT 'Database already contains data. Aborting seed.';
    RETURN;
END

PRINT 'Starting EMS-v2 data seed...';
PRINT 'Start time: ' + CONVERT(VARCHAR, GETUTCDATE(), 121);

-- Wrap everything in a single transaction with TRY/CATCH
-- If any error occurs, ALL changes are rolled back
BEGIN TRANSACTION;
BEGIN TRY

-- ============================================================================
-- Configuration
-- ============================================================================
DECLARE @SystemUser NVARCHAR(256) = N'System';
DECLARE @Now DATETIME = GETUTCDATE();
DECLARE @PersonCount INT = 5000;
DECLARE @DisplayIdCounter BIGINT = 100000000000; -- Starting display ID

-- ============================================================================
-- Helper: Name & location lookup tables (in-memory)
-- ============================================================================

-- Male first names
CREATE TABLE #MaleFirstNames (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #MaleFirstNames (Name) VALUES
('Juan'),('Pedro'),('Jose'),('Carlos'),('Miguel'),('Antonio'),('Roberto'),('Ricardo'),('Eduardo'),('Fernando'),
('Rafael'),('Manuel'),('Francisco'),('Alejandro'),('Gabriel'),('Luis'),('Marco'),('Paolo'),('Andres'),('Ramon'),
('Ernesto'),('Alfredo'),('Vicente'),('Domingo'),('Reynaldo'),('Armando'),('Leonardo'),('Rolando'),('Benjamin'),('Romeo'),
('Danilo'),('Orlando'),('Virgilio'),('Mariano'),('Nestor'),('Rodolfo'),('Arturo'),('Renato'),('Felix'),('Sergio'),
('Jaime'),('Teodoro'),('Gregorio'),('Ismael'),('Leandro'),('Marcelo'),('Noel'),('Oscar'),('Patricio'),('Quintin'),
('Rodel'),('Salvador'),('Tomas'),('Ulysses'),('Victor'),('William'),('Xavier'),('Yosef'),('Zeno'),('Adrian'),
('Bryan'),('Christian'),('Dennis'),('Elmer'),('Francis'),('Gerald'),('Harold'),('Ivan'),('Jerome'),('Kenneth'),
('Larry'),('Michael'),('Nathan'),('Oliver'),('Patrick'),('Quincy'),('Ronald'),('Stephen'),('Timothy'),('Vincent');

-- Female first names
CREATE TABLE #FemaleFirstNames (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #FemaleFirstNames (Name) VALUES
('Maria'),('Ana'),('Rosa'),('Elena'),('Carmen'),('Lucia'),('Teresa'),('Josefina'),('Guadalupe'),('Patricia'),
('Gloria'),('Esperanza'),('Dolores'),('Beatriz'),('Cristina'),('Diana'),('Estela'),('Francisca'),('Gabriela'),('Helena'),
('Isabel'),('Julia'),('Katherine'),('Leonora'),('Margarita'),('Norma'),('Olivia'),('Pilar'),('Regina'),('Sofia'),
('Valentina'),('Yolanda'),('Zenaida'),('Adelaida'),('Barbara'),('Cecilia'),('Daniela'),('Evelyn'),('Felicia'),('Geraldine'),
('Herminia'),('Imelda'),('Jasmine'),('Karen'),('Lourdes'),('Maribel'),('Natividad'),('Ophelia'),('Paulina'),('Queenie'),
('Rosalinda'),('Stephanie'),('Theresa'),('Ursula'),('Veronica'),('Wendy'),('Ximena'),('Yvonne'),('Zoe'),('Angela'),
('Bernadette'),('Charlene'),('Desiree'),('Emily'),('Florence'),('Grace'),('Hannah'),('Irene'),('Jennifer'),('Katrina'),
('Liza'),('Michelle'),('Nicole'),('Paula'),('Rachel'),('Sarah'),('Trisha'),('Uma'),('Vivian'),('Wanda');

-- Last names
CREATE TABLE #LastNames (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #LastNames (Name) VALUES
('Santos'),('Reyes'),('Cruz'),('Bautista'),('Garcia'),('Mendoza'),('Torres'),('Gonzales'),('Fernandez'),('Lopez'),
('Martinez'),('Rodriguez'),('Hernandez'),('Ramos'),('Aquino'),('Villanueva'),('Castro'),('Pascual'),('Dela Cruz'),('Flores'),
('Rivera'),('Morales'),('Perez'),('Sanchez'),('Jimenez'),('Romero'),('Diaz'),('Alvarez'),('Vargas'),('Castillo'),
('Gutierrez'),('Ortega'),('Salazar'),('Mercado'),('Soriano'),('Tan'),('Lim'),('Chua'),('Uy'),('Go'),
('Sy'),('Co'),('Ang'),('Ong'),('Yap'),('Chan'),('Lee'),('Wong'),('Ng'),('Tiu'),
('Aguilar'),('Domingo'),('Francisco'),('Ignacio'),('Laurel'),('Magsaysay'),('Osmena'),('Quezon'),('Roxas'),('Bonifacio'),
('Mabini'),('Luna'),('Del Pilar'),('Jacinto'),('Silang'),('Tupas'),('Legaspi'),('Urdaneta'),('Salcedo'),('Lacson'),
('Escudero'),('Cayetano'),('Ejercito'),('Marcos'),('Arroyo'),('Estrada'),('Duterte'),('Pacquiao'),('Sotto'),('Villar'),
('Pangilinan'),('Binay'),('Poe'),('Robredo'),('Moreno'),('Isko'),('Lacson'),('Defensor'),('Abalos'),('Remulla'),
('Gatchalian'),('Tolentino'),('Recto'),('Angara'),('Drilon'),('Enrile'),('Honasan'),('Trillanes'),('Hontiveros'),('Villanueva');

-- Middle names
CREATE TABLE #MiddleNames (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #MiddleNames (Name) VALUES
('Cruz'),('Reyes'),('Santos'),('Garcia'),('Lopez'),('Aquino'),('Ramos'),('Castro'),('Martinez'),('Gonzales'),
('Fernandez'),('Rodriguez'),('Hernandez'),('Flores'),('Rivera'),('Morales'),('Perez'),('Sanchez'),('Jimenez'),('Romero');

-- Cities
CREATE TABLE #Cities (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #Cities (Name) VALUES
('Manila'),('Quezon City'),('Makati'),('Pasig'),('Taguig'),('Mandaluyong'),('San Juan'),('Marikina'),
('Paranaque'),('Las Pinas'),('Muntinlupa'),('Caloocan'),('Malabon'),('Navotas'),('Valenzuela'),('Pasay');

-- Barangays
CREATE TABLE #Barangays (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #Barangays (Name) VALUES
('Poblacion'),('San Antonio'),('San Jose'),('Santo Nino'),('Santa Cruz'),('Concepcion'),('Bagumbayan'),('Kapitolyo'),
('Ugong'),('Pinagkaisahan'),('Manggahan'),('Rosario'),('Rizal'),('Malanday'),('Parang'),('Tumana'),
('Industrial Valley'),('Santa Elena'),('Wack-Wack'),('Addition Hills'),('Hagdang Bato'),('Plainview'),('Barangka'),('Marikina Heights');

-- Street names
CREATE TABLE #StreetNames (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #StreetNames (Name) VALUES
('Rizal'),('Mabini'),('Bonifacio'),('Aguinaldo'),('Quezon'),('Roxas'),('Osmena'),('Laurel'),
('Magsaysay'),('Garcia'),('Macapagal'),('Ramos'),('Estrada'),('Arroyo'),('Aquino'),('Marcos'),
('Luna'),('Del Pilar'),('Jacinto'),('Silang'),('Katipunan'),('EDSA'),('Shaw'),('Ortigas');

-- Street types
CREATE TABLE #StreetTypes (Id INT IDENTITY(1,1), Name NVARCHAR(20));
INSERT INTO #StreetTypes (Name) VALUES ('St.'),('Ave.'),('Blvd.'),('Road'),('Drive'),('Lane'),('Highway'),('Circle');

-- Email domains
CREATE TABLE #EmailDomains (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #EmailDomains (Name) VALUES ('email.com'),('gmail.com'),('yahoo.com'),('outlook.com'),('hotmail.com'),('mail.com');

-- Mobile prefixes
CREATE TABLE #MobilePrefixes (Id INT IDENTITY(1,1), Prefix NVARCHAR(10));
INSERT INTO #MobilePrefixes (Prefix) VALUES ('0917'),('0918'),('0919'),('0920'),('0921'),('0927'),('0928'),('0929'),('0939'),('0949');

-- School types
CREATE TABLE #SchoolTypes (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO #SchoolTypes (Name) VALUES ('Elementary School'),('High School'),('National High School'),('Science High School'),('Integrated School'),('Central School');

-- ============================================================================
-- Enum Reference
-- Gender:            Male=1, Female=2
-- CivilStatus:       Single=1, Married=2, SoloParent=3, Widow=4, Separated=5, Other=99
-- AddressType:       Business=1, Home=2
-- ContactType:       Work=1, Personal=2
-- AppointmentStatus: Original=1, Promotion=2, Transfer=3, Reappointment=4
-- EmploymentStatus:  Regular=1, Permanent=2
-- Eligibility:       LET=1, PBET=2, CivilServiceProfessional=3, CivilServiceSubProfessional=4, Other=99
-- ============================================================================

-- Counts for random selection
DECLARE @MaleCount INT = (SELECT COUNT(*) FROM #MaleFirstNames);
DECLARE @FemaleCount INT = (SELECT COUNT(*) FROM #FemaleFirstNames);
DECLARE @LastNameCount INT = (SELECT COUNT(*) FROM #LastNames);
DECLARE @MiddleNameCount INT = (SELECT COUNT(*) FROM #MiddleNames);
DECLARE @CityCount INT = (SELECT COUNT(*) FROM #Cities);
DECLARE @BarangayCount INT = (SELECT COUNT(*) FROM #Barangays);
DECLARE @StreetNameCount INT = (SELECT COUNT(*) FROM #StreetNames);
DECLARE @StreetTypeCount INT = (SELECT COUNT(*) FROM #StreetTypes);
DECLARE @EmailDomainCount INT = (SELECT COUNT(*) FROM #EmailDomains);
DECLARE @MobilePrefixCount INT = (SELECT COUNT(*) FROM #MobilePrefixes);
DECLARE @SchoolTypeCount INT = (SELECT COUNT(*) FROM #SchoolTypes);

-- CivilStatus values for random selection
CREATE TABLE #CivilStatuses (Id INT IDENTITY(1,1), Val INT);
INSERT INTO #CivilStatuses (Val) VALUES (1),(2),(3),(4),(5),(99);
DECLARE @CivilStatusCount INT = 6;

-- Eligibility values for random selection
CREATE TABLE #Eligibilities (Id INT IDENTITY(1,1), Val INT);
INSERT INTO #Eligibilities (Val) VALUES (1),(2),(3),(4),(99);
DECLARE @EligibilityCount INT = 5;

-- ============================================================================
-- 1. SCHOOLS (50 schools)
-- ============================================================================
PRINT 'Creating schools...';

CREATE TABLE #Schools (Idx INT, Id UNIQUEIDENTIFIER, DisplayId BIGINT);

DECLARE @si INT = 0;
WHILE @si < 50
BEGIN
    DECLARE @schoolId UNIQUEIDENTIFIER = NEWID();
    SET @DisplayIdCounter = @DisplayIdCounter + 1;
    DECLARE @schoolDisplayId BIGINT = @DisplayIdCounter;

    DECLARE @cityIdx INT = (@si % @CityCount) + 1;
    DECLARE @schoolTypeIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @SchoolTypeCount) + 1;
    DECLARE @schoolCity NVARCHAR(50);
    SELECT @schoolCity = Name FROM #Cities WHERE Id = @cityIdx;
    DECLARE @schoolType NVARCHAR(50);
    SELECT @schoolType = Name FROM #SchoolTypes WHERE Id = @schoolTypeIdx;
    DECLARE @schoolNum INT = (@si / @CityCount) + 1;
    DECLARE @schoolName NVARCHAR(200) = @schoolCity + N' ' + @schoolType + N' ' + CAST(@schoolNum AS NVARCHAR(5));

    INSERT INTO #Schools (Idx, Id, DisplayId) VALUES (@si, @schoolId, @schoolDisplayId);

    INSERT INTO Schools (Id, DisplayId, SchoolName, IsActive, CreatedBy, CreatedOn, IsDeleted)
    VALUES (@schoolId, @schoolDisplayId, @schoolName, 1, @SystemUser, @Now, 0);

    -- School address
    DECLARE @schoolAddrId UNIQUEIDENTIFIER = NEWID();
    SET @DisplayIdCounter = @DisplayIdCounter + 1;
    DECLARE @sStreetIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @StreetNameCount) + 1;
    DECLARE @sStreetTypeIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @StreetTypeCount) + 1;
    DECLARE @sBarangayIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @BarangayCount) + 1;

    -- Pre-resolve lookup values
    DECLARE @sStreetName NVARCHAR(50);
    SELECT @sStreetName = Name FROM #StreetNames WHERE Id = @sStreetIdx;
    DECLARE @sStreetType NVARCHAR(20);
    SELECT @sStreetType = Name FROM #StreetTypes WHERE Id = @sStreetTypeIdx;
    DECLARE @sBarangay NVARCHAR(50);
    SELECT @sBarangay = Name FROM #Barangays WHERE Id = @sBarangayIdx;
    DECLARE @sHouseNum NVARCHAR(10) = CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 999 + 1 AS NVARCHAR(10));
    DECLARE @sAddress2 NVARCHAR(50) = CASE WHEN ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 < 30 THEN N'Building ' + CHAR(65 + ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 5) ELSE NULL END;
    DECLARE @sZip NVARCHAR(10) = CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 1000 + 1000 AS NVARCHAR(10));

    INSERT INTO Addresses (Id, DisplayId, Address1, Address2, Barangay, City, Province, Country, ZipCode, IsCurrent, IsPermanent, IsActive, AddressType, PersonId, SchoolId, CreatedBy, CreatedOn, IsDeleted)
    VALUES (
        @schoolAddrId, @DisplayIdCounter,
        @sHouseNum + N' ' + @sStreetName + N' ' + @sStreetType,
        @sAddress2,
        @sBarangay,
        @schoolCity, N'Metro Manila', N'Philippines',
        @sZip,
        1, 0, 1, 1, -- IsCurrent=1, IsPermanent=0, IsActive=1, AddressType=Business(1)
        NULL, @schoolId,
        @SystemUser, @Now, 0
    );

    -- School contact
    DECLARE @schoolContactId UNIQUEIDENTIFIER = NEWID();
    SET @DisplayIdCounter = @DisplayIdCounter + 1;
    DECLARE @sMobileIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @MobilePrefixCount) + 1;
    DECLARE @sMobilePrefix NVARCHAR(10);
    SELECT @sMobilePrefix = Prefix FROM #MobilePrefixes WHERE Id = @sMobileIdx;
    DECLARE @schoolEmailName NVARCHAR(200) = LOWER(REPLACE(REPLACE(@schoolName, N' ', N'.'), N'-', N''));
    DECLARE @schoolMobile NVARCHAR(20) = @sMobilePrefix + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 9000000 + 1000000 AS NVARCHAR(10));
    DECLARE @schoolLandLine NVARCHAR(20) = N'02' + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 90000000 + 10000000 AS NVARCHAR(10));

    INSERT INTO Contacts (Id, DisplayId, Mobile, LandLine, Fax, Email, IsActive, ContactType, PersonId, SchoolId, CreatedBy, CreatedOn, IsDeleted)
    VALUES (
        @schoolContactId, @DisplayIdCounter,
        @schoolMobile,
        @schoolLandLine,
        NULL,
        @schoolEmailName + N'@deped.gov.ph',
        1, 1, -- IsActive=1, ContactType=Work(1)
        NULL, @schoolId,
        @SystemUser, @Now, 0
    );

    SET @si = @si + 1;
END

PRINT '  50 schools created.';

-- ============================================================================
-- 2. POSITIONS (10 positions)
-- ============================================================================
PRINT 'Creating positions...';

CREATE TABLE #Positions (Idx INT, Id UNIQUEIDENTIFIER, DisplayId BIGINT);

DECLARE @posData TABLE (Idx INT, TitleName NVARCHAR(100), Description NVARCHAR(500));
INSERT INTO @posData VALUES
(0, 'Teacher I', 'Entry-level teaching position for elementary and secondary schools'),
(1, 'Teacher II', 'Teaching position with 3+ years of experience'),
(2, 'Teacher III', 'Senior teaching position with 5+ years of experience'),
(3, 'Master Teacher I', 'Master teacher position for curriculum development'),
(4, 'Master Teacher II', 'Senior master teacher position'),
(5, 'Head Teacher I', 'Department head position'),
(6, 'Head Teacher II', 'Senior department head position'),
(7, 'Principal I', 'School principal for small schools'),
(8, 'Principal II', 'School principal for medium schools'),
(9, 'Administrative Officer III', 'Administrative support position');

DECLARE @pi INT = 0;
WHILE @pi < 10
BEGIN
    DECLARE @posId UNIQUEIDENTIFIER = NEWID();
    SET @DisplayIdCounter = @DisplayIdCounter + 1;

    INSERT INTO #Positions (Idx, Id, DisplayId) VALUES (@pi, @posId, @DisplayIdCounter);

    INSERT INTO Positions (Id, DisplayId, TitleName, [Description], IsActive, CreatedBy, CreatedOn, IsDeleted)
    SELECT @posId, @DisplayIdCounter, TitleName, [Description], 1, @SystemUser, @Now, 0
    FROM @posData WHERE Idx = @pi;

    SET @pi = @pi + 1;
END

PRINT '  10 positions created.';

-- ============================================================================
-- 3. SALARY GRADES (12 salary grades)
-- ============================================================================
PRINT 'Creating salary grades...';

CREATE TABLE #SalaryGrades (Idx INT, Id UNIQUEIDENTIFIER, DisplayId BIGINT);

DECLARE @sgData TABLE (Idx INT, SalaryGradeName NVARCHAR(50), Description NVARCHAR(500), Step INT, MonthlySalary DECIMAL(18,2));
INSERT INTO @sgData VALUES
(0,  'SG 11', 'Salary Grade 11 - Teacher I',                          1, 27000.00),
(1,  'SG 11', 'Salary Grade 11 - Teacher I',                          2, 27500.00),
(2,  'SG 11', 'Salary Grade 11 - Teacher I',                          3, 28000.00),
(3,  'SG 12', 'Salary Grade 12 - Teacher II',                         1, 29165.00),
(4,  'SG 12', 'Salary Grade 12 - Teacher II',                         2, 29700.00),
(5,  'SG 13', 'Salary Grade 13 - Teacher III',                        1, 31320.00),
(6,  'SG 14', 'Salary Grade 14 - Head Teacher I',                     1, 33843.00),
(7,  'SG 15', 'Salary Grade 15 - Head Teacher II',                    1, 36619.00),
(8,  'SG 18', 'Salary Grade 18 - Master Teacher I',                   1, 46725.00),
(9,  'SG 19', 'Salary Grade 19 - Master Teacher II / Principal I',    1, 51357.00),
(10, 'SG 20', 'Salary Grade 20 - Principal II',                       1, 57347.00),
(11, 'SG 9',  'Salary Grade 9 - Administrative Officer III',          1, 23877.00);

DECLARE @sgi INT = 0;
WHILE @sgi < 12
BEGIN
    DECLARE @sgId UNIQUEIDENTIFIER = NEWID();
    SET @DisplayIdCounter = @DisplayIdCounter + 1;

    INSERT INTO #SalaryGrades (Idx, Id, DisplayId) VALUES (@sgi, @sgId, @DisplayIdCounter);

    INSERT INTO SalaryGrades (Id, DisplayId, SalaryGradeName, [Description], Step, MonthlySalary, IsActive, CreatedBy, CreatedOn, IsDeleted)
    SELECT @sgId, @DisplayIdCounter, SalaryGradeName, [Description], Step, MonthlySalary, 1, @SystemUser, @Now, 0
    FROM @sgData WHERE Idx = @sgi;

    SET @sgi = @sgi + 1;
END

PRINT '  12 salary grades created.';

-- ============================================================================
-- 4. ITEMS (50 items)
-- ============================================================================
PRINT 'Creating items...';

CREATE TABLE #Items (Idx INT IDENTITY(0,1), Id UNIQUEIDENTIFIER, DisplayId BIGINT, Code NVARCHAR(10));

DECLARE @itemDefs TABLE (Code NVARCHAR(10), Description NVARCHAR(100), ItemCount INT);
INSERT INTO @itemDefs VALUES
('TCH1',  'Teaching Item - Teacher I',        15),
('TCH2',  'Teaching Item - Teacher II',        10),
('TCH3',  'Teaching Item - Teacher III',        6),
('MT1',   'Master Teacher Item I',              4),
('MT2',   'Master Teacher Item II',             3),
('HT1',   'Head Teacher Item I',                3),
('HT2',   'Head Teacher Item II',               3),
('PRIN1', 'Principal Item I',                    2),
('PRIN2', 'Principal Item II',                   2),
('AO3',   'Administrative Officer Item',         2);

DECLARE @itemCode NVARCHAR(10), @itemDesc NVARCHAR(100), @itemCnt INT, @itemI INT;

DECLARE itemCursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT Code, Description, ItemCount FROM @itemDefs ORDER BY
        CASE Code
            WHEN 'TCH1' THEN 1 WHEN 'TCH2' THEN 2 WHEN 'TCH3' THEN 3
            WHEN 'MT1' THEN 4 WHEN 'MT2' THEN 5 WHEN 'HT1' THEN 6
            WHEN 'HT2' THEN 7 WHEN 'PRIN1' THEN 8 WHEN 'PRIN2' THEN 9
            WHEN 'AO3' THEN 10
        END;

OPEN itemCursor;
FETCH NEXT FROM itemCursor INTO @itemCode, @itemDesc, @itemCnt;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @itemI = 1;
    WHILE @itemI <= @itemCnt
    BEGIN
        DECLARE @itmId UNIQUEIDENTIFIER = NEWID();
        SET @DisplayIdCounter = @DisplayIdCounter + 1;

        INSERT INTO #Items (Id, DisplayId, Code)
        VALUES (@itmId, @DisplayIdCounter, @itemCode);

        INSERT INTO Items (Id, DisplayId, ItemName, [Description], IsActive, CreatedBy, CreatedOn, IsDeleted)
        VALUES (
            @itmId, @DisplayIdCounter,
            N'OSEC-DECSB-' + @itemCode + N'-' + RIGHT('0000' + CAST(@itemI AS NVARCHAR(10)), 4),
            @itemDesc + N' #' + CAST(@itemI AS NVARCHAR(10)),
            1, @SystemUser, @Now, 0
        );

        SET @itemI = @itemI + 1;
    END

    FETCH NEXT FROM itemCursor INTO @itemCode, @itemDesc, @itemCnt;
END

CLOSE itemCursor;
DEALLOCATE itemCursor;

DECLARE @TotalItems INT = (SELECT COUNT(*) FROM #Items);
PRINT '  ' + CAST(@TotalItems AS VARCHAR(10)) + ' items created.';

-- ============================================================================
-- Item range mapping per position (for employment assignment)
-- ============================================================================
CREATE TABLE #ItemRanges (PosIdx INT, StartIdx INT, ItemCount INT);
INSERT INTO #ItemRanges VALUES
(0, 0, 15),   -- Teacher I
(1, 15, 10),  -- Teacher II
(2, 25, 6),   -- Teacher III
(3, 31, 4),   -- Master Teacher I
(4, 35, 3),   -- Master Teacher II
(5, 38, 3),   -- Head Teacher I
(6, 41, 3),   -- Head Teacher II
(7, 44, 2),   -- Principal I
(8, 46, 2),   -- Principal II
(9, 48, 2);   -- Administrative Officer III

-- Salary grade mapping per position
CREATE TABLE #PosSalaryGrades (PosIdx INT, SgIdx INT);
INSERT INTO #PosSalaryGrades VALUES
(0,0),(0,1),(0,2),  -- Teacher I -> SG 11 Step 1-3
(1,3),(1,4),        -- Teacher II -> SG 12 Step 1-2
(2,5),              -- Teacher III -> SG 13
(3,8),              -- Master Teacher I -> SG 18
(4,9),              -- Master Teacher II -> SG 19
(5,6),              -- Head Teacher I -> SG 14
(6,7),              -- Head Teacher II -> SG 15
(7,9),              -- Principal I -> SG 19
(8,10),             -- Principal II -> SG 20
(9,11);             -- Administrative Officer III -> SG 9

-- Position weights for distribution
CREATE TABLE #PosWeights (PosIdx INT, Weight INT, CumulativeWeight INT);
INSERT INTO #PosWeights VALUES (0,40,40),(1,24,64),(2,16,80),(3,6,86),(4,4,90),(5,4,94),(6,2,96),(7,2,98),(8,1,99),(9,1,100);

-- ============================================================================
-- 5. PERSONS (5,000 with addresses and contacts)
-- ============================================================================
PRINT 'Creating persons (5,000)...';

CREATE TABLE #Persons (
    Idx INT,
    Id UNIQUEIDENTIFIER,
    DisplayId BIGINT,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    DateOfBirth DATE,
    Gender INT
);

DECLARE @pIdx INT = 0;

WHILE @pIdx < @PersonCount
BEGIN
    DECLARE @personId UNIQUEIDENTIFIER = NEWID();
    SET @DisplayIdCounter = @DisplayIdCounter + 1;
    DECLARE @personDisplayId BIGINT = @DisplayIdCounter;

    -- Random gender: 50/50
    DECLARE @isMale BIT = CASE WHEN ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 < 50 THEN 1 ELSE 0 END;
    DECLARE @gender INT = CASE WHEN @isMale = 1 THEN 1 ELSE 2 END;

    -- Random names (pre-compute indices to avoid NEWID() inside subqueries)
    DECLARE @firstName NVARCHAR(100);
    DECLARE @nameIdx INT;
    IF @isMale = 1
    BEGIN
        SET @nameIdx = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @MaleCount) + 1;
        SELECT @firstName = Name FROM #MaleFirstNames WHERE Id = @nameIdx;
    END
    ELSE
    BEGIN
        SET @nameIdx = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @FemaleCount) + 1;
        SELECT @firstName = Name FROM #FemaleFirstNames WHERE Id = @nameIdx;
    END

    DECLARE @lastNameIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @LastNameCount) + 1;
    DECLARE @lastName NVARCHAR(100);
    SELECT @lastName = Name FROM #LastNames WHERE Id = @lastNameIdx;

    DECLARE @middleNameIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @MiddleNameCount) + 1;
    DECLARE @middleName NVARCHAR(100);
    SELECT @middleName = Name FROM #MiddleNames WHERE Id = @middleNameIdx;

    -- Random civil status
    DECLARE @civilStatusIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @CivilStatusCount) + 1;
    DECLARE @civilStatus INT;
    SELECT @civilStatus = Val FROM #CivilStatuses WHERE Id = @civilStatusIdx;

    -- Random birth date (age 22-65)
    DECLARE @minBirthYear INT = YEAR(@Now) - 65;
    DECLARE @maxBirthYear INT = YEAR(@Now) - 22;
    DECLARE @birthYear INT = @minBirthYear + ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % (@maxBirthYear - @minBirthYear + 1);
    DECLARE @birthMonth INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 12) + 1;
    DECLARE @maxDay INT = DAY(EOMONTH(DATEFROMPARTS(@birthYear, @birthMonth, 1)));
    DECLARE @birthDay INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @maxDay) + 1;
    DECLARE @dob DATE = DATEFROMPARTS(@birthYear, @birthMonth, @birthDay);

    INSERT INTO #Persons (Idx, Id, DisplayId, FirstName, LastName, DateOfBirth, Gender)
    VALUES (@pIdx, @personId, @personDisplayId, @firstName, @lastName, @dob, @gender);

    INSERT INTO Persons (Id, DisplayId, FirstName, LastName, MiddleName, DateOfBirth, Gender, CivilStatus, ProfileImageUrl, HasProfileImage, CreatedBy, CreatedOn, IsDeleted)
    VALUES (@personId, @personDisplayId, @firstName, @lastName, @middleName, @dob, @gender, @civilStatus, NULL, 0, @SystemUser, @Now, 0);

    -- Addresses (1-4 per person)
    DECLARE @addrCount INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 4) + 1;
    DECLARE @ai INT = 0;
    WHILE @ai < @addrCount
    BEGIN
        SET @DisplayIdCounter = @DisplayIdCounter + 1;
        DECLARE @addrCityIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @CityCount) + 1;
        DECLARE @addrStreetIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @StreetNameCount) + 1;
        DECLARE @addrStreetTypeIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @StreetTypeCount) + 1;
        DECLARE @addrBarangayIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @BarangayCount) + 1;

        -- Pre-resolve lookup values
        DECLARE @addrStreetName NVARCHAR(50);
        SELECT @addrStreetName = Name FROM #StreetNames WHERE Id = @addrStreetIdx;
        DECLARE @addrStreetType NVARCHAR(20);
        SELECT @addrStreetType = Name FROM #StreetTypes WHERE Id = @addrStreetTypeIdx;
        DECLARE @addrBarangay NVARCHAR(50);
        SELECT @addrBarangay = Name FROM #Barangays WHERE Id = @addrBarangayIdx;
        DECLARE @addrCity NVARCHAR(50);
        SELECT @addrCity = Name FROM #Cities WHERE Id = @addrCityIdx;

        DECLARE @addrType INT = CASE WHEN @ai = 0 THEN 2 ELSE (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 2) + 1 END; -- First=Home(2), rest=random
        DECLARE @addrIsCurrent BIT = CASE WHEN @ai = 0 THEN 1 ELSE 0 END;
        DECLARE @addrIsPermanent BIT = CASE WHEN @ai = 0 THEN 1 WHEN ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 < 50 THEN 1 ELSE 0 END;
        DECLARE @addrHouseNum NVARCHAR(10) = CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 9999 + 1 AS NVARCHAR(10));
        DECLARE @addrAddress2 NVARCHAR(50) = CASE WHEN ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 < 30 THEN N'Unit ' + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 + 1 AS NVARCHAR(5)) ELSE NULL END;
        DECLARE @addrZip NVARCHAR(10) = CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 1000 + 1000 AS NVARCHAR(10));

        INSERT INTO Addresses (Id, DisplayId, Address1, Address2, Barangay, City, Province, Country, ZipCode, IsCurrent, IsPermanent, IsActive, AddressType, PersonId, SchoolId, CreatedBy, CreatedOn, IsDeleted)
        VALUES (
            NEWID(), @DisplayIdCounter,
            @addrHouseNum + N' ' + @addrStreetName + N' ' + @addrStreetType,
            @addrAddress2,
            @addrBarangay,
            @addrCity,
            N'Metro Manila', N'Philippines',
            @addrZip,
            @addrIsCurrent, @addrIsPermanent, 1, @addrType,
            @personId, NULL,
            @SystemUser, @Now, 0
        );

        SET @ai = @ai + 1;
    END

    -- Contacts (1-4 per person)
    DECLARE @contCount INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 4) + 1;
    DECLARE @ci INT = 0;
    DECLARE @normalizedFirst NVARCHAR(100) = LOWER(REPLACE(@firstName, N' ', N''));
    DECLARE @normalizedLast NVARCHAR(100) = LOWER(REPLACE(@lastName, N' ', N''));

    WHILE @ci < @contCount
    BEGIN
        SET @DisplayIdCounter = @DisplayIdCounter + 1;
        DECLARE @contType INT = CASE WHEN @ci = 0 THEN 2 ELSE (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 2) + 1 END; -- First=Personal(2), rest=random
        DECLARE @mobileIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @MobilePrefixCount) + 1;
        DECLARE @emailDomIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @EmailDomainCount) + 1;
        DECLARE @emailDomain NVARCHAR(50);
        SELECT @emailDomain = Name FROM #EmailDomains WHERE Id = @emailDomIdx;
        DECLARE @emailVariant INT = ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 4;
        DECLARE @email NVARCHAR(256);

        SET @email = CASE @emailVariant
            WHEN 0 THEN @normalizedFirst + N'.' + @normalizedLast + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 AS NVARCHAR(5)) + N'@' + @emailDomain
            WHEN 1 THEN @normalizedFirst + LEFT(@normalizedLast, 1) + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 1000 AS NVARCHAR(5)) + N'@' + @emailDomain
            WHEN 2 THEN LEFT(@normalizedFirst, 1) + @normalizedLast + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 AS NVARCHAR(5)) + N'@' + @emailDomain
            ELSE @normalizedFirst + N'_' + @normalizedLast + N'@' + @emailDomain
        END;

        -- Pre-resolve mobile prefix
        DECLARE @mobilePrefix NVARCHAR(10);
        SELECT @mobilePrefix = Prefix FROM #MobilePrefixes WHERE Id = @mobileIdx;
        DECLARE @mobileNum NVARCHAR(20) = @mobilePrefix + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 9000000 + 1000000 AS NVARCHAR(10));
        DECLARE @landLine NVARCHAR(20) = CASE WHEN ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 < 40 THEN N'02' + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 90000000 + 10000000 AS NVARCHAR(10)) ELSE NULL END;

        INSERT INTO Contacts (Id, DisplayId, Mobile, LandLine, Fax, Email, IsActive, ContactType, PersonId, SchoolId, CreatedBy, CreatedOn, IsDeleted)
        VALUES (
            NEWID(), @DisplayIdCounter,
            @mobileNum,
            @landLine,
            NULL,
            @email,
            1, @contType,
            @personId, NULL,
            @SystemUser, @Now, 0
        );

        SET @ci = @ci + 1;
    END

    SET @pIdx = @pIdx + 1;

    IF @pIdx % 1000 = 0
        PRINT '  ' + CAST(@pIdx AS VARCHAR(10)) + ' / ' + CAST(@PersonCount AS VARCHAR(10)) + ' persons...';
END

PRINT '  ' + CAST(@PersonCount AS VARCHAR(10)) + ' persons created.';

-- ============================================================================
-- 6. EMPLOYMENTS (1 per person = 5,000)
-- ============================================================================
PRINT 'Creating employments...';

CREATE TABLE #Employments (Idx INT, Id UNIQUEIDENTIFIER, PersonIdx INT, AppointmentDate DATE);

DECLARE @eIdx INT = 0;
WHILE @eIdx < @PersonCount
BEGIN
    DECLARE @empId UNIQUEIDENTIFIER = NEWID();
    SET @DisplayIdCounter = @DisplayIdCounter + 1;

    -- Get person data
    DECLARE @empPersonId UNIQUEIDENTIFIER;
    DECLARE @empDob DATE;
    SELECT @empPersonId = Id, @empDob = DateOfBirth FROM #Persons WHERE Idx = @eIdx;

    -- Select position by weighted distribution
    DECLARE @randWeight INT = ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100;
    DECLARE @posIdx INT = (SELECT TOP 1 PosIdx FROM #PosWeights WHERE CumulativeWeight > @randWeight ORDER BY CumulativeWeight);

    -- Get salary grade for this position (random from available)
    DECLARE @sgOptions TABLE (SgIdx INT);
    DELETE FROM @sgOptions;
    INSERT INTO @sgOptions SELECT SgIdx FROM #PosSalaryGrades WHERE PosIdx = @posIdx;
    DECLARE @sgOptionCount INT = (SELECT COUNT(*) FROM @sgOptions);
    DECLARE @sgRandIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @sgOptionCount) + 1;
    DECLARE @selectedSgIdx INT;
    SELECT @selectedSgIdx = SgIdx FROM (
        SELECT SgIdx, ROW_NUMBER() OVER (ORDER BY SgIdx) AS RN FROM @sgOptions
    ) x WHERE RN = @sgRandIdx;

    -- Get item for this position
    DECLARE @itemStartIdx INT, @itemRangeCount INT;
    SELECT @itemStartIdx = StartIdx, @itemRangeCount = ItemCount FROM #ItemRanges WHERE PosIdx = @posIdx;
    DECLARE @selectedItemIdx INT = @itemStartIdx + (@eIdx % @itemRangeCount);
    IF @selectedItemIdx >= @TotalItems SET @selectedItemIdx = @TotalItems - 1;

    -- Get FK IDs
    DECLARE @empPositionId UNIQUEIDENTIFIER;
    SELECT @empPositionId = Id FROM #Positions WHERE Idx = @posIdx;
    DECLARE @empSalaryGradeId UNIQUEIDENTIFIER;
    SELECT @empSalaryGradeId = Id FROM #SalaryGrades WHERE Idx = @selectedSgIdx;
    DECLARE @empItemId UNIQUEIDENTIFIER;
    SELECT @empItemId = Id FROM #Items WHERE Idx = @selectedItemIdx;

    -- Appointment date (at least 22 years after birth, up to current year)
    DECLARE @minApptYear INT = YEAR(@empDob) + 22;
    DECLARE @maxApptYear INT = YEAR(@Now);
    IF @minApptYear > @maxApptYear SET @minApptYear = @maxApptYear;
    DECLARE @apptYear INT = @minApptYear + ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % (@maxApptYear - @minApptYear + 1);
    DECLARE @apptMonth INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 12) + 1;
    DECLARE @apptMaxDay INT = CASE WHEN DAY(EOMONTH(DATEFROMPARTS(@apptYear, @apptMonth, 1))) > 28 THEN 28 ELSE DAY(EOMONTH(DATEFROMPARTS(@apptYear, @apptMonth, 1))) END;
    DECLARE @apptDay INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @apptMaxDay) + 1;
    DECLARE @apptDate DATE = DATEFROMPARTS(@apptYear, @apptMonth, @apptDay);

    -- Employment/Appointment status based on years of service
    DECLARE @yearsOfService INT = YEAR(@Now) - @apptYear;
    DECLARE @empStatus INT = CASE WHEN @yearsOfService >= 2 THEN 2 ELSE 1 END;       -- Permanent(2) or Regular(1)
    DECLARE @apptStatus INT = CASE WHEN @yearsOfService >= 3 THEN 2 ELSE 1 END;      -- Promotion(2) or Original(1)

    -- Random eligibility
    DECLARE @eligibilityIdx INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @EligibilityCount) + 1;
    DECLARE @eligibility INT;
    SELECT @eligibility = Val FROM #Eligibilities WHERE Id = @eligibilityIdx;

    INSERT INTO #Employments (Idx, Id, PersonIdx, AppointmentDate) VALUES (@eIdx, @empId, @eIdx, @apptDate);

    INSERT INTO Employments (Id, DisplayId, DepEdId, PSIPOPItemNumber, TINId, GSISId, PhilHealthId,
        DateOfOriginalAppointment, AppointmentStatus, EmploymentStatus, Eligibility,
        IsActive, PersonId, PositionId, SalaryGradeId, ItemId, CreatedBy, CreatedOn, IsDeleted)
    VALUES (
        @empId, @DisplayIdCounter,
        N'DEPED-' + CAST(@apptYear AS NVARCHAR(5)) + N'-' + RIGHT('000000' + CAST(@eIdx + 1 AS NVARCHAR(10)), 6),
        N'PSIPOP-' + RIGHT('00000' + CAST(@eIdx + 1 AS NVARCHAR(10)), 5),
        CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 900 + 100 AS NVARCHAR(5)) + N'-' + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 900 + 100 AS NVARCHAR(5)) + N'-' + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 900 + 100 AS NVARCHAR(5)) + N'-' + RIGHT('000' + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 1000 AS NVARCHAR(5)), 3),
        CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 1000000000 + 1000000000 AS NVARCHAR(15)),
        CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 90 + 10 AS NVARCHAR(5)) + N'-' + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 900000000 + 100000000 AS NVARCHAR(15)) + N'-' + CAST(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 10 AS NVARCHAR(2)),
        @apptDate, @apptStatus, @empStatus, @eligibility,
        1, @empPersonId, @empPositionId, @empSalaryGradeId, @empItemId,
        @SystemUser, @Now, 0
    );

    SET @eIdx = @eIdx + 1;

    IF @eIdx % 1000 = 0
        PRINT '  ' + CAST(@eIdx AS VARCHAR(10)) + ' / ' + CAST(@PersonCount AS VARCHAR(10)) + ' employments...';
END

PRINT '  ' + CAST(@PersonCount AS VARCHAR(10)) + ' employments created.';

-- ============================================================================
-- 7. EMPLOYMENT-SCHOOL RELATIONSHIPS
-- ============================================================================
PRINT 'Creating employment-school assignments...';

DECLARE @schoolCount INT = (SELECT COUNT(*) FROM #Schools);
DECLARE @esCount INT = 0;

DECLARE @esIdx INT = 0;
WHILE @esIdx < @PersonCount
BEGIN
    DECLARE @esEmpId UNIQUEIDENTIFIER;
    DECLARE @esApptDate DATE;
    SELECT @esEmpId = Id, @esApptDate = AppointmentDate FROM #Employments WHERE Idx = @esIdx;

    DECLARE @currentSchoolIdx INT = ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % @schoolCount;
    DECLARE @currentSchoolId UNIQUEIDENTIFIER;
    SELECT @currentSchoolId = Id FROM #Schools WHERE Idx = @currentSchoolIdx;
    DECLARE @esStartDate DATE = @esApptDate;

    -- 20% chance of having a previous school
    IF ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 100 < 20
    BEGIN
        DECLARE @prevSchoolIdx INT = (@currentSchoolIdx + 1) % @schoolCount;
        DECLARE @prevSchoolId UNIQUEIDENTIFIER;
        SELECT @prevSchoolId = Id FROM #Schools WHERE Idx = @prevSchoolIdx;
        DECLARE @prevEndYearsLater INT = (ABS(CAST(CHECKSUM(NEWID()) AS BIGINT)) % 3) + 1;
        DECLARE @prevEndDate DATE = DATEADD(YEAR, @prevEndYearsLater, @esApptDate);

        IF @prevEndDate < @Now
        BEGIN
            SET @DisplayIdCounter = @DisplayIdCounter + 1;

            INSERT INTO EmploymentSchools (Id, DisplayId, EmploymentId, SchoolId, StartDate, EndDate, IsCurrent, IsActive, CreatedBy, CreatedOn, IsDeleted)
            VALUES (NEWID(), @DisplayIdCounter, @esEmpId, @prevSchoolId, @esApptDate, @prevEndDate, 0, 1, @SystemUser, @Now, 0);

            SET @esStartDate = @prevEndDate;
            SET @esCount = @esCount + 1;
        END
    END

    -- Current school assignment
    SET @DisplayIdCounter = @DisplayIdCounter + 1;

    INSERT INTO EmploymentSchools (Id, DisplayId, EmploymentId, SchoolId, StartDate, EndDate, IsCurrent, IsActive, CreatedBy, CreatedOn, IsDeleted)
    VALUES (NEWID(), @DisplayIdCounter, @esEmpId, @currentSchoolId, @esStartDate, NULL, 1, 1, @SystemUser, @Now, 0);

    SET @esCount = @esCount + 1;
    SET @esIdx = @esIdx + 1;
END

PRINT '  ' + CAST(@esCount AS VARCHAR(10)) + ' employment-school assignments created.';

-- ============================================================================
-- COMMIT — only reached if no errors occurred
-- ============================================================================
COMMIT TRANSACTION;

PRINT '';
PRINT '============================================';
PRINT 'Seed Complete! All data committed.';
PRINT '============================================';

-- Summary
DECLARE @cnt VARCHAR(10);

SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM Schools;
PRINT 'Schools:              ' + @cnt;
SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM Positions;
PRINT 'Positions:            ' + @cnt;
SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM SalaryGrades;
PRINT 'Salary Grades:        ' + @cnt;
SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM Items;
PRINT 'Items:                ' + @cnt;
SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM Persons;
PRINT 'Persons:              ' + @cnt;
SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM Addresses;
PRINT 'Addresses:            ' + @cnt;
SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM Contacts;
PRINT 'Contacts:             ' + @cnt;
SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM Employments;
PRINT 'Employments:          ' + @cnt;
SELECT @cnt = CAST(COUNT(*) AS VARCHAR(10)) FROM EmploymentSchools;
PRINT 'Employment-Schools:   ' + @cnt;

END TRY
BEGIN CATCH
    -- Roll back ALL changes if any error occurred
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT '';
    PRINT '============================================';
    PRINT 'ERROR — All changes have been rolled back!';
    PRINT '============================================';
    PRINT 'Error Number:  ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Error Line:    ' + CAST(ERROR_LINE() AS VARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT '';
    PRINT 'No data was inserted. Fix the error and re-run the script.';
END CATCH

-- ============================================================================
-- Cleanup temp tables (always runs, even after rollback)
-- ============================================================================
IF OBJECT_ID('tempdb..#MaleFirstNames') IS NOT NULL DROP TABLE #MaleFirstNames;
IF OBJECT_ID('tempdb..#FemaleFirstNames') IS NOT NULL DROP TABLE #FemaleFirstNames;
IF OBJECT_ID('tempdb..#LastNames') IS NOT NULL DROP TABLE #LastNames;
IF OBJECT_ID('tempdb..#MiddleNames') IS NOT NULL DROP TABLE #MiddleNames;
IF OBJECT_ID('tempdb..#Cities') IS NOT NULL DROP TABLE #Cities;
IF OBJECT_ID('tempdb..#Barangays') IS NOT NULL DROP TABLE #Barangays;
IF OBJECT_ID('tempdb..#StreetNames') IS NOT NULL DROP TABLE #StreetNames;
IF OBJECT_ID('tempdb..#StreetTypes') IS NOT NULL DROP TABLE #StreetTypes;
IF OBJECT_ID('tempdb..#EmailDomains') IS NOT NULL DROP TABLE #EmailDomains;
IF OBJECT_ID('tempdb..#MobilePrefixes') IS NOT NULL DROP TABLE #MobilePrefixes;
IF OBJECT_ID('tempdb..#SchoolTypes') IS NOT NULL DROP TABLE #SchoolTypes;
IF OBJECT_ID('tempdb..#CivilStatuses') IS NOT NULL DROP TABLE #CivilStatuses;
IF OBJECT_ID('tempdb..#Eligibilities') IS NOT NULL DROP TABLE #Eligibilities;
IF OBJECT_ID('tempdb..#Schools') IS NOT NULL DROP TABLE #Schools;
IF OBJECT_ID('tempdb..#Positions') IS NOT NULL DROP TABLE #Positions;
IF OBJECT_ID('tempdb..#SalaryGrades') IS NOT NULL DROP TABLE #SalaryGrades;
IF OBJECT_ID('tempdb..#Items') IS NOT NULL DROP TABLE #Items;
IF OBJECT_ID('tempdb..#ItemRanges') IS NOT NULL DROP TABLE #ItemRanges;
IF OBJECT_ID('tempdb..#PosSalaryGrades') IS NOT NULL DROP TABLE #PosSalaryGrades;
IF OBJECT_ID('tempdb..#PosWeights') IS NOT NULL DROP TABLE #PosWeights;
IF OBJECT_ID('tempdb..#Persons') IS NOT NULL DROP TABLE #Persons;
IF OBJECT_ID('tempdb..#Employments') IS NOT NULL DROP TABLE #Employments;

PRINT '';
PRINT 'End time: ' + CONVERT(VARCHAR, GETUTCDATE(), 121);
