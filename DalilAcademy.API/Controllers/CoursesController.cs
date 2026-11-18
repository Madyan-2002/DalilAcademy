using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Service;
using DalilAcademy.Core.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController(ICourseService courseService , IImageService images) : ControllerBase
    {
        // GET /api/courses?page=1&pageSize=10&minPrice=100&maxPrice=500
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PageResult<CourseResponseDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
            => Ok(await courseService.GetPageAsync(page, pageSize, minPrice, maxPrice));

        // GET /api/courses/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCount()
            => Ok(await courseService.CountAsync());

        // GET /api/courses/1
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseResponseDto>> GetById(int id)
            => Ok(await courseService.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<CourseResponseDto>> CreateCourse(CreateCourseDto dto)
        {
            var created = await courseService.CreateCourseAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseDto dto)
        {
            await courseService.UpdateCourseAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            await courseService.DeleteCourseAsync(id);
            return NoContent();
        }

        [HttpPost]
        [Route("upload-image")]
        [Authorize(Roles = Roles.Admin)]

        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            var url = await images.SaveCourseImageAsync(stream , file.FileName, file.Length);
            return Ok(new { ImageUrl = url });
        }
    }
}