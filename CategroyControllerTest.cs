using AutoMapper;
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
    public class CategoryControllerTest
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryController _categoryController;

        public CategoryControllerTest()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _categoryController = new CategoryController(_mockCategoryRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetCategories_ReturnsOkResult_WithListOfCategories()
        {
            // Arrange
            var categoryList = new List<Category>
            {
                new Category { Id = 1, Name = "Black tea" },
                new Category { Id = 2, Name = "Green tea" }
            };
            var categoryDTOList = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Black tea" },
                new CategoryDTO { Id = 2, Name = "Green tea" }
            };

            _mockCategoryRepository.Setup(repo => repo.GetCategories()).ReturnsAsync(categoryList);
            _mockMapper.Setup(mapper => mapper.Map<List<CategoryDTO>>(categoryList)).Returns(categoryDTOList);

            // Act
            var result = await _categoryController.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<CategoryDTO>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            int categoryId = 1;
            _mockCategoryRepository.Setup(repo => repo.GetCategory(categoryId)).ReturnsAsync((Category)null);

            // Act
            var result = await _categoryController.GetCategory(categoryId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCategory_ReturnsOkResult_WhenCategoryExists()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Black tea" };
            var categoryDTO = new CategoryDTO { Id = categoryId, Name = "Black tea" };

            _mockCategoryRepository.Setup(repo => repo.GetCategory(categoryId)).ReturnsAsync(category);
            _mockMapper.Setup(mapper => mapper.Map<CategoryDTO>(category)).Returns(categoryDTO);

            // Act
            var result = await _categoryController.GetCategory(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryDTO>(okResult.Value);
            Assert.Equal(categoryId, returnValue.Id);
        }

        [Fact]
        public async Task AddCategory_ReturnsCreatedAtAction()
        {
            // Arrange
            var categoryDTO = new CategoryDTO { Id = 1, Name = "Green tea" };
            var category = new Category { Id = 1, Name = "Red tea" };

            _mockMapper.Setup(mapper => mapper.Map<Category>(categoryDTO)).Returns(category);
            _mockCategoryRepository.Setup(repo => repo.AddCategory(category)).ReturnsAsync(category);
            _mockMapper.Setup(mapper => mapper.Map<CategoryDTO>(category)).Returns(categoryDTO);

            // Act
            var result = await _categoryController.AddCategory(categoryDTO);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<CategoryDTO>(createdAtActionResult.Value);
            Assert.Equal(categoryDTO.Id, returnValue.Id);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            int categoryId = 1;
            var categoryDTO = new CategoryDTO { Id = categoryId, Name = "Updated Category" };

            _mockCategoryRepository.Setup(repo => repo.UpdateCategory(categoryId, It.IsAny<Category>())).ReturnsAsync((Category)null);

            // Act
            var result = await _categoryController.UpdateCategory(categoryId, categoryDTO);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            int categoryId = 1;
            var categoryDTO = new CategoryDTO { Id = categoryId, Name = "Updated Category" };
            var category = new Category { Id = categoryId, Name = "Updated Category" };

            _mockMapper.Setup(mapper => mapper.Map<Category>(categoryDTO)).Returns(category);
            _mockCategoryRepository.Setup(repo => repo.UpdateCategory(categoryId, category)).ReturnsAsync(category);
            _mockMapper.Setup(mapper => mapper.Map<CategoryDTO>(category)).Returns(categoryDTO);

            // Act
            var result = await _categoryController.UpdateCategory(categoryId, categoryDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryDTO>(okResult.Value);
            Assert.Equal(categoryDTO.Name, returnValue.Name);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            int categoryId = 1;

            _mockCategoryRepository.Setup(repo => repo.DeleteCategory(categoryId)).ReturnsAsync((Category)null);

            // Act
            var result = await _categoryController.DeleteCategory(categoryId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsOkResult_WhenCategoryIsDeleted()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Red tea" };
            var categoryList = new List<Category>
    {
        new Category { Id = 2, Name = "Black tea" },
        new Category { Id = 3, Name = "Green tea" }
    };
            var categoryDTOList = new List<CategoryDTO>
    {
        new CategoryDTO { Id = 2, Name = "Black tea" },
        new CategoryDTO { Id = 3, Name = "Green tea" }
    };

            _mockCategoryRepository.Setup(repo => repo.DeleteCategory(categoryId)).ReturnsAsync(category);
            _mockCategoryRepository.Setup(repo => repo.GetCategories()).ReturnsAsync(categoryList);
            _mockMapper.Setup(mapper => mapper.Map<List<CategoryDTO>>(categoryList)).Returns(categoryDTOList);

            // Act
            var result = await _categoryController.DeleteCategory(categoryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); 
            var returnValue = Assert.IsType<List<CategoryDTO>>(okResult.Value); 
            Assert.Equal(2, returnValue.Count); 
        }

    }
}
