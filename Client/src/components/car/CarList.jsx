import { useState, useEffect } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import { SearchBar } from './CarSearch';
import { CarCard } from './CarCard';
import { Pagination } from './Pagination';

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
    <Page>
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
              <CarCard key={car.carId} car={car} />
            ))}
            
            <Pagination
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={paginate}
            />
          </>
        ) : (
          <div>No cars found matching your search criteria</div>
        )}
      </div>
    </Page>
  );
}