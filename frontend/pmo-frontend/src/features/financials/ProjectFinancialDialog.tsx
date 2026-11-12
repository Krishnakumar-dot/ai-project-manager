import { useEffect, useState } from "react";
import {
  makeStyles, tokens, Dialog, DialogSurface, DialogBody, DialogTitle, DialogContent, DialogActions,
  Button, Text, Badge, Spinner, Input, Field, MessageBar, MessageBarBody,
  Table, TableHeader, TableHeaderCell, TableRow, TableBody, TableCell,
} from "@fluentui/react-components";
import { useProjectFinancials, useUpdateProjectFinancials } from "./useFinancials";
import { canEditProjectFinancials, canManageRates } from "../auth/session";
import { formatMoney, formatPercent } from "../../lib/format";
import { parseIsoDate } from "../../lib/dateUtils";

const useStyles = makeStyles({
  kpis: { display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(150px, 1fr))", gap: "12px", marginBottom: "16px" },
  kpi: { padding: "12px", borderRadius: "8px", backgroundColor: tokens.colorNeutralBackground3, display: "flex", flexDirection: "column", gap: "2px" },
  kpiValue: { fontSize: "20px", fontWeight: 700 },
  muted: { color: tokens.colorNeutralForeground3 },
  section: { marginTop: "20px", marginBottom: "8px" },
  editRow: { display: "flex", gap: "12px", alignItems: "flex-end", flexWrap: "wrap" },
  weekRow: { display: "grid", gridTemplateColumns: "110px 1fr 190px", alignItems: "center", gap: "10px", marginBottom: "6px" },
  bars: { display: "flex", flexDirection: "column", gap: "3px" },
  barCost: { height: "8px", borderRadius: "4px", backgroundColor: tokens.colorBrandBackground },
  barRevenue: { height: "8px", borderRadius: "4px", backgroundColor: tokens.colorPaletteGreenBackground3 },
  legend: { display: "flex", gap: "16px", marginBottom: "8px" },
  dot: { display: "inline-block", width: "10px", height: "10px", borderRadius: "50%", marginRight: "6px" },
});

interface Props {
  projectId: string | null;
  onClose: () => void;
}

