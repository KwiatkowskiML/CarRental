#!/bin/bash

# Exit on any error
set -e

# Configuration
PROJECT_ID="awesomecarrental"
REGION="europe-west4"
BACKEND_SERVICE="car-rental-api"
BACKEND_IMAGE="gcr.io/$PROJECT_ID/$BACKEND_SERVICE"
DB_INSTANCE="awesomecarrental:europe-west4:car-rental-pg-0"

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
}

warn() {
    echo -e "${YELLOW}[WARNING] $1${NC}"
}

# Check if required tools are installed
check_requirements() {
    log "Checking requirements..."
    
    command -v gcloud >/dev/null 2>&1 || {
        error "gcloud CLI is not installed. Please install it first."
        exit 1
    }

    command -v firebase >/dev/null 2>&1 || {
        error "Firebase CLI is not installed. Please install it first."
        exit 1
    }

    command -v dotnet >/dev/null 2>&1 || {
        error "dotnet SDK is not installed. Please install it first."
        exit 1
    }

    command -v npm >/dev/null 2>&1 || {
        error "npm is not installed. Please install it first."
        exit 1
    }
}

# Deploy backend
deploy_backend() {
    log "Starting backend deployment..."
    
    # Navigate to backend directory
    cd WebAPI || {
        error "WebAPI directory not found"
        return 1
    }

    # Build Docker image
    log "Building Docker image..."
    gcloud builds submit --tag "$BACKEND_IMAGE" || {
        error "Failed to build Docker image"
        return 1
    }

    # Deploy to Cloud Run
    log "Deploying to Cloud Run..."
    gcloud run deploy "$BACKEND_SERVICE" \
        --image "$BACKEND_IMAGE" \
        --platform managed \
        --region "$REGION" \
        --add-cloudsql-instances "$DB_INSTANCE" \
        --allow-unauthenticated || {
        error "Failed to deploy to Cloud Run"
        return 1
    }

    cd ..
    log "Backend deployment completed successfully"
    return 0
}

# Deploy frontend
deploy_frontend() {
    log "Starting frontend deployment..."
    
    # Navigate to frontend directory
    cd Client || {
        error "Client directory not found"
        return 1
    }

    # Install dependencies
    log "Installing dependencies..."
    npm install || {
        error "Failed to install dependencies"
        return 1
    }

    # Build the project
    log "Building frontend..."
    npm run build || {
        error "Failed to build frontend"
        return 1
    }

    # Deploy to Firebase
    log "Deploying to Firebase..."
    firebase deploy || {
        error "Failed to deploy to Firebase"
        return 1
    }

    cd ..
    log "Frontend deployment completed successfully"
    return 0
}

# Main deployment function
main() {
    log "Starting deployment process..."

    # Check requirements first
    check_requirements

    # Confirm deployment
    read -p "Are you sure you want to deploy to dev environment? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        log "Deployment cancelled"
        exit 1
    fi

    # Deploy backend first
    if ! deploy_backend; then
        error "Backend deployment failed"
        exit 1
    fi

    # Deploy frontend
    if ! deploy_frontend; then
        error "Frontend deployment failed"
        warn "Backend was deployed successfully, but frontend deployment failed"
        exit 1
    fi

    log "🎉 Full deployment completed successfully!"
}

# Run the script
main