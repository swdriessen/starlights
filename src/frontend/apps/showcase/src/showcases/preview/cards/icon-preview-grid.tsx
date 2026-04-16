"use client"

import {
    ArrowLeftIcon,
    ArrowRightIcon,
    CheckIcon,
    ChevronDownIcon,
    ChevronRightIcon,
    CircleAlertIcon,
    CopyIcon,
    Loader2Icon,
    MinusIcon,
    MoreHorizontalIcon,
    PlusIcon,
    SearchIcon,
    SettingsIcon,
    ShareIcon,
    ShoppingBagIcon,
    TrashIcon,
    type LucideIcon,
} from "lucide-react"

import { Card, CardContent } from "ui-framework/card"

const PREVIEW_ICONS: LucideIcon[] = [
  CopyIcon,
  CircleAlertIcon,
  TrashIcon,
  ShareIcon,
  ShoppingBagIcon,
  MoreHorizontalIcon,
  Loader2Icon,
  PlusIcon,
  MinusIcon,
  ArrowLeftIcon,
  ArrowRightIcon,
  CheckIcon,
  ChevronDownIcon,
  ChevronRightIcon,
  SearchIcon,
  SettingsIcon,
]

export function IconPreviewGrid() {
  return (
    <Card>
      <CardContent>
        <div className="grid grid-cols-8 place-items-center gap-4">
          {PREVIEW_ICONS.map((Icon, index) => (
            <div
              key={index}
              className="flex size-8 items-center justify-center rounded-md ring ring-border *:[svg]:size-4"
            >
              <Icon />
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}
