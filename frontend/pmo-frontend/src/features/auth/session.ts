const APPROVER_ROLES = ["Admin", "PM", "PMO"];

export const getRole = (): string => localStorage.getItem("role") ?? "";
export const getName = (): string => localStorage.getItem("name") ?? "";
export const isApprover = (): boolean => APPROVER_ROLES.includes(getRole());

export const clearSession = (): void => {
  localStorage.removeItem("token");
  localStorage.removeItem("role");
  localStorage.removeItem("name");
};