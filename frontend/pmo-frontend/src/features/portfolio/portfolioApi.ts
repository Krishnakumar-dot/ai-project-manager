import { api } from "../../lib/axiosClient";
import { type ApiResponse } from "../../lib/apiResponse";

export interface PortfolioListItem {
  id: string;
  name: string;
  description: string | null;
  projectCount: number;
}

export const getPortfolios = async (): Promise<ApiResponse<PortfolioListItem[]>> => {
  const response = await api.get<ApiResponse<PortfolioListItem[]>>("/portfolio");
  return response.data;
};

export interface CreatePortfolioRequest {
  name: string;
  description?: string;
}

export const createPortfolio = async (data: CreatePortfolioRequest): Promise<ApiResponse<string>> => {
  const response = await api.post<ApiResponse<string>>("/portfolio", data);
  return response.data;
};