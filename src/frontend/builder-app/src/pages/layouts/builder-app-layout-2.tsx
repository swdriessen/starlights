import { MainNavigation } from "./default-layout";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Button } from "@starlights/ui";

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

function BuilderHeader() {
  return (
    <div className="sticky top-0 bg-background/80 backdrop-blur-md z-20 border-b  ">
      {/* border-l border-r border-dashed */}
      <header className="container mx-auto px-4 ">
        <MainNavigation />
      </header>
      <div className="h-20 hidden ">
        {/* <LandingBackground2 /> */}
        <div className=" h-full container m-auto px-4 border-dashed border-l border-r flex flex-row items-center gap-4 justify-between">
          {/*  */}
          <div>
            <Button variant="ghost" size="default" className="">
              Build
            </Button>
          </div>

          <div className="flex flex-row items-center gap-4">
            <div className="grid flex-1 max-w-50 min-w-25 text-right text-sm font-overpass ">
              <span className="truncate font-medium">Zook 'Ku' Timbers</span>
              <span className="truncate text-sm  opacity-60">Level 3 Svirfneblin Rogue</span>
              {/* <Progress value={45} className="mt-1 h-1 rounded-full  " /> */}
            </div>

            <Avatar className="size-16 rounded-full border-4 border-double">
              <AvatarImage src="portraits/portrait-4.jpg" alt="User Name" className="scale-125" />
              <AvatarFallback className="rounded-lg">CH</AvatarFallback>
            </Avatar>
          </div>
        </div>
      </div>
    </div>
  );
}

function Page() {
  return (
    <>
      <LandingBackground2 />
      <BuilderHeader />
      <div className="  "></div>

      {/* <div className="">
        <div
          className="container mx-auto bg-background border-4 border-double rounded-tl-lg rounded-tr-lg p-4 mt-12 
        flex items-center justify-between gap-4 relative overflow-hidden shadow-md"
        >
          <div></div>
          <div className="flex flex-row items-center gap-4 ">
            <div className="grid flex-1 max-w-50 min-w-25 text-right text-sm font-overpass ">
              <span className="truncate text-xs font-medium">Zook 'Ku' Timbers</span>
              <span className="truncate text-xs  opacity-60">Level 3 Svirfneblin Rogue</span>
            </div>

            <Avatar className="size-16 rounded-md border-4 border-double">
              <AvatarImage src="portraits/portrait-3.jpg" alt="User Name" className="scale-125" />
              <AvatarFallback className="rounded-lg">CH</AvatarFallback>
            </Avatar>
          </div>
        </div>
      </div> */}

      {/* <div className="h-12 bg-sidebar">
        <div className="h-full container m-auto px-4 border-dashed border-b border-l border-r flex flex-row items-center gap-4 justify-between">
          Class Options | Character Origin
          <div className="flex flex-row items-center gap-2">
            <ArrowLeftIcon /> | <ArrowRightIcon />
          </div>
        </div>
      </div> */}

      <div className="mt-12">
        <div className="bg-background container m-auto border-4 border-double rounded-lg p-4">
          <Tabs defaultValue="a" className="mb-0 pb-0 ">
            <div className="flex flex-row justify-between items-end ">
              <TabsList className="bg-transparent ">
                <TabsTrigger value="a" className="data-[state=active]:bg-accent data-[state=active]:shadow-sm border-none  font-overpass  pt-1.75">
                  Class Options
                </TabsTrigger>
                <TabsTrigger value="b" className="data-[state=active]:bg-accent data-[state=active]:shadow-sm border-none  font-overpass  pt-1.75">
                  Character Origin
                </TabsTrigger>
                <TabsTrigger value="c" className="data-[state=active]:bg-accent data-[state=active]:shadow-sm border-none  font-overpass  pt-1.75">
                  Ability Scores
                </TabsTrigger>
                <TabsTrigger value="d" className="data-[state=active]:bg-accent data-[state=active]:shadow-sm border-none  font-overpass  pt-1.75">
                  Languages & Proficiencies
                </TabsTrigger>
                <TabsTrigger value="e" className="data-[state=active]:bg-accent data-[state=active]:shadow-sm border-none  font-overpass  pt-1.75">
                  Feats
                </TabsTrigger>
                {/* <TabsTrigger value="spellcasting" className="data-[state=active]:bg-accent data-[state=active]:shadow-sm border-none  font-overpass  pt-1.75">
                  Spellcasting
                </TabsTrigger> */}
                {/* <TabsTrigger value="equipment" className="data-[state=active]:bg-accent data-[state=active]:shadow-sm border-none  font-overpass  pt-1.75">
                  Equipment
                </TabsTrigger> */}
                {/* <TabsTrigger value="manage" className="data-[state=active]:bg-accent data-[state=active]:shadow-sm border-none  font-overpass  pt-1.75">
                  Manage
                </TabsTrigger> */}
              </TabsList>
              <div className="flex flex-row items-center gap-4 ">
                <div className="grid flex-1 max-w-50 min-w-25 text-right text-sm font-overpass ">
                  <span className="truncate  font-medium">Zook 'Ku' Timbers</span>
                  <span className="truncate text-xs  opacity-60">Level 3 Svirfneblin Rogue</span>
                  {/* <Progress value={45} className="mt-1 h-1 rounded-full  " /> */}
                </div>

                <Avatar className="size-12 rounded-md border-2">
                  <AvatarImage src="portraits/portrait-4.jpg" alt="User Name" className="scale-125" />
                  <AvatarFallback className="rounded-lg">CH</AvatarFallback>
                </Avatar>
              </div>{" "}
              {/* <div className="flex flex-row items-center gap-4 h-12">

                <Button variant="ghost" size="icon-sm" className="">
                  <ChevronLeftIcon />
                </Button>
                <Button variant="ghost" size="icon-sm" className="">
                  <ChevronRightIcon />
                </Button>
                <Separator orientation="vertical" className="data-[orientation=vertical]:max-h-5" />
                <Button variant="ghost" size="icon-sm" className="">
                  <MoreVerticalIcon />
                </Button>
              </div> */}
            </div>
            <div className="">
              <TabsContent value="a">
                <div className="min-h-100 h-100 p-2">
                  <p>Class Content</p>
                </div>
              </TabsContent>
              <TabsContent value="b">
                <div className="min-h-100 h-100 p-2">
                  <p>Origin Page Content</p>
                </div>
              </TabsContent>
              <TabsContent value="c">
                <div className="min-h-100 h-100">
                  <p>Ability Scores Page Content</p>
                </div>
              </TabsContent>
              <TabsContent value="d">
                <div className="min-h-100 h-100">
                  <p>Languages & Proficiencies Page Content</p>
                </div>
              </TabsContent>
              <TabsContent value="e">
                <div className="min-h-100 h-100">
                  <p>Feats Page Content</p>
                </div>
              </TabsContent>
              <TabsContent value="f">
                <div className="min-h-100 h-100">
                  <p>Some Other Page Content</p>
                </div>
              </TabsContent>
            </div>
          </Tabs>
        </div>
      </div>
    </>
  );
}

function BuilderAppLayout2() {
  return (
    <>
      <Page />
    </>
  );
}

export default BuilderAppLayout2;
