# Aibolit API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4.svg?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![C#](https://img.shields.io/badge/C%23-12-239120.svg?logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791.svg?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![EF Core](https://img.shields.io/badge/ORM-Entity%20Framework%20Core-512BD4.svg?logo=entityframework&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![Keycloak](https://img.shields.io/badge/Auth-Keycloak-4D4D4D.svg?logo=keycloak&logoColor=white)](https://www.keycloak.org/)
[![Docker](https://img.shields.io/badge/Container-Docker-2496ED.svg?logo=docker&logoColor=white)](https://www.docker.com/)

A layered ASP.NET Core 8.0 RESTful API designed to manage hospital operations, patient records, and medical staff scheduling. The system integrates Keycloak for centralized identity management, background services for automated notifications, and external AI/API providers for health-related information search.

> **Medical Disclaimer:** The "Disease Search" feature provides informational responses based on external databases or AI models. It is not a diagnostic tool and does not replace professional medical consultation.

---

## Table of Contents

- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Role-Based Features & Business Logic](#role-based-features--business-logic)
- [Background Services](#background-services)
- [Email Notifications](#email-notifications)
- [API Endpoints Overview](#api-endpoints-overview)
- [Authentication & Testing API](#authentication--testing-api)
- [Setup & Deployment](#setup--deployment)
- [Docker Image (Experimental)](#docker-image-experimental)
- [Known Limitations & Design Decisions](#known-limitations--design-decisions)
- [Troubleshooting](#troubleshooting)

---

## Technology Stack

*   **Framework:** ASP.NET Core 8.0 (Controller-based API)
*   **Database & ORM:** PostgreSQL, Entity Framework Core 8.0 (with `UseLazyLoadingProxies`)
*   **Identity & Security:** Keycloak (OAuth 2.0 / OpenID Connect). JWT validation via `JwtBearer` with custom `KeycloakMiddleware` for on-the-fly user synchronization.
*   **External Integrations:** 
    *   Google Cloud Storage V1 (Profile picture uploads via local file path)
    *   Google Cloud Translation V2 (Data localization)
    *   NLM Clinical Tables Search API & Google Gemini (Symptom information lookup)
*   **Background Processing:** `IHostedService` for appointment reminders and success status evaluation.
*   **Utilities:** AutoMapper, MailKit + PreMailer.Net (HTML email rendering), Serilog (File logging).

---

## Project Structure

```text
AibolitAPI/
├── AibolitAPI/                # Main application project
│   ├── Attributes/            # Custom validation/authorization attributes
│   ├── Auth/                  # Keycloak token parsing and SSO synchronization
│   ├── Controllers/           # API endpoints (13 controllers)
│   ├── Data/                  # EF Core DbContext
│   ├── DTOs/                  # Data Transfer Objects
│   ├── EmailManager/          # MailKit + HTML templates
│   ├── Interfaces/            # Service and repository contracts
│   ├── Mappers/               # AutoMapper profiles
│   ├── Middleware/            # KeycloakMiddleware (token processing)
│   ├── Migrations/            # EF Core migrations
│   ├── Models/                # EF Core entities
│   ├── Repositories/          # Data access layer
│   ├── SearchProviders/       # Strategy pattern (Gemini vs External API)
│   ├── Services/              # Business logic + BackgroundServices
│   ├── realms/                # Keycloak realm export (aibolit-api-realm.json)
│   ├── appsettings.json       # Base configuration
│   └── Program.cs             # DI, middleware pipeline, Swagger
├── AibolitAPI.sln             # Solution file
└── docker-compose.yml         # PostgreSQL + Keycloak (start-dev mode)
```

---

## Role-Based Features & Business Logic

### Anonymous User
*   **Self-Registration:** Users can register via Keycloak account console. New users receive only the `user` role; API-specific roles must be assigned by an administrator.
*   **Disease Search:** Public access to symptom information (`provider=openai` → Gemini, `provider=external` → NLM API).

### Authenticated User (General)
*   **Profile Management:** Update contact info. Profile photos are processed from a local file path and uploaded to Google Cloud Storage.
*   **SSO Synchronization:** `KeycloakMiddleware` extracts claims (`sub`, `email`, `name`, `birthdate`) from JWT and auto-creates/updates local `User` record on first API request.

### Patient
*   **Doctor Discovery:** Search by specialization, view available time slots, and manage a "Favorites" list.
*   **Appointment Booking:** Book appointments (triggers primary doctor assignment if applicable). Relationship mapping links doctor-patient immediately, even if later canceled.
*   **Medical Records:** Read-only access to personal history, prescriptions, and recommendations.

### Doctor
*   **Patient Management:** View assigned/treated patients and their data.
*   **Medical Record Ownership:** Can create/edit records. **Constraint:** Only the doctor who created a record can modify/delete it.
*   **Schedule:** View upcoming appointments, manage `WorkSchedule` slots, and cancel appointments (triggers patient notification).

### Chief Doctor
*   **Hospital Oversight:** Bound to single `HospitalId`. Views all doctors/patients in the hospital.
*   **Schedule Admin:** Create/assign `WorkSchedule` templates to hospital doctors.

### System Administrator
*   **User Lifecycle:** Managed via Keycloak Admin Console.
*   **Doctor/Admin Onboarding:** Create user in Keycloak → call `POST /api/Doctor` or `POST /api/Administrator` with `keycloakId` as a query parameter → API provisions local entity with default schedule.
*   **Soft-Deletion:** Entities are never hard-deleted. `IsActive = false` preserves historical data.

---

## Background Services

Two `IHostedService` implementations run continuously:

### 1. AppointmentReminderService (every 30s)
*   **Appointment Reminders:** Sends email 1 minute before appointment (calculated in `Europe/Kiev` timezone).
*   **Weekly Broadcasts:** Monday 12:00 → "Friendly Reminder"; Friday 09:00 → "Disease Search Info".

### 2. AppointmentSuccessCheckService (every 1 min)
*   **Success Logic:** If `DateTime.UtcNow >= AppointmentDate.AddMinutes(5)` AND `IsScheduled=true` AND `IsActive=true`:
    *   Sets `IsScheduled = false`
    *   Increments `Doctor.VisitCount++`
*   **⚠️ Important:** Currently set to 5 minutes (test value, intended: 20 min). Uncanceled no-shows will increment `VisitCount`.

---

## Email Notifications

| Template | Trigger |
| :--- | :--- |
| `appointment_reminder` | 1 minute before scheduled appointment |
| `appointment_cancellation` | When patient or doctor cancels appointment |
| `new_prescription` | Doctor creates new prescription |
| `new_recommendation` | Doctor creates new recommendation |
| `registration` | First login via Keycloak (SSO sync) |
| `friendly_reminder` | Weekly broadcast (Monday 12:00) |
| `disease_search_info` | Weekly broadcast (Friday 09:00) |

---

## API Endpoints Overview

All protected endpoints require a valid `Bearer` token. Swagger available at `/swagger` in Development mode. **Note: There is no `/v1` prefix in the routes.**

### 🔍 Search & Profile
| Method | Endpoint | Role | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/DiseaseSearch/search?term={q}&provider={openai\|external}` | Anonymous/Auth | `openai` → Google Gemini, `external` → NLM Clinical Tables. Invalid provider → 400 Bad Request. |
| `GET` | `/api/User/{id}` | Auth | Fetches profile + enriches with Keycloak SSO attributes. |
| `PUT` | `/api/User/{id}` | Owner | Updates profile data. |

### 👦 Patient Operations
| Method | Endpoint | Role | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Patient/doctors` | Patient | List doctors (filter by specialization). |
| `GET` | `/api/Patient/doctors/{doctorId}/slots?date={date}` | Patient | Get available appointment slots for a specific doctor. |
| `POST` | `/api/Patient/{doctorId}/appointments?patientId={id}&appointmentDate={date}` | Patient | Book appointment. |
| `DELETE`| `/api/Patient/{patientId}/appointments/{appointmentId}/cancel` | Patient | Cancel appointment + notify doctor. |
| `GET` | `/api/Patient/{patientId}/appointments` | Patient | Full history (scheduled/completed/canceled). |
| `POST` | `/api/Patient/{patientId}/favorite/{doctorId}` | Patient | Add doctor to favorites. |
| `DELETE`| `/api/Patient/{patientId}/favorites/{doctorId}` | Patient | Remove doctor from favorites. |
| `GET` | `/api/Patient/{patientId}/prescriptions` | Patient | List medications. |
| `GET` | `/api/Patient/{patientId}/recommendations` | Patient | List recommendations. |

### 🩺 Doctor & Medical Record Operations
| Method | Endpoint | Role | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Doctor/{doctorId}/patients` | Doctor | List assigned/treated patients. |
| `GET` | `/api/Doctor/{doctorId}/patients/{patientId}/prescriptions` | Doctor | List prescriptions for a specific patient. |
| `POST` | `/api/Doctor/{doctorId}/patients/{patientId}/prescriptions` | Doctor | Create new prescription. |
| `GET` | `/api/Doctor/{doctorId}/patients/{patientId}/recommendations` | Doctor | List recommendations for a specific patient. |
| `POST` | `/api/Doctor/{doctorId}/patients/{patientId}/recommendations` | Doctor | Create new recommendation. |
| `GET` | `/api/Doctor/{doctorId}/appointments` | Doctor | Upcoming scheduled appointments (`IsScheduled=true`). |
| `DELETE`| `/api/Doctor/{doctorId}/appointments/{appointmentId}/cancel` | Doctor | Cancel appointment + notify patient. |
| `GET` | `/api/Doctor/{doctorId}/work-schedules` | Doctor | Get doctor's work schedules. |
| `PATCH` | `/api/Doctor/{id}/work-schedules` | Doctor | Update doctor's work schedules (Body: `List<WorkScheduleDTO>`). |
| `POST` | `/api/Doctor?keycloakId={id}` | Admin | Onboard doctor (Body: `DoctorDTO`, photo processed from local path). |
| `DELETE`| `/api/Doctor/{id}` | Admin | Soft-delete doctor (`IsActive=false`). |
| `GET` | `/api/MedicalRecord` | Auth | Paginated list of medical records. |
| `POST` | `/api/MedicalRecord` | Doctor | Create medical record. |
| `PUT` | `/api/MedicalRecord/{id}` | Doctor (Creator) | Update record. Ownership validation enforced. |
| `DELETE`| `/api/MedicalRecord/{id}` | Doctor (Creator) | Soft-delete record (`IsActive=false`). |

### 👨‍⚕️ Chief Doctor & Hospital Management
| Method | Endpoint | Role | Description |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Statistics?id={hospitalId}` | Chief Doctor | Hospital analytics (auto-resolved from Chief Doctor's profile). |
| `GET` | `/api/Hospital/{hospitalId}/doctors` | Chief Doctor | List hospital doctors. |
| `GET` | `/api/Hospital/{hospitalId}/doctors/{doctorId}` | Chief Doctor | Doctor details + nested patient list. |
| `GET` | `/api/Hospital/{hospitalId}/doctors/{doctorId}/patients/{patientId}` | Chief Doctor | Full patient dossier. |
| `GET` | `/api/Hospital/{hospitalId}/doctors/{doctorId}/patients/{patientId}/prescriptions` | Chief Doctor | Patient's prescriptions within hospital context. |
| `GET` | `/api/Hospital/{hospitalId}/doctors/{doctorId}/patients/{patientId}/recommendations`| Chief Doctor | Patient's recommendations within hospital context. |
| `CRUD` | `/api/WorkSchedule` | Chief Doctor | Manage global schedule templates (GET, POST, PUT, DELETE). |

### 🛠️ System Administrator
| Method | Endpoint | Role | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Administrator?keycloakId={id}` | Admin | Onboard administrator (Body: `AdministratorDTO`). |
| `DELETE`| `/api/Administrator/{id}` | Admin | Soft-delete administrator (`IsActive=false`). |
| `POST` | `/api/Notification/send-email?recipient={email}&type={type}` | System | Trigger email notification (Body: JSON model string). |

> **Note:** Standard CRUD endpoints (`GET`, `POST`, `PUT`, `DELETE`) also exist for `User`, `Patient`, `Appointment`, `Prescription`, and `Recommendation` entities. Refer to Swagger UI (`/swagger`) for their complete parameter definitions.

---

## Authentication & Testing API

The imported realm `aibolit-api` contains **no pre-created users**. Create a test user first, then request a token.

### Roles
Roles are client roles of the Keycloak client `aibolit-api` (emitted into the token as `resource_access.aibolit-api.roles`):
| Keycloak role | API role |
| :--- | :--- |
| `Patient` | Patient |
| `Doctor` | Doctor |
| `ChiefDoctor` | Chief Doctor |
| `Administrator` | System Administrator |
| `user` | Default role assigned to every new user |

> The Keycloak console login `admin` / `admin` belongs to the `master` realm. It is **not** an API administrator: create a separate user in realm `aibolit-api` and assign the `Administrator` role.

### Create a test user
1. Open http://localhost:8081/admin, sign in as `admin` / `admin` and switch to realm **aibolit-api**.
2. **Users → Add user**: fill in username, **email, first name and last name** (otherwise token request fails with `Account is not fully set up`), then **Create**.
3. **Credentials → Set password** (turn off *Temporary*).
4. **Role mapping → Assign role → Filter by clients → `aibolit-api`**, select a role, e.g. `Patient`.

### Obtain Token

**Windows PowerShell:**
```powershell
$response = Invoke-RestMethod -Method POST -Uri "http://localhost:8081/realms/aibolit-api/protocol/openid-connect/token" -Body @{client_id="aibolit-api"; username="YOUR_USERNAME"; password="YOUR_PASSWORD"; grant_type="password"} -ContentType "application/x-www-form-urlencoded"
$token = $response.access_token
```

**Linux/macOS (Bash):**
```bash
TOKEN=$(curl -X POST "http://localhost:8081/realms/aibolit-api/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=aibolit-api" \
  -d "username=YOUR_USERNAME" \
  -d "password=YOUR_PASSWORD" \
  -d "grant_type=password" | jq -r '.access_token')
```

**Use token:** Add header `Authorization: Bearer <token>` (or `$TOKEN` in bash) to all requests.

> **Important:** Swagger UI has **no Authorize button** (no security definition is configured), so protected endpoints cannot be called from it. Use curl, Postman, or PowerShell:
> ```powershell
> Invoke-RestMethod -Uri "http://localhost:5000/api/Doctor" -Headers @{ Authorization = "Bearer $token" }
> ```
> *(Replace `5000` with your actual local port if different).*

**Token lifetime:** 300 seconds (5 minutes), `ClockSkew=0`.

---

## Setup & Deployment

### Prerequisites
*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 1. Clone & Configure
```bash
git clone https://github.com/Tokar08/AibolitAPI.git
cd AibolitAPI
```

Create `AibolitAPI/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AibolitDB;Username=postgres;Password=postgres"
  },
  "ApiSettings": {
    "GeminiKey": "YOUR_GEMINI_API_KEY",
    "ExternalApiUrl": "https://clinicaltables.nlm.nih.gov/api/conditions/v3/search?sf=info_link_data&df=info_link_data&terms="
  },
  "EmailSettings": {
    "SenderEmail": "your_email@gmail.com",
    "SenderPassword": "YOUR_APP_PASSWORD"
  },
  "GoogleCloud": {
    "BucketName": "aibolit-bucket"
  }
}
```
**Note:** Google Cloud credentials expected via `GOOGLE_APPLICATION_CREDENTIALS` env var.

### 2. Start Infrastructure (CRITICAL STEP)
The realm file is located in `AibolitAPI/realms/`, but `docker-compose.yml` expects it in `./realms` at the repo root.

```bash
# From repository root
cp -r AibolitAPI/realms ./realms        # PowerShell: Copy-Item -Recurse AibolitAPI\realms .\realms
docker-compose up -d
```
*Wait 30-60s for Keycloak to import realm. Realm imports only on first start; for re-import: `docker-compose down -v && docker-compose up -d`.*

### 3. Apply Migrations & Run
```bash
cd AibolitAPI
dotnet tool update --global dotnet-ef --version 8.*
dotnet ef database update
dotnet run
```
**Swagger URL:** Check console output (`Now listening on: http://localhost:XXXX`) or `Properties/launchSettings.json`.

---

## Docker Image (Experimental)

```bash
docker build -f AibolitAPI/Dockerfile -t aibolit-api .
```

**⚠️ Critical Limitation:** `Authority` and `ValidIssuer` are hardcoded in `Program.cs` to `http://localhost:8081/realms/aibolit-api`. Inside a container, `localhost` refers to the container itself, not the host. The containerized API **cannot validate Keycloak tokens** unless:
1. Running with `--network host` (Linux only), OR
2. Connect the API container to the compose network and update `Program.cs` to use Keycloak's container network alias.

Configuration values CAN be overridden via environment variables (e.g., `ConnectionStrings__DefaultConnection`), but NOT the Keycloak URLs. Swagger is disabled outside Development mode.

---

## Known Limitations & Design Decisions

1. **Appointment Success Logic:** Based purely on time (5 min post-start, intended: 20 min). Uncanceled no-shows increment `VisitCount`.
2. **Deactivated User Access:** Soft-deleted doctors (`IsActive=false`) disappear from API lists, but their Keycloak account stays active. Disable the user in the Admin Console. Already issued tokens stay valid for up to 5 minutes.
3. **Sensitive Data Logging:** `EnableSensitiveDataLogging()` is enabled globally. PII/PHI may be written to `test-sql-log-<date>.txt`. Wrap in `if (builder.Environment.IsDevelopment())` for production.
4. **Keycloak URL Hardcoded:** `Authority`/`ValidIssuer` in `Program.cs` cannot be changed via config or environment variables.
5. **Timezone Hardcoded:** `Europe/Kiev` (Kyiv) is hardcoded in `AppointmentReminderService`.
6. **Potential Duplicate Emails:** 30-second polling interval combined with minute-level precision checks may send duplicate emails.
7. **No Unsubscribe Mechanism:** Weekly broadcasts to all patients have no opt-out.
8. **User/Doctor Role Dependency:** If a doctor logs in via Keycloak before `POST /api/Doctor` is called by admin, `KeycloakMiddleware` creates a generic `User` (not `Doctor`).
9. **Data Access on Booking:** Doctor gains access to patient data immediately upon booking, even if appointment is later canceled.
10. **No Automated Tests:** Project has no unit/integration tests.
11. **Seed Data Required:** Hospitals and specializations must be seeded manually or via initial migration.
12. **Anonymous Disease Search:** Public endpoint may incur Google Gemini costs without rate limiting.
13. **Dev-only Keycloak realm:** the client `aibolit-api` is public with wildcard redirect URIs, brute-force protection is off, no SMTP is configured. Not suitable for production.
14. **Keycloak version drift:** the realm was exported from Keycloak 25.0.4, while `docker-compose.yml` uses `:latest`. If import fails, pin the image to `25.0.4`.

---

## Troubleshooting

| Issue | Solution |
| :--- | :--- |
| `404 Realm does not exist` | Realm not imported. Check that `./realms` folder exists at repo root. Run `docker-compose down -v && docker-compose up -d`. |
| `401 Unauthorized` after 5 minutes | Token expired (`accessTokenLifespan=300`). Request a new token. |
| `401` with valid token | Make sure you requested token from `http://localhost:8081` (not `127.0.0.1`), otherwise issuer won't match `Program.cs`. |
| `Account is not fully set up` | User missing email, first name, or last name. Update user profile in Keycloak Admin Console. |
| `403 Forbidden` | User doesn't have required client role `aibolit-api`. Check role assignment in Keycloak. |
| `500` on DiseaseSearch | Invalid `provider` parameter. Use only `openai` or `external`. |
| Ports 5432 or 8081 in use | Stop conflicting services or change ports in `docker-compose.yml`. |
| Swagger not available | Swagger only enabled in Development mode. Run with `dotnet run` (not Docker). |

---
