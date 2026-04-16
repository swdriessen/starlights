"use client"

import { SearchIcon } from "lucide-react"

import { Button } from "ui-framework/button"
import { Card, CardContent } from "ui-framework/card"
import {
    Empty,
    EmptyContent,
    EmptyDescription,
    EmptyHeader,
    EmptyTitle,
} from "ui-framework/empty"
import {
    InputGroup,
    InputGroupAddon,
    InputGroupInput,
} from "ui-framework/input-group"
import { Kbd } from "ui-framework/kbd"

export function NotFound() {
  return (
    <Card>
      <CardContent>
        <Empty className="h-72">
          <EmptyHeader>
            <EmptyTitle>404 - Not Found</EmptyTitle>
            <EmptyDescription>
              The page you&apos;re looking for doesn&apos;t exist. Try searching
              for what you need below.
            </EmptyDescription>
          </EmptyHeader>
          <EmptyContent>
            <InputGroup className="w-3/4">
              <InputGroupInput placeholder="Try searching for pages..." />
              <InputGroupAddon>
                <SearchIcon />
              </InputGroupAddon>
              <InputGroupAddon align="inline-end">
                <Kbd>/</Kbd>
              </InputGroupAddon>
            </InputGroup>
            <Button variant="link">Go to homepage</Button>
          </EmptyContent>
        </Empty>
      </CardContent>
    </Card>
  )
}
