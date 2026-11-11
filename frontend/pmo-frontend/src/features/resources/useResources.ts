import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getAllocations, createAllocation, deleteAllocation, getUtilization } from "./resourcesApi";
import type { CreateAllocationRequest } from "./resourcesApi";

export function useAllocations() {
  return useQuery({
    queryKey: ["allocations"],
    queryFn: async () => {
      const result = await getAllocations();
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useUtilization(weekStart: string) {
  return useQuery({
    queryKey: ["utilization", weekStart],
    queryFn: async () => {
      const result = await getUtilization(weekStart);
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

// Allocations change the utilization report too, so both caches are refreshed.
function useRefreshResources() {
  const queryClient = useQueryClient();
  return () => {
    queryClient.invalidateQueries({ queryKey: ["allocations"] });
    queryClient.invalidateQueries({ queryKey: ["utilization"] });
  };
}

export function useCreateAllocation() {
  const refresh = useRefreshResources();
  return useMutation({
    mutationFn: (data: CreateAllocationRequest) => createAllocation(data),
    onSuccess: (result) => {
      if (result.isSuccessful) refresh();
    },
  });
}

export function useDeleteAllocation() {
  const refresh = useRefreshResources();
  return useMutation({
    mutationFn: (id: string) => deleteAllocation(id),
    onSuccess: (result) => {
      if (result.isSuccessful) refresh();
    },
  });
}