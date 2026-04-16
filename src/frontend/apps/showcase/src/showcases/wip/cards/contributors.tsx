"use client"

import { Avatar, AvatarFallback, AvatarImage } from "ui-framework/avatar"
import { Badge } from "ui-framework/badge"
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "ui-framework/card"

const players = [
  "portrait-1.jpg",
  "portrait-2.jpg",
  "portrait-3.jpg",
  "portrait-4.jpg",
]

export function Contributors() {
  return (
    <Card className="max-w-sm">
      <CardHeader>
        <CardTitle>
          Players <Badge variant="secondary">{players.length}</Badge>
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div className="flex flex-wrap gap-2">
          {players.map((player) => (
            <Avatar key={player} className="grayscale">
              <AvatarImage src={`/portraits/${player}`} alt={player} />
              <AvatarFallback className="uppercase">
                {player.charAt(0)}
                {player.charAt(1)}
              </AvatarFallback>
            </Avatar>
          ))}
        </div>
      </CardContent>
      <CardFooter>
        <a href="#" className="text-sm underline underline-offset-4">
          + 81 waiting players
        </a>
      </CardFooter>
    </Card>
  )
}
