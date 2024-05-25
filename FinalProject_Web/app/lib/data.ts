import { sql } from '@vercel/postgres';
import {
  CustomerField,
  CustomersTableType,
  InvoiceForm,
  InvoicesTable,
  LatestInvoiceRaw,
  User,
  Revenue,
  MeetingForm,
} from './definitions';
import { formatCurrency } from './utils';
import { unstable_noStore as noStore } from 'next/cache';
import axios from 'axios';

const ITEMS_PER_PAGE = 5;

export async function fetchFilteredMeetingForms(
    query: string,
    currentPage: number
) {
    noStore();

    try {
        console.log(query)

        if (currentPage == null || currentPage < 1) currentPage = 1;

        const response = await fetch(`http://localhost:7057/meeting-form/search-form?actor_id=4efyqow4ywdutzb52oymalf5d&meeting_title=${query}&PageNumber=${currentPage}&PageSize=${ITEMS_PER_PAGE}`);
        const responseData = await response.json();

        // Extract the array of meetings from the response data
        const meetingForms = responseData.data;

        return meetingForms;
    }
    catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch meetings.');
    }
}

export async function fetchMeetingFormPages() {
    noStore();

    try {
        console.log("checkpoint1");
        // Make a request to your server API to fetch the total count of meeting forms
        const response = await fetch(`http://localhost:7057/meeting-form/get-all-form?actor_id=4efyqow4ywdutzb52oymalf5d`);
        console.log("checkpoint2");

        const responseData = await response.json();

        const count = responseData.data.length;

        const totalPages = Math.ceil(Number(count) / ITEMS_PER_PAGE);
        return totalPages;
    } catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch total number of invoices.');
    }
}

export async function fetchMeetingFormById(id: string) {
    noStore();

    try {
        // Make a request to your server API to fetch the meeting form by ID
        const response = await fetch(`http://localhost:7057/meeting-form/get-form/${id}?actor_id=4efyqow4ywdutzb52oymalf5d`);
        const responseData = await response.json();

        // Extract the array of invoices from the response data
        const meetingFormData = responseData.data;

        // Transforming the times array
        const timesData = meetingFormData.times.map(time => ({
            id: time.id,
            time: new Date(time.time),
        }));

        // Map the fetched data to the MeetingForm type definition
        const meetingForm: MeetingForm = {
            id: meetingFormData.id,
            meeting_title: meetingFormData.meeting_title,
            meeting_description: meetingFormData.meeting_description,
            location: meetingFormData.location,
            platform: meetingFormData.platform,
            duration: meetingFormData.duration,
            times: timesData,
            attendee: meetingFormData.attendee,
        };

        return meetingForm;

    } catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch meeting.');
    }
}

export async function fetchFilteredMeeting(
    query: string,
    currentPage: number
) {
    noStore();

    try {
        console.log(query)

        if (currentPage == null || currentPage < 1) currentPage = 1;

        const response = await fetch(`http://localhost:7057/meeting/search-meeting?actor_id=4efyqow4ywdutzb52oymalf5d&meeting_title=${query}&PageNumber=${currentPage}&PageSize=${ITEMS_PER_PAGE}`);
        const responseData = await response.json();

        // Extract the array of meetings from the response data
        const meetingForms = responseData.data;

        return meetingForms;
    }
    catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch meetings.');
    }
}

export async function fetchMeetingPages() {
    noStore();

    try {
        console.log("checkpoint1");
        // Make a request to your server API to fetch the total count of meeting forms
        const response = await fetch(`http://localhost:7057/meeting/get-all-meeting?actor_id=4efyqow4ywdutzb52oymalf5d`);
        console.log("checkpoint2");

        const responseData = await response.json();

        const count = responseData.data.length;

        const totalPages = Math.ceil(Number(count) / ITEMS_PER_PAGE);
        return totalPages;
    } catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch total number of invoices.');
    }
}

export async function fetchMeetingById(id: string) {
    noStore();

    try {
        // Make a request to your server API to fetch the meeting form by ID
        const response = await fetch(`http://localhost:7057/meeting/get-meeting/${id}?actor_id=4efyqow4ywdutzb52oymalf5d`);
        const responseData = await response.json();

        // Extract the array of invoices from the response data
        const meetingFormData = responseData.data;

        // Transforming the times array
        const timesData = meetingFormData.times.map(time => ({
            id: time.id,
            time: new Date(time.time),
        }));

        // Map the fetched data to the MeetingForm type definition
        const meetingForm: MeetingForm = {
            id: meetingFormData.id,
            meeting_title: meetingFormData.meeting_title,
            meeting_description: meetingFormData.meeting_description,
            location: meetingFormData.location,
            platform: meetingFormData.platform,
            duration: meetingFormData.duration,
            times: timesData,
            attendee: meetingFormData.attendee,
        };

        return meetingForm;

    } catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch meeting.');
    }
}

