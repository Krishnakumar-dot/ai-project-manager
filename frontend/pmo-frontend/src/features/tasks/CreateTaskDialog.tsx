import { useState } from "react";
import {
  Dialog, DialogTrigger, DialogSurface, DialogTitle, DialogBody, DialogActions, DialogContent,
  Button, Input, Field, Dropdown, Option, Textarea,
} from "@fluentui/react-components";
import { AddCircle24Regular } from "@fluentui/react-icons";
import { useCreateTask, useUsersList } from "./useTasks";
import type { TaskPriority } from "./tasksApi";

const priorities: TaskPriority[] = ["Low", "Medium", "High", "Critical"];

interface CreateTaskDialogProps {
  projectId: string;
}

export function CreateTaskDialog({ projectId }: CreateTaskDialogProps) {
  const [open, setOpen] = useState(false);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [priority, setPriority] = useState<TaskPriority>("Medium");
  const [dueDate, setDueDate] = useState("");
  const [assigneeId, setAssigneeId] = useState("");
  const [error, setError] = useState("");

  const { data: users } = useUsersList();
  const createTask = useCreateTask(projectId);

  const resetForm = () => {
    setTitle(""); setDescription(""); setPriority("Medium");
    setDueDate(""); setAssigneeId(""); setError("");
  };

  const handleCreate = () => {
    if (!title.trim()) {
      setError("Task title is required.");
      return;
    }
    setError("");
    createTask.mutate(
      {
        title: title.trim(),
        description: description.trim() || undefined,
        projectId,
        priority,
        dueDate: dueDate || undefined,
        assigneeId: assigneeId || undefined,
      },
      {
        onSuccess: (result) => {
          if (result.isSuccessful) {
            setOpen(false);
            resetForm();
          } else {
            setError(result.message);
          }
        },
        onError: () => setError("Something went wrong. Please try again."),
      }
    );
  };

  return (
    <Dialog open={open} onOpenChange={(_, data) => setOpen(data.open)}>
      <DialogTrigger disableButtonEnhancement>
        <Button appearance="primary" icon={<AddCircle24Regular />} disabled={!projectId}>
          New Task
        </Button>
      </DialogTrigger>
      <DialogSurface>
        <DialogBody>
          <DialogTitle>Create Task</DialogTitle>
          <DialogContent style={{ display: "flex", flexDirection: "column", gap: "12px" }}>
            <Field label="Title" required>
              <Input value={title} onChange={(_, d) => setTitle(d.value)} />
            </Field>
            <Field label="Description">
              <Textarea value={description} onChange={(_, d) => setDescription(d.value)} />
            </Field>
            <Field label="Priority">
              <Dropdown
                value={priority}
                selectedOptions={[priority]}
                onOptionSelect={(_, d) => setPriority((d.optionValue as TaskPriority) ?? "Medium")}
              >
                {priorities.map((p) => (
                  <Option key={p} value={p}>{p}</Option>
                ))}
              </Dropdown>
            </Field>
            <Field label="Assign to">
              <Dropdown
                placeholder="Unassigned"
                value={users?.find((u) => u.id === assigneeId)?.name ?? ""}
                selectedOptions={assigneeId ? [assigneeId] : []}
                onOptionSelect={(_, d) => setAssigneeId(d.optionValue ?? "")}
              >
                {users?.map((u) => (
                  <Option key={u.id} value={u.id}>{u.name}</Option>
                ))}
              </Dropdown>
            </Field>
            <Field label="Due date">
              <Input type="date" value={dueDate} onChange={(_, d) => setDueDate(d.value)} />
            </Field>
            {error && <span style={{ color: "red", fontSize: "13px" }}>{error}</span>}
          </DialogContent>
          <DialogActions>
            <Button appearance="secondary" onClick={() => setOpen(false)}>Cancel</Button>
            <Button appearance="primary" onClick={handleCreate} disabled={createTask.isPending}>
              {createTask.isPending ? "Creating..." : "Create"}
            </Button>
          </DialogActions>
        </DialogBody>
      </DialogSurface>
    </Dialog>
  );
}