export function ProjectFinancialDialog({ projectId, onClose }: Props) {
  const styles = useStyles();
  const { data: detail, isLoading, isError, error } = useProjectFinancials(projectId);
  const update = useUpdateProjectFinancials();
  const canEdit = canEditProjectFinancials();

  const [budget, setBudget] = useState("");
  const [rate, setRate] = useState("");
  const [feedback, setFeedback] = useState<{ intent: "success" | "error"; text: string } | null>(null);

  // Load the current values into the edit fields whenever the project data arrives.
  useEffect(() => {
    if (detail) {
      setBudget(detail.summary.budget?.toString() ?? "");
      setRate(detail.summary.billingRatePerHour?.toString() ?? "");
      setFeedback(null);
    }
  }, [detail]);

  const handleSave = async () => {
    if (!projectId) return;
    try {
      const result = await update.mutateAsync({
        projectId,
        budget: budget.trim() === "" ? null : Number(budget),
        billingRatePerHour: rate.trim() === "" ? null : Number(rate),
      });
      setFeedback(
        result.isSuccessful
          ? { intent: "success", text: "Saved." }
          : { intent: "error", text: result.message }
      );
    } catch {
      setFeedback({ intent: "error", text: "Could not save. Please try again." });
    }
  };

  const s = detail?.summary;
  const maxWeek = detail ? Math.max(1, ...detail.byWeek.map((w) => Math.max(w.cost, w.revenue))) : 1;
  const forecastOver = s && s.budget !== null && s.forecastCost !== null ? s.forecastCost - s.budget : null;

  return (
    <Dialog open={!!projectId} onOpenChange={(_, d) => { if (!d.open) onClose(); }}>
      <DialogSurface style={{ maxWidth: "900px", width: "92vw" }}>
        <DialogBody>
          <DialogTitle>{s ? `${s.projectName} — Financials` : "Financials"}</DialogTitle>
          <DialogContent style={{ maxHeight: "70vh", overflowY: "auto" }}>
            {isLoading && <Spinner label="Loading..." />}
            {isError && (
              <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
                {(error as Error).message || "Failed to load financials."}
              </Text>
            )}

            {s && detail && (
              <>
                {s.hoursMissingRate > 0 && (
                  <MessageBar intent="warning" style={{ marginBottom: "12px" }}>
                    <MessageBarBody>
                      {s.hoursMissingRate} approved hours belong to people with no cost rate, so their cost is counted as 0.
                      {canManageRates() ? " Set their rates in the Rates tab." : " Ask Finance to set their rates."}
                    </MessageBarBody>
                  </MessageBar>
                )}

                <div className={styles.kpis}>
                  <div className={styles.kpi}><Text className={styles.muted} size={200}>Budget</Text><Text className={styles.kpiValue}>{formatMoney(s.budget)}</Text></div>
                  <div className={styles.kpi}><Text className={styles.muted} size={200}>Actual cost</Text><Text className={styles.kpiValue}>{formatMoney(s.actualCost)}</Text></div>
                  <div className={styles.kpi}><Text className={styles.muted} size={200}>Budget used</Text><Text className={styles.kpiValue}>{formatPercent(s.budgetUsedPercent)}</Text></div>
                  <div className={styles.kpi}><Text className={styles.muted} size={200}>Revenue</Text><Text className={styles.kpiValue}>{formatMoney(s.revenue)}</Text></div>
                  <div className={styles.kpi}>
                    <Text className={styles.muted} size={200}>Profit</Text>
                    <Text className={styles.kpiValue} style={{ color: s.profit < 0 ? tokens.colorPaletteRedForeground1 : undefined }}>{formatMoney(s.profit)}</Text>
                  </div>
                  <div className={styles.kpi}><Text className={styles.muted} size={200}>Margin</Text><Text className={styles.kpiValue}>{formatPercent(s.marginPercent)}</Text></div>
                  <div className={styles.kpi}><Text className={styles.muted} size={200}>Approved hours</Text><Text className={styles.kpiValue}>{s.approvedHours}</Text></div>
                  <div className={styles.kpi}>
                    <Text className={styles.muted} size={200}>Forecast at completion</Text>
                    <Text className={styles.kpiValue}>{s.forecastCost === null ? "Not enough data" : formatMoney(s.forecastCost)}</Text>
                  </div>
                </div>

                {forecastOver !== null && (
                  <MessageBar intent={forecastOver > 0 ? "error" : "success"} style={{ marginBottom: "12px" }}>
                    <MessageBarBody>
                      {forecastOver > 0
                        ? `At the current pace the project will finish ${formatMoney(forecastOver)} over budget.`
                        : `At the current pace the project will finish ${formatMoney(Math.abs(forecastOver))} under budget.`}
                    </MessageBarBody>
                  </MessageBar>
                )}

                {canEdit && (
                  <>
                    <Text weight="semibold" className={styles.section} style={{ display: "block" }}>Budget and billing rate</Text>
                    <div className={styles.editRow}>
                      <Field label="Budget">
                        <Input type="number" min={0} value={budget} onChange={(_, d) => setBudget(d.value)} />
                      </Field>
                      <Field label="Billing rate per hour">
                        <Input type="number" min={0} value={rate} onChange={(_, d) => setRate(d.value)} />
                      </Field>
                      <Button appearance="primary" onClick={handleSave} disabled={update.isPending}>
                        {update.isPending ? "Saving..." : "Save"}
                      </Button>
                    </div>
                    {feedback && (
                      <MessageBar intent={feedback.intent} style={{ marginTop: "8px" }}>
                        <MessageBarBody>{feedback.text}</MessageBarBody>
                      </MessageBar>
                    )}
                  </>
                )}

                <Text weight="semibold" className={styles.section} style={{ display: "block" }}>Cost by person</Text>
                {detail.byPerson.length === 0 ? (
                  <Text className={styles.muted}>No approved hours yet.</Text>
                ) : (
                  <Table size="small" aria-label="Cost by person">
                    <TableHeader>
                      <TableRow>
                        <TableHeaderCell>Person</TableHeaderCell>
                        <TableHeaderCell>Hours</TableHeaderCell>
                        <TableHeaderCell>Billable</TableHeaderCell>
                        {canManageRates() && <TableHeaderCell>Rate / hr</TableHeaderCell>}
                        <TableHeaderCell>Cost</TableHeaderCell>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {detail.byPerson.map((p) => (
                        <TableRow key={p.userId}>
                          <TableCell>{p.userName}</TableCell>
                          <TableCell>{p.hours}</TableCell>
                          <TableCell>{p.billableHours}</TableCell>
                          {canManageRates() && (
                            <TableCell>
                              {p.hourlyCostRate === null
                                ? <Badge appearance="tint" color="warning">Not set</Badge>
                                : formatMoney(p.hourlyCostRate)}
                            </TableCell>
                          )}
                          <TableCell>{formatMoney(p.cost)}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                )}

                <Text weight="semibold" className={styles.section} style={{ display: "block" }}>Cost and revenue by week</Text>
                {detail.byWeek.length === 0 ? (
                  <Text className={styles.muted}>No approved hours yet.</Text>
                ) : (
                  <>
                    <div className={styles.legend}>
                      <Text size={200}><span className={styles.dot} style={{ backgroundColor: tokens.colorBrandBackground }} />Cost</Text>
                      <Text size={200}><span className={styles.dot} style={{ backgroundColor: tokens.colorPaletteGreenBackground3 }} />Revenue</Text>
                    </div>
                    {detail.byWeek.map((w) => (
                      <div key={w.weekStart} className={styles.weekRow}>
                        <Text size={200}>
                          {parseIsoDate(w.weekStart).toLocaleDateString(undefined, { day: "numeric", month: "short" })}
                        </Text>
                        <div className={styles.bars}>
                          <div className={styles.barCost} style={{ width: `${(w.cost / maxWeek) * 100}%` }} />
                          <div className={styles.barRevenue} style={{ width: `${(w.revenue / maxWeek) * 100}%` }} />
                        </div>
                        <Text size={200}>{formatMoney(w.cost)} / {formatMoney(w.revenue)}</Text>
                      </div>
                    ))}
                  </>
                )}
              </>
            )}
          </DialogContent>
          <DialogActions>
            <Button appearance="secondary" onClick={onClose}>Close</Button>
          </DialogActions>
        </DialogBody>
      </DialogSurface>
    </Dialog>
  );
}