import { cn } from "@/lib/utils";

export default function ProseSection({ children, className }: { children: React.ReactNode; className?: string }) {
  return <div className={cn("prose prose-neutral dark:prose-invert max-w-none", className)}>{children}</div>;
}
