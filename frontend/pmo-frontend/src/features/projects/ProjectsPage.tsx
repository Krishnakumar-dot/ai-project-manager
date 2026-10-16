import {
  makeStyles, tokens, Text, Card, Badge, Spinner,
} from "@fluentui/react-components";
import { useProjectsList } from "./useProjects";
import { CreateProjectDialog } from "./CreateProjectDialog";
import type { ProjectStatus } from "./projectsApi";

const useStyles = makeStyles({
  header: { display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "20px" },
  grid: { display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))", gap: "16px" },
  card: { padding: "18px", display: "flex", flexDirection: "column", gap: "10px" },
  cardTop: { display: "flex", justifyContent: "space-between", alignItems: "flex-start" },
  meta: { color: tokens.colorNeutralForeground3, fontSize: "13px" },
  empty: { textAlign: "center", padding: "60px 0", color: tokens.colorNeutralForeground3 },
});

const statusColor: Record<ProjectStatus, "informative" | "warning" | "success" | "danger" | "subtle"> = {
  NotStarted: "subtle",
  InProgress: "informative",
  OnHold: "warning",
  Completed: "success",
  Cancelled: "danger",
};

const statusLabel: Record<ProjectStatus, string> = {
  NotStarted: "Not Started",
  InProgress: "In Progress",
  OnHold: "On Hold",
  Completed: "Completed",
  Cancelled: "Cancelled",
};

export default function ProjectsPage() {
  const styles = useStyles();
  const { data: projects, isLoading, isError, error } = useProjectsList();

  return (
    <div>
      <div className={styles.header}>
        <Text as="h1" size={700} weight="bold">Projects</Text>
        <CreateProjectDialog />
      </div>

      {isLoading && <Spinner label="Loading projects..." />}

      {isError && (
        <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
          {(error as Error).message || "Failed to load projects."}
        </Text>
      )}

      {!isLoading && !isError && projects && projects.length === 0 && (
        <div className={styles.empty}>
          <Text size={500}>No projects yet</Text>
          <div style={{ marginTop: "4px" }}>
            <Text size={300}>Create your first project to get started.</Text>
          </div>
        </div>
      )}

      {!isLoading && !isError && projects && projects.length > 0 && (
        <div className={styles.grid}>
          {projects.map((project) => (
            <Card key={project.id} className={styles.card}>
              <div className={styles.cardTop}>
                <Text weight="semibold" size={400}>{project.name}</Text>
                <Badge color={statusColor[project.status]} appearance="filled">
                  {statusLabel[project.status]}
                </Badge>
              </div>
              <Text className={styles.meta}>{project.portfolioName}</Text>
              <Text className={styles.meta}>
                Starts {new Date(project.startDate).toLocaleDateString()}
                {project.endDate && ` · Ends ${new Date(project.endDate).toLocaleDateString()}`}
              </Text>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}