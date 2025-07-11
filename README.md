# SnacksPOS

SnacksPOS is a minimal mobile-first point of sale system built with ASP.NET Core. It's designed for small teams and cafeterias to track snack purchases and payments.

## Features

- **Mobile-first design** with dark/light mode support
- **User authentication** with demo accounts
- **Product catalog** management
- **Shopping cart** functionality
- **Balance tracking** and payment system
- **Admin dashboard** for user management
- **PWA support** for offline functionality
- **Responsive design** with Tailwind CSS

## Requirements

- .NET 8 SDK
- SQLite (included)

## Quick Start

1. Clone the repository
2. Navigate to the project directory
3. Run the application:

```bash
dotnet run --project SnacksPOS.Web
```

4. Open your browser to `http://localhost:5173`
5. Sign in with demo credentials:
   - **Admin**: `admin` / `password`
   - **User**: `demo` / `password`

## Usage

### For Users
1. **Browse Products**: Navigate to the Snacks page to see available items
2. **Add to Cart**: Click on any product to add it to your cart
3. **Checkout**: Go to Cart page and click Checkout to finalize your order
4. **Check Balance**: Visit Account page to see your current balance and payment status

### For Admins
1. Sign in with admin credentials
2. Access the Admin page for user and product management
3. Monitor user balances and payment status

## Architecture

The application follows Clean Architecture principles:

- **SnacksPOS.Domain**: Core entities and business rules
- **SnacksPOS.Application**: Application services and MediatR handlers
- **SnacksPOS.Infrastructure**: Data access, Identity, and external services
- **SnacksPOS.Web**: Razor Pages UI and API endpoints

## Technology Stack

- **ASP.NET Core 8**: Web framework
- **Entity Framework Core**: ORM with SQLite
- **ASP.NET Core Identity**: Authentication and authorization
- **MediatR**: CQRS pattern implementation
- **Tailwind CSS**: Utility-first CSS framework
- **Razor Pages**: Server-side rendered UI
- **PWA**: Progressive Web App capabilities

## API Endpoints

- `POST /api/cart/checkout`: Process cart checkout
- `GET /api/user/balance`: Get user balance information

## Database

The application uses SQLite with Entity Framework Core migrations. The database is automatically created and seeded with demo data on first run.

## Development

### Adding Migrations
```bash
dotnet ef migrations add [MigrationName] --project SnacksPOS.Infrastructure --startup-project SnacksPOS.Web
```

### Running Tests
```bash
dotnet test
```

### Building for Production
```bash
dotnet publish --configuration Release
```

## Configuration

Key configuration options in `appsettings.json`:

- **ConnectionStrings:Default**: SQLite connection string
- **CORS settings**: Configured for `cafeteria.internal` domain
- **Identity options**: Password and lockout policies

## Security Features

- Cookie-based authentication
- CSRF protection
- Role-based authorization
- Secure cookie policies
- Input validation

## Extensibility

The application is designed to be easily extended:

- Add new MediatR commands/queries in `SnacksPOS.Application`
- Extend the domain models in `SnacksPOS.Domain`
- Add new data services in `SnacksPOS.Infrastructure`
- Create new pages in `SnacksPOS.Web/Pages`

## License

This project is provided as-is for educational and demonstration purposes.
