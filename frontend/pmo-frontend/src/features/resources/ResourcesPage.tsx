import { useState } from "react";
import {
  makeStyles, tokens, Text, Button, Badge, Spinner, ProgressBar, Card, TabList, Tab,
  Table, TableHeader, TableHeaderCell, TableRow, TableBody, TableCell,
} from "@fluentui/react-components";
import { ChevronLeft24Regular, ChevronRight24Regular, Delete24Regular } from "@fluentui/react-icons";
import { useAllocations, useDeleteAllocation, useUtilization } from "./useResources";
import { CreateAllocationDialog } from "./CreateAllocationDialog";
import type { UtilizationStatus } from "./resourcesApi";
import { addDays, parseIsoDate, startOfWeek, toIsoDate } from "../../lib/dateUtils";

const useStyles = makeStyles({
  header: { display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "16px" },
  weekNav: { display: "flex", alignItems: "center", gap: "8px", marginBottom: "16px" },
  summary: { display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(180px, 1fr))", gap: "16px", marginBottom: "16px" },
  summaryCard: { padding: "16px", display: "flex", flexDirection: "column", gap: "4px" },
  summaryValue: { fontSize: "28px", fontWeight: 700 },
  tableWrap: {
    backgroundColor: tokens.colorNeutralBackground1,
    border: `1px solid ${tokens.colorNeutralStroke2}`,
    borderRadius: "8px", overflowX: "auto", padding: "4px",
  },
  projects: { display: "flex", gap: "4px", flexWrap: "wrap" },
  muted: { color: tokens.colorNeutralForeground3 },
  empty: { textAlign: "center", padding: "50px 0", color: tokens.colorNeutralForeground3 },
});

const statusColor: Record<UtilizationStatus, "subtle" | "success" | "danger"> = {
  Bench: "subtle",
  Allocated: "success",
  Overallocated: "danger",
};

function barColor(percent: number): "success" | "warning" | "error" {
  if (percent > 100) return "error";
  if (percent >= 90) return "warning";
  return "success";
}

