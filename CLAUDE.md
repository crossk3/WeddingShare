# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WeddingShare is an ASP.NET Core 9.0 (C#) web application for sharing wedding photo/video galleries with guests. Guests access galleries via URLs or QR codes and can upload photos. Features include admin gallery management, image/video processing, multi-language support (70+ languages), dark mode, 2FA, and notification integrations (SMTP, Ntfy, Gotify).

- **Docs**: https://docs.wedding-share.org
- **Docker Hub**: https://hub.docker.com/r/cirx08/wedding_share

## Build & Test Commands

```bash
dotnet restore                          # Restore NuGet packages
dotnet build                            # Build entire solution
dotnet test                             # Run all unit tests (NUnit)
dotnet test --filter "FullyQualifiedName~ClassName.MethodName"  # Run single test
dotnet run --project WeddingShare       # Run the app locally (port 5000)
```

The CI pipeline (`.gitlab-ci.yml`) runs `dotnet restore && dotnet test`, then builds multi-platform Docker images (arm/v7, arm64/v8, amd64).

## Solution Structure

Two projects in `WeddingShare.sln`:

- **WeddingShare/** — Main web application (NET 9.0)
- **WeddingShare.UnitTests/** — NUnit tests with NSubstitute for mocking

## Architecture

### Entry Point & Configuration
- `Program.cs` — Kestrel host on port 5000
- `Startup.cs` — DI registration, middleware pipeline, authentication, background services
- `Configurations/` — Service registration split into: `DependencyInjectionConfiguration`, `DatabaseConfiguration`, `WebClientConfiguration`, `LocalizationConfiguration`, `NotificationConfiguration`

### Configuration System
`ConfigHelper` resolves settings with a priority chain: **environment variables > appsettings.json > database settings** (via `SettingsHelper`). Environment variable keys are derived from the appsettings path by dropping the first segment, joining with underscores, and uppercasing (e.g., `Settings:Gallery:Columns` → `GALLERY_COLUMNS`). Per-gallery overrides are supported by appending `_<galleryId>` to the env var name.

### Database
- SQLite (default) or MySQL, configured via `Settings:Database:Type`
- Migrations handled by **DbUp** running SQL scripts from `SqlScripts/SQLite/` or `SqlScripts/MySQL/`
- Entity models in `Models/Database/`: `UserModel`, `GalleryModel`, `GalleryItemModel`, `GalleryItemLikeModel`, `SettingModel`, `AuditLogModel`, `CustomResourceModel`
- `DatabaseHelper` provides the data access abstraction (not EF Core — uses raw SQL via DbUp + manual queries)

### Controllers
- `BaseController` — Shared logic inherited by all controllers
- `HomeController` — Homepage with gallery selector
- `GalleryController` — Core gallery CRUD, upload, review, display modes (grid/full-width/presentation/slideshow)
- `AccountController` — Auth, registration, 2FA, account management
- `MediaViewerController` — Media viewing and download
- `LanguageController` — Language switching
- `SponsorsController`, `ErrorController`

### Background Services (Hosted Services)
- `DirectoryScanner` — Scans for new media files on a cron schedule (default: every 30 min)
- `NotificationReport` — Sends daily email summaries
- `CleanupService` — Cleans temporary files (default: 4 AM daily)

Schedules are cron-based via NCrontab, configured in `appsettings.json` under `BackgroundServices`.

### Helpers (Business Logic)
Key helpers in `Helpers/`: `ImageHelper` (SixLabors.ImageSharp + Xabe.FFmpeg for video), `FileHelper`, `EncryptionHelper`, `PasswordHelper`, `UrlHelper`, `GalleryHelper`, `DeviceDetector`. Notification helpers in `Helpers/Notifications/`.

### Frontend
Razor views in `Views/` with layouts in `Views/Shared/`. Static assets in `wwwroot/` (CSS, JS, third-party libs like jQuery). Localization via `.resx` resource files in `Lang/`.

## Docker

The Dockerfile uses a multi-stage build (sdk:9.0 for build, aspnet:9.0 for runtime). The app runs on port 5000. Database and uploaded files persist in `/app/config/` and `/app/wwwroot/uploads/` respectively.

## Key Conventions

- All settings are configurable via environment variables or `appsettings.json` (see Configuration System above)
- Not all image formats work in browsers (notably Apple .heic); allowed file types are configurable
- The app supports per-gallery configuration overrides
- Database migrations are forward-only SQL scripts, numbered sequentially in `SqlScripts/`
