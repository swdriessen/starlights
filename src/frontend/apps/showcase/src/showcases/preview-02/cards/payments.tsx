"use client"

import {
    CalendarIcon,
    ChevronRightIcon,
    GaugeIcon,
    MoreHorizontalIcon,
    RefreshCwIcon,
    RepeatIcon,
} from "lucide-react"

import {
    Breadcrumb,
    BreadcrumbItem,
    BreadcrumbLink,
    BreadcrumbList,
    BreadcrumbPage,
    BreadcrumbSeparator,
} from "ui-framework/breadcrumb"
import { Button } from "ui-framework/button"
import { Card, CardContent, CardHeader } from "ui-framework/card"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuGroup,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from "ui-framework/dropdown-menu"
import {
    Item,
    ItemContent,
    ItemDescription,
    ItemGroup,
    ItemMedia,
    ItemTitle,
} from "ui-framework/item"

export function Payments() {
  return (
    <Card>
      <CardHeader className="flex flex-col gap-3">
        <Breadcrumb>
          <BreadcrumbList>
            <BreadcrumbItem>
              <BreadcrumbLink href="#">Home</BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <DropdownMenu>
                <DropdownMenuTrigger
                  render={<Button size="icon-sm" variant="ghost" />}
                >
                  <MoreHorizontalIcon />
                  <span className="sr-only">Account options</span>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="start">
                  <DropdownMenuGroup>
                    <DropdownMenuItem>Profile</DropdownMenuItem>
                    <DropdownMenuItem>Statements</DropdownMenuItem>
                    <DropdownMenuItem>Documents</DropdownMenuItem>
                  </DropdownMenuGroup>
                </DropdownMenuContent>
              </DropdownMenu>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <BreadcrumbPage>Payments</BreadcrumbPage>
            </BreadcrumbItem>
          </BreadcrumbList>
        </Breadcrumb>
      </CardHeader>
      <CardContent>
        <ItemGroup>
          <Item variant="muted" render={<a href="#" />}>
            <ItemMedia variant="icon">
              <GaugeIcon />
            </ItemMedia>
            <ItemContent>
              <ItemTitle>Change transfer limit</ItemTitle>
              <ItemDescription>
                Adjust how much you can send from your balance.
              </ItemDescription>
            </ItemContent>
            <ChevronRightIcon
              className="size-4 shrink-0 text-muted-foreground"
            />
          </Item>
          <Item variant="muted" render={<a href="#" />}>
            <ItemMedia variant="icon">
              <CalendarIcon />
            </ItemMedia>
            <ItemContent>
              <ItemTitle>Scheduled transfers</ItemTitle>
              <ItemDescription>
                Set up a transfer to send at a later date.
              </ItemDescription>
            </ItemContent>
            <ChevronRightIcon
              className="size-4 shrink-0 text-muted-foreground"
            />
          </Item>
          <Item variant="muted" render={<a href="#" />}>
            <ItemMedia variant="icon">
              <RepeatIcon />
            </ItemMedia>
            <ItemContent>
              <ItemTitle>Direct Debits</ItemTitle>
              <ItemDescription>
                Set up and manage regular payments.
              </ItemDescription>
            </ItemContent>
            <ChevronRightIcon
              className="size-4 shrink-0 text-muted-foreground"
            />
          </Item>
          <Item variant="muted" render={<a href="#" />}>
            <ItemMedia variant="icon">
              <RefreshCwIcon />
            </ItemMedia>
            <ItemContent>
              <ItemTitle>Recurring card payments</ItemTitle>
              <ItemDescription>
                Manage your repeated card transactions.
              </ItemDescription>
            </ItemContent>
            <ChevronRightIcon
              className="size-4 shrink-0 text-muted-foreground"
            />
          </Item>
        </ItemGroup>
      </CardContent>
    </Card>
  )
}
