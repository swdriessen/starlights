"use client"

import {
    ActivityIcon,
    ArrowLeftRightIcon,
    BellIcon,
    CircleHelpIcon,
    CreditCardIcon,
    FileBarChartIcon,
    FileTextIcon,
    LayoutDashboardIcon,
    MessageSquareIcon,
    ShieldIcon,
    TargetIcon,
    TrendingUpIcon,
    UserIcon,
    WalletIcon,
} from "lucide-react"

import { Card } from "ui-framework/card"
import {
    Sidebar,
    SidebarContent,
    SidebarGroup,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
    SidebarProvider,
    SidebarSeparator,
} from "ui-framework/sidebar"

export function SidebarNav() {
  return (
    <div className="grid grid-cols-2 items-start gap-6">
      <Card className="overflow-hidden py-0">
        <SidebarProvider className="min-h-0">
          <Sidebar collapsible="none" className="w-full bg-transparent">
            <SidebarContent className="gap-0">
              <SidebarGroup className="pb-1">
                <SidebarGroupLabel>Overview</SidebarGroupLabel>
                <SidebarGroupContent>
                  <SidebarMenu>
                    <SidebarMenuItem>
                      <SidebarMenuButton isActive>
                        <LayoutDashboardIcon />
                        Dashboard
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <ArrowLeftRightIcon />
                        Transactions
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <TrendingUpIcon />
                        Investments
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  </SidebarMenu>
                </SidebarGroupContent>
              </SidebarGroup>
              <SidebarSeparator className="w-auto!" />
              <SidebarGroup className="pt-1">
                <SidebarGroupLabel>Planning</SidebarGroupLabel>
                <SidebarGroupContent>
                  <SidebarMenu>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <TargetIcon />
                        Goals
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <WalletIcon />
                        Budget
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <FileBarChartIcon />
                        Reports
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <FileTextIcon />
                        Documents
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  </SidebarMenu>
                </SidebarGroupContent>
              </SidebarGroup>
            </SidebarContent>
          </Sidebar>
        </SidebarProvider>
      </Card>
      <Card className="overflow-hidden py-0">
        <SidebarProvider className="min-h-0">
          <Sidebar collapsible="none" className="w-full bg-transparent">
            <SidebarContent className="gap-0">
              <SidebarGroup className="pb-1">
                <SidebarGroupLabel>Account</SidebarGroupLabel>
                <SidebarGroupContent>
                  <SidebarMenu>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <UserIcon />
                        Profile
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton isActive>
                        <CreditCardIcon />
                        Billing
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <BellIcon />
                        Notifications
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <ShieldIcon />
                        Security
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  </SidebarMenu>
                </SidebarGroupContent>
              </SidebarGroup>
              <SidebarSeparator className="w-auto!" />
              <SidebarGroup className="pt-1">
                <SidebarGroupLabel>Support</SidebarGroupLabel>
                <SidebarGroupContent>
                  <SidebarMenu>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <CircleHelpIcon />
                        Help Center
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <MessageSquareIcon />
                        Contact Us
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                    <SidebarMenuItem>
                      <SidebarMenuButton>
                        <ActivityIcon />
                        Status
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  </SidebarMenu>
                </SidebarGroupContent>
              </SidebarGroup>
            </SidebarContent>
          </Sidebar>
        </SidebarProvider>
      </Card>
    </div>
  )
}
