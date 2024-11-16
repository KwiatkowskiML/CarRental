import { useState } from 'react';

export function SearchBar({ onSearch }) {
  const [searchText, setSearchText] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    if (searchText.trim()) {
      onSearch(searchText);
    }
  };

  return (
    <form 
      onSubmit={handleSubmit}
      style={{
        display: 'flex',
        gap: '10px',
        marginBottom: '20px'
      }}
    >
      <input
        type="text"
        placeholder="Search by brand or model..."
        value={searchText}
        onChange={(e) => setSearchText(e.target.value)}
        style={{
          padding: '8px 12px',
          borderRadius: '4px',
          border: '1px solid #ddd',
          minWidth: '250px'
        }}
      />
      <button
        type="submit"
        disabled={!searchText.trim()}
        style={{
          padding: '8px 16px',
          borderRadius: '4px',
          border: 'none',
          backgroundColor: !searchText.trim() ? '#cccccc' : '#007bff',
          color: 'white',
          cursor: !searchText.trim() ? 'not-allowed' : 'pointer'
        }}
      >
        Search
      </button>
      {searchText && (
        <button
          type="button"
          onClick={() => {
            setSearchText('');
            onSearch('');
          }}
          style={{
            padding: '8px 16px',
            borderRadius: '4px',
            border: 'none',
            backgroundColor: '#6c757d',
            color: 'white',
            cursor: 'pointer'
          }}
        >
          Clear
        </button>
      )}
    </form>
  );
}