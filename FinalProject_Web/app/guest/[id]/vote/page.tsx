import Form from '@/app/ui/meetingforms/vote-form';
import Breadcrumbs from '@/app/ui/meetingforms/breadcrumbs';
import { fetchMeetingFormById } from '@/app/lib/data';
import { notFound } from 'next/navigation';
import { Metadata } from 'next';
import { cookies } from "next/headers";

export const metadata: Metadata = {
    title: 'Vote meeting times | Scheduler',
};
export default async function Page({ params }: { params: { id: string } }) {
    const cookieStore = cookies();
    const actor_id = cookieStore.get("actor_id")?.value;
    console.log(actor_id);
    const id = params.id;
    const [meetingform] = await Promise.all([
        fetchMeetingFormById(id, actor_id),
    ]);

    if (!meetingform) {
        notFound();
    }
    return (
        <main>
            <Breadcrumbs
                breadcrumbs={[
                    { label: 'Vote meeting time', href: '/guest' },
                    {
                        label: 'Vote Meeting',
                        href: `guest/${id}/vote`,
                        active: true,
                    },
                ]}
            />
            <Form meetingform={meetingform} />
        </main>
    );
}