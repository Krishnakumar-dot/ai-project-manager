import { useState } from "react";
import {
  makeStyles, tokens, Text, Badge, Spinner, ProgressBar, Card, TabList, Tab,
  Table, TableHeader, TableHeaderCell, TableRow, TableBody, TableCell,
} from "@fluentui/react-components";
import { useFinancialSummary } from "./useFinancials";
import { ProjectFinancialDialog } from "./ProjectFinancialDialog";
import { RatesTab } from "./RatesTab";
import type { BudgetStatus } from "./financialsApi";
import { canManageRates } from "../auth/session";
import { formatMoney, formatPercent } from "../../lib/format";

const useStyles = makeStyles({
  summary: { display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))", gap: "16px", marginBottom: "16px" },
  summaryCard: { padding: "16px", display: "flex", flexDirection: "column", gap: "4px" },
  summaryValue: { fontSize: "26px", fontWeight: 700 },
  tableWrap: {
    backgroundColor: tokens.colorNeutralBackground1,
    border: `1px solid ${tokens.colorNeutralStroke2}`,
    borderRadius: "8px", overflowX: "auto", padding: "4px",
  },
  row: { cursor: "pointer", ":hover": { backgroundColor: tokens.colorNeutralBackground1Hover } },
  muted: { color: tokens.colorNeutralForeground3 },
  empty: { textAlign: "center", padding: "50px 0", color: tokens.colorNeutralForeground3 },
});

const statusColor: Record<BudgetStatus, "subtle" | "success" | "warning" | "danger"> = {
  NoBudget: "subtle",
  OnTrack: "success",
  AtRisk: "warning",
  OverBudget: "danger",
};

const statusLabel: Record<BudgetStatus, string> = {
  NoBudget: "No budget",
  OnTrack: "On track",
  AtRisk: "At risk",
  OverBudget: "Over budget",
};

function barColor(percent: number): "success" | "warning" | "error" {
  if (percent > 100) return "error";
  if (percent >= 80) return "warning";
  return "success";
}

function ProjectsTab() {
  const styles = useStyles();
  const { data, isLoading, isError, error } = useFinancialSummary();
  const [openProjectId, setOpenProjectId] = useState<string | null>(null);

  const totalBudget = data?.reduce((s, p) => s + (p.budget ?? 0), 0) ?? 0;
  const totalCost = data?.reduce((s, p) => s + p.actualCost, 0) ?? 0;
  const totalRevenue = data?.reduce((s, p) => s + p.revenue, 0) ?? 0;
  const totalProfit = totalRevenue - totalCost;

  return (
    <div>
      {isLoading && <Spinner label="Loading financials..." />}
      {isError && (
        <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
          {(error as Error).message || "Failed to load financials."}
        </Text>
      )}

      {!isLoading && !isError && data && data.length === 0 && (
        <div className={styles.empty}><Text size={500}>No projects yet</Text></div>
      )}

      {!isLoading && !isError && data && data.length > 0 && (
        <>
          <div className={styles.summary}>
            <Card className={styles.summaryCard}>
              <Text className={styles.muted} size={300}>Total budget</Text>
              <Text className={styles.summaryValue}>{formatMoney(totalBudget)}</Text>
            </Card>
            <Card className={styles.summaryCard}>
              <Text className={styles.muted} size={300}>Actual cost</Text>
              <Text className={styles.summaryValue}>{formatMoney(totalCost)}</Text>
            </Card>
            <Card className={styles.summaryCard}>
              <Text className={styles.muted} size={300}>Revenue</Text>
              <Text className={styles.summaryValue}>{formatMoney(totalRevenue)}</Text>
            </Card>
            <Card className={styles.summaryCard}>
              <Text className={styles.muted} size={300}>Profit</Text>
              <Text className={styles.summaryValue} style={{ color: totalProfit < 0 ? tokens.colorPaletteRedForeground1 : undefined }}>
                {formatMoney(totalProfit)}
              </Text>
            </Card>
          </div>

          <div className={styles.tableWrap}>
            <Table size="small" aria-label="Project financials">
              <TableHeader>
                <TableRow>
                  <TableHeaderCell>Project</TableHeaderCell>
                  <TableHeaderCell>Budget</TableHeaderCell>
                  <TableHeaderCell>Actual cost</TableHeaderCell>
                  <TableHeaderCell style={{ minWidth: "170px" }}>Budget used</TableHeaderCell>
                  <TableHeaderCell>Revenue</TableHeaderCell>
                  <TableHeaderCell>Profit</TableHeaderCell>
                  <TableHeaderCell>Margin</TableHeaderCell>
                  <TableHeaderCell>Status</TableHeaderCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {data.map((p) => (
                  <TableRow key={p.projectId} className={styles.row} onClick={() => setOpenProjectId(p.projectId)}>
                    <TableCell>
                      <div>
                        <Text weight="semibold">{p.projectName}</Text>
                        <div><Text className={styles.muted} size={200}>{p.portfolioName}</Text></div>
                      </div>
                    </TableCell>
                    <TableCell>{formatMoney(p.budget)}</TableCell>
                    <TableCell>{formatMoney(p.actualCost)}</TableCell>
                    <TableCell>
                      {p.budgetUsedPercent === null ? (
                        <Text className={styles.muted}>—</Text>
                      ) : (
                        <div style={{ display: "flex", alignItems: "center", gap: "10px" }}>
                          <div style={{ flex: 1 }}>
                            <ProgressBar
                              value={Math.min(p.budgetUsedPercent, 100) / 100}
                              max={1}
                              color={barColor(p.budgetUsedPercent)}
                              thickness="large"
                            />
                          </div>
                          <Text weight="semibold">{formatPercent(p.budgetUsedPercent)}</Text>
                        </div>
                      )}
                    </TableCell>
                    <TableCell>{formatMoney(p.revenue)}</TableCell>
                    <TableCell>
                      <Text style={{ color: p.profit < 0 ? tokens.colorPaletteRedForeground1 : undefined }}>
                        {formatMoney(p.profit)}
                      </Text>
                    </TableCell>
                    <TableCell>{formatPercent(p.marginPercent)}</TableCell>
                    <TableCell>
                      <Badge appearance="filled" color={statusColor[p.budgetStatus]}>
                        {statusLabel[p.budgetStatus]}
                      </Badge>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
          <Text className={styles.muted} size={200} style={{ display: "block", marginTop: "8px" }}>
            Click a project for the full breakdown. Only approved timesheets are counted.
          </Text>
        </>
      )}

      <ProjectFinancialDialog projectId={openProjectId} onClose={() => setOpenProjectId(null)} />
    </div>
  );
}

export default function FinancialsPage() {
  const [tab, setTab] = useState<"projects" | "rates">("projects");

  return (
    <div>
      <Text as="h1" size={700} weight="bold" style={{ display: "block", marginBottom: "16px" }}>
        Financials
      </Text>

      {canManageRates() && (
        <TabList
          selectedValue={tab}
          onTabSelect={(_, d) => setTab(d.value as "projects" | "rates")}
          style={{ marginBottom: "16px" }}
        >
          <Tab value="projects">Projects</Tab>
          <Tab value="rates">Rates</Tab>
        </TabList>
      )}

      {tab === "projects" || !canManageRates() ? <ProjectsTab /> : <RatesTab />}
    </div>
  );
}