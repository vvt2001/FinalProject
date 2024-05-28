import { CheckIcon, ClockIcon, MicrophoneIcon } from '@heroicons/react/24/outline';
import clsx from 'clsx';

export default function MeetingFormStatus({ status }: { status: number }) {
  return (
    <span
      className={clsx(
        'inline-flex items-center rounded-full px-2 py-1 text-xs',
        {
            'bg-gray-100 text-gray-500': status === 1,
            'bg-green-500 text-white': status === 2,
        },
      )}
    >
      {status === 1 ? (
        <>
          Pending
          <ClockIcon className="ml-1 w-4 text-gray-500" />
        </>
      ) : null}
      {status === 2 ? (
        <>
          Booked
          <CheckIcon className="ml-1 w-4 text-white" />
        </>
      ) : null}
    </span>
  );
}
