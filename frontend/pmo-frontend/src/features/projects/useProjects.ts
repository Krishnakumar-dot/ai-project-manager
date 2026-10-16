import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getProjects, createProject, type CreateProjectRequest } from "./projectsApi";
import { getPortfolios } from "../portfolio/portfolioApi";

export function useProjectsList() {
  return useQuery({
    queryKey: ["projects"],
    queryFn: async () => {
      const result = await getProjects();
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function usePortfoliosList() {
  return useQuery({
    queryKey: ["portfolios"],
    queryFn: async () => {
      const result = await getPortfolios();
      if (!result.isSuccessful) throw new Error(result.message);
      return result.response;
    },
  });
}

export function useCreateProject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateProjectRequest) => createProject(data),
    onSuccess: (result) => {
      if (result.isSuccessful) {
        queryClient.invalidateQueries({ queryKey: ["projects"] }); // re-fetch the list automatically
      }
    },
  });
}