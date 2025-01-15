import { useState, useEffect } from 'react';
import { SearchFilters } from './SearchFilters';
import { Button } from '../../ui/Button';

export function SearchBar({ onSearch }) {
  const [filters, setFilters] = useState({
    brand: '',
    model: '',
    year: '',
    fuelType: '',
    location: ''
  });

  const [availableFilters, setAvailableFilters] = useState({
    brands: [],
    models: [],
    years: [],
    fuelTypes: [],
    locations: []
  });

  useEffect(() => {
    // Fetch initial filter options from the API
    const fetchFilterOptions = async () => {
      try {
        const response = await fetch('/api/Cars');
        if (response.ok) {
          const cars = await response.json();

          setAvailableFilters({
            brands: [...new Set(cars.map(car => car.brand))].sort(),
            models: [...new Set(cars
              .filter(car => !filters.brand || car.brand === filters.brand)
              .map(car => car.model))
            ].sort(),
            years: [...new Set(cars.map(car => car.year))].sort((a, b) => b - a),
            fuelTypes: [...new Set(cars.map(car => car.fuelType))].sort(),
            locations: [...new Set(cars.map(car => car.location).filter(Boolean))].sort()
          });
        }
      } catch (error) {
        console.error('Error fetching filter options:', error);
      }
    };

    fetchFilterOptions();
  }, [filters.brand]);

  const handleSubmit = (e) => {
    e.preventDefault();
    onSearch(filters);
  };

  const handleClear = () => {
    setFilters({
      brand: '',
      model: '',
      year: '',
      fuelType: '',
      location: ''
    });
    onSearch({});
  };

  const isAnyFilterActive = () => {
    return Object.values(filters).some(value => value !== '');
  };

  return (
    <form
      onSubmit={handleSubmit}
      style={{
        display: 'flex',
        flexDirection: 'column',
        gap: '15px',
        marginBottom: '20px',
        padding: '20px',
        backgroundColor: '#f8f9fa',
        borderRadius: '8px'
      }}
    >
      <SearchFilters
        filters={filters}
        availableFilters={availableFilters}
        onBrandChange={(brand) => setFilters(prev => ({ ...prev, brand, model: '' }))}
        onModelChange={(model) => setFilters(prev => ({ ...prev, model }))}
        onYearChange={(year) => setFilters(prev => ({ ...prev, year }))}
        onFuelTypeChange={(fuelType) => setFilters(prev => ({ ...prev, fuelType }))}
        onLocationChange={(location) => setFilters(prev => ({ ...prev, location }))}
      />

      <div style={{ display: 'flex', gap: '10px' }}>
        <Button type="submit" variant="primary">
          Apply Filters
        </Button>
        {isAnyFilterActive() && (
          <Button type="button" variant="secondary" onClick={handleClear}>
            Clear All
          </Button>
        )}
      </div>
    </form>
  );
}