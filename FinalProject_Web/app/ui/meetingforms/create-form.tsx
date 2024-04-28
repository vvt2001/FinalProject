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

export default function Form({ customers }: { customers: CustomerField[] }) {
    const initialState = { message: null, errors: {} };
    const [state, dispatch] = useFormState(createMeetingForm, initialState);

    const handleAddTime = () => {
        const newTimes = [...times];
        newTimes.push('');
        setTimes(newTimes);
    };

    const handleTimeChange = (index: number, value: string) => {
        const newTimes = [...times];
        newTimes[index] = value;
        setTimes(newTimes);
    };
  return (
      <form action={dispatch}>
      <div className="rounded-md bg-gray-50 p-4 md:p-6">

        {/* Meeting Title */}
        <div className="mb-4">
            <label htmlFor="amount" className="mb-2 block text-sm font-medium">
            Choose an amount
            </label>
            <div className="relative mt-2 rounded-md">
            <div className="relative">
                <input
                id="amount"
                name="amount"
                type="number"
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
            <label htmlFor="amount" className="mb-2 block text-sm font-medium">
                Choose an amount
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="amount"
                        name="amount"
                        type="number"
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
            <label htmlFor="amount" className="mb-2 block text-sm font-medium">
                Choose an amount
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="amount"
                        name="amount"
                        type="number"
                        step="0.01"
                        placeholder="Enter meeting location"
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        required
                    />
                </div>
            </div>
        </div>

        {/* Customer Name */}
        <div className="mb-4">
            <label htmlFor="customer" className="mb-2 block text-sm font-medium">
                Choose platform
            </label>
            <div className="relative">
                <select
                    id="customer"
                    name="customerId"
                    className="peer block w-full cursor-pointer rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                    defaultValue=""
                    aria-describedby="customer-error"
                >
                    <option value="" disabled>
                        Select an online meeting platform
                    </option>
                    {customers.map((customer) => (
                        <option key={customer.id} value={customer.id}>
                            {customer.name}
                        </option>
                    ))}
                </select>
                <UserCircleIcon className="pointer-events-none absolute left-3 top-1/2 h-[18px] w-[18px] -translate-y-1/2 text-gray-500" />
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
            <label htmlFor="amount" className="mb-2 block text-sm font-medium">
                Choose an amount
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="amount"
                        name="amount"
                        type="number"
                        step="0.01"
                        placeholder="Enter meeting duration"
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        required
                    />
                </div>
            </div>
        </div>

        {times.map((time, index) => (
            <div key={index}>
                <label>
                    Time:
                    <input type="datetime-local" value={time} onChange={(e) => handleTimeChange(index, e.target.value)} />
                </label>
                <br />
            </div>
        ))}
        <button onClick={handleAddTime}>Add Time</button>

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
