import { useState } from "react";
import {
  Dialog, DialogTrigger, DialogSurface, DialogTitle, DialogBody, DialogActions, DialogContent,
  Button, Input, Field, Dropdown, Option, Switch,
} from "@fluentui/react-components";
import { AddCircle24Regular } from "@fluentui/react-icons";
import { useCreateAllocation } from "./useResources";
import { useUsersList } from "../tasks/useTasks";
import { useProjectsList } from "../projects/useProjects";

export function CreateAllocationDialog() {
  const [open, setOpen] = useState(false);
  const [userId, setUserId] = useState("");
  const [projectId, setProjectId] = useState("");
  const [percent, setPercent] = useState("50");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [roleOnProject, setRoleOnProject] = useState("");
  const [allowOver, setAllowOver] = useState(false);
  const [error, setError] = useState("");

  const { data: users } = useUsersList();
  const { data: projects } = useProjectsList();
  const create = useCreateAllocation();

  const reset = () => {
    setUserId(""); setProjectId(""); setPercent("50");
    setStartDate(""); setEndDate(""); setRoleOnProject("");
    setAllowOver(false); setError("");
  };

  const handleCreate = () => {
    if (!userId || !projectId || !startDate || !endDate) {
      setError("Person, project, start date and end date are required.");
      return;
    }
    setError("");
    create.mutate(
      {
        userId,
        projectId,
        allocationPercent: Number(percent),
        startDate,
        endDate,
        roleOnProject: roleOnProject.trim() || undefined,
        allowOverallocation: allowOver,
      },
      {
        onSuccess: (result) => {
          if (result.isSuccessful) {
            setOpen(false);
            reset();
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
        <Button appearance="primary" icon={<AddCircle24Regular />}>New Allocation</Button>
      </DialogTrigger>
      <DialogSurface>
        <DialogBody>
          <DialogTitle>Allocate a person to a project</DialogTitle>
          <DialogContent style={{ display: "flex", flexDirection: "column", gap: "12px" }}>
            <Field label="Person" required>
              <Dropdown
                placeholder="Select a person"
                value={users?.find((u) => u.id === userId)?.name ?? ""}
                selectedOptions={userId ? [userId] : []}
                onOptionSelect={(_, d) => setUserId(d.optionValue ?? "")}
              >
                {users?.map((u) => <Option key={u.id} value={u.id}>{u.name}</Option>)}
              </Dropdown>
            </Field>
            <Field label="Project" required>
              <Dropdown
                placeholder="Select a project"
                value={projects?.find((p) => p.id === projectId)?.name ?? ""}
                selectedOptions={projectId ? [projectId] : []}
                onOptionSelect={(_, d) => setProjectId(d.optionValue ?? "")}
              >
                {projects?.map((p) => <Option key={p.id} value={p.id}>{p.name}</Option>)}
              </Dropdown>
            </Field>
            <Field label="Allocation (%)" required>
              <Input type="number" min={1} max={100} value={percent} onChange={(_, d) => setPercent(d.value)} />
            </Field>
            <Field label="Start date" required>
              <Input type="date" value={startDate} onChange={(_, d) => setStartDate(d.value)} />
            </Field>
            <Field label="End date" required>
              <Input type="date" value={endDate} onChange={(_, d) => setEndDate(d.value)} />
            </Field>
            <Field label="Role on project">
              <Input placeholder="e.g. Backend Developer" value={roleOnProject} onChange={(_, d) => setRoleOnProject(d.value)} />
            </Field>
            <Switch
              checked={allowOver}
              onChange={(_, d) => setAllowOver(d.checked)}
              label="Allow over-allocation (go past 100%)"
            />
            {error && <span style={{ color: "red", fontSize: "13px" }}>{error}</span>}
          </DialogContent>
          <DialogActions>
            <Button appearance="secondary" onClick={() => setOpen(false)}>Cancel</Button>
            <Button appearance="primary" onClick={handleCreate} disabled={create.isPending}>
              {create.isPending ? "Saving..." : "Create"}
            </Button>
          </DialogActions>
        </DialogBody>
      </DialogSurface>
    </Dialog>
  );
}