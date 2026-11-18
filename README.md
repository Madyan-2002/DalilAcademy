# Dalil Academy LMS: Enrollment Payments (PayPal Sandbox)

## Run the project
1. Create a PostgreSQL database and set the connection string in User Secrets (below).
2. Set the user secrets (right-click DalilAcademy.API → Manage User Secrets):
   - ConnectionStrings:DefaultConnection
   - Jwt:Key (and the other Jwt settings)
   - PayPal:ClientId
   - PayPal:Secret
3. Apply the migrations (Package Manager Console, default project = DalilAcademy.Infra):
   `Update-Database`
4. Run DalilAcademy.API and open https://localhost:7011 (Swagger).
5. Default admin (seeded): admin@dalil.jo

## New endpoints (Admin only unless noted)
| Method | Route | Description |
|---|---|---|
| POST | /api/enrollments | Create an enrollment (Pending, or Paid if the course is free) |
| GET | /api/enrollments/me | The student's Paid courses (Student) |
| POST | /api/enrollments/{id}/pay | Create a PayPal order and a Payment row; returns approvalUrl |
| POST | /api/enrollments/{id}/capture | Capture the latest Created payment; enrollment becomes Paid |
| GET | /api/courses/{courseId}/students | Paid students of a course |

## Payment flow (Swagger)
1. Login as Admin and authorize.
2. Create an enrollment for a paid course (Pending).
3. Call /pay and open approvalUrl; approve with the PayPal sandbox buyer account.
4. Call /capture: payment Completed, enrollment Paid.

## Rules
- Amount = course price (JOD) x JodToUsdRate (1.41), rounded to 2 decimals, computed on the server.
- Paid enrollments cannot be deleted or moved (409).
- Capture failure: 400, payment Failed, enrollment stays Pending, call /pay again.
- PayPal unreachable: 503.