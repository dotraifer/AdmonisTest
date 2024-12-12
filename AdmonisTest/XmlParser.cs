using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AdmonisTest.Admonis;

namespace AdmonisTest
{
    /// <summary>
    /// Responsible for the parsing of the product xml into Admonis objects functionallity
    /// </summary>
    public class XmlParser
    {
        private readonly XDocument _xmlDocument;
        private readonly XNamespace _xmlNamespace;

        public XmlParser(XDocument xmlDocument)
        {
            _xmlDocument = xmlDocument;
            _xmlNamespace = xmlDocument.Root?.Name.Namespace;
        }

        /// <summary>
        /// Lists and creating the Admonis products list out of the xml 
        /// </summary>
        /// <returns>List of all the products</returns>
        public List<AdmonisProduct> GetProducts()
        {
            var products = _xmlDocument.Descendants(_xmlNamespace + "product")
                .Where(x => x.Element(_xmlNamespace + "variations") != null);

            return products.Select(CreateAdmonisProduct).ToList();
        }
        
        
        private AdmonisProduct CreateAdmonisProduct(XElement product)
        {
            var customAttributes = product.Descendants(_xmlNamespace + "custom-attribute");
            return new AdmonisProduct
            {
                CustomerID = string.IsNullOrEmpty(product.Element(_xmlNamespace + "customer-id")?.Value) 
                    ? 0
                    : Convert.ToInt32(product.Element(_xmlNamespace + "customer-id")?.Value),
                Name = product.Element(_xmlNamespace + "display-name")
                    ?.Value,
                UPC = product.Element(_xmlNamespace + "upc")
                    ?.Value,
                StatusID = product.Element(_xmlNamespace + "status-id")
                    ?.Value,
                StatusComments = product.Element(_xmlNamespace + "status-comments")?.Value,
                Package = product.Element(_xmlNamespace + "package")?.Value,
                Description = product.Element(_xmlNamespace + "short-description")
                    ?.Value,
                DescriptionLong = product.Element(_xmlNamespace + "long-description")
                    ?.Value,
                WarrantyPeriod = product.Element(_xmlNamespace + "warranty-period")?.Value,
                WarrantyBy = product.Element(_xmlNamespace + "warranty-by")?.Value,
                Makat = product.Element(_xmlNamespace + "makat")?.Value,
                Model = product.Element(_xmlNamespace + "model")?.Value,
                Price_Cost_Customer = string.IsNullOrEmpty(product.Element(_xmlNamespace + "price-cost-customer")?.Value) 
                    ? 0
                    : Convert.ToDecimal(product.Element(_xmlNamespace + "customer-id")?.Value),
                Price_Cost = string.IsNullOrEmpty(product.Element(_xmlNamespace + "price-cost")?.Value) 
                    ? 0 
                    : Convert.ToDecimal(product.Element(_xmlNamespace + "customer-id")?.Value),
                Price_Market = string.IsNullOrEmpty(product.Element(_xmlNamespace + "price-market")?.Value) 
                    ? 0 
                    : Convert.ToDecimal(product.Element(_xmlNamespace + "price-market")?.Value),
                Price_Publish = string.IsNullOrEmpty(product.Element(_xmlNamespace + "price-publish")?.Value) 
                    ? 0 
                    : Convert.ToDecimal(product.Element(_xmlNamespace + "price-publish")?.Value),
                Brand = product.Element(_xmlNamespace + "brand")
                    ?.Value,
                Volume = product.Element(_xmlNamespace + "volume")?.Value,
                ClassificationID = product.Element(_xmlNamespace + "classification-id")?.Value,
                SubClass = product.Element(_xmlNamespace + "subclass")?.Value,
                PlatformCategoryID = product.Element(_xmlNamespace + "platform-category-id")?.Value,
                VolumeType = product.Element(_xmlNamespace + "volume-type")?.Value,
                VideoLink =
                    customAttributes
                        .FirstOrDefault(attr => attr.Attribute("attribute-id")
                                                    ?.Value ==
                                                "f54ProductVideo")
                        ?.Value,
                StorageLocation = product.Element(_xmlNamespace + "storage-location")?.Value,
                Options = GetAdmonisProductOptions(product)
            };
        }

        /// <summary>
        /// Listing and creating the Admonis product options for the product
        /// </summary>
        /// <param name="product">The main product</param>
        /// <returns>List of the product options</returns>
        private List<AdmonisProductOption> GetAdmonisProductOptions(XElement product)
        {
            var productOptionsIds = product.Descendants(_xmlNamespace + "variant")
                .Select(v => v.Attribute("product-id")?.Value)
                .Where(id => !string.IsNullOrEmpty(id));

            var productOptions = productOptionsIds.Select(productOptionId => _xmlDocument.Element(_xmlNamespace + productOptionId)).ToList();
            return productOptions.Select(CreateAdmonisProductOption).ToList();
        }

        private AdmonisProductOption CreateAdmonisProductOption(XElement productOption)
        {
            var customAttributes = productOption.Descendants(_xmlNamespace + "custom-attribute");
            return new AdmonisProductOption
            {
                optionSugName1 = null,
                optionSugName1Title = null,
                optionSugName2 = productOption.Element(_xmlNamespace + "f54ProductColor")
                    ?.Value,
                optionSugName2Title = null,
                ProductMakat = null,
                optionMakat = null,
                optionName = productOption.Element(_xmlNamespace + "f54ProductSize")
                    ?.Value,
                optionModel = null,
                optionPrice_Cost_Customer = 0,
                optionPrice_Cost = 0,
                optionPrice_Publish = 0,
                optionPrice_Market = 0,
                optionstorageLocation = null,
                optionupc = null
            };
        }
    }
}