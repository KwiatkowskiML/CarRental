import { useState, useEffect } from 'react';
import { SearchFilters } from './SearchFilters';
import { Button } from '../../ui/Button';

export function SearchBar({ cars, onSearch }) {
  const [searchText, setSearchText] = useState('');
  const [selectedBrand, setSelectedBrand] = useState('');
  const [selectedModel, setSelectedModel] = useState('');
  const [selectedYear, setSelectedYear] = useState('');
  const [selectedFuelType, setSelectedFuelType] = useState('');
  const [powerRange, setPowerRange] = useState({
    min: '',
    max: ''
  });

  useEffect(() => {
    setSelectedModel('');
  }, [selectedBrand]);

  const handleSubmit = (e) => {
    e.preventDefault();
    
    const filters = {
      searchText,
      brand: selectedBrand,
      model: selectedModel,
      year: selectedYear,
      fuelType: selectedFuelType,
      power: {
        min: powerRange.min ? parseInt(powerRange.min) : null,
        max: powerRange.max ? parseInt(powerRange.max) : null
      }
    };

    onSearch(filters);
  };

  const handleClear = () => {
    setSearchText('');
    setSelectedBrand('');
    setSelectedModel('');
    setSelectedYear('');
    setSelectedFuelType('');
    setPowerRange({ min: '', max: '' });
    
    onSearch({
      searchText: '',
      brand: '',
      model: '',
      year: '',
      fuelType: '',
      power: { min: null, max: null }
    });
  };

  const isAnyFilterActive = () => {
    return searchText || selectedBrand || selectedModel || selectedYear || 
           selectedFuelType || powerRange.min || powerRange.max;
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
        placeholder="Search..."
        value={searchText}
        onChange={(e) => setSearchText(e.target.value)}
        style={{
          padding: '8px 12px',
          borderRadius: '4px',
          border: '1px solid #ddd',
          width: '100%'
        }}
      />

      <SearchFilters
        cars={cars}
        selectedBrand={selectedBrand}
        selectedModel={selectedModel}
        selectedYear={selectedYear}
        selectedFuelType={selectedFuelType}
        powerRange={powerRange}
        onBrandChange={setSelectedBrand}
        onModelChange={setSelectedModel}
        onYearChange={setSelectedYear}
        onFuelTypeChange={setSelectedFuelType}
        onPowerRangeChange={setPowerRange}
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