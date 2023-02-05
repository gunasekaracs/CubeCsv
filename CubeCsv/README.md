# CubeCsv

## Examples

## Reading header from a CSV file 

```
    string csvText =
        $@"""Department"",""FirstName"",""LastName"",""DateOrBirth"",""Age"",""Worth""
        TG,Neil,White,1980-07-20,42,47654
        TG,Richard,Johnes,07/04/1995,23,8765986
        BBS,Peter,Bell,07-05-2002,34,214748364700000
        IT,Paul,Wolf,07-07-2005,34,2349.876";
    var data = encoding.GetBytes(csvText);
    using var stream = new MemoryStream(data);
    using var streamReader = new StreamReader(stream);
    using var csvFile = 
        new CsvFile(streamReader, 
            new CsvConfiguration() { HasHeader = true, 
                CultureInfo = CultureInfo.InvariantCulture };
    CsvFieldHeader departmentHeader = csvFile.Header["Department"];
```

## Reading values from a CSV row at line 3

```
    string csvText =
        $@"""Department"",""FirstName"",""LastName"",""DateOrBirth"",""Age"",""Worth""
        TG,Neil,White,1980-07-20,42,47654
        TG,Richard,Johnes,07/04/1995,23,8765986
        BBS,Peter,Bell,07-05-2002,34,214748364700000
        IT,Paul,Wolf,07-07-2005,34,2349.876";
    var data = encoding.GetBytes(csvText);
    using var stream = new MemoryStream(data);
    using var streamReader = new StreamReader(stream);
    using var csvFile = 
        new CsvFile(streamReader, 
            new CsvConfiguration() { HasHeader = true,
                CultureInfo = CultureInfo.InvariantCulture };
    for (int i = 0; i < 3; i++)
        csvFile.ReadAsync().Wait();
    csvFile.GetValueAsString("Department") => "BBS";
    csvFile.GetValue<string>("Department") => "BBS";
    csvFile.GetValueAsString("FirstName") => "Peter";
    csvFile.GetValue<string>("FirstName") => "Peter";
    csvFile.GetValueAsString("LastName") =>"Bell";
    csvFile.GetValue<string>("LastName") => "Bell";
    csvFile.GetValue<DateTime>("DateOrBirth") => DateTime.Parse("07-05-2002";
    csvFile.GetValue<int>("Age") => 34;
    csvFile.GetValue<float>("Worth") => 214748364700000f;
    csvFile.GetValue<DateTime>("FirstName") => ThrowsException<CsvInvalidCastException>();
```

## Validating Schema

```
    string csvText =
        $@"""Department"",""FirstName"",""LastName"",""DateOrBirth"",""Age"",""Worth""
        TG,Neil,White,1980-07-20,42,47654
        TG,Richard,Johnes,07/04/1995,23,8765986
        BBS,Peter,Bell,07-05-2002,,214748364700000
        IT,Paul,Wolf,07-07-2005,34,2349.876";
    var data = encoding.GetBytes(csvText);
    using var stream = new MemoryStream(data);
    using var streamReader = new StreamReader(stream);
    using var csvFile = 
        new CsvFile(streamReader, 
            new CsvConfiguration() { HasHeader = true, 
                CultureInfo = CultureInfo.InvariantCulture };
    CsvFieldHeader departmentHeader = csvFile.Header["Department];
    departmentHeader.Schema.Name => "Department;
    departmentHeader.Ordinal => 0;
    departmentHeader.Schema.Type => typeof(string);

    CsvFieldHeader firstNameHeader = csvFile.Header["FirstName"];
    firstNameHeader.Schema.Name => "FirstName";
    firstNameHeader.Ordinal => 1;
    firstNameHeader.Schema.Type => typeof(string);

    CsvFieldHeader lastNameHeader = csvFile.Header["LastName"];
    lastNameHeader.Schema.Name => "LastName";
    lastNameHeader.Ordinal => 2;
    lastNameHeader.Schema.Length => 12;
    lastNameHeader.Schema.Type => typeof(string);

    CsvFieldHeader dateOrBirthHeader = csvFile.Header["DateOrBirth"];
    dateOrBirthHeader.Schema.Name => "DateOrBirth";
    dateOrBirthHeader.Ordinal => 3;
    dateOrBirthHeader.Schema.Type => typeof(DateTime);

    CsvFieldHeader ageHeader = csvFile.Header["Age"];
    ageHeader.Schema.Name => "Age";
    ageHeader.Ordinal => 4;
    ageHeader.Schema.Type => typeof(int);

    CsvFieldHeader worthHeader = csvFile.Header["Worth"];
    worthHeader.Schema.Name => "Worth";
    worthHeader.Ordinal => 5;
    worthHeader.Schema.Type => typeof(float);
```

## Reading from SQL Table

Table Name: Employees

