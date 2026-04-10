import * as React from "react";

import { NavMain } from "@/components/nav-main";
import { NavProjects } from "@/components/nav-projects";
import { NavUser } from "@/components/nav-user";
import { TeamSwitcher } from "@/components/team-switcher";
import { Sidebar, SidebarContent, SidebarFooter, SidebarHeader, SidebarRail } from "@/components/ui/sidebar";
import {
  GalleryVerticalEndIcon,
  AudioLinesIcon,
  TerminalSquareIcon,
  BotIcon,
  BookOpenIcon,
  Settings2Icon,
  FrameIcon,
  PieChartIcon,
  MapIcon,
  BookIcon,
  BookUpIcon,
  BookAIcon,
  BookUp2Icon,
  BookTextIcon,
  NotebookTabsIcon,
} from "lucide-react";

// This is sample data.
const data = {
  user: {
    name: "shadcn",
    email: "m@example.com",
    avatar: "/avatars/shadcn.jpg",
  },
  teams: [
    {
      name: "Dungeons & Dragons",
      logo: <NotebookTabsIcon />,
      plan: "DND2024",
    },
    {
      name: "Daggerheart",
      logo: <NotebookTabsIcon />,
      plan: "DH",
    },
  ],
  navMain: [
    {
      title: "Content",
      url: "#",
      icon: <TerminalSquareIcon />,
      isActive: true,
      items: [
        {
          title: "Spell",
          url: "#",
        },
        {
          title: "Proficiency",
          url: "#",
        },
        {
          title: "Item",
          url: "/ruleset/item",
        },
        {
          title: "Abilities",
          url: "#",
        },
        {
          title: "Saving Throws",
          url: "#",
        },
        {
          title: "Skills",
          url: "#",
        },
        {
          title: "Creation Options",
          url: "#",
        },
        {
          title: "Character Options",
          url: "#",
        },
      ],
    },
    // {
    //   title: "Rules",
    //   url: "#",
    //   icon: <BotIcon />,
    //   items: [
    //     {
    //       title: "Genesis",
    //       url: "#",
    //     },
    //     {
    //       title: "Explorer",
    //       url: "#",
    //     },
    //     {
    //       title: "Quantum",
    //       url: "#",
    //     },
    //   ],
    // },
    // {
    //   title: "Ruleset Core",
    //   url: "#",
    //   icon: <BookOpenIcon />,
    //   items: [
    //     {
    //       title: "Abilities",
    //       url: "#",
    //     },
    //     {
    //       title: "Saving Throws",
    //       url: "#",
    //     },
    //     {
    //       title: "Skills",
    //       url: "#",
    //     },
    //     {
    //       title: "Creation Options",
    //       url: "#",
    //     },
    //     {
    //       title: "Character Options",
    //       url: "#",
    //     },
    //   ],
    // },
    // {
    //   title: "Character",
    //   url: "#",
    //   icon: <Settings2Icon />,
    //   items: [
    //     {
    //       title: "Creation Options",
    //       url: "#",
    //     },
    //     {
    //       title: "Character Options",
    //       url: "#",
    //     },
    //   ],
    // },
  ],
  projects: [
    {
      name: "Player's Handbook",
      url: "#",
      icon: <BookTextIcon />,
    },
    {
      name: "Dungeon Master's Guide",
      url: "#",
      icon: <BookTextIcon />,
    },
    {
      name: "Monster Manual",
      url: "#",
      icon: <BookTextIcon />,
    },
  ],
};

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  return (
    <Sidebar
      collapsible="icon"
      {...props}
    >
      <SidebarHeader>
        <TeamSwitcher teams={data.teams} />
      </SidebarHeader>
      <SidebarContent>
        <NavMain items={data.navMain} />
        <NavProjects projects={data.projects} />
      </SidebarContent>
      <SidebarFooter>
        <NavUser user={data.user} />
      </SidebarFooter>
      <SidebarRail />
    </Sidebar>
  );
}
