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
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 5;
  const [currentFilters, setCurrentFilters] = useState({});

  const fetchCars = async (filters = {}, page = 1) => {
    setLoading(true);
    setError(null);

    try {
      const queryParams = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString()
      });

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
      setCars(data.cars);
      setTotalCount(data.totalCount);
      setTotalPages(data.totalPages);
      setCurrentPage(data.currentPage);
    } catch (err) {
      setError('Error fetching cars. Please try again.');
      console.error('Error fetching cars:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCars(currentFilters, currentPage);
  }, [currentPage, user]);

  const handleSearch = (filters) => {
    setCurrentFilters(filters);
    setCurrentPage(1);
    fetchCars(filters, 1);
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
    window.scrollTo(0, 0);
  };

  return (
    <Page>
      <h1 className="text-2xl font-bold mb-6">Available Cars</h1>

      <SearchBar onSearch={handleSearch} />

      {error && (
        <div className="p-3 mb-5 bg-red-100 text-red-700 rounded-md">
          {error}
        </div>
      )}

      <div className="flex flex-col gap-5 mt-5">
        {loading ? (
          <div>Loading...</div>
        ) : cars.length > 0 ? (
          <>
            {cars.map(car => (
              <CarCard key={car.carId} car={car} />
            ))}

            {totalPages > 1 && (
              <div className="mt-6">
                <Pagination
                  currentPage={currentPage}
                  totalPages={totalPages}
                  onPageChange={handlePageChange}
                />
              </div>
            )}

            <div className="text-sm text-gray-600 text-center mt-4">
              Showing {Math.min(pageSize * (currentPage - 1) + 1, totalCount)} to {Math.min(pageSize * currentPage, totalCount)} of {totalCount} cars
            </div>
          </>
        ) : (
          <div className="text-center py-8 text-gray-600">
            No cars found matching your search criteria
          </div>
        )}
      </div>
    </Page>
  );
}