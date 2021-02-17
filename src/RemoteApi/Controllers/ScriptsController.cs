using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RemoteApi.Models.Mongo;
using RemoteCommon.Connector.Mongo;

namespace RemoteApi.Controllers
{
    [Route("api/scripts")]
    [ApiController]
    public class ScriptsController : ControllerBase
    {
        private readonly MinioClient _minioClient;
        private readonly MongoDbContext _mongoDbContext;

        public ScriptsController(MinioClient minioClient, MongoDbContext mongoDbContext)
        {
            _minioClient = minioClient;
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// Get scripts
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List(string keyword)
        {
            var query = _mongoDbContext.Collection<Script, string>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(n => n.Id.Contains(keyword) || n.Description.Contains(keyword));
            }

            var scripts = await query.ToListAsync();

            return Ok(scripts);
        }

        /// <summary>
        /// Upload script
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Upload(string name, string description, IFormFile file)
        {
            name = name.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Please specify name.");
            }

            var isExisted = await _mongoDbContext.Collection<Script, string>()
                                                       .Find(n => n.Id.Equals(name, StringComparison.OrdinalIgnoreCase))
                                                       .AnyAsync();

            if (isExisted)
            {
                return BadRequest("The name is duplicated.");
            }

            var bucket = "scripts";
            var contentType = "text/plain";

            var isBucketExisted = await _minioClient.BucketExistsAsync(bucket);
            if (!isBucketExisted)
            {
                await _minioClient.MakeBucketAsync(bucket);
            }

            await using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                await _minioClient.PutObjectAsync(bucket, name, stream, stream.Length, contentType);

                var script = new Script
                {
                    Id = name,
                    Description = description
                };

                await _mongoDbContext.Collection<Script, string>().InsertOneAsync(script);
            }
            
            return Ok();
        }

        /// <summary>
        /// Delete script
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var isExisted = await _mongoDbContext.Collection<Script, string>().Find(n => n.Id == id).AnyAsync();

            if (!isExisted)
            {
                return NotFound("The specified script is not found.");
            }

            await _mongoDbContext.Collection<Script, string>().DeleteOneAsync(n => n.Id == id);

            return NoContent();
        }
    }
}