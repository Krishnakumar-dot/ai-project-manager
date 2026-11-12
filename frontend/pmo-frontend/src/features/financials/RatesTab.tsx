import { useState } from "react";
import {
  makeStyles, tokens, Text, Button, Input, Spinner, Badge, MessageBar, MessageBarBody,
  Table, TableHeader, TableHeaderCell, TableRow, TableBody, TableCell,
} from "@fluentui/react-components";
import { useUserRates, useUpdateUserRate } from "./useFinancials";
import type { UserRate } from "./financialsApi";

const useStyles = makeStyles({
  tableWrap: {
    backgroundColor: tokens.colorNeutralBackground1,
    border: `1px solid ${tokens.colorNeutralStroke2}`,
    borderRadius: "8px", overflowX: "auto", padding: "4px",
  },
  muted: { color: tokens.colorNeutralForeground3 },
});

function RateRow({ user }: { user: UserRate }) {
  const update = useUpdateUserRate();
  const [value, setValue] = useState(user.hourlyCostRate?.toString() ?? "");
  const [message, setMessage] = useState<{ intent: "success" | "error"; text: string } | null>(null);

  const original = user.hourlyCostRate?.toString() ?? "";
  const changed = value.trim() !== original;

  const handleSave = async () => {
    try {
      const result = await update.mutateAsync({
        userId: user.id,
        hourlyCostRate: value.trim() === "" ? null : Number(value),
      });
      setMessage(result.isSuccessful ? { intent: "success", text: "Saved" } : { intent: "error", text: result.message });
    } catch {
      setMessage({ intent: "error", text: "Could not save" });
    }
  };

  return (
    <TableRow>
      <TableCell><Text weight="semibold">{user.name}</Text></TableCell>
      <TableCell>{user.email}</TableCell>
      <TableCell><Badge appearance="tint" color="informative">{user.role}</Badge></TableCell>
      <TableCell>
        <Input
          type="number" size="small" min={0} style={{ width: "120px" }}
          placeholder="Not set"
          value={value}
          onChange={(_, d) => { setValue(d.value); setMessage(null); }}
        />
      </TableCell>
      <TableCell>
        <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
          <Button size="small" appearance="primary" disabled={!changed || update.isPending} onClick={handleSave}>
            Save
          </Button>
          {message && (
            <Text size={200} style={{ color: message.intent === "error" ? tokens.colorPaletteRedForeground1 : tokens.colorPaletteGreenForeground1 }}>
              {message.text}
            </Text>
          )}
        </div>
      </TableCell>
    </TableRow>
  );
}

export function RatesTab() {
  const styles = useStyles();
  const { data, isLoading, isError, error } = useUserRates();

  return (
    <div>
      <MessageBar intent="info" style={{ marginBottom: "16px" }}>
        <MessageBarBody>
          The hourly cost rate is what one hour of this person costs the company. A change applies to all of their approved hours, including past weeks.
        </MessageBarBody>
      </MessageBar>

      {isLoading && <Spinner label="Loading rates..." />}
      {isError && (
        <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
          {(error as Error).message || "Failed to load rates."}
        </Text>
      )}

      {!isLoading && !isError && data && (
        <div className={styles.tableWrap}>
          <Table size="small" aria-label="Hourly cost rates">
            <TableHeader>
              <TableRow>
                <TableHeaderCell>Person</TableHeaderCell>
                <TableHeaderCell>Email</TableHeaderCell>
                <TableHeaderCell>Role</TableHeaderCell>
                <TableHeaderCell>Hourly cost rate</TableHeaderCell>
                <TableHeaderCell />
              </TableRow>
            </TableHeader>
            <TableBody>
              {data.map((u) => <RateRow key={u.id} user={u} />)}
            </TableBody>
          </Table>
        </div>
      )}
    </div>
  );
}