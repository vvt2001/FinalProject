'use client';

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
import { useState, useEffect, ChangeEvent } from 'react';
import { MeetingFormState, User } from '@/app/lib/definitions';

export default function Form({ user }: { user: User }) {
    const initialState = { message: null, errors: {} };
    const [state, dispatch] = useFormState(createMeetingForm, initialState);
    const platformOptions = ["Google Meet"];

    // State to store the selected datetime values
    const [selectedDateTimes, setSelectedDateTimes] = useState<string[]>([]);

    // Function to handle changes in the datetime input
    const handleDateTimeChange = (index: number, event: ChangeEvent<HTMLInputElement>) => {
        const newDateTimes: string[] = [...selectedDateTimes];
        newDateTimes[index] = event.target.value;
        setSelectedDateTimes(newDateTimes);
    };

    // Function to add a new datetime input
    const addDateTimeInput = () => {
        setSelectedDateTimes([...selectedDateTimes, '']);
    };

    // Function to remove a datetime input
    const removeDateTimeInput = (index: number) => {
        const newDateTimes = [...selectedDateTimes];
        newDateTimes.splice(index, 1);
        setSelectedDateTimes(newDateTimes);
    };

    // State to store the actor_id
    const [actor_id, setActorId] = useState('');
    const [access_token, setAccessToken] = useState('');

    // Retrieve actor_id and access_token from cookies
    useEffect(() => {
        const getCookie = (name: string) => {
            const value = `; ${document.cookie}`;
            const parts = value.split(`; ${name}=`);
            if (parts != undefined && parts.length === 2) {
                return parts.pop()?.split(';').shift();
            }

            // Return undefined if parts is undefined or length is not equal to 2
            return undefined;
        };

        const actorIdFromCookie = getCookie("actor_id");
        const accessTokenFromCookie = getCookie("access_token");

        setActorId(actorIdFromCookie || '');
        setAccessToken(accessTokenFromCookie || '');
    }, []);

    // Function to handle form submission
    const handleSubmit = async (event: any) => {
        event.preventDefault(); // Prevent default form submission

        // Create FormData object
        const formData = new FormData(event.target);
        formData.append('actor_id', actor_id);
        formData.append('access_token', access_token);

        //// Call createMeetingForm function with form data and actor_id
        //const result = await createMeetingForm(state, formData);

        if (user.has_googlecredentials)
            // Update state with result
            dispatch(formData);
        else {
            //this part will handle the add credentials

            const clientId = process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID;
            //const redirectUri = 'http://scheduler.id.vn:7057/api/auth/google/callback'; // API endpoint
            const redirectUri = 'http://localhost:7057/api/auth/google/callback/create-meeting'; // API endpoint
            const scope = encodeURIComponent('https://www.googleapis.com/auth/calendar');
            const responseType = 'code';
            const state = encodeURIComponent(JSON.stringify({
                //meeting_title: formData.get('meeting_title'),
                //meeting_description: formData.get('meeting_description'),
                //location: formData.get('location'),
                //times: formData.getAll('times'),
                //duration: parseInt(formData.get('duration')?.toString() || '0', 10),
                //platform: parseInt(formData.get('platform')?.toString() || '0', 10),
                meeting_title: formData.get('meeting_title'),
                meeting_description: formData.get('meeting_description'),
                location: formData.get('location'),
                times: formData.getAll('times'),
                duration: formData.get('duration'),
                platform: formData.get('platform'),
                user_id: formData.get('actor_id'),
            })); // Encode the user ID to ensure it is safely included in the URL

            const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?client_id=${clientId}&redirect_uri=${redirectUri}&response_type=${responseType}&scope=${scope}&state=${state}&access_type=offline&prompt=consent`;

            window.location.href = authUrl;
        }
    };

    return (
    <form onSubmit={handleSubmit}>
      <div className="rounded-md bg-gray-50 p-4 md:p-6">

        {/* Meeting Title */}
        <div className="mb-4">
            <label htmlFor="meeting_title" className="mb-2 block text-sm font-medium">
                Choose a title
            </label>
            <div className="relative mt-2 rounded-md">
            <div className="relative">
                <input
                id="meeting_title"
                name="meeting_title"
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
            <label htmlFor="meeting_description" className="mb-2 block text-sm font-medium">
                Write a description
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="meeting_description"
                        name="meeting_description"
                        type="string"
                        step="0.01"
                        placeholder="Enter meeting description"
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
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
                    name="platform"
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
        </div>

        {/* Duration (minutes) */}
        <div className="mb-4">
            <label htmlFor="duration" className="mb-2 block text-sm font-medium">
                Meeting duration (minutes)
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

        <Button className="mb-4" type="button" onClick={addDateTimeInput}>Add meeting times</Button>
        {/* Render datetime inputs */}
        {selectedDateTimes.map((times, index) => (
            <div key={index} className="mb-4">
                <label htmlFor={`times-${index}`} className="mb-2 block text-sm font-medium">
                    Choose a date and time
                </label>
                <div className= "flex flex-col md:flex-row items-start md:items-center">
                <div className="relative mt-2 rounded-md">
                        <div className="relative">
                            <input
                                id={`times-${index}`}
                                name={`times`}
                                type="datetime-local" // Change type to datetime-local
                                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                                value={times} // Bind value to state
                                onChange={(event) => handleDateTimeChange(index, event)} // Handle changes
                                required
                            />
                        </div>
                    </div>
                    <Button className="ml-4" type="button" onClick={() => removeDateTimeInput(index)}>x</Button>
                </div>
            </div>
        ))}

      </div>
            {state.message && (
                <div className="text-red-500 mt-4">{state.message}</div>
            )}
      <div className="mt-6 flex justify-end gap-4">
        <Link
          href="/dashboard/meetingforms"
          className="flex h-10 items-center rounded-lg bg-gray-100 px-4 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-200"
        >
          Cancel
        </Link>
        <Button type="submit">Create Schedule</Button>
      </div>
    </form>
  );
}
