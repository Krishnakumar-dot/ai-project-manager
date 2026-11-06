import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getMyWeek, saveWeek, submitTimesheet, getPendingTimesheets, reviewTimesheet,
} from "./timesheetsApi";
import type { SaveEntryInput } from "./timesheetsApi";
import { isApprover } from "../auth/session";

export function useMyWeek(weekStart: string) {
  return useQuery({
    queryKey: ["timesheet", weekStart],
    queryFn: async () => {
      const result = await getMyWeek(weekStart);
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useSaveWeek(weekStart: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (entries: SaveEntryInput[]) => saveWeek(weekStart, entries),
    onSuccess: (result) => {
      if (result.isSuccessful) {
        queryClient.invalidateQueries({ queryKey: ["timesheet", weekStart] });
      }
    },
  });
}

export function useSubmitTimesheet(weekStart: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => submitTimesheet(id),
    onSuccess: (result) => {
      if (result.isSuccessful) {
        queryClient.invalidateQueries({ queryKey: ["timesheet", weekStart] });
        queryClient.invalidateQueries({ queryKey: ["timesheets", "pending"] });
      }
    },
  });
}

export function usePendingTimesheets() {
  return useQuery({
    queryKey: ["timesheets", "pending"],
    enabled: isApprover(), // never call the approver endpoint for normal users
    queryFn: async () => {
      const result = await getPendingTimesheets();
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useReviewTimesheet() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, approve, comment }: { id: string; approve: boolean; comment?: string }) =>
      reviewTimesheet(id, approve, comment),
    onSuccess: (result) => {
      if (result.isSuccessful) {
        queryClient.invalidateQueries({ queryKey: ["timesheets", "pending"] });
      }
    },
  });
}