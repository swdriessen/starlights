import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { useIsMobile } from "@/hooks/use-mobile";
import { cn } from "@/lib/utils";
import {
  AnvilIcon,
  CodeIcon,
  CrownIcon,
  Icon,
  LibraryBigIcon,
  LibraryIcon,
  MapIcon,
  OrbitIcon,
  PencilRulerIcon,
  RouteIcon,
  SwordIcon,
  WandIcon,
} from "lucide-react";
import { cauldron } from "@lucide/lab";
import { Link } from "react-router-dom";
import {
  NavigationMenu,
  NavigationMenuContent,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  NavigationMenuTrigger,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";
import { Outlet } from "react-router-dom";
import { ModeToggle } from "@/components/mode-toggle";
import { GitHubIconButton } from "@/components/navigation/github-icon-button";

export function LandingBackground() {
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

export function LandingBackground2() {
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

export function LandingBackground3() {
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

export function MainNavigation() {
  const isMobile = useIsMobile();
  return (
    <>
      <nav className="flex items-center justify-between h-16 ">
        <div className="flex items-center justify-start gap-2 ">
          <Link to="/" className=" flex items-center font-heading relative">
            <OrbitIcon className="size-6 mr-3 stroke-starlights-purple-600" />
            {/* <CrownIcon className="size-6 mr-3 stroke-starlights-purple-600 " /> */}
            {/* <Icon iconNode={cauldron} size={16} className=" size-6 mr-3  stroke-starlights-purple-600"></Icon> */}
            <span className="hidden lg:inline tracking-widest mt-0.5">Aurora Realms</span>
            <span className="lg:hidden tracking-widest mt-0.5">Aurora Realms</span>
          </Link>

          <div className="hidden md:flex items-center justify-start gap-2 ms-4">
            {/* <Separator orientation="vertical" className="ms-4 me-0 min-h-5 hidden md:block" /> */}
            <NavigationMenu viewport={isMobile} className="z-10 ">
              <NavigationMenuList className="hidden md:flex">
                {/*  */}
                {/* <NavigationMenuItem className="hidden lg:block ">
                  <NavigationMenuLink asChild className={cn(navigationMenuTriggerStyle(), "bg-transparent")}>
                    <Link to="/characters">Characters</Link>
                  </NavigationMenuLink>
                </NavigationMenuItem> */}

                <NavigationMenuItem className="hidden md:block ">
                  <NavigationMenuTrigger className={cn(navigationMenuTriggerStyle(), "bg-transparent")}>Collections</NavigationMenuTrigger>
                  <NavigationMenuContent>
                    <ul className="grid w-[300px] gap-1">
                      <li>
                        <NavigationMenuLink asChild>
                          <div>
                            <Link to="/characters" className="flex flex-row items-center gap-3 font-overpass leading-snug">
                              <CrownIcon size={16} className=" size-9 rounded-sm p-2 stroke-starlights-indigo-600" />
                              <div>
                                <div className="font-medium mt-0.5">Character Collection</div>
                                <div className="text-muted-foreground text-xs">The characters in your collection.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div className="opacity-50 pointer-events-none">
                            <Link to="/campaigns" className="flex flex-row items-center gap-3 font-overpass leading-snug ">
                              <MapIcon size={16} className=" size-9 rounded-sm p-2 stroke-starlights-indigo-600" />
                              <div>
                                <div className="font-medium mt-0.5">Campaigns</div>
                                <div className="text-muted-foreground text-xs ">Manage your character's adventures.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div className="opacity-50 pointer-events-none">
                            <Link to="/campaigns" className="flex flex-row items-center gap-3 font-overpass leading-snug ">
                              <LibraryBigIcon size={16} className=" size-9 rounded-sm p-2 stroke-starlights-indigo-600" />
                              <div>
                                <div className="font-medium mt-0.5">Compendium</div>
                                <div className="text-muted-foreground text-xs ">A searchable archive of player options.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <Separator className="mx-3 max-w-[276px]" />
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div className="opacity-50 pointer-events-none">
                            <Link to="/homebrew" className="flex flex-row items-center gap-3 font-overpass leading-snug ">
                              <Icon iconNode={cauldron} size={16} className=" size-9 rounded-sm p-2 stroke-starlights-purple-600"></Icon>
                              <div>
                                <div className="font-medium mt-0.5">Homebrew Content</div>
                                <div className="text-muted-foreground text-xs ">Create and manage your custom content.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                    </ul>
                  </NavigationMenuContent>
                </NavigationMenuItem>

                <NavigationMenuItem className="hidden md:block ">
                  <NavigationMenuTrigger className={cn(navigationMenuTriggerStyle(), "bg-transparent")}>Character Builder</NavigationMenuTrigger>
                  <NavigationMenuContent>
                    <ul className="grid w-[300px] gap-1">
                      <li>
                        <NavigationMenuLink asChild>
                          <div className=" ">
                            <Link to="/characters" className="flex flex-row items-center gap-3 font-overpass leading-snug ">
                              <RouteIcon size={16} className=" size-9 rounded-sm p-2 stroke-yellow-500 dark:stroke-yellow-600" />
                              <div>
                                <div className="font-medium mt-0.5">Start Page</div>
                                <div className="text-muted-foreground text-xs">Your journey starts here.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div className=" ">
                            <Link to="/characters/12345/builder/class-options" className="flex flex-row items-center gap-3 font-overpass leading-snug ">
                              <AnvilIcon size={16} className=" size-9 rounded-sm p-2 stroke-yellow-500 dark:stroke-yellow-600" />
                              <div>
                                <div className="font-medium mt-0.5">Build Options</div>
                                <div className="text-muted-foreground text-xs ">Choose your build options.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div className="opacity-50 pointer-events-none">
                            <Link to="/characters/12345/builder/spellcasting" className="flex flex-row items-center gap-3 font-overpass leading-snug ">
                              <WandIcon size={16} className=" size-9 rounded-sm p-2 stroke-yellow-500 dark:stroke-yellow-600" />
                              <div>
                                <div className="font-medium mt-0.5 upper">Spellcasting Options</div>
                                <div className="text-muted-foreground text-xs ">Choose your spellcasting options.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div className="opacity-50 pointer-events-none">
                            <Link to="/characters/12345/builder/equipment" className="flex flex-row items-center gap-3 font-overpass leading-snug">
                              <SwordIcon size={16} className=" size-9 rounded-sm p-2 stroke-yellow-500 dark:stroke-yellow-600" />
                              <div>
                                <div className="font-medium mt-0.5">Equipment</div>
                                <div className="text-muted-foreground text-xs ">Choose your equipment.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div className=" ">
                            <Link to="/characters/12345/builder/manage" className="flex flex-row items-center gap-3 font-overpass leading-snug">
                              <PencilRulerIcon size={16} className=" size-9 rounded-sm p-2 stroke-yellow-500 dark:stroke-yellow-600" />
                              <div>
                                <div className="font-medium mt-0.5">Manage Character</div>
                                <div className="text-muted-foreground text-xs ">Manage your character details.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                    </ul>
                  </NavigationMenuContent>
                </NavigationMenuItem>
                {/* <NavigationMenuItem>
                  <NavigationMenuLink asChild className={cn(navigationMenuTriggerStyle(), "bg-transparent")}>
                    <Link to="/app2">Compendium</Link>
                  </NavigationMenuLink>
                </NavigationMenuItem> */}

                <NavigationMenuItem className="hidden lg:block ">
                  <NavigationMenuTrigger className={cn(navigationMenuTriggerStyle(), "bg-transparent")}>Developer Nexus</NavigationMenuTrigger>
                  <NavigationMenuContent>
                    <ul className="grid w-[300px] gap-1">
                      <li>
                        <NavigationMenuLink asChild>
                          <div className="">
                            <Link to="/development" className="flex flex-row items-center gap-3 font-overpass leading-snug">
                              <CodeIcon size={16} className=" size-9 rounded-sm p-2 stroke-starlights-purple-600" />
                              <div>
                                <div className="font-medium mt-0.5">Development Nexus</div>
                                <div className="text-muted-foreground text-xs">Lorem ipsum dolor sit amet consectetur.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div>
                            <Link to="/lib" className="flex flex-row items-center gap-3 font-overpass leading-snug">
                              <LibraryIcon size={16} className=" size-9 rounded-sm p-2 stroke-starlights-purple-600" />
                              <div>
                                <div className="font-medium mt-0.5">Library Page</div>
                                <div className="text-muted-foreground text-xs ">Lorem ipsum dolor sit amet.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                      <li>
                        <Separator className="mx-3 max-w-[276px]" />
                      </li>
                      <li>
                        <NavigationMenuLink asChild>
                          <div>
                            <Link to="/app2" className="flex flex-row items-center gap-3 font-overpass leading-snug">
                              <LibraryIcon size={16} className=" size-9 rounded-sm p-2 stroke-starlights-purple-600" />
                              <div>
                                <div className="font-medium mt-0.5">Builder Layout Demo Page</div>
                                <div className="text-muted-foreground text-xs ">Lorem ipsum dolor sit amet.</div>
                              </div>
                            </Link>
                          </div>
                        </NavigationMenuLink>
                      </li>
                    </ul>
                  </NavigationMenuContent>
                </NavigationMenuItem>
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

          {/* <div className="flex items-center justify-end gap-2">
            <AppMenuComponent />
          </div> */}

          {/* <Separator orientation="vertical" className="min-h-5 ms-2" />
          <Button size="sm" className="ms-2 bg-starlights-purple-600 hover:bg-starlights-purple-700 focus:ring-starlights-purple-600">
            <OrbitIcon className="me-2 " size={16} /> Get Started
          </Button> */}

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

export function Header() {
  return (
    <div className="sticky top-0 bg-background/80 backdrop-blur-md z-20 border-b ">
      <header className="container mx-auto px-4 ">
        <MainNavigation />
      </header>
    </div>
  );
}

export function CharactersLayout() {
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
