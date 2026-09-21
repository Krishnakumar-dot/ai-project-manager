import { makeStyles, tokens, Card, Text } from "@fluentui/react-components";
import {
  FolderOpen24Regular, TaskListSquareLtr24Regular,
  Warning24Regular, CheckmarkCircle24Regular,
} from "@fluentui/react-icons";

const useStyles = makeStyles({
  grid: { display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(220px, 1fr))", gap: "16px", marginBottom: "24px" },
  statCard: { padding: "20px", display: "flex", flexDirection: "column", gap: "8px" },
  statHeader: { display: "flex", alignItems: "center", gap: "8px", color: tokens.colorNeutralForeground3 },
  statValue: { fontSize: "32px", fontWeight: 700 },
});

const stats = [
  { label: "Active Projects", value: "8", icon: FolderOpen24Regular, color: tokens.colorBrandForeground1 },
  { label: "Open Tasks", value: "34", icon: TaskListSquareLtr24Regular, color: tokens.colorPaletteBlueForeground2 },
  { label: "At Risk", value: "2", icon: Warning24Regular, color: tokens.colorPaletteRedForeground1 },
  { label: "Completed This Month", value: "12", icon: CheckmarkCircle24Regular, color: tokens.colorPaletteGreenForeground1 },
];

export default function DashboardPage() {
  const styles = useStyles();
  return (
    <div>
      <Text as="h1" size={700} weight="bold">Dashboard</Text>
      <div className={styles.grid} style={{ marginTop: "20px" }}>
        {stats.map((s) => {
          const Icon = s.icon as any;
          return (
            <Card key={s.label} className={styles.statCard}>
              <div className={styles.statHeader}>
                <Icon style={{ color: s.color }} />
                <Text size={300}>{s.label}</Text>
              </div>
              <Text className={styles.statValue}>{s.value}</Text>
            </Card>
          );
        })}
      </div>
    </div>
  );
}