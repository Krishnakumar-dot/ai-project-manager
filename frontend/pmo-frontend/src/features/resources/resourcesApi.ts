import { api } from "../../lib/axiosClient";
import type { ApiResponse } from "../../lib/apiResponse";

export type UtilizationStatus = "Bench" | "Allocated" | "Overallocated";

export interface Allocation {
  id: string;
  userId: string;
  userName: string;
  projectId: string;
  projectName: string;
  allocationPercent: number;
  startDate: string; // yyyy-mm-dd
  endDate: string;
  roleOnProject: string | null;
}

export interface Utilization {
  userId: string;
  userName: string;
  allocatedPercent: number;
  capacityHours: number;
  plannedHours: number;
  loggedHours: number;
  status: UtilizationStatus;
  projects: string[];
}

export interface CreateAllocationRequest {
  userId: string;
  projectId: string;
  allocationPercent: number;
  startDate: string;
  endDate: string;
  roleOnProject?: string;
  allowOverallocation: boolean;
}

export const getAllocations = async (): Promise<ApiResponse<Allocation[]>> => {
  const response = await api.get<ApiResponse<Allocation[]>>("/resources/allocations");
  return response.data;
};

export const createAllocation = async (data: CreateAllocationRequest): Promise<ApiResponse<string>> => {
  const response = await api.post<ApiResponse<string>>("/resources/allocations", data);
  return response.data;
};

export const deleteAllocation = async (id: string): Promise<ApiResponse<string>> => {
  const response = await api.delete<ApiResponse<string>>(`/resources/allocations/${id}`);
  return response.data;
};

export const getUtilization = async (weekStart: string): Promise<ApiResponse<Utilization[]>> => {
  const response = await api.get<ApiResponse<Utilization[]>>("/resources/utilization", { params: { weekStart } });
  return response.data;
};