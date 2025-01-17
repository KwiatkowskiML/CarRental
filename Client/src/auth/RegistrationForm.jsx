import React, { useState } from 'react';
import { Button } from '../components/ui/Button';

export function RegistrationForm({ googleData, onRegistrationComplete }) {
  const [formData, setFormData] = useState({
    firstName: googleData.firstName || '',
    lastName: googleData.lastName || '',
    age: '',
    drivingLicenseYears: ''
  });
  const [errors, setErrors] = useState({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const validateForm = () => {
    const newErrors = {};
    if (!formData.firstName.trim()) newErrors.firstName = 'First name is required';
    if (!formData.lastName.trim()) newErrors.lastName = 'Last name is required';
    if (!formData.age) newErrors.age = 'Age is required';
    else if (formData.age < 18) newErrors.age = 'Must be at least 18 years old';
    if (!formData.drivingLicenseYears) newErrors.drivingLicenseYears = 'Years of driving license is required';
    else if (formData.drivingLicenseYears < 0) newErrors.drivingLicenseYears = 'Invalid number of years';
    else if (formData.drivingLicenseYears > formData.age - 18) {
      newErrors.drivingLicenseYears = 'Cannot exceed years since turning 18';
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;
    
    setIsSubmitting(true);
    try {
      const response = await fetch('/api/Auth/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: googleData.email,
          firstName: formData.firstName,
          lastName: formData.lastName,
          age: parseInt(formData.age),
          drivingLicenseYears: parseInt(formData.drivingLicenseYears),
          googleToken: googleData.token
        })
      });

      if (!response.ok) throw new Error('Registration failed');
      
      const data = await response.json();
      if (data.token) {
        localStorage.setItem('token', data.token);
        onRegistrationComplete();
      }
    } catch (error) {
      console.error('Registration error:', error);
      setErrors({ submit: 'Registration failed. Please try again.' });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const inputClassName = (error) => `
    w-full p-3 border rounded-lg bg-white
    focus:outline-none focus:ring-2 focus:ring-brown-500
    ${error ? 'border-red-500' : 'border-gray-300'}
  `;

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full bg-white rounded-xl shadow-lg p-8">
        <div className="text-center mb-8">
          <h2 className="text-3xl font-bold text-gray-900 mb-2">Complete Your Profile</h2>
          <p className="text-gray-600">Please provide additional information to complete your registration</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">First Name</label>
            <input
              type="text"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              className={inputClassName(errors.firstName)}
              placeholder="Enter your first name"
            />
            {errors.firstName && (
              <p className="mt-1 text-sm text-red-500">{errors.firstName}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Last Name</label>
            <input
              type="text"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              className={inputClassName(errors.lastName)}
              placeholder="Enter your last name"
            />
            {errors.lastName && (
              <p className="mt-1 text-sm text-red-500">{errors.lastName}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Age</label>
            <input
              type="number"
              name="age"
              value={formData.age}
              onChange={handleChange}
              min="18"
              className={inputClassName(errors.age)}
              placeholder="Enter your age"
            />
            {errors.age && (
              <p className="mt-1 text-sm text-red-500">{errors.age}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Years of Having a Driver's License
            </label>
            <input
              type="number"
              name="drivingLicenseYears"
              value={formData.drivingLicenseYears}
              onChange={handleChange}
              min="0"
              className={inputClassName(errors.drivingLicenseYears)}
              placeholder="Enter years of experience"
            />
            {errors.drivingLicenseYears && (
              <p className="mt-1 text-sm text-red-500">{errors.drivingLicenseYears}</p>
            )}
          </div>

          {errors.submit && (
            <div className="bg-red-50 p-4 rounded-lg">
              <p className="text-sm text-red-500 text-center">{errors.submit}</p>
            </div>
          )}

          <Button
            type="submit"
            style={{
              width: '100%',
              marginTop: '24px',
              opacity: isSubmitting ? 0.7 : 1,
              cursor: isSubmitting ? 'not-allowed' : 'pointer',
            }}
            disabled={isSubmitting}
          >
            {isSubmitting ? 'Creating Account...' : 'Complete Registration'}
          </Button>
        </form>
      </div>
    </div>
  );
}

export default RegistrationForm;