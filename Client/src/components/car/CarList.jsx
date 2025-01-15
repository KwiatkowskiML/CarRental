import { useState, useEffect } from 'react';
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

      if (filters.brand) {
        queryParams.append('brand', filters.brand);
      }
      if (filters.model) {
        queryParams.append('model', filters.model);
      }
      if (filters.minYear) {
        queryParams.append('minYear', filters.minYear.toString());
      }
      if (filters.maxYear) {
        queryParams.append('maxYear', filters.maxYear.toString());
      }
      if (filters.fuelType) {
        queryParams.append('fuelType', filters.fuelType);
      }
      if (filters.location) {
        queryParams.append('location', filters.location);
      }

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
      setCars(data.filter(car => car.status === 'available'));
      setCurrentPage(1);
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
      minYear: filters.year ? parseInt(filters.year) : null,
      maxYear: filters.year ? parseInt(filters.year) : null
    };

    fetchCars(apiFilters);
  };

  const indexOfLastCar = currentPage * carsPerPage;
  const indexOfFirstCar = indexOfLastCar - carsPerPage;
  const currentCars = cars.slice(indexOfFirstCar, indexOfLastCar);
  const totalPages = Math.ceil(cars.length / carsPerPage);

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