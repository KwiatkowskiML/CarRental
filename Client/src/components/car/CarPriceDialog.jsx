import { useState } from 'react';
import { Button } from '../ui/Button';
import { useAuth } from '../../auth/AuthContext';

const INSURANCE_TYPES = [
  { insurance_id: 1, name: 'Standard Insurance' },
  { insurance_id: 2, name: 'Full Insurance' }
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
      const userResponse = await fetch('/api/User/current', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${user.token}`,
          'Cache-Control': 'no-cache, no-store, must-revalidate',
          'Pragma': 'no-cache'
        },
        cache: 'no-store'
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

      try {
        await onSubmit(formData);
        onClose();
      } catch (submitError) {
        if (submitError.message?.includes('not available for the selected dates')) {
          setError('Car is not available for the selected dates. Please choose different dates for your rental period.');
        } else {
          throw submitError;
        }
      }
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
        <h2 style={{ marginBottom: '20px' }}>Rental Options</h2>

        <form onSubmit={handleSubmit}>
          <div style={{ marginBottom: '20px' }}>
            <label style={{ display: 'block', marginBottom: '8px' }}>
              Start Date:*
            </label>
            <input
              type="date"
              value={startDate}
              onChange={(e) => {
                setStartDate(e.target.value);
                setError(null);
              }}
              style={{
                width: '100%',
                padding: '8px',
                borderRadius: '4px',
                border: '1px solid #ddd'
              }}
              min={new Date().toISOString().split('T')[0]}
              required
            />
          </div>

          <div style={{ marginBottom: '20px' }}>
            <label style={{ display: 'block', marginBottom: '8px' }}>
              End Date:*
            </label>
            <input
              type="date"
              value={endDate}
              onChange={(e) => {
                setEndDate(e.target.value);
                setError(null);
              }}
              min={startDate || new Date().toISOString().split('T')[0]}
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
              Insurance Type:*
            </label>
            <select
              value={insuranceId}
              onChange={(e) => {
                setInsuranceId(e.target.value);
                setError(null);
              }}
              style={{
                width: '100%',
                padding: '8px',
                borderRadius: '4px',
                border: '1px solid #ddd'
              }}
              required
            >
              <option value="">Select Insurance</option>
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
                onChange={(e) => setHasGps(e.checked)}
              />
              GPS Navigation
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
                onChange={(e) => setHasChildSeat(e.checked)}
              />
              Child Seat
            </label>
          </div>

          {error && (
            <div style={{
              padding: '16px',
              backgroundColor: '#fee2e2',
              border: '1px solid #dc2626',
              borderRadius: '4px',
              marginBottom: '16px'
            }}>
              <p style={{ color: '#dc2626' }}>
                {error}
              </p>
            </div>
          )}

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
              Cancel
            </Button>
            <Button
              type="submit"
              variant="primary"
              disabled={!isFormValid || loading}
            >
              {loading ? 'Processing...' : 'Get Price'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}