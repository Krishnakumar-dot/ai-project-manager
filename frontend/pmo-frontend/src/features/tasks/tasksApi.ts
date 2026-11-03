import { api } from "../../lib/axiosClient";
import type { ApiResponse } from "../../lib/apiResponse";

export type ProjectTaskStatus = "ToDo" | "InProgress" | "InReview" | "Done";
export type TaskPriority = "Low" | "Medium" | "High" | "Critical";

export interface TaskItem {
  id: string;
  title: string;
  description: string | null;
  status: ProjectTaskStatus;
  priority: TaskPriority;
  dueDate: string | null;
  assigneeId: string | null;
  assigneeName: string | null;
}

export const getTasksByProject = async (projectId: string): Promise<ApiResponse<TaskItem[]>> => {
  const response = await api.get<ApiResponse<TaskItem[]>>(`/tasks/project/${projectId}`);
  return response.data;
};

export interface CreateTaskRequest {
  title: string;
  description?: string;
  projectId: string;
  priority: TaskPriority;
  dueDate?: string;
  assigneeId?: string;
}

export const createTask = async (data: CreateTaskRequest): Promise<ApiResponse<string>> => {
  const response = await api.post<ApiResponse<string>>("/tasks", data);
  return response.data;
};

export const updateTaskStatus = async (
  id: string,
  status: ProjectTaskStatus
): Promise<ApiResponse<string>> => {
  const response = await api.put<ApiResponse<string>>(`/tasks/${id}/status`, { status });
  return response.data;
};