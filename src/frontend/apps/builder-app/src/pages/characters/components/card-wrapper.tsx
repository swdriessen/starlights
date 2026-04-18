import { cn } from "@starlights/ui";

function CardWrapper({ children, className }: { children: React.ReactNode; className?: string }) {
  return <div className={cn(`bg-background border rounded-2xl p-1`, className)}>{children}</div>;
}

export { CardWrapper };
