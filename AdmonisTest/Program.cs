using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace AdmonisTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string filePath = "Admonis/Product.xml";
            // xml file loading the XDocument object
            var xmlDocument = XDocument.Load(filePath);
            
            var xmlParser = new XmlParser(xmlDocument);
            var sw = new Stopwatch();
            sw.Start();
            var results = xmlParser.GetProducts();
            
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }
}
