// This file contains type definitions for your data.
// It describes the shape of the data, and what data type each property should accept.
// For simplicity of teaching, we're manually defining these types.
// However, these types are generated automatically if you're using an ORM such as Prisma.
export type User = {
    id: string;
    name: string;
    email: string;
    //password: string;
    access_token: string;
    has_googlecredentials: boolean;
};

export type MeetingFormsTable = {
    id: string;
    meeting_title: string;
    meeting_description: string;
    platform: number;
    duration: number;
    status: 'pending' | 'done';
};

export type CustomersTableType = {
  id: string;
  name: string;
  email: string;
  image_url: string;
  total_invoices: number;
  total_pending: number;
  total_paid: number;
};

export type FormattedCustomersTable = {
  id: string;
  name: string;
  email: string;
  image_url: string;
  total_invoices: number;
  total_pending: string;
  total_paid: string;
};

export type CustomerField = {
  id: string;
  name: string;
};

export type InvoiceForm = {
  id: string;
  customer_id: string;
  amount: number;
  status: 'pending' | 'paid';
};

export type MeetingForm = {
    id: string;
    meeting_title: string;
    meeting_description: string;
    location: string;
    platform: number;
    duration: number;
    times: any[];
};

export type Meeting = {
    id: string;
    meeting_title: string;
    meeting_description: string;
    location: string;
    platform: number;
    duration: number;
    starttime: Date;
    attendees: Attendee[]
};

export type Attendee = {
    name: string;
    email: string;
    [key: string]: string;
};

export type MeetingFormState = {
    errors?: {
        meeting_title?: string[];
        meeting_description?: string[];
        location?: string[];
        duration?: string[];
        platform?: string[];
        times?: string[];
    };
    message?: string | null;
};

export type MeetingState = {
    errors?: {
        meeting_title?: string[];
        meeting_description?: string[];
        location?: string[];
        duration?: string[];
        platform?: string[];
        starttime?: string[];
        attendees?: string[];
    };
    message?: string | null;
};

export type AccountState = {
    errors?: {
        name?: string[];
        email?: string[];
        username?: string[];
        password?: string[];
        confirm_password?: string[];
    };
    message?: string | null;
};

export type UserState = {
    errors?: {
        id?: string[];
        name?: string[];
        email?: string[];
        access_token?: string[];
        has_googlecredentials?: boolean[];
    };
    message?: string | null;
};
