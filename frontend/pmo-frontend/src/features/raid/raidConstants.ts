export type RaidType = "Risk" | "Assumption" | "Issue" | "Dependency";
export type RaidStatus = "Open" | "InProgress" | "Resolved" | "Closed";
export type RiskLevel = "Low" | "Medium" | "High" | "Critical";

export const RAID_TYPES: RaidType[] = ["Risk", "Assumption", "Issue", "Dependency"];
export const RAID_STATUSES: RaidStatus[] = ["Open", "InProgress", "Resolved", "Closed"];

export const STATUS_LABEL: Record<RaidStatus, string> = {
  Open: "Open",
  InProgress: "In progress",
  Resolved: "Resolved",
  Closed: "Closed",
};

export const IMPACT_LABELS: Record<number, string> = {
  1: "1 - Very low",
  2: "2 - Low",
  3: "3 - Medium",
  4: "4 - High",
  5: "5 - Very high",
};

export const PROBABILITY_LABELS: Record<number, string> = {
  1: "1 - Rare",
  2: "2 - Unlikely",
  3: "3 - Possible",
  4: "4 - Likely",
  5: "5 - Almost certain",
};

// Must match RaidRules.Level on the backend. Used only for the live preview in the dialog.
export const levelFromScore = (score: number): RiskLevel =>
  score >= 20 ? "Critical" : score >= 10 ? "High" : score >= 5 ? "Medium" : "Low";

export const LEVEL_BADGE: Record<RiskLevel, "success" | "warning" | "severe" | "danger"> = {
  Low: "success",
  Medium: "warning",
  High: "severe",
  Critical: "danger",
};

export const STATUS_BADGE: Record<RaidStatus, "informative" | "warning" | "success" | "subtle"> = {
  Open: "informative",
  InProgress: "warning",
  Resolved: "success",
  Closed: "subtle",
};

export const TYPE_BADGE: Record<RaidType, "danger" | "informative" | "severe" | "brand"> = {
  Risk: "danger",
  Assumption: "informative",
  Issue: "severe",
  Dependency: "brand",
};