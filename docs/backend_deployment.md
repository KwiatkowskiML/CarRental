# Updating the application
When making changes to the application:

1. Make code changes
2. Rebuild Docker image:
```bash
gcloud builds submit --tag gcr.io/awesomecarrental/car-rental-api
```
3. Deploy new version:
```bash
gcloud run deploy car-rental-api \
  --image gcr.io/awesomecarrental/car-rental-api \
  --platform managed \
  --region europe-west4 \
  --add-cloudsql-instances awesomecarrental:europe-west4:car-rental-pg-0 \
  --allow-unauthenticated
```

# Deploying Backend to Google Cloud Run

## 1. Database Setup

1. Go to GCP Console → SQL
2. Create PostgreSQL instance:
   - Name: "car-rental-pg-0"
   - Choose region (e.g., europe-west4)
   - Note the instance connection name: "awesomecarrental:europe-west4:car-rental-pg-0"
   - Set up database user (postgres) and password
3. Create database:
   - Name: "car_rental_db"
4. Run SQL setup scripts to create tables

## 2. Enable Required Google Cloud APIs

```bash
gcloud services enable cloudbuild.googleapis.com
gcloud services enable run.googleapis.com
gcloud services enable sqladmin.googleapis.com
```

## 3. Setup Cloud Build Permissions

```bash
# Get project number
PROJECT_NUMBER=$(gcloud projects describe $(gcloud config get-value project) --format='value(projectNumber)')

# Grant permissions
gcloud projects add-iam-policy-binding $(gcloud config get-value project) \
    --member=serviceAccount:$PROJECT_NUMBER@cloudbuild.gserviceaccount.com \
    --role=roles/storage.admin

gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member=serviceAccount:$PROJECT_NUMBER@cloudbuild.gserviceaccount.com \
    --role=roles/cloudbuild.builds.builder
```

## 4. Setup Database Access Permissions

```bash
# Replace 795304686308 with your project number
gcloud projects add-iam-policy-binding awesomecarrental \
    --member="serviceAccount:795304686308-compute@developer.gserviceaccount.com" \
    --role="roles/cloudsql.client"

gcloud projects add-iam-policy-binding awesomecarrental \
    --member="serviceAccount:service-795304686308@serverless-robot-prod.iam.gserviceaccount.com" \
    --role="roles/cloudsql.client"
```

## 5. Build and Submit Docker Image

```bash
gcloud builds submit --tag gcr.io/awesomecarrental/car-rental-api
```

## 6. Deploy to Cloud Run

```bash
gcloud run deploy car-rental-api \
  --image gcr.io/awesomecarrental/car-rental-api \
  --platform managed \
  --region europe-west4 \
  --add-cloudsql-instances awesomecarrental:europe-west4:car-rental-pg-0 \
  --allow-unauthenticated
```

## 7. Configure Environment Variables

1. Go to Cloud Run → car-rental-api → EDIT & DEPLOY NEW REVISION
2. Add these environment variables:
```
INSTANCE_CONNECTION_NAME=awesomecarrental:europe-west4:car-rental-pg-0
DB_USER=postgres
DB_PASS=[your-database-password]
DB_NAME=car_rental_db
JWT_SECRET=[your-secure-secret-key]
BASE_URL=[your-cloud-run-url]
GOOGLE_CLIENT_ID=[your-google-client-id]
GOOGLE_CLIENT_SECRET=[your-google-client-secret]
EMAIL_API_KEY=[your-sendgrid-api-key]
EMAIL_FROM_EMAIL=[your-verified-sender@domain.com]
EMAIL_FROM_NAME=Car Rental Service
```

4. Update environment variables if needed

## 8. Verification

1. Check Cloud Run service URL
3. Check service logs for any errors
4. Test API endpoints with local frontend configured in vite.config for cloud-run url