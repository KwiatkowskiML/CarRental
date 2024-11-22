import { useState } from 'react';
import { Button } from '../ui/Button';
import { useAuth } from '../../auth/AuthContext';

const INSURANCE_TYPES = [
  { insurance_id: 1, name: 'Standard Insurance', price: 50.00 },
  { insurance_id: 2, name: 'Full Insurance', price: 100.00 }
];

export function CarPriceDialog({ isOpen, onClose, onSubmit }) {
  const { user } = useAuth();
  const [insuranceId, setInsuranceId] = useState('');
  const [hasGps, setHasGps] = useState(false);
  const [hasChildSeat, setHasChildSeat] = useState(false);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      // Get user ID from backend using the token
      const userResponse = await fetch('/api/user/current', {
        headers: {
          'Authorization': `Bearer ${user.token}`
        }
      });

      if (!userResponse.ok) {
        throw new Error('Failed to get user information');
      }

      const userData = await userResponse.json();

      const formData = {
        insuranceId: parseInt(insuranceId),
        hasGps,
        hasChildSeat,
        startDate,
        endDate,
        userId: userData.userId
      };

      await onSubmit(formData);
      onClose();
    } catch (error) {
      console.error('Error submitting form:', error);
      setError('Failed to submit form. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  const isFormValid = insuranceId !== '' && startDate && endDate;

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
        <h2 style={{ marginBottom: '20px' }}>Dodatkowe opcje</h2>
        
        <form onSubmit={handleSubmit}>
          <div style={{ marginBottom: '20px' }}>
            <label style={{ display: 'block', marginBottom: '8px' }}>
              Data rozpoczęcia:*
            </label>
            <input
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              style={{
                width: '100%',
                padding: '8px',
                borderRadius: '4px',
                border: '1px solid #ddd'
              }}
              required
            />
          </div>

          <div style={{ marginBottom: '20px' }}>
            <label style={{ display: 'block', marginBottom: '8px' }}>
              Data zakończenia:*
            </label>
            <input
              type="date"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
              min={startDate}
              style={{
                width: '100%',
                padding: '8px',
                borderRadius: '4px',
                border: '1px solid #ddd'
              }}
              required
            />
          </div>

          <div style={{ marginBottom: '20px' }}>
            <label style={{ display: 'block', marginBottom: '8px' }}>
              Typ ubezpieczenia:*
            </label>
            <select
              value={insuranceId}
              onChange={(e) => setInsuranceId(e.target.value)}
              style={{
                width: '100%',
                padding: '8px',
                borderRadius: '4px',
                border: '1px solid #ddd'
              }}
              required
            >
              <option value="">Wybierz ubezpieczenie</option>
              {INSURANCE_TYPES.map(insurance => (
                <option key={insurance.insurance_id} value={insurance.insurance_id}>
                  {insurance.name}
                </option>
              ))}
            </select>
          </div>

          <div style={{ marginBottom: '20px' }}>
            <label style={{ 
              display: 'flex', 
              alignItems: 'center',
              gap: '8px'
            }}>
              <input
                type="checkbox"
                checked={hasGps}
                onChange={(e) => setHasGps(e.target.checked)}
              />
              GPS
            </label>
          </div>

          <div style={{ marginBottom: '20px' }}>
            <label style={{ 
              display: 'flex', 
              alignItems: 'center',
              gap: '8px'
            }}>
              <input
                type="checkbox"
                checked={hasChildSeat}
                onChange={(e) => setHasChildSeat(e.target.checked)}
              />
              Fotelik dla dziecka
            </label>
          </div>

          <div style={{ 
            display: 'flex', 
            justifyContent: 'flex-end',
            gap: '12px'
          }}>
            <Button
              type="button"
              variant="secondary"
              onClick={onClose}
            >
              Anuluj
            </Button>
            <Button
              type="submit"
              variant="primary"
              disabled={!isFormValid}
            >
              Zapytaj o cenę
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}