using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using storeAPI.Controllers;
using storeAPI.DTOs;
using storeAPI.Interfaces;
using storeAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace storeAPI.Tests
{
    public class ProductControllerTest
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductController _productController;

        public ProductControllerTest()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockMapper = new Mock<IMapper>();
            _productController = new ProductController(_mockProductRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var productList = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Effect = "Effect 1", Caffeine = "High caffeine", Type = "Type 1", CategoryId = 1 },
                new Product { Id = 2, Name = "Product 2", Effect = "Effect 2", Caffeine = "High caffeine", Type = "Type 2", CategoryId = 1  }
            };
            var productDTOList = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product 1", Effect = "Effect 1", Caffeine = "High caffeine", Type = "Type 1", CategoryId = 1 },
                new ProductDTO { Id = 2, Name = "Product 2", Effect = "Effect 2", Caffeine = "High caffeine", Type = "Type 2", CategoryId = 1 }
            };

            _mockProductRepository.Setup(repo => repo.GetProducts()).ReturnsAsync(productList);
            _mockMapper.Setup(m => m.Map<List<ProductDTO>>(productList)).Returns(productDTOList);

            // Act
            var result = await _productController.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal(productDTOList[0].Name, returnValue[0].Name);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            _mockProductRepository.Setup(repo => repo.GetProduct(productId)).ReturnsAsync((Product)null);

            // Act
            var result = await _productController.GetProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            var product = new Product { Id = productId, Name = "Product 1", Effect = "Effect 1", Caffeine = "High caffeine", Type = "Type 1", CategoryId = 1 };
            var productDTO = new ProductDTO { Id = productId, Name = "Product 1", Effect = "Effect 1", Caffeine = "High caffeine", Type = "Type 1", CategoryId = 1 };

            _mockProductRepository.Setup(repo => repo.GetProduct(productId)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductDTO>(product)).Returns(productDTO);

            // Act
            var result = await _productController.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProductDTO>(okResult.Value);
            Assert.Equal(productId, returnValue.Id);
            Assert.Equal("Product 1", returnValue.Name);
        }

        [Fact]
        public async Task AddProduct_ReturnsCreatedAtAction()
        {
            // Arrange
            var productDTO = new ProductDTO { Id = 1, Name = "New Product", Effect = "New Effect", Caffeine = "High caffeine", Type = "New Type", CategoryId = 1 };
            var product = new Product { Id = 1, Name = "New Product", Effect = "New Effect", Caffeine = "High caffeine", Type = "New Type", CategoryId = 1 };

            _mockMapper.Setup(m => m.Map<Product>(productDTO)).Returns(product);
            _mockProductRepository.Setup(repo => repo.AddProduct(product, null)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductDTO>(product)).Returns(productDTO);

            // Act
            var result = await _productController.AddProduct(productDTO, null);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result); // Check if result is CreatedAtActionResult
            var returnValue = Assert.IsType<ProductDTO>(createdAtActionResult.Value); // Check if the value is ProductDTO
            Assert.Equal(productDTO.Id, returnValue.Id); // Ensure that the returned ProductDTO has the correct Id
        }


        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            var productDTO = new ProductDTO { Id = productId, Name = "Updated Product", Effect = "Updated Effect", Caffeine = "High caffeine", Type = "Updated Type", CategoryId = 1 };

            _mockProductRepository.Setup(repo => repo.UpdateProduct(productId, It.IsAny<Product>(), It.IsAny<List<IFormFile>>())).ReturnsAsync((Product)null);

            // Act
            var result = await _productController.UpdateProduct(productId, productDTO, null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            int productId = 1;
            var productDTO = new ProductDTO { Id = productId, Name = "Updated Product", Effect = "Updated Effect", Caffeine = "High caffeine", Type = "Updated Type", CategoryId = 1 };
            var product = new Product { Id = productId, Name = "Updated Product", Effect = "Updated Effect", Caffeine = "High caffeine", Type = "Updated Type", CategoryId = 1 };

            _mockMapper.Setup(m => m.Map<Product>(productDTO)).Returns(product);
            _mockProductRepository.Setup(repo => repo.UpdateProduct(productId, product, null)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductDTO>(product)).Returns(productDTO);

            // Act
            var result = await _productController.UpdateProduct(productId, productDTO, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Check if result is OkObjectResult
            var returnValue = Assert.IsType<ProductDTO>(okResult.Value); // Check if the value is ProductDTO
            Assert.Equal(productDTO.Name, returnValue.Name); // Ensure that the returned ProductDTO has the updated name
        }


        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;

            // Mock the DeleteProduct method to return null, indicating the product does not exist
            _mockProductRepository.Setup(repo => repo.DeleteProduct(productId)).ReturnsAsync((Product)null);

            // Act
            var result = await _productController.DeleteProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            var product = new Product { Id = productId, Name = "Product 1", Effect = "Effect 1", Caffeine = "High caffeine", Type = "Type 1", CategoryId = 1 };

            _mockProductRepository.Setup(repo => repo.DeleteProduct(productId)).ReturnsAsync(product);
            _mockProductRepository.Setup(repo => repo.GetProducts()).ReturnsAsync(new List<Product> { product });
            _mockMapper.Setup(m => m.Map<List<ProductDTO>>(It.IsAny<List<Product>>())).Returns(new List<ProductDTO>
    {
        new ProductDTO { Id = 1, Name = "Product 1", Effect = "Effect 1", Caffeine = "High caffeine", Type = "Type 1", CategoryId = 1 }
    });

            // Act
            var result = await _productController.DeleteProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var returnValue = Assert.IsType<List<ProductDTO>>(okResult.Value); 
            Assert.Single(returnValue); 
        }

    }
}
