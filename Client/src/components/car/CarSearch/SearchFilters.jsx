import React from 'react';

export function SearchFilters({
  filters,
  availableFilters,
  onBrandChange,
  onModelChange,
  onYearChange,
  onFuelTypeChange,
  onLocationChange
}) {
  const selectStyle = {
    padding: '8px 12px',
    borderRadius: '4px',
    border: '1px solid #ddd',
    minWidth: '150px'
  };

  return (
    <div style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
      <select
        value={filters.brand}
        onChange={(e) => onBrandChange(e.target.value)}
        style={selectStyle}
      >
        <option value="">All Brands</option>
        {availableFilters.brands.map(brand => (
          <option key={brand} value={brand}>{brand}</option>
        ))}
      </select>

      <select
        value={filters.model}
        onChange={(e) => onModelChange(e.target.value)}
        disabled={!filters.brand}
        style={{
          ...selectStyle,
          backgroundColor: !filters.brand ? '#f5f5f5' : 'white'
        }}
      >
        <option value="">All Models</option>
        {availableFilters.models.map(model => (
          <option key={model} value={model}>{model}</option>
        ))}
      </select>

      <select
        value={filters.year}
        onChange={(e) => onYearChange(e.target.value)}
        style={selectStyle}
      >
        <option value="">All Years</option>
        {availableFilters.years.map(year => (
          <option key={year} value={year}>{year}</option>
        ))}
      </select>

      <select
        value={filters.fuelType}
        onChange={(e) => onFuelTypeChange(e.target.value)}
        style={selectStyle}
      >
        <option value="">All Fuel Types</option>
        {availableFilters.fuelTypes.map(fuel => (
          <option key={fuel} value={fuel}>{fuel}</option>
        ))}
      </select>

      <select
        value={filters.location}
        onChange={(e) => onLocationChange(e.target.value)}
        style={selectStyle}
      >
        <option value="">All Locations</option>
        {availableFilters.locations.map(location => (
          <option key={location} value={location}>{location}</option>
        ))}
      </select>
    </div>
  );
}