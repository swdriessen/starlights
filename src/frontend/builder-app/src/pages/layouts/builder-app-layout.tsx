import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { AppSidebar, RightAppSidebar } from "@starlights/ui/components/app-sidebar";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@starlights/ui/components/ui/breadcrumb";
import { SidebarInset, SidebarProvider, SidebarTrigger, useSidebar } from "@starlights/ui/components/ui/sidebar";
import { PanelRight } from "lucide-react";
import React from "react";
import { Outlet } from "react-router-dom";

export function Page() {
  return (
    <SidebarProvider className="">
      <AppSidebar className="" />
      <SidebarInset className=" ">
        <header className="flex h-16 shrink-0 items-center gap-2 ">
          <div className="flex items-center gap-2 px-4 ">
            <SidebarTrigger className="-ml-1" />
            <Separator orientation="vertical" className="mr-2 data-[orientation=vertical]:h-4" />
            <Breadcrumb>
              <BreadcrumbList>
                <BreadcrumbItem className="hidden md:block">
                  <BreadcrumbLink href="#">Building Your Application</BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbSeparator className="hidden md:block" />
                <BreadcrumbItem>
                  <BreadcrumbPage>Class Selections</BreadcrumbPage>
                </BreadcrumbItem>
              </BreadcrumbList>
            </Breadcrumb>
          </div>
        </header>
        <div className="flex flex-1 flex-col gap-4 p-4 pt-4 ">
          <div className="grid auto-rows-min gap-4 md:grid-cols-3">
            <div className="bg-muted/50 aspect-video rounded-xl" />
            <div className="bg-muted/50 aspect-video rounded-xl" />
            <div className="bg-muted/50  rounded-xl overflow-hidden"></div>
          </div>
          <div className="bg-muted/50 min-h-screen flex-1 rounded-xl md:min-h-min">
            <Outlet />
          </div>
        </div>
      </SidebarInset>
    </SidebarProvider>
  );
}

export function MobileSidebarController({ open, setOpen }: { open: boolean; setOpen: (open: boolean) => void }) {
  const { openMobile, setOpenMobile, isMobile } = useSidebar();
  const lastOpen = React.useRef<boolean | undefined>(undefined);
  const lastOpenMobile = React.useRef<boolean | undefined>(undefined);
  const lastIsMobile = React.useRef<boolean | undefined>(undefined);

  React.useEffect(() => {
    // If not mobile, we don't need to sync openMobile
    if (!isMobile) {
      lastIsMobile.current = isMobile;
      return;
    }

    // If we just switched to mobile, force sync from open -> openMobile
    if (lastIsMobile.current !== isMobile) {
      setOpenMobile(open);
      lastIsMobile.current = isMobile;
      // Update refs to current values to avoid double trigger
      lastOpen.current = open;
      // We don't update lastOpenMobile here because we want the next render (when openMobile updates)
      // to be handled normally or we assume it will match.
      // Actually, if we setOpenMobile, the next render will have new openMobile.
      // We should let the next render handle the "openMobile changed" check if needed,
      // but since we initiated it, we expect it.
      return;
    }

    // Check if 'open' prop changed
    if (open !== lastOpen.current) {
      lastOpen.current = open;
      if (openMobile !== open) {
        setOpenMobile(open);
      }
    }
    // Check if 'openMobile' state changed
    else if (openMobile !== lastOpenMobile.current) {
      lastOpenMobile.current = openMobile;
      if (open !== openMobile) {
        setOpen(openMobile);
      }
    }
  }, [open, openMobile, isMobile, setOpenMobile, setOpen]);

  return null;
}

function Page2() {
  const [rightOpen, setRightOpen] = React.useState(true);

  return (
    <>
      <div className="">
        <SidebarProvider
          className="flex flex-auto border-4 border-amber-400"
          // style={
          //   {
          //     "--sidebar-width": "350px",
          //   } as React.CSSProperties
          // }
        >
          <AppSidebar side="left" variant="inset" collapsible="icon" />

          <SidebarInset className="debug">
            <header className="flex h-16 shrink-0 items-center gap-2 justify-between pr-4">
              <div className="flex items-center gap-2 px-4 ">
                <SidebarTrigger className="-ml-1" />
                <Separator orientation="vertical" className="mr-2 data-[orientation=vertical]:h-4" />
                <Breadcrumb>
                  <BreadcrumbList>
                    <BreadcrumbItem className="hidden md:block">
                      <BreadcrumbLink href="#">Building Your Application</BreadcrumbLink>
                    </BreadcrumbItem>
                    <BreadcrumbSeparator className="hidden md:block" />
                    <BreadcrumbItem>
                      <BreadcrumbPage>Class Selections</BreadcrumbPage>
                    </BreadcrumbItem>
                  </BreadcrumbList>
                </Breadcrumb>
              </div>
              <Button variant="ghost" size="icon" className="h-7 w-7" onClick={() => setRightOpen(!rightOpen)}>
                <PanelRight />
                <span className="sr-only">Toggle right sidebar</span>
              </Button>
            </header>
            <div className="flex flex-1 flex-col gap-4 p-4 pt-4 ">
              <div className="grid auto-rows-min gap-4 md:grid-cols-3">
                <div className="bg-muted/50 aspect-video rounded-xl" />
                <div className="bg-muted/50 aspect-video rounded-xl" />
                <div className="bg-muted/50  rounded-xl overflow-hidden"></div>
              </div>
              <div className="bg-muted/50 min-h-screen flex-1 rounded-xl md:min-h-min">
                <Outlet />
              </div>
            </div>
          </SidebarInset>
        </SidebarProvider>

        <SidebarProvider
          open={rightOpen}
          onOpenChange={setRightOpen}
          className="hidden md:flex border-4 border-cyan-400"
          style={
            {
              "--sidebar-width": "450px",
            } as React.CSSProperties
          }
        >
          {/* <MobileSidebarController open={rightOpen} setOpen={setRightOpen} /> */}
          <RightAppSidebar side="right" variant="inset" collapsible="icon" className="border-l" />
        </SidebarProvider>
      </div>
    </>
  );
}
function BuilderAppLayout() {
  return (
    <>
      <Page2 />
    </>
  );
}

export default BuilderAppLayout;
