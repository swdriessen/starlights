"use client"

import {
    CarIcon,
    CoffeeIcon,
    MoreHorizontalIcon,
    ShoppingCartIcon,
    TvIcon,
    WalletIcon,
} from "lucide-react"

import { Button } from "ui-framework/button"
import {
    Card,
    CardAction,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "ui-framework/card"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "ui-framework/dropdown-menu"
import { Table, TableBody, TableCell, TableRow } from "ui-framework/table"

export function RecentTransactions() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Recent Transactions</CardTitle>
        <CardDescription>Your latest account activity.</CardDescription>
        <CardAction>
          <Button variant="outline" size="sm">
            View All
          </Button>
        </CardAction>
      </CardHeader>
      <CardContent>
        <Table>
          <TableBody>
            <TableRow>
              <TableCell className="w-10">
                <div className="flex size-10 items-center justify-center rounded-lg bg-muted">
                  <CoffeeIcon
                    className="size-4 shrink-0"
                  />
                </div>
              </TableCell>
              <TableCell>
                <div className="flex flex-col">
                  <span className="font-medium">Blue Bottle Coffee</span>
                  <span className="text-sm text-muted-foreground">
                    Food & Drink
                  </span>
                </div>
              </TableCell>
              <TableCell className="text-sm text-muted-foreground">
                Today, 10:24 AM
              </TableCell>
              <TableCell className="text-right">
                <span className="text-sm font-semibold tabular-nums">
                  -$6.50
                </span>
              </TableCell>
              <TableCell className="w-8">
                <DropdownMenu>
                  <DropdownMenuTrigger
                    render={<Button variant="ghost" size="icon-sm" />}
                  >
                    <MoreHorizontalIcon />
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem>View details</DropdownMenuItem>
                    <DropdownMenuItem>Add note</DropdownMenuItem>
                    <DropdownMenuItem>Categorize</DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem>Dispute</DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell className="w-10">
                <div className="flex size-10 items-center justify-center rounded-lg bg-muted">
                  <ShoppingCartIcon className="size-4 shrink-0" />
                </div>
              </TableCell>
              <TableCell>
                <div className="flex flex-col">
                  <span className="font-medium">Whole Foods Market</span>
                  <span className="text-sm text-muted-foreground">
                    Groceries
                  </span>
                </div>
              </TableCell>
              <TableCell className="text-sm text-muted-foreground">
                Yesterday
              </TableCell>
              <TableCell className="text-right">
                <span className="text-sm font-semibold tabular-nums">
                  -$142.30
                </span>
              </TableCell>
              <TableCell className="w-8">
                <DropdownMenu>
                  <DropdownMenuTrigger
                    render={<Button variant="ghost" size="icon-sm" />}
                  >
                    <MoreHorizontalIcon />

                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem>View details</DropdownMenuItem>
                    <DropdownMenuItem>Add note</DropdownMenuItem>
                    <DropdownMenuItem>Categorize</DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem>Dispute</DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell className="w-10">
                <div className="flex size-10 items-center justify-center rounded-lg bg-muted">
                  <WalletIcon className="size-4 shrink-0" />

                </div>
              </TableCell>
              <TableCell>
                <div className="flex flex-col">
                  <span className="font-medium">Stripe Payout</span>
                  <span className="text-sm text-muted-foreground">Income</span>
                </div>
              </TableCell>
              <TableCell className="text-sm text-muted-foreground">
                Oct 12
              </TableCell>
              <TableCell className="text-right">
                <span className="text-sm font-semibold text-emerald-500 tabular-nums">
                  +$4,200.00
                </span>
              </TableCell>
              <TableCell className="w-8">
                <DropdownMenu>
                  <DropdownMenuTrigger
                    render={<Button variant="ghost" size="icon-sm" />}
                  >
                    <MoreHorizontalIcon />

                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem>View details</DropdownMenuItem>
                    <DropdownMenuItem>Add note</DropdownMenuItem>
                    <DropdownMenuItem>Categorize</DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem>Dispute</DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell className="w-10">
                <div className="flex size-10 items-center justify-center rounded-lg bg-muted">
                  <CarIcon className="size-4 shrink-0" />

                </div>
              </TableCell>
              <TableCell>
                <div className="flex flex-col">
                  <span className="font-medium">Uber Technologies</span>
                  <span className="text-sm text-muted-foreground">
                    Transport
                  </span>
                </div>
              </TableCell>
              <TableCell className="text-sm text-muted-foreground">
                Oct 11
              </TableCell>
              <TableCell className="text-right">
                <span className="text-sm font-semibold tabular-nums">
                  -$24.10
                </span>
              </TableCell>
              <TableCell className="w-8">
                <DropdownMenu>
                  <DropdownMenuTrigger
                    render={<Button variant="ghost" size="icon-sm" />}
                  >
                    <MoreHorizontalIcon />

                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem>View details</DropdownMenuItem>
                    <DropdownMenuItem>Add note</DropdownMenuItem>
                    <DropdownMenuItem>Categorize</DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem>Dispute</DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </TableCell>
            </TableRow>
            <TableRow>
              <TableCell className="w-10">
                <div className="flex size-10 items-center justify-center rounded-lg bg-muted">
                  <TvIcon className="size-4 shrink-0" />

                </div>
              </TableCell>
              <TableCell>
                <div className="flex flex-col">
                  <span className="font-medium">Netflix Subscription</span>
                  <span className="text-sm text-muted-foreground">
                    Entertainment
                  </span>
                </div>
              </TableCell>
              <TableCell className="text-sm text-muted-foreground">
                Oct 10
              </TableCell>
              <TableCell className="text-right">
                <span className="text-sm font-semibold tabular-nums">
                  -$19.99
                </span>
              </TableCell>
              <TableCell className="w-8">
                <DropdownMenu>
                  <DropdownMenuTrigger
                    render={<Button variant="ghost" size="icon-sm" />}
                  >
                    <MoreHorizontalIcon />

                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem>View details</DropdownMenuItem>
                    <DropdownMenuItem>Add note</DropdownMenuItem>
                    <DropdownMenuItem>Categorize</DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem>Dispute</DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </TableCell>
            </TableRow>
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  )
}
