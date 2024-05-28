import { CheckIcon, ClockIcon, MicrophoneIcon, XMarkIcon } from '@heroicons/react/24/outline';
import clsx from 'clsx';

export default function MeetingStatus({ status }: { status: number }) {
    return (
        <span
            className={clsx(
                'inline-flex items-center rounded-full px-2 py-1 text-xs',
                {
                    'bg-gray-100 text-gray-500': status === 1,
                    'bg-green-500 text-white': status === 2,
                    'bg-yellow-500 text-white': status === 3,
                    'bg-red-500 text-white': status === 4,
                },
            )}
        >
            {status === 1 ? (
                <>
                    Waiting
                    <ClockIcon className="ml-1 w-4 text-gray-500" />
                </>
            ) : null}
            {status === 2 ? (
                <>
                    Ongoing
                    <MicrophoneIcon className="ml-1 w-4 text-white" />
                </>
            ) : null}
            {status === 3 ? (
                <>
                    Done
                    <CheckIcon className="ml-1 w-4 text-white" />
                </>
            ) : null}
            {status === 4 ? (
                <>
                    Canceled
                    <XMarkIcon className="ml-1 w-4 text-white" />
                </>
            ) : null}
        </span>
    );
}