| Id | FirstName | LastName | Age | DateOfBirth |
| --- | ----------- | ------------ | ---- | -------- |
| 1 | Neil| White| 42| 1980-07-20|
| 2 | Peter| Bell| 34| 1988-01-10|
| 3 | Richard|Johnes| 30 |1992-05-02|
| 4 | Nancy| Bella| 30| 1992-05-20|
| 5 | Bob| Way| 30 |1992-08-05|

```
    CsvFile csvFile = new(
        "Employees",
        new SqlConnection("<Connection String>"),
        string.Empty,
        string.Empty,
        new CsvConfiguration() { ColumnExlusions = new() { "Id" } });

    csvFile = csvFile.ReadRowsFromTableAsync().Result;

    csvFile.Header[0].Ordinal => 0;
    csvFile.Header[0].Schema.Name => "FirstName";
    csvFile.Header[0].Schema.Type => typeof(string);

    csvFile.Header[1].Ordinal => 1);
    csvFile.Header[1].Schema.Name => "LastName";
    csvFile.Header[1].Schema.Type => typeof(string);

    csvFile.Header[2].Ordinal => 2;
    csvFile.Header[2].Schema.Name => "Age";
    csvFile.Header[2].Schema.Type => typeof(long);

    csvFile.Header[3].Ordinal => 3;
    csvFile.Header[3].Schema.Name => "DateOfBirth";
    csvFile.Header[3].Schema.Type => typeof(string);

    int count = csvFile.CountAsync().Result;
    count => 5;

    while (csvFile.ReadAsync().Result)
    {
        if (csvFile.Location == 0)
        {
            csvFile.GetValue<string>("FirstName) => "Neil";
            csvFile.GetValue<string>("LastName) => "White";
            csvFile.GetValue<long>("Age) => 42;
            csvFile.GetValueAsString("DateOfBirth) => "1980-07-20";
        }
        if (csvFile.Location == 1)
        {
            csvFile.GetValue<string>("FirstName) => "Peter";
            csvFile.GetValue<string>("LastName) => "Bell";
            csvFile.GetValue<long>("Age) => 34;
            csvFile.GetValueAsString("DateOfBirth) => "1988-01-10";
        }
        if (csvFile.Location == 2)
        {
            csvFile.GetValueAsString("FirstName) => "Richard";
            csvFile.GetValueAsString("LastName) => "Johnes";
            csvFile.GetValue<long>("Age) => 30;
            csvFile.GetValueAsString("DateOfBirth) => "1992-05-02";
        }
        if (csvFile.Location == 3)
        {
            csvFile.GetValueAsString("FirstName) => "Nancy";
            csvFile.GetValueAsString("LastName) => "Bella";
            csvFile.GetValue<long>("Age) => 30;
            csvFile.GetValueAsString("DateOfBirth) => "1992-05-20";
        }
        if (csvFile.Location == 4)
        {
            csvFile.GetValueAsString("FirstName) => "Bob";
            csvFile.GetValueAsString("LastName) => "Way";
            csvFile.GetValue<long>("Age) => 30;
            csvFile.GetValueAsString("DateOfBirth) => "1992-08-05";
        }
    }
```

## Reading from SQL Table with where clause and order by clause

Table Name: Employees

| Id | FirstName | LastName | Age | DateOfBirth |
| --- | ----------- | ------------ | ---- | -------- |
| 1 | Neil| White| 42| 1980-07-20|
| 2 | Peter| Bell| 34| 1988-01-10|
| 3 | Richard|Johnes| 30 |1992-05-02|
| 4 | Nancy| Bella| 30| 1992-05-20|
| 5 | Bob| Way| 30 |1992-08-05|

```
    CsvFile csvFile = new(
        "Employees",
        new SqlConnection("<Connection String>"),
        "Age < 40",
        "FirstName",
        new CsvConfiguration() { ColumnExlusions = new() { "Id" } });

    csvFile = csvFile.ReadRowsFromTableAsync().Result;

    int count = csvFile.CountAsync().Result;
    count => 4;
    while (csvFile.ReadAsync().Result)
    {
        if (csvFile.Location == 0)
        {
            csvFile.GetValue<string>("FirstName) => "Bob";
            csvFile.GetValue<string>("LastName) => "Way";
            csvFile.GetValue<long>("Age) => 30;
            csvFile.GetValueAsString("DateOfBirth) => "1992-08-05";
        }
    }
```

## CSV Validation 

```
    string InvalidFieldCsv =
        $@"""Department"",""FirstName"",""LastName"",""Age""
        TG,Neil,White,42
        TG,Richard,Johnes,ttt
        BBS,Peter,Bell,tyu
        IT,Paul,Wolf,65";
    var result = csv.ValidateSchema(
        new CsvFieldSchema(HeaderNames.Department, typeof(string), 3),
        new CsvFieldSchema(HeaderNames.FirstName, typeof(string)),
        new CsvFieldSchema(HeaderNames.LastName, typeof(string), 6),
        new CsvFieldSchema(HeaderNames.Age, typeof(int)));
    result.HasErrors && result.Errors.Count > 0;
    result.Errors[0].Error => "Column at the index 3 is a type mismatch";
```