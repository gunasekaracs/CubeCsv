# CubeCsv

## Reading header from a CSV file 

```
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture };
            CsvFieldHeader departmentHeader = csvFile.Header["Department""];
```


## Reading values from a CSV row at line 3

```

            var data = encoding.GetBytes(CsvFiles.GeneralCsv);
            using var stream = new MemoryStream(data);
            using var streamReader = new StreamReader(stream);
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture };
            for (int i = 0; i < 3; i++)
                csvFile.ReadAsync().Wait();
            csvFile.GetValueAsString("Department") => "BBS";
            csvFile.GetValue<string>("Department") => "BBS";
            csvFile.GetValueAsString("FirstName") => "Dilshan";
            csvFile.GetValue<string>("FirstName") => "Dilshan";
            csvFile.GetValueAsString("LastName") =>"Amarasinghe";
            csvFile.GetValue<string>("LastName") => "Amarasinghe";
            csvFile.GetValue<DateTime>("DateOrBirth") => DateTime.Parse("07-05-2002";
            csvFile.GetValue<int>("Age") => 34;
            csvFile.GetValue<float>("Worth") => 214748364700000f;
            csvFile.GetValue<DateTime>("FirstName") => DateTime.Parse("07-05-2002") => ThrowsException<CsvInvalidCastException>();

```

## Validating Schema

```

            ASCIIEncoding encoding = new(;
            var data = encoding.GetBytes(CsvFiles.ParserCsv;
            using var stream = new MemoryStream(data;
            using var streamReader = new StreamReader(stream;
            using var csvFile = new CsvFile(streamReader, new CsvConfiguration() { HasHeader = true, CultureInfo = CultureInfo.InvariantCulture };
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