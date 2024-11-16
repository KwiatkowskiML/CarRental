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
        setCars(availableCars);
        setFilteredCars(availableCars);
        setLoading(false);
      })
      .catch(error => {
        console.error('Error fetching cars:', error);
        setLoading(false);
      });
  }, [user.token]);

  const handleSearch = (filters) => {
    let filtered = [...cars];

    // Filtrowanie po tekście wyszukiwania
    if (filters.searchText) {
      const searchLower = filters.searchText.toLowerCase();
      filtered = filtered.filter(car => 
        car.brand.toLowerCase().includes(searchLower) ||
        car.model.toLowerCase().includes(searchLower)
      );
    }

    // Filtrowanie po marce
    if (filters.brand) {
      filtered = filtered.filter(car => car.brand === filters.brand);
    }

    // Filtrowanie po modelu
    if (filters.model) {
      filtered = filtered.filter(car => car.model === filters.model);
    }

    // Filtrowanie po roku
    if (filters.year) {
      filtered = filtered.filter(car => car.year.toString() === filters.year);
    }

    // Filtrowanie po typie paliwa
    if (filters.fuelType) {
      filtered = filtered.filter(car => car.fuelType === filters.fuelType);
    }

    // Filtrowanie po mocy
    if (filters.power.min !== null) {
      filtered = filtered.filter(car => car.power >= filters.power.min);
    }
    if (filters.power.max !== null) {
      filtered = filtered.filter(car => car.power <= filters.power.max);
    }

    setFilteredCars(filtered);
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      <Navbar />
      <div style={{ padding: '20px' }}>
        <h1>Available Cars</h1>
        
        <SearchBar cars={cars} onSearch={handleSearch} />

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
                <p>Power: {car.power} HP</p>
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