import React, { useState, useEffect } from 'react';
import { ReturnConfirmationDialog } from './ReturnConfirmationDialog';
import { useAuth } from '../../auth/AuthContext';
import { Button } from '../ui/Button';

function WorkerRentalCard({ rental, onStatusUpdate }) {
    const { user } = useAuth();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [showFullImage, setShowFullImage] = useState(false);
    const [returnInfo, setReturnInfo] = useState(null);
    const [loadingReturn, setLoadingReturn] = useState(false);

    const { offer } = rental;
    const { car, startDate, endDate, totalPrice, hasGps, hasChildSeat, insurance } = offer;
    const { brand, model, year, description, carProvider } = car;

    useEffect(() => {
        const fetchReturnInfo = async () => {
            if (rental.rentalStatus?.description === 'Completed') {
                setLoadingReturn(true);
                try {
                    const response = await fetch(`/api/Rentals/${rental.rentalId}/return-info`, {
                        headers: {
                            'Authorization': `Bearer ${user.token}`
                        }
                    });

                    if (response.ok) {
                        const data = await response.json();
                        setReturnInfo(data);
                    }
                } catch (err) {
                    console.error('Error fetching return info:', err);
                } finally {
                    setLoadingReturn(false);
                }
            }
        };

        fetchReturnInfo();
    }, [rental.rentalId, rental.rentalStatus?.description, user.token]);

    const handleAcceptReturn = async (returnData) => {
        setIsSubmitting(true);
        setError(null);

        try {
            const response = await fetch('/api/Worker/accept-return', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${user.token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    ...returnData,
                    rentalId: rental.rentalId
                })
            });

            if (!response.ok) {
                throw new Error('Failed to accept return');
            }

            setIsDialogOpen(false);
            onStatusUpdate();
        } catch (err) {
            setError(err.message);
        } finally {
            setIsSubmitting(false);
        }
    };

    const getStatusColor = (status) => {
        if (!status?.description) return '#6b7280';

        switch (status.description.toLowerCase()) {
            case 'confirmed':
                return '#22c55e';
            case 'pending return':
                return '#eab308';
            case 'completed':
                return '#6b7280';
            default:
                return '#6b7280';
        }
    };

    const ImageModal = ({ imageUrl, onClose }) => (
        <div style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.8)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            zIndex: 1000,
            cursor: 'pointer'
        }} onClick={onClose}>
            <img 
                src={imageUrl}
                alt="Return photo full size"
                style={{
                    maxWidth: '90%',
                    maxHeight: '90vh',
                    objectFit: 'contain'
                }}
                crossOrigin="anonymous"
            />
        </div>
    );

    return (
        <div style={{
            border: '1px solid #ddd',
            borderRadius: '8px',
            padding: '16px',
            backgroundColor: 'white',
            boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
        }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                <div style={{ flex: 1 }}>
                    <h2 style={{ fontSize: '20px', marginBottom: '8px' }}>
                        {brand} {model} ({year})
                    </h2>
                    <div style={{
                        display: 'inline-block',
                        padding: '4px 8px',
                        borderRadius: '4px',
                        backgroundColor: getStatusColor(rental.rentalStatus),
                        color: 'white',
                        fontSize: '14px',
                        marginBottom: '12px'
                    }}>
                        {rental.rentalStatus?.description || 'Unknown'}
                    </div>
                </div>

                {returnInfo?.photoUrl && (
                    <div style={{
                        marginLeft: '16px',
                        cursor: 'pointer'
                    }} onClick={() => setShowFullImage(true)}>
                        <img
                            src={returnInfo.photoUrl}
                            alt="Return photo"
                            style={{
                                width: '120px',
                                height: '80px',
                                objectFit: 'cover',
                                borderRadius: '4px',
                                border: '1px solid #ddd'
                            }}
                            crossOrigin="anonymous"
                        />
                    </div>
                )}

                {rental.rentalStatus?.description === 'Pending return' && (
                    <div style={{ marginLeft: '16px' }}>
                        <Button
                            onClick={() => setIsDialogOpen(true)}
                            disabled={isSubmitting}
                        >
                            Accept Return
                        </Button>
                    </div>
                )}
            </div>

            <div style={{ 
                display: 'grid', 
                gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', 
                gap: '16px', 
                marginTop: '16px' 
            }}>
                <div>
                    <DetailRow label="Start Date" value={new Date(startDate).toLocaleDateString()} />
                    <DetailRow label="End Date" value={new Date(endDate).toLocaleDateString()} />
                    <DetailRow label="Total Price" value={`${totalPrice.toFixed(2)} PLN`} />
                </div>
                <div>
                    <DetailRow label="GPS" value={hasGps ? 'Yes' : 'No'} />
                    <DetailRow label="Child Seat" value={hasChildSeat ? 'Yes' : 'No'} />
                    <DetailRow label="Insurance" value={insurance ? insurance.name : 'None'} />
                </div>
                <div>
                    <DetailRow label="Provider" value={carProvider.name} />
                    <DetailRow label="Contact" value={carProvider.contactEmail} />
                    <DetailRow label="Rental ID" value={rental.rentalId} />
                </div>
            </div>

            {returnInfo && (
                <div style={{ 
                    marginTop: '16px',
                    padding: '12px',
                    backgroundColor: '#f8f9fa',
                    borderRadius: '4px'
                }}>
                    <h3 style={{ 
                        fontSize: '16px', 
                        fontWeight: 'bold',
                        marginBottom: '8px'
                    }}>
                        Return Details
                    </h3>
                    <DetailRow label="Return Date" value={new Date(returnInfo.returnDate).toLocaleDateString()} />
                    {returnInfo.conditionDescription && (
                        <DetailRow label="Condition" value={returnInfo.conditionDescription} />
                    )}
                </div>
            )}

            <ReturnConfirmationDialog
                isOpen={isDialogOpen}
                onClose={() => setIsDialogOpen(false)}
                onConfirm={handleAcceptReturn}
                isSubmitting={isSubmitting}
            />

            {showFullImage && returnInfo?.photoUrl && (
                <ImageModal 
                    imageUrl={returnInfo.photoUrl} 
                    onClose={() => setShowFullImage(false)} 
                />
            )}

            {error && (
                <div style={{
                    marginTop: '12px',
                    padding: '8px',
                    backgroundColor: '#fee2e2',
                    color: '#dc2626',
                    borderRadius: '4px'
                }}>
                    {error}
                </div>
            )}
        </div>
    );
}

function DetailRow({ label, value }) {
    return (
        <div style={{ marginBottom: '8px' }}>
            <span style={{ fontWeight: 'bold', marginRight: '8px', color: '#666' }}>
                {label}:
            </span>
            <span>{value}</span>
        </div>
    );
}

export default WorkerRentalCard;