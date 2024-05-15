import NextAuth from 'next-auth';
import Credentials from 'next-auth/providers/credentials';
import { authConfig } from './auth.config';
import { z } from 'zod';
import { sql } from '@vercel/postgres';
import type { User } from '@/app/lib/definitions';

async function getUser(username: string, password: string): Promise<User | undefined> {
    const apiUrl = 'http://localhost:7057/user/authenticate'; // Replace with your actual API URL
    try {
        console.log(username);
        console.log(password);

        const response = await fetch('http://localhost:7057/user/authenticate', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Add any additional headers if needed
            },
            body: JSON.stringify({
                Username: username,
                Password: password,
            }),
        });
        const responseData = await response.json();

        // Extract the array of invoices from the response data
        const userData = responseData.data;

        if (!response.ok) {
            console.log("checkpoint 3");

            throw new Error(`HTTP error! status: ${response.status}`);
        }
        console.log("checkpoint 2");


        // Map the fetched data to the MeetingForm type definition
        const user: User = {
            id: userData.id,
            name: userData.name,
            email: userData.email,
            access_token: userData.access_token,
        };

        return user;

    } catch (error) {
        console.error('Failed to fetch user:', error);
        throw new Error('Failed to fetch user.');
    }
}

export const { auth, signIn, signOut } = NextAuth({
    ...authConfig,
    providers: [
        Credentials({
            async authorize(credentials) {
                const parsedCredentials = z
                    .object({ username: z.string(), password: z.string().min(6) })
                    .safeParse(credentials);

                if (parsedCredentials.success) {
                    const { username, password } = parsedCredentials.data;
                    const user = await getUser(username, password);

                    if (user) return user;
                }

                console.log('Invalid credentials');
                return null;
            },
        }),
    ],
});