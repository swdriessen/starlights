import { useState } from "react";
import { usePlatformStatus } from "./lib/queries";
import "./App.css";
import { Button } from "@/components/ui/button";
import { ThemeProvider } from "@/components/theme-provider";
import { ModeToggle } from "./components/mode-toggle";

function App() {
  const [count, setCount] = useState(0);

  const { data: status, isLoading, isError, error, refetch } = usePlatformStatus(count);

  return (
    <>
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <div>
          <h1>Project Starlights</h1>
          <div className="flex items-center gap-2">
            <Button size="default" onClick={() => setCount((c) => c + 1)}>
              {count}
            </Button>
            <Button size="default" onClick={() => refetch()}>
              Refresh Status
            </Button>
            <ModeToggle />
          </div>
        </div>
        <hr className="my-4" />
        <div>
          <h2>Platform</h2>
          {isLoading && <p>Loading…</p>}
          {isError && <p>Error: {error?.message}</p>}
          {status && (
            <>
              <p>{status.message}</p>
              <p>{status.timestamp}</p>
            </>
          )}
        </div>
      </ThemeProvider>
    </>
  );
}

export default App;
