import { useState, useEffect } from 'react';
import { useAuth } from '../auth/AuthContext';
import { Navbar } from './Navbar';
import { SearchBar } from './SearchBar';

export function CarList() {
  const [cars, setCars] = useState([]);
  const [filteredCars, setFilteredCars] = useState([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const carsPerPage = 5;
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

    if (filters.searchText) {
      const searchTerms = filters.searchText.toLowerCase().split(' ');
      filtered = filtered.filter(car => {
        const carFullName = `${car.brand} ${car.model}`.toLowerCase();
        return searchTerms.every(term => 
          carFullName.includes(term) ||
          car.brand.toLowerCase().includes(term) ||
          car.model.toLowerCase().includes(term)
        );
      });
    }

    if (filters.brand) {
      filtered = filtered.filter(car => car.brand === filters.brand);
    }

    if (filters.model) {
      filtered = filtered.filter(car => car.model === filters.model);
    }

    if (filters.year) {
      filtered = filtered.filter(car => car.year.toString() === filters.year);
    }

    if (filters.fuelType) {
      filtered = filtered.filter(car => car.fuelType === filters.fuelType);
    }

    if (filters.power.min !== null) {
      filtered = filtered.filter(car => car.power >= filters.power.min);
    }
    if (filters.power.max !== null) {
      filtered = filtered.filter(car => car.power <= filters.power.max);
    }

    setFilteredCars(filtered);
    setCurrentPage(1); // Reset to first page when filtering
  };

  // Obliczanie indeksów dla aktualnej strony
  const indexOfLastCar = currentPage * carsPerPage;
  const indexOfFirstCar = indexOfLastCar - carsPerPage;
  const currentCars = filteredCars.slice(indexOfFirstCar, indexOfLastCar);
  const totalPages = Math.ceil(filteredCars.length / carsPerPage);

  // Funkcja zmiany strony
  const paginate = (pageNumber) => {
    if (pageNumber < 1 || pageNumber > totalPages) return;
    setCurrentPage(pageNumber);
    window.scrollTo(0, 0); // Przewiń do góry strony
  };

  if (loading) return <div>Loading...</div>;

  return (
    <div>
      <Navbar />
      <div style={{ padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
        <h1>Available Cars</h1>
        
        <SearchBar cars={cars} onSearch={handleSearch} />

        <div style={{ 
          display: 'flex',
          flexDirection: 'column',
          gap: '20px',
          marginTop: '20px'
        }}>
          {currentCars.length > 0 ? (
            <>
              {currentCars.map(car => (
                <div 
                  key={car.carId} 
                  style={{
                    display: 'flex',
                    backgroundColor: 'white',
                    borderRadius: '8px',
                    boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
                    overflow: 'hidden',
                    alignItems: 'center',
                    padding: '16px',
                    gap: '20px'
                  }}
                >
                  <img 
                    src="" // to be set
                    alt={`${car.brand} ${car.model}`}
                    style={{
                      width: '300px',
                      height: '200px',
                      objectFit: 'cover',
                      borderRadius: '4px'
                    }}
                  />
                  <div style={{ flex: 1 }}>
                    <h2 style={{ 
                      fontSize: '24px', 
                      marginBottom: '8px' 
                    }}>
                      {car.brand} {car.model}
                    </h2>
                    <div style={{ 
                      fontSize: '14px',
                      color: '#666',
                      marginBottom: '4px'
                    }}>
                      {car.fuelType} • {car.year}
                    </div>
                    <p style={{ 
                      color: '#666',
                      marginTop: '8px' 
                    }}>
                      {car.description || '(opis)'}
                    </p>
                  </div>
                  <button
                    style={{
                      backgroundColor: '#8B4513',
                      color: 'white',
                      border: 'none',
                      padding: '12px 24px',
                      borderRadius: '4px',
                      cursor: 'pointer',
                      fontWeight: 'bold',
                      whiteSpace: 'nowrap'
                    }}
                    onClick={() => {}}
                  >
                    Zapytaj o cenę
                  </button>
                </div>
              ))}
              
              {/* Paginacja */}
              <div style={{
                display: 'flex',
                justifyContent: 'center',
                gap: '10px',
                marginTop: '20px',
                padding: '20px'
              }}>
                <button
                  onClick={() => paginate(currentPage - 1)}
                  disabled={currentPage === 1}
                  style={{
                    padding: '8px 16px',
                    borderRadius: '4px',
                    border: '1px solid #ddd',
                    backgroundColor: currentPage === 1 ? '#f5f5f5' : 'white',
                    cursor: currentPage === 1 ? 'default' : 'pointer'
                  }}
                >
                  Poprzednia
                </button>
                
                {[...Array(totalPages)].map((_, index) => (
                  <button
                    key={index + 1}
                    onClick={() => paginate(index + 1)}
                    style={{
                      padding: '8px 16px',
                      borderRadius: '4px',
                      border: '1px solid #ddd',
                      backgroundColor: currentPage === index + 1 ? '#8B4513' : 'white',
                      color: currentPage === index + 1 ? 'white' : 'black',
                      cursor: 'pointer'
                    }}
                  >
                    {index + 1}
                  </button>
                ))}
                
                <button
                  onClick={() => paginate(currentPage + 1)}
                  disabled={currentPage === totalPages}
                  style={{
                    padding: '8px 16px',
                    borderRadius: '4px',
                    border: '1px solid #ddd',
                    backgroundColor: currentPage === totalPages ? '#f5f5f5' : 'white',
                    cursor: currentPage === totalPages ? 'default' : 'pointer'
                  }}
                >
                  Następna
                </button>
              </div>
            </>
          ) : (
            <div>No cars found matching your search criteria</div>
          )}
        </div>
      </div>
    </div>
  );
}