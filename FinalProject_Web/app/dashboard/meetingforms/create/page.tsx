import Form from '@/app/ui/meetingforms/create-form';
import Breadcrumbs from '@/app/ui/meetingforms/breadcrumbs';
import { fetchCustomers } from '@/app/lib/data';

export default async function Page() {
    const customers = await fetchCustomers();

    return (
        <main>
            <Breadcrumbs
                breadcrumbs={[
                    { label: 'meetingforms', href: '/dashboard/meetingforms' },
                    {
                        label: 'Create Meeting Form',
                        href: '/dashboard/meetingforms/create',
                        active: true,
                    },
                ]}
            />
            <Form customers={customers} />
        </main>
    );
}