// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using CsvHelper;

class Program
{
    public static List<ResultEntry> Results = new List<ResultEntry>();
    private const string pattern = @"^\s+(?<NO>\S+)\s+(?<ProgramNumber>\S+)\s+(?<Torque>\S+)\s+(?<Angle>\S+)\s+(?<D>\S+)\s+(?<PT>\S+)\s+(?<Time>\S+)\s+(?<Result>\S+)\s+(?<Cycle>\S+|\S+)$";

    private static void Main(string[] args)
    {

#if DEBUG
        // No Arguments have been Passed
        if (args.Length == 0)
        {
            ParseFile("./DummyData.txt");
        }
#else        
        if (args.Length == 0)
        {
            Console.WriteLine("Drag and Drop the File on the EXE");
            return;
        }
        ParseFile(args[0]);
#endif
    }

    private static void ParseFile(string file)
    {
        
        if (!File.Exists(file))
        {
            Console.WriteLine("The File does not exists");
            return;
        }
        
        
        var lines = File.ReadLines(file);
        foreach (var line in lines)
        {
            var matches = Regex.Matches(line, pattern);
            foreach (Match match in matches)
            {
                Results.Add(new ResultEntry()
                {
                    No=Convert.ToInt32(match.Groups[1].Value),
                    Program=Convert.ToInt32(match.Groups[2].Value),
                    Tq=Convert.ToSingle(match.Groups[3].Value),
                    Ang=Convert.ToSingle(match.Groups[4].Value),
                    D=Convert.ToSingle(match.Groups[5].Value),
                    PT=Convert.ToSingle(match.Groups[6].Value),
                    Time=Convert.ToSingle(match.Groups[7].Value),
                    Result=match.Groups[8].Value,
                    Cycle=Convert.ToInt32(match.Groups[9].Value),
                    
                });
            }
        }

        // Convert the File Into Json
        var JsonString = JsonSerializer.Serialize(Results);
        File.WriteAllText("Export.json",JsonString);
        
        // Convert to CSV
        using (var writer = new StreamWriter("Export.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Results);
        }

    }

    public struct ResultEntry
    {
        public int No { get; set; }
        public int Program { get; set; }
        public float Tq { get; set; }
        public float Ang { get; set; }
        public float D { get; set; }
        public float PT { get; set; }
        public float Time { get; set; }
        public string Result { get; set; }
        public int Cycle { get; set; }
    }
}