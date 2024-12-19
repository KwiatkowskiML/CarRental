# Making changes
### For Frontend Changes

1. Make your code changes in the React application in Client
```bash
cd Client
```

2. Test locally:
```bash
npm run dev
```

3. Update environment variables if needed in `.env`, change api target to cloud run url in vite.config
```
target: 'https://car-rental-api-795304686308.europe-west4.run.app',
```

4. Build the new version:
```bash
npm run build
```

5. Deploy the new version to Firebase:
```bash
firebase deploy
```

### For Changes that Affect Both Frontend and Backend

1. First deploy backend changes:
   ```bash
   gcloud builds submit --tag gcr.io/awesomecarrental/car-rental-api
   
   gcloud run deploy car-rental-api \
     --image gcr.io/awesomecarrental/car-rental-api \
     --platform managed \
     --region europe-west4 \
     --add-cloudsql-instances awesomecarrental:europe-west4:car-rental-pg-0 \
     --allow-unauthenticated
   ```

2. Then deploy frontend changes:
   ```bash
   npm run build
   firebase deploy
   ```

### Rollback Procedures

If issues are found after deployment:

1. Frontend rollback:
   - Go to Firebase Console
   - Navigate to Hosting
   - Find the previous deployment
   - Click "Rollback"

2. Backend rollback:
   ```bash
   gcloud run services rollback car-rental-api \
     --platform managed \
     --region europe-west4
   ```

# Deploying Frontend to Firebase Hosting

## 1. Enable Firebase in GCP Console

1. Go to Google Cloud Console
2. Navigate to your project "awesomecarrental"
3. Search for "Firebase" in the APIs & Services
4. Enable Firebase API:			
- Firebase Cloud Messaging API					
- Firebase Dynamic Links API					
- Firebase Hosting API					
- Firebase Installations API					
- Firebase Management API					
- Firebase Remote Config API					
- Firebase Remote Config Realtime API					
- Firebase Rules API

## 2. Environment Configuration

Create `.env.production` file in your React project root:
```
VITE_GOOGLE_CLIENT_ID=your-google-client-id
VITE_API_URL=https://car-rental-api-795304686308.europe-west4.run.app
```

## 3. Firebase Project Setup

1. Install Firebase CLI:
```bash
npm install -g firebase-tools
```

2. Login to Firebase:
```bash
firebase login
```

3. Initialize Firebase in your project directory:
```bash
firebase init
```
- Select "Hosting: Configure files for Firebase Hosting"
- Choose existing project (awesomecarrental)
- Use `dist` as your public directory
- Configure as single-page app: Yes

## 4. Firebase Configuration

Create `firebase.json` with specific configuration:
```json
{
  "hosting": {
    "public": "dist",
    "ignore": [
      "firebase.json",
      "**/.*",
      "**/node_modules/**"
    ],
    "rewrites": [
      {
        "source": "/api/**",
        "run": {
          "serviceId": "car-rental-api",
          "region": "europe-west4"
        }
      },
      {
        "source": "**",
        "destination": "/index.html"
      }
    ],
    "headers": [
      {
        "source": "**",
        "headers": [
          {
            "key": "Cross-Origin-Opener-Policy",
            "value": "same-origin-allow-popups"
          },
          {
            "key": "Cross-Origin-Embedder-Policy",
            "value": "require-corp"
          },
          {
            "key": "Cache-Control",
            "value": "public, max-age=3600"
          }
        ]
      }
    ]
  }
}
```

## 5. Build and Deploy

1. Build your React application:
```bash
npm run build
```

2. Deploy to Firebase:
```bash
firebase deploy
```

## 6. Post-Deployment Configuration

1. Update Google OAuth settings:
   - Go to Google Cloud Console → APIs & Services → Credentials
   - Edit OAuth 2.0 Client ID
   - Add Firebase URLs to authorized JavaScript origins:
     ```
     https://awesomecarrental.web.app
     https://awesomecarrental.firebaseapp.com
     ```
   - and to authorized reroutes
     ```
     https://awesomecarrental.web.app
     https://awesomecarrental.firebaseapp.com
     https://awesomecarrental.web.app/login
     https://awesomecarrental.firebaseapp.com/login
     ```