import { useState } from "react";
import {
  makeStyles, tokens, Text, Button, Badge, Spinner, Card, TabList, Tab, Dropdown, Option, Switch,
  MessageBar, MessageBarBody,
  Table, TableHeader, TableHeaderCell, TableRow, TableBody, TableCell,
} from "@fluentui/react-components";
import {
  AddCircle24Regular, Edit24Regular, Delete24Regular, Flag24Regular, Flag24Filled,
} from "@fluentui/react-icons";
import { useProjectsList } from "../projects/useProjects";
import { useRaidItems, useRaidSummary, useSetRaidEscalation, useDeleteRaidItem } from "./useRaid";
import { RaidItemDialog } from "./RaidItemDialog";
import { HeatMap } from "./HeatMap";
import type { RaidItem } from "./raidApi";
import { RAID_TYPES, STATUS_LABEL, STATUS_BADGE, TYPE_BADGE, LEVEL_BADGE } from "./raidConstants";
import type { RaidType } from "./raidConstants";
import { isApprover } from "../auth/session";
import { parseIsoDate } from "../../lib/dateUtils";

const useStyles = makeStyles({
  header: { display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "16px", flexWrap: "wrap", gap: "12px" },
  filters: { display: "flex", alignItems: "center", gap: "12px", marginBottom: "16px", flexWrap: "wrap" },
  summary: { display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(170px, 1fr))", gap: "16px", marginBottom: "16px" },
  summaryCard: { padding: "16px", display: "flex", flexDirection: "column", gap: "4px" },
  summaryValue: { fontSize: "28px", fontWeight: 700 },
  tableWrap: {
    backgroundColor: tokens.colorNeutralBackground1,
    border: `1px solid ${tokens.colorNeutralStroke2}`,
    borderRadius: "8px", overflowX: "auto", padding: "4px",
  },
  muted: { color: tokens.colorNeutralForeground3 },
  empty: { textAlign: "center", padding: "50px 0", color: tokens.colorNeutralForeground3 },
  actions: { display: "flex", gap: "2px" },
});

const ALL = "all";

