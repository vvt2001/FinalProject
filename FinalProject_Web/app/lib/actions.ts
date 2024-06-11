'use server';

import { z } from 'zod';
import { sql } from '@vercel/postgres';
import { revalidatePath } from 'next/cache';
import { redirect } from 'next/navigation';
import { signIn } from '@/auth';
import { AuthError } from 'next-auth';
import { AccountState, MeetingFormState, MeetingState } from './definitions';
//import { cookies } from "next/headers";

const MeetingFormSchema = z.object({
    id: z.string(),
    meeting_title: z.string({
        invalid_type_error: 'Please select a title.',
    }),
    meeting_description: z.string(),
    location: z.string(),
    duration: z.coerce
        .number()
        .gt(0, { message: 'Please enter an amount greater than 0.' }),
    platform: z.coerce.number({
        invalid_type_error: 'Please select a meeting platform.',
    }),
    times: z.array(
        z.coerce.date({
            invalid_type_error: 'Please select a date and time for the meeting.',
        })
    ),
});

const MeetingSchema = z.object({
    id: z.string(),
    meeting_title: z.string({
        invalid_type_error: 'Please select a title.',
    }),
    meeting_description: z.string(),
    location: z.string(),
    duration: z.coerce
        .number()
        .gt(0, { message: 'Please enter an amount greater than 0.' }),
    starttime: z.coerce.date({
        invalid_type_error: 'Please select a date and time for the meeting.',
    }),
    attendees: z.array(
        z.coerce.date({
            invalid_type_error: 'Please select a date and time for the meeting.',
        })
    ),
});


const VotingFormSchema = z.object({
    meetingform_id: z.string({
        invalid_type_error: 'Please select a title.',
    }),
    meetingtime_ids: z.array(
        z.string({
            invalid_type_error: 'Please vote atleast one date and time for the meeting.',
        })
    ),
    name: z.string(),
    email: z.string(),
});

const BookingFormSchema = z.object({
    meetingform_id: z.string({
        invalid_type_error: 'Please select a title.',
    }),
});

const CreateMeetingForm = MeetingFormSchema.omit({ id: true });
const UpdateMeetingForm = MeetingFormSchema.omit({ id: true });
const VoteMeetingForm = VotingFormSchema;
const BookMeetingForm = BookingFormSchema;
const UpdateMeeting = MeetingSchema.omit({ id: true });

//const cookieStore = cookies();

//const actor_id = cookieStore.get("actor_id")?.value;
//const access_token = cookieStore.get("access_token")?.value;
//console.log(actor_id);
//console.log(access_token);

//export type MeetingFormState = {
//    errors?: {
//        meeting_title?: string[];
//        meeting_description?: string[];
//        location?: string[];
//        duration?: string[];
//        platform?: string[];
//        times?: string[];
//    };
//    message?: string | null;
//};

//export type MeetingState = {
//    errors?: {
//        meeting_title?: string[];
//        meeting_description?: string[];
//        location?: string[];
//        duration?: string[];
//        platform?: string[];
//        starttime?: string[];
//        attendees?: string[];
//    };
//    message?: string | null;
//};

//export type AccountState = {
//    errors?: {
//        name?: string[];
//        email?: string[];
//        username?: string[];
//        password?: string[];
//        confirm_password?: string[];
//    };
//    message?: string | null;
//};

