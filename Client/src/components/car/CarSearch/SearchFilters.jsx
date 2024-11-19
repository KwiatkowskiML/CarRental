import React from 'react';

export function SearchFilters({ 
  cars, 
  selectedBrand,
  selectedModel,
  selectedYear,
  selectedFuelType,
  powerRange,
  onBrandChange,
  onModelChange,
  onYearChange,
  onFuelTypeChange,
  onPowerRangeChange 
}) {
  const uniqueBrands = [...new Set(cars.map(car => car.brand))].sort();
  const uniqueModels = [...new Set(cars
    .filter(car => !selectedBrand || car.brand === selectedBrand)
    .map(car => car.model))
  ].sort();
  const uniqueYears = [...new Set(cars.map(car => car.year))].sort((a, b) => b - a);
  const uniqueFuelTypes = [...new Set(cars.map(car => car.fuelType))].sort();

  return (
    <div style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
      <select
        value={selectedBrand}
        onChange={(e) => onBrandChange(e.target.value)}
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
        onChange={(e) => onModelChange(e.target.value)}
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
        onChange={(e) => onYearChange(e.target.value)}
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
        onChange={(e) => onFuelTypeChange(e.target.value)}
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

      <div style={{ display: 'flex', gap: '10px', alignItems: 'center' }}>
        <span>Power range (HP):</span>
        <input
          type="number"
          placeholder="Min"
          value={powerRange.min}
          onChange={(e) => onPowerRangeChange({ ...powerRange, min: e.target.value })}
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
          onChange={(e) => onPowerRangeChange({ ...powerRange, max: e.target.value })}
          style={{
            padding: '8px 12px',
            borderRadius: '4px',
            border: '1px solid #ddd',
            width: '100px'
          }}
          min="0"
        />
      </div>
    </div>
  );
}