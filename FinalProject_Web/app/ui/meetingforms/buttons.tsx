// @ts-ignore
'use client';

import { PencilIcon, PlusIcon, TrashIcon, LinkIcon, CalendarIcon  } from '@heroicons/react/24/outline';
import Link from 'next/link';
import { deleteMeetingForm } from '@/app/lib/actions';
import { useState, useEffect } from 'react';
import React from 'react';
import { useFormState } from 'react-dom';

export function CreateMeetingForm() {
  return (
    <Link
      href="/dashboard/meetingforms/create"
      className="flex h-10 items-center rounded-lg bg-blue-600 px-4 text-sm font-medium text-white transition-colors hover:bg-blue-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600"
    >
      <span className="hidden md:block">Create Schedule</span>{' '}
      <PlusIcon className="h-5 md:ml-4" />
    </Link>
  );
}

export function UpdateMeetingForm({ id }: { id: string }) {
  return (
      <Link
          href={`/dashboard/meetingforms/${id}/edit`}
          className="rounded-md border p-2 hover:bg-gray-100"
    >
      <PencilIcon className="w-5" />
    </Link>
  );
}

export function BookMeeting({ id }: { id: string }) {
    return (
        <Link
            href={`/dashboard/meetingforms/${id}/book`}
            className="rounded-md border p-2 hover:bg-gray-100"
        >
            <CalendarIcon className="w-5" />
        </Link>
    );
}

export function VoteMeetingForm({ id }: { id: string }) {
    return (
        <Link
            href={`/dashboard/meetingforms/${id}/vote`}
            className="rounded-md border p-2 hover:bg-gray-100"
        >
            <PencilIcon className="w-5" />
        </Link>
    );
}

export function CopyVoteUrl({ url }: { url: string }) {
    const [copySuccess, setCopySuccess] = useState(false);

    const copyToClipboard = async () => {
        if (navigator.clipboard) {
            try {
                await navigator.clipboard.writeText(url);
                setCopySuccess(true);
                setTimeout(() => {
                    setCopySuccess(false);
                }, 1000); // Reset copy success message after 1 second
            } catch (error) {
                console.error('Failed to copy URL:', error);
            }
        } else {
            // Fallback method for older browsers
            const textArea = document.createElement('textarea');
            textArea.value = url;
            document.body.appendChild(textArea);
            textArea.select();
            try {
                document.execCommand('copy');
                setCopySuccess(true);
                setTimeout(() => {
                    setCopySuccess(false);
                }, 1000); // Reset copy success message after 1 second
            } catch (error) {
                console.error('Fallback: Failed to copy URL:', error);
            } finally {
                document.body.removeChild(textArea);
            }
        }
    };

    return (
        <div style={{ position: 'relative', display: 'inline-block' }}>
            <button
                onClick={copyToClipboard}
                className="rounded-md border p-2 hover:bg-gray-100"
            >
                <LinkIcon className="w-5" />
            </button>
            {copySuccess && (
                <div
                    className="bg-gray-200 text-gray-800 rounded-md p-1 absolute bottom-full left-1/2 transform -translate-x-1/2 mb-2"
                    style={{ maxWidth: '100px' }}
                >
                    Copied URL
                </div>
            )}
        </div>
    );
}


export function DeleteMeetingForm({ id }: { id: string }) {
    //const deleteMeetingFormWithId = deleteMeetingForm.bind(null, id);
    //const initialState = { message: null, errors: {} };
    //const [state, dispatch] = useFormState(deleteMeetingForm, initialState);

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
    const handleSubmit = async (event: { preventDefault: () => void }) => {
        event.preventDefault(); // Prevent default form submission

        // Create FormData object
        //const formData = new FormData(event.target);

        // Call createMeetingForm function with form data and actor_id
        const result = await deleteMeetingForm(id, actor_id, access_token);

        // Update state with result
/*        dispatch(result);*/
    };
    return (
        <form onSubmit={handleSubmit}>
            <button className="rounded-md border p-2 hover:bg-gray-100">
                <span className="sr-only">Delete</span>
                <TrashIcon className="w-4" />
            </button>
        </form>
  );
}