export async function createMeetingForm(prevState: MeetingFormState, formData: FormData) {
    console.log("actionside");
    console.log(formData);
    // Validate form using Zod
    const validatedFields = CreateMeetingForm.safeParse({
        meeting_title: formData.get('meeting_title'),
        meeting_description: formData.get('meeting_description'),
        location: formData.get('location'),
        times: formData.getAll('times'),
        duration: formData.get('duration'),
        platform: formData.get('platform'),
    });

    // If form validation fails, return errors early. Otherwise, continue.
    if (!validatedFields.success) {
        return {
            errors: validatedFields.error.flatten().fieldErrors,
            message: 'Missing Fields. Failed to Creat meeting schedule.',
        };
    }
    let response;
    // Insert data into the database
    try {
        // Make a POST request to your server API endpoint
        const { meeting_title, meeting_description, location, times, duration, platform } = validatedFields.data;
        const actor_id = formData.get('actor_id');
        const access_token = formData.get('access_token');

        response = await fetch(`http://localhost:7057/meeting-form/create-form?actor_id=${actor_id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
                Authorization: `Bearer ${access_token}` 
            },
            body: JSON.stringify({
                //meeting_title: formData.get('meeting_title'),
                //meeting_description: formData.get('meeting_description'),
                //location: formData.get('location'),
                //times: formData.getAll('times'),
                //duration: parseInt(formData.get('duration')?.toString() || '0', 10),
                //platform: parseInt(formData.get('platform')?.toString() || '0', 10),
                meeting_title: meeting_title,
                meeting_description: meeting_description,
                location: location,
                times: times,
                duration: duration,
                platform: platform,
            }),
        });
        if (!response.ok) {
            const errorData = await response.json();
            console.error('Create meeting schedule error:', errorData);
            return {
                ...prevState,
                message: `Create meeting schedule error: ${errorData.detail || response.statusText}`,
                errors: errorData.errors || {},
            };
        }

    } catch (error) {

        // Handle any errors that occur during the request
        console.error('Create meeting schedule error:', error);
        return {
            ...prevState,
            message: 'Create meeting schedule error.',
        };
    }
    if (response.ok) {
        // Revalidate the cache for the invoices page and redirect the user.
        revalidatePath('/dashboard');
        redirect('/dashboard');
        return {
            ...prevState,
            message: 'Meeting schedule created successfully.',
            errors: {},
        };
    }
    // Return the default state if no conditions are met
    return prevState;
}

export async function voteMeetingForm(requestBody: { meetingform_id: any; meetingtime_ids: any; name: any; email: any; }) {
    // Validate form using Zod
    const validatedFields = VoteMeetingForm.safeParse({
        meetingform_id: requestBody.meetingform_id,
        meetingtime_ids: requestBody.meetingtime_ids,
        name: requestBody.name,
        email: requestBody.email,
    });

    // If form validation fails, return errors early. Otherwise, continue.
    if (!validatedFields.success) {
        return {
            errors: validatedFields.error.flatten().fieldErrors,
            message: 'Missing Fields. Failed to Create Meeting Schedule.',
        };
    }
    let response;

    // Insert data into the database
    try {
        // Make a POST request to your server API endpoint
        //const { meetingform_id, meetingtime_ids, name, email } = validatedFields.data;

        response = await fetch(`http://localhost:7057/meeting-form/vote-form`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
            },
            body: JSON.stringify({
                meetingform_id: requestBody.meetingform_id,
                meetingtime_ids: requestBody.meetingtime_ids,
                name: requestBody.name,
                email: requestBody.email,
            }),
        });

    } catch (error) {

        // Handle any errors that occur during the request
        console.error('Creat meeting schedule error:', error);
        return {
            message: 'Failed to Creat meeting schedule.',
        };
    }
    if (response.ok) {
        // Revalidate the cache for the invoices page and redirect the user.
        revalidatePath('/dashboard');
        redirect('/dashboard');
    }
}

