import { useState } from "react";
import {
  Dialog, DialogSurface, DialogTitle, DialogBody, DialogActions, DialogContent,
  Button, Input, Field, Dropdown, Option, Textarea, Badge, Text,
} from "@fluentui/react-components";
import { useCreateRaidItem, useUpdateRaidItem } from "./useRaid";
import { useUsersList } from "../tasks/useTasks";
import { useProjectsList } from "../projects/useProjects";
import type { RaidItem } from "./raidApi";
import {
  RAID_TYPES, RAID_STATUSES, STATUS_LABEL, IMPACT_LABELS, PROBABILITY_LABELS,
  LEVEL_BADGE, levelFromScore,
} from "./raidConstants";
import type { RaidType, RaidStatus } from "./raidConstants";

interface Props {
  item?: RaidItem;          // present = edit mode
  defaultProjectId?: string;
  onClose: () => void;
}

const NO_OWNER = "none";

export function RaidItemDialog({ item, defaultProjectId, onClose }: Props) {
  const isEdit = !!item;

  const [projectId, setProjectId] = useState(item?.projectId ?? defaultProjectId ?? "");
  const [type, setType] = useState<RaidType>(item?.type ?? "Risk");
  const [title, setTitle] = useState(item?.title ?? "");
  const [description, setDescription] = useState(item?.description ?? "");
  const [impact, setImpact] = useState(item?.impact ?? 3);
  const [probability, setProbability] = useState(item?.probability ?? 3);
  const [status, setStatus] = useState<RaidStatus>(item?.status ?? "Open");
  const [ownerId, setOwnerId] = useState(item?.ownerId ?? NO_OWNER);
  const [dueDate, setDueDate] = useState(item?.dueDate ?? "");
  const [mitigationPlan, setMitigationPlan] = useState(item?.mitigationPlan ?? "");
  const [rootCause, setRootCause] = useState(item?.rootCause ?? "");
  const [error, setError] = useState("");

  const { data: projects } = useProjectsList();
  const { data: users } = useUsersList();
  const create = useCreateRaidItem();
  const update = useUpdateRaidItem();
  const busy = create.isPending || update.isPending;

  // An issue has already happened, so probability does not apply.
  const isIssue = type === "Issue";
  const effectiveProbability = isIssue ? 5 : probability;
  const score = impact * effectiveProbability;
  const level = levelFromScore(score);

  const handleSave = async () => {
    if (!projectId) { setError("Please select a project."); return; }
    if (!title.trim()) { setError("Title is required."); return; }
    setError("");

    const common = {
      title: title.trim(),
      description: description.trim() || undefined,
      impact,
      probability: effectiveProbability,
      ownerId: ownerId === NO_OWNER ? undefined : ownerId,
      dueDate: dueDate || undefined,
      mitigationPlan: mitigationPlan.trim() || undefined,
      rootCause: rootCause.trim() || undefined,
    };

    try {
      const result = isEdit
        ? await update.mutateAsync({ id: item!.id, data: { ...common, status } })
        : await create.mutateAsync({ ...common, projectId, type });

      if (result.isSuccessful) onClose();
      else setError(result.message);
    } catch {
      setError("Something went wrong. Please try again.");
    }
  };

  return (
    <Dialog open onOpenChange={(_, d) => { if (!d.open) onClose(); }}>
      <DialogSurface style={{ maxWidth: "640px", width: "92vw" }}>
        <DialogBody>
          <DialogTitle>{isEdit ? `Edit ${item!.type}` : "Log a new item"}</DialogTitle>
          <DialogContent style={{ display: "flex", flexDirection: "column", gap: "12px", maxHeight: "70vh", overflowY: "auto" }}>
            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "12px" }}>
              <Field label="Project" required>
                <Dropdown
                  placeholder="Select a project"
                  disabled={isEdit}
                  value={projects?.find((p) => p.id === projectId)?.name ?? ""}
                  selectedOptions={projectId ? [projectId] : []}
                  onOptionSelect={(_, d) => setProjectId(d.optionValue ?? "")}
                >
                  {projects?.map((p) => <Option key={p.id} value={p.id}>{p.name}</Option>)}
                </Dropdown>
              </Field>
              <Field label="Type" required>
                <Dropdown
                  disabled={isEdit}
                  value={type}
                  selectedOptions={[type]}
                  onOptionSelect={(_, d) => setType((d.optionValue as RaidType) ?? "Risk")}
                >
                  {RAID_TYPES.map((t) => <Option key={t} value={t}>{t}</Option>)}
                </Dropdown>
              </Field>
            </div>

            <Field label="Title" required>
              <Input value={title} onChange={(_, d) => setTitle(d.value)} />
            </Field>

            <Field label="Description">
              <Textarea value={description} onChange={(_, d) => setDescription(d.value)} />
            </Field>

            <div style={{ display: "grid", gridTemplateColumns: isIssue ? "1fr" : "1fr 1fr", gap: "12px" }}>
              <Field label="Impact" required>
                <Dropdown
                  value={IMPACT_LABELS[impact]}
                  selectedOptions={[String(impact)]}
                  onOptionSelect={(_, d) => setImpact(Number(d.optionValue))}
                >
                  {[1, 2, 3, 4, 5].map((n) => <Option key={n} value={String(n)} text={IMPACT_LABELS[n]}>{IMPACT_LABELS[n]}</Option>)}
                </Dropdown>
              </Field>
              {!isIssue && (
                <Field label="Probability" required>
                  <Dropdown
                    value={PROBABILITY_LABELS[probability]}
                    selectedOptions={[String(probability)]}
                    onOptionSelect={(_, d) => setProbability(Number(d.optionValue))}
                  >
                    {[1, 2, 3, 4, 5].map((n) => <Option key={n} value={String(n)} text={PROBABILITY_LABELS[n]}>{PROBABILITY_LABELS[n]}</Option>)}
                  </Dropdown>
                </Field>
              )}
            </div>

            <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
              <Text>Score:</Text>
              <Badge appearance="filled" color={LEVEL_BADGE[level]}>{score} — {level}</Badge>
              {isIssue && <Text size={200}>An issue has already happened, so its probability counts as 5.</Text>}
            </div>

            <div style={{ display: "grid", gridTemplateColumns: isEdit ? "1fr 1fr 1fr" : "1fr 1fr", gap: "12px" }}>
              <Field label="Owner">
                <Dropdown
                  value={users?.find((u) => u.id === ownerId)?.name ?? "Unassigned"}
                  selectedOptions={[ownerId]}
                  onOptionSelect={(_, d) => setOwnerId(d.optionValue ?? NO_OWNER)}
                >
                  <Option value={NO_OWNER}>Unassigned</Option>
                  {users?.map((u) => <Option key={u.id} value={u.id}>{u.name}</Option>)}
                </Dropdown>
              </Field>
              <Field label="Due date">
                <Input type="date" value={dueDate} onChange={(_, d) => setDueDate(d.value)} />
              </Field>
              {isEdit && (
                <Field label="Status">
                  <Dropdown
                    value={STATUS_LABEL[status]}
                    selectedOptions={[status]}
                    onOptionSelect={(_, d) => setStatus((d.optionValue as RaidStatus) ?? "Open")}
                  >
                    {RAID_STATUSES.map((s) => <Option key={s} value={s} text={STATUS_LABEL[s]}>{STATUS_LABEL[s]}</Option>)}
                  </Dropdown>
                </Field>
              )}
            </div>

            <Field label="Mitigation plan">
              <Textarea value={mitigationPlan} onChange={(_, d) => setMitigationPlan(d.value)} />
            </Field>

            {(type === "Issue" || type === "Risk") && (
              <Field label="Root cause">
                <Textarea value={rootCause} onChange={(_, d) => setRootCause(d.value)} />
              </Field>
            )}

            {error && <span style={{ color: "red", fontSize: "13px" }}>{error}</span>}
          </DialogContent>
          <DialogActions>
            <Button appearance="secondary" onClick={onClose}>Cancel</Button>
            <Button appearance="primary" onClick={handleSave} disabled={busy}>
              {busy ? "Saving..." : isEdit ? "Save changes" : "Create"}
            </Button>
          </DialogActions>
        </DialogBody>
      </DialogSurface>
    </Dialog>
  );
}