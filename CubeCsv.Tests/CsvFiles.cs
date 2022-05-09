namespace CubeCsv.Tests
{
    public class CsvFiles
    {
        public static string GeneralCsv =
        $@"""{ HeaderNames.Department }"",""{ HeaderNames.FirstName }"",""{ HeaderNames.LastName }"",""{ HeaderNames.DateOrBirth }"",""{ HeaderNames.Age }"",""{ HeaderNames.Worth }""
                TG,Charith,Gunasekara,1980-07-20,42,47654
                TG,Kumanan,Panchalingam,07/04/1995,23,8765986
                BBS,Dilshan,Amarasinghe,07-05-2002,34,214748364700000
                IT,Paul,Wolf,07-07-2005,34,2349.876";

        public static string ParserCsv =
                $@"""{ HeaderNames.Department }"",""{ HeaderNames.FirstName }"",""{ HeaderNames.LastName }"",""{ HeaderNames.DateOrBirth }"",""{ HeaderNames.Age }"",""{ HeaderNames.Worth }""
                TG,Charith,Gunasekara,1980-07-20,42,47654
                TG,Kumanan,Panchalingam,07/04/1995,23,8765986
                BBS,Dilshan,Amarasinghe,07-05-2002,,214748364700000
                IT,Paul,Wolf,07-07-2005,34,2349.876";

        public static string InvalidFieldCsv =
                $@"""{ HeaderNames.Department }"",""{ HeaderNames.FirstName }"",""{ HeaderNames.LastName }"",""{ HeaderNames.Age }""
                TG,Charith,Gunasekara,42
                TG,Kumanan,Panchalingam,ttt
                BBS,Dilshan,Amarasinghe,tyu
                IT,Paul,Wolf,65";
        public static string EmptyRowsCsv =
                $@"""{ HeaderNames.FullName }"",""{ HeaderNames.CustomerCode }"",""{ HeaderNames.BirthDate }""
                Test,,
                Dilshan,,
                ,CON2429,
                ,,2014-03-13";
        public static string CsvFileRowReading =
                @"LDS2;TW Market OU Team A;T002222;P9WG;2010/01/05
                P9WG;TW Market OU Group A;T004444;;2010/01/05
                ABC1;TW Market CAM team 1;T123456;;2010/01/05
                ABC2;TW Market CAM team 2;T123457;;2010/01/05
                ABC3;TW Market CAM team 3;T123458;;2010/01/05
                ABC4;TW Market CAM team 4;T123459;;2010/01/05
                OPS1;TW Market OPS team 1;T123470;;2010/01/05
                OPS2;TW Market OPS team 2;T123471;;2010/01/05
                OPS3;TW Market OPS team 3;T123472;;2010/01/05";
    }

}
