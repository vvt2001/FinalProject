'use client';

import { MeetingForm } from '@/app/lib/definitions';
import { useState } from 'react';
import { Button } from '@/app/ui/button';
import Link from 'next/link';
import { voteMeetingForm } from '@/app/lib/actions';

export default function VoteMeetingForm({
    meetingform
}: {
    meetingform: MeetingForm;
}) {

    console.log(meetingform);
    const [selectedTimes, setSelectedTimes] = useState([]);
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const platformOptions = ["Zoom", "Microsoft Teams", "Google Meet"];

    const handleCheckboxChange = (timeId) => {
        if (selectedTimes.includes(timeId)) {
            setSelectedTimes(selectedTimes.filter((id) => id !== timeId));
        } else {
            setSelectedTimes([...selectedTimes, timeId]);
        }
    };

    const handleSubmit = (event) => {
        event.preventDefault(); // Prevent default form submission behavior

        // Prepare the request body
        const requestBody = {
            meetingform_id: meetingform.id,
            meetingtime_ids: selectedTimes,
            name: name,
            email: email
        };

        // Send request to vote on meeting times
        voteMeetingForm(requestBody);
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
                    {meetingform.times.map((time, index) => (
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
                                <input
                                    type="checkbox"
                                    id={`time-${index}`}
                                    checked={selectedTimes.includes(time.id)}
                                    onChange={() => handleCheckboxChange(time.id)}
                                    className="peer rounded-md border-gray-200 text-indigo-600 focus:ring-indigo-500 h-4 w-4 ml-4"
                                />
                            </div>
                        </div>
                    ))}
                </div>

                {/* Name */}
                <div className="mb-4">
                    <label htmlFor="name" className="mb-2 block text-sm font-medium">
                        Enter your name
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <div className="relative">
                            <input
                                id="name"
                                name="name"
                                type="string"
                                step="0.01"
                                placeholder="Enter your name"
                                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                                value={name}
                                onChange={(e) => setName(e.target.value)}
                                required
                            />
                        </div>
                    </div>
                </div>

                {/* Email */}
                <div className="mb-4">
                    <label htmlFor="email" className="mb-2 block text-sm font-medium">
                        Enter your email
                    </label>
                    <div className="relative mt-2 rounded-md">
                        <div className="relative">
                            <input
                                id="email"
                                name="email"
                                type="string"
                                step="0.01"
                                placeholder="Enter your email"
                                className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </div>
                    </div>
                </div>

                {/* Render submit button */}
                <div className="mt-6 flex justify-end gap-4">
                    <Link href="/dashboard/meetingforms" className="flex h-10 items-center rounded-lg bg-gray-100 px-4 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-200">
                        Cancel
                    </Link>
                    <Button type="submit">Vote</Button>
                </div>
            </div>
        </form>
    );
}
