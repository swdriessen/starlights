import { Link, Outlet } from "react-router-dom";
import { Plus, SwordsIcon } from "lucide-react";
import { ModeToggle } from "./components/mode-toggle";
import { GitHubIconButton } from "./components/navigation/github-icon-button";
import "./App.css";
import { NavigationMenuDemo } from "./pages/landing/components/navigation";
import { Separator } from "./components/ui/separator";
import { NavigationMenu, NavigationMenuItem, NavigationMenuLink, NavigationMenuList } from "./components/ui/navigation-menu";
import { useIsMobile } from "./hooks/use-mobile";
import { Button } from "@/components/ui/button";
import { ButtonGroup } from "@/components/ui/button-group";
import { Badge } from "./components/ui/badge";

function SizeIndicatorBadge({ className, ...props }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className={className} {...props}>
      <Badge variant={"secondary"} className="uppercase block sm:hidden">
        xs
      </Badge>
      <Badge variant={"secondary"} className="uppercase hidden sm:block md:hidden">
        sm
      </Badge>
      <Badge variant={"secondary"} className="uppercase hidden md:block lg:hidden">
        md
      </Badge>
      <Badge variant={"secondary"} className="uppercase hidden lg:block xl:hidden">
        lg
      </Badge>
      <Badge variant={"secondary"} className="uppercase hidden xl:block 2xl:hidden">
        xl
      </Badge>
      <Badge variant={"secondary"} className="uppercase hidden 2xl:block">
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
        <div className="flex items-center justify-start gap-2">
          <Link to="/" className=" flex items-center font-heading">
            <SwordsIcon className="h-6 w-6 mr-3  " />
            <span className="hidden lg:inline">Project Starlights</span>
            <span className="lg:hidden">Starlights</span>
          </Link>
          <div className="hidden md:flex items-center justify-start gap-2">
            <Separator orientation="vertical" className="ms-4 me-0 min-h-5 hidden md:block" />
            <NavigationMenu viewport={isMobile} className="z-10">
              <NavigationMenuList className="hidden md:flex">
                <NavigationMenuItem>
                  <NavigationMenuLink asChild>
                    <Link to="/characters">Characters</Link>
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

        <div className="flex items-center justify-end">
          <div className="flex items-center gap-2">
            <ButtonGroup className="hidden lg:block">
              <Button variant="outline" size="sm">
                <Link to="/characters/create" className="flex items-center gap-2">
                  <Plus size={16} />
                  New Character
                </Link>
              </Button>
            </ButtonGroup>
          </div>
          <Separator orientation="vertical" className="min-h-5 mx-4 hidden lg:block" />
          <div className="flex items-center justify-end gap-2">
            <SizeIndicatorBadge className="" />
            <GitHubIconButton />
            <ModeToggle />
          </div>
        </div>
      </nav>
    </>
  );
}

function App() {
  return (
    <>
      <div className="sticky top-0 bg-background/80 backdrop-blur-md z-20 border-b border-b-slate-200/50 dark:border-b-slate-700/50">
        <header className="px-4">
          <MainNavigation />
        </header>
      </div>
      {/* <div className="border-b border-b-slate-200/50 dark:border-b-slate-700/50">
        <NavigationMenuDemo />
      </div> */}

      <div className="container mx-auto px-4 ">
        <main className="mt-12">
          <Outlet />
        </main>
      </div>
    </>
  );
}

export default App;
