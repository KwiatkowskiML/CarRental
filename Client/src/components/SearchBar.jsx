import { useState, useEffect } from 'react';

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

  const uniqueBrands = [...new Set(cars.map(car => car.brand))].sort();
  const uniqueModels = [...new Set(cars
    .filter(car => !selectedBrand || car.brand === selectedBrand)
    .map(car => car.model))
  ].sort();
  const uniqueYears = [...new Set(cars.map(car => car.year))].sort((a, b) => b - a);
  const uniqueFuelTypes = [...new Set(cars.map(car => car.fuelType))].sort();

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
      <div style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
        <input
          type="text"
          placeholder="Search..."
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          style={{
            padding: '8px 12px',
            borderRadius: '4px',
            border: '1px solid #ddd',
            flexGrow: 1,
            minWidth: '200px'
          }}
        />

        <select
          value={selectedBrand}
          onChange={(e) => setSelectedBrand(e.target.value)}
          style={{
            padding: '8px 12px',
            borderRadius: '4px',
            border: '1px solid #ddd',
            minWidth: '150px'
          }}
        >
          <option value="">All Brands</option>
          {uniqueBrands.map(brand => (
            <option key={brand} value={brand}>{brand}</option>
          ))}
        </select>

        <select
          value={selectedModel}
          onChange={(e) => setSelectedModel(e.target.value)}
          disabled={!selectedBrand}
          style={{
            padding: '8px 12px',
            borderRadius: '4px',
            border: '1px solid #ddd',
            minWidth: '150px',
            backgroundColor: !selectedBrand ? '#f5f5f5' : 'white'
          }}
        >
          <option value="">All Models</option>
          {uniqueModels.map(model => (
            <option key={model} value={model}>{model}</option>
          ))}
        </select>

        <select
          value={selectedYear}
          onChange={(e) => setSelectedYear(e.target.value)}
          style={{
            padding: '8px 12px',
            borderRadius: '4px',
            border: '1px solid #ddd',
            minWidth: '100px'
          }}
        >
          <option value="">All Years</option>
          {uniqueYears.map(year => (
            <option key={year} value={year}>{year}</option>
          ))}
        </select>

        <select
          value={selectedFuelType}
          onChange={(e) => setSelectedFuelType(e.target.value)}
          style={{
            padding: '8px 12px',
            borderRadius: '4px',
            border: '1px solid #ddd',
            minWidth: '150px'
          }}
        >
          <option value="">All Fuel Types</option>
          {uniqueFuelTypes.map(fuel => (
            <option key={fuel} value={fuel}>{fuel}</option>
          ))}
        </select>
      </div>

      <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
        <span>Power range (HP):</span>
        <input
          type="number"
          placeholder="Min"
          value={powerRange.min}
          onChange={(e) => setPowerRange(prev => ({ ...prev, min: e.target.value }))}
          style={{
            padding: '8px 12px',
            borderRadius: '4px',
            border: '1px solid #ddd',
            width: '100px'
          }}
          min="0"
        />
        <span>-</span>
        <input
          type="number"
          placeholder="Max"
          value={powerRange.max}
          onChange={(e) => setPowerRange(prev => ({ ...prev, max: e.target.value }))}
          style={{
            padding: '8px 12px',
            borderRadius: '4px',
            border: '1px solid #ddd',
            width: '100px'
          }}
          min="0"
        />
      </div>

      <div style={{ display: 'flex', gap: '10px' }}>
        <button
          type="submit"
          style={{
            padding: '8px 16px',
            borderRadius: '4px',
            border: 'none',
            backgroundColor: '#007bff',
            color: 'white',
            cursor: 'pointer'
          }}
        >
          Apply Filters
        </button>
        {isAnyFilterActive() && (
          <button
            type="button"
            onClick={handleClear}
            style={{
              padding: '8px 16px',
              borderRadius: '4px',
              border: 'none',
              backgroundColor: '#6c757d',
              color: 'white',
              cursor: 'pointer'
            }}
          >
            Clear All
          </button>
        )}
      </div>
    </form>
  );
}