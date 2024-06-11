import Form from '@/app/ui/meetingforms/create-form';
import Breadcrumbs from '@/app/ui/meetingforms/breadcrumbs';

export default async function Page() {

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
            <Form />
        </main>
    );
}