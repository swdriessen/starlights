"use client"

import * as React from "react"

import { XIcon } from "lucide-react"

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
    Field,
    FieldDescription,
    FieldGroup,
    FieldLabel,
} from "ui-framework/field"
import {
    Select,
    SelectContent,
    SelectGroup,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "ui-framework/select"
import { Slider } from "ui-framework/slider"
import { Textarea } from "ui-framework/textarea"

const CURRENCIES = [
  { label: "USD — United States Dollar", value: "usd" },
  { label: "EUR — Euro", value: "eur" },
  { label: "GBP — British Pound", value: "gbp" },
  { label: "JPY — Japanese Yen", value: "jpy" },
]

export function PayoutThreshold() {
  const [amount, setAmount] = React.useState([2500])

  return (
    <Card>
      <CardHeader>
        <CardTitle>Payout Threshold</CardTitle>
        <CardDescription>
          Set the minimum balance required before a payout is triggered.
        </CardDescription>
        <CardAction>
          <Button variant="ghost" size="icon-sm" className="bg-muted">
            <XIcon />
          </Button>
        </CardAction>
      </CardHeader>
      <CardContent>
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="preferred-currency">
              Preferred Currency
            </FieldLabel>
            <Select items={CURRENCIES} defaultValue="usd">
              <SelectTrigger id="preferred-currency" className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  {CURRENCIES.map((item) => (
                    <SelectItem key={item.value} value={item.value}>
                      {item.label}
                    </SelectItem>
                  ))}
                </SelectGroup>
              </SelectContent>
            </Select>
          </Field>
          <Field>
            <div className="flex items-baseline justify-between">
              <FieldLabel htmlFor="min-payout">
                Minimum Payout Amount
              </FieldLabel>
              <span className="text-2xl font-semibold tabular-nums">
                ${amount[0].toFixed(2)}
              </span>
            </div>
            <Slider
              id="min-payout"
              value={amount}
              onValueChange={(value) =>
                setAmount(Array.isArray(value) ? [...value] : [value])
              }
              min={50}
              max={10000}
              step={50}
            />
            <div className="flex items-center justify-between">
              <FieldDescription>$50 (MIN)</FieldDescription>
              <FieldDescription>$10,000 (MAX)</FieldDescription>
            </div>
          </Field>
          <Field>
            <FieldLabel htmlFor="payout-notes">Notes</FieldLabel>
            <Textarea
              id="payout-notes"
              placeholder="Add any notes for this payout configuration..."
              className="min-h-[100px]"
            />
          </Field>
        </FieldGroup>
      </CardContent>
      <CardFooter>
        <Button className="w-full">Save Threshold</Button>
      </CardFooter>
    </Card>
  )
}
