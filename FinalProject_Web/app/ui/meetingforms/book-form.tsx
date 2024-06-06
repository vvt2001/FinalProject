'use client';

import { MeetingForm } from '@/app/lib/definitions';
import { useState, useEffect } from 'react';
import { Button } from '@/app/ui/button';
import Link from 'next/link';
import { bookMeetingForm } from '@/app/lib/actions';

interface Time { id: any; time: string | number | Date; vote_count: any; }

export default function BookMeetingForm({
    meetingform
}: {
    meetingform: MeetingForm;
}) {
    console.log(meetingform.times);
    const [selectedTimes, setSelectedTimes] = useState([]);
    const platformOptions = ["Zoom", "Microsoft Teams", "Google Meet"];

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

    const handleSubmit = (event: { preventDefault: () => void; }) => {
        event.preventDefault(); // Prevent default form submission behavior

        // Prepare the request body
        const requestBody = {
            meetingform_id: meetingform.id,
        };

        // Send request to book meeting
        bookMeetingForm(requestBody, actor_id, access_token);
    };

    return (
        <form onSubmit={handleSubmit}>
            <div className="rounded-md bg-gray-50 p-4 md:p-6">
                {/* Meeting title */}
                <div className="mb-4">
                    <label htmlFor="meeting_title" className="mb-2 block text-sm font-medium">
                        Title
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <div className="relative">
                            <input
                                id="meeting_title"
                                name="meeting_title"
                                type="string"
                                step="0.01"
                                defaultValue={meetingform.meeting_title}
                                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                                disabled
                            />
                        </div>
                    </div>
                </div>

                {/* Meeting Descriptions (optional) */}
                <div className="mb-4">
                    <label htmlFor="meeting_description" className="mb-2 block text-sm font-medium">
                        Description
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <div className="relative">
                            <input
                                id="meeting_description"
                                name="meeting_description"
                                type="string"
                                step="0.01"
                                placeholder="None"
                                defaultValue={meetingform.meeting_description}
                                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                                disabled
                            />
                        </div>
                    </div>
                </div>

                {/* Location (optional) */}
                <div className="mb-4">
                    <label htmlFor="location" className="mb-2 block text-sm font-medium">
                        Location
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <div className="relative">
                            <input
                                id="location"
                                name="location"
                                type="string"
                                step="0.01"
                                placeholder="None"
                                defaultValue={meetingform.location}
                                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                                disabled
                            />
                        </div>
                    </div>
                </div>

                {/* Meeting platform */}
                <div className="mb-4">
                    <label htmlFor="platform" className="mb-2 block text-sm font-medium">
                        Meeting platform
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <div className="relative">
                            <input
                                id="platform"
                                name="platform"
                                type="string"
                                step="0.01"
                                placeholder="Enter meeting platform"
                                defaultValue={platformOptions[meetingform.platform]}
                                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                                disabled
                            />
                        </div>
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
                                type="string"
                                step="0.01"
                                placeholder="Enter meeting duration"
                                defaultValue={meetingform.duration}
                                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                                disabled
                            />
                        </div>
                    </div>
                </div>
                {/* Render select list with checkboxes for meeting times */}
                <div className="mb-4">
                    <label className="mb-2 block text-sm font-medium">
                        Vote for a meeting time
                    </label>
                    {meetingform.times.map((time: Time, index) => (
                        <div key={index} className="mb-4">
                            <div className="flex items-center">
                                <div className="w-10 h-10 flex items-center justify-center rounded-md bg-gray-200 mr-4">
                                    <span className="text-sm font-medium">{time.vote_count}</span>
                                </div>
                                <div className="relative mt-2 rounded-md">
                                    <div className="relative">
                                        <label htmlFor={`time-${index}`} className="peer inline-block w-80 rounded-md border border-gray-200 py-2 pl-10 pr-2 text-sm outline-2 placeholder:text-gray-500 overflow-hidden">
                                            {new Date(time.time).toLocaleString()}
                                        </label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

                {/* Render submit button */}
                <div className="mt-6 flex justify-end gap-4">
                    <Link href="/dashboard/meetingforms" className="flex h-10 items-center rounded-lg bg-gray-100 px-4 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-200">
                        Cancel
                    </Link>
                    <Button type="submit">Book</Button>
                </div>
            </div>
        </form>
    );
}
