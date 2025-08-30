import { Button } from "@/components/ui/button";
import { RefreshCcw } from "lucide-react";
import { usePlatformStatus } from "@/lib/queries";

export default function LandingPage() {
  const { data: status, isLoading, isError, error, refetch } = usePlatformStatus(1);

  return (
    <div className="space-y-4">
      <div className="space-y-2">
        <h2 className="text-xl font-semibold">Landing</h2>
        <p className="text-sm text-muted-foreground">This is the Landing page for the Starlights builder app.</p>
      </div>
      <div className="flex items-center gap-2">
        <Button size="sm" onClick={() => refetch()}>
          <RefreshCcw />
          Refresh Status
        </Button>
      </div>
      <div>
        {isLoading && <p>Loading…</p>}
        {isError && <p>Error: {error?.message}</p>}
        {status && (
          <>
            <p>{status.message}</p>
            <p>{status.timestamp}</p>
          </>
        )}
      </div>
    </div>
  );
}
