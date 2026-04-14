"use client"

import { CopyIcon, PlusIcon } from "lucide-react"

import { Button } from "ui-framework/button"
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "ui-framework/card"
import { Field, FieldLabel } from "ui-framework/field"
import { Input } from "ui-framework/input"
import {
    InputGroup,
    InputGroupAddon,
    InputGroupButton,
    InputGroupInput,
} from "ui-framework/input-group"
import {
    Select,
    SelectContent,
    SelectGroup,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "ui-framework/select"
import { Separator } from "ui-framework/separator"

export function InviteTeam() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Invite Team</CardTitle>
        <CardDescription>Add members to your workspace</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-4">
        <div className="flex flex-col gap-3">
          {[
            { email: "alex@example.com", role: "Editor" },
            { email: "sam@example.com", role: "Viewer" },
          ].map((invite) => (
            <div key={invite.email} className="flex items-center gap-2">
              <Input defaultValue={invite.email} className="flex-1" />
              <Select
                items={[
                  { label: "Admin", value: "admin" },
                  { label: "Editor", value: "editor" },
                  { label: "Viewer", value: "viewer" },
                ]}
                defaultValue={invite.role.toLowerCase()}
              >
                <SelectTrigger className="w-24">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent alignItemWithTrigger={false} align="end">
                  <SelectGroup>
                    <SelectItem value="admin">Admin</SelectItem>
                    <SelectItem value="editor">Editor</SelectItem>
                    <SelectItem value="viewer">Viewer</SelectItem>
                  </SelectGroup>
                </SelectContent>
              </Select>
            </div>
          ))}
        </div>
        <Button variant="outline">
          <PlusIcon
            data-icon="inline-start"
          />
          Add another
        </Button>
        <Separator />
        <Field>
          <FieldLabel htmlFor="invite-link">Or share invite link</FieldLabel>
          <InputGroup>
            <InputGroupInput
              id="invite-link"
              defaultValue="https://app.co/invite/x8f2k"
              readOnly
            />
            <InputGroupAddon align="inline-end">
              <InputGroupButton size="icon-xs" aria-label="Copy link">
                <CopyIcon />
              </InputGroupButton>
            </InputGroupAddon>
          </InputGroup>
        </Field>
      </CardContent>
      <CardFooter>
        <Button className="w-full">Send Invites</Button>
      </CardFooter>
    </Card>
  )
}
