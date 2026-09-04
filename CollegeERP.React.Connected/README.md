# CollegeERP React — Connected to ASP.NET Core

This project is the React conversion of the supplied single-file CollegeERP HTML/JavaScript application. Browser localStorage data has been replaced with calls to the supplied ASP.NET Core Web API.

## Requirements
- Node.js 20+
- ASP.NET Core backend running on `http://localhost:5158`

## Run
1. Open this frontend folder in VS Code.
2. Run `npm install`.
3. Confirm `.env` contains `VITE_API_URL=http://localhost:5158/api`.
4. Start backend in Visual Studio using the **http** profile.
5. Run `npm run dev`.
6. Open `http://localhost:5173`.

## Demo login
- Admin: `admin@collegeerp.com` / `admin123`
- Principal: `principal@college.edu` / `principal123`
- HOD: `ramesh.k@college.edu` / `hod123`
- Faculty: `amit.v@college.edu` / `faculty123`
- Student: `aarav.m@college.edu` / `student123`

## Architecture
- `src/api/client.js`: the only backend HTTP client
- `src/config/modules.js`: fields, tables, role pages, API resource configuration
- `src/pages/GenericCrudPage.jsx`: reusable CRUD page for most modules
- `src/context/AuthContext.jsx`: JWT authentication
- `src/context/ToastContext.jsx`: alerts

No CDN Font Awesome or Google Fonts are used, so tracking prevention warnings are avoided.
