import { api } from "../../lib/axiosClient";
import type { ApiResponse } from "../../lib/apiResponse";

export type BudgetStatus = "NoBudget" | "OnTrack" | "AtRisk" | "OverBudget";

export interface ProjectFinancialSummary {
  projectId: string;
  projectName: string;
  portfolioName: string;
  budget: number | null;
  billingRatePerHour: number | null;
  actualCost: number;
  revenue: number;
  profit: number;
  marginPercent: number | null;
  budgetUsedPercent: number | null;
  forecastCost: number | null;
  approvedHours: number;
  billableHours: number;
  hoursMissingRate: number;
  budgetStatus: BudgetStatus;
}

export interface CostByPerson {
  userId: string;
  userName: string;
  hours: number;
  billableHours: number;
  hourlyCostRate: number | null;
  cost: number;
}

export interface WeeklyCost {
  weekStart: string; // yyyy-mm-dd
  cost: number;
  revenue: number;
}

export interface ProjectFinancialDetail {
  summary: ProjectFinancialSummary;
  byPerson: CostByPerson[];
  byWeek: WeeklyCost[];
}

export interface UserRate {
  id: string;
  name: string;
  email: string;
  role: string;
  hourlyCostRate: number | null;
}

export const getFinancialSummary = async (): Promise<ApiResponse<ProjectFinancialSummary[]>> => {
  const response = await api.get<ApiResponse<ProjectFinancialSummary[]>>("/financials/summary");
  return response.data;
};

export const getProjectFinancials = async (projectId: string): Promise<ApiResponse<ProjectFinancialDetail>> => {
  const response = await api.get<ApiResponse<ProjectFinancialDetail>>(`/financials/projects/${projectId}`);
  return response.data;
};

export const updateProjectFinancials = async (
  projectId: string,
  budget: number | null,
  billingRatePerHour: number | null
): Promise<ApiResponse<string>> => {
  const response = await api.put<ApiResponse<string>>(`/financials/projects/${projectId}`, {
    budget,
    billingRatePerHour,
  });
  return response.data;
};

export const getUserRates = async (): Promise<ApiResponse<UserRate[]>> => {
  const response = await api.get<ApiResponse<UserRate[]>>("/financials/rates");
  return response.data;
};

export const updateUserRate = async (
  userId: string,
  hourlyCostRate: number | null
): Promise<ApiResponse<string>> => {
  const response = await api.put<ApiResponse<string>>(`/financials/rates/${userId}`, { hourlyCostRate });
  return response.data;
};