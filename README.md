# CatalogoApi

Mini API REST de catálogo (categorías y productos) en **.NET 10**, hecha para practicar arquitectura limpia y buenas prácticas.

## Tecnologías
- C# / ASP.NET Core Web API, Swagger
- SQL Server 2022 + Entity Framework Core (migraciones, seed, `Include`, `AsNoTracking`)
- CQRS con MediatR, Repository, Unit of Work, Factory, FluentValidation (pipeline behavior)
- Capas: Dominio → Aplicación → Infraestructura → Api
- Pruebas unitarias con xUnit
- Docker (multi-etapa) y docker-compose
- CI con GitHub Actions (compila, prueba y construye la imagen)

## Ejecutar con Docker
```bash
cp .env.example .env      # cambia la contraseña de SQL Server
docker compose up -d --build
```
Swagger: http://localhost:5100/swagger  
Las migraciones se aplican solas al arrancar.

## Endpoints
- `GET/POST /api/v1/categorias`
- `GET/POST /api/v1/productos`, `GET/PUT/DELETE /api/v1/productos/{id}`

## Pruebas
```bash
dotnet test
```

Autor: Juan Pablo Araujo — [@pandemonio2024](https://github.com/pandemonio2024)
