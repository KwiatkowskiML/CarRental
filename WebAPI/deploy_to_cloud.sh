gcloud services enable cloudbuild.googleapis.com
gcloud services enable run.googleapis.com
gcloud services enable sqladmin.googleapis.com

gcloud builds submit --tag gcr.io/awesomecarrental/car-rental-api

gcloud run deploy car-rental-api \
  --image gcr.io/awesomecarrental/car-rental-api \
  --platform managed \
  --region europe-west4 \
  --add-cloudsql-instances awesomecarrental:europe-west4:car-rental-db \
  --set-env-vars "ASPNETCORE_ENVIRONMENT=Production"