import { useEffect, useMemo, useState } from "react";
import {
  makeStyles, tokens, Text, Button, Badge, Input, Spinner, Dropdown, Option, Switch,
  MessageBar, MessageBarBody,
  Table, TableHeader, TableHeaderCell, TableRow, TableBody, TableCell,
} from "@fluentui/react-components";
import {
  ChevronLeft24Regular, ChevronRight24Regular, Add24Regular, Delete24Regular,
} from "@fluentui/react-icons";
import { useProjectsList } from "../projects/useProjects";
import { useTasksByProject } from "../tasks/useTasks";
import { useMyWeek, useSaveWeek, useSubmitTimesheet } from "./useTimesheets";
import type { TimesheetEntry, TimesheetStatus, SaveEntryInput } from "./timesheetsApi";
import { addDays, parseIsoDate, startOfWeek, toIsoDate } from "../../lib/dateUtils";

const useStyles = makeStyles({
  header: { display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "16px", flexWrap: "wrap", gap: "12px" },
  weekNav: { display: "flex", alignItems: "center", gap: "8px" },
  actions: { display: "flex", alignItems: "center", gap: "8px" },
  addBar: {
    display: "flex", alignItems: "center", gap: "12px", flexWrap: "wrap",
    padding: "12px", marginBottom: "16px",
    backgroundColor: tokens.colorNeutralBackground1,
    border: `1px solid ${tokens.colorNeutralStroke2}`, borderRadius: "8px",
  },
  tableWrap: {
    backgroundColor: tokens.colorNeutralBackground1,
    border: `1px solid ${tokens.colorNeutralStroke2}`, borderRadius: "8px",
    overflowX: "auto", padding: "4px",
  },
  total: { fontWeight: 600 },
  over: { color: tokens.colorPaletteRedForeground1, fontWeight: 700 },
  muted: { color: tokens.colorNeutralForeground3 },
});

interface GridRow {
  key: string;
  projectId: string;
  projectName: string;
  taskId: string | null;
  taskTitle: string | null;
  isBillable: boolean;
  hours: Record<string, string>; // date (yyyy-mm-dd) -> hours as typed
}

const makeKey = (projectId: string, taskId: string | null, billable: boolean) =>
  `${projectId}|${taskId ?? ""}|${billable ? "B" : "N"}`;

function rowsFromEntries(entries: TimesheetEntry[]): GridRow[] {
  const map = new Map<string, GridRow>();
  for (const e of entries) {
    const key = makeKey(e.projectId, e.projectTaskId, e.isBillable);
    let row = map.get(key);
    if (!row) {
      row = {
        key, projectId: e.projectId, projectName: e.projectName,
        taskId: e.projectTaskId, taskTitle: e.taskTitle,
        isBillable: e.isBillable, hours: {},
      };
      map.set(key, row);
    }
    row.hours[e.date] = String(e.hours);
  }
  return Array.from(map.values());
}

function buildEntries(rows: GridRow[], days: string[]): SaveEntryInput[] {
  const entries: SaveEntryInput[] = [];
  for (const row of rows) {
    for (const day of days) {
      const hours = Number(row.hours[day] || 0);
      if (hours > 0) {
        entries.push({
          projectId: row.projectId,
          projectTaskId: row.taskId ?? undefined,
          date: day,
          hours,
          isBillable: row.isBillable,
        });
      }
    }
  }
  return entries;
}

const statusColor: Record<TimesheetStatus, "subtle" | "informative" | "success" | "danger"> = {
  Draft: "subtle", Submitted: "informative", Approved: "success", Rejected: "danger",
};

function AddRowBar({ onAdd }: { onAdd: (row: Omit<GridRow, "hours">) => void }) {
  const styles = useStyles();
  const { data: projects } = useProjectsList();
  const [projectId, setProjectId] = useState("");
  const [taskId, setTaskId] = useState("none");
  const [billable, setBillable] = useState(true);
  const { data: tasks } = useTasksByProject(projectId);

  const project = projects?.find((p) => p.id === projectId);
  const task = tasks?.find((t) => t.id === taskId);

  const handleAdd = () => {
    if (!project) return;
    const realTaskId = taskId === "none" ? null : taskId;
    onAdd({
      key: makeKey(project.id, realTaskId, billable),
      projectId: project.id,
      projectName: project.name,
      taskId: realTaskId,
      taskTitle: realTaskId ? task?.title ?? null : null,
      isBillable: billable,
    });
  };

  return (
    <div className={styles.addBar}>
      <Dropdown
        placeholder="Select project"
        style={{ minWidth: "220px" }}
        value={project?.name ?? ""}
        selectedOptions={projectId ? [projectId] : []}
        onOptionSelect={(_, d) => { setProjectId(d.optionValue ?? ""); setTaskId("none"); }}
      >
        {projects?.map((p) => <Option key={p.id} value={p.id}>{p.name}</Option>)}
      </Dropdown>

      <Dropdown
        style={{ minWidth: "220px" }}
        disabled={!projectId}
        value={taskId === "none" ? "No specific task" : task?.title ?? ""}
        selectedOptions={[taskId]}
        onOptionSelect={(_, d) => setTaskId(d.optionValue ?? "none")}
      >
        <Option value="none">No specific task</Option>
        {tasks?.map((t) => <Option key={t.id} value={t.id}>{t.title}</Option>)}
      </Dropdown>

      <Switch checked={billable} onChange={(_, d) => setBillable(d.checked)} label="Billable" />

      <Button appearance="secondary" icon={<Add24Regular />} disabled={!projectId} onClick={handleAdd}>
        Add row
      </Button>
    </div>
  );
}

