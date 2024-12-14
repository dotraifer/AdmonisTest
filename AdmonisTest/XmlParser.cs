using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AdmonisTest.Admonis;
using Serilog;

namespace AdmonisTest
{
    /// <summary>
    /// Responsible for parsing product XML data into Admonis objects.
    /// </summary>
    public class XmlParser
    {
        private readonly Dictionary<string, XElement> _products;
        private readonly XNamespace _xmlNamespace;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParser" /> class.
        /// </summary>
        /// <param name="xmlDocument">The XML document containing product data.</param>
        public XmlParser(XDocument xmlDocument)
        {
            // Get the root namespace of the XML document
            _xmlNamespace = xmlDocument.Root?.Name.Namespace;
            // Create a dictionary mapping product IDs to their respective XML elements for O(1) access
            _products = xmlDocument.Descendants(GetElementPath("product")).ToDictionary(
                product => product.Attribute("product-id")?.Value, product => product);
        }

        /// <summary>
        /// Retrieves a list of Admonis products parsed from the XML.
        /// </summary>
        /// <returns>A list of <see cref="AdmonisProduct" /> objects.</returns>
        public List<AdmonisProduct> GetProducts()
        {
            // filter variants products
            var products = _products
                .Where(productKvp => productKvp.Value.Element(
                    GetElementPath("variations")) != null).ToList();
            Log.Information("Found {TotalProducts} total Admonis products", products.Count);
            return products.Select(productKvp => CreateAdmonisProduct(productKvp.Value)).ToList();
        }

        /// <summary>
        /// Creates an Admonis product from the given XML element.
        /// </summary>
        /// <param name="product">The XML element representing a product.</param>
        /// <returns>An <see cref="AdmonisProduct" /> object.</returns>
        private AdmonisProduct CreateAdmonisProduct(XElement product)
        {
            // Extract custom attributes for additional option details
            var customAttributes = product.Descendants(GetElementPath("custom-attribute"));
            var admonisProduct = new AdmonisProduct
            {
                CustomerID = int.Parse(product.Element(GetElementPath("customer-id"))?.Value ?? 0.ToString()),
                Name = product.Element(GetElementPath("display-name"))?.Value,
                UPC = product.Element(GetElementPath("upc"))?.Value,
                StatusID = product.Element(GetElementPath("status-id"))?.Value,
                StatusComments = product.Element(GetElementPath("status-comments"))?.Value,
                Package = product.Element(GetElementPath("package"))?.Value,
                Description = product.Element(GetElementPath("short-description"))?.Value,
                DescriptionLong = product.Element(GetElementPath("long-description"))?.Value,
                WarrantyPeriod = product.Element(GetElementPath("warranty-period"))?.Value,
                WarrantyBy = product.Element(GetElementPath("warranty-by"))?.Value,
                Makat = product.Element(GetElementPath("makat"))?.Value,
                Model = product.Element(GetElementPath("model"))?.Value,
                Price_Cost_Customer =
                    decimal.Parse(product.Element(GetElementPath("price-cost-customer"))?.Value ?? 0.ToString()),
                Price_Cost = decimal.Parse(product.Element(GetElementPath("price-cost"))?.Value ?? 0.ToString()),
                Price_Market = decimal.Parse(product.Element(GetElementPath("price-market"))?.Value ?? 0.ToString()),
                Price_Publish = decimal.Parse(product.Element(GetElementPath("price-publish"))?.Value ?? 0.ToString()),
                Brand = product.Element(GetElementPath("brand"))?.Value,
                Volume = product.Element(GetElementPath("volume"))?.Value,
                ClassificationID = product.Element(GetElementPath("classification-id"))?.Value,
                SubClass = product.Element(GetElementPath("subclass"))?.Value,
                PlatformCategoryID = product.Element(GetElementPath("platform-category-id"))?.Value,
                VolumeType = product.Element(GetElementPath("volume-type"))?.Value,
                VideoLink =
                    customAttributes
                        .FirstOrDefault(customAttribute => customAttribute.Attribute("attribute-id")
                                                               ?.Value ==
                                                           "f54ProductVideo")
                        ?.Value,
                StorageLocation = product.Element(GetElementPath("storage-location"))?.Value
            };
            admonisProduct.Options = GetAdmonisProductOptions(product, admonisProduct);
            return admonisProduct;
        }

