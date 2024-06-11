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
  Meeting,
} from './definitions';
import { formatCurrency } from './utils';
import { unstable_noStore as noStore } from 'next/cache';
import axios from 'axios';

const ITEMS_PER_PAGE = 5;

export async function fetchFilteredMeetingForms(
    query: string,
    currentPage: number,
    actor_id: any
) {
    noStore();

    try {

        if (currentPage == null || currentPage < 1) currentPage = 1;

        const response = await fetch(`http://localhost:7057/meeting-form/search-form?actor_id=${actor_id}&meeting_title=${query}&PageNumber=${currentPage}&PageSize=${ITEMS_PER_PAGE}`);
        const responseData = await response.json();

        // Extract the array of meetings from the response data
        const meetingForms = responseData.data;

        return meetingForms;
    }
    catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch meeting schedules.');
    }
}

export async function fetchMeetingFormPages(actor_id: any) {
    noStore();

    try {
        // Make a request to your server API to fetch the total count of meeting forms
        const response = await fetch(`http://localhost:7057/meeting-form/get-all-form?actor_id=${actor_id}`);

        const responseData = await response.json();

        const count = responseData.data.length;

        const totalPages = Math.ceil(Number(count) / ITEMS_PER_PAGE);
        return totalPages;
    } catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch total number of meeting schedules.');
    }
}

export async function fetchMeetingFormById(id: string, actor_id: any) {
    noStore();

    try {
        // Make a request to your server API to fetch the meeting form by ID
        const response = await fetch(`http://localhost:7057/meeting-form/get-form/${id}?actor_id=${actor_id}`);
        const responseData = await response.json();

        // Extract the array of invoices from the response data
        const meetingFormData = responseData.data;

        // Transforming the times array
        const timesData = meetingFormData.times.map((time: { id: any; time: string | number | Date; vote_count: any; }) => ({
            id: time.id,
            time: new Date(time.time),
            vote_count: time.vote_count
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
        };

        return meetingForm;

    } catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch meeting.');
    }
}

export async function fetchFilteredMeeting(
    query: string,
    currentPage: number,
    actor_id: any
) {
    noStore();

    try {

        if (currentPage == null || currentPage < 1) currentPage = 1;

        const response = await fetch(`http://localhost:7057/meeting/search-meeting?actor_id=${actor_id}&meeting_title=${query}&PageNumber=${currentPage}&PageSize=${ITEMS_PER_PAGE}`);
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

export async function fetchMeetingPages(actor_id: any) {
    noStore();

    try {
        // Make a request to your server API to fetch the total count of meeting forms
        const response = await fetch(`http://localhost:7057/meeting/get-all-meeting?actor_id=${actor_id}`);

        const responseData = await response.json();

        const count = responseData.data.length;

        const totalPages = Math.ceil(Number(count) / ITEMS_PER_PAGE);
        return totalPages;
    } catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch total number of meetings.');
    }
}

export async function fetchMeetingById(id: string, actor_id: any) {
    noStore();

    try {
        // Make a request to your server API to fetch the meeting form by ID
        const response = await fetch(`http://localhost:7057/meeting/get-meeting/${id}?actor_id=${actor_id}`);
        const responseData = await response.json();

        // Extract the array of invoices from the response data
        const meetingData = responseData.data;

        // Map the fetched data to the MeetingForm type definition
        const meetingForm: Meeting = {
            id: meetingData.id,
            meeting_title: meetingData.meeting_title,
            meeting_description: meetingData.meeting_description,
            location: meetingData.location,
            platform: meetingData.platform,
            duration: meetingData.duration,
            starttime: meetingData.starttime,
            attendees: meetingData.attendees,
        };

        return meetingForm;

    } catch (error) {
        console.error('Database Error:', error);
        throw new Error('Failed to fetch meeting.');
    }
}

