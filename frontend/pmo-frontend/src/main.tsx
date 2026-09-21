import React from "react";
import ReactDOM from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import { FluentProvider } from "@fluentui/react-components";
import { QueryClientProvider } from "@tanstack/react-query";
import { router } from "./app/router";
import { lightTheme } from "./app/theme";
import { queryClient } from "./app/queryClient";

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <FluentProvider theme={lightTheme}>
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={router} />
      </QueryClientProvider>
    </FluentProvider>
  </React.StrictMode>
);