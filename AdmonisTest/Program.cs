using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace AdmonisTest
{
    internal class Program
    {
        private const string XmlFilePath = "Admonis/Product.xml";
        static void Main(string[] args)
        {
            // xml file loading the XDocument object
            var xmlDocument = XDocument.Load(XmlFilePath);
            
            var xmlParser = new XmlParser(xmlDocument);
            var results = xmlParser.GetProducts();
        }
    }
}
