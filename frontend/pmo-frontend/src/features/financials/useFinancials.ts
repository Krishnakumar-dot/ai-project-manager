import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getFinancialSummary, getProjectFinancials, updateProjectFinancials, getUserRates, updateUserRate,
} from "./financialsApi";
import { canManageRates } from "../auth/session";

export function useFinancialSummary() {
  return useQuery({
    queryKey: ["financials", "summary"],
    queryFn: async () => {
      const result = await getFinancialSummary();
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useProjectFinancials(projectId: string | null) {
  return useQuery({
    queryKey: ["financials", "project", projectId],
    enabled: !!projectId,
    queryFn: async () => {
      const result = await getProjectFinancials(projectId!);
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useUserRates() {
  return useQuery({
    queryKey: ["financials", "rates"],
    enabled: canManageRates(), // never call the rates endpoint for people who can't use it
    queryFn: async () => {
      const result = await getUserRates();
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

// Any money change can affect every figure, so the whole "financials" cache is refreshed.
export function useUpdateProjectFinancials() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (v: { projectId: string; budget: number | null; billingRatePerHour: number | null }) =>
      updateProjectFinancials(v.projectId, v.budget, v.billingRatePerHour),
    onSuccess: (result) => {
      if (result.isSuccessful) queryClient.invalidateQueries({ queryKey: ["financials"] });
    },
  });
}

export function useUpdateUserRate() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (v: { userId: string; hourlyCostRate: number | null }) =>
      updateUserRate(v.userId, v.hourlyCostRate),
    onSuccess: (result) => {
      if (result.isSuccessful) queryClient.invalidateQueries({ queryKey: ["financials"] });
    },
  });
}