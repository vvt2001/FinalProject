'use client';

import { useRouter } from 'next/navigation';
import { useState, useEffect } from 'react';
import SideNav from '@/app/ui/dashboard/sidenav';

export default function Layout({ children }: { children: React.ReactNode }) {
    const router = useRouter();
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

        const accessTokenFromCookie = getCookie("access_token");
        setAccessToken(accessTokenFromCookie || '');

        if (!accessTokenFromCookie) {
            router.push('/login');
        }
    }, []);

    if (!access_token) {
        return null; // Or a loading spinner or any fallback UI
    }

    return (
        <div className="flex h-screen flex-col md:flex-row md:overflow-hidden">
            <div className="w-full flex-none md:w-64">
                <SideNav />
            </div>
            <div className="flex-grow p-6 md:overflow-y-auto md:p-12">{children}</div>
        </div>
    );
}