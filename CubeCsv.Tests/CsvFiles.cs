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

        public static string InvalidCsv =
                $@"""{ HeaderNames.Department }"",""{ HeaderNames.FirstName }"",""{ HeaderNames.LastName }"",""{ HeaderNames.Age }""
                TG,Charith,Gunasekara,42
                TG,Kumanan,Panchalingam,ttt
                BBS,Dilshan,Amarasinghe,tyu
                IT,Paul,Wolf,65";
    }

}
