'use server';

import { z } from 'zod';
import { sql } from '@vercel/postgres';
import { revalidatePath } from 'next/cache';
import { redirect } from 'next/navigation';
import { signIn } from '@/auth';
import { AuthError } from 'next-auth';
import { axios } from 'axios';
import { cookies } from "next/headers";

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
            message: 'Please select a date and time for the meeting.',
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
        message: 'Please select a date and time for the meeting.',
    }),
    attendees: z.array(
        z.coerce.date({
            message: 'Please select a date and time for the meeting.',
        })
    ),
});


const VotingFormSchema = z.object({
    meetingform_id: z.string({
        invalid_type_error: 'Please select a title.',
    }),
    meetingtime_ids: z.array(
        z.string({
            message: 'Please vote atleast one date and time for the meeting.',
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

const cookieStore = cookies();

const actor_id = cookieStore.get("actor_id")?.value;
const access_token = cookieStore.get("access_token")?.value;
console.log(actor_id);
console.log(access_token);

export type MeetingFormState = {
    errors?: {
        meeting_title?: string[];
        meeting_description?: string[];
        location?: string[];
        duration?: string[];
        platform?: string[];
        times?: string[];
    };
    message?: string | null;
};

export type MeetingState = {
    errors?: {
        meeting_title?: string[];
        meeting_description?: string[];
        location?: string[];
        duration?: string[];
        platform?: string[];
        starttime?: string[];
        attendees?: string[];
    };
    message?: string | null;
};

export type AccountState = {
    errors?: {
        name?: string[];
        email?: string[];
        username?: string[];
        password?: string[];
        confirm_password?: string[];
    };
    message?: string | null;
};
export async function createMeetingForm(prevState: MeetingFormState, formData: FormData) {
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
            message: 'Missing Fields. Failed to Create Meeting Form.',
        };
    }
    let response;
    // Insert data into the database
    try {
        // Make a POST request to your server API endpoint
        const { meeting_title, meeting_description, location, times, duration, platform } = validatedFields.data;

        response = await fetch(`http://localhost:7057/meeting-form/create-form?actor_id=${actor_id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
                Authorization: `Bearer ${access_token}` 
            },
            body: JSON.stringify({
                meeting_title: meeting_title,
                meeting_description: meeting_description,
                location: location,
                times: times,
                duration: parseInt(duration || '0', 10),
                platform: parseInt(platform || '0', 10),
            }),
        });
        if (!response.ok) {
            const errorData = await response.json();
            console.error('Lỗi tạo lịch họp:', errorData);
            return {
                ...prevState,
                message: `Lỗi tạo lịch họp: ${errorData.detail || response.statusText}`,
                errors: errorData.errors || {},
            };
        }

    } catch (error) {

        // Handle any errors that occur during the request
        console.error('Lỗi tạo lịch họp:', error);
        return {
            message: 'Lỗi tạo lịch họp.',
        };
    }
    if (response.ok) {
        // Revalidate the cache for the invoices page and redirect the user.
        revalidatePath('/dashboard');
        redirect('/dashboard');
    }
}

export async function voteMeetingForm(requestBody) {
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
            message: 'Missing Fields. Failed to Create Meeting Form.',
        };
    }

    // Insert data into the database
    try {
        // Make a POST request to your server API endpoint
        const { meetingform_id, meetingtime_ids, name, email } = validatedFields.data;

        const response = await fetch(`http://localhost:7057/meeting-form/vote-form?actor_id=${actor_id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
            },
            body: JSON.stringify({
                meetingform_id: meetingform_id,
                meetingtime_ids: meetingtime_ids,
                name: name,
                email: email,
            }),
        });

    } catch (error) {

        // Handle any errors that occur during the request
        console.error('Error creating meeting form:', error);
        return {
            message: 'Failed to create meeting form.',
        };
    }

    // Revalidate the cache for the invoices page and redirect the user.
    revalidatePath('/dashboard');
    redirect('/dashboard');
}

export async function bookMeetingForm(requestBody) {
    // Validate form using Zod
    const validatedFields = BookMeetingForm.safeParse({
        meetingform_id: requestBody.meetingform_id,
    });

    // If form validation fails, return errors early. Otherwise, continue.
    if (!validatedFields.success) {
        return {
            errors: validatedFields.error.flatten().fieldErrors,
            message: 'Missing Fields. Failed to Create Meeting Form.',
        };
    }

    // Insert data into the database
    try {

        // Make a POST request to your server API endpoint
        const { meetingform_id } = validatedFields.data;
        const response = await fetch(`http://localhost:7057/meeting-form/book-meeting?actor_id=${actor_id}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
                Authorization: `Bearer ${access_token}` 
            },
            body: JSON.stringify({
                meetingform_id: meetingform_id,
            }),
        });

    } catch (error) {

        // Handle any errors that occur during the request
        console.error('Error creating meeting form:', error);
        return {
            message: 'Failed to create meeting form.',
        };
    }

    // Revalidate the cache for the invoices page and redirect the user.
    revalidatePath('/dashboard');
    redirect('/dashboard');
}

export async function updateMeetingForm(
    id: string,
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

    try {
        // Make a PUT request to your server API endpoint
        const { meeting_title, meeting_description, location, times, duration, platform } = validatedFields.data;

        const response = await fetch(`http://localhost:7057/meeting-form/update-form?actor_id=${actor_id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
                Authorization: `Bearer ${access_token}` 
            },
            body: JSON.stringify({
                id: id,
                meeting_title: meeting_title,
                meeting_description: meeting_description,
                location: location,
                times: times,
                duration: parseInt(duration || '0', 10),
                platform: parseInt(platform || '0', 10),
            }),
        });

    } catch (error) {
        return { message: 'Database Error: Failed to Update Meeting Form.' };
    }

    revalidatePath('/dashboard');
    redirect('/dashboard');
}

