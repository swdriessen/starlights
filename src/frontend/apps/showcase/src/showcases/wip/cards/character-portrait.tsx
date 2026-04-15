import {
  BookOpenTextIcon,
  MoreHorizontalIcon,
  PencilIcon,
  Trash2Icon,
} from "lucide-react"
import { useState } from "react"
import { Separator } from "ui-framework"

import { Button } from "ui-framework/button"
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "ui-framework/card"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "ui-framework/dropdown-menu"

export function CharacterPortrait() {
  return (
    <Card>
      <CardContent className="flex flex-col gap-3">
        <img
          src="/images/compendium.jpeg"
          alt="Character Portrait"
          className="aspect-square size-40 w-full rounded-md object-cover"
        />
      </CardContent>
      <CardFooter className="flex-col gap-2">
        <div className="flex flex-col text-center">
          <CardTitle className="font-semibold tracking-widest uppercase">
            Artemis Entreri
          </CardTitle>
          <CardDescription className="text-center text-xs">
            Level 4
            <br />
            Assassin Rogue
          </CardDescription>
        </div>
        <Separator className="" />

        <div className="flex w-full justify-end gap-2">
          <Button variant="outline">Edit</Button>
          <Button variant="outline">Level Up</Button>
        </div>
      </CardFooter>
    </Card>
  )
}

export function CharacterPortraitStory() {
  return (
    <Card>
      <CardHeader className="gap-1">
        <CardTitle className="font-semibold tracking-widest uppercase">
          Artemis Entreri
        </CardTitle>
        <CardDescription>
          A feared blade-for-hire now bound to a fragile alliance in the
          Underdark.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
        <img
          src="/images/compendium.jpeg"
          alt="Character Portrait"
          className="aspect-square size-40 w-full rounded-md object-cover"
        />
      </CardContent>
      <CardFooter className="flex-col gap-3">
        <div className="flex w-full items-center justify-between text-xs text-muted-foreground">
          <span>Level 4 Assassin Rogue</span>
          <span>Neutral</span>
        </div>
        <div className="flex w-full gap-2">
          <Button variant="outline" className="flex-1">
            Character Sheet
          </Button>
          <Button className="flex-1">Continue Build</Button>
        </div>
      </CardFooter>
    </Card>
  )
}

function CharacterActionsMenu() {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger
        render={
          <Button
            variant="ghost"
            size="icon-sm"
            className="rounded-full border border-border/70 bg-background/10 backdrop-blur-sm hover:bg-background/75"
          />
        }
      >
        <MoreHorizontalIcon />
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="w-44">
        <DropdownMenuGroup>
          <DropdownMenuLabel>Character</DropdownMenuLabel>
          <DropdownMenuItem>
            <PencilIcon />
            Edit Character
          </DropdownMenuItem>
          <DropdownMenuItem>
            <BookOpenTextIcon />
            View Character Sheet
          </DropdownMenuItem>
        </DropdownMenuGroup>
        <DropdownMenuSeparator />
        <DropdownMenuGroup>
          <DropdownMenuItem variant="destructive">
            <Trash2Icon />
            Delete Character
          </DropdownMenuItem>
        </DropdownMenuGroup>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}

export function CharacterPortraitCampaignBrief() {
  return (
    <Card>
      <CardHeader className="gap-1">
        <CardAction>
          <CharacterActionsMenu />
        </CardAction>
        <CardTitle className="font-semibold tracking-widest uppercase">
          The Call of the Underdark
        </CardTitle>
        <CardDescription>
          The first echoes have reached the surface. Gather allies and descend
          before the breach expands.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
        <img
          src="/images/underdark.jpg"
          alt="Campaign scene"
          className="aspect-square size-40 w-full rounded-md object-cover"
        />
      </CardContent>
      <CardFooter className="flex w-full gap-2">
        <Button variant="outline" className="flex-1">
          Session Notes
        </Button>
        <Button className="flex-1">Continue</Button>
      </CardFooter>
    </Card>
  )
}

