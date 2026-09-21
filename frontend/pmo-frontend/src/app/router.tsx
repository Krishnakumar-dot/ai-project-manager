import { createBrowserRouter, Navigate } from "react-router-dom";
import { lazy, Suspense } from "react";
import { Spinner } from "@fluentui/react-components";
import { AppShell } from "../layouts/AppShell";
import { LoginPage } from "../features/auth/LoginPage";

// Lazy-loaded: each becomes its own JS chunk, fetched on demand
const DashboardPage = lazy(() => import("../features/dashboard/DashboardPage"));
// As you build these, add them the same way:
// const ProjectsPage = lazy(() => import("../features/projects/ProjectsPage"));
// const TasksPage = lazy(() => import("../features/tasks/TasksPage"));

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

const withSuspense = (Component: React.LazyExoticComponent<() => React.ReactElement>) => (  <Suspense fallback={<PageFallback />}>
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
      // { path: "projects", element: withSuspense(ProjectsPage) },
      // { path: "tasks", element: withSuspense(TasksPage) },
    ],
  },
]);