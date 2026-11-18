using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController(IStudentService studentService) : ControllerBase
    {
        // GET /api/students?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult<PageResult<StudentDto>>> GetAll(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await studentService.GetPagedAsync(page, pageSize));

        // GET /api/students/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StudentDto>> GetById(int id)
            => Ok(await studentService.GetByIdAsync(id));

        // GET /api/students/1/courses
        [HttpGet("{id:int}/courses")]
        public async Task<ActionResult<List<CourseResponseDto>>> GetCourses(int id)
            => Ok(await studentService.GetStudentCoursesAsync(id));

        [HttpPost]
        public async Task<ActionResult<StudentDto>> Create(CreateStudentDto dto)
        {
            var created = await studentService.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateStudentDto dto)
        {
            await studentService.UpdateStudentAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await studentService.DeleteStudentAsync(id);
            return NoContent();
        }
    }
}