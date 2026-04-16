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
    FieldContent,
    FieldDescription,
    FieldGroup,
    FieldLabel,
    FieldSeparator,
} from "ui-framework/field"
import {
    Select,
    SelectContent,
    SelectGroup,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "ui-framework/select"
import { Switch } from "ui-framework/switch"

const CURRENCIES = [
  { label: "USD — United States Dollar", value: "usd" },
  { label: "EUR — Euro", value: "eur" },
  { label: "GBP — British Pound", value: "gbp" },
  { label: "JPY — Japanese Yen", value: "jpy" },
]

export function Preferences() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Preferences</CardTitle>
        <CardDescription>
          Manage your account settings and notifications.
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
            <FieldLabel htmlFor="default-currency">Default Currency</FieldLabel>
            <Select items={CURRENCIES} defaultValue="usd">
              <SelectTrigger id="default-currency" className="w-full">
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
          <FieldSeparator className="-my-4" />
          <Field orientation="horizontal">
            <FieldContent>
              <FieldLabel htmlFor="public-statistics">
                Public Statistics
              </FieldLabel>
              <FieldDescription>
                Allow others to see your total stream count and listening
                activity
              </FieldDescription>
            </FieldContent>
            <Switch id="public-statistics" defaultChecked />
          </Field>
          <FieldSeparator className="-my-4" />
          <Field orientation="horizontal">
            <FieldContent>
              <FieldLabel htmlFor="email-notifications">
                Email Notifications
              </FieldLabel>
              <FieldDescription>
                Monthly royalty reports and distribution updates
              </FieldDescription>
            </FieldContent>
            <Switch id="email-notifications" defaultChecked />
          </Field>
        </FieldGroup>
      </CardContent>
      <CardFooter>
        <Button variant="outline">Reset</Button>
        <Button className="ml-auto">Save Preferences</Button>
      </CardFooter>
    </Card>
  )
}
