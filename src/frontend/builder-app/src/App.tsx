import { Link, Outlet } from "react-router-dom";
import { SwordsIcon } from "lucide-react";
import { ModeToggle } from "./components/mode-toggle";
import { GitHubIconButton } from "./components/navigation/github-icon-button";
import "./App.css";
import { Separator } from "./components/ui/separator";
import { NavigationMenu, NavigationMenuItem, NavigationMenuLink, NavigationMenuList } from "./components/ui/navigation-menu";
import { useIsMobile } from "./hooks/use-mobile";
import { Badge } from "./components/ui/badge";
import AppMenuComponent from "./components/navigation/app-menu";
import { cn } from "./lib/utils";

function SizeIndicatorBadge({ className, ...props }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className={cn("uppercase", className)} {...props}>
      <Badge variant={"default"} className="bg-starlights-development block sm:hidden">
        xs
      </Badge>
      <Badge variant={"default"} className="bg-starlights-development hidden sm:block md:hidden">
        sm
      </Badge>
      <Badge variant={"default"} className="bg-starlights-development hidden md:block lg:hidden">
        md
      </Badge>
      <Badge variant={"default"} className="bg-starlights-development hidden lg:block xl:hidden ">
        lg
      </Badge>
      <Badge variant={"default"} className="bg-starlights-development hidden xl:block 2xl:hidden">
        xl
      </Badge>
      <Badge variant={"default"} className="bg-starlights-development hidden 2xl:block">
        2xl
      </Badge>
    </div>
  );
}

function MainNavigation() {
  const isMobile = useIsMobile();
  return (
    <>
      <nav className="flex items-center justify-between h-16">
        <div className="flex items-center justify-start gap-2 ">
          <Link to="/" className=" flex items-center font-heading">
            <SwordsIcon className="h-6 w-6 mr-3  stroke-starlights-accent-600" />
            <span className="hidden lg:inline">Project Starlights</span>
            <span className="lg:hidden">Starlights</span>
          </Link>

          <div className="hidden md:flex items-center justify-start gap-2">
            <Separator orientation="vertical" className="ms-4 me-0 min-h-5 hidden md:block" />
            <NavigationMenu viewport={isMobile} className="z-10 ">
              <NavigationMenuList className="hidden md:flex">
                <NavigationMenuItem>
                  <NavigationMenuLink asChild className="hover:bg-transparent hover:text-primary focus:bg-transparent focus:text-primary">
                    <Link to="/characters">Character Builder</Link>
                  </NavigationMenuLink>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <NavigationMenuLink asChild className="hover:bg-transparent hover:text-primary focus:bg-transparent focus:text-primary">
                    <Link to="/development">Development</Link>
                  </NavigationMenuLink>
                </NavigationMenuItem>
                {/* <NavigationMenuItem className="hidden md:block">
                  <NavigationMenuTrigger>Homebrew Options</NavigationMenuTrigger>
                  <NavigationMenuContent>
                    <ul className="grid w-[200px] gap-4 ">
                      <li>
                        <NavigationMenuLink asChild>
                          <Link to="#">Spell</Link>
                        </NavigationMenuLink>
                        <NavigationMenuLink asChild>
                          <Link to="#">Documentation</Link>
                        </NavigationMenuLink>
                        <NavigationMenuLink asChild>
                          <Link to="#">Blocks</Link>
                        </NavigationMenuLink>
                      </li>
                    </ul>
                  </NavigationMenuContent>
                </NavigationMenuItem> */}
                {/* <NavigationMenuItem>
                  <NavigationMenuLink asChild>
                    <Link to="/characters">Campaigns</Link>
                  </NavigationMenuLink>
                </NavigationMenuItem> */}
                {/* <NavigationMenuItem>
                  <NavigationMenuLink asChild >
                    <Link to="/characters">Compendium</Link>
                  </NavigationMenuLink>
                </NavigationMenuItem> */}
              </NavigationMenuList>
            </NavigationMenu>
          </div>
        </div>

        <div className="flex items-center justify-end gap-2">
          {/* <Separator orientation="vertical" className="min-h-5 mx-4 hidden lg:block" /> */}

          <div className="flex items-center justify-end gap-2">
            <ModeToggle />
            <GitHubIconButton />
          </div>

          {/* <Separator orientation="vertical" className="min-h-5 mx-4 hidden lg:block" />

          <div className="flex items-center gap-2">
            <ButtonGroup className="hidden lg:block">
              <Button variant="default" size="icon-sm">
                <Link to="/characters/create" className="flex items-center gap-2">
                  <Plus size={16} />
                </Link>
              </Button>
            </ButtonGroup>
          </div> */}

          <Separator orientation="vertical" className="min-h-5" />

          <SizeIndicatorBadge className="ms-2" />

          <div className="flex items-center justify-end gap-2">
            <AppMenuComponent />
          </div>

          {/* debug indicator */}
          {/* <div className="flex items-center justify-end gap-2 ms-2 debug">
            <Separator orientation="vertical" className="min-h-5  hidden lg:block" />
            <SizeIndicatorBadge className="ms-2" />
          </div> */}
        </div>
      </nav>
    </>
  );
}

