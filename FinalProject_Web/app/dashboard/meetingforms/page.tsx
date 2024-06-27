import Pagination from '@/app/ui/meetingforms/pagination';
import Search from '@/app/ui/search';
import Table from '@/app/ui/meetingforms/table';
import { CreateMeetingForm } from '@/app/ui/meetingforms/buttons';
import { lusitana } from '@/app/ui/fonts';
import { MeetingFormTableSkeleton } from '@/app/ui/skeletons';
import { Suspense } from 'react';
import { fetchMeetingFormPages } from '@/app/lib/data';
import { cookies } from "next/headers";

export default async function Page({
    searchParams,
}: {
    searchParams?: {
        query?: string;
        page?: string;
    };
    }) {
    const cookieStore = cookies();
    const actor_id = cookieStore.get("actor_id")?.value;
    console.log(actor_id);
    const query = searchParams?.query || '';
    const currentPage = Number(searchParams?.page) || 1;
    const totalPages = await fetchMeetingFormPages(actor_id);

    return (
        <div className="w-full">
            <div className="flex w-full items-center justify-between">
                <h1 className={`${lusitana.className} text-2xl`}>Schedules</h1>
            </div>
            <div className="mt-4 flex items-center justify-between gap-2 md:mt-8">
                <Search placeholder="Search schedules ..." />
                <CreateMeetingForm />
            </div>
            <Suspense key={currentPage} fallback={<MeetingFormTableSkeleton />}>
                <Table query={query} currentPage={currentPage} />
            </Suspense>
            <div className="mt-5 flex w-full justify-center">
                <Pagination totalPages={totalPages} />
            </div>
        </div>
    );
}