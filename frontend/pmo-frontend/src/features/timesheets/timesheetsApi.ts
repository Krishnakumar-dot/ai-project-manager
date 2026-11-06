import { api } from "../../lib/axiosClient";
import type { ApiResponse } from "../../lib/apiResponse";

export type TimesheetStatus = "Draft" | "Submitted" | "Approved" | "Rejected";

export interface TimesheetEntry {
  projectId: string;
  projectName: string;
  projectTaskId: string | null;
  taskTitle: string | null;
  date: string; // yyyy-mm-dd
  hours: number;
  isBillable: boolean;
  notes: string | null;
}

export interface Timesheet {
  id: string | null;
  weekStartDate: string;
  status: TimesheetStatus;
  reviewComment: string | null;
  totalHours: number;
  entries: TimesheetEntry[];
}

export interface PendingTimesheet {
  id: string;
  userName: string;
  weekStartDate: string;
  submittedAt: string | null;
  totalHours: number;
  billableHours: number;
  entries: TimesheetEntry[];
}

export interface SaveEntryInput {
  projectId: string;
  projectTaskId?: string;
  date: string;
  hours: number;
  isBillable: boolean;
}

export const getMyWeek = async (weekStart: string): Promise<ApiResponse<Timesheet>> => {
  const response = await api.get<ApiResponse<Timesheet>>("/timesheets/week", { params: { weekStart } });
  return response.data;
};

export const saveWeek = async (
  weekStart: string,
  entries: SaveEntryInput[]
): Promise<ApiResponse<string>> => {
  const response = await api.put<ApiResponse<string>>("/timesheets/week", { weekStart, entries });
  return response.data;
};

export const submitTimesheet = async (id: string): Promise<ApiResponse<string>> => {
  const response = await api.post<ApiResponse<string>>(`/timesheets/${id}/submit`);
  return response.data;
};

export const getPendingTimesheets = async (): Promise<ApiResponse<PendingTimesheet[]>> => {
  const response = await api.get<ApiResponse<PendingTimesheet[]>>("/timesheets/pending");
  return response.data;
};

export const reviewTimesheet = async (
  id: string,
  approve: boolean,
  comment?: string
): Promise<ApiResponse<string>> => {
  const response = await api.post<ApiResponse<string>>(`/timesheets/${id}/review`, { approve, comment });
  return response.data;
};