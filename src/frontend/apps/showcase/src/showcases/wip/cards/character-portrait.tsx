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

// export function CharacterPortraitVariantDuelist() {
//   return (
//     <Card>
//       <CardHeader className="gap-1">
//         <CardAction>
//           <CharacterActionsMenu />
//         </CardAction>
//         <CardTitle className="font-semibold tracking-widest uppercase">
//           Artemis Entreri
//         </CardTitle>
//         <CardDescription>Level 7 Assassin Rogue</CardDescription>
//       </CardHeader>
//       <CardContent>
//         <a href="#character-artemis" className="group block">
//           <img
//             src="/images/compendium.jpeg"
//             alt="Artemis Entreri portrait"
//             className="aspect-square w-full rounded-md object-cover"
//           />
//         </a>
//       </CardContent>
//       <CardFooter className="flex w-full gap-2">
//         <Button variant="outline" className="flex-1">
//           Edit
//         </Button>
//         <Button className="flex-1">Character Sheet</Button>
//       </CardFooter>
//     </Card>
//   )
// }

// export function CharacterPortraitVariantMage() {
//   return (
//     <Card>
//       <CardContent className="pt-6">
//         <div className="flex items-start gap-3">
//           <a href="#character-catti-brie" className="block w-24 shrink-0">
//             <img
//               src="/images/underdark.jpg"
//               alt="Catti-brie portrait"
//               className="aspect-square w-full rounded-md object-cover"
//             />
//           </a>
//           <div className="min-w-0 flex-1">
//             <div className="flex items-start justify-between gap-2">
//               <a href="#character-catti-brie" className="group min-w-0">
//                 <CardTitle className="truncate font-semibold tracking-widest uppercase transition-opacity group-hover:opacity-80">
//                   Catti-brie
//                 </CardTitle>
//                 <CardDescription>Level 8 Arcane Archer Fighter</CardDescription>
//               </a>
//               <CharacterActionsMenu />
//             </div>
//           </div>
//         </div>
//       </CardContent>
//     </Card>
//   )
// }
