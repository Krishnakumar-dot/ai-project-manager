import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getRaidItems, getRaidSummary, createRaidItem, updateRaidItem, setRaidEscalation, deleteRaidItem,
} from "./raidApi";
import type { RaidFilters, CreateRaidRequest, UpdateRaidRequest } from "./raidApi";

export function useRaidItems(filters: RaidFilters) {
  return useQuery({
    queryKey: ["raid", "items", filters],
    queryFn: async () => {
      const result = await getRaidItems(filters);
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useRaidSummary(projectId?: string) {
  return useQuery({
    queryKey: ["raid", "summary", projectId ?? "all"],
    queryFn: async () => {
      const result = await getRaidSummary(projectId);
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

// Every change can affect the list and the summary, so the whole "raid" cache is refreshed.
function useRefreshRaid() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: ["raid"] });
}

export function useCreateRaidItem() {
  const refresh = useRefreshRaid();
  return useMutation({
    mutationFn: (data: CreateRaidRequest) => createRaidItem(data),
    onSuccess: (result) => { if (result.isSuccessful) refresh(); },
  });
}

export function useUpdateRaidItem() {
  const refresh = useRefreshRaid();
  return useMutation({
    mutationFn: (v: { id: string; data: UpdateRaidRequest }) => updateRaidItem(v.id, v.data),
    onSuccess: (result) => { if (result.isSuccessful) refresh(); },
  });
}

export function useSetRaidEscalation() {
  const refresh = useRefreshRaid();
  return useMutation({
    mutationFn: (v: { id: string; escalate: boolean }) => setRaidEscalation(v.id, v.escalate),
    onSuccess: (result) => { if (result.isSuccessful) refresh(); },
  });
}

export function useDeleteRaidItem() {
  const refresh = useRefreshRaid();
  return useMutation({
    mutationFn: (id: string) => deleteRaidItem(id),
    onSuccess: (result) => { if (result.isSuccessful) refresh(); },
  });
}