export default function RaidPage() {
  const styles = useStyles();
  const [projectId, setProjectId] = useState("");
  const [type, setType] = useState<RaidType | typeof ALL>(ALL);
  const [activeOnly, setActiveOnly] = useState(true);
  const [tab, setTab] = useState<"log" | "heatmap">("log");
  const [dialog, setDialog] = useState<{ open: boolean; item?: RaidItem }>({ open: false });
  const [feedback, setFeedback] = useState<string | null>(null);

  const { data: projects } = useProjectsList();
  const items = useRaidItems({
    projectId: projectId || undefined,
    type: type === ALL ? undefined : type,
    activeOnly,
  });
  const summary = useRaidSummary(projectId || undefined);
  const escalate = useSetRaidEscalation();
  const remove = useDeleteRaidItem();
  const canDelete = isApprover();

  const handleEscalate = async (item: RaidItem) => {
    setFeedback(null);
    try {
      const result = await escalate.mutateAsync({ id: item.id, escalate: !item.isEscalated });
      if (!result.isSuccessful) setFeedback(result.message);
    } catch {
      setFeedback("Could not update the escalation. Please try again.");
    }
  };

  const handleDelete = async (item: RaidItem) => {
    if (!window.confirm(`Delete "${item.title}"? This cannot be undone.`)) return;
    setFeedback(null);
    try {
      const result = await remove.mutateAsync(item.id);
      if (!result.isSuccessful) setFeedback(result.message);
    } catch {
      setFeedback("Could not delete the item. Please try again.");
    }
  };

  const fmt = (iso: string) =>
    parseIsoDate(iso).toLocaleDateString(undefined, { day: "numeric", month: "short", year: "numeric" });

  const s = summary.data;

  return (
    <div>
      <div className={styles.header}>
        <Text as="h1" size={700} weight="bold">Risks & Issues</Text>
        <Button appearance="primary" icon={<AddCircle24Regular />} onClick={() => setDialog({ open: true })}>
          New Item
        </Button>
      </div>

      <div className={styles.filters}>
        <Dropdown
          style={{ minWidth: "220px" }}
          value={projects?.find((p) => p.id === projectId)?.name ?? "All projects"}
          selectedOptions={[projectId || ALL]}
          onOptionSelect={(_, d) => setProjectId(d.optionValue === ALL ? "" : d.optionValue ?? "")}
        >
          <Option value={ALL}>All projects</Option>
          {projects?.map((p) => <Option key={p.id} value={p.id}>{p.name}</Option>)}
        </Dropdown>

        <Dropdown
          style={{ minWidth: "160px" }}
          value={type === ALL ? "All types" : type}
          selectedOptions={[type]}
          onOptionSelect={(_, d) => setType((d.optionValue as RaidType | typeof ALL) ?? ALL)}
        >
          <Option value={ALL}>All types</Option>
          {RAID_TYPES.map((t) => <Option key={t} value={t}>{t}</Option>)}
        </Dropdown>

        <Switch checked={activeOnly} onChange={(_, d) => setActiveOnly(d.checked)} label="Active only" />
      </div>

      {s && (
        <div className={styles.summary}>
          <Card className={styles.summaryCard}>
            <Text className={styles.muted} size={300}>Active items</Text>
            <Text className={styles.summaryValue}>{s.activeTotal}</Text>
            <Text className={styles.muted} size={200}>
              {s.activeRisks} risks · {s.activeIssues} issues
            </Text>
          </Card>
          <Card className={styles.summaryCard}>
            <Text className={styles.muted} size={300}>Critical / High</Text>
            <Text className={styles.summaryValue} style={{ color: s.criticalCount > 0 ? tokens.colorPaletteRedForeground1 : undefined }}>
              {s.criticalCount} / {s.highCount}
            </Text>
            <Text className={styles.muted} size={200}>risks and issues</Text>
          </Card>
          <Card className={styles.summaryCard}>
            <Text className={styles.muted} size={300}>Overdue</Text>
            <Text className={styles.summaryValue} style={{ color: s.overdueCount > 0 ? tokens.colorPaletteRedForeground1 : undefined }}>
              {s.overdueCount}
            </Text>
          </Card>
          <Card className={styles.summaryCard}>
            <Text className={styles.muted} size={300}>Escalated</Text>
            <Text className={styles.summaryValue}>{s.escalatedCount}</Text>
          </Card>
        </div>
      )}

      {feedback && (
        <MessageBar intent="error" style={{ marginBottom: "12px" }}>
          <MessageBarBody>{feedback}</MessageBarBody>
        </MessageBar>
      )}

      <TabList
        selectedValue={tab}
        onTabSelect={(_, d) => setTab(d.value as "log" | "heatmap")}
        style={{ marginBottom: "16px" }}
      >
        <Tab value="log">Log</Tab>
        <Tab value="heatmap">Heat map</Tab>
      </TabList>

      {tab === "heatmap" && (s ? <HeatMap cells={s.heatMap} /> : <Spinner label="Loading..." />)}

      {tab === "log" && (
        <>
          {items.isLoading && <Spinner label="Loading items..." />}
          {items.isError && (
            <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
              {(items.error as Error).message || "Failed to load items."}
            </Text>
          )}

          {!items.isLoading && !items.isError && items.data && items.data.length === 0 && (
            <div className={styles.empty}>
              <Text size={500}>Nothing logged here yet</Text>
              <div style={{ marginTop: "4px" }}>
                <Text size={300}>Log a risk, issue, assumption or dependency to get started.</Text>
              </div>
            </div>
          )}

          {!items.isLoading && !items.isError && items.data && items.data.length > 0 && (
            <div className={styles.tableWrap}>
              <Table size="small" aria-label="RAID log">
                <TableHeader>
                  <TableRow>
                    <TableHeaderCell style={{ minWidth: "260px" }}>Item</TableHeaderCell>
                    <TableHeaderCell>Type</TableHeaderCell>
                    <TableHeaderCell>Project</TableHeaderCell>
                    <TableHeaderCell>Score</TableHeaderCell>
                    <TableHeaderCell>Status</TableHeaderCell>
                    <TableHeaderCell>Owner</TableHeaderCell>
                    <TableHeaderCell>Due</TableHeaderCell>
                    <TableHeaderCell />
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {items.data.map((item) => (
                    <TableRow key={item.id}>
                      <TableCell>
                        <div>
                          <div style={{ display: "flex", alignItems: "center", gap: "8px", flexWrap: "wrap" }}>
                            <Text weight="semibold">{item.title}</Text>
                            {item.isEscalated && <Badge appearance="tint" color="danger" size="small">Escalated</Badge>}
                          </div>
                          {item.mitigationPlan && (
                            <Text className={styles.muted} size={200}>Plan: {item.mitigationPlan}</Text>
                          )}
                        </div>
                      </TableCell>
                      <TableCell><Badge appearance="tint" color={TYPE_BADGE[item.type]}>{item.type}</Badge></TableCell>
                      <TableCell>{item.projectName}</TableCell>
                      <TableCell>
                        <Badge appearance="filled" color={LEVEL_BADGE[item.level]} title={`Impact ${item.impact} × probability ${item.probability}`}>
                          {item.score} · {item.level}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <Badge appearance="tint" color={STATUS_BADGE[item.status]}>{STATUS_LABEL[item.status]}</Badge>
                      </TableCell>
                      <TableCell>{item.ownerName ?? <Text className={styles.muted}>—</Text>}</TableCell>
                      <TableCell>
                        {item.dueDate ? (
                          <Text style={{ color: item.isOverdue ? tokens.colorPaletteRedForeground1 : undefined, fontWeight: item.isOverdue ? 700 : undefined }}>
                            {fmt(item.dueDate)}{item.isOverdue && " (overdue)"}
                          </Text>
                        ) : <Text className={styles.muted}>—</Text>}
                      </TableCell>
                      <TableCell>
                        <div className={styles.actions}>
                          <Button
                            appearance="subtle"
                            icon={item.isEscalated ? <Flag24Filled /> : <Flag24Regular />}
                            aria-label={item.isEscalated ? "Remove escalation" : "Escalate"}
                            title={item.isEscalated ? "Remove escalation" : "Escalate"}
                            disabled={escalate.isPending}
                            onClick={() => handleEscalate(item)}
                          />
                          <Button
                            appearance="subtle"
                            icon={<Edit24Regular />}
                            aria-label="Edit"
                            onClick={() => setDialog({ open: true, item })}
                          />
                          {canDelete && (
                            <Button
                              appearance="subtle"
                              icon={<Delete24Regular />}
                              aria-label="Delete"
                              disabled={remove.isPending}
                              onClick={() => handleDelete(item)}
                            />
                          )}
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </>
      )}

      {dialog.open && (
        <RaidItemDialog
          key={dialog.item?.id ?? "new"}
          item={dialog.item}
          defaultProjectId={projectId || undefined}
          onClose={() => setDialog({ open: false })}
        />
      )}
    </div>
  );
}