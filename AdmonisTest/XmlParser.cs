using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using AdmonisTest.Admonis;

namespace AdmonisTest
{
    /// <summary>
    /// Responsible for the parsing of the product xml into Admonis objects functionality
    /// </summary>
    public class XmlParser
    {
        private readonly XNamespace _xmlNamespace;
        private readonly Dictionary<string, XElement> _products;

        public XmlParser(XDocument xmlDocument)
        {
            _xmlNamespace = xmlDocument.Root?.Name.Namespace;
            // Create <product-id, product> Dictionary to get product with O(1) time complexity
            _products = xmlDocument.Descendants(_xmlNamespace + "product").ToDictionary(
                product => product.Attribute("product-id")?.Value, product => product);
        }

        /// <summary>
        /// Lists and creating the Admonis products list out of the xml 
        /// </summary>
        /// <returns>List of all the products</returns>
        public List<AdmonisProduct> GetProducts()
        {
            var products = _products
                .Where(x => x.Value.Element(_xmlNamespace + "variations") != null).ToList();
            return products.Select(x => CreateAdmonisProduct(x.Value)).ToList();
        }
        
        
        private AdmonisProduct CreateAdmonisProduct(XElement product)
        {
            var customAttributes = product.Descendants(_xmlNamespace + "custom-attribute");
            var admonisProduct = new AdmonisProduct
            {
                CustomerID = int.Parse(product.Element( _xmlNamespace + "customer-id")?.Value ?? 0.ToString()),
                Name = product.Element(_xmlNamespace + "display-name")?.Value,
                UPC = product.Element(_xmlNamespace + "upc")?.Value,
                StatusID = product.Element(_xmlNamespace + "status-id")?.Value,
                StatusComments = product.Element(_xmlNamespace + "status-comments")?.Value,
                Package = product.Element(_xmlNamespace + "package")?.Value,
                Description = product.Element(_xmlNamespace + "short-description")?.Value,
                DescriptionLong = product.Element(_xmlNamespace + "long-description")?.Value,
                WarrantyPeriod = product.Element(_xmlNamespace + "warranty-period")?.Value,
                WarrantyBy = product.Element(_xmlNamespace + "warranty-by")?.Value,
                Makat = product.Element(_xmlNamespace + "makat")?.Value,
                Model = product.Element(_xmlNamespace + "model")?.Value,
                Price_Cost_Customer = decimal.Parse(product.Element(_xmlNamespace + "price-cost-customer")?.Value ?? 0.ToString()),
                Price_Cost = decimal.Parse(product.Element(_xmlNamespace + "price-cost")?.Value ?? 0.ToString()),
                Price_Market = decimal.Parse(product.Element(_xmlNamespace + "price-market")?.Value ?? 0.ToString()),
                Price_Publish = decimal.Parse(product.Element(_xmlNamespace + "price-publish")?.Value ?? 0.ToString()),
                Brand = product.Element(_xmlNamespace + "brand")?.Value,
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
            };
            admonisProduct.Options = GetAdmonisProductOptions(product, admonisProduct);
            return admonisProduct;
        }

        /// <summary>
        /// Listing and creating the Admonis product options for the product
        /// </summary>
        /// <param name="product">The main product element</param>
        /// <param name="admonisProduct">The main product object</param>
        /// <returns>List of the product options</returns>
        private List<AdmonisProductOption> GetAdmonisProductOptions(XElement product, AdmonisProduct admonisProduct)
        {

            var productOptionsIds = product.Descendants(_xmlNamespace + "variant")
                .Select(v => v.Attribute("product-id")?.Value)
                .Where(id => !string.IsNullOrEmpty(id)).ToList();
            
            var productOptions = productOptionsIds
                .Select(productOptionId => _products[productOptionId]) // Exclude null matches
                .ToList();
            
            return productOptions.Select(productOption => 
                CreateAdmonisProductOption(productOption, admonisProduct)).ToList();
        }

        private AdmonisProductOption CreateAdmonisProductOption(XElement productOption, AdmonisProduct product)
        {
            var customAttributes = productOption.Descendants(_xmlNamespace + "custom-attribute").ToList();
            return new AdmonisProductOption
            {
                optionSugName1 = productOption.Element(_xmlNamespace + "option-sug-name-1")?.Value ?? "צבע",
                optionSugName1Title = productOption.Element(_xmlNamespace + "option-sug-name-1-title")?.Value ?? "בחר צבע",
                ProductMakat = product.Makat,
                optionMakat = productOption.Element("option-maket")?.Value,
                optionSugName2 = customAttributes
                    .FirstOrDefault(x => (string)x.Attribute("attribute-id") == "f54ProductColor")
                    ?.Value,
                optionSugName2Title = productOption.Element(_xmlNamespace + "option-sug-name-2-title")?.Value ?? "בחר מידה",
                optionName = customAttributes
                    .FirstOrDefault(x => (string)x.Attribute("attribute-id") == "f54ProductSize")
                    ?.Value,
                optionModel = productOption.Element("model")?.Value,
                optionPrice_Cost_Customer = decimal.Parse(productOption.Element("price-cost-customer")?.Value ?? 0.ToString()),
                optionPrice_Cost = decimal.Parse(productOption.Element("price-cost")?.Value ?? 0.ToString()),
                optionPrice_Publish = decimal.Parse(productOption.Element("price-publish")?.Value ?? 0.ToString()),
                optionPrice_Market = decimal.Parse(productOption.Element("price-market")?.Value ?? 0.ToString()),
                optionstorageLocation = productOption.Element("storage-location")?.Value,
                optionupc = productOption.Element("upc")?.Value
            };
        }
    }
}