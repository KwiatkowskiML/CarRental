import React, { useState } from 'react';
import { Button } from '../ui/Button';

export function SearchForm({ onSearch }) {
    const [searchQuery, setSearchQuery] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        const [brand, model] = searchQuery.split(' ').filter(Boolean);
        onSearch({ brand, model });
    };

    return (
        <form onSubmit={handleSubmit} className="flex flex-col sm:flex-row gap-4 mb-6">
            <input
                type="text"
                placeholder="Search by brand and/or model (e.g. 'Mercedes C300')"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="flex-1 px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-brown-500"
            />
            <Button type="submit" className="px-6">
                Search
            </Button>
        </form>
    );
}