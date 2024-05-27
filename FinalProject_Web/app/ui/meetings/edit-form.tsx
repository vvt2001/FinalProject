'use client';

import { Meeting } from '@/app/lib/definitions';
import {
  CheckIcon,
  ClockIcon,
  CurrencyDollarIcon,
  UserCircleIcon,
} from '@heroicons/react/24/outline';
import Link from 'next/link';
import { Button } from '@/app/ui/button';
import { updateMeeting } from '@/app/lib/actions';
import { useFormState } from 'react-dom';
import { useState } from 'react';

export default function EditMeeting({
  meeting
}: {
  meeting: Meeting;
    }) {
    const initialState = { message: null, errors: {} };
    const updateInvoiceWithId = updateMeeting.bind(null, meeting.id);
    const [state, dispatch] = useFormState(updateInvoiceWithId, initialState);
    const platformOptions = ["Zoom", "Microsoft Teams", "Google Meet"];

    const formatDateTimeLocal = (date) => {
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

  return (
      <form action={dispatch}>
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
                defaultValue={meeting.meeting_title}
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
                        defaultValue={meeting.meeting_description}
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
                        defaultValue={meeting.location}
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                    />
                </div>
            </div>
        </div>

        {/* Meeting platform */}
        <div className="mb-4">
            <label htmlFor="platform" className="mb-2 block text-sm font-medium">
                Meeting Platform
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="platform"
                        name="platform"
                        type="string"
                        step="0.01"
                        defaultValue={platformOptions[meeting.platform]}
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        disabled
                    />
                </div>
            </div>
        </div>

        {/* Duration (minutes) */}
        <div className="mb-4">
            <label htmlFor="duration" className="mb-2 block text-sm font-medium">
                Enter your meeting's duration
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="duration"
                        name="duration"
                        type="text"
                        step="0.01"
                        placeholder="Enter meeting duration"
                        defaultValue={meeting.duration}
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        required
                    />
                </div>
            </div>
        </div>

        {/* Start time */}
        <div className="mb-4">
            <label htmlFor="starttime" className="mb-2 block text-sm font-medium">
                Enter your meeting's start time
            </label>
            <div className="relative mt-2 rounded-md">
                <div className="relative">
                    <input
                        id="starttime"
                        name="starttime"
                        type="datetime-local"
                        step="0.01"
                        placeholder="Enter meeting start time"
                        defaultValue={formatDateTimeLocal(meeting.starttime)}
                        className="peer block w-full rounded-md border border-gray-200 py-2 pl-10 text-sm outline-2 placeholder:text-gray-500"
                        required
                    />
                </div>
            </div>
        </div>
      </div>
      <div className="mt-6 flex justify-end gap-4">
        <Link
          href="/dashboard/meetingforms"
          className="flex h-10 items-center rounded-lg bg-gray-100 px-4 text-sm font-medium text-gray-600 transition-colors hover:bg-gray-200"
        >
          Cancel
        </Link>
        <Button type="submit">Edit Meeting</Button>
      </div>
    </form>
  );
}
