import {
  Checkbox,
  Field,
  FieldContent,
  FieldDescription,
  FieldGroup,
  FieldLabel,
  FieldTitle,
} from "ui-framework"

import { ShowcaseContainer } from "./showcase-container"

export function CheckboxShowcase() {
  return (
    <ShowcaseContainer
      title="Checkbox"
      description="Basic checkbox layouts using the field primitives for labels and helper text."
    >
      <FieldGroup>
        <Field orientation="horizontal">
          <Checkbox id="checkbox-basic" />
          <FieldLabel htmlFor="checkbox-basic">
            Accept terms and conditions
          </FieldLabel>
        </Field>

        <Field orientation="horizontal">
          <Checkbox id="checkbox-description" defaultChecked />
          <FieldContent>
            <FieldLabel htmlFor="checkbox-description">
              Enable notifications
            </FieldLabel>
            <FieldDescription>
              You can enable or disable notifications at any time.
            </FieldDescription>
          </FieldContent>
        </Field>

        <Field orientation="horizontal" data-disabled>
          <Checkbox id="checkbox-disabled" disabled />
          <FieldContent>
            <FieldTitle>Read-only setting</FieldTitle>
            <FieldDescription>
              This example shows the disabled state styling for the checkbox.
            </FieldDescription>
          </FieldContent>
        </Field>
      </FieldGroup>
    </ShowcaseContainer>
  )
}
