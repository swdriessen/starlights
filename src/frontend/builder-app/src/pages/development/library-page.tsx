import { Button } from "@starlights/ui";

export function LibraryDevelopmentPage() {
  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <p className="text-sm text-muted-foreground">Buttons provided by the starlights-ui workspace build.</p>
        <div className="flex flex-wrap gap-2">
          <Button>Primary</Button>
          <Button variant="outline">Outline</Button>
          <Button variant="ghost">Ghost</Button>
          <Button variant="destructive">Ghost</Button>
        </div>
      </div>
    </div>
  );
}
