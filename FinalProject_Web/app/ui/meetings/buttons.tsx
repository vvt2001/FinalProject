// @ts-ignore
'use client';

import { XMarkIcon, TrashIcon, PencilIcon } from '@heroicons/react/24/outline';
import Link from 'next/link';
import { deleteMeeting, cancelMeeting } from '@/app/lib/actions';
import { useState, useEffect } from 'react';
import React from 'react';

export function DeleteMeeting({ id }: { id: string }) {
    //const deleteMeetingWithId = deleteMeeting.bind(null, id);
    //const initialState = { message: null, errors: {} };
    //const [state, dispatch] = useFormState(deleteMeeting, initialState);

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
    const handleSubmit = async (event: { preventDefault: () => void; }) => {
        event.preventDefault(); // Prevent default form submission

        // Create FormData object
        //const formData = new FormData(event.target);

        // Call createMeetingForm function with form data and actor_id
        const result = await deleteMeeting(id, actor_id, access_token);

        // Update state with result
        //    dispatch(result);
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

export function CancelMeeting({ id }: { id: string }) {
    //const cancelMeetingWithId = cancelMeeting.bind(null, id);
    //const initialState = { message: null, errors: {} };
    //const [state, dispatch] = useFormState(cancelMeeting, initialState);

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
    const handleSubmit = async (event: { preventDefault: () => void; }) => {
        event.preventDefault(); // Prevent default form submission

        // Create FormData object
        //const formData = new FormData(event.target);

        // Call createMeetingForm function with form data and actor_id
        const result = await cancelMeeting(id, actor_id, access_token);

        // Update state with result
        //    dispatch(result);
    };
    return (
        <form onSubmit={handleSubmit}>
            <button className="rounded-md border p-2 hover:bg-gray-100">
                <span className="sr-only">Delete</span>
                <XMarkIcon className="w-4" />
            </button>
        </form>
    );
}

export function UpdateMeeting({ id }: { id: string }) {
    return (
        <Link
            href={`/dashboard/meetings/${id}/edit`}
            className="rounded-md border p-2 hover:bg-gray-100"
        >
            <PencilIcon className="w-5" />
        </Link>
    );
}
