import { Moon, Sun } from "lucide-react";
import { Toggle } from "@/components/ui/toggle";
import { useTheme } from "@/components/theme-provider";

export function ModeToggle() {
  const { theme, setTheme } = useTheme();

  return (
    <>
      <Toggle
        size="sm"
        variant="default"
        className="group hover:bg-accent/50 data-[state=on]:hover:bg-accent data-[state=on]:bg-transparent"
        onPressedChange={() => {
          setTheme(theme === "dark" ? "light" : "dark");
        }}
      >
        <Sun size={16} className="shrink-0 scale-0 opacity-0 transition-all dark:scale-100 dark:opacity-100" aria-hidden="true" />
        <Moon size={16} className="absolute shrink-0 scale-100 opacity-100 transition-all dark:scale-0 dark:opacity-0" aria-hidden="true" />
        <span className="sr-only">Toggle theme</span>
      </Toggle>
    </>
  );
}
