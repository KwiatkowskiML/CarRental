# Car Rental System

A full-stack application for managing car rentals, built with .NET 8 and React.

## Quick Setup
### Build the database
```sh
chmod +x sql/initdb/init_database.sh
docker-compose up
```

### Running the Backend
1. Copy and fill the WebAPI/Properties/launchSettings.example.json to WebAPI/Properties/launchSettings.json
2. Run the application:
```bash
dotnet run
```
### Running the Frontend
1. Create the secret .env (as in template)

Install dependencies:
```bash
npm install
```

Start the development server:
```bash
npm run dev
```

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

1. Set up the WebAPI/Properties/launchSettings.json

### Frontend Configuration
TODO: move detailed instructions to another document
1. Create the environment file:
```bash
cd Client
cp .env.example .env
```

2. Update the .env file with your Google Client ID:
```plaintext
VITE_GOOGLE_CLIENT_ID=your-client-id-from-google-console.apps.googleusercontent.com
```

3. Make sure the target in vite.config.js is set to your desired api address.


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

### Backend Configuration
1. Copy and fill the WebAPI/Properties/launchSettings.example.json to WebAPI/Properties/launchSettings.json
2. Run the application:
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
cp .env.template .env
```

Install dependencies:
```bash
npm install
```

Start the development server:
```bash
npm run dev
```
The frontend will be available at http://localhost:5173

## Development

For local development, you'll need to run both the backend and frontend:

Start the database:
```bash
docker-compose up -d
```
Start the backend (in a new terminal):
```bash
cd WebAPI
dotnet run
```
Start the frontend (in a new terminal):
```bash
cd Client
npm run dev
```

## Project Structure
TODO


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