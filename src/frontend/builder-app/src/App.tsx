import { useState } from "react";
import { usePlatformStatus } from "./lib/queries";
import "./App.css";
import { Button } from "@/components/ui/button";

function App() {
  const [count, setCount] = useState(0);

  const { data: status, isLoading, isError, error, refetch } = usePlatformStatus(count);

  return (
    <>
      <div>
        <h1>Project Starlights</h1>
        <div className="flex items-center gap-2">
          <Button size={"sm"} onClick={() => setCount((c) => c + 1)}>
            {count}
          </Button>
          <Button size={"sm"} onClick={() => refetch()}>
            Refresh Status
          </Button>
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
    </>
  );
}

export default App;
