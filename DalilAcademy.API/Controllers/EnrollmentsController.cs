using System.Security.Claims;
using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entites;
using DalilAcademy.Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DalilAcademy.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController(
        IEnrollmentService enrollmentService,
        IPaymentService paymentService,
        IStudentService studentService) : ControllerBase
    {
        // GET /api/enrollments
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<List<EnrollmentResponseDto>>> GetAll()
            => Ok(await enrollmentService.GetAllAsync());

        // GET /api/enrollments/me  (Student: الكورسات المدفوعة فقط)
        [HttpGet("me")]
        public async Task<ActionResult<List<CourseResponseDto>>> GetMine()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await studentService.GetMyCoursesAsync(userId));
        }

        // GET /api/enrollments/5
        [HttpGet("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<EnrollmentResponseDto>> GetById(int id)
            => Ok(await enrollmentService.GetByIdAsync(id));

        // POST /api/enrollments
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<EnrollmentResponseDto>> Create(CreateEnrollmentDto dto)
        {
            var created = await enrollmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT /api/enrollments/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, UpdateEnrollmentDto dto)
        {
            await enrollmentService.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE /api/enrollments/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await enrollmentService.DeleteAsync(id);
            return NoContent();
        }

        // POST /api/enrollments/5/pay
        [HttpPost("{id:int}/pay")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<PayResponseDto>> Pay(int id, CancellationToken ct)
            => Ok(await paymentService.PayAsync(id, ct));

        // POST /api/enrollments/5/capture
        [HttpPost("{id:int}/capture")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<EnrollmentResponseDto>> Capture(int id, CancellationToken ct)
            => Ok(await paymentService.CaptureAsync(id, ct));
    }
}