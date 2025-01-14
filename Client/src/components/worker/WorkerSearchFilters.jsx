import React from 'react';

export function WorkerSearchFilters({
    filters,
    onBrandChange,
    onModelChange,
    availableBrands,
    availableModels,
}) {
    return (
        <div style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
            <select
                value={filters.brand}
                onChange={(e) => onBrandChange(e.target.value)}
                style={{
                    padding: '8px 12px',
                    borderRadius: '4px',
                    border: '1px solid #ddd',
                    minWidth: '150px'
                }}
            >
                <option value="">All Brands</option>
                {availableBrands.map(brand => (
                    <option key={brand} value={brand}>{brand}</option>
                ))}
            </select>

            <select
                value={filters.model}
                onChange={(e) => onModelChange(e.target.value)}
                disabled={!filters.brand}
                style={{
                    padding: '8px 12px',
                    borderRadius: '4px',
                    border: '1px solid #ddd',
                    minWidth: '150px',
                    backgroundColor: !filters.brand ? '#f5f5f5' : 'white'
                }}
            >
                <option value="">All Models</option>
                {availableModels.map(model => (
                    <option key={model} value={model}>{model}</option>
                ))}
            </select>
        </div>
    );
}