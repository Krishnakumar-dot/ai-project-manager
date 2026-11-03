import { useState } from "react";
import { makeStyles, tokens, Text, Spinner, Dropdown, Option } from "@fluentui/react-components";
import { useProjectsList } from "../projects/useProjects";
import { useTasksByProject, useUpdateTaskStatus } from "./useTasks";
import { KanbanBoard } from "./KanbanBoard";
import { CreateTaskDialog } from "./CreateTaskDialog";

const useStyles = makeStyles({
  header: { display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "20px" },
  controls: { display: "flex", alignItems: "center", gap: "12px" },
  empty: { textAlign: "center", padding: "60px 0", color: tokens.colorNeutralForeground3 },
});

export default function TasksPage() {
  const styles = useStyles();
  const { data: projects, isLoading: projectsLoading } = useProjectsList();
  const [selectedProjectId, setSelectedProjectId] = useState("");

  // Default to the first project until the user picks one.
  const activeProjectId = selectedProjectId || projects?.[0]?.id || "";
  const activeProject = projects?.find((p) => p.id === activeProjectId);

  const { data: tasks, isLoading: tasksLoading, isError, error } = useTasksByProject(activeProjectId);
  const updateStatus = useUpdateTaskStatus(activeProjectId);

  if (projectsLoading) return <Spinner label="Loading..." />;

  if (!projects || projects.length === 0) {
    return (
      <div className={styles.empty}>
        <Text size={500}>No projects yet</Text>
        <div style={{ marginTop: "4px" }}>
          <Text size={300}>Create a project first, then add tasks to it.</Text>
        </div>
      </div>
    );
  }

  return (
    <div>
      <div className={styles.header}>
        <Text as="h1" size={700} weight="bold">Tasks</Text>
        <div className={styles.controls}>
          <Dropdown
            style={{ minWidth: "240px" }}
            value={activeProject?.name ?? ""}
            selectedOptions={activeProjectId ? [activeProjectId] : []}
            onOptionSelect={(_, d) => setSelectedProjectId(d.optionValue ?? "")}
          >
            {projects.map((p) => (
              <Option key={p.id} value={p.id}>{p.name}</Option>
            ))}
          </Dropdown>
          <CreateTaskDialog projectId={activeProjectId} />
        </div>
      </div>

      {tasksLoading && <Spinner label="Loading tasks..." />}

      {isError && (
        <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
          {(error as Error).message || "Failed to load tasks."}
        </Text>
      )}

      {!tasksLoading && !isError && tasks && (
        <KanbanBoard
          tasks={tasks}
          onMove={(id, status) => updateStatus.mutate({ id, status })}
        />
      )}
    </div>
  );
}