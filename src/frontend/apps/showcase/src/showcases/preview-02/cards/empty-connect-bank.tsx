import { CreditCardIcon } from "lucide-react"

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

export function EmptyConnectBank() {
  return (
    <Card>
      <CardContent>
        <Empty className="p-4">
          <EmptyMedia variant="icon">
            <CreditCardIcon />
          </EmptyMedia>
          <EmptyHeader>
            <EmptyTitle>Connect Bank</EmptyTitle>
            <EmptyDescription>
              Link your payout method to receive monthly royalty distributions
              automatically.
            </EmptyDescription>
          </EmptyHeader>
          <EmptyContent>
            <Button>Set Up Payouts</Button>
          </EmptyContent>
        </Empty>
      </CardContent>
    </Card>
  )
}
