using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using Serilog;

namespace AdmonisTest
{
    internal class Program
    {
        private const string XmlFilePath = "Admonis/Product.xml";
        private const string OutputJsonPath = "Output/Products.json";

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            // xml file loading the XDocument object
            Log.Information("Loading XML file from path: {XmlFilePath}", XmlFilePath);
            var xmlDocument = XDocument.Load(XmlFilePath);
            Log.Information("Successfully loaded xml from {XmlFilePath}", XmlFilePath);

            var xmlParser = new XmlParser(xmlDocument);
            var results = xmlParser.GetProducts();
            Log.Information("Successfully parsed {ProductCount} Admonis products", results.Count);

            // Serialize the parsed products to JSON
            var json = JsonConvert.SerializeObject(results, Formatting.Indented);

            // Ensure the output directory exists
            var outputDirectory = Path.GetDirectoryName(OutputJsonPath);
            if (!Directory.Exists(outputDirectory))
            {
                Log.Information("Creating output directory: {OutputDirectory}", outputDirectory);
                if (outputDirectory != null) Directory.CreateDirectory(outputDirectory);
            }

            // Write the JSON to a file
            Log.Information("Writing products JSON to file: {OutputJsonFile}", OutputJsonPath);
            File.WriteAllText(OutputJsonPath, json);
            Log.Information("Products have been successfully written to {OutputJsonFile}", OutputJsonPath);
        }
    }
}