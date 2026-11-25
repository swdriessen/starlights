import { AlertCircleIcon } from "lucide-react";
import { Alert, AlertDescription, AlertTitle } from "@starlights/ui-framework/alert";
import { Button } from "@starlights/ui-framework/button";
import { Skeleton } from "@starlights/ui-framework/skeleton";

export function CharacterLoading() {
  return <div role="status" aria-label="Loading Characters" className="flex flex-col gap-4">
    <Skeleton className="h-8 w-64" />
    <Skeleton className="h-40 w-full" />
    <span className="sr-only">Loading saved Character information...</span>
  </div>;
}

export function CharacterError({ title, children, retry }: { title: string; children: React.ReactNode; retry?: () => void }) {
  return <Alert variant="destructive" role="alert">
    <AlertCircleIcon aria-hidden="true" />
    <AlertTitle>{title}</AlertTitle>
    <AlertDescription>
      {children}
      {retry && <Button variant="outline" size="sm" onClick={retry}>Try again</Button>}
    </AlertDescription>
  </Alert>;
}
