/**
 * Copyright 1993 - 2025 Cybernius Medical Ltd. All rights reserved.
 * Neither this document nor any part of it may be reproduced, stored in a
 * retrieval system or transmitted in any form or by any means, electronic,
 * mechanical, photocopying, recording or otherwise without the prior
 * permission of Cybernius Medical Ltd.
 * 
 * Description: A collection of all of the BlogPost related objects. This includes
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
    public class BlogPostController : ControllerBase {

        private readonly IBlogPostRepository blogPostRepository;
        public BlogPostController(IBlogPostRepository blogPostRepository) {
            this.blogPostRepository = blogPostRepository;
        }

        // Creates a blogPost in the database by way of an HTTP POST request,
        // this request should have an accompanying blogPost DTO to put into the db 
        // The request should a title, description, content, urlhandle, url to an image,
        // the date created, the author of the post, and if the post should be visible or not
        [HttpPost] // https://localhost:xxxx/api/BlogPosts
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateBlogPost(CreateBlogPostRequest request, ICategoryRepository categoryRepository) {
            // convert DTO (front-end-representation) to Domain Model (back-end-representation)
            BlogPost blogPost = new BlogPost {
                title = request.title,
                shortDescription = request.shortDescription,
                content = request.content,
                urlHandle = request.urlHandle,
                featuredImageUrl = request.featuredImageUrl,
                publishedDate = request.publishedDate,
                author = request.author,
                isVisible = request.isVisible,
                categories = new List<Category>()
            };

            foreach (Guid categoryGuid in request.categories) {
                Category? existingCategory = await categoryRepository.getByID(categoryGuid);
                if (existingCategory is not null) {
                    blogPost.categories.Add(existingCategory);
                }
            }

            await blogPostRepository.create(blogPost);

            // convert from Domain Model (back-end-representation) back to DTO (front-end-representation)
            BlogPostDTO response = new BlogPostDTO {
                id = blogPost.id,
                title = blogPost.title,
                shortDescription = blogPost.shortDescription,
                content = blogPost.content,
                urlHandle = blogPost.urlHandle,
                featuredImageUrl = blogPost.featuredImageUrl,
                publishedDate = blogPost.publishedDate,
                author = blogPost.author,
                isVisible = blogPost.isVisible,
                categories = blogPost.categories.Select(x => new CategoryDTO {
                    id = x.id,
                    name = x.name,
                    urlHandle = x.urlHandle
                }).ToList()
            };

            return Ok(response);
        }

        // Collects all of the blogPosts stored in the database by way of an HTTP GET request
        [HttpGet] // https://localhost:xxxx/api/BlogPosts
        public async Task<IActionResult> getAllBlogPosts() {
            IEnumerable<BlogPost> BlogPosts = await blogPostRepository.getAll();

            //convert Domain Model to DTO
            List<BlogPostDTO> response = new List<BlogPostDTO>();
            foreach (BlogPost blogPost in BlogPosts) {
                // convert from Domain Model (back-end-representation) back to DTO (front-end-representation)
                response.Add(new BlogPostDTO {
                    id = blogPost.id,
                    title = blogPost.title,
                    shortDescription = blogPost.shortDescription,
                    content = blogPost.content,
                    urlHandle = blogPost.urlHandle,
                    featuredImageUrl = blogPost.featuredImageUrl,
                    publishedDate = blogPost.publishedDate,
                    author = blogPost.author,
                    isVisible = blogPost.isVisible,
                    categories = blogPost.categories.Select(x => new CategoryDTO {
                        id = x.id,
                        name = x.name,
                        urlHandle = x.urlHandle
                    }).ToList()
                });
            }
            return Ok(response);
        }

        // Collects a specific blogPost by its ID by way of an HTTP GET request
        [HttpGet]
        [Route("{id:Guid}")] // https://localhost:xxxx/api/BlogPosts/{id}
        public async Task<IActionResult> getBlogPostByID([FromRoute] Guid id) {
            BlogPost? blogPost = await blogPostRepository.getByID(id);

            if (blogPost is null) {
                return NotFound();
            }

            // convert from Domain Model (back-end-representation) back to DTO (front-end-representation)
            BlogPostDTO response = new BlogPostDTO {
                id = blogPost.id,
                title = blogPost.title,
                shortDescription = blogPost.shortDescription,
                content = blogPost.content,
                urlHandle = blogPost.urlHandle,
                featuredImageUrl = blogPost.featuredImageUrl,
                publishedDate = blogPost.publishedDate,
                author = blogPost.author,
                isVisible = blogPost.isVisible,
                categories = blogPost.categories.Select(x => new CategoryDTO {
                    id = x.id,
                    name = x.name,
                    urlHandle = x.urlHandle
                }).ToList()
            };
            return Ok(response);
        }

        // Collects a specific blogPost by its ID by way of an HTTP GET request
        [HttpGet]
        [Route("{urlHandle}")] // https://localhost:xxxx/api/BlogPosts/{urlHandle}
        public async Task<IActionResult> getBlogPostByUrlHandle([FromRoute] string urlHandle) {
            BlogPost? blogPost = await blogPostRepository.getByUrlHandle(urlHandle);

            if (blogPost is null) {
                return NotFound();
            }

            // convert from Domain Model (back-end-representation) back to DTO (front-end-representation)
            BlogPostDTO response = new BlogPostDTO {
                id = blogPost.id,
                title = blogPost.title,
                shortDescription = blogPost.shortDescription,
                content = blogPost.content,
                urlHandle = blogPost.urlHandle,
                featuredImageUrl = blogPost.featuredImageUrl,
                publishedDate = blogPost.publishedDate,
                author = blogPost.author,
                isVisible = blogPost.isVisible,
                categories = blogPost.categories.Select(x => new CategoryDTO {
                    id = x.id,
                    name = x.name,
                    urlHandle = x.urlHandle
                }).ToList()
            };
            return Ok(response);
        }

        // Updates a blogPost matching it on its unique ID by way of an HTTP PUT request
        [HttpPut]
        [Route("{id:Guid}")] // https://localhost:xxxx/api/BlogPosts/{id}
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> updateBlogPostByID([FromRoute] Guid id, UpdateBlogPostRequest request, ICategoryRepository categoryRepository) {
            // convert DTO (front-end-representation) to Domain Model (back-end-representation)
            BlogPost? blogPost = new BlogPost {
                id = id,
                title = request.title,
                shortDescription = request.shortDescription,
                content = request.content,
                urlHandle = request.urlHandle,
                featuredImageUrl = request.featuredImageUrl,
                publishedDate = request.publishedDate,
                author = request.author,
                isVisible = request.isVisible,
                categories = new List<Category>()
            };
            foreach (Guid categoryGuid in request.categories) {
                Category? existingCategory = await categoryRepository.getByID(categoryGuid);
                if (existingCategory != null) {
                    blogPost.categories.Add(existingCategory);
                }
            }

            blogPost = await blogPostRepository.update(blogPost);

            if (blogPost == null) {
                return NotFound();
            }

            // convert from Domain Model (back-end-representation) back to DTO (front-end-representation)
            BlogPostDTO response = new BlogPostDTO {
                id = blogPost.id,
                title = blogPost.title,
                shortDescription = blogPost.shortDescription,
                content = blogPost.content,
                urlHandle = blogPost.urlHandle,
                featuredImageUrl = blogPost.featuredImageUrl,
                publishedDate = blogPost.publishedDate,
                author = blogPost.author,
                isVisible = blogPost.isVisible,
                categories = blogPost.categories.Select(categoryDTO => new CategoryDTO {
                    id = categoryDTO.id,
                    name = categoryDTO.name,
                    urlHandle = categoryDTO.urlHandle
                }).ToList()
            };
            return Ok(response);
        }

        // Delete a blogPost by its unique index by way of an HTTP DELETE request
        [HttpDelete]
        [Route("{id:Guid}")] // https://localhost:xxxx/api/BlogPosts/{id}
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> deleteBlogPostByID([FromRoute] Guid id) {
            BlogPost? blogPost = await blogPostRepository.delete(id);

            if (blogPost == null) {
                return NotFound();
            }

            // convert from Domain Model (back-end-representation) back to DTO (front-end-representation)
            BlogPostDTO response = new BlogPostDTO {
                id = blogPost.id,
                title = blogPost.title,
                shortDescription = blogPost.shortDescription,
                content = blogPost.content,
                urlHandle = blogPost.urlHandle,
                featuredImageUrl = blogPost.featuredImageUrl,
                publishedDate = blogPost.publishedDate,
                author = blogPost.author,
                isVisible = blogPost.isVisible
            };
            return Ok(response);
        }

    }

    /**
     * The CRUD methods of a blogPost (interface)
     */
    public interface IBlogPostRepository {
        Task<BlogPost> create(BlogPost blogPost);
        Task<IEnumerable<BlogPost>> getAll();
        Task<BlogPost?> getByID(Guid id);
        Task<BlogPost?> getByUrlHandle(string urlHandle);
        Task<BlogPost?> update(BlogPost blogPost);
        Task<BlogPost?> delete(Guid id);
    }

    /**
     * The CRUD methods of a blogPost (implementation)
     */
    public class BlogPostDB : IBlogPostRepository {
        private readonly ApplicationDbContext dbContext;

        public BlogPostDB(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<BlogPost> create(BlogPost blogPost) {
            await dbContext.BlogPosts.AddAsync(blogPost);
            await dbContext.SaveChangesAsync();

            return blogPost;
        }

        public async Task<IEnumerable<BlogPost>> getAll() {
            return await dbContext.BlogPosts.Include(x => x.categories).ToListAsync();
        }

        public async Task<BlogPost?> getByID(Guid id) {
            return await dbContext.BlogPosts.Include(x => x.categories).FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<BlogPost?> getByUrlHandle(string urlHandle) {
            return await dbContext.BlogPosts.Include(x => x.categories).FirstOrDefaultAsync(x => x.urlHandle == urlHandle);
        }

        public async Task<BlogPost?> update(BlogPost blogPost) {
            BlogPost? existingBlogPost = await dbContext.BlogPosts.Include(x => x.categories).FirstOrDefaultAsync(x => x.id == blogPost.id);
            if (existingBlogPost == null) {
                return null;
            }

            //update blogpost
            dbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
            //update categories
            existingBlogPost.categories = blogPost.categories;

            await dbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> delete(Guid id) {
            BlogPost? existingBlogPost = await dbContext.BlogPosts.FirstOrDefaultAsync(x => x.id == id);
            if (existingBlogPost == null) {
                return null;
            }
            dbContext.BlogPosts.Remove(existingBlogPost);
            await dbContext.SaveChangesAsync();
            return existingBlogPost;
        }
    }

    /**
     * The domain model (back-end representation) of a BlogPost
     */
    public class BlogPost {
        public Guid id { get; set; }
        public required string title { get; set; }
        public required string shortDescription { get; set; }
        public required string content { get; set; }
        public required string urlHandle { get; set; }
        public required string featuredImageUrl { get; set; }
        public DateTime publishedDate { get; set; }
        public required string author { get; set; }
        public required bool isVisible { get; set; }
        public required ICollection<Category> categories { get; set; }
    }

    /**
     * The values of a BlogPost that we want to reveal to the users
     * on the front-end of our application
     */
    public class BlogPostDTO {
        public Guid id { get; set; }
        public required string title { get; set; }
        public required string shortDescription { get; set; }
        public required string content { get; set; }
        public required string urlHandle { get; set; }
        public required string featuredImageUrl { get; set; }
        public DateTime publishedDate { get; set; }
        public required string author { get; set; }
        public required bool isVisible { get; set; }
        public List<CategoryDTO> categories { get; set; } = new List<CategoryDTO>();
    }

    /**
     * The different sets of information needed to make different CRUD requests
     */
    public class DeleteBlogPostRequest {
        public required string title { get; set; }
        public required string shortDescription { get; set; }
        public required string content { get; set; }
        public required string urlHandle { get; set; }
        public required string featuredImageUrl { get; set; }
        public DateTime publishedDate { get; set; }
        public required string author { get; set; }
        public required bool isVisible { get; set; }
    }
    public class CreateBlogPostRequest {
        public required string title { get; set; }
        public required string shortDescription { get; set; }
        public required string content { get; set; }
        public required string urlHandle { get; set; }
        public required string featuredImageUrl { get; set; }
        public DateTime publishedDate { get; set; }
        public required string author { get; set; }
        public required bool isVisible { get; set; }
        public required Guid[] categories {get; set;}
    }
    public class UpdateBlogPostRequest {
        public required string title { get; set; }
        public required string shortDescription { get; set; }
        public required string content { get; set; }
        public required string urlHandle { get; set; }
        public required string featuredImageUrl { get; set; }
        public DateTime publishedDate { get; set; }
        public required string author { get; set; }
        public required bool isVisible { get; set; }
        public List<Guid> categories { get; set; } = new List<Guid>();
    }
}

