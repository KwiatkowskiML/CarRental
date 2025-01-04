import React, { useState, useEffect } from 'react';
import { ReturnConfirmationDialog } from './ReturnConfirmationDialog';
import { useAuth } from '../../auth/AuthContext';
import { Button } from '../ui/Button';

function WorkerRentalCard({ rental, onStatusUpdate }) {
    const { user } = useAuth();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState(null);
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [employeeId, setEmployeeId] = useState(null);

    const { offer } = rental;
    const { car, startDate, endDate, totalPrice, hasGps, hasChildSeat, insurance } = offer;
    const { brand, model, year, description, carProvider } = car;

    // Fetch employee ID when component mounts
    useEffect(() => {
        const fetchEmployeeId = async () => {
            try {
                const response = await fetch('/api/User/current', {
                    method: 'GET',
                    headers: {
                        'Authorization': `Bearer ${user.token}`,
                        'Cache-Control': 'no-cache, no-store, must-revalidate',
                        'Pragma': 'no-cache'
                    },
                    cache: 'no-store'
                });
                if (response.ok) {
                    const data = await response.json();
                    setEmployeeId(data.userId);
                }
            } catch (err) {
                console.error('Error fetching employee ID:', err);
                setError('Failed to fetch employee information');
            }
        };

        fetchEmployeeId();
    }, [user.token]);

    const handleAcceptReturn = async (returnData) => {
        setIsSubmitting(true);
        setError(null);

        const requestBody = {
            rentalId: rental.rentalId,
            employeeId: employeeId,
            conditionDescription: returnData.conditionDescription,
            photoUrl: returnData.photoUrl,
            returnDate: returnData.returnDate
        };

        try {
            const response = await fetch('/api/Worker/accept-return', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${user.token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || 'Failed to accept return');
            }

            setIsDialogOpen(false);

            if (onStatusUpdate) {
                onStatusUpdate();
            }
        } catch (err) {
            console.error('Error accepting return:', err);
            setError(err.message || 'Failed to accept return. Please try again.');
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

    return (
        <div className="rental-card" style={cardStyle}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                <div>
                    <h2 style={{ marginBottom: '12px', fontSize: '24px' }}>
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
                {rental.rentalStatus?.description === 'Pending return' && (
                    <Button
                        variant="primary"
                        onClick={() => setIsDialogOpen(true)}
                        disabled={isSubmitting}
                        style={{ minWidth: '120px' }}
                    >
                        Accept Return
                    </Button>
                )}

                <ReturnConfirmationDialog
                    isOpen={isDialogOpen}
                    onClose={() => setIsDialogOpen(false)}
                    onConfirm={handleAcceptReturn}
                    isSubmitting={isSubmitting}
                />
            </div>

            {description && (
                <div style={{ marginBottom: '8px', color: '#666' }}>
                    {description}
                </div>
            )}

            <div style={detailsGridStyle}>
                <DetailRow label="Start Date" value={new Date(startDate).toLocaleDateString()} />
                <DetailRow label="End Date" value={new Date(endDate).toLocaleDateString()} />
                <DetailRow label="Total Price" value={`${totalPrice.toFixed(2)} PLN`} />
                <DetailRow label="GPS" value={hasGps ? 'Yes' : 'No'} />
                <DetailRow label="Child Seat" value={hasChildSeat ? 'Yes' : 'No'} />
                <DetailRow label="Insurance" value={insurance ? insurance.name : 'None'} />
                <DetailRow label="Provider" value={carProvider.name} />
                <DetailRow label="Contact" value={carProvider.contactEmail} />
                <DetailRow label="Rental ID" value={rental.rentalId} />
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
            <span style={{ fontWeight: 'bold', marginRight: '8px', color: '#666' }}>
                {label}:
            </span>
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

export default WorkerRentalCard;