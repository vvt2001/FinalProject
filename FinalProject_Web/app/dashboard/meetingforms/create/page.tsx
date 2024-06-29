import Form from '@/app/ui/meetingforms/create-form';
import Breadcrumbs from '@/app/ui/meetingforms/breadcrumbs';
import { fetchUserById } from '@/app/lib/data';
import { cookies } from "next/headers";

export default async function Page() {
    const cookieStore = cookies();
    const actor_id: any = cookieStore.get("actor_id")?.value;
    const [user] = await Promise.all([
        fetchUserById(actor_id, actor_id),
    ]);

    return (
        <main>
            <Breadcrumbs
                breadcrumbs={[
                    { label: 'Meetings', href: '/dashboard/meetingforms' },
                    {
                        label: 'Create Meeting',
                        href: '/dashboard/meetingforms/create',
                        active: true,
                    },
                ]}
            />
            <Form user={user} />
        </main>
    );
}