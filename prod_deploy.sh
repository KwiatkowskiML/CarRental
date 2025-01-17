#!/bin/bash

# Exit on any error
set -e

# Production Configuration
PROJECT_ID="awesomecarrental"
REGION="europe-west4"
BACKEND_SERVICE="car-rental-api-prod"
BACKEND_IMAGE="gcr.io/$PROJECT_ID/$BACKEND_SERVICE"
DB_INSTANCE="car-rental-pg-prod-0"
DB_INSTANCE_FULL="awesomecarrental:europe-west4:car-rental-pg-prod-0"
PROD_TAG=$(date +"%Y%m%d-%H%M%S")

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Helper function for logging
log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR] $1${NC}"
    exit 1
}

warn() {
    echo -e "${YELLOW}[WARNING] $1${NC}"
}

# Check if running on main branch
check_branch() {
    log "Checking current branch..."
    
    CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
    if [ "$CURRENT_BRANCH" != "main" ]; then
        error "Production deployment must be done from main branch. Current branch: $CURRENT_BRANCH"
    fi
    
    # Check if branch is up to date
    git fetch origin
    LOCAL=$(git rev-parse @)
    REMOTE=$(git rev-parse @{u})
    
    if [ $LOCAL != $REMOTE ]; then
        error "Local branch is not up to date with remote. Please pull latest changes."
    fi
}

# Check if required tools are installed
check_requirements() {
    log "Checking requirements..."
    
    if ! command -v gcloud >/dev/null 2>&1; then
        error "gcloud CLI is not installed. Please install it first."
    fi

    if ! command -v firebase >/dev/null 2>&1; then
        error "Firebase CLI is not installed. Please install it first."
    fi

    if ! command -v dotnet >/dev/null 2>&1; then
        error "dotnet SDK is not installed. Please install it first."
    fi

    if ! command -v npm >/dev/null 2>&1; then
        error "npm is not installed. Please install it first."
    fi
    
    # Check if logged in to gcloud and verify project
    if ! gcloud auth print-access-token &>/dev/null; then
        error "Not logged in to gcloud. Please run 'gcloud auth login' first."
    fi
    
    # Verify and set the correct project
    CURRENT_PROJECT=$(gcloud config get-value project)
    if [ "$CURRENT_PROJECT" != "$PROJECT_ID" ]; then
        log "Setting project to $PROJECT_ID..."
        if ! gcloud config set project "$PROJECT_ID"; then
            error "Failed to set project to $PROJECT_ID"
        fi
    fi
    
    # Check if logged in to firebase
    if firebase login:list | grep -q "No users"; then
        error "Not logged in to Firebase. Please run 'firebase login' first."
    fi
}

# Run comprehensive tests
run_tests() {
    log "Running comprehensive test suite..."

    # Run tests with coverage
    log "Executing tests with coverage analysis..."
    if ! dotnet test --no-restore --verbosity normal /p:CollectCoverage=true /p:CoverletOutputFormat=opencover; then
        error "Tests failed"
    fi
    
    log "All tests passed successfully"
    return 0
}

# Deploy backend to production
deploy_backend() {
    log "Starting backend production deployment..."
    
    if ! cd WebAPI; then
        error "WebAPI directory not found"
    fi

    # Tag and build Docker image
    log "Building and tagging Docker image..."
    TAGGED_IMAGE="${BACKEND_IMAGE}:${PROD_TAG}"
    if ! gcloud builds submit --tag "$TAGGED_IMAGE"; then
        error "Failed to build Docker image"
    fi

    # Deploy to Cloud Run with production configurations
    log "Deploying to Cloud Run production..."
    if ! gcloud run deploy "$BACKEND_SERVICE" \
        --image "$TAGGED_IMAGE" \
        --platform managed \
        --region "$REGION" \
        --add-cloudsql-instances "$DB_INSTANCE_FULL" \
        --min-instances 2 \
        --max-instances 10 \
        --memory 1Gi \
        --cpu 1 \
        --timeout 300 \
        --allow-unauthenticated; then
        error "Failed to deploy to Cloud Run"
    fi

    # Tag the successful deployment in git
    git tag -a "prod-${PROD_TAG}" -m "Production deployment ${PROD_TAG}"
    git push origin "prod-${PROD_TAG}"

    cd ..
    log "Backend deployment completed successfully"
    return 0
}

# Deploy frontend to production
deploy_frontend() {
    log "Starting frontend production deployment..."
    
    if ! cd Client; then
        error "Client directory not found"
    fi

    # Clean install dependencies
    log "Clean installing dependencies..."
    rm -rf node_modules package-lock.json
    if ! npm install; then
        error "Failed to install dependencies"
    fi

    # Build the project with production optimizations
    log "Building frontend for production..."
    if ! NODE_ENV=production npm run build; then
        error "Failed to build frontend"
    fi

    # Deploy to Firebase production
    log "Deploying to Firebase production target..."
    if ! firebase deploy --only hosting:prod --project "$PROJECT_ID"; then
        error "Failed to deploy to Firebase"
    fi

    cd ..
    log "Frontend deployment completed successfully"
    return 0
}

# Main deployment function
main() {
    log "Starting production deployment process..."

    # Check requirements first
    check_branch
    check_requirements

    # Show warning and require explicit confirmation
    echo -e "${YELLOW}"
    echo "⚠️  WARNING: You are about to deploy to PRODUCTION ⚠️"
    echo "This will affect real users and data."
    echo -e "${NC}"
    read -p "Are you absolutely sure you want to deploy to production? (type 'DEPLOY' to confirm) " -r
    echo
    if [ "$REPLY" != "DEPLOY" ]; then
        log "Deployment cancelled"
        exit 1
    fi
    
    # Run comprehensive tests
    if ! run_tests; then
        error "Tests failed - aborting deployment"
    fi

    # Deploy backend first
    if ! deploy_backend; then
        error "Backend deployment failed"
    fi

    # Deploy frontend
    if ! deploy_frontend; then
        error "Frontend deployment failed"
        warn "Backend was deployed successfully, but frontend deployment failed"
        exit 1
    fi

    log "🎉 Production deployment completed successfully!"
    log "Deployment tag: $PROD_TAG"
    log "Please monitor the following:"
    log "1. Backend metrics in Google Cloud Console"
    log "2. Error reporting in Firebase Console"
    log "3. Database performance in Cloud SQL"
}

# Run the script
main