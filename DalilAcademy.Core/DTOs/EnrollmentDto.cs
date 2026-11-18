namespace DalilAcademy.Core.DTOs
{
    public record CreateEnrollmentDto(int StudentId, int CourseId);

    public record UpdateEnrollmentDto(int CourseId);

    public record EnrollmentResponseDto(
        int Id,
        int StudentId,
        string StudentName,
        int CourseId,
        string CourseTitle,
        DateTime EnrolledAt,
        string Status);

    public record PayResponseDto(
        int PaymentId,
        string OrderId,
        decimal Amount,
        string Currency,
        string ApprovalUrl);
}