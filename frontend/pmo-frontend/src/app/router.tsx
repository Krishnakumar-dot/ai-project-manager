import { createBrowserRouter, Navigate } from "react-router-dom";
import { lazy, Suspense } from "react";
import { Spinner } from "@fluentui/react-components";
import { AppShell } from "../layouts/AppShell";
import { LoginPage } from "../features/auth/LoginPage";
import { isApprover } from "../features/auth/session";

const DashboardPage = lazy(() => import("../features/dashboard/DashboardPage"));
const ProjectsPage = lazy(() => import("../features/projects/ProjectsPage"));
const TasksPage = lazy(() => import("../features/tasks/TasksPage"));
const TimesheetPage = lazy(() => import("../features/timesheets/TimesheetPage"));
const ApprovalsPage = lazy(() => import("../features/timesheets/ApprovalsPage"));

function RequireAuth({ children }: { children: React.ReactNode }) {
  const token = localStorage.getItem("token");
  return token ? <>{children}</> : <Navigate to="/login" replace />;
}

function PageFallback() {
  return (
    <div style={{ display: "flex", justifyContent: "center", paddingTop: "80px" }}>
      <Spinner label="Loading..." />
    </div>
  );
}

const withSuspense = (Component: React.LazyExoticComponent<() => React.ReactElement>) => (
  <Suspense fallback={<PageFallback />}>
    <Component />
  </Suspense>
);

export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  {
    path: "/",
    element: <RequireAuth><AppShell /></RequireAuth>,
    children: [
      { index: true, element: withSuspense(DashboardPage) },
      { path: "projects", element: withSuspense(ProjectsPage) },
      { path: "tasks", element: withSuspense(TasksPage) },
      { path: "timesheets", element: withSuspense(TimesheetPage) },
      { path: "approvals", element: isApprover() ? withSuspense(ApprovalsPage) : <Navigate to="/" replace /> },
    ],
  },
]);