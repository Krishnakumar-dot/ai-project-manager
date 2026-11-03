import { useState } from "react";
import { makeStyles, tokens, Text, Badge, Avatar } from "@fluentui/react-components";
import type { TaskItem, ProjectTaskStatus, TaskPriority } from "./tasksApi";

const columns: { status: ProjectTaskStatus; label: string }[] = [
  { status: "ToDo", label: "To Do" },
  { status: "InProgress", label: "In Progress" },
  { status: "InReview", label: "In Review" },
  { status: "Done", label: "Done" },
];

const priorityColor: Record<TaskPriority, "subtle" | "informative" | "warning" | "danger"> = {
  Low: "subtle",
  Medium: "informative",
  High: "warning",
  Critical: "danger",
};

const useStyles = makeStyles({
  board: {
    display: "grid",
    gridTemplateColumns: "repeat(4, minmax(250px, 1fr))",
    gap: "16px",
    overflowX: "auto",
    alignItems: "start",
  },
  column: {
    backgroundColor: tokens.colorNeutralBackground3,
    borderRadius: "8px",
    padding: "12px",
    minHeight: "320px",
    display: "flex",
    flexDirection: "column",
    gap: "10px",
    border: "2px solid transparent",
  },
  columnOver: {
    border: `2px dashed ${tokens.colorBrandStroke1}`,
    backgroundColor: tokens.colorBrandBackground2,
  },
  columnHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    padding: "2px 4px 6px",
  },
  card: {
    backgroundColor: tokens.colorNeutralBackground1,
    borderRadius: "6px",
    padding: "12px",
    cursor: "grab",
    boxShadow: tokens.shadow2,
    display: "flex",
    flexDirection: "column",
    gap: "8px",
    ":hover": { boxShadow: tokens.shadow8 },
  },
  cardDragging: { opacity: 0.4 },
  cardFooter: { display: "flex", justifyContent: "space-between", alignItems: "center" },
  muted: { color: tokens.colorNeutralForeground3, fontSize: "12px" },
  emptyHint: { color: tokens.colorNeutralForeground4, fontSize: "13px", textAlign: "center", padding: "20px 0" },
});

interface KanbanBoardProps {
  tasks: TaskItem[];
  onMove: (taskId: string, newStatus: ProjectTaskStatus) => void;
}

export function KanbanBoard({ tasks, onMove }: KanbanBoardProps) {
  const styles = useStyles();
  const [draggingId, setDraggingId] = useState<string | null>(null);
  const [overColumn, setOverColumn] = useState<ProjectTaskStatus | null>(null);

  const handleDrop = (e: React.DragEvent, status: ProjectTaskStatus) => {
    e.preventDefault();
    const id = e.dataTransfer.getData("text/plain");
    const task = tasks.find((t) => t.id === id);
    if (task && task.status !== status) onMove(id, status);
    setOverColumn(null);
    setDraggingId(null);
  };

  return (
    <div className={styles.board}>
      {columns.map((col) => {
        const columnTasks = tasks.filter((t) => t.status === col.status);
        const isOver = overColumn === col.status;

        return (
          <div
            key={col.status}
            className={`${styles.column} ${isOver ? styles.columnOver : ""}`}
            onDragOver={(e) => {
              e.preventDefault(); // required, otherwise the drop event never fires
              setOverColumn(col.status);
            }}
            onDragLeave={() => setOverColumn((current) => (current === col.status ? null : current))}
            onDrop={(e) => handleDrop(e, col.status)}
          >
            <div className={styles.columnHeader}>
              <Text weight="semibold">{col.label}</Text>
              <Badge appearance="tint" color="brand">{columnTasks.length}</Badge>
            </div>

            {columnTasks.length === 0 && <div className={styles.emptyHint}>Drop tasks here</div>}

            {columnTasks.map((task) => (
              <div
                key={task.id}
                draggable
                className={`${styles.card} ${draggingId === task.id ? styles.cardDragging : ""}`}
                onDragStart={(e) => {
                  e.dataTransfer.setData("text/plain", task.id);
                  e.dataTransfer.effectAllowed = "move";
                  setDraggingId(task.id);
                }}
                onDragEnd={() => {
                  setDraggingId(null);
                  setOverColumn(null);
                }}
              >
                <Text weight="semibold">{task.title}</Text>
                {task.description && <Text className={styles.muted}>{task.description}</Text>}
                <div className={styles.cardFooter}>
                  <Badge color={priorityColor[task.priority]} appearance="filled" size="small">
                    {task.priority}
                  </Badge>
                  <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                    {task.dueDate && (
                      <Text className={styles.muted}>
                        {new Date(task.dueDate).toLocaleDateString()}
                      </Text>
                    )}
                    {task.assigneeName && <Avatar name={task.assigneeName} size={24} color="colorful" />}
                  </div>
                </div>
              </div>
            ))}
          </div>
        );
      })}
    </div>
  );
}