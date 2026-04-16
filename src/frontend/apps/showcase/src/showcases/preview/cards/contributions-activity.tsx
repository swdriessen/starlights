"use client"

import { Button } from "ui-framework/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "ui-framework/card"
import { Checkbox } from "ui-framework/checkbox"
import {
  Field,
  FieldContent,
  FieldDescription,
  FieldGroup,
  FieldLabel,
  FieldLegend,
  FieldSet,
} from "ui-framework/field"

export function ContributionsActivity() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Contributions & Activity</CardTitle>
        <CardDescription>
          Manage your contributions and activity visibility.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form id="contributions-activity">
          <FieldGroup>
            <FieldSet>
              <FieldLegend className="sr-only">
                Contributions & activity
              </FieldLegend>
              <FieldGroup>
                <Field orientation="horizontal">
                  <Checkbox id="activity-private-profile" />
                  <FieldContent>
                    <FieldLabel htmlFor="activity-private-profile">
                      Make profile private and hide activity
                    </FieldLabel>
                    <FieldDescription>
                      Enabling this will hide your contributions and activity
                      from your GitHub profile and from social features like
                      followers, stars, feeds, leaderboards and releases.
                    </FieldDescription>
                  </FieldContent>
                </Field>
              </FieldGroup>
            </FieldSet>
          </FieldGroup>
        </form>
      </CardContent>
      <CardFooter>
        <Button form="contributions-activity">Save Changes</Button>
      </CardFooter>
    </Card>
  )
}
