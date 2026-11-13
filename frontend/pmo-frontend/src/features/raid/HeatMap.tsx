import { makeStyles, tokens, Text } from "@fluentui/react-components";
import type { HeatMapCell } from "./raidApi";
import { IMPACT_LABELS, PROBABILITY_LABELS, levelFromScore } from "./raidConstants";
import type { RiskLevel } from "./raidConstants";

const useStyles = makeStyles({
  wrap: {
    backgroundColor: tokens.colorNeutralBackground1,
    border: `1px solid ${tokens.colorNeutralStroke2}`,
    borderRadius: "8px",
    padding: "20px",
    overflowX: "auto",
  },
  grid: {
    display: "grid",
    gridTemplateColumns: "130px repeat(5, minmax(80px, 1fr))",
    gap: "4px",
    maxWidth: "760px",
  },
  axisLabel: { display: "flex", alignItems: "center", justifyContent: "flex-end", paddingRight: "8px", textAlign: "right" },
  axisTop: { textAlign: "center", paddingTop: "4px" },
  cell: {
    height: "64px",
    borderRadius: "6px",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    fontSize: "20px",
    fontWeight: 700,
  },
  muted: { color: tokens.colorNeutralForeground3 },
  legend: { display: "flex", gap: "16px", marginTop: "16px", flexWrap: "wrap" },
  dot: { display: "inline-block", width: "12px", height: "12px", borderRadius: "3px", marginRight: "6px" },
});

const levelBackground: Record<RiskLevel, string> = {
  Low: tokens.colorPaletteGreenBackground2,
  Medium: tokens.colorPaletteYellowBackground2,
  High: tokens.colorPaletteDarkOrangeBackground2,
  Critical: tokens.colorPaletteRedBackground2,
};

interface Props {
  cells: HeatMapCell[];
}

export function HeatMap({ cells }: Props) {
  const styles = useStyles();

  const countAt = (impact: number, probability: number) =>
    cells.find((c) => c.impact === impact && c.probability === probability)?.count ?? 0;

  const probabilities = [5, 4, 3, 2, 1]; // highest probability at the top
  const impacts = [1, 2, 3, 4, 5];

  return (
    <div className={styles.wrap}>
      <Text weight="semibold" style={{ display: "block", marginBottom: "12px" }}>
        Active risks by impact and probability
      </Text>

      <div className={styles.grid}>
        {probabilities.map((p) => (
          <div key={`row-${p}`} style={{ display: "contents" }}>
            <div className={styles.axisLabel}>
              <Text size={200} className={styles.muted}>{PROBABILITY_LABELS[p]}</Text>
            </div>
            {impacts.map((i) => {
              const count = countAt(i, p);
              const level = levelFromScore(i * p);
              return (
                <div
                  key={`${i}-${p}`}
                  className={styles.cell}
                  style={{ backgroundColor: levelBackground[level] }}
                  title={`Impact ${i}, probability ${p}: ${count} risk${count === 1 ? "" : "s"} (${level})`}
                >
                  {count > 0 ? count : ""}
                </div>
              );
            })}
          </div>
        ))}

        <div />
        {impacts.map((i) => (
          <div key={`axis-${i}`} className={styles.axisTop}>
            <Text size={200} className={styles.muted}>{IMPACT_LABELS[i]}</Text>
          </div>
        ))}
      </div>

      <div className={styles.legend}>
        {(["Low", "Medium", "High", "Critical"] as RiskLevel[]).map((l) => (
          <Text key={l} size={200}>
            <span className={styles.dot} style={{ backgroundColor: levelBackground[l] }} />{l}
          </Text>
        ))}
      </div>
      <Text size={200} className={styles.muted} style={{ display: "block", marginTop: "8px" }}>
        Probability runs up the side, impact runs across the bottom. Only active risks are plotted.
      </Text>
    </div>
  );
}