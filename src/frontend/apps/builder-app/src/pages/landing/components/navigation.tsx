// "use client";

import * as React from "react";
import { CircleIcon } from "lucide-react";

import {
  NavigationMenu,
  NavigationMenuContent,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  NavigationMenuTrigger,
  navigationMenuTriggerStyle,
} from "@starlights/ui-framework/navigation-menu";
import { Link } from "react-router-dom";

const components: { title: string; href: string; description: string }[] = [
  {
    title: "Alert Dialog",
    href: "/docs/primitives/alert-dialog",
    description: "A modal dialog that interrupts the user with important content and expects a response.",
  },
  {
    title: "Hover Card",
    href: "/docs/primitives/hover-card",
    description: "For sighted users to preview content available behind a link.",
  },
  {
    title: "Progress",
    href: "/docs/primitives/progress",
    description: "Displays an indicator showing the completion progress of a task, typically displayed as a progress bar.",
  },
  {
    title: "Scroll-area",
    href: "/docs/primitives/scroll-area",
    description: "Visually or semantically separates content.",
  },
  {
    title: "Tabs",
    href: "/docs/primitives/tabs",
    description: "A set of layered sections of content—known as tab panels—that are displayed one at a time.",
  },
  {
    title: "Tooltip",
    href: "/docs/primitives/tooltip",
    description: "A popup that displays information related to an element when the element receives keyboard focus or the mouse hovers over it.",
  },
];

export function NavigationMenuDemo() {
  return (
    <NavigationMenu className="z-10 h-16 max-w-screen ">
      <NavigationMenuList className="flex justify-between flex-wrap w-screen items-center px-4">
        <span className="flex-1  flex">
          <CircleIcon className="mr-2 h-4 w-4" />
        </span>
        <span className="flex-grow "></span>
        <NavigationMenuItem>
          <NavigationMenuLink render={<Link to="/docs" />} className={navigationMenuTriggerStyle()}>
            Docs
          </NavigationMenuLink>
        </NavigationMenuItem>

        <NavigationMenuItem className=" ">
          <NavigationMenuTrigger className="">Collections</NavigationMenuTrigger>
          <NavigationMenuContent className="max-w-[100vw]">
            <ul className="grid gap-2 w-screen lg:grid-cols-[.75fr_1fr]">
              <li className="row-span-3">
                <NavigationMenuLink
                  render={
                    <a
                      className="from-muted/50 to-muted relative flex h-full w-full flex-col justify-end rounded-md bg-linear-to-b p-4 no-underline outline-hidden transition-all duration-200 select-none focus:shadow-md md:p-6"
                      href="/"
                    />
                  }
                >
                    {/* <img
                      src={"https://www.dndbeyond.com/attachments/12/424/flash-sale.jpg"}
                      alt="test"
                      className={`absolute inset-0 w-full h-full object-cover transition-transform duration-500 scale-100 group-hover:scale-105`}
                    /> */}
                    <div className="mb-2 text-lg font-medium sm:mt-4">shadcn/ui</div>
                    <p className="text-muted-foreground text-sm leading-tight">Beautifully designed components built with Tailwind CSS.</p>
                </NavigationMenuLink>
              </li>
              <ListItem href="/docs" title="Introduction">
                Re-usable components built using Base UI and Tailwind CSS.
              </ListItem>
              <ListItem href="/docs/installation" title="Installation">
                How to install dependencies and structure your app.
              </ListItem>
              <ListItem href="/docs/primitives/typography" title="Typography">
                Styles for headings, paragraphs, lists...etc
              </ListItem>
            </ul>
          </NavigationMenuContent>
        </NavigationMenuItem>

        <NavigationMenuItem>
          <NavigationMenuTrigger>Components</NavigationMenuTrigger>
          <NavigationMenuContent className="max-w-[100vw]">
            <ul className="grid gap-2  w-screen md:grid-cols-2 ">
              {components.map((component) => (
                <ListItem key={component.title} title={component.title} href={component.href}>
                  {component.description}
                </ListItem>
              ))}
            </ul>
          </NavigationMenuContent>
        </NavigationMenuItem>
        <span className="flex-grow "></span>
        <span className="flex-1  flex justify-end">
          <CircleIcon className="mr-2 h-4 w-4" />
        </span>
      </NavigationMenuList>
    </NavigationMenu>
  );
}

function ListItem({ title, children, href, ...props }: React.ComponentPropsWithoutRef<"li"> & { href: string }) {
  return (
    <li {...props}>
      <NavigationMenuLink render={<Link to={href} />}>
        <div className="text-sm leading-none font-medium">{title}</div>
        <p className="text-muted-foreground line-clamp-2 text-sm leading-snug">{children}</p>
      </NavigationMenuLink>
    </li>
  );
}
