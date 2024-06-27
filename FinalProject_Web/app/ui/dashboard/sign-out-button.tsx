// app/ui/SignOutButton.tsx
'use client';

import { useRouter } from 'next/navigation';
import { useState } from 'react';
import { signOut } from '@/app/lib/actions'; // Updated import path
import { PowerIcon } from '@heroicons/react/24/outline';

const SignOutButton = () => {
    const router = useRouter();

    const handleSignOut = async () => {
        await signOut();
        // Redirect to login page
        router.push('/login');
    };

    return (
        <button
            onClick={handleSignOut}
            className="flex h-[48px] grow items-center justify-center gap-2 rounded-md bg-gray-50 p-3 text-sm font-medium hover:bg-sky-100 hover:text-blue-600 md:flex-none md:justify-start md:p-2 md:px-3"
        >
            <PowerIcon className="w-6" />
            <div className="hidden md:block">Sign Out</div>
        </button>
    );
};

export default SignOutButton;
