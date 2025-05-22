/**
 * Copyright 1993 - 2025 Cybernius Medical Ltd. All rights reserved.
 * Neither this document nor any part of it may be reproduced, stored in a
 * retrieval system or transmitted in any form or by any means, electronic,
 * mechanical, photocopying, recording or otherwise without the prior
 * permission of Cybernius Medical Ltd.
 * 
 * Description: A collection of all of the Category related objects. This includes
 * the Database functions, the controller to route to the DB functions, and the 
 * different models for the object (DTO, Domain, specific HTTP requests)
 * 
 * Changelog:
 * Ver  Author Bug           Date       Comments
 * =============================================================================
 * 100  mxl    N/A           01May2025  Initial Creation
 */
using CodePulse.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Models {

    /**
     * Routes HTTP actions to their associated database calls
     */
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase {

        private readonly ICategoryRepository categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository) {
            this.categoryRepository = categoryRepository;
        }

        // Creates a category in the database by leiu of an HTTP POST request,
        // this request should have an accompanying category DTO to put into the db 
        // The request should specify a name, and a urlHandle
        [HttpPost] // https://localhost:xxxx/api/Categories
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequest request) {
            // convert DTO (front-end-representation) to Domain Model (back-end-representation)
            Category category = new Category {
                name = request.name,
                urlHandle = request.urlHandle
            };

            await categoryRepository.createAsync(category);

            // convert from Domain Model (back-end-representation) back to DTO (front-end-representation)
            CategoryDTO response = new CategoryDTO {
                id = category.id,
                name = category.name,
                urlHandle = category.urlHandle
            };

            return Ok(response);
        }

        // Collects all of the categories stored in the database by leiu of an HTTP GET request
        [HttpGet] // https://localhost:xxxx/api/Categories
        public async Task<IActionResult> getAllCategories() {
            IEnumerable<Category> Categories = await categoryRepository.getAll();

            //convert Domain Model to DTO
            List<CategoryDTO> response = new List<CategoryDTO>();
            foreach (Category category in Categories) {
                response.Add(new CategoryDTO {
                    id = category.id,
                    name = category.name,
                    urlHandle = category.urlHandle
                });
            }
            return Ok(response);
        }

        // Collects a specific category by its ID by leiu of an HTTP GET request
        [HttpGet]
        [Route("{id:Guid}")] // https://localhost:xxxx/api/Categories/{id}
        public async Task<IActionResult> getCategoryByID([FromRoute] Guid id) {
            Category? category = await categoryRepository.getByID(id);

            if (category is null) {
                return NotFound();
            }

            //convert Domain Model to DTO
            CategoryDTO response = new CategoryDTO() {
                id = category.id,
                name = category.name,
                urlHandle = category.urlHandle
            };
            return Ok(response);
        }

        // Updates a category matching it on its unique ID by leiu of an HTTP PUT request
        [HttpPut]
        [Route("{id:Guid}")] // https://localhost:xxxx/api/Categories/{id}
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> updateCategoryByID([FromRoute] Guid id, UpdateCategoryRequest request) {
            // convert DTO to domain model
            Category? category = new Category {
                id = id,
                name = request.name,
                urlHandle = request.urlHandle
            };

            category = await categoryRepository.updateAsync(category);

            if (category == null) {
                return NotFound();
            }

            //convert domain model to DTO
            CategoryDTO response = new CategoryDTO {
                id = category.id,
                name = category.name,
                urlHandle = category.urlHandle
            };
            return Ok(response);
        }

        // Delete a category by its unique index by leiu of an HTTP DELETE request
        [HttpDelete]
        [Route("{id:Guid}")] // https://localhost:xxxx/api/Categories/{id}
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> deleteCategoryByID([FromRoute] Guid id) {
            Category? category = await categoryRepository.deleteAsync(id);

            if (category == null) {
                return NotFound();
            }

            //convert domain model to DTO
            CategoryDTO response = new CategoryDTO {
                id = category.id,
                name = category.name,
                urlHandle = category.urlHandle
            };
            return Ok(response);
        }

    }

    /**
     * The CRUD methods of a category (interface)
     */
    public interface ICategoryRepository {
        Task<Category> createAsync(Category category);

        Task<IEnumerable<Category>> getAll();

        Task<Category?> getByID(Guid id);

        Task<Category?> updateAsync(Category category);

        Task<Category?> deleteAsync(Guid id);
    }

    /**
     * The CRUD methods of a category (implementation)
     */
    public class CategoryDB : ICategoryRepository {
        private readonly ApplicationDbContext dbContext;

        public CategoryDB(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<Category> createAsync(Category category) {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            return category;
        }

        public async Task<IEnumerable<Category>> getAll() {
            return await dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> getByID(Guid id) {
            return await dbContext.Categories.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<Category?> updateAsync(Category category) {
            Category? existingCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.id == category.id);
            if (existingCategory == null) {
                return null;
            }

            dbContext.Entry(existingCategory).CurrentValues.SetValues(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> deleteAsync(Guid id) {
            Category? existingCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.id == id);
            if (existingCategory == null) {
                return null;
            }
            dbContext.Categories.Remove(existingCategory);
            await dbContext.SaveChangesAsync();
            return existingCategory;
        }
    }

    /**
     * The domain model (back-end representation) of a Category
     */
    public class Category {
        public Guid id { get; set; }
        public required string name { get; set; }
        public required string urlHandle { get; set; }

        public ICollection<BlogPost> blogPosts { get; set; }
    }

    /**
     * The values of a Category that we want to reveal to the users
     * on the front-end of our application
     */
    public class CategoryDTO {
        public Guid id { get; set; }
        public required string name { get; set; }
        public required string urlHandle { get; set; }
    }

    /**
     * The different sets of information needed to make different CRUD requests
     */
    public class DeleteCategoryRequest {
        public required string name { get; set; }
        public required string urlHandle { get; set; }
    }
    public class CreateCategoryRequest {
        public required string name { get; set; }
        public required string urlHandle { get; set; }

    }
    public class UpdateCategoryRequest {
        public required string name { get; set; }
        public required string urlHandle { get; set; }
    }
}
