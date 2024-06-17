'use client';

import {
  UserGroupIcon,
  HomeIcon,
  DocumentDuplicateIcon,
  CalendarDaysIcon,
  UsersIcon,
  VideoCameraIcon
} from '@heroicons/react/24/outline';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import clsx from 'clsx';
import { useState, useEffect, ChangeEvent } from 'react';
export default function NavLinks() {
    // Map of links to display in the side navigation.
    // Depending on the size of the application, this would be stored in a database.

    // State to store the actor_id
    const [actor_id, setActorId] = useState('');
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

        const actorIdFromCookie = getCookie("actor_id");
        const accessTokenFromCookie = getCookie("access_token");
        setActorId(actorIdFromCookie || '');
        setAccessToken(accessTokenFromCookie || '');
    }, []);

    const links = [
        { name: 'My Schedules', href: '/dashboard', icon: CalendarDaysIcon },
        { name: 'My Meetings', href: '/dashboard/meetings', icon: VideoCameraIcon },
        { name: 'User Info', href: `/dashboard/user/${actor_id}/edit`, icon: UsersIcon },
        //{
        //    name: 'Invoices',
        //    href: '/dashboard/invoices',
        //    icon: DocumentDuplicateIcon,
        //},
        //{ name: 'Customers', href: '/dashboard/customers', icon: UserGroupIcon },
        //{ name: 'Meeting Forms', href: '/dashboard/meetingforms', icon: DocumentDuplicateIcon },
    ];

    const pathname = usePathname();

  return (
    <>
      {links.map((link) => {
        const LinkIcon = link.icon;
          return (
              <Link
            key={link.name}
            href={link.href}
                className={clsx(
                    'flex h-[48px] grow items-center justify-center gap-2 rounded-md bg-gray-50 p-3 text-sm font-medium hover:bg-sky-100 hover:text-blue-600 md:flex-none md:justify-start md:p-2 md:px-3',
                    {
                        'bg-sky-100 text-blue-600': pathname === link.href,
                    },
                )}          >
            <LinkIcon className="w-6" />
            <p className="hidden md:block">{link.name}</p>
          </Link>
        );
      })}
    </>
  );
}
