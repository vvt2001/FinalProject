'use client';

import { CustomerField } from '@/app/lib/definitions';
import Link from 'next/link';
import {
  CheckIcon,
  ClockIcon,
  CurrencyDollarIcon,
  UserCircleIcon,
} from '@heroicons/react/24/outline';
import { Button } from '@/app/ui/button';
import { createMeetingForm } from '@/app/lib/actions';
import { useFormState } from 'react-dom';
import { useState } from 'react';

export default function Form() {
    const initialState = { message: null, errors: {} };
    const [state, dispatch] = useFormState(createMeetingForm, initialState);
    const platformOptions = ["Zoom", "Microsoft Teams", "Google Meet"];

    // State to store the selected datetime value
    const [selectedDateTime, setSelectedDateTime] = useState('');

    // Function to handle changes in the datetime input
    const handleDateTimeChange = (event) => {
        setSelectedDateTime(event.target.value);
    };

  return (
      <form action={dispatch}>
      <div className="rounded-md bg-gray-50 p-4 md:p-6">

        {/* Meeting Title */}
        <div className="mb-4">
            <label htmlFor="title" className="mb-2 block text-sm font-medium">
            Choose a title
            </label>
            <div className="relative mt-2 rounded-md">
            <div className="relative">
                <input
                id="title"
                name="title"
                type="string"
                step="0.01"
                placeholder="Enter meeting title"
                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                required
                />
            </div>
            </div>
        </div>

        {/* Meeting Descriptions (optional) */}
        <div className="mb-4">
            <label htmlFor="description" className="mb-2 block text-sm font-medium">
                Write a description
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="description"
                        name="description"
                        type="string"
                        step="0.01"
                        placeholder="Enter meeting description"
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        required
                    />
                </div>
            </div>
        </div>

        {/* Location (optional) */}
        <div className="mb-4">
            <label htmlFor="location" className="mb-2 block text-sm font-medium">
                Choose a location
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="location"
                        name="location"
                        type="string"
                        step="0.01"
                        placeholder="Enter meeting location"
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        required
                    />
                </div>
            </div>
        </div>

        {/* Meeting Platform */}
        <div className="mb-4">
            <label htmlFor="platform" className="mb-2 block text-sm font-medium">
                Choose platform
            </label>
            <div className="relative">
                <select
                    id="platform"
                    name="platformid"
                    className="peer block w-full cursor-pointer rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                    defaultValue=""
                    aria-describedby="customer-error"
                >
                    <option value="" disabled>
                        Select an online meeting platform
                    </option>
                    {platformOptions.map((option, index) => (
                        <option key={index} value={index}>
                            {option}
                        </option>
                    ))}
                </select>
            </div>
            <div id="customer-error" aria-live="polite" aria-atomic="true">
                {state.errors?.customerId &&
                    state.errors.customerId.map((error: string) => (
                        <p className="mt-2 text-sm text-red-500" key={error}>
                            {error}
                        </p>
                    ))}
            </div>
        </div>

        {/* Duration (minutes) */}
        <div className="mb-4">
            <label htmlFor="duration" className="mb-2 block text-sm font-medium">
                Choose a duration
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="duration"
                        name="duration"
                        type="text"
                        step="0.01"
                        placeholder="Enter meeting duration"
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        required
                    />
                </div>
            </div>
        </div>

        {/* Times */}
        <div className="mb-4">
          <label htmlFor="datetime" className="mb-2 block text-sm font-medium">
            Choose a date and time
          </label>
          <div className="relative mt-2 rounded-md">
            <div className="relative">
              <input
                id="datetime"
                name="datetime"
                type="datetime-local" // Change type to datetime-local
                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                value={selectedDateTime} // Bind value to state
                onChange={handleDateTimeChange} // Handle changes
                required
              />
            </div>
          </div>
        </div>

        {/*<button onClick={handleAddTime}>Add Time</button>*/}

      </div>
      <div className="mt-6 flex justify-end gap-4">
        <Link
          href="/dashboard/meetingforms"
          className="flex h-10 items-center rounded-lg bg-gray-100 px-4 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-200"
        >
          Cancel
        </Link>
        <Button type="submit">Create Invoice</Button>
      </div>
    </form>
  );
}
