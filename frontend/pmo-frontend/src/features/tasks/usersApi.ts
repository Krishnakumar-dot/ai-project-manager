import { api } from "../../lib/axiosClient";
import type { ApiResponse } from "../../lib/apiResponse";

export interface UserListItem {
  id: string;
  name: string;
  email: string;
  role: string;
}

export const getUsers = async (): Promise<ApiResponse<UserListItem[]>> => {
  const response = await api.get<ApiResponse<UserListItem[]>>("/users");
  return response.data;
};