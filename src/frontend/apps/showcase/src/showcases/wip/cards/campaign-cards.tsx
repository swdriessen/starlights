import { Separator } from "ui-framework"

import { Button } from "ui-framework/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "ui-framework/card"

// export function CharacterPortraitStory() {
//   return (
//     <Card>
//       <CardHeader className="gap-1">
//         <CardTitle className="font-semibold tracking-widest uppercase">
//           Artemis Entreri
//         </CardTitle>
//         <CardDescription>
//           A feared blade-for-hire now bound to a fragile alliance in the
//           Underdark.
//         </CardDescription>
//       </CardHeader>
//       <CardContent className="flex flex-col gap-3">
//         <img
//           src="/images/compendium.jpeg"
//           alt="Character Portrait"
//           className="aspect-square size-40 w-full rounded-md object-cover"
//         />
//       </CardContent>
//       <CardFooter className="flex-col gap-3">
//         <div className="flex w-full items-center justify-between text-xs text-muted-foreground">
//           <span>Level 4 Assassin Rogue</span>
//           <span>Neutral</span>
//         </div>
//         <div className="flex w-full gap-2">
//           <Button variant="outline" className="flex-1">
//             Character Sheet
//           </Button>
//           <Button className="flex-1">Continue Build</Button>
//         </div>
//       </CardFooter>
//     </Card>
//   )
// }

export function CampaignBrief() {
  return (
    <Card>
      <CardHeader className="gap-1">
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

export function CampaignRoster() {
  return (
    <Card className="">
      <CardHeader className="gap-1">
        <CardTitle className="font-semibold tracking-widest uppercase">
          Underdark Excursion
        </CardTitle>
        <CardDescription>
          The party descends into the depths, seeking the source of the
          disturbance and forging uneasy alliances with the denizens of the
          Underdark.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
        <img
          src="/images/underdark.jpg"
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

export function CampaignQuestLog() {
  return (
    <Card>
      <CardHeader className="gap-1">
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

export function CampaignBriefTopImage() {
  return (
    <Card>
      <img
        src="/images/underdark.jpg"
        alt="Campaign scene"
        className="aspect-video w-full rounded-t-xl object-cover"
      />
      <CardHeader className="gap-1">
        <CardTitle className="font-semibold tracking-widest uppercase">
          The Call of the Underdark
        </CardTitle>
        <CardDescription>
          The first echoes have reached the surface. Gather allies and descend
          before the breach expands.
        </CardDescription>
      </CardHeader>
      <CardFooter className="flex w-full gap-2">
        <Button variant="outline" className="flex-1">
          Session Notes
        </Button>
        <Button className="flex-1">Continue</Button>
      </CardFooter>
    </Card>
  )
}

export function CampaignRosterTopImage() {
  return (
    <Card>
      <img
        src="/images/underdark.jpg"
        alt="Campaign party portrait"
        className="aspect-video w-full rounded-t-xl object-cover"
      />
      <CardHeader className="gap-1">
        <CardTitle className="font-semibold tracking-widest uppercase">
          Underdark Excursion
        </CardTitle>
        <CardDescription>
          The party descends into the depths, seeking the source of the
          disturbance and forging uneasy alliances with the denizens of the
          Underdark.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
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

export function CampaignQuestLogTopImage() {
  return (
    <Card>
      <img
        src="/images/underdark.jpg"
        alt="Campaign quest scene"
        className="aspect-video w-full rounded-t-xl object-cover"
      />
      <CardHeader className="gap-1">
        <CardTitle className="font-semibold tracking-widest uppercase">
          Active Quest Log
        </CardTitle>
        <CardDescription>
          Three objectives are active. The party must secure the relic before
          the rival expedition reaches the vault.
        </CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-3">
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
