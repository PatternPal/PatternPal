namespace PatternPal.Helpers;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Google.Protobuf.WellKnownTypes;
using PatternPal.LoggingServer.Models;
using Enum = System.Enum;

public class CSVManager
{
    private CsvReader reader;
    private CsvWriter writer;

    public CSVManager(string filepath)
    {
        using StreamReader reader = new StreamReader(filepath);
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);


        this.reader = csv;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };
        //  TODO: Check if file exists, if not, create it and write header
        StreamWriter writer = new StreamWriter(filepath, true);
        CsvWriter csvWriter = new CsvWriter(writer, config);
        this.writer = csvWriter;

    }
    public List<ProgSnap2Event> GetEvents(Dictionary<string, string > indexes)
    {
        List<ProgSnap2Event> events = new List<ProgSnap2Event>();
        while (reader.Read())
        {
            ProgSnap2Event e;
            foreach (KeyValuePair<string, string> index in indexes)
            {
                if (reader.GetField(index.Key) == index.Value)
                {
                    e = reader.GetRecord<ProgSnap2Event>();
                    
                    if (e != null)
                    {
                        events.Add(e);
                    }
                }
            }   
        }
        return events;
    }

    public void WriteEvent(ProgSnap2Event e)
    {
        this.writer.WriteRecord(e);
        this.writer.NextRecord();
    }

    public void WriteEvents(List<ProgSnap2Event> events)
    {
        foreach (ProgSnap2Event e in events)
        {
            this.writer.WriteRecord(e);
            this.writer.NextRecord();
        }
    }




}
