/**
 * Copyright 1993 - 2025 Cybernius Medical Ltd. All rights reserved.
 * Neither this document nor any part of it may be reproduced, stored in a
 * retrieval system or transmitted in any form or by any means, electronic,
 * mechanical, photocopying, recording or otherwise without the prior
 * permission of Cybernius Medical Ltd.
 * 
 * Description: A collection of all of the Image related objects. This includes
 * the Database functions, the controller to route to the DB functions, and the 
 * different models for the object (DTO, Domain, specific HTTP requests)
 * 
 * Changelog:
 * Ver  Author Bug           Date       Comments
 * =============================================================================
 * 100  mxl    N/A           14May2025  Initial Creation
 */

using CodePulse.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System;
using Microsoft.AspNetCore.Authorization;

namespace CodePulse.API.Models {

    /**
     * Routes HTTP actions to their associated database calls
     */
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase {

        private readonly IImageRepository imageRepository;
        public ImageController(IImageRepository imageRepository) {
            this.imageRepository = imageRepository;
        }

        [HttpGet] //https://localhost:xxxx/api/Image
        public async Task<IActionResult> getAll() {
            var images = await imageRepository.getAll();

            // convert the domain model to the DTO
            List<ImageDTO> response = new List<ImageDTO>();
            foreach (Image image in images) {
                response.Add(new ImageDTO {
                    id = image.id,
                    fileName = image.fileName,
                    fileExtension = image.fileExtension,
                    title = image.title,
                    url = image.url,
                    dateCreated = image.dateCreated
                });
            }

            return Ok(response);
        }


        // Creates a image in the database by way of an HTTP POST request,
        // this request should have an accompanying image DTO to put into the db 
        // The request should a title, description, conent, urlhandle, url to an image,
        // the date created, the author of the post, and if the post should be visible or not
        [HttpPost] // https://localhost:xxxx/api/Image
        [Authorize(Roles = "Writerode")]
        public async Task<IActionResult> uploadImage(IFormFile file, [FromForm] string fileName, [FromForm] string title) {
            ValidateFileUpload(file);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            // convert DTO to domain object
            Image image = new Image {
                fileExtension = Path.GetExtension(file.FileName).ToLower(),
                fileName = fileName,
                title = title,
                dateCreated = DateTime.Now
            };

            // upload the file to the database
            image = await imageRepository.upload(file, image);

            // convert the DTO back to a domain object
            ImageDTO response = new ImageDTO {
                id = image.id,
                fileName = image.fileName,
                fileExtension = image.fileExtension,
                title = image.title,
                url = image.url,
                dateCreated = image.dateCreated
            };

            return Ok(response);
        }

        private void ValidateFileUpload(IFormFile file) {
            // we only allow certain extensions
            string[] allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower())) {
                ModelState.AddModelError("file", "Unsupported file format");
            }

            // 10 MB is the max file size allowed
            if (file.Length > 10 * 1024 * 1024) {
                ModelState.AddModelError("file", "File size cannot be more than 10MB");
            }
        }

        
    }

    /**
     * The CRUD methods of a image (interface)
     */
    public interface IImageRepository {
        Task<Image> upload(IFormFile file, Image image);
        Task<IEnumerable<Image>> getAll();
    }

    /**
     * The CRUD methods of a image (implementation)
     */
    public class ImageDB : IImageRepository {
        private readonly ApplicationDbContext dbContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ImageDB(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext dbContext) {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Image>> getAll() {
            return await dbContext.Images.ToListAsync();
        }

        public async Task<Image> upload(IFormFile file, Image image) {
            // upload the image to API/Images
            string localPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", $"{image.fileName}{image.fileExtension}");
            using FileStream stream = new FileStream(localPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // update the database
            //https://codepulse.com/images/FILENAME.png
            HttpRequest httpRequest = httpContextAccessor.HttpContext.Request;
            string urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{image.fileName}{image.fileExtension}";

            image.url = urlPath;
            
            await dbContext.Images.AddAsync(image);
            await dbContext.SaveChangesAsync();

            return image;
        }
    }

    /**
     * The values of an Image that wholistically define it in our database
     */
    public class Image {
        public Guid id { get; set; }
        public string fileName { get; set; }
        public string fileExtension { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public DateTime dateCreated { get; set; }
    }

    /**
     * The values of a Image that we want to reveal to the users
     * on the front-end of our application
     */
    public class ImageDTO {
        public Guid id { get; set; }
        public string fileName { get; set; }
        public string fileExtension { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public DateTime dateCreated { get; set; }
    }
}


