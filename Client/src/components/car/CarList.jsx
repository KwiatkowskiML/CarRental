import { useState, useEffect, useMemo } from 'react';
import { useAuth } from '../../auth/AuthContext';
import { Page } from '../layout/Page';
import { SearchBar } from './CarSearch';
import { CarCard } from './CarCard';
import { Pagination } from './Pagination';

export function CarList() {
  const { user } = useAuth();
  const [cars, setCars] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const carsPerPage = 5;

  const fetchCars = async (filters = {}) => {
    setLoading(true);
    setError(null);

    try {
      const queryParams = new URLSearchParams();
      Object.entries(filters).forEach(([key, value]) => {
        if (value) {
          queryParams.append(key, value.toString());
        }
      });

      const headers = {};
      if (user?.token) {
        headers.Authorization = `Bearer ${user.token}`;
      }

      const response = await fetch(`/api/Cars?${queryParams.toString()}`, {
        headers
      });

      if (!response.ok) {
        throw new Error('Failed to fetch cars');
      }

      const data = await response.json();
      
      // Add unique identifiers for cars that don't have them
      const carsWithIds = data.map((car, index) => ({
        ...car,
        uniqueId: car.carId || `external-${car.carProvider.name}-${car.brand}-${car.model}-${index}`
      }));

      setCars(carsWithIds.filter(car => car.status === 'available'));
      setCurrentPage(1); // Reset to first page when new data is loaded
    } catch (err) {
      setError('Error fetching cars. Please try again.');
      console.error('Error fetching cars:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCars();
  }, [user]);

  const handleSearch = (filters) => {
    const apiFilters = {
      brand: filters.brand || '',
      model: filters.model || '',
      fuelType: filters.fuelType || '',
      location: filters.location || '',
      minYear: filters.year || null,
      maxYear: filters.year || null
    };

    fetchCars(apiFilters);
  };

  // Memoize the pagination calculations to avoid unnecessary recalculations
  const {
    currentCars,
    totalPages,
    indexOfFirstCar,
    indexOfLastCar
  } = useMemo(() => {
    const indexOfLastCar = currentPage * carsPerPage;
    const indexOfFirstCar = indexOfLastCar - carsPerPage;
    const currentCars = cars.slice(indexOfFirstCar, indexOfLastCar);
    const totalPages = Math.ceil(cars.length / carsPerPage);

    return {
      currentCars,
      totalPages,
      indexOfFirstCar,
      indexOfLastCar
    };
  }, [cars, currentPage, carsPerPage]);

  const paginate = (pageNumber) => {
    if (pageNumber < 1 || pageNumber > totalPages) return;
    setCurrentPage(pageNumber);
    window.scrollTo(0, 0);
  };

  return (
    <Page>
      <h1>Available Cars</h1>

      <SearchBar onSearch={handleSearch} />

      {error && (
        <div style={{
          padding: '12px',
          marginBottom: '20px',
          backgroundColor: '#fee2e2',
          color: '#dc2626',
          borderRadius: '4px'
        }}>
          {error}
        </div>
      )}

      <div style={{
        display: 'flex',
        flexDirection: 'column',
        gap: '20px',
        marginTop: '20px'
      }}>
        {loading ? (
          <div>Loading...</div>
        ) : currentCars.length > 0 ? (
          <>
            {currentCars.map(car => (
              <CarCard 
                key={car.uniqueId} 
                car={car}
              />
            ))}

            <Pagination
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={paginate}
            />

            <div style={{ 
              textAlign: 'center', 
              color: '#666', 
              marginTop: '10px',
              fontSize: '0.875rem'
            }}>
              Showing {indexOfFirstCar + 1} to {Math.min(indexOfLastCar, cars.length)} of {cars.length} cars
            </div>
          </>
        ) : (
          <div>No cars found matching your search criteria</div>
        )}
      </div>
    </Page>
  );
}