"use client"

import * as React from "react"

import {
    ContainerIcon,
    CopyIcon,
    DownloadIcon,
    InfoIcon,
    MonitorIcon,
    MoreHorizontalIcon,
    PlusIcon,
    ServerIcon,
    ShareIcon,
    TerminalIcon,
    ZapIcon,
} from "lucide-react"

import { Button } from "ui-framework/button"
import { Card, CardContent } from "ui-framework/card"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuGroup,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "ui-framework/dropdown-menu"
import {
    Empty,
    EmptyContent,
    EmptyDescription,
    EmptyHeader,
    EmptyMedia,
    EmptyTitle,
} from "ui-framework/empty"
import { Field, FieldDescription, FieldLabel } from "ui-framework/field"
import {
    InputGroup,
    InputGroupAddon,
    InputGroupButton,
    InputGroupInput,
} from "ui-framework/input-group"
import {
    Item,
    ItemActions,
    ItemContent,
    ItemDescription,
    ItemTitle,
} from "ui-framework/item"
import { Separator } from "ui-framework/separator"
import { Spinner } from "ui-framework/spinner"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "ui-framework/tabs"
import { Tooltip, TooltipContent, TooltipTrigger } from "ui-framework/tooltip"

export function CodespacesCard() {
  const [isCreatingCodespace, setIsCreatingCodespace] = React.useState(false)
  return (
    <Card>
      <CardContent>
        <Tabs defaultValue="codespaces">
          <TabsList className="w-full">
            <TabsTrigger value="codespaces">Codespaces</TabsTrigger>
            <TabsTrigger value="local">Local</TabsTrigger>
          </TabsList>
          <TabsContent value="codespaces">
            <Item size="sm" className="px-1 pt-2">
              <ItemContent>
                <ItemTitle>Codespaces</ItemTitle>
                <ItemDescription>Your workspaces in the cloud</ItemDescription>
              </ItemContent>
              <ItemActions>
                <Tooltip>
                  <TooltipTrigger
                    render={<Button variant="ghost" size="icon-sm" />}
                  >
                    <PlusIcon />
                  </TooltipTrigger>
                  <TooltipContent side="bottom">
                    Create a codespace on main
                  </TooltipContent>
                </Tooltip>
                <DropdownMenu>
                  <DropdownMenuTrigger
                    render={<Button variant="ghost" size="icon-sm" />}
                  >
                    <MoreHorizontalIcon />
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end" className="w-56">
                    <DropdownMenuGroup>
                      <DropdownMenuItem>
                        <PlusIcon />
                        New with options...
                      </DropdownMenuItem>
                      <DropdownMenuItem>
                        <ContainerIcon />
                        Configure dev container
                      </DropdownMenuItem>
                      <DropdownMenuItem>
                        <ZapIcon />
                        Set up prebuilds
                      </DropdownMenuItem>
                    </DropdownMenuGroup>
                    <DropdownMenuSeparator />
                    <DropdownMenuGroup>
                      <DropdownMenuItem>
                        <ServerIcon />
                        Manage codespaces
                      </DropdownMenuItem>
                      <DropdownMenuItem>
                        <ShareIcon />
                        Share deep link
                      </DropdownMenuItem>
                      <DropdownMenuItem>
                        <InfoIcon />
                        What are codespaces?
                      </DropdownMenuItem>
                    </DropdownMenuGroup>
                  </DropdownMenuContent>
                </DropdownMenu>
              </ItemActions>
            </Item>
            <Separator className="-mx-2 my-2 w-auto!" />
            <Empty className="p-4">
              <EmptyHeader>
                <EmptyMedia variant="icon">
                  <ServerIcon />
                </EmptyMedia>
                <EmptyTitle>No codespaces</EmptyTitle>
                <EmptyDescription>
                  You don&apos;t have any codespaces with this repository
                  checked out
                </EmptyDescription>
              </EmptyHeader>
              <EmptyContent>
                <Button
                  size="sm"
                  onClick={() => {
                    setIsCreatingCodespace(true)
                    setTimeout(() => {
                      setIsCreatingCodespace(false)
                    }, 2000)
                  }}
                  disabled={isCreatingCodespace}
                >
                  {isCreatingCodespace ? (
                    <Spinner data-icon="inline-start" />
                  ) : (
                    ""
                  )}
                  Create Codespace
                </Button>
                <a
                  href="#learn-more"
                  className="text-xs text-muted-foreground underline underline-offset-4"
                >
                  Learn more about codespaces
                </a>
              </EmptyContent>
            </Empty>
            <Separator className="-mx-2 my-2 w-auto!" />
            <div className="p-1.5 text-xs text-muted-foreground">
              Codespace usage for this repository is paid for by{" "}
              <span className="font-medium">shadcn</span>.
            </div>
          </TabsContent>
          <TabsContent value="local">
            <Item size="sm" className="hidden p-0">
              <ItemContent>
                <ItemTitle className="gap-2">
                  <TerminalIcon className="size-4" />
                  Clone
                </ItemTitle>
              </ItemContent>
              <ItemActions>
                <Tooltip>
                  <TooltipTrigger
                    render={<Button variant="ghost" size="icon" />}
                  >
                    <InfoIcon />
                  </TooltipTrigger>
                  <TooltipContent side="left">
                    Which remote URL should I use?
                  </TooltipContent>
                </Tooltip>
              </ItemActions>
            </Item>
            <Tabs defaultValue="https">
              <TabsList
                variant="line"
                className="w-full justify-start border-b *:[button]:flex-0"
              >
                <TabsTrigger value="https">HTTPS</TabsTrigger>
                <TabsTrigger value="ssh">SSH</TabsTrigger>
                <TabsTrigger value="cli">GitHub CLI</TabsTrigger>
              </TabsList>
              <div className="rounded-md border bg-muted/30 p-2">
                <TabsContent value="https">
                  <Field className="gap-2">
                    <FieldLabel htmlFor="https-url" className="sr-only">
                      HTTPS URL
                    </FieldLabel>
                    <InputGroup>
                      <InputGroupAddon align="inline-end">
                        <InputGroupButton variant="ghost" size="icon-xs">
                          <CopyIcon />
                        </InputGroupButton>
                      </InputGroupAddon>
                      <InputGroupInput
                        id="https-url"
                        defaultValue="https://github.com/shadcn-ui/ui.git"
                        readOnly
                      />
                    </InputGroup>
                    <FieldDescription>
                      Clone using the web URL.
                    </FieldDescription>
                  </Field>
                </TabsContent>
                <TabsContent value="ssh">
                  <Field className="gap-2">
                    <FieldLabel htmlFor="ssh-url" className="sr-only">
                      SSH URL
                    </FieldLabel>
                    <InputGroup>
                      <InputGroupAddon align="inline-end">
                        <InputGroupButton variant="ghost" size="icon-xs">
                          <CopyIcon />
                        </InputGroupButton>
                      </InputGroupAddon>
                      <InputGroupInput
                        id="ssh-url"
                        defaultValue="git@github.com:shadcn-ui/ui.git"
                        readOnly
                      />
                    </InputGroup>
                    <FieldDescription>
                      Use a password-protected SSH key.
                    </FieldDescription>
                  </Field>
                </TabsContent>
                <TabsContent value="cli">
                  <Field className="gap-2">
                    <FieldLabel htmlFor="cli-command" className="sr-only">
                      CLI Command
                    </FieldLabel>
                    <InputGroup>
                      <InputGroupAddon align="inline-end">
                        <InputGroupButton variant="ghost" size="icon-xs">
                          <CopyIcon />
                        </InputGroupButton>
                      </InputGroupAddon>
                      <InputGroupInput
                        id="cli-command"
                        defaultValue="gh repo clone shadcn-ui/ui"
                        readOnly
                      />
                    </InputGroup>
                    <FieldDescription>
                      Work fast with our official CLI.{" "}
                      <a href="#learn-more">Learn more</a>
                    </FieldDescription>
                  </Field>
                </TabsContent>
              </div>
            </Tabs>
            <Separator className="-mx-2 my-2 w-auto!" />
            <div className="flex flex-col">
              <Button
                variant="ghost"
                size="sm"
                className="justify-start gap-1.5"
              >
                <MonitorIcon
                  data-icon="inline-start"
                />
                Open with GitHub Desktop
              </Button>
              <Button
                variant="ghost"
                size="sm"
                className="justify-start gap-1.5"
              >
                <DownloadIcon
                  data-icon="inline-start"
                />
                Download ZIP
              </Button>
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  )
}
