using AdmonisTest.Admonis;
using System;
using System.Linq;
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

            var results = xmlParser.GetProducts();
        }
    }
}