export async function deleteMeetingForm(id: string) {
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

export async function deleteMeeting(id: string) {
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

export async function cancelMeeting(id: string) {
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
    id: string,
    prevState: MeetingState,
    formData: FormData,
) {
    //// Validate form using Zod
    //const validatedFields = UpdateMeeting.safeParse({
    //    meeting_title: formData.get('meeting_title'),
    //    meeting_description: formData.get('meeting_description'),
    //    location: formData.get('location'),
    //    duration: formData.get('duration'),
    //    starttime: formData.get('starttime'),
    //    platform: formData.get('platform'),
    //    attendees: formData.getAll('attendees'),
    //});

    //// If form validation fails, return errors early. Otherwise, continue.
    //if (!validatedFields.success) {
    //    return {
    //        errors: validatedFields.error.flatten().fieldErrors,
    //        message: 'Missing Fields. Failed to Update Meeting Form.',
    //    };
    //}
    let response;
    try {
        // Make a PUT request to your server API endpoint
        //const { meeting_title, meeting_description, location, duration, platform, starttime, attendees } = validatedFields.data;

        console.log(formData);

        response = await fetch(`http://localhost:7057/meeting/update-meeting?actor_id=${actor_id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
                Authorization: `Bearer ${access_token}`
            },
            body: JSON.stringify({
                id: id,
                meeting_title: formData.meeting_title,
                meeting_description: formData.meeting_description,
                location: formData.location,
                duration: formData.duration,
                starttime: formData.starttime,
                platform: formData.platform,
                attendees: formData.attendees,
            }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.error('Lỗi sửa cuộc họp:', errorData);
            return {
                ...prevState,
                message: `Lỗi sửa cuộc họp: ${errorData.detail || response.statusText}`,
                errors: errorData.errors || {},
            };
        }
    } catch (error) {
        return { message: 'Database Error: Failed to Update Meeting Form.' };
    }
    if (response.ok) {
        revalidatePath('/dashboard/meetings');
        redirect('/dashboard/meetings');
    }
}

export async function authenticate(
    prevState: string | undefined,
    formData: FormData,
) {
    try {
        await signIn('credentials', formData);
    } catch (error) {
        if (error instanceof AuthError) {
            switch (error.type) {
                case 'CredentialsSignin':
                    return 'Tài khoản không hợp lệ.';
                default:
                    return 'Lỗi đăng nhập.';
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
            console.error('Lỗi đăng ký:', errorData);
            return {
                ...prevState,
                message: `Lỗi đăng ký: ${errorData.detail || response.statusText}`,
                errors: errorData.errors || {},
            };
        }
    } catch (error) {
        console.error('Lỗi đăng ký:', error);
        return {
            ...prevState,
            message: 'Lỗi đăng ký tài khoản.',
            errors: {},
        };
    }

    if (response.ok) {
        revalidatePath('/login');
        redirect('/login');
    }

    return prevState;
}
