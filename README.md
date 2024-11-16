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

## Configuration Setup

### Backend Configuration

1. Create the development configuration file:
```bash
cd WebAPI
cp appsettings.template.json appsettings.Development.json
```

2. Create the production configuration file:
```bash
cp appsettings.template.json appsettings.json
```

3. Update both files with your actual values:
   - Database connection string
   - Google OAuth credentials (from Google Cloud Console - you can find it there)
   - JWT configuration:
     ```json
     {
       "GoogleAuth": {
         "ClientId": "your-client-id-from-google-console.apps.googleusercontent.com",
         "ClientSecret": "your-client-secret"
       },
       "Jwt": {
         "Secret": "your-secure-jwt-secret-at-least-32-characters",
         "Issuer": "http://localhost:5024",
         "Audience": "http://localhost:5173",
         "ExpiryMinutes": 60
       }
     }
     ```

### Frontend Configuration

1. Create the environment file:
```bash
cd Client
cp .env.template .env
```

2. Update the .env file with your Google Client ID:
```plaintext
VITE_GOOGLE_CLIENT_ID=your-client-id-from-google-console.apps.googleusercontent.com
```

> ⚠️ IMPORTANT: Never commit `appsettings.json`, `appsettings.Development.json`, or `.env` files to version control as they contain sensitive information!

### Getting Google OAuth Credentials - this is running on our GCP.

1. Go to [Google Cloud Console](https://console.cloud.google.com)
2. Create a new project or select an existing one
3. Enable the OAuth 2.0 API:
   - Go to "APIs & Services" > "Library"
   - Search for "Google OAuth 2.0"
   - Enable it
4. Create OAuth credentials:
   - Go to "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "OAuth client ID"
   - Select "Web application"
   - Add authorized origins:
     ```
     http://localhost:5173
     ```
   - Add authorized redirect URIs:
     ```
     http://localhost:5173
     http://localhost:5173/login
     ```

### Generating JWT Secret
Generate a secure JWT secret using one of these methods:

```bash
# Using Node.js
node -e "console.log(require('crypto').randomBytes(32).toString('base64'));"
```

## Backend (.NET)

[Rest of the existing Backend section...]

## Frontend (React)

[Rest of the existing Frontend section...]

## Development

[Rest of the existing Development section...]

## Project Structure
```
CarRental/
├── WebAPI/          # Backend .NET application
│   ├── appsettings.template.json  # Template for settings
│   ├── appsettings.json          # Production settings (gitignored)
│   └── appsettings.Development.json  # Development settings (gitignored)
├── Client/          # Frontend React application
│   ├── .env.template  # Template for environment variables
│   └── .env          # Environment variables (gitignored)
├── sql/            # Database scripts
└── docs/           # Documentation
```

## Technologies

[Rest of the existing Technologies section...]