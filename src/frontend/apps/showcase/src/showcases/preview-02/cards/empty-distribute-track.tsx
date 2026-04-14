import { PlusIcon } from "lucide-react"

import { Button } from "ui-framework/button"
import { Card, CardContent } from "ui-framework/card"
import {
    Empty,
    EmptyContent,
    EmptyDescription,
    EmptyHeader,
    EmptyMedia,
    EmptyTitle,
} from "ui-framework/empty"

export function EmptyDistributeTrack() {
  return (
    <Card>
      <CardContent>
        <Empty className="p-4">
          <EmptyMedia variant="icon">
            <PlusIcon />
          </EmptyMedia>
          <EmptyHeader>
            <EmptyTitle>Distribute Track</EmptyTitle>
            <EmptyDescription>
              Upload your first master to start reaching listeners on Spotify,
              Apple Music, and more.
            </EmptyDescription>
          </EmptyHeader>
          <EmptyContent>
            <Button>Create Release</Button>
          </EmptyContent>
        </Empty>
      </CardContent>
    </Card>
  )
}
