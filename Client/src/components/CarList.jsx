import { useState, useEffect } from 'react';
import { useAuth } from '../auth/AuthContext';
import { Navbar } from './Navbar';
import { SearchBar } from './SearchBar';

export function CarList() {
  const [cars, setCars] = useState([]);
  const [filteredCars, setFilteredCars] = useState([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();

  useEffect(() => {
    fetch('/api/cars', {
      headers: {
        'Authorization': `Bearer ${user.token}`
      }
    })
      .then(res => res.json())
      .then(data => {
        const availableCars = data.filter(car => car.status === 'available');
        console.log('Available cars:', availableCars);
        setCars(availableCars);
        setFilteredCars(availableCars);
        setLoading(false);
      })
      .catch(error => {
        console.error('Error fetching cars:', error);
        setLoading(false);
      });
  }, [user.token]);

  const handleSearch = (searchText) => {
    console.log('Search text:', searchText);
    
    if (!searchText.trim()) {
      setFilteredCars(cars);
      return;
    }

    const searchLower = searchText.toLowerCase();
    const filtered = cars.filter(car => {
      const brandMatch = car.brand?.toLowerCase().includes(searchLower) || false;
      const modelMatch = car.model?.toLowerCase().includes(searchLower) || false;
      return brandMatch || modelMatch;
    });
    
    console.log('Filtered cars:', filtered);
    setFilteredCars(filtered);
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      <Navbar />
      <div style={{ padding: '20px' }}>
        <h1>Available Cars</h1>
        
        <SearchBar onSearch={handleSearch} />

        <div style={{ 
          display: 'grid', 
          gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
          gap: '20px',
          marginTop: '20px'
        }}>
          {filteredCars.length > 0 ? (
            filteredCars.map(car => (
              <div key={car.carId} style={{
                border: '1px solid #ddd',
                padding: '15px',
                borderRadius: '8px'
              }}>
                <h3>{car.brand} {car.model}</h3>
                <p>Year: {car.year}</p>
                <p>Location: {car.location}</p>
                <p>Fuel Type: {car.fuelType}</p>
              </div>
            ))
          ) : (
            <div>No cars found matching your search criteria</div>
          )}
        </div>
      </div>
    </div>
  );
}