'use client';

import { Meeting, Attendee } from '@/app/lib/definitions';
import { CheckIcon, ClockIcon, CurrencyDollarIcon, UserCircleIcon } from '@heroicons/react/24/outline';
import Link from 'next/link';
import { Button } from '@/app/ui/button';
import { updateMeeting } from '@/app/lib/actions';
import { useFormState } from 'react-dom';
import { useState, useEffect } from 'react';

export default function EditMeeting({ meeting }: { meeting: Meeting }) {
    const initialState = { message: null, errors: {} };
    //const updateMeetingWithId = updateMeeting.bind(null, meeting.id);
    const [state, dispatch] = useFormState(updateMeeting, initialState);
    const platformOptions = ["Google Meet"];

    const formatDateTimeLocal = (date: string | number | Date) => {
        if (!date) return ''; // early exit for undefined/null dates

        let d = new Date(date);
        // Adjust for Vietnam time UTC+7
        d.setMinutes(d.getMinutes() + d.getTimezoneOffset() + 420); // 420 minutes for UTC+7

        let year = d.getFullYear();
        let month = (d.getMonth() + 1).toString().padStart(2, '0');
        let day = d.getDate().toString().padStart(2, '0');
        let hours = d.getHours().toString().padStart(2, '0');
        let minutes = d.getMinutes().toString().padStart(2, '0');
        return `${year}-${month}-${day}T${hours}:${minutes}`;
    };

    const [attendees, setAttendees] = useState(meeting.attendees || []);
    const [starttime, setStarttime] = useState(formatDateTimeLocal(meeting.starttime));

    const handleAddAttendee = () => {
        setAttendees([...attendees, { name: '', email: '' }]);
    };

    const handleRemoveAttendee = (index: number) => {
        const newAttendees = [...attendees];
        newAttendees.splice(index, 1);
        setAttendees(newAttendees);
    };

    const handleAttendeeChange = (index: number, field: keyof Attendee, value: string) => {
        const newAttendees = [...attendees];
        newAttendees[index][field] = value;
        setAttendees(newAttendees);
    };

    // State to store the actor_id
    const [actor_id, setActorId] = useState('');
    const [access_token, setAccessToken] = useState('');

    // Retrieve actor_id and access_token from cookies
    useEffect(() => {
        const getCookie = (name: string) => {
            const value = `; ${document.cookie}`;
            const parts = value.split(`; ${name}=`);
            // Check if parts array is defined and has a length of 2
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
        const formData = {
            id: meeting.id,
            meeting_title: event.target.meeting_title.value,
            meeting_description: event.target.meeting_description.value,
            location: event.target.location.value,
            starttime: event.target.starttime.value,
            duration: event.target.duration.value,
            attendees,
            actor_id: actor_id,
            access_token: access_token
        };

        // Call createMeetingForm function with form data and actor_id
        //const result = await updateMeeting(state, formData);

        // Update state with result
        dispatch(formData);
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
                        <input
                            id="meeting_title"
                            name="meeting_title"
                            type="text"
                            placeholder="Enter meeting title"
                            defaultValue={meeting.meeting_title}
                            className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                            required
                        />
                    </div>
                </div>

                {/* Meeting Descriptions (optional) */}
                <div className="mb-4">
                    <label htmlFor="meeting_description" className="mb-2 block text-sm font-medium">
                        Write a description
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <input
                            id="meeting_description"
                            name="meeting_description"
                            type="text"
                            placeholder="Enter meeting description"
                            defaultValue={meeting.meeting_description}
                            className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        />
                    </div>
                </div>

                {/* Location (optional) */}
                <div className="mb-4">
                    <label htmlFor="location" className="mb-2 block text-sm font-medium">
                        Choose a location
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <input
                            id="location"
                            name="location"
                            type="text"
                            placeholder="Enter meeting location"
                            defaultValue={meeting.location}
                            className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        />
                    </div>
                </div>

                {/* Meeting platform */}
                <div className="mb-4">
                    <label htmlFor="platform" className="mb-2 block text-sm font-medium">
                        Meeting Platform
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <select
                            id="platform"
                            name="platform"
                            className="peer block w-full cursor-pointer rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                            defaultValue={platformOptions[meeting.platform]}
                            disabled
                        >
                            {platformOptions.map((option, index) => (
                                <option key={index} value={index}>{option}</option>
                            ))}
                        </select>
                    </div>
                </div>

                {/* Duration (minutes) */}
                <div className="mb-4">
                    <label htmlFor="duration" className="mb-2 block text-sm font-medium">
                        Enter your meeting duration (minutes)
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <input
                            id="duration"
                            name="duration"
                            type="number"
                            placeholder="Enter meeting duration"
                            defaultValue={meeting.duration}
                            className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                            required
                        />
                    </div>
                </div>

                {/* Start time */}
                <div className="mb-4">
                    <label htmlFor="starttime" className="mb-2 block text-sm font-medium">
                        Enter your meeting start time
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <input
                            id="starttime"
                            name="starttime"
                            type="datetime-local"
                            defaultValue={formatDateTimeLocal(meeting.starttime)}
                            onChange={(e) => setStarttime(e.target.value)}
                            className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                            required
                        />
                    </div>
                </div>

                {/* Attendees */}
                <div className="mb-4">
                    <label className="mb-2 block text-sm font-medium">
                        Attendees
                    </label>
                    {attendees.map((attendee, index) => (
                        <div key={index} className="mb-2 flex flex-col md:flex-row items-start md:items-center">
                            <input
                                type="text"
                                placeholder="Name"
                                value={attendee.name}
                                onChange={(e) => handleAttendeeChange(index, 'name', e.target.value)}
                                className="peer block w-full md:w-1/2 rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500 mb-2 md:mb-0"
                            />
                            <input
                                type="email"
                                placeholder="Email"
                                value={attendee.email}
                                onChange={(e) => handleAttendeeChange(index, 'email', e.target.value)}
                                className="peer block w-full md:w-1/2 rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                            />
                            <Button className="ml-4" type="button" onClick={() => handleRemoveAttendee(index)}>x</Button>
                        </div>
                    ))}
                    <Button type="button" onClick={handleAddAttendee}>Add Attendee</Button>
                </div>
            </div>

            {state.message && (
                <div className="text-red-500 mt-4">{state.message}</div>
            )}

            <div className="mt-6 flex justify-end gap-4">
                <Link
                    href="/dashboard/meetings"
                    className="flex h-10 items-center rounded-lg bg-gray-100 px-4 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-200"
                >
                    Cancel
                </Link>
                <Button type="submit">Edit Meeting</Button>
            </div>
        </form>
    );
}
