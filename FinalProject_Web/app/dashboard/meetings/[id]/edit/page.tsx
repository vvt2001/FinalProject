import Form from '@/app/ui/meetings/edit-form';
import Breadcrumbs from '@/app/ui/meetings/breadcrumbs';
import { fetchMeetingById } from '@/app/lib/data';
import { notFound } from 'next/navigation';
import { Metadata } from 'next';

export const metadata: Metadata = {
    title: 'Edit Meeting | Scheduler',
};
export default async function Page({ params }: { params: { id: string } }) {
    const id = params.id;
    const [meeting] = await Promise.all([
        fetchMeetingById(id),
    ]);

    if (!meeting) {
        notFound();
    }
    return (
        <main>
            <Breadcrumbs
                breadcrumbs={[
                    { label: 'Meetings', href: '/dashboard/meetings' },
                    {
                        label: 'Edit Meeting',
                        href: `/dashboard/meetings/${id}/edit`,
                        active: true,
                    },
                ]}
            />
            <Form meeting={meeting} />
        </main>
    );
}