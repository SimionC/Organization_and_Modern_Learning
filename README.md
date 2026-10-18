# Course Notes and Collaboration

A React and ASP.NET Core application for managing courses, assignments, notes, and shared workspaces.

## Why I built this

The app brings course material and student work together with a personal note editor and a workspace for sharing notes with collaborators.

## Features

- Cookie-based sign-in and account profiles, with admin account creation and password reset flows.
- Course dashboards and course pages, including resources, assignments, submissions, and grading workflows.
- Personal notes with folders, Markdown rendering, code highlighting, and math rendering.
- Direct note sharing and collaborative workspaces with viewer/editor roles.
- A dashboard with recent courses, notes, and upcoming assignments.

## Tech stack

- Backend: ASP.NET Core Web API on .NET 8, Entity Framework Core 9, SQLite.
- Frontend: React 19, React Router, Vite 6, Bootstrap 5.
- Tests: xUnit with EF Core in-memory tests for authentication service behavior.

## Screenshots and demo

TODO: Add screenshots or a short demo GIF. No screenshots or hosted demo are included yet.

## Getting started

### Prerequisites

- .NET 8 SDK or newer.
- Node.js 20 or newer and npm (React Router 7 requires Node 20).

### Install dependencies

From the repository root:

```sh
dotnet restore App.sln
cd app.client
npm ci
```

### Configuration

The backend uses SQLite. Its default connection string is `Data Source=./db/reviewer.db`, resolved relative to the `App.Server` project directory. On first startup, the server creates the database directory, applies EF Core migrations, and inserts fake demo data if the database has no users. The ignored `App.Server/db/app.db` file is left untouched.

To use another SQLite file, set the standard ASP.NET Core environment variable before starting the server. `.env.example` documents the setting; ASP.NET Core does not load `.env` files automatically.

PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = "Data Source=./db/reviewer.db"
```

Bash:

```sh
export ConnectionStrings__DefaultConnection='Data Source=./db/reviewer.db'
```

### Run the app

Start the API from the `App.Server` directory. `--no-launch-profile` prevents the Visual Studio SPA proxy from starting a second frontend process; the separate Vite terminal below serves the client:

```sh
cd App.Server
dotnet run --no-launch-profile --urls https://localhost:7166
```

In another terminal, start the Vite development server:

```sh
cd app.client
npm run dev
```

Open the Vite URL printed in the client terminal (normally `https://localhost:7167`). The Vite proxy forwards `/api` requests to the backend at `https://localhost:7166` by default. If the local HTTPS certificate is not trusted yet, run `dotnet dev-certs https --trust`.

Demo sign-in (created only when the database has no users):

- Email: `reviewer@example.com`
- Password: `Reviewer123!`

### Build, lint, and test

From the repository root:

```sh
dotnet build App.sln
dotnet test App.sln
cd app.client
npm run lint
npm run build
```

## Project structure

```text
App.Server/       ASP.NET Core API, EF Core models, migrations, and services
App.Tests/        xUnit tests for server authentication logic
app.client/       React and Vite frontend
Data/             SQL schema/data script
db/               Database schema notes and SQL script
tools/            Python scripts for generating study-note PDFs
output/           Generated PDF output from the scripts in tools/
```

## Design decisions and challenges

- EF Core migrations create and update the SQLite schema at server startup; a small seeder supplies fake reviewer data on an empty database.
- Cookie authentication lets the browser keep the signed-in session while the client sends same-origin API requests through the Vite proxy.
- Note content is stored as Markdown and rendered with plugins for GitHub-flavored Markdown, syntax highlighting, and mathematical notation.
- Course and note access vary by user role and sharing membership, so the API applies access checks in its controllers and services.

## Future improvements

- Add API and frontend tests around authorization, course workflows, and note sharing.
- Add deployment configuration and production secret management.
- Add screenshots and a hosted demo after preparing a public-safe environment.

## License and contact

Released under the MIT License; see [LICENSE](LICENSE). Replace the placeholder copyright name in `LICENSE` before publishing.

Contact: TODO — add your name and preferred contact link.
