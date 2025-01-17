import React, { useState } from 'react';
import { Button } from '../ui/Button';
import PhotoUpload from './PhotoUpload';

export function ReturnConfirmationDialog({ isOpen, onClose, onConfirm, isSubmitting }) {
    const [formData, setFormData] = useState({
        conditionDescription: '',
        photoUrl: '',
        returnDate: new Date().toISOString().split('T')[0]
    });
    const [error, setError] = useState(null);

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!formData.photoUrl) {
            setError('Please upload a photo of the returned vehicle');
            return;
        }
        onConfirm(formData);
    };

    const handlePhotoSelect = (url) => {
        setFormData(prev => ({
            ...prev,
            photoUrl: url
        }));
        setError(null);
    };

    const handlePhotoError = (errorMessage) => {
        setError(errorMessage);
    };

    if (!isOpen) return null;

    return (
        <div style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            zIndex: 1000
        }}>
            <div style={{
                backgroundColor: 'white',
                padding: '24px',
                borderRadius: '8px',
                width: '100%',
                maxWidth: '500px'
            }}>
                <h2 style={{ 
                    marginBottom: '20px', 
                    fontSize: '1.5rem', 
                    fontWeight: 'bold' 
                }}>
                    Confirm Return
                </h2>

                <form onSubmit={handleSubmit}>
                    <div style={{ marginBottom: '16px' }}>
                        <label style={{ 
                            display: 'block', 
                            marginBottom: '8px',
                            fontWeight: '500'
                        }}>
                            Return Date:
                        </label>
                        <input
                            type="date"
                            value={formData.returnDate}
                            onChange={(e) => setFormData(prev => ({
                                ...prev,
                                returnDate: e.target.value
                            }))}
                            style={{
                                width: '100%',
                                padding: '8px',
                                border: '1px solid #ddd',
                                borderRadius: '4px'
                            }}
                            required
                        />
                    </div>

                    <div style={{ marginBottom: '16px' }}>
                        <label style={{ 
                            display: 'block', 
                            marginBottom: '8px',
                            fontWeight: '500'
                        }}>
                            Condition Description:
                        </label>
                        <textarea
                            value={formData.conditionDescription}
                            onChange={(e) => setFormData(prev => ({
                                ...prev,
                                conditionDescription: e.target.value
                            }))}
                            style={{
                                width: '100%',
                                padding: '8px',
                                border: '1px solid #ddd',
                                borderRadius: '4px',
                                minHeight: '100px'
                            }}
                            required
                            placeholder="Describe the condition of the returned vehicle..."
                        />
                    </div>

                    <div style={{ marginBottom: '16px' }}>
                        <label style={{ 
                            display: 'block', 
                            marginBottom: '8px',
                            fontWeight: '500'
                        }}>
                            Vehicle Photo:
                        </label>
                        <PhotoUpload 
                            onPhotoSelect={handlePhotoSelect}
                            onError={handlePhotoError}
                        />
                    </div>

                    {error && (
                        <div style={{
                            padding: '8px 12px',
                            backgroundColor: '#fee2e2',
                            color: '#dc2626',
                            borderRadius: '4px',
                            marginBottom: '16px',
                            fontSize: '14px'
                        }}>
                            {error}
                        </div>
                    )}

                    <div style={{
                        display: 'flex',
                        justifyContent: 'flex-end',
                        gap: '12px',
                        marginTop: '24px'
                    }}>
                        <Button
                            type="button"
                            variant="secondary"
                            onClick={onClose}
                            disabled={isSubmitting}
                        >
                            Cancel
                        </Button>
                        <Button
                            type="submit"
                            disabled={isSubmitting}
                        >
                            {isSubmitting ? 'Processing...' : 'Confirm Return'}
                        </Button>
                    </div>
                </form>
            </div>
        </div>
    );
}