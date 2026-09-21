import { api } from "../../lib/axiosClient";

export interface LoginRequest { email: string; password: string; }
export interface LoginResponse { token: string; name: string; role: string; }

export const login = async (data: LoginRequest): Promise<LoginResponse> => {
  const response = await api.post<LoginResponse>("/Auth/login", data);
  return response.data;
};