function UtilizationView() {
  const styles = useStyles();
  const [weekStart, setWeekStart] = useState(() => startOfWeek(new Date()));
  const weekKey = toIsoDate(weekStart);
  const { data, isLoading, isError, error } = useUtilization(weekKey);

  const weekEnd = addDays(weekStart, 6);
  const rangeLabel = `${weekStart.toLocaleDateString(undefined, { day: "numeric", month: "short" })} – ${weekEnd.toLocaleDateString(undefined, { day: "numeric", month: "short", year: "numeric" })}`;

  const total = data?.length ?? 0;
  const overCount = data?.filter((r) => r.status === "Overallocated").length ?? 0;
  const benchCount = data?.filter((r) => r.status === "Bench").length ?? 0;
  const avgAllocation = total > 0 ? Math.round(data!.reduce((s, r) => s + r.allocatedPercent, 0) / total) : 0;

  return (
    <div>
      <div className={styles.weekNav}>
        <Button appearance="subtle" icon={<ChevronLeft24Regular />} onClick={() => setWeekStart((w) => addDays(w, -7))} aria-label="Previous week" />
        <Text weight="semibold">{rangeLabel}</Text>
        <Button appearance="subtle" icon={<ChevronRight24Regular />} onClick={() => setWeekStart((w) => addDays(w, 7))} aria-label="Next week" />
        <Button appearance="subtle" onClick={() => setWeekStart(startOfWeek(new Date()))}>This week</Button>
      </div>

      {isLoading && <Spinner label="Loading utilization..." />}
      {isError && (
        <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
          {(error as Error).message || "Failed to load utilization."}
        </Text>
      )}

      {!isLoading && !isError && data && (
        <>
          <div className={styles.summary}>
            <Card className={styles.summaryCard}>
              <Text className={styles.muted} size={300}>Resources</Text>
              <Text className={styles.summaryValue}>{total}</Text>
            </Card>
            <Card className={styles.summaryCard}>
              <Text className={styles.muted} size={300}>Average allocation</Text>
              <Text className={styles.summaryValue}>{avgAllocation}%</Text>
            </Card>
            <Card className={styles.summaryCard}>
              <Text className={styles.muted} size={300}>Overallocated</Text>
              <Text className={styles.summaryValue} style={{ color: overCount > 0 ? tokens.colorPaletteRedForeground1 : undefined }}>
                {overCount}
              </Text>
            </Card>
            <Card className={styles.summaryCard}>
              <Text className={styles.muted} size={300}>On bench</Text>
              <Text className={styles.summaryValue}>{benchCount}</Text>
            </Card>
          </div>

          <div className={styles.tableWrap}>
            <Table size="small" aria-label="Weekly utilization">
              <TableHeader>
                <TableRow>
                  <TableHeaderCell>Person</TableHeaderCell>
                  <TableHeaderCell>Projects</TableHeaderCell>
                  <TableHeaderCell style={{ minWidth: "200px" }}>Allocated</TableHeaderCell>
                  <TableHeaderCell>Planned hrs</TableHeaderCell>
                  <TableHeaderCell>Logged hrs</TableHeaderCell>
                  <TableHeaderCell>Status</TableHeaderCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {data.map((r) => (
                  <TableRow key={r.userId}>
                    <TableCell><Text weight="semibold">{r.userName}</Text></TableCell>
                    <TableCell>
                      <div className={styles.projects}>
                        {r.projects.length === 0
                          ? <Text className={styles.muted}>None</Text>
                          : r.projects.map((p) => <Badge key={p} appearance="tint" color="informative">{p}</Badge>)}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div style={{ display: "flex", alignItems: "center", gap: "10px" }}>
                        <div style={{ flex: 1 }}>
                          <ProgressBar
                            value={Math.min(r.allocatedPercent, 100) / 100}
                            max={1}
                            color={barColor(r.allocatedPercent)}
                            thickness="large"
                          />
                        </div>
                        <Text weight="semibold">{r.allocatedPercent}%</Text>
                      </div>
                    </TableCell>
                    <TableCell>{r.plannedHours} / {r.capacityHours}</TableCell>
                    <TableCell>{r.loggedHours}</TableCell>
                    <TableCell>
                      <Badge appearance="filled" color={statusColor[r.status]}>{r.status}</Badge>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </>
      )}
    </div>
  );
}

function AllocationsView() {
  const styles = useStyles();
  const { data, isLoading, isError, error } = useAllocations();
  const remove = useDeleteAllocation();

  const handleDelete = (id: string, label: string) => {
    if (window.confirm(`Remove the allocation for ${label}?`)) remove.mutate(id);
  };

  const fmt = (iso: string) => parseIsoDate(iso).toLocaleDateString(undefined, { day: "numeric", month: "short", year: "numeric" });

  return (
    <div>
      <div style={{ display: "flex", justifyContent: "flex-end", marginBottom: "16px" }}>
        <CreateAllocationDialog />
      </div>

      {isLoading && <Spinner label="Loading allocations..." />}
      {isError && (
        <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
          {(error as Error).message || "Failed to load allocations."}
        </Text>
      )}

      {!isLoading && !isError && data && data.length === 0 && (
        <div className={styles.empty}>
          <Text size={500}>No allocations yet</Text>
          <div style={{ marginTop: "4px" }}>
            <Text size={300}>Allocate people to projects to see utilization.</Text>
          </div>
        </div>
      )}

      {!isLoading && !isError && data && data.length > 0 && (
        <div className={styles.tableWrap}>
          <Table size="small" aria-label="Allocations">
            <TableHeader>
              <TableRow>
                <TableHeaderCell>Person</TableHeaderCell>
                <TableHeaderCell>Project</TableHeaderCell>
                <TableHeaderCell>Role</TableHeaderCell>
                <TableHeaderCell>Allocation</TableHeaderCell>
                <TableHeaderCell>Period</TableHeaderCell>
                <TableHeaderCell />
              </TableRow>
            </TableHeader>
            <TableBody>
              {data.map((a) => (
                <TableRow key={a.id}>
                  <TableCell><Text weight="semibold">{a.userName}</Text></TableCell>
                  <TableCell>{a.projectName}</TableCell>
                  <TableCell>{a.roleOnProject ?? <Text className={styles.muted}>—</Text>}</TableCell>
                  <TableCell><Badge appearance="tint" color="brand">{a.allocationPercent}%</Badge></TableCell>
                  <TableCell>{fmt(a.startDate)} – {fmt(a.endDate)}</TableCell>
                  <TableCell>
                    <Button
                      appearance="subtle"
                      icon={<Delete24Regular />}
                      aria-label="Remove allocation"
                      disabled={remove.isPending}
                      onClick={() => handleDelete(a.id, `${a.userName} on ${a.projectName}`)}
                    />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )}
    </div>
  );
}

export default function ResourcesPage() {
  const styles = useStyles();
  const [tab, setTab] = useState<"utilization" | "allocations">("utilization");

  return (
    <div>
      <div className={styles.header}>
        <Text as="h1" size={700} weight="bold">Resources</Text>
      </div>

      <TabList
        selectedValue={tab}
        onTabSelect={(_, d) => setTab(d.value as "utilization" | "allocations")}
        style={{ marginBottom: "16px" }}
      >
        <Tab value="utilization">Utilization</Tab>
        <Tab value="allocations">Allocations</Tab>
      </TabList>

      {tab === "utilization" ? <UtilizationView /> : <AllocationsView />}
    </div>
  );
}