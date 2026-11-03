import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getTasksByProject, createTask, updateTaskStatus,
} from "./tasksApi";
import type { CreateTaskRequest, ProjectTaskStatus, TaskItem } from "./tasksApi";
import { getUsers } from "./usersApi";

export function useTasksByProject(projectId: string) {
  return useQuery({
    queryKey: ["tasks", projectId],
    enabled: !!projectId,
    queryFn: async () => {
      const result = await getTasksByProject(projectId);
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useUsersList() {
  return useQuery({
    queryKey: ["users"],
    queryFn: async () => {
      const result = await getUsers();
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useCreateTask(projectId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateTaskRequest) => createTask(data),
    onSuccess: (result) => {
      if (result.isSuccessful) {
        queryClient.invalidateQueries({ queryKey: ["tasks", projectId] });
      }
    },
  });
}

// Optimistic update: the card moves instantly on drop, and rolls back if the server rejects it.
export function useUpdateTaskStatus(projectId: string) {
  const queryClient = useQueryClient();
  const key = ["tasks", projectId];

  return useMutation({
    mutationFn: async ({ id, status }: { id: string; status: ProjectTaskStatus }) => {
      const result = await updateTaskStatus(id, status);
      if (!result.isSuccessful) throw new Error(result.message);
      return result;
    },
    onMutate: async ({ id, status }) => {
      await queryClient.cancelQueries({ queryKey: key });
      const previous = queryClient.getQueryData<TaskItem[]>(key);
      queryClient.setQueryData<TaskItem[]>(key, (old) =>
        old?.map((t) => (t.id === id ? { ...t, status } : t))
      );
      return { previous };
    },
    onError: (_error, _variables, context) => {
      if (context?.previous) queryClient.setQueryData(key, context.previous);
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: key });
    },
  });
}