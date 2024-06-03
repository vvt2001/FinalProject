import Image from 'next/image';
import { UpdateMeeting, DeleteMeeting, CancelMeeting } from '@/app/ui/meetingforms/buttons';
import MeetingStatus from '@/app/ui/meetings/status';
import { formatDateToLocal, formatCurrency } from '@/app/lib/utils';
import { fetchFilteredMeeting } from '@/app/lib/data';
import { cookies } from "next/headers";

export default async function MeetingTable({
    query,
    currentPage,
}: {
    query: string;
    currentPage: number;
}) {
    const cookieStore = cookies();
    const actor_id = cookieStore.get("actor_id")?.value;
    console.log(actor_id);
    const meetings = await fetchFilteredMeeting(query, currentPage, actor_id);

  return (
    <div className="mt-6 flow-root">
      <div className="inline-block min-w-full align-middle">
        <div className="rounded-lg bg-gray-50 p-2 md:pt-0">
          <div className="md:hidden">
            {meetings?.map((meeting) => (
              <div
                key={meeting.id}
                className="mb-2 w-full rounded-md bg-white p-4"
              >
                <div className="flex items-center justify-between border-b pb-4">
                  <div>
                    <div className="mb-2 flex items-center">
                      <p>{meeting.meeting_title}</p>
                    </div>
                    <p className="text-sm text-gray-500">{meeting.meeting_description}</p>
                  </div>
                  <MeetingStatus status={meeting.trangthai} />
                </div>
                <div className="flex w-full items-center justify-between pt-4">
                  <div>
                    <p className="text-xl font-medium">
                      {meeting.attendees.length}
                    </p>
                    <p>{meeting.duration}</p>
                  </div>
                  <div className="flex justify-end gap-2">
                    <UpdateMeeting id={meeting.id} />
                    <CancelMeeting id={meeting.id} />
                    <DeleteMeeting id={meeting.id} />
                  </div>
                </div>
              </div>
            ))}
          </div>
          <table className="hidden min-w-full text-gray-900 md:table">
            <thead className="rounded-lg text-left text-sm font-normal">
              <tr>
                <th scope="col" className="px-4 py-5 font-medium sm:pl-6 text-center">
                  Title
                </th>
                <th scope="col" className="px-3 py-5 font-medium text-center">
                  Description
                </th>
                <th scope="col" className="px-3 py-5 font-medium text-center">
                  Duration
                </th>
                <th scope="col" className="px-3 py-5 font-medium text-center">
                  Status
                </th>
                <th scope="col" className="px-3 py-5 font-medium text-center">
                  Attendee count
                </th>
                <th scope="col" className="relative py-3 pl-6 pr-3 text-center">
                  <span className="sr-only">Edit</span>
                </th>
              </tr>
            </thead>
            <tbody className="bg-white">
              {meetings?.map((meeting) => (
                <tr
                  key={meeting.id}
                  className="w-full border-b py-3 text-sm last-of-type:border-none [&:first-child>td:first-child]:rounded-tl-lg [&:first-child>td:last-child]:rounded-tr-lg [&:last-child>td:first-child]:rounded-bl-lg [&:last-child>td:last-child]:rounded-br-lg"
                >
                  <td className="whitespace-nowrap py-3 pl-6 pr-3 text-center">
                    <div className="flex items-center gap-3 text-center">
                      <p>{meeting.meeting_title}</p>
                    </div>
                  </td>
                  <td className="whitespace-nowrap px-3 py-3 text-center ">
                    {meeting.meeting_description}
                  </td>
                  <td className="whitespace-nowrap px-3 py-3 text-center">
                    {meeting.duration}
                  </td>
                  <td className="whitespace-nowrap px-3 py-3 text-center">
                    <MeetingStatus status={meeting.trangthai} />
                  </td>
                  <td className="whitespace-nowrap px-3 py-3 text-center">
                    {meeting.attendees.length}
                  </td>
                  <td className="whitespace-nowrap py-3 pl-6 pr-3 text-center">
                    <div className="flex justify-end gap-3">
                        <UpdateMeeting id={meeting.id} />
                        <CancelMeeting id={meeting.id} />
                        <DeleteMeeting id={meeting.id} />
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
