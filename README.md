# 3. Semester Eksamensprojekt - Slottet
## Formål
Projektet er udarbejdet som en prototype på et overbliks- og noteringsværktøj for en institution, der håndterer beboer med forskellige udfordringer. Formålet er at skabe overblik for institutionen, særligt ved vagtskifte. 

---
## Features
- [ ] Opret / læs / opdater / slet data
- [ ] Login / brugerroller
- [ ] MVC views
- [ ] Databaseintegration
- [ ] Authentication og authorization
- [ ] Tests
- [ ] Docker containerisation

---
## Teknologier
- **C# / .NET 10** - Programmeringssprog og framework.
- **ASP.NET Core Web API** - Backend API til at håndtere requests fra client.
- **Blazor WebAssembly** - Frontend client til user interface.
- **Entity Framework Core** - Object-relational mapper brugt til database adgang.
- **Microsoft SQL Server** - Relational database brugt til at opbevare data.
- **ASP.NET Core Identity** - User og  role management.
- **Role-based Authorization** - Access control ved brug af roller som Admin, Employee og Storskaerm.
- **Swagger / OpenAPI** - API dokumentation og test.
- **DTOs** - Brugt til at transporterer data imellem frontend og backend.
- **EF Core Migrations** - Brugt til at håndtere databaseændringer.
- **Docker / Visual Studio Container Tools** - Container support for applikationen.

---
## Projektstruktur og arkitektur
Projektet er struktureret efter Clean Architecture. Se evt. mere her: [Clean Architecture with ASP.NET Core 10](https://www.youtube.com/live/rjefnUC9Z90) 

```txt

Slottet

│

├── Slottet                      # Server-side rendering

├── Slottet.API                  # Controllers & Middleware

├── Slottet.Application          # Interfaces

├── Slottet.Client               # Client-side rendering & UI

├── Slottet.Client.Test          # Test class

├── Slottet.Domain               # Entities 

├── Slottet.Infrastructure       # EF Core, DTOServices & DBcontext

├── Slottet.Infrastructure.Test  # Test class

├── Slottet.Shared               # DTO

│

├── Dockerfile

├── docker-compose.yml

└── README.md

```

---
## Installation
### Krav
- .NET SDK 10.0
- Docker Desktop
- Visual Studio 2022 og nyere
- Git
---
## Kør projektet lokalt med Visual Studio

```bash

git clone [repository-url]

Åben projekt i Visual Studio

Kør multilaunch på Slottet og Slottet.API

```

---
## Kør med Docker

```bash

docker compose up --build

```

Applikationen kører på:

```txt

https://localhost:[5001]

https://localhost:[7201]

```

---
## Database
Connection string ligger i secrets eller Docker environment variables.
### Migrations

```bash

dotnet ef migrations add InitialCreate

dotnet ef database update

```

--- 
## Test
Udvalgte klasser er testet på baggrund af, hvor meget funktionalitet, der minder om hinanden. Vi har altså valgt eksemplariske klasser, der ligner andre klasser. Der er foretaget integrationstest og unittest. 

```bash

dotnet test .\Slottet\Slottet.Client.Test\Slottet.Client.Test.csproj
dotnet test .\Slottet\Slottet.API.Test\Slottet.API.Test.csproj
dotnet test .\Slottet\Slottet.Infrastructure.Test\Slottet.Infrastructure.Test.csproj

```

---
## Security
Der er i projektet implementeret login med ASP.Net Identity, hvor der er rollebaseret authentication. Der er brugt SHA512 til password hashing med 210.000 iterationer for sikkerhed. Projektet er derudover sammenlignet med OWASP top 10 sikkerhedsbrister og der er lavet Sonarqube analyser med jævne mellemrum. 

---
## Roadmap

- [ ] Deployment
- [ ] CI/CD pipeline

---
## Læringsmål
Projektet er lavet med følgende læringsmål for øje: 

- ASP.NET Core MVC
- Clean Architecture
- SQL Server
- Entity Framework Core
- Docker
- Lagdeling
- SOLID
- Testbarhed

---
