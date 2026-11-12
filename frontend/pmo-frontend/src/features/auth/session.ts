const APPROVER_ROLES = ["Admin", "PM", "PMO"];
const FINANCE_VIEW_ROLES = ["Admin", "PMO", "PM", "Finance"];
const PROJECT_FINANCE_EDIT_ROLES = ["Admin", "PMO", "Finance"];
const RATE_ROLES = ["Admin", "Finance"];

export const getRole = (): string => localStorage.getItem("role") ?? "";
export const getName = (): string => localStorage.getItem("name") ?? "";
export const isApprover = (): boolean => APPROVER_ROLES.includes(getRole());

export const canViewFinancials = (): boolean => FINANCE_VIEW_ROLES.includes(getRole());
export const canEditProjectFinancials = (): boolean => PROJECT_FINANCE_EDIT_ROLES.includes(getRole());
export const canManageRates = (): boolean => RATE_ROLES.includes(getRole());

export const clearSession = (): void => {
  localStorage.removeItem("token");
  localStorage.removeItem("role");
  localStorage.removeItem("name");
};