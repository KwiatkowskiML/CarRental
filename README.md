# Car Rental System

A full-stack application for managing car rentals, built with .NET 8 and React.

## Database
![alt text](docs/img/database_schema.png)

### Setting up the Database
1. Start the PostgreSQL database using Docker:
```bash
docker-compose up
```

2. Connect to the database (if needed):
```bash
psql -h localhost -p 5432 -U root car_rental_db
```

## Backend (.NET)

### Prerequisites
- .NET 8.0 SDK

### Running the Backend
1. Navigate to the WebAPI directory:
```bash
cd WebAPI
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5024
- HTTPS: https://localhost:7141
- Swagger UI: http://localhost:5024/swagger/

## Frontend (React)

### Prerequisites
- Node.js (v18 or later)
- npm (included with Node.js)

### Running the Frontend
1. Navigate to the Client directory:
```bash
cd Client
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The frontend will be available at http://localhost:5173

## Development

For local development, you'll need to run both the backend and frontend:

1. Start the database:
```bash
docker-compose up -d
```

2. Start the backend (in a new terminal):
```bash
cd WebAPI
dotnet run
```

3. Start the frontend (in a new terminal):
```bash
cd Client
npm run dev
```

## Project Structure
```
CarRental/
├── WebAPI/          # Backend .NET application
├── Client/          # Frontend React application
├── sql/            # Database scripts
└── docs/           # Documentation
```

## Technologies
- Backend:
  - .NET 8
  - Entity Framework Core
  - PostgreSQL
- Frontend:
  - React
  - Vite
- Infrastructure:
  - Docker
  - Docker Compose

## Contributing
1. Ensure you have all prerequisites installed
2. Fork the repository
3. Create a new branch for your feature
4. Make your changes
5. Submit a pull request