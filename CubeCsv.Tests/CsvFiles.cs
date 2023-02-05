namespace CubeCsv.Tests
{
    public class CsvFiles
    {
        public static readonly string GeneralCsv =
            $@"""{ HeaderNames.Department }"",""{ HeaderNames.FirstName }"",""{ HeaderNames.LastName }"",""{ HeaderNames.DateOrBirth }"",""{ HeaderNames.Age }"",""{ HeaderNames.Worth }""
            TG,Neil,White,1980-07-20,42,47654
            TG,Richard,Johnes,07/04/1995,23,8765986
            BBS,Peter,Bell,07-05-2002,34,214748364700000
            IT,Paul,Wolf,07-07-2005,34,2349.876";

        public static readonly string ParserCsv =
            $@"""{ HeaderNames.Department }"",""{ HeaderNames.FirstName }"",""{ HeaderNames.LastName }"",""{ HeaderNames.DateOrBirth }"",""{ HeaderNames.Age }"",""{ HeaderNames.Worth }""
            TG,Neil,White,1980-07-20,42,47654
            TG,Richard,Johnes,07/04/1995,23,8765986
            BBS,Peter,Bell,07-05-2002,,214748364700000
            IT,Paul,Wolf,07-07-2005,34,2349.876";

        public static readonly string InvalidFieldCsv =
            $@"""{ HeaderNames.Department }"",""{ HeaderNames.FirstName }"",""{ HeaderNames.LastName }"",""{ HeaderNames.Age }""
            TG,Neil,White,42
            TG,Richard,Johnes,ttt
            BBS,Peter,Bell,tyu
            IT,Paul,Wolf,65";

        public static readonly string EmptyRowsCsv =
            $@"""{ HeaderNames.FullName }"",""{ HeaderNames.CustomerCode }"",""{ HeaderNames.BirthDate }""
            Test,,
            Peter,,
            ,CON2429,
            ,,2014-03-13";

        public static readonly string CsvFileRowReading =
            @"LDS2;TW Market OU Team A;T002222;P9WG;2010/01/05
            P9WG;TW Market OU Group A;T004444;;2010/01/05
            ABC1;TW Market CAM team 1;T123456;;2010/01/05
            ABC2;TW Market CAM team 2;T123457;;2010/01/05
            ABC3;TW Market CAM team 3;T123458;;2010/01/05
            ABC4;TW Market CAM team 4;T123459;;2010/01/05
            OPS1;TW Market OPS team 1;T123470;;2010/01/05
            OPS2;TW Market OPS team 2;T123471;;2010/01/05
            OPS3;TW Market OPS team 3;T123472;;2010/01/05";

        public static readonly string IncorrectDataTypeOnLastColumnsLastRow =
            @"UBS-123
                UBS-234
                UBS-345
                UBS-456
                UBS-567
                UBS-678
                UBS-789
                UBS-890
                UBS-321
                UBS-432
                543";

        public static readonly string WithQualifyingQuotesAndCommaInTheCellValues =
            @"""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABC"",""D,EFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABC""DE""FG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABC""DEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCD,EFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABC""D,EFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",123456789";
        public static readonly string SomeDataLargerThanTheSchemaLimitsAllows =
            @"ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFGH¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFG¤ABCDEFGH¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFG¤ABCDEFG¤ABCDEFGH¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456
              ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤ABCDEFG¤123456";
        public static readonly string DataTypeIssues =
            @"ABCDEFG¤2009-06-15¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤12345
              ABCDEFG¤2009-06-16¤ABCDEFG¤12.352¤ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤12345
              ABCDEFG¤2009-06-17¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤12345
              ABCDEFG¤2009-06-18¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤12345
              ABCDEFG¤2009-06-31¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤12345
              ABCDEFG¤2009-06-15¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤12345
              ABCDEFG¤2009-06-15¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T24:45:30.0000000Z¤ABCDEFG¤12345
              ABCDEFG¤2009-06-15¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤12345
              ABCDEFG¤2009-06-15¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T13:45:30.0000000-07:00¤ABCDEFG¤12345
              ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤12.35¤ABCDEFG¤2009-06-15T13:45:30.0000000Z¤ABCDEFG¤1234A";
        public static readonly string MultiLineData =
            @"""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG
                HIJKLMN"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG
                HIJKLMN"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG
                HIJKLMN"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG
                HIJKLMN"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG
                HIJKLMN"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG
                HIJKLMN"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890";
        public static readonly string InvalidIntegerAtTheLastCell =
            @"""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890
              ""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",""ABCDEFG"",1234567890,""ABCDEFG""";
    }

}
