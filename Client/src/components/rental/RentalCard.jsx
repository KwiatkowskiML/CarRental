import React, { useState } from 'react';
import { Button } from '../ui/Button';
import { useAuth } from '../../auth/AuthContext';

function RentalCard({ rental, onStatusUpdate }) {
    const { user } = useAuth();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);

    const { offer } = rental;
    const { car, startDate, endDate, totalPrice, hasGps, hasChildSeat, insurance } = offer;
    const { brand, model, year, description, carProvider } = car;

    const handleReturn = async () => {
        setIsSubmitting(true);
        setError(null);

        try {
            const response = await fetch(`/api/rentals/${rental.rentalId}/return`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${user.token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    status: 'pending_return'
                })
            });

            if (!response.ok) {
                throw new Error('Failed to update rental status');
            }

            // Call the parent component's update function
            if (onStatusUpdate) {
                onStatusUpdate();
            }
        } catch (err) {
            console.error('Error updating rental status:', err);
            setError('Failed to update rental status. Please try again.');
        } finally {
            setIsSubmitting(false);
        }
    };

    const getStatusColor = (status) => {
        switch (status) {
            case 'active':
                return '#22c55e'; // green
            case 'pending_return':
                return '#eab308'; // yellow
            case 'completed':
                return '#6b7280'; // gray
            default:
                return '#6b7280';
        }
    };

    return (
        <div className="rental-card" style={cardStyle}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                <div>
                    <h2 style={{ marginBottom: '12px' }}>{brand} {model} ({year})</h2>
                    <div style={{
                        display: 'inline-block',
                        padding: '4px 8px',
                        borderRadius: '4px',
                        backgroundColor: getStatusColor(rental.status),
                        color: 'white',
                        fontSize: '14px',
                        marginBottom: '12px'
                    }}>
                        {rental.status}
                    </div>
                </div>
                {rental.status === 'active' && (
                    <Button
                        variant="secondary"
                        onClick={handleReturn}
                        disabled={isSubmitting}
                        style={{ minWidth: '120px' }}
                    >
                        {isSubmitting ? 'Processing...' : 'Return Car'}
                    </Button>
                )}
            </div>

            <div style={{ marginBottom: '8px' }}>{description}</div>

            <div style={detailsGridStyle}>
                <DetailRow label="Start Date" value={new Date(startDate).toLocaleDateString()} />
                <DetailRow label="End Date" value={new Date(endDate).toLocaleDateString()} />
                <DetailRow label="Total Price" value={`$${totalPrice}`} />
                <DetailRow label="GPS" value={hasGps ? 'Yes' : 'No'} />
                <DetailRow label="Child Seat" value={hasChildSeat ? 'Yes' : 'No'} />
                <DetailRow label="Insurance" value={insurance ? insurance.name : 'None'} />
                <DetailRow label="Provider" value={carProvider.name} />
                <DetailRow label="Contact" value={carProvider.contactEmail} />
            </div>

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
        <div style={{ marginBottom: '4px' }}>
            <span style={{ fontWeight: 'bold', marginRight: '8px' }}>{label}:</span>
            <span>{value}</span>
        </div>
    );
}

const cardStyle = {
    border: '1px solid #ddd',
    borderRadius: '8px',
    padding: '16px',
    margin: '16px 0',
    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
    backgroundColor: 'white'
};

const detailsGridStyle = {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
    gap: '12px',
    marginTop: '16px'
};

export default RentalCard;