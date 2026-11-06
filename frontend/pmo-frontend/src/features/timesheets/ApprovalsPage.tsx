import { useState } from "react";
import {
  makeStyles, tokens, Text, Card, Button, Badge, Spinner, Textarea,
  MessageBar, MessageBarBody,
} from "@fluentui/react-components";
import { Checkmark24Regular, Dismiss24Regular } from "@fluentui/react-icons";
import { usePendingTimesheets, useReviewTimesheet } from "./useTimesheets";
import type { PendingTimesheet } from "./timesheetsApi";
import { parseIsoDate } from "../../lib/dateUtils";

const useStyles = makeStyles({
  list: { display: "flex", flexDirection: "column", gap: "16px" },
  card: { padding: "18px", display: "flex", flexDirection: "column", gap: "12px" },
  top: { display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: "8px" },
  badges: { display: "flex", gap: "8px" },
  entries: { display: "flex", flexDirection: "column", gap: "4px" },
  entryLine: { display: "flex", justifyContent: "space-between", fontSize: "13px" },
  actions: { display: "flex", gap: "8px", justifyContent: "flex-end" },
  muted: { color: tokens.colorNeutralForeground3 },
  empty: { textAlign: "center", padding: "60px 0", color: tokens.colorNeutralForeground3 },
});

function ApprovalCard({ sheet }: { sheet: PendingTimesheet }) {
  const styles = useStyles();
  const review = useReviewTimesheet();
  const [rejecting, setRejecting] = useState(false);
  const [comment, setComment] = useState("");
  const [error, setError] = useState("");

  const send = async (approve: boolean) => {
    setError("");
    try {
      const result = await review.mutateAsync({ id: sheet.id, approve, comment: comment.trim() || undefined });
      if (!result.isSuccessful) setError(result.message);
    } catch {
      setError("Something went wrong. Please try again.");
    }
  };

  const weekLabel = parseIsoDate(sheet.weekStartDate).toLocaleDateString(undefined, {
    day: "numeric", month: "short", year: "numeric",
  });

  return (
    <Card className={styles.card}>
      <div className={styles.top}>
        <div>
          <Text weight="semibold" size={400}>{sheet.userName}</Text>
          <div><Text className={styles.muted} size={200}>Week of {weekLabel}</Text></div>
        </div>
        <div className={styles.badges}>
          <Badge appearance="filled" color="informative">{sheet.totalHours}h total</Badge>
          <Badge appearance="tint" color="success">{sheet.billableHours}h billable</Badge>
        </div>
      </div>

      <div className={styles.entries}>
        {sheet.entries.map((e, i) => (
          <div key={i} className={styles.entryLine}>
            <span>
              {parseIsoDate(e.date).toLocaleDateString(undefined, { weekday: "short", day: "numeric", month: "short" })}
              {" · "}{e.projectName}{e.taskTitle ? ` / ${e.taskTitle}` : ""}
              {!e.isBillable && " (non-billable)"}
            </span>
            <span>{e.hours}h</span>
          </div>
        ))}
      </div>

      {error && (
        <MessageBar intent="error"><MessageBarBody>{error}</MessageBarBody></MessageBar>
      )}

      {rejecting && (
        <Textarea
          placeholder="Reason for rejection (required)"
          value={comment}
          onChange={(_, d) => setComment(d.value)}
        />
      )}

      <div className={styles.actions}>
        {!rejecting ? (
          <>
            <Button appearance="secondary" icon={<Dismiss24Regular />} onClick={() => setRejecting(true)} disabled={review.isPending}>
              Reject
            </Button>
            <Button appearance="primary" icon={<Checkmark24Regular />} onClick={() => send(true)} disabled={review.isPending}>
              {review.isPending ? "Working..." : "Approve"}
            </Button>
          </>
        ) : (
          <>
            <Button appearance="subtle" onClick={() => { setRejecting(false); setComment(""); setError(""); }}>Cancel</Button>
            <Button appearance="primary" onClick={() => send(false)} disabled={review.isPending}>
              Confirm rejection
            </Button>
          </>
        )}
      </div>
    </Card>
  );
}

export default function ApprovalsPage() {
  const styles = useStyles();
  const { data: sheets, isLoading, isError, error } = usePendingTimesheets();

  return (
    <div>
      <Text as="h1" size={700} weight="bold" style={{ display: "block", marginBottom: "20px" }}>
        Timesheet Approvals
      </Text>

      {isLoading && <Spinner label="Loading..." />}

      {isError && (
        <Text style={{ color: tokens.colorPaletteRedForeground1 }}>
          {(error as Error).message || "Failed to load pending timesheets."}
        </Text>
      )}

      {!isLoading && !isError && sheets && sheets.length === 0 && (
        <div className={styles.empty}>
          <Text size={500}>Nothing waiting for approval</Text>
        </div>
      )}

      {!isLoading && !isError && sheets && sheets.length > 0 && (
        <div className={styles.list}>
          {sheets.map((s) => <ApprovalCard key={s.id} sheet={s} />)}
        </div>
      )}
    </div>
  );
}