function LandingBackground() {
  return (
    <>
      <div
        className="absolute inset-0 -z-10 pointer-events-none bg-background dark:hidden"
        style={{
          backgroundImage: `
        linear-gradient(45deg, transparent 49%, #e5e7eb 49%, #e5e7eb 51%, transparent 51%),
        linear-gradient(-45deg, transparent 49%, #e5e7eb 49%, #e5e7eb 51%, transparent 51%)
            `,
          backgroundSize: "40px 40px",
          WebkitMaskImage: "radial-gradient(ellipse 70% 60% at 50% 0%, #000 60%, transparent 100%)",
          maskImage: "radial-gradient(ellipse 70% 60% at 50% 0%, #000 60%, transparent 100%)",
        }}
      />
      <div
        className="absolute inset-0 -z-10 pointer-events-none bg-background hidden dark:block"
        style={{
          backgroundImage: `
        linear-gradient(45deg, transparent 49%, rgba(255,255,255,0.06) 49%, rgba(255,255,255,0.06) 51%, transparent 51%),
        linear-gradient(-45deg, transparent 49%, rgba(255,255,255,0.04) 49%, rgba(255,255,255,0.04) 51%, transparent 51%)
            `,
          backgroundSize: "40px 40px",
          WebkitMaskImage: "radial-gradient(ellipse 70% 60% at 50% 0%, #000 60%, transparent 100%)",
          maskImage: "radial-gradient(ellipse 70% 60% at 50% 0%, #000 60%, transparent 100%)",
        }}
      />
    </>
  );
}

function LandingBackground2() {
  return (
    <>
      <div
        className="absolute inset-0 -z-10 pointer-events-none bg-background dark:hidden"
        style={{
          backgroundImage: `
        linear-gradient(45deg, transparent 49%, #e5e7eb 49%, #e5e7eb 51%, transparent 51%),
        linear-gradient(-45deg, transparent 49%, #e5e7eb 49%, #e5e7eb 51%, transparent 51%)
            `,
          backgroundSize: "40px 40px",
          WebkitMaskImage: "radial-gradient(ellipse 70% 60% at 50% 0%, #000 60%, transparent 100%)",
          maskImage: "radial-gradient(ellipse 70% 60% at 50% 0%, #000 60%, transparent 100%)",
        }}
      />
      <div
        className="absolute inset-0 -z-10 pointer-events-none bg-background hidden dark:block"
        style={{
          backgroundImage: `
        linear-gradient(45deg, transparent 49%, rgba(255,255,255,0.06) 49%, rgba(255,255,255,0.06) 51%, transparent 51%),
        linear-gradient(-45deg, transparent 49%, rgba(255,255,255,0.04) 49%, rgba(255,255,255,0.04) 51%, transparent 51%)
            `,
          backgroundSize: "40px 40px",
          WebkitMaskImage: "radial-gradient(ellipse 70% 60% at 50% 0%, #000 60%, transparent 100%)",
          maskImage: "radial-gradient(ellipse 70% 60% at 50% 0%, #000 60%, transparent 100%)",
        }}
      />
    </>
  );
}

function Header() {
  return (
    <div className="sticky top-0 bg-background/80 backdrop-blur-md z-20 border-b border-b-slate-200/50 dark:border-b-slate-700/50">
      <header className="container mx-auto px-4">
        <MainNavigation />
      </header>
    </div>
  );
}

function App() {
  return (
    <>
      <Header />
      {/* <div className="border-b border-b-slate-200/50 dark:border-b-slate-700/50">
        <NavigationMenuDemo />
      </div> */}

      <div className="container mx-auto px-4">
        <LandingBackground />
        <main className="mt-12">
          <Outlet />
        </main>
      </div>
    </>
  );
}

function CharactersLayout() {
  return (
    <>
      <Header />
      {/* <div className="border-b border-b-slate-200/50 dark:border-b-slate-700/50">
        <NavigationMenuDemo />
      </div> */}

      <div className="">
        <div className="">
          <LandingBackground2 />
          <Outlet />
        </div>
      </div>
    </>
  );
}

export default App;
export { CharactersLayout as BuilderLayout };
