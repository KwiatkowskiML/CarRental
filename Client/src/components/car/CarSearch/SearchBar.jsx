import { useState, useEffect } from 'react';
import { SearchFilters } from './SearchFilters';
import { Button } from '../../ui/Button';

export function SearchBar({ onSearch }) {
  const [searchText, setSearchText] = useState('');
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
    const fetchFilterOptions = async () => {
      try {
        const response = await fetch('/api/Cars/filter-options');
        if (response.ok) {
          const data = await response.json();
          setAvailableFilters({
            brands: data.brands,
            models: filters.brand
              ? data.models.filter(model =>
                data.cars?.some(car =>
                  car.brand === filters.brand && car.model === model
                )
              )
              : data.models,
            years: data.years,
            fuelTypes: data.fuelTypes,
            locations: data.locations
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
    let finalFilters = { ...filters };

    if (searchText.trim()) {
      const words = searchText.trim().split(/\s+/);

      if (words.length === 1) {
        const matchingBrand = availableFilters.brands.find(brand =>
          brand.toLowerCase().includes(words[0].toLowerCase())
        );
        if (matchingBrand) {
          finalFilters.brand = matchingBrand;
        } else {
          finalFilters.model = words[0];
        }
      }
      else if (words.length > 1) {
        finalFilters.brand = words[0];
        finalFilters.model = words.slice(1).join(' ');
      }
    }

    if (filters.brand) {
      finalFilters.brand = filters.brand;
      finalFilters.model = searchText.trim();
    }

    if (finalFilters.year) {
      finalFilters.minYear = parseInt(finalFilters.year);
      finalFilters.maxYear = parseInt(finalFilters.year);
      delete finalFilters.year;
    }

    onSearch(finalFilters);
  };

  const handleClear = () => {
    setSearchText('');
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
    return Object.values(filters).some(value => value !== '') || searchText.trim() !== '';
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
      <input
        type="text"
        placeholder="Search by brand or model"
        value={searchText}
        onChange={(e) => setSearchText(e.target.value)}
        style={{
          padding: '12px',
          borderRadius: '4px',
          border: '1px solid #ddd',
          width: '100%',
          fontSize: '16px'
        }}
      />

      <SearchFilters
        filters={filters}
        availableFilters={availableFilters}
        onBrandChange={(brand) => {
          setFilters(prev => ({ ...prev, brand, model: '' }));
          setSearchText('');
        }}
        onModelChange={(model) => {
          setFilters(prev => ({ ...prev, model }));
          setSearchText('');
        }}
        onYearChange={(year) => setFilters(prev => ({ ...prev, year }))}
        onFuelTypeChange={(fuelType) => setFilters(prev => ({ ...prev, fuelType }))}
        onLocationChange={(location) => setFilters(prev => ({ ...prev, location }))}
      />

      <div style={{ display: 'flex', gap: '10px' }}>
        <Button type="submit" variant="primary">
          Search
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