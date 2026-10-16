import { api } from "../../lib/axiosClient";
import type { ApiResponse } from "../../lib/apiResponse";

export type ProjectStatus = "NotStarted" | "InProgress" | "OnHold" | "Completed" | "Cancelled";

export interface ProjectListItem {
  id: string;
  name: string;
  status: ProjectStatus;
  startDate: string;
  endDate: string | null;
  portfolioName: string;
}

export const getProjects = async (): Promise<ApiResponse<ProjectListItem[]>> => {
  const response = await api.get<ApiResponse<ProjectListItem[]>>("/projects");
  return response.data;
};

export interface CreateProjectRequest {
  name: string;
  description?: string;
  portfolioId: string;
  startDate: string;
  endDate?: string;
}

export const createProject = async (data: CreateProjectRequest): Promise<ApiResponse<string>> => {
  const response = await api.post<ApiResponse<string>>("/projects", data);
  return response.data;
};