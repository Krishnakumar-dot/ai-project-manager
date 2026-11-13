import { api } from "../../lib/axiosClient";
import type { ApiResponse } from "../../lib/apiResponse";
import type { RaidType, RaidStatus, RiskLevel } from "./raidConstants";

export interface RaidItem {
  id: string;
  projectId: string;
  projectName: string;
  type: RaidType;
  title: string;
  description: string | null;
  impact: number;
  probability: number;
  score: number;
  level: RiskLevel;
  status: RaidStatus;
  ownerId: string | null;
  ownerName: string | null;
  dueDate: string | null; // yyyy-mm-dd
  mitigationPlan: string | null;
  rootCause: string | null;
  isEscalated: boolean;
  reportedByName: string;
  createdAt: string;
  resolvedAt: string | null;
  isOverdue: boolean;
}

export interface HeatMapCell {
  impact: number;
  probability: number;
  count: number;
}

export interface RaidSummary {
  activeTotal: number;
  activeRisks: number;
  activeAssumptions: number;
  activeIssues: number;
  activeDependencies: number;
  criticalCount: number;
  highCount: number;
  overdueCount: number;
  escalatedCount: number;
  heatMap: HeatMapCell[];
}

export interface RaidFilters {
  projectId?: string;
  type?: RaidType;
  activeOnly?: boolean;
}

export interface CreateRaidRequest {
  projectId: string;
  type: RaidType;
  title: string;
  description?: string;
  impact: number;
  probability: number;
  ownerId?: string;
  dueDate?: string;
  mitigationPlan?: string;
  rootCause?: string;
}

export interface UpdateRaidRequest {
  title: string;
  description?: string;
  impact: number;
  probability: number;
  status: RaidStatus;
  ownerId?: string;
  dueDate?: string;
  mitigationPlan?: string;
  rootCause?: string;
}

export const getRaidItems = async (filters: RaidFilters): Promise<ApiResponse<RaidItem[]>> => {
  const response = await api.get<ApiResponse<RaidItem[]>>("/raid", { params: filters });
  return response.data;
};

export const getRaidSummary = async (projectId?: string): Promise<ApiResponse<RaidSummary>> => {
  const response = await api.get<ApiResponse<RaidSummary>>("/raid/summary", { params: { projectId } });
  return response.data;
};

export const createRaidItem = async (data: CreateRaidRequest): Promise<ApiResponse<string>> => {
  const response = await api.post<ApiResponse<string>>("/raid", data);
  return response.data;
};

export const updateRaidItem = async (id: string, data: UpdateRaidRequest): Promise<ApiResponse<string>> => {
  const response = await api.put<ApiResponse<string>>(`/raid/${id}`, data);
  return response.data;
};

export const setRaidEscalation = async (id: string, escalate: boolean): Promise<ApiResponse<string>> => {
  const response = await api.put<ApiResponse<string>>(`/raid/${id}/escalation`, { escalate });
  return response.data;
};

export const deleteRaidItem = async (id: string): Promise<ApiResponse<string>> => {
  const response = await api.delete<ApiResponse<string>>(`/raid/${id}`);
  return response.data;
};