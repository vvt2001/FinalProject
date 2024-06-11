import NextAuth from 'next-auth';
import Credentials from 'next-auth/providers/credentials';
import { authConfig } from './auth.config';
import { z } from 'zod';
import { sql } from '@vercel/postgres';
import type { User } from '@/app/lib/definitions';
import { cookies } from "next/headers";

async function getUser(username: string, password: string): Promise<User | undefined> {
    const apiUrl = 'http://localhost:7057/user/authenticate'; // Replace with your actual API URL
    try {

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

            throw new Error(`HTTP error! status: ${response.status}`);
        }


        // Map the fetched data to the MeetingForm type definition
        const user: User = {
            id: userData.id,
            name: userData.name,
            email: userData.email,
            access_token: userData.access_token,
        };

        cookies().set(
            {
                name: "actor_id",
                value: user.id,
                httpOnly: true,
                path: "/",
                maxAge: 60 * 60 * 24 * 30 * 1000,
                expires: new Date(Date.now() + 60 * 60 * 24 * 30 * 1000),
            }
        );
        cookies().set(
            {
                name: "access_token",
                value: user.access_token,
                httpOnly: true,
                path: "/",
                maxAge: 60 * 60 * 24 * 30 * 1000,
                expires: new Date(Date.now() + 60 * 60 * 24 * 30 * 1000),
            }
        );
        cookies().set('actor_id', user.id);
        cookies().set('access_token', user.access_token);

        //const cookieStore = cookies();
        //const actor_id = cookieStore.get("actor_id")?.value;
        //const access_token = cookieStore.get("access_token")?.value;
        //console.log(actor_id);
        //console.log(access_token);

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

                return null;
            },
        }),
    ],
});