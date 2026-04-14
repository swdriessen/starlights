"use client"

import * as React from "react"

import { PlusIcon } from "lucide-react"

import { Avatar, AvatarFallback, AvatarImage } from "ui-framework/avatar"
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
  Combobox,
  ComboboxChip,
  ComboboxChips,
  ComboboxChipsInput,
  ComboboxContent,
  ComboboxEmpty,
  ComboboxItem,
  ComboboxList,
  ComboboxValue,
  useComboboxAnchor,
} from "ui-framework/combobox"
import { Tooltip, TooltipContent, TooltipTrigger } from "ui-framework/tooltip"

// Users available for assignment.
const users = [
  "shadcn",
  "maxleiter",
  "evilrabbit",
  "pranathip",
  "jorgezreik",
  "shuding",
  "rauchg",
]

export function AssignIssue() {
  const anchor = useComboboxAnchor()
  return (
    <Card className="w-full max-w-sm" size="sm">
      <CardHeader className="border-b">
        <CardTitle className="text-sm">Assign Issue</CardTitle>
        <CardDescription className="text-sm">
          Select users to assign to this issue.
        </CardDescription>
        <CardAction>
          <Tooltip>
            <TooltipTrigger
              render={<Button variant="outline" size="icon-xs" />}
            >
              <PlusIcon />
            </TooltipTrigger>
            <TooltipContent>Add user</TooltipContent>
          </Tooltip>
        </CardAction>
      </CardHeader>
      <CardContent>
        <Combobox
          multiple
          autoHighlight
          items={users}
          defaultValue={[users[0]]}
        >
          <ComboboxChips ref={anchor}>
            <ComboboxValue>
              {(values) => (
                <React.Fragment>
                  {values.map((username: string) => (
                    <ComboboxChip key={username}>
                      <Avatar className="size-4">
                        <AvatarImage
                          src={`https://github.com/${username}.png`}
                          alt={username}
                        />
                        <AvatarFallback>{username.charAt(0)}</AvatarFallback>
                      </Avatar>
                      {username}
                    </ComboboxChip>
                  ))}
                  <ComboboxChipsInput
                    placeholder={
                      values.length > 0 ? undefined : "Select a item..."
                    }
                  />
                </React.Fragment>
              )}
            </ComboboxValue>
          </ComboboxChips>
          <ComboboxContent anchor={anchor}>
            <ComboboxEmpty>No users found.</ComboboxEmpty>
            <ComboboxList>
              {(username: string) => (
                <ComboboxItem key={username} value={username}>
                  <Avatar className="size-5">
                    <AvatarImage
                      src={`https://github.com/${username}.png`}
                      alt={username}
                    />
                    <AvatarFallback>{username.charAt(0)}</AvatarFallback>
                  </Avatar>
                  {username}
                </ComboboxItem>
              )}
            </ComboboxList>
          </ComboboxContent>
        </Combobox>
      </CardContent>
    </Card>
  )
}