        /// <summary>
        /// Retrieves and creates a list of product options for the given product.
        /// </summary>
        /// <param name="product">The main product XML element.</param>
        /// <param name="admonisProduct">The parent Admonis product object.</param>
        /// <returns>A list of <see cref="AdmonisProductOption" /> objects.</returns>
        private List<AdmonisProductOption> GetAdmonisProductOptions(XElement product, AdmonisProduct admonisProduct)
        {
            // Extract variant IDs from the XML
            var productOptionsIds = product.Descendants(GetElementPath("variant"))
                .Select(variant => variant.Attribute("product-id")?.Value)
                .Where(id => !string.IsNullOrEmpty(id));

            var productOptions = productOptionsIds
                .Select(productOptionId => _products[productOptionId]);

            return productOptions.Select(productOption =>
                CreateAdmonisProductOption(productOption, admonisProduct)).ToList();
        }

        /// <summary>
        /// Creates an Admonis product option from the given XML element.
        /// </summary>
        /// <param name="productOption">The XML element representing a product option.</param>
        /// <param name="product">The parent Admonis product object.</param>
        /// <returns>An <see cref="AdmonisProductOption" /> object.</returns>
        private AdmonisProductOption CreateAdmonisProductOption(XElement productOption, AdmonisProduct product)
        {
            // Extract custom attributes for additional option details
            var customAttributes = productOption.Descendants(GetElementPath("custom-attribute")).ToList();
            return new AdmonisProductOption
            {
                optionSugName1 = productOption.Element(GetElementPath("option-sug-name-1"))?.Value ?? "צבע",
                optionSugName1Title =
                    productOption.Element(GetElementPath("option-sug-name-1-title"))?.Value ?? "בחר צבע",
                ProductMakat = product.Makat,
                optionMakat = productOption.Element(GetElementPath("option-makat"))?.Value,
                optionSugName2 = customAttributes
                    .FirstOrDefault(customAttribute =>
                        (string)customAttribute.Attribute("attribute-id") == "f54ProductColor")
                    ?.Value,
                optionSugName2Title = productOption.Element(GetElementPath("option-sug-name-2-title"))?.Value ??
                                      "בחר מידה",
                optionName = customAttributes
                    .FirstOrDefault(customAttribute =>
                        (string)customAttribute.Attribute("attribute-id") == "f54ProductSize")
                    ?.Value,
                optionModel = productOption.Element(GetElementPath("model"))?.Value,
                optionPrice_Cost_Customer =
                    decimal.Parse(productOption.Element(GetElementPath("price-cost-customer"))?.Value ?? 0.ToString()),
                optionPrice_Cost =
                    decimal.Parse(productOption.Element(GetElementPath("price-cost"))?.Value ?? 0.ToString()),
                optionPrice_Publish =
                    decimal.Parse(productOption.Element(GetElementPath("price-publish"))?.Value ?? 0.ToString()),
                optionPrice_Market =
                    decimal.Parse(productOption.Element(GetElementPath("price-market"))?.Value ?? 0.ToString()),
                optionstorageLocation = productOption.Element(GetElementPath("storage-location"))?.Value,
                optionupc = productOption.Element(GetElementPath("upc"))?.Value
            };
        }


        /// <summary>
        /// Constructs the fully qualified XML element path for a given element name.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>The fully qualified path as a string.</returns>
        private string GetElementPath(string elementName)
        {
            return (_xmlNamespace + elementName).ToString();
        }
    }
}