export function CharacterPortraitCampaignRoster() {
  return (
    <Card>
      <CardHeader className="gap-1">
        <CardAction>
          <CharacterActionsMenu />
        </CardAction>
        <CardTitle className="font-semibold tracking-widest uppercase">
          Shadowbound Company
        </CardTitle>
        <CardDescription>
          Core party assembled for the descent. One slot remains open for a
          scout or support caster.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
        <img
          src="/images/spiritdragon_olivierbernard_full.jpg"
          alt="Campaign party portrait"
          className="aspect-square size-40 w-full rounded-md object-cover"
        />
        <Separator />
        <div className="flex w-full items-center justify-between text-sm">
          <span className="text-muted-foreground">Party Status</span>
          <span className="font-medium">3 / 4 Ready</span>
        </div>
      </CardContent>
      <CardFooter className="flex w-full gap-2">
        <Button variant="outline" className="flex-1">
          Invite
        </Button>
        <Button className="flex-1">Open Roster</Button>
      </CardFooter>
    </Card>
  )
}

export function CharacterPortraitCampaignQuestLog() {
  return (
    <Card>
      <CardHeader className="gap-1">
        <CardAction>
          <CharacterActionsMenu />
        </CardAction>
        <CardTitle className="font-semibold tracking-widest uppercase">
          Active Quest Log
        </CardTitle>
        <CardDescription>
          Three objectives are active. The party must secure the relic before
          the rival expedition reaches the vault.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
        <img
          src="/images/underdark.jpg"
          alt="Campaign quest scene"
          className="aspect-square size-40 w-full rounded-md object-cover"
        />
        <Separator />
        <div className="flex w-full items-center justify-between text-sm">
          <span className="text-muted-foreground">Completed Objectives</span>
          <span className="font-medium">4 / 7</span>
        </div>
      </CardContent>
      <CardFooter className="flex w-full gap-2">
        <Button variant="outline" className="flex-1">
          View Notes
        </Button>
        <Button className="flex-1">Track Quest</Button>
      </CardFooter>
    </Card>
  )
}

export function CharacterPortraitVariantDuelist() {
  return (
    <Card>
      <CardHeader className="gap-1">
        <CardAction>
          <CharacterActionsMenu />
        </CardAction>
        <CardTitle className="font-semibold tracking-widest uppercase">
          Artemis Entreri
        </CardTitle>
        <CardDescription>Level 7 Assassin Rogue</CardDescription>
      </CardHeader>
      <CardContent>
        <a href="#character-artemis" className="group block">
          <img
            src="/images/compendium.jpeg"
            alt="Artemis Entreri portrait"
            className="aspect-square w-full rounded-md object-cover"
          />
        </a>
      </CardContent>
      <CardFooter className="flex w-full gap-2">
        <Button variant="outline" className="flex-1">
          Edit
        </Button>
        <Button className="flex-1">Character Sheet</Button>
      </CardFooter>
    </Card>
  )
}

