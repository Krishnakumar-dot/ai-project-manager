import { useState } from "react";
import {
  Dialog, DialogTrigger, DialogSurface, DialogTitle, DialogBody, DialogActions, DialogContent,
  Button, Input, Field, Dropdown, Option, Spinner,
} from "@fluentui/react-components";
import { AddCircle24Regular } from "@fluentui/react-icons";
import { useCreateProject, usePortfoliosList } from "./useProjects";

export function CreateProjectDialog() {
  const [open, setOpen] = useState(false);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [portfolioId, setPortfolioId] = useState("");
  const [startDate, setStartDate] = useState("");
  const [error, setError] = useState("");

const { data: portfolios, isLoading: portfoliosLoading, isError: portfoliosError } = usePortfoliosList();  const createProject = useCreateProject();

  const resetForm = () => {
    setName(""); setDescription(""); setPortfolioId(""); setStartDate(""); setError("");
  };

  const handleCreate = () => {
    if (!name || !portfolioId || !startDate) {
      setError("Name, portfolio, and start date are required.");
      return;
    }
    createProject.mutate(
      { name, description, portfolioId, startDate },
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
        <Button appearance="primary" icon={<AddCircle24Regular />}>New Project</Button>
      </DialogTrigger>
      <DialogSurface>
        <DialogBody>
          <DialogTitle>Create Project</DialogTitle>
          <DialogContent style={{ display: "flex", flexDirection: "column", gap: "12px" }}>
            <Field label="Project Name" required>
              <Input value={name} onChange={(_, d) => setName(d.value)} />
            </Field>
            <Field label="Description">
              <Input value={description} onChange={(_, d) => setDescription(d.value)} />
            </Field>
           <Field
  label="Portfolio"
  required
  validationState={portfoliosError ? "error" : "none"}
  validationMessage={
    portfoliosError
      ? "Could not load portfolios."
      : portfolios && portfolios.length === 0
      ? "No portfolios yet. Create one first."
      : undefined
  }
>
  {portfoliosLoading ? (
    <Spinner size="tiny" />
  ) : (
    <Dropdown
      placeholder="Select a portfolio"
      value={portfolios?.find((p) => p.id === portfolioId)?.name ?? ""}
      selectedOptions={portfolioId ? [portfolioId] : []}
      onOptionSelect={(_, d) => setPortfolioId(d.optionValue ?? "")}
    >
      {portfolios?.map((p) => (
        <Option key={p.id} value={p.id}>{p.name}</Option>
      ))}
    </Dropdown>
  )}
</Field>
            <Field label="Start Date" required>
              <Input type="date" value={startDate} onChange={(_, d) => setStartDate(d.value)} />
            </Field>
            {error && <span style={{ color: "red", fontSize: "13px" }}>{error}</span>}
          </DialogContent>
          <DialogActions>
            <Button appearance="secondary" onClick={() => setOpen(false)}>Cancel</Button>
            <Button appearance="primary" onClick={handleCreate} disabled={createProject.isPending}>
              {createProject.isPending ? "Creating..." : "Create"}
            </Button>
          </DialogActions>
        </DialogBody>
      </DialogSurface>
    </Dialog>
  );
}