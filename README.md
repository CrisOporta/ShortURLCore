# ShortURLCore

ShortURLCore es una aplicación web desarrollada en ASP.NET Core sobre .NET 9.0 para la gestión y redirección de URLs cortas. Permite a los usuarios acortar enlaces largos y redirigirlos de manera eficiente, ideal para compartir en redes sociales, campañas de marketing y análisis de tráfico.

## Características principales
- Acortamiento de URLs personalizado.
- Redirección rápida y segura.
- Registro y seguimiento de clics.
- Administración de enlaces por áreas.
- Configuración flexible mediante archivos `appsettings.json`.

## Estructura del proyecto

- **ShortURLCore.Web/**: Proyecto principal web (ASP.NET Core MVC), contiene controladores, vistas, servicios y middleware.
- **ShortURLCore.Infrastructure/**: Lógica de acceso a datos, migraciones y repositorios (Entity Framework Core).
- **ShortURLCore.Models/**: Modelos de datos y configuración.
- **ShortURLCore.Utils/**: Utilidades y entidades base.

## Tecnologías utilizadas
- .NET 9.0
- ASP.NET Core MVC
- Entity Framework Core
- PostgreSQL
- Docker (opcional)

## Instalación y ejecución
1. Clona el repositorio:
	```powershell
	git clone https://github.com/CrisOporta/ShortURLCore.git
	```
2. Restaura los paquetes NuGet:
	```powershell
	dotnet restore
	```
3. Aplica las migraciones de la base de datos:
	```powershell
	dotnet ef database update --project ShortURLCore.Infrastructure
	```
4. Ejecuta la aplicación:
	```powershell
	dotnet run --project ShortURLCore.Web
	```

## Uso
Accede a la aplicación en `https://localhost:7134`, `http://localhost:5237` (o el puerto configurado). Utiliza la interfaz para acortar URLs y gestionar tus enlaces.

## Docker
Para ejecutar con Docker:
```powershell
docker build -t shorturlcore .
docker run --rm -d -p 8080:8080 --name shorturlcore
```

## Licencia
Este proyecto está bajo la licencia MIT. Consulta el archivo `LICENSE.txt` para más detalles.

## Autor
Desarrollado por Cristian Oporta.