export async function bookMeetingForm(requestBody: { meetingform_id: any; }, actor_id: any, access_token: any) {
    // Validate form using Zod
    const validatedFields = BookMeetingForm.safeParse({
        meetingform_id: requestBody.meetingform_id,
    });

    // If form validation fails, return errors early. Otherwise, continue.
    if (!validatedFields.success) {
        return {
            errors: validatedFields.error.flatten().fieldErrors,
            message: 'Missing Fields. Failed to Book Meeting.',
        };
    }
    let response;
    // Insert data into the database
    try {

        // Make a POST request to your server API endpoint
        //const { meetingform_id } = validatedFields.data;
        response = await fetch(`http://localhost:7057/meeting-form/book-meeting?actor_id=${actor_id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
                Authorization: `Bearer ${access_token}` 
            },
            body: JSON.stringify({
                meetingform_id: requestBody.meetingform_id,
            }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.error('Book Meeting error:', errorData);
            return {
                message: `Book Meeting error: ${errorData.detail || response.statusText}`,
                errors: errorData.errors || {},
            };
        }

    } catch (error) {

        // Handle any errors that occur during the request
        console.error('Book Meeting error:', error);
        return {
            message: 'Failed to Book Meeting.',
        };
    }
    if (response.ok) {
        // Revalidate the cache for the invoices page and redirect the user.
        revalidatePath('/dashboard');
        redirect('/dashboard');
    }
}

export async function updateMeetingForm(
    prevState: MeetingFormState,
    formData: FormData,
) {
    // Validate form using Zod
    const validatedFields = UpdateMeetingForm.safeParse({
        meeting_title: formData.get('meeting_title'),
        meeting_description: formData.get('meeting_description'),
        location: formData.get('location'),
        times: formData.getAll('times'),
        duration: formData.get('duration'),
        platform: formData.get('platform'),
    });

    // If form validation fails, return errors early. Otherwise, continue.
    if (!validatedFields.success) {
        return {
            errors: validatedFields.error.flatten().fieldErrors,
            message: 'Missing Fields. Failed to Update Meeting Form.',
        };
    }
    let response;
    try {
        // Make a PUT request to your server API endpoint
        const { meeting_title, meeting_description, location, times, duration, platform } = validatedFields.data;
        const actor_id = formData.get('actor_id');
        const access_token = formData.get('access_token');
        const id = formData.get('id');

        response = await fetch(`http://localhost:7057/meeting-form/update-form?actor_id=${actor_id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
                Authorization: `Bearer ${access_token}` 
            },
            body: JSON.stringify({
                //id: id,
                //meeting_title: formData.get('meeting_title'),
                //meeting_description: formData.get('meeting_description'),
                //location: formData.get('location'),
                //times: formData.getAll('times'),
                //duration: parseInt(formData.get('duration')?.toString() || '0', 10),
                //platform: parseInt(formData.get('platform')?.toString() || '0', 10),
                id: id,
                meeting_title: meeting_title,
                meeting_description: meeting_description,
                location: location,
                times: times,
                duration: duration,
                platform: platform,
            }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.error('Update Meeting Schedule error:', errorData);
            return {
                ...prevState,
                message: `Update Meeting Schedule error: ${errorData.detail || response.statusText}`,
                errors: errorData.errors || {},
            };
        }

    } catch (error) {
        // Handle any errors that occur during the request
        console.error('Update meeting schedule error:', error);
        return {
            ...prevState,
            message: 'Update meeting schedule error.',
        };
    }
    if (response.ok) {
        revalidatePath('/dashboard');
        redirect('/dashboard');
        return {
            ...prevState,
            message: 'Meeting schedule created successfully.',
            errors: {},
        };
    }

    return prevState;
}

export async function deleteMeetingForm(id: string, actor_id: any, access_token: any) {
    const apiUrl = `http://localhost:7057/meeting-form/delete-form/${id}?actor_id=${actor_id}`; 

    try {
        // Make an HTTP DELETE request to your delete API endpoint
        const response = await fetch(apiUrl, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if required
                Authorization: `Bearer ${access_token}` 
            },
        });

        if (response.ok) {
            // Optionally handle any revalidation or additional logic after successful deletion
            revalidatePath('/dashboard/meetingforms');

            return { message: 'Deleted Meeting Form.' };
        } else {
            // If response status is not successful, parse error response
            const errorResponse = await response.json();
            console.error('Delete API Error:', errorResponse);
            return { message: 'Failed to delete Meeting Form.', error: errorResponse };
        }

    } catch (error) {
        console.error('API Request Error:', error);
        return { message: 'Failed to delete Meeting Form.' };
    }
}

