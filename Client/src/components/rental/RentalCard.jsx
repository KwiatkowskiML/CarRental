import React from 'react';

function RentalCard({ rental }) {
    return (
        <div className="rental-card" style={cardStyle}>
            <h2>{rental.title}</h2>
            <p>{rental.description}</p>
            <p><strong>Start Date:</strong> {new Date(rental.startDate).toLocaleDateString()}</p>
            <p><strong>End Date:</strong> {new Date(rental.endDate).toLocaleDateString()}</p>
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