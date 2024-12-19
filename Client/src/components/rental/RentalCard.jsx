import React from 'react';

function RentalCard({ rental }) {
    const { offer } = rental;
    const { car, startDate, endDate, totalPrice, hasGps, hasChildSeat, insurance } = offer;
    const { brand, model, year, description, carProvider } = car;

    return (
        <div className="rental-card" style={cardStyle}>
            <h2>{brand} {model} ({year})</h2>
            <p>{description}</p>
            <p><strong>Start Date:</strong> {new Date(startDate).toLocaleDateString()}</p>
            <p><strong>End Date:</strong> {new Date(endDate).toLocaleDateString()}</p>
            <p><strong>Total Price:</strong> ${totalPrice}</p>
            <p><strong>GPS:</strong> {hasGps ? 'Yes' : 'No'}</p>
            <p><strong>Child Seat:</strong> {hasChildSeat ? 'Yes' : 'No'}</p>
            <p><strong>Insurance:</strong> {insurance ? insurance : 'None'}</p>
            <p><strong>Provider:</strong> {carProvider.name}</p>
            <p><strong>Contact:</strong> {carProvider.contactEmail}</p>
        </div>
    );
}

const cardStyle = {
    border: '1px solid #ddd',
    borderRadius: '8px',
    padding: '16px',
    margin: '16px 0',
    boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
};

export default RentalCard;