export async function deleteMeeting(id: string, actor_id: any, access_token: any) {
    const apiUrl = `http://localhost:7057/meeting/delete-meeting/${id}?actor_id=${actor_id}`;
    try {
        // Make an HTTP DELETE request to your delete API endpoint
        const response = await fetch(apiUrl, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if required
                Authorization: `Bearer ${access_token}`
            },
        });

        if (response.ok) {
            // Optionally handle any revalidation or additional logic after successful deletion
            revalidatePath('/dashboard/meetings');

            return { message: 'Deleted Meeting.' };
        } else {
            // If response status is not successful, parse error response
            const errorResponse = await response.json();
            console.error('Delete API Error:', errorResponse);
            return { message: 'Failed to delete Meeting.', error: errorResponse };
        }

    } catch (error) {
        console.error('API Request Error:', error);
        return { message: 'Failed to delete Meeting.' };
    }
}

export async function cancelMeeting(id: string, actor_id: any, access_token: any) {
    const apiUrl = `http://localhost:7057/meeting/cancel-meeting/${id}?actor_id=${actor_id}`;
    try {
        // Make an HTTP PUT request to your cancel API endpoint
        const response = await fetch(apiUrl, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if required
                Authorization: `Bearer ${access_token}`
            },
        });

        if (response.ok) {
            // Optionally handle any revalidation or additional logic after successful deletion
            revalidatePath('/dashboard/meetings');

            return { message: 'Canceled Meeting.' };
        } else {
            // If response status is not successful, parse error response
            const errorResponse = await response.json();
            console.error('Cancel API Error:', errorResponse);
            return { message: 'Failed to cancel Meeting.', error: errorResponse };
        }

    } catch (error) {
        console.error('API Request Error:', error);
        return { message: 'Failed to cancel Meeting.' };
    }
}

export async function updateMeeting(
    prevState: MeetingState,
    formData: {
        id: any,
        meeting_title: any,
        meeting_description: any,
        location: any,
        starttime: any,
        duration: any,
        attendees: any,
        actor_id: any,
        access_token: any
    },
) {

    let response;
    try {
        // Make a PUT request to your server API endpoint
        //const { meeting_title, meeting_description, location, duration, platform, starttime, attendees } = validatedFields.data;

        console.log(formData);
        const actor_id = formData.actor_id;
        const access_token = formData.access_token;

        response = await fetch(`http://localhost:7057/meeting/update-meeting?actor_id=${actor_id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
                Authorization: `Bearer ${access_token}`
            },
            body: JSON.stringify({
                id: formData.id,
                meeting_title: formData.meeting_title,
                meeting_description: formData.meeting_description,
                location: formData.location,
                duration: formData.duration,
                starttime: formData.starttime,
                attendees: formData.attendees,
            }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.error('Update Meeting error:', errorData);
            return {
                ...prevState,
                message: `Update Meeting error: ${errorData.detail || response.statusText}`,
                errors: errorData.errors || {},
            };
        }
    } catch (error) {
        // Handle any errors that occur during the request
        console.error('Update meeting error:', error);
        return {
            ...prevState,
            message: 'Update meeting error.',
        };
    }
    if (response.ok) {
        revalidatePath('/dashboard');
        redirect('/dashboard');
        return {
            ...prevState,
            message: 'Meeting created successfully.',
            errors: {},
        };
    }

    return prevState;
}

export async function authenticate(
    prevState: string | undefined,
    formData: FormData,
) {
    try {
        await signIn('credentials', formData);
    } catch (error: any) {
        if (error instanceof AuthError) {
            switch (error.type) {
                case 'CredentialsSignin':
                    return 'Invalid credentials.';
                default:
                    return 'Login error.';
            }
        }
        throw error;
    }
}

export async function register(
    prevState: AccountState,
    formData: FormData
) {
    let response;
    try {
        response = await fetch(`http://localhost:7057/user/create`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                name: formData.get('name'),
                email: formData.get('email'),
                username: formData.get('username'),
                password: formData.get('password'),
                confirm_password: formData.get('confirm_password'),
            }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.error('Register eror:', errorData);
            return {
                ...prevState,
                message: `Register eror: ${errorData.detail || response.statusText}`,
                errors: errorData.errors || {},
            };
        }
    } catch (error) {
        console.error('Register eror:', error);
        return {
            ...prevState,
            message: 'Register eror.',
            errors: {},
        };
    }

    if (response.ok) {
        revalidatePath('/login');
        redirect('/login');
    }

    return prevState;
}
