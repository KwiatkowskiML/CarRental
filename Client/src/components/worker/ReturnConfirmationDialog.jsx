import React, { useState } from 'react';
import { Button } from '../ui/Button';

export function ReturnConfirmationDialog({ isOpen, onClose, onConfirm, isSubmitting }) {
    const [formData, setFormData] = useState({
        conditionDescription: '',
        photoUrl: '',
        returnDate: new Date().toISOString().split('T')[0]
    });

    const handleSubmit = (e) => {
        e.preventDefault();
        onConfirm(formData);
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
                <h2 style={{ marginBottom: '20px', fontSize: '1.5rem', fontWeight: 'bold' }}>
                    Confirm Return
                </h2>

                <form onSubmit={handleSubmit}>
                    <div style={{ marginBottom: '16px' }}>
                        <label style={{ display: 'block', marginBottom: '8px' }}>
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
                        <label style={{ display: 'block', marginBottom: '8px' }}>
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
                        <label style={{ display: 'block', marginBottom: '8px' }}>
                            Photo URL:
                        </label>
                        <input
                            type="url"
                            value={formData.photoUrl}
                            onChange={(e) => setFormData(prev => ({
                                ...prev,
                                photoUrl: e.target.value
                            }))}
                            style={{
                                width: '100%',
                                padding: '8px',
                                border: '1px solid #ddd',
                                borderRadius: '4px'
                            }}
                            placeholder="Enter URL to vehicle photos..."
                        />
                    </div>

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