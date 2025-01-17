import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Page } from '../layout/Page';
import { Button } from '../ui/Button';

// Updated mock data to include all three rentals
const MOCK_RENTALS = [
    {
        rentalId: 123456,
        status: 'gotowy do zwrotu',
        createdAt: '2024-01-10',
        offer: {
            offerId: 963852741,
            totalPrice: 299.99,
            startDate: '2024-01-10',
            endDate: '2024-01-15',
            hasGps: true,
            hasChildSeat: false,
            car: {
                carId: 987654,
                brand: 'Opel',
                model: 'Corsa E',
                licensePlate: 'WH 00001',
                year: 2022,
                location: 'Warsaw',
                fuelType: 'Gasoline',
                power: 150
            },
            customer: {
                firstName: 'John',
                lastName: 'Doe',
                email: 'john.doe@example.com'
            }
        }
    },
    {
        rentalId: 234577,
        status: 'w trakcie',
        createdAt: '2024-02-15',
        offer: {
            offerId: 963852742,
            totalPrice: 449.99,
            startDate: '2024-02-15',
            endDate: '2024-02-20',
            hasGps: true,
            hasChildSeat: true,
            car: {
                carId: 987655,
                brand: 'Polonez',
                model: 'Caro',
                licensePlate: 'WH 00002',
                year: 1995,
                location: 'Krakow',
                fuelType: 'Gasoline',
                power: 90
            },
            customer: {
                firstName: 'Jane',
                lastName: 'Smith',
                email: 'jane.smith@example.com'
            }
        }
    },
    {
        rentalId: 345478,
        status: 'w trakcie',
        createdAt: '2024-03-01',
        offer: {
            offerId: 963852743,
            totalPrice: 349.99,
            startDate: '2024-03-01',
            endDate: '2024-03-05',
            hasGps: false,
            hasChildSeat: true,
            car: {
                carId: 987656,
                brand: 'Skoda',
                model: 'Fabia',
                licensePlate: 'WH 00003',
                year: 2021,
                location: 'Gdansk',
                fuelType: 'Diesel',
                power: 110
            },
            customer: {
                firstName: 'Mike',
                lastName: 'Wilson',
                email: 'mike.wilson@example.com'
            }
        }
    }
];

export function RentalDetails() {
    const { rentalId } = useParams();
    const navigate = useNavigate();

    // Find the specific rental from mock data
    const rental = MOCK_RENTALS.find(r => r.rentalId === parseInt(rentalId));

    if (!rental) {
        return (
            <Page>
                <div className="text-center py-12">
                    <p className="text-gray-500">Nie znaleziono wypożyczenia.</p>
                </div>
            </Page>
        );
    }

    const handleReturn = () => {
        console.log(`Processing return for rental ${rentalId}`);
    };

    const handleBack = () => {
        navigate('/worker/rentals');
    };

    const getAdditionalServices = () => {
        const services = [];
        if (rental.offer.hasGps) services.push('GPS');
        if (rental.offer.hasChildSeat) services.push('Fotelik dziecięcy');
        return services;
    };

    return (
        <Page>
            <div className="max-w-4xl mx-auto">
                <div className="flex items-center gap-4 mb-6">
                    <button
                        onClick={handleBack}
                        className="p-2 hover:bg-gray-100 rounded-full"
                    >
                        <svg
                            width="24"
                            height="24"
                            viewBox="0 0 24 24"
                            fill="none"
                            stroke="currentColor"
                            strokeWidth="2"
                            strokeLinecap="round"
                            strokeLinejoin="round"
                        >
                            <path d="M19 12H5M12 19l-7-7 7-7" />
                        </svg>
                    </button>
                    <h1 className="text-2xl font-bold">Szczegóły wypożyczenia</h1>
                </div>

                <div className="bg-white rounded-lg shadow-sm overflow-hidden">
                    <div className="grid md:grid-cols-2 gap-6 p-6">
                        <div className="bg-gray-100 rounded-lg aspect-video flex items-center justify-center">
                            <img
                                src="/api/placeholder/800/600"
                                alt={`${rental.offer.car.brand} ${rental.offer.car.model}`}
                                className="w-full h-full object-cover rounded-lg"
                            />
                        </div>

                        <div className="space-y-6">
                            <h2 className="text-2xl font-bold">
                                {rental.offer.car.brand} {rental.offer.car.model}
                            </h2>

                            <div className="space-y-3">
                                <DetailRow
                                    label="Data wypożyczenia"
                                    value={rental.offer.startDate}
                                />
                                <DetailRow
                                    label="ID wynajmu"
                                    value={rental.rentalId}
                                />
                                <DetailRow
                                    label="ID samochodu"
                                    value={rental.offer.car.carId}
                                />
                                <DetailRow
                                    label="ID oferty"
                                    value={rental.offer.offerId}
                                />
                                <DetailRow
                                    label="Status wynajmu"
                                    value={rental.status}
                                />
                                <DetailRow
                                    label="Nr rejestracyjny"
                                    value={rental.offer.car.licensePlate}
                                />
                            </div>

                            {getAdditionalServices().length > 0 && (
                                <div className="space-y-2">
                                    <h3 className="font-medium">Usługi dodatkowe:</h3>
                                    <p>{getAdditionalServices().join(', ')}</p>
                                </div>
                            )}

                            {rental.status === 'gotowy do zwrotu' && (
                                <div className="pt-4">
                                    <Button
                                        onClick={handleReturn}
                                        className="w-full"
                                    >
                                        Odbierz zwrot
                                    </Button>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </Page>
    );
}

const DetailRow = ({ label, value }) => (
    <div className="flex items-center">
        <span className="text-gray-600 min-w-[160px]">{label}:</span>
        <span className="font-medium">{value}</span>
    </div>
);

export default RentalDetails;