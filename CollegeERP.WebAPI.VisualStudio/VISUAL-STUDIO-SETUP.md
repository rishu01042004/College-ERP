# CollegeERP ASP.NET Core Web API — Visual Studio Setup

This package is a **single ASP.NET Core Web API**. It does not use Docker or microservices.

## 1. What is inside the project?

```text
CollegeERP.WebAPI.VisualStudio/
├── CollegeERP.WebAPI.sln
├── START-HERE.txt
├── VISUAL-STUDIO-SETUP.md
├── docs/
│   ├── react-api.ts
│   └── CollegeERP.postman_collection.json
└── src/
    └── CollegeERP.Api/
        ├── Controllers/       API endpoints
        ├── Services/          login, JWT, password and current-user logic
        ├── Models/            database entity classes
        ├── DTOs/              request and response classes
        ├── Data/              DbContext, database seeding and seed JSON
        ├── Middleware/        global API error handling
        ├── Extensions/        pagination helpers
        ├── Properties/        Visual Studio launch settings
        ├── Program.cs         dependency injection and API configuration
        ├── appsettings.json   database, JWT and React CORS settings
        └── CollegeERP.Api.csproj
```

## 2. Install Visual Studio

Install **Visual Studio 2022 Community**. In Visual Studio Installer, select:

- **ASP.NET and web development**
- .NET 8 SDK

## 3. Open the project

1. Extract the ZIP.
2. Open the extracted folder.
3. Double-click `CollegeERP.WebAPI.sln`.
4. Wait for NuGet packages to restore.
5. In Solution Explorer, right-click `CollegeERP.Api`.
6. Select **Set as Startup Project**.

## 4. Run the API

Press **Ctrl+F5**, or click the green `https` button.

Swagger should open automatically:

```text
https://localhost:7158/swagger
```

The HTTP address is:

```text
http://localhost:5158/swagger
```

## 5. Database

The default database is SQLite:

```json
"DatabaseProvider": "Sqlite",
"Sqlite": "Data Source=collegeerp.db"
```

You do not need to install MySQL or SQL Server. On the first run:

1. `collegeerp.db` is created automatically.
2. All tables are created automatically.
3. CollegeERP demo data is inserted automatically.

To reset the database, stop the API, delete `collegeerp.db`, and run the API again.

## 6. Test login in Swagger

Open `POST /api/auth/login`, click **Try it out**, and send:

```json
{
  "email": "admin@collegeerp.com",
  "password": "admin123"
}
```

Copy the returned token. Click **Authorize** at the top of Swagger and enter:

```text
Bearer YOUR_TOKEN
```

Then test protected endpoints such as `GET /api/students`.

## 7. Connect the React project

In the React project root, create `.env`:

```env
VITE_API_URL=http://localhost:5158/api
```

Copy this file into the React project:

```text
docs/react-api.ts  ->  your-react-project/src/api/react-api.ts
```

Restart React after changing `.env`:

```powershell
npm run dev
```

## 8. Example React login

```ts
const response = await fetch(`${import.meta.env.VITE_API_URL}/auth/login`, {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    email: "admin@collegeerp.com",
    password: "admin123"
  })
});

const data = await response.json();
localStorage.setItem("erp_jwt_token", data.token);
```

## 9. Example authenticated request

```ts
const token = localStorage.getItem("erp_jwt_token");

const response = await fetch(
  `${import.meta.env.VITE_API_URL}/students?page=1&pageSize=20`,
  {
    headers: {
      Authorization: `Bearer ${token}`
    }
  }
);

const students = await response.json();
```

## 10. Main API controllers

- Auth
- Dashboard
- Departments
- Courses
- Subjects
- Semesters
- Exams
- Students
- Faculty
- Attendance
- Marks
- Results
- Fees
- Books
- Categories
- Announcements
- Notifications
- Users
- Roles
- HOD authorizations
- Login history
- Settings

## Common Visual Studio fixes

### NuGet packages are not restored

Right-click the solution and select **Restore NuGet Packages**.

### HTTPS certificate warning

Open Developer PowerShell and run:

```powershell
dotnet dev-certs https --trust
```

### Port is already being used

Edit:

```text
src/CollegeERP.Api/Properties/launchSettings.json
```

Change ports `5158` and `7158`, then update `VITE_API_URL` in React.

### React shows a CORS error

Confirm your React URL appears under `Cors:AllowedOrigins` in `appsettings.json`. The project already allows ports `5173` and `3000`.
