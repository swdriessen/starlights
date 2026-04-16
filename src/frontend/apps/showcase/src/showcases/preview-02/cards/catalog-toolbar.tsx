"use client"

import { PlusIcon, SearchIcon } from "lucide-react"

import { Button } from "ui-framework/button"
import {
    InputGroup,
    InputGroupAddon,
    InputGroupInput,
} from "ui-framework/input-group"
import { ToggleGroup, ToggleGroupItem } from "ui-framework/toggle-group"

export function CatalogToolbar() {
  return (
    <div className="flex items-center gap-3">
      <InputGroup className="flex-1">
        <InputGroupAddon>
          <SearchIcon />
        </InputGroupAddon>
        <InputGroupInput placeholder="Search releases or catalog..." />
      </InputGroup>
      <Button>
        <PlusIcon />
        Upload New Release
      </Button>
      <ToggleGroup defaultValue={["releases"]} variant="outline">
        <ToggleGroupItem value="all-tracks">All Tracks</ToggleGroupItem>
        <ToggleGroupItem value="releases">Releases</ToggleGroupItem>
        <ToggleGroupItem value="top-earners">Top Earners</ToggleGroupItem>
      </ToggleGroup>
    </div>
  )
}