export default function TimesheetPage() {
  const styles = useStyles();
  const [weekStart, setWeekStart] = useState(() => startOfWeek(new Date()));
  const weekKey = toIsoDate(weekStart);
  const days = useMemo(
    () => Array.from({ length: 7 }, (_, i) => toIsoDate(addDays(weekStart, i))),
    [weekKey] // eslint-disable-line react-hooks/exhaustive-deps
  );

  const { data: sheet, isLoading, isError, error } = useMyWeek(weekKey);
  const save = useSaveWeek(weekKey);
  const submit = useSubmitTimesheet(weekKey);

  const [rows, setRows] = useState<GridRow[]>([]);
  const [dirty, setDirty] = useState(false);
  const [feedback, setFeedback] = useState<{ intent: "success" | "error"; text: string } | null>(null);

  useEffect(() => {
    if (sheet) {
      setRows(rowsFromEntries(sheet.entries));
      setDirty(false);
    }
  }, [sheet]);

  const status: TimesheetStatus = sheet?.status ?? "Draft";
  const editable = status === "Draft" || status === "Rejected";
  const busy = save.isPending || submit.isPending;

  const rowTotal = (row: GridRow) => days.reduce((sum, d) => sum + Number(row.hours[d] || 0), 0);
  const dayTotal = (day: string) => rows.reduce((sum, r) => sum + Number(r.hours[day] || 0), 0);
  const grandTotal = days.reduce((sum, d) => sum + dayTotal(d), 0);

  const changeWeek = (offset: number) => {
    if (dirty && !window.confirm("You have unsaved changes. Leave this week anyway?")) return;
    setFeedback(null);
    setWeekStart((current) => addDays(current, offset * 7));
  };

  const setHours = (rowKey: string, day: string, value: string) => {
    setRows((prev) => prev.map((r) => (r.key === rowKey ? { ...r, hours: { ...r.hours, [day]: value } } : r)));
    setDirty(true);
  };

  const addRow = (row: Omit<GridRow, "hours">) => {
    if (rows.some((r) => r.key === row.key)) {
      setFeedback({ intent: "error", text: "That row is already in the grid." });
      return;
    }
    setRows((prev) => [...prev, { ...row, hours: {} }]);
    setDirty(true);
    setFeedback(null);
  };

  const removeRow = (rowKey: string) => {
    setRows((prev) => prev.filter((r) => r.key !== rowKey));
    setDirty(true);
  };

  // Saves the grid. Returns the sheet id, or null if saving failed.
  const runSave = async (): Promise<string | null> => {
    try {
      const result = await save.mutateAsync(buildEntries(rows, days));
      if (!result.isSuccessful) {
        setFeedback({ intent: "error", text: result.message });
        return null;
      }
      setDirty(false);
      return result.response;
    } catch {
      setFeedback({ intent: "error", text: "Could not save the timesheet. Please try again." });
      return null;
    }
  };

  const handleSave = async () => {
    const id = await runSave();
    if (id) setFeedback({ intent: "success", text: "Timesheet saved." });
  };

  const handleSubmit = async () => {
    const id = dirty || !sheet?.id ? await runSave() : sheet.id;
    if (!id) return;
    try {
      const result = await submit.mutateAsync(id);
      setFeedback(
        result.isSuccessful
          ? { intent: "success", text: "Timesheet submitted for approval." }
          : { intent: "error", text: result.message }
      );
    } catch {
      setFeedback({ intent: "error", text: "Could not submit the timesheet. Please try again." });
    }
  };

  const weekEnd = addDays(weekStart, 6);
  const rangeLabel = `${weekStart.toLocaleDateString(undefined, { day: "numeric", month: "short" })} – ${weekEnd.toLocaleDateString(undefined, { day: "numeric", month: "short", year: "numeric" })}`;

  return (
    <div>
      <div className={styles.header}>
        <div className={styles.weekNav}>
          <Text as="h1" size={700} weight="bold">Timesheet</Text>
          <Button appearance="subtle" icon={<ChevronLeft24Regular />} onClick={() => changeWeek(-1)} aria-label="Previous week" />
          <Text weight="semibold">{rangeLabel}</Text>
          <Button appearance="subtle" icon={<ChevronRight24Regular />} onClick={() => changeWeek(1)} aria-label="Next week" />
          <Button appearance="subtle" onClick={() => { setFeedback(null); setWeekStart(startOfWeek(new Date())); }}>
            This week
          </Button>
          <Badge color={statusColor[status]} appearance="filled">{status}</Badge>
        </div>
        <div className={styles.actions}>
          {dirty && <Text className={styles.muted}>Unsaved changes</Text>}
          <Button appearance="secondary" onClick={handleSave} disabled={!editable || busy || !dirty}>
            {save.isPending ? "Saving..." : "Save draft"}
          </Button>
          <Button appearance="primary" onClick={handleSubmit} disabled={!editable || busy || rows.length === 0}>
            {submit.isPending ? "Submitting..." : "Submit for approval"}
          </Button>
        </div>
      </div>

      {feedback && (
        <MessageBar intent={feedback.intent} style={{ marginBottom: "12px" }}>
          <MessageBarBody>{feedback.text}</MessageBarBody>
        </MessageBar>
      )}

      {status === "Rejected" && sheet?.reviewComment && (
        <MessageBar intent="warning" style={{ marginBottom: "12px" }}>
          <MessageBarBody>Rejected: {sheet.reviewComment}. Edit the hours and submit again.</MessageBarBody>
        </MessageBar>
      )}

      {status === "Submitted" && (
        <MessageBar intent="info" style={{ marginBottom: "12px" }}>
          <MessageBarBody>Submitted. Waiting for approval, so this week is locked.</MessageBarBody>
        </MessageBar>
      )}

      {status === "Approved" && (
        <MessageBar intent="success" style={{ marginBottom: "12px" }}>
          <MessageBarBody>This timesheet has been approved.</MessageBarBody>
        </MessageBar>
      )}

      {editable && <AddRowBar onAdd={addRow} />}

      {isLoading && <Spinner label="Loading timesheet..." />}
      {isError && (
        <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
          {(error as Error).message || "Failed to load the timesheet."}
        </Text>
      )}

      {!isLoading && !isError && (
        <div className={styles.tableWrap}>
          <Table size="small" aria-label="Weekly timesheet">
            <TableHeader>
              <TableRow>
                <TableHeaderCell style={{ minWidth: "220px" }}>Project / Task</TableHeaderCell>
                <TableHeaderCell>Type</TableHeaderCell>
                {days.map((d) => (
                  <TableHeaderCell key={d}>
                    {parseIsoDate(d).toLocaleDateString(undefined, { weekday: "short", day: "numeric", month: "short" })}
                  </TableHeaderCell>
                ))}
                <TableHeaderCell>Total</TableHeaderCell>
                <TableHeaderCell />
              </TableRow>
            </TableHeader>
            <TableBody>
              {rows.length === 0 && (
                <TableRow>
                  <TableCell colSpan={11}>
                    <Text className={styles.muted}>
                      {editable ? "No rows yet. Pick a project above and click Add row." : "No hours were logged this week."}
                    </Text>
                  </TableCell>
                </TableRow>
              )}

              {rows.map((row) => (
                <TableRow key={row.key}>
                  <TableCell>
                    <div>
                      <Text weight="semibold">{row.projectName}</Text>
                      {row.taskTitle && <div><Text className={styles.muted} size={200}>{row.taskTitle}</Text></div>}
                    </div>
                  </TableCell>
                  <TableCell>
                    <Badge appearance="tint" color={row.isBillable ? "success" : "subtle"}>
                      {row.isBillable ? "Billable" : "Non-billable"}
                    </Badge>
                  </TableCell>
                  {days.map((d) => (
                    <TableCell key={d}>
                      <Input
                        type="number" size="small" min={0} max={24} step={0.25}
                        style={{ width: "76px" }}
                        value={row.hours[d] ?? ""}
                        disabled={!editable}
                        onChange={(_, data) => setHours(row.key, d, data.value)}
                      />
                    </TableCell>
                  ))}
                  <TableCell><Text className={styles.total}>{rowTotal(row)}</Text></TableCell>
                  <TableCell>
                    {editable && (
                      <Button appearance="subtle" icon={<Delete24Regular />} onClick={() => removeRow(row.key)} aria-label="Remove row" />
                    )}
                  </TableCell>
                </TableRow>
              ))}

              {rows.length > 0 && (
                <TableRow>
                  <TableCell><Text className={styles.total}>Daily total</Text></TableCell>
                  <TableCell />
                  {days.map((d) => (
                    <TableCell key={d}>
                      <Text className={dayTotal(d) > 24 ? styles.over : styles.total}>{dayTotal(d)}</Text>
                    </TableCell>
                  ))}
                  <TableCell><Text className={styles.total}>{grandTotal}</Text></TableCell>
                  <TableCell />
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      )}
    </div>
  );
}