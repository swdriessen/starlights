import { CameraIcon, CirclePlusIcon, CloudIcon, GlobeIcon } from "lucide-react"

import { Button } from "ui-framework/button"
import {
    Card,
    CardContent,
    CardFooter,
    CardHeader,
    CardTitle,
} from "ui-framework/card"
import { Field, FieldGroup, FieldLabel } from "ui-framework/field"
import {
    InputGroup,
    InputGroupAddon,
    InputGroupInput,
} from "ui-framework/input-group"

export function SocialLinks() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Social Links</CardTitle>
      </CardHeader>
      <CardContent>
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="spotify-url">Spotify Artist URL</FieldLabel>
            <InputGroup>
              <InputGroupAddon>
                <CirclePlusIcon />
              </InputGroupAddon>
              <InputGroupInput
                id="spotify-url"
                defaultValue="spotify.com/artist/3j...2k"
              />
            </InputGroup>
          </Field>
          <Field>
            <FieldLabel htmlFor="instagram-handle">Instagram Handle</FieldLabel>
            <InputGroup>
              <InputGroupAddon>
                <CameraIcon />
              </InputGroupAddon>
              <InputGroupInput
                id="instagram-handle"
                defaultValue="@julianduryea_music"
              />
            </InputGroup>
          </Field>
          <Field>
            <FieldLabel htmlFor="soundcloud-url">SoundCloud URL</FieldLabel>
            <InputGroup>
              <InputGroupAddon>
                <CloudIcon />
              </InputGroupAddon>
              <InputGroupInput
                id="soundcloud-url"
                placeholder="soundcloud.com/username"
              />
            </InputGroup>
          </Field>
          <Field>
            <FieldLabel htmlFor="website-url">Website</FieldLabel>
            <InputGroup>
              <InputGroupAddon>
                <GlobeIcon />
              </InputGroupAddon>
              <InputGroupInput
                id="website-url"
                placeholder="https://yoursite.com"
              />
            </InputGroup>
          </Field>
        </FieldGroup>
      </CardContent>
      <CardFooter className="justify-end gap-2">
        <Button variant="secondary">Discard</Button>
        <Button>Save Changes</Button>
      </CardFooter>
    </Card>
  )
}
