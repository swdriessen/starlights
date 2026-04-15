"use client"

import { PlusIcon } from "lucide-react"

import { Badge } from "ui-framework/badge"
import { Button } from "ui-framework/button"
import {
  Card,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "ui-framework/card"

export function ObservabilityCard() {
  return (
    <Card className="relative w-full max-w-md overflow-hidden pt-0">
      <div className="absolute inset-0 z-30 aspect-video bg-primary opacity-50 mix-blend-color" />
      <img
        src="/images/wip_card.jpg"
        alt="Photo by author"
        title="Photo by author"
        className="relative z-20 aspect-video w-full object-cover brightness-60 grayscale"
      />
      <CardHeader>
        <CardTitle>The Call of the Underdark</CardTitle>
        <CardDescription>
          A forgotten signal rises from beneath the world. Rally your party,
          brave the lightless halls, and uncover the power stirring in the
          depths before it reaches the surface.
        </CardDescription>
      </CardHeader>
      <CardFooter>
        <Button>
          <PlusIcon data-icon="inline-end" /> Add Player
        </Button>
        <Badge variant="secondary" className="ml-auto">
          Forgotten Realms
        </Badge>
      </CardFooter>
    </Card>
  )
}
