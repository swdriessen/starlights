"use client"

import {
  Avatar,
  AvatarFallback,
  AvatarGroup,
  AvatarImage,
} from "ui-framework/avatar"
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

export function NoTeamMembers() {
  return (
    <Card>
      <CardContent>
        <Empty className="h-56 border">
          <EmptyHeader>
            <EmptyMedia>
              <AvatarGroup className="grayscale">
                <Avatar size="lg">
                  <AvatarImage
                    src="https://github.com/shadcn.png"
                    alt="@shadcn"
                  />
                  <AvatarFallback>CN</AvatarFallback>
                </Avatar>
                <Avatar size="lg">
                  <AvatarImage
                    src="https://github.com/maxleiter.png"
                    alt="@maxleiter"
                  />
                  <AvatarFallback>LR</AvatarFallback>
                </Avatar>
                <Avatar size="lg">
                  <AvatarImage
                    src="https://github.com/evilrabbit.png"
                    alt="@evilrabbit"
                  />
                  <AvatarFallback>ER</AvatarFallback>
                </Avatar>
              </AvatarGroup>
            </EmptyMedia>
            <EmptyTitle>No Team Members</EmptyTitle>
            <EmptyDescription>
              Invite your team to collaborate on this project.
            </EmptyDescription>
          </EmptyHeader>
          <EmptyContent>
            <Button size="sm">Invite Members</Button>
          </EmptyContent>
        </Empty>
      </CardContent>
    </Card>
  )
}
