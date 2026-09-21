import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { makeStyles, tokens, Card, Input, Button, Text, Field } from "@fluentui/react-components";
import { login } from "./authApi";

const useStyles = makeStyles({
    page: {
        height: "100vh", display: "flex", alignItems: "center", justifyContent: "center",
        background: `linear-gradient(135deg, ${tokens.colorBrandBackground2} 0%, ${tokens.colorNeutralBackground2} 100%)`,
    },
    card: { width: "380px", padding: "32px" },
    logoRow: { display: "flex", alignItems: "center", gap: "10px", marginBottom: "8px" },
    form: { display: "flex", flexDirection: "column", gap: "16px", marginTop: "20px" },
});

export function LoginPage() {
    const styles = useStyles();
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError("");
        setLoading(true);
        try {
            const result = await login({ email, password });
            if (!result.isSuccessful) {
                setError(result.message);
            } else {
                localStorage.setItem("token", result.response.token);
                navigate("/");
            }
        } catch {
            setError("Invalid email or password.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className={styles.page}>
            <Card className={styles.card}>
                <div className={styles.logoRow}>
                    <span aria-hidden="true" style={{ color: tokens.colorBrandForeground1, fontSize: "24px" }}>✦</span>
                    <Text weight="bold" size={600}>PMO Platform</Text>
                </div>
                <Text size={300} style={{ color: tokens.colorNeutralForeground3 }}>Sign in to continue</Text>
                <form className={styles.form} onSubmit={handleSubmit}>
                    <Field label="Email">
                        <Input value={email} onChange={(_, d) => setEmail(d.value)} type="email" required />
                    </Field>
                    <Field label="Password">
                        <Input value={password} onChange={(_, d) => setPassword(d.value)} type="password" required />
                    </Field>
                    {error && <Text style={{ color: tokens.colorPaletteRedForeground1 }}>{error}</Text>}
                    <Button appearance="primary" type="submit" disabled={loading}>
                        {loading ? "Signing in..." : "Sign in"}
                    </Button>
                </form>
            </Card>
        </div>
    );
}