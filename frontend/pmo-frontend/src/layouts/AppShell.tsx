import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { createElement, type ElementType } from "react";
import { makeStyles, tokens, Avatar, Text, Button } from "@fluentui/react-components";
import {
  Home24Regular, FolderOpen24Regular, TaskListSquareLtr24Regular,
  Clock24Regular, MoneyHand24Regular, Warning24Regular,
  SignOut24Regular, Sparkle24Regular,
} from "@fluentui/react-icons";

const useStyles = makeStyles({
  shell: { display: "flex", height: "100vh", backgroundColor: tokens.colorNeutralBackground2 },
  sidebar: {
    width: "260px", backgroundColor: tokens.colorNeutralBackground1,
    borderRight: `1px solid ${tokens.colorNeutralStroke2}`,
    display: "flex", flexDirection: "column", padding: "16px 12px",
  },
  logo: { display: "flex", alignItems: "center", gap: "8px", padding: "8px 12px", marginBottom: "24px" },
  navItem: {
    display: "flex", alignItems: "center", gap: "12px",
    padding: "10px 12px", borderRadius: "6px", cursor: "pointer",
    color: tokens.colorNeutralForeground2, fontSize: "14px", fontWeight: 500,
    ":hover": { backgroundColor: tokens.colorNeutralBackground1Hover },
  },
  navItemActive: { backgroundColor: tokens.colorBrandBackground2, color: tokens.colorBrandForeground1 },
  main: { flex: 1, display: "flex", flexDirection: "column", overflow: "hidden" },
  topbar: {
    height: "56px", display: "flex", alignItems: "center", justifyContent: "flex-end",
    padding: "0 24px", borderBottom: `1px solid ${tokens.colorNeutralStroke2}`, gap: "12px",
  },
  content: { flex: 1, overflow: "auto", padding: "24px" },
});

const navItems = [
  { label: "Dashboard", icon: Home24Regular, path: "/" },
  { label: "Projects", icon: FolderOpen24Regular, path: "/projects" },
  { label: "Tasks", icon: TaskListSquareLtr24Regular, path: "/tasks" },
  { label: "Timesheets", icon: Clock24Regular, path: "/timesheets" },
  { label: "Financials", icon: MoneyHand24Regular, path: "/financials" },
  { label: "Risks", icon: Warning24Regular, path: "/risks" },
  { label: "AI Co-Pilot", icon: Sparkle24Regular, path: "/copilot" },
];

export function AppShell() {
  const styles = useStyles();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <div className={styles.shell}>
      <aside className={styles.sidebar}>
        <div className={styles.logo}>
          {createElement(Sparkle24Regular as unknown as ElementType, {
            style: { color: tokens.colorBrandForeground1 },
          })}
          <Text weight="bold" size={500}>PMO Platform</Text>
        </div>
        {navItems.map((item) => {
          const isActive = location.pathname === item.path;
          const Icon = item.icon;
          return (
            <div
              key={item.path}
              className={`${styles.navItem} ${isActive ? styles.navItemActive : ""}`}
              onClick={() => navigate(item.path)}
            >
              {createElement(Icon as unknown as ElementType)}
              <span>{item.label}</span>
            </div>
          );
        })}
      </aside>

      <div className={styles.main}>
        <header className={styles.topbar}>
          <Avatar name="Krishnakumar" size={32} color="colorful" />
          <Button
            appearance="subtle"
            icon={createElement(SignOut24Regular as unknown as ElementType)}
            onClick={handleLogout}
          >
            Sign out
          </Button>
        </header>
        <main className={styles.content}>
          <Outlet />
        </main>
      </div>
    </div>
  );
}