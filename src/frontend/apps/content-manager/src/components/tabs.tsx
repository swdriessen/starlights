import { BoxIcon, ChartLine, HouseIcon, PanelsTopLeftIcon, SettingsIcon, UsersRoundIcon } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { ScrollArea, ScrollBar } from "@/components/ui/scroll-area";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";

export default function TableComponent() {
  return (
    <Tabs
      defaultValue="tab-1"
      className="border"
    >
      <ScrollArea>
        <TabsList
          variant="line"
          className="w-full bordermb-3 h-auto min-h-10 gap-2 rounded-none border-b bg-transparent px-0 py-1 text-foreground"
        >
          <TabsTrigger
            value="tab-1"
            className="font-semibold uppercase tracking-wider text-xs"
          >
            Description
          </TabsTrigger>
          <TabsTrigger
            value="tab-2"
            className="font-semibold uppercase tracking-wider text-xs"
          >
            Properties
            {/* <Badge
              className="ms-1.5 min-w-5 bg-primary/15 px-1"
              variant="secondary"
            >
              3
            </Badge> */}
          </TabsTrigger>
          <TabsTrigger
            value="tab-3"
            className="font-semibold uppercase tracking-wider text-xs"
          >
            Settings
          </TabsTrigger>
        </TabsList>
        {/* <ScrollBar orientation="horizontal" /> */}
      </ScrollArea>
      <TabsContent value="tab-1">
        <p className="pt-1 text-center text-muted-foreground text-xs">Content for Tab 1</p>
      </TabsContent>
      <TabsContent value="tab-2">
        <p className="pt-1 text-center text-muted-foreground text-xs">Content for Tab 2</p>
      </TabsContent>
      <TabsContent value="tab-3">
        <p className="pt-1 text-center text-muted-foreground text-xs">Content for Tab 3</p>
      </TabsContent>
    </Tabs>
  );
}
