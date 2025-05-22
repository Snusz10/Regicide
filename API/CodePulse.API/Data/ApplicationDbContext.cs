/**
 * Copyright 1993 - 2025 Cybernius Medical Ltd. All rights reserved.
 * Neither this document nor any part of it may be reproduced, stored in a
 * retrieval system or transmitted in any form or by any means, electronic,
 * mechanical, photocopying, recording or otherwise without the prior
 * permission of Cybernius Medical Ltd.
 * 
 * Description: A middle man to service all of the database calls we would like to
 * make. The settings we need to query our database are saved within the options
 * parameter, and can be reused by this class instead of having to redfine a connection
 * string (or similar) every time we are to make a db call.
 * 
 * Changelog:
 * Ver  Author Bug           Date       Comments
 * =============================================================================
 * 100  mxl    N/A           01May2025  Initial Creation
 */
using CodePulse.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

        }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }

    }
}
