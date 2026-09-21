import { api } from "../../lib/axiosClient";

export interface ApiResponse<T> { isSuccessful: boolean; message: string; response: T; }
export interface LoginRequest { email: string; password: string; }
export interface LoginResult { token: string; name: string; role: string; }


export const login = async (data: LoginRequest): Promise<ApiResponse<LoginResult>> => {
  const response = await api.post<ApiResponse<LoginResult>>("/auth/login", data);
  return response.data;
};