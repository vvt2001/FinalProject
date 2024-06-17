import Form from '@/app/ui/user/edit-form';
import Breadcrumbs from '@/app/ui/meetings/breadcrumbs';
import { fetchUserById } from '@/app/lib/data';
import { notFound } from 'next/navigation';
import { Metadata } from 'next';
import { cookies } from "next/headers";

export const metadata: Metadata = {
    title: 'Edit User | Scheduler',
};
export default async function Page({ params }: { params: { id: string } }) {
    const cookieStore = cookies();
    const actor_id = cookieStore.get("actor_id")?.value;
    console.log(actor_id);
    const id = params.id;
    const [user] = await Promise.all([
        fetchUserById(id, actor_id),
    ]);

    if (!user) {
        notFound();
    }
    return (
        <main>
            <Breadcrumbs
                breadcrumbs={[
                    { label: 'User', href: '/dashboard/user' },
                    {
                        label: 'Edit User',
                        href: `/dashboard/user/${id}/edit`,
                        active: true,
                    },
                ]}
            />
            <Form user={user} />
        </main>
    );
}