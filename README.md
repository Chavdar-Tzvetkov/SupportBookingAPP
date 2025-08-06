# SupportBookingAPP - ASP.NET Core MVC Project

## Project Overview
This is a fully functional role-based support booking platform developed using ASP.NET Core MVC with Identity and Entity Framework Core. It supports three types of users:

- **Admin** – Manages engineers and support categories.
- **Engineer** – Handles assigned support bookings and reviews notifications.
- **User** – Books support appointments with engineers.

The system integrates:
- Identity with custom roles and users
- Notifications for upcoming bookings
- Engineer and Admin Dashboards with role-restricted access
- Email notification stubs (using IEmailSender)
- xUnit test coverage for key controllers



## Login Credentials

> These credentials are stored in `secrets.json` and **not included** in the Git repository.
> You must configure these locally using the .NET Secret Manager before running the project.

### Admin Account

Email: admin@supportapp.com
Password: YourStrongP@ssword123
Role: Admin


### Engineer Accounts

| Email                         | Password       | Role     |
|------------------------------|----------------|----------|
| ivan.ivanov@example.com      | Passw0rd1!     | Engineer |
| maria.petrova@example.com    | Passw0rd2!     | Engineer |
| georgi.dimitrov@example.com  | Passw0rd3!     | Engineer |

These accounts are linked to seeded Engineer entities and can log in to the Engineer Dashboard.



##  Unit Testing

The solution includes **unit tests** for:
- Admin Area (EngineersController)
- Engineer Area (DashboardController)
- Booking logic with EntityFrameworkCore in-memory DB
- Notification display logic


## How to Run the Application

1. **Clone the repo and restore dependencies**
   

2. **Set up secrets.json**
   
   dotnet user-secrets set "AdminUser:Email" "admin@supportapp.com"
   dotnet user-secrets set "AdminUser:Password" "YourStrongP@ssword123"
   dotnet user-secrets set "EngineerAccounts:ivan.ivanov@example.com" "Passw0rd1!"
   dotnet user-secrets set "EngineerAccounts:maria.petrova@example.com" "Passw0rd2!"
   dotnet user-secrets set "EngineerAccounts:georgi.dimitrov@example.com" "Passw0rd3!"
   

3. **Apply EF Core migrations**
   Ensure the database is created and seeded.

   
   dotnet ef database update
   

4. **Run the app**
   
   dotnet run
   

5. **Login using the credentials above.**



## Project Structure

- `Areas/Admin` – Admin controllers and views
- `Areas/Engineers` – Engineer dashboards and actions
- `Controllers` – Global controllers like BookingsController
- `Models` – Application models and Identity user
- `Views` – Razor views per area
- `Services` – Logic for bookings and notifications
- `Tests` – Unit test coverage using xUnit





## Notes

- The `ConnectionStrings:DefaultConnection` is stored in `secrets.json`. Update it for your local environment.
- Email services are mocked and will not send real emails.
- The project uses `Bootstrap 5` for styling.



## Maintainer

Developed by **Chavdar Tzvetkov** as part of the C# Web Advanced course at SoftUni.

###### END ####