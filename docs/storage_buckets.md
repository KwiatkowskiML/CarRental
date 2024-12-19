# Google Cloud Storage Setup for Car Images

## Bucket Creation and Configuration

1. Create a new bucket in Google Cloud Storage:
```bash
gsutil mb gs://car-images-dev-0
```

2. Make the bucket publicly accessible:
```bash
gsutil iam ch allUsers:objectViewer gs://car-images-dev-0
```

## CORS Configuration

1. Create a file named `cors.json` (in bucket_config):
```json
[
  {
    "origin": ["*"],
    "method": ["GET", "HEAD", "OPTIONS"],
    "responseHeader": [
      "Content-Type",
      "Access-Control-Allow-Origin",
      "Cross-Origin-Resource-Policy",
      "Cross-Origin-Embedder-Policy"
    ],
    "maxAgeSeconds": 3600
  }
]
```

2. Apply CORS configuration:
```bash
gsutil cors set bucket_config/cors.json gs://car-images-dev-0
```

## Image URLs Format

Use the following format for image URLs:
```
https://storage.googleapis.com/car-images-dev-0/image-name.jpg
```

## Frontend Implementation

When displaying images, add the `crossOrigin` attribute:
```jsx
<img 
  src={imageUrl} 
  alt="Car image" 
  crossOrigin="anonymous"
/>
```

## Security Notes

- The bucket is configured for public access. In production, consider using signed URLs for better security.
