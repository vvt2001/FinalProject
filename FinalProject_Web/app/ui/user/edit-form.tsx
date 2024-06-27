'use client';

import { User } from '@/app/lib/definitions';
import { CheckIcon, ClockIcon, CurrencyDollarIcon, UserCircleIcon } from '@heroicons/react/24/outline';
import Link from 'next/link';
import { Button } from '@/app/ui/button';
import { updateUser, RemoveCredentials, AddCredentials } from '@/app/lib/actions';
import { useFormState } from 'react-dom';
import { useState, useEffect } from 'react';

export default function EditUser({ user }: { user: User }) {
    const initialState = { message: null, errors: {} };
    const [state, dispatch] = useFormState(updateUser, initialState);

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
        formData.append('id', user.id);

        //// Call createMeetingForm function with form data and actor_id
        //const result = await updateMeetingForm( state, formData);

        // Update state with result
        dispatch(formData);
    };
    console.log(user.has_googlecredentials);
    // Function to handle Google credentials
    const handleGoogleCredentials = async () => {
        try {
            let result;
            if (user.has_googlecredentials) {
                result = await RemoveCredentials(actor_id, access_token);
            } else {
                result = await AddCredentials(actor_id, access_token);
            }

            if (result.message) {
                console.log(result.message);
                // Optionally update the UI or state here based on the result
                // For example, show a success message or handle errors
                alert(result.message);
            } else if (result.error) {
                console.error('Error:', result.error);
                alert('An error occurred. Please try again.');
            }
        } catch (error) {
            console.error('Error:', error);
            alert('An error occurred. Please try again.');
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <div className="rounded-md bg-gray-50 p-4 md:p-6">

                {/* Name */}
                <div className="mb-4">
                    <label htmlFor="name" className="mb-2 block text-sm font-medium">
                        Your name
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <input
                            id="name"
                            name="name"
                            type="text"
                            placeholder="Enter a name"
                            defaultValue={user.name}
                            className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                            required
                        />
                    </div>
                </div>

                {/* Email */}
                <div className="mb-4">
                    <label htmlFor="email" className="mb-2 block text-sm font-medium">
                        Email
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <input
                            id="email"
                            name="email"
                            type="email"
                            placeholder="Enter an email"
                            defaultValue={user.email}
                            className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                            required
                        />
                    </div>
                </div>
            </div>

            <div className="mt-6 flex justify-end gap-4">
                <Link
                    href="/dashboard"
                    className="flex h-10 items-center rounded-lg bg-gray-100 px-4 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-200"
                >
                    Cancel
                </Link>
                <Button type="submit">Edit</Button>
                <Button type="button" onClick={handleGoogleCredentials}>
                    {user.has_googlecredentials ? 'Remove Google Credentials' : 'Add Google Credentials'}
                </Button>
            </div>
        </form>
    );
}
