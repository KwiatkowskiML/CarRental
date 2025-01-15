import React, { useState } from 'react';
import { Camera, X } from 'lucide-react';

const PhotoUpload = ({ onPhotoSelect, onError }) => {
  const [preview, setPreview] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleFileSelect = async (event) => {
    const file = event.target.files[0];
    if (!file) return;

    // Validate file type
    const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg'];
    if (!allowedTypes.includes(file.type.toLowerCase())) {
      onError?.('Please select a JPG or PNG file');
      return;
    }

    // Validate file size (100MB limit)
    if (file.size > 100 * 1024 * 1024) {
      onError?.('File size should be less than 100MB');
      return;
    }

    setIsLoading(true);
    try {
      // Create preview first
      const reader = new FileReader();
      reader.onloadend = () => {
        setPreview(reader.result);
      };
      reader.readAsDataURL(file);

      // Create form data
      const formData = new FormData();
      formData.append('file', file);

      // Upload file
      const response = await fetch('/api/Worker/upload-photo', {
        method: 'POST',
        body: formData,
      });

      if (!response.ok) {
        throw new Error(await response.text() || 'Failed to upload photo');
      }

      const data = await response.json();
      onPhotoSelect(data.url);
    } catch (error) {
      console.error('Upload error:', error);
      onError?.(error.message || 'Failed to upload photo');
      setPreview(null);
    } finally {
      setIsLoading(false);
    }
  };

  const handleRemovePhoto = () => {
    setPreview(null);
    onPhotoSelect('');
  };

  return (
    <div style={{ width: '100%', maxWidth: '400px', margin: '0 auto' }}>
      <div 
        style={{ 
          width: '100%', 
          height: '200px',
          position: 'relative',
          border: '2px dashed #ddd',
          borderRadius: '8px',
          backgroundColor: '#f8f9fa',
          overflow: 'hidden'
        }}
      >
        <input
          type="file"
          accept="image/jpeg,image/png"
          onChange={handleFileSelect}
          className="hidden"
          id="photo-upload"
          style={{ display: 'none' }}
        />
        
        {!preview ? (
          <label 
            htmlFor="photo-upload"
            style={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              justifyContent: 'center',
              width: '100%',
              height: '100%',
              cursor: 'pointer',
            }}
          >
            {isLoading ? (
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900" />
            ) : (
              <>
                <Camera style={{ width: 32, height: 32, marginBottom: 8, color: '#666' }} />
                <span style={{ color: '#666', fontSize: '14px' }}>Click to upload photo</span>
              </>
            )}
          </label>
        ) : (
          <div style={{ width: '100%', height: '100%', position: 'relative' }}>
            <img
              src={preview}
              alt="Return photo preview"
              style={{
                width: '100%',
                height: '100%',
                objectFit: 'contain',
                padding: '8px'
              }}
            />
            <button
              onClick={handleRemovePhoto}
              style={{
                position: 'absolute',
                top: '8px',
                right: '8px',
                backgroundColor: '#dc2626',
                color: 'white',
                borderRadius: '50%',
                padding: '4px',
                cursor: 'pointer',
                border: 'none',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center'
              }}
              type="button"
            >
              <X style={{ width: 16, height: 16 }} />
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default PhotoUpload;