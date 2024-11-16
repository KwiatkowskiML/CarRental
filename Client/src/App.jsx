import { useState, useEffect } from 'react'

function App() {
  const [cars, setCars] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetch('/api/cars')
      .then(res => res.json())
      .then(data => {
        setCars(data)
        setLoading(false)
      })
      .catch(error => {
        console.error('Error fetching cars:', error)
        setLoading(false)
      })
  }, [])

  if (loading) return <div>Loading...</div>

  return (
    <div style={{ padding: '20px' }}>
      <h1>Available Cars</h1>
      <div style={{ 
        display: 'grid', 
        gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
        gap: '20px'
      }}>
        {cars.map(car => (
          <div key={car.carId} style={{
            border: '1px solid #ddd',
            padding: '15px',
            borderRadius: '8px'
          }}>
            <h3>{car.brand} {car.model}</h3>
            <p>Year: {car.year}</p>
            <p>Location: {car.location}</p>
            <p>Fuel Type: {car.fuelType}</p>
          </div>
        ))}
      </div>
    </div>
  )
}

export default App