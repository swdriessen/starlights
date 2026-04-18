import { Outlet } from "react-router-dom";
import { Header, LandingBackground } from "./default-layout";
import DescriptionPanel from "../characters/builder/components/description-panel";
import { Button } from "@starlights/ui";
import { ChevronLeftIcon, ChevronRightIcon, MoreVerticalIcon, SwordsIcon } from "lucide-react";
import { Separator } from "@/components/ui/separator";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import DescriptionProseSection from "@/components/description-section";

function DescriptionSidebar() {
  return (
    <>
      <div className="bg-card border-l border-t  fixed top-16 bottom-0 right-0 w-120 2xl:w-140 hidden">
        <Tabs defaultValue="description" className=" bg-yellow-500">
          <div className="flex flex-row justify-between items-center bg-card h-14 px-2 -mb-2">
            <TabsList className="bg-transparent ">
              <TabsTrigger
                value="description"
                className="data-[state=active]:bg-muted data-[state=active]:shadow-none border-none uppercase tracking-widest font-overpass text-xs pt-1.75"
              >
                Information
              </TabsTrigger>
              {/* <TabsTrigger
                value="rules"
                className="data-[state=active]:bg-muted data-[state=active]:shadow-none border-none uppercase tracking-widest font-overpass text-xs pt-1.25"
              >
                Images
              </TabsTrigger> */}
            </TabsList>

            <div className="flex flex-row items-center gap-2 h-12">
              {/* <Separator orientation="vertical" className="max-h-5" /> */}

              <Button variant="ghost" size="icon" className="">
                <ChevronLeftIcon />
              </Button>
              <Button variant="ghost" size="icon" className="">
                <ChevronRightIcon />
              </Button>
              <Separator orientation="vertical" className="max-h-5" />
              <Button variant="ghost" size="icon" className="">
                <MoreVerticalIcon />
              </Button>
            </div>
          </div>
          <div className="bg-card  border-t ps-4">
            <TabsContent value="description">
              <DescriptionPanel id="123" />
            </TabsContent>
            <TabsContent value="rules">
              <div className="pt-3 pe-3">
                <DescriptionProseSection className="font-overpass">
                  <h2>Panel 2</h2>
                  <p>This is a second tab in the description panel.</p>
                </DescriptionProseSection>
              </div>
            </TabsContent>
          </div>
        </Tabs>
      </div>
    </>
  );
}

function CharactersLayout() {
  return (
    <>
      <Header />

      <div></div>

      <div className="bg-card border-b h-14 flex flex-row justify-end gap-2">
        <div className="flex flex-row items-center justify-between gap-2 p-4 border-l w-120">
          <Button variant="ghost" size="icon" className="">
            <SwordsIcon />
          </Button>
          <span className="grow"></span>

          {/* <Separator orientation="vertical" /> */}
          <Button variant="ghost" size="icon" className="">
            <ChevronLeftIcon />
          </Button>
          <Button variant="ghost" size="icon" className="">
            <ChevronRightIcon />
          </Button>
          <Separator orientation="vertical" />
          <Button variant="ghost" size="icon" className="">
            <MoreVerticalIcon />
          </Button>
        </div>
        {/* <Button variant="ghost" size="icon" className="">
          <SidebarOpenIcon />
        </Button> */}
      </div>

      <div className="">
        {/* temp margin */}
        <div className="mr-120">
          <LandingBackground />
          <Outlet />
        </div>
      </div>

      <DescriptionSidebar />

      {/* <div className="fixed h-10 bottom-16 left-0 w-full bg-background/80 backdrop-blur-md z-20 border-t border-t-slate-200/50 dark:border-t-slate-700/50">
        this is a footer
      </div> */}
    </>
  );
}

export default CharactersLayout;