export function CharacterPortraitVariantMage() {
  return (
    <Card>
      <CardContent className="pt-6">
        <div className="flex items-start gap-3">
          <a href="#character-catti-brie" className="block w-24 shrink-0">
            <img
              src="/images/underdark.jpg"
              alt="Catti-brie portrait"
              className="aspect-square w-full rounded-md object-cover"
            />
          </a>
          <div className="min-w-0 flex-1">
            <div className="flex items-start justify-between gap-2">
              <a href="#character-catti-brie" className="group min-w-0">
                <CardTitle className="truncate font-semibold tracking-widest uppercase transition-opacity group-hover:opacity-80">
                  Catti-brie
                </CardTitle>
                <CardDescription>Level 8 Arcane Archer Fighter</CardDescription>
              </a>
              <CharacterActionsMenu />
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

export function CharacterPortraitVariantWarden() {
  return (
    <Card>
      <CardHeader className="gap-1">
        <CardAction>
          <CharacterActionsMenu />
        </CardAction>
        <CardDescription>Level 9 Oath of Ancients Paladin</CardDescription>
        <CardTitle className="font-semibold tracking-widest uppercase">
          Bruenor Battlehammer
        </CardTitle>
      </CardHeader>
      <CardContent>
        <a href="#character-bruenor" className="group block">
          <img
            src="/images/spiritdragon_olivierbernard_full.jpg"
            alt="Bruenor Battlehammer portrait"
            className="aspect-square w-full rounded-md object-cover"
          />
        </a>
      </CardContent>
      <CardFooter className="flex w-full justify-end gap-2">
        <Button variant="outline">Edit</Button>
        <Button>Character Sheet</Button>
      </CardFooter>
    </Card>
  )
}

export function CharacterPortraitVariantScout() {
  return (
    <Card>
      <CardContent className="pt-6">
        <div className="flex items-start gap-3">
          <a href="#character-jarlaxle" className="block w-24 shrink-0">
            <img
              src="/images/compendium.jpeg"
              alt="Jarlaxle portrait"
              className="aspect-square w-full rounded-md object-cover"
            />
          </a>
          <div className="min-w-0 flex-1">
            <div className="flex items-start justify-between gap-2">
              <a href="#character-jarlaxle" className="group min-w-0">
                <CardTitle className="truncate font-semibold tracking-widest uppercase transition-opacity group-hover:opacity-80">
                  Jarlaxle
                </CardTitle>
                <CardDescription>Level 10 Swashbuckler Rogue</CardDescription>
              </a>
              <CharacterActionsMenu />
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}

export function CharacterPortraitVariantImageOnlyRogue() {
  return (
    <div className="w-60 max-w-full rounded-[14px] bg-linear-to-b from-border via-border/80 to-border/60 p-0.5 shadow-md">
      <div className="group relative overflow-hidden rounded-xl border border-border bg-card ring-2 ring-border/60 transition-shadow hover:shadow-md">
        <a href="#character-artemis-image-only" className="block">
          <img
            src="/portraits/portrait-3.jpg"
            alt="Artemis Entreri portrait"
            className="aspect-square w-full object-cover grayscale-40 transition duration-500 group-hover:scale-110 group-hover:grayscale-0"
          />
        </a>
        <div className="absolute top-2 right-2 overflow-hidden rounded-full border border-border bg-background/90 shadow-sm backdrop-blur-sm">
          <CharacterActionsMenu />
        </div>
        <a
          href="#character-artemis-image-only"
          className="absolute inset-x-0 bottom-0 block bg-linear-to-t from-black/75 to-transparent px-3 py-3 text-white"
        >
          <p className="truncate text-sm font-semibold tracking-widest uppercase">
            Artemis Entreri
          </p>
          <p className="text-xs text-white/90">Level 7 Assassin Rogue</p>
        </a>
      </div>
    </div>
  )
}

export function CharacterPortraitVariantImageOnlyDragon() {
  return (
    <div className="w-40 max-w-full rounded-[14px] bg-linear-to-b from-border via-border/80 to-border/60 p-0.5 shadow-md">
      <div className="group relative overflow-hidden rounded-xl border border-border bg-card ring-2 ring-border/60 transition-shadow hover:shadow-md">
        <a href="#character-dragon-image-only" className="block">
          <img
            src="/portraits/portrait-4.jpg"
            alt="Spirit dragon portrait"
            className="aspect-square w-full object-cover grayscale-40 transition duration-500 group-hover:scale-110 group-hover:grayscale-0"
          />
        </a>
        <div className="absolute top-2 right-2 overflow-hidden rounded-full border border-border bg-background/90 shadow-sm backdrop-blur-sm">
          <CharacterActionsMenu />
        </div>
        <a
          href="#character-dragon-image-only"
          className="absolute inset-x-0 bottom-0 block bg-linear-to-t from-black/75 to-transparent px-3 py-3 text-white"
        >
          <p className="truncate text-sm font-semibold tracking-widest uppercase">
            Icingdeath
          </p>
          <p className="text-xs text-white/90">Level 11 Draconic Sorcerer</p>
        </a>
      </div>
    </div>
  )
}

export function CharacterPortraitVariantImageOnlyRanger() {
  return (
    <div className="w-52 max-w-full rounded-[14px] bg-linear-to-b from-border via-border/80 to-border/60 p-0.5 shadow-md">
      <div className="group relative overflow-hidden rounded-xl border border-border bg-card ring-2 ring-border/60 transition-shadow hover:shadow-md">
        <a href="#character-drizzt-image-only" className="block">
          <img
            src="/portraits/portrait-2.jpg"
            alt="Drizzt portrait"
            className="aspect-square w-full object-cover grayscale-40 transition duration-500 group-hover:scale-110 group-hover:grayscale-0"
          />
        </a>
        <div className="absolute top-2 right-2 overflow-hidden rounded-full border border-border bg-background/90 shadow-sm backdrop-blur-sm">
          <CharacterActionsMenu />
        </div>
        <a
          href="#character-drizzt-image-only"
          className="absolute inset-x-0 bottom-0 block bg-linear-to-t from-black/75 to-transparent px-3 py-3 text-white"
        >
          <p className="truncate text-sm font-semibold tracking-widest uppercase">
            Drizzt Do'Urden
          </p>
          <p className="text-xs text-white/90">Level 9 Gloom Stalker Ranger</p>
        </a>
      </div>
    </div>
  )
}

export function CharacterPortraitVariantImageOnlyRanger2() {
  const [isSelected, setIsSelected] = useState(false)

  return (
    <div className="w-52 max-w-full rounded-[14px] bg-linear-to-b from-border via-border/80 to-border/60 p-0.5 shadow-md">
      <div
        className={`group relative overflow-hidden rounded-xl border border-border bg-card ring-2 ring-border/60 transition-shadow hover:shadow-md ${isSelected ? "outline-2 outline-offset-2 outline-amber-400/90" : "outline-none"}`}
        role="button"
        tabIndex={0}
        aria-pressed={isSelected}
        onClick={() => setIsSelected((value) => !value)}
        onKeyDown={(event) => {
          if (event.key === "Enter" || event.key === " ") {
            event.preventDefault()
            setIsSelected((value) => !value)
          }
        }}
      >
        <div className="block">
          <img
            src="/portraits/portrait-2.jpg"
            alt="Drizzt portrait"
            className="aspect-square w-full object-cover grayscale-40 transition duration-500 group-hover:scale-110 group-hover:grayscale-0"
          />
        </div>
        <div className="absolute inset-x-0 bottom-0 block bg-linear-to-t from-black/75 to-transparent px-3 py-3 text-white">
          <div
            className={`transition-transform duration-300 ${isSelected ? "-translate-y-2" : "translate-y-0"}`}
          >
            <p className="truncate text-sm font-semibold tracking-widest uppercase">
              Drizzt Do'Urden
            </p>
            <p className="text-xs text-white/90">
              Level 9 Gloom Stalker Ranger
            </p>
          </div>
          <div
            className={`mt-0 flex max-h-0 gap-2 overflow-hidden opacity-0 transition-all duration-300 ${isSelected ? "group-hover:mt-0" : "group-hover:mt-2"} group-hover:max-h-10 group-hover:opacity-100`}
          >
            <Button
              size="sm"
              variant="outline"
              className="h-8 flex-1 border-zinc-600 bg-zinc-900/90 text-zinc-100 hover:bg-zinc-800/95 hover:text-zinc-50"
              onClick={(event) => event.stopPropagation()}
            >
              View
            </Button>
            <Button
              size="sm"
              variant="destructive"
              className="h-8 flex-1 border-red-500 bg-red-600/95 text-white hover:bg-red-500"
              onClick={(event) => event.stopPropagation()}
            >
              Delete
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}
export function CharacterPortraitVariantImageOnlyCleric() {
  return (
    <div className="w-48 max-w-full rounded-[14px] bg-linear-to-b from-border via-border/80 to-border/60 p-0.5 shadow-md">
      <div className="group relative overflow-hidden rounded-xl border border-border bg-card ring-2 ring-border/60 transition-shadow hover:shadow-md">
        <a href="#character-cadence-image-only" className="block">
          <img
            src="/portraits/portrait-1.jpg"
            alt="Cadence portrait"
            className="aspect-square w-full object-cover grayscale-40 transition duration-500 group-hover:scale-110 group-hover:grayscale-0"
          />
        </a>
        <div className="absolute top-2 right-2 overflow-hidden rounded-full border border-border bg-background/90 shadow-sm backdrop-blur-sm">
          <CharacterActionsMenu />
        </div>
        <a
          href="#character-cadence-image-only"
          className="absolute inset-x-0 bottom-0 block bg-linear-to-t from-black/75 to-transparent px-3 py-3 text-white"
        >
          <p className="truncate text-sm font-semibold tracking-widest uppercase">
            Cadence Vell
          </p>
          <p className="text-xs text-white/90">Level 8 Light Domain Cleric</p>
        </a>
      </div>
    </div>
  )
}
