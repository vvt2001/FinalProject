import Pagination from '@/app/ui/meetingforms/pagination';
import Search from '@/app/ui/search';
import Table from '@/app/ui/meetings/table';
import { CreateMeetingForm } from '@/app/ui/meetingforms/buttons';
import { lusitana } from '@/app/ui/fonts';
import { MeetingFormTableSkeleton } from '@/app/ui/skeletons';
import { Suspense } from 'react';
import { fetchMeetingPages } from '@/app/lib/data';

export default async function Page({
    searchParams,
}: {
    searchParams?: {
        query?: string;
        page?: string;
    };
}) {
    const query = searchParams?.query || '';
    const currentPage = Number(searchParams?.page) || 1;
    const totalPages = await fetchMeetingPages();

    return (
        <div className="w-full">
            <div className="flex w-full items-center justify-between">
                <h1 className={`${lusitana.className} text-2xl`}>Meetings</h1>
            </div>
            <div className="mt-4 flex items-center justify-between gap-2 md:mt-8">
                <Search placeholder="Search meetings ..." />
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