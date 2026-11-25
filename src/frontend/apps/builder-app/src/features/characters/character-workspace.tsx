import { Link, Outlet } from "react-router-dom";
import { SparklesIcon } from "lucide-react";
import { ModeToggle } from "@/components/mode-toggle";

export function CharacterWorkspace() {
  return <div className="min-h-screen bg-background">
    <header className="border-b">
      <div className="mx-auto flex max-w-6xl items-center justify-between gap-6 px-6 py-5">
        <Link to="/characters" className="flex items-center gap-3 font-semibold tracking-tight">
          <SparklesIcon className="size-5 text-primary" aria-hidden="true" />
          Starlights <span className="hidden font-normal text-muted-foreground sm:inline">/ character workshop</span>
        </Link>
        <ModeToggle />
      </div>
    </header>
    <main id="main-content" className="mx-auto flex max-w-6xl flex-col gap-8 px-6 py-10 md:py-14">
      <Outlet />
    </main>
  </div>;
}
