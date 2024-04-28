'use server';

import { z } from 'zod';
import { sql } from '@vercel/postgres';
import { revalidatePath } from 'next/cache';
import { redirect } from 'next/navigation';
import { signIn } from '@/auth';
import { AuthError } from 'next-auth';
import { axios } from 'axios';

//const FormSchema = z.object({
//    id: z.string(),
//    customerId: z.string({
//        invalid_type_error: 'Please select a customer.',
//    }),
//    amount: z.coerce
//        .number()
//        .gt(0, { message: 'Please enter an amount greater than $0.' }),
//    status: z.enum(['pending', 'paid'], {
//        invalid_type_error: 'Please select an invoice status.',
//    }),
//    date: z.string(),
//});

//const CreateInvoice = FormSchema.omit({ id: true, date: true });
//const UpdateInvoice = FormSchema.omit({ id: true, date: true });

export type State = {
    errors?: {
        customerId?: string[];
        amount?: string[];
        status?: string[];
    };
    message?: string | null;
};

export async function createInvoice(prevState: State, formData: FormData) {
    // Validate form using Zod
    const validatedFields = CreateInvoice.safeParse({
        customerId: formData.get('customerId'),
        amount: formData.get('amount'),
        status: formData.get('status'),
    });

    // If form validation fails, return errors early. Otherwise, continue.
    if (!validatedFields.success) {
        return {
            errors: validatedFields.error.flatten().fieldErrors,
            message: 'Missing Fields. Failed to Create Invoice.',
        };
    }

    // Prepare data for insertion into the database
    const { customerId, amount, status } = validatedFields.data;
    const amountInCents = amount * 100;
    const date = new Date().toISOString().split('T')[0];

    // Insert data into the database
    try {
        await sql`
      INSERT INTO invoices (customer_id, amount, status, date)
      VALUES (${customerId}, ${amountInCents}, ${status}, ${date})
    `;
    } catch (error) {
        // If a database error occurs, return a more specific error.
        return {
            message: 'Database Error: Failed to Create Invoice.',
        };
    }

    // Revalidate the cache for the invoices page and redirect the user.
    revalidatePath('/dashboard/invoices');
    redirect('/dashboard/invoices');
}

export async function createMeetingForm(prevState: State, formData: FormData) {
    //// Validate form using Zod
    //const validatedFields = CreateInvoice.safeParse({
    //    meeting_title: formData.get('meeting_title'),
    //    meeting_description: formData.get('meeting_description'),
    //    location: formData.get('location'),
    //    times: formData.get('times'),
    //    duration: formData.get('duration'),
    //    platform: formData.get('platform'),
    //});

    //// If form validation fails, return errors early. Otherwise, continue.
    //if (!validatedFields.success) {
    //    return {
    //        errors: validatedFields.error.flatten().fieldErrors,
    //        message: 'Missing Fields. Failed to Create Meeting Form.',
    //    };
    //}

    // Insert data into the database
    try {
        // Make a POST request to your server API endpoint
        const response = await fetch('http://localhost:7057/meeting/create-form?actor_id=4efyqow4ywdutzb52oymalf5d', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
            },
            body: JSON.stringify({
                meeting_title: formData.get('meeting_title'),
                meeting_description: formData.get('meeting_description'),
                times: JSON.parse(formData.get('times') || '[]'),
                duration: parseInt(formData.get('duration') || '0', 10),
                platform: parseInt(formData.get('platform') || '0', 10),
            }),
        });

        // Check if the request was successful
        if (response.ok) {
            // Revalidate cache and redirect if successful
            revalidatePath('/dashboard/meetingforms');
            redirect('/dashboard/meetingforms');
        } else {
            // Handle error response
            const responseData = await response.json();
            return {
                message: responseData.message || 'Failed to create meeting form.',
            };
        }
    } catch (error) {
        // Handle any errors that occur during the request
        console.error('Error creating meeting form:', error);
        return {
            message: 'Failed to create meeting form.',
        };
    }

    // Revalidate the cache for the invoices page and redirect the user.
    revalidatePath('/dashboard/invoices');
    redirect('/dashboard/invoices');
}

//export async function updateInvoice(id: string, formData: FormData) {
//    const { customerId, amount, status } = UpdateInvoice.parse({
//        customerId: formData.get('customerId'),
//        amount: formData.get('amount'),
//        status: formData.get('status'),
//    });

//    const amountInCents = amount * 100;

//    try {
//        await sql`
//        UPDATE invoices
//        SET customer_id = ${customerId}, amount = ${amountInCents}, status = ${status}
//        WHERE id = ${id}
//      `;
//    } catch (error) {
//        return { message: 'Database Error: Failed to Update Invoice.' };
//    }

//    revalidatePath('/dashboard/invoices');
//    redirect('/dashboard/invoices');
//}

export async function updateInvoice(
    id: string,
    prevState: State,
    formData: FormData,
) {
    const validatedFields = UpdateInvoice.safeParse({
        customerId: formData.get('customerId'),
        amount: formData.get('amount'),
        status: formData.get('status'),
    });

    if (!validatedFields.success) {
        return {
            errors: validatedFields.error.flatten().fieldErrors,
            message: 'Missing Fields. Failed to Update Invoice.',
        };
    }

    const { customerId, amount, status } = validatedFields.data;
    const amountInCents = amount * 100;

    try {
        await sql`
      UPDATE invoices
      SET customer_id = ${customerId}, amount = ${amountInCents}, status = ${status}
      WHERE id = ${id}
    `;
    } catch (error) {
        return { message: 'Database Error: Failed to Update Invoice.' };
    }

    revalidatePath('/dashboard/invoices');
    redirect('/dashboard/invoices');
}

export async function deleteInvoice(id: string) {
    throw new Error('Failed to Delete Invoice');

    try {
        await sql`DELETE FROM invoices WHERE id = ${id}`;
        revalidatePath('/dashboard/invoices');
        return { message: 'Deleted Invoice.' };
    } catch (error) {
        return { message: 'Database Error: Failed to Delete Invoice.' };
    }
}

export async function deleteMeetingForm(id: string) {
    const apiUrl = `http://localhost:7057/meeting/delete-form/${id}?actor_id=4efyqow4ywdutzb52oymalf5d`; 

    try {
        // Make an HTTP DELETE request to your delete API endpoint
        const response = await fetch(apiUrl, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if required
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
                    return 'Invalid credentials.';
                default:
                    return 'Something went wrong.';
            }
        }
        throw